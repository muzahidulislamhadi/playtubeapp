using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Gms.Auth.Api.Credentials;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using Android.Views.InputMethods;
using Newtonsoft.Json;
using Org.Json;
using PlayTube.Activities.Base;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.SocialLogins;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Auth;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Task = System.Threading.Tasks.Task;

namespace PlayTube.Activities.Default
{
    [Activity]
    public abstract class SocialLoginBaseActivity : BaseActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback, IOnCompleteListener, IOnFailureListener 
    {
        #region Variables Basic

        private LinearLayout BntLoginWowonder, BntLoginGoogle, BntLoginFacebook;
        private TextView ContinueWithText;
        private FbMyProfileTracker MprofileTracker;
        private ICallbackManager MFbCallManager;
        public static GoogleSignInClient MGoogleSignInClient;
         
        #endregion

        #region General 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        public void InitSocialLogins()
        {
            try
            { 
                //#Facebook
                if (AppSettings.ShowFacebookLogin)
                {
                    LoginButton loginButton = new LoginButton(this);
                    MprofileTracker = new FbMyProfileTracker();
                    MprofileTracker.StartTracking();

                    BntLoginFacebook = FindViewById<LinearLayout>(Resource.Id.bntLoginFacebook);
                    BntLoginFacebook.Visibility = ViewStates.Visible;
                    BntLoginFacebook.Click += BntLoginFacebookOnClick;

                    MprofileTracker.MOnProfileChanged += MprofileTrackerOnMOnProfileChanged;
                    loginButton.SetPermissions(new List<string>
                    {
                        "email",
                        "public_profile"
                    });

                    MFbCallManager = CallbackManagerFactory.Create();
                    LoginManager.Instance.RegisterCallback(MFbCallManager, this);

                    //FB accessToken
                    var accessToken = AccessToken.CurrentAccessToken;
                    var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                    if (isLoggedIn && Profile.CurrentProfile != null)
                    {
                        LoginManager.Instance.LogOut();
                    }

                    string hashId = Methods.App.GetKeyHashesConfigured(this);
                    Console.WriteLine(hashId);
                }
                else
                {
                    BntLoginFacebook = FindViewById<LinearLayout>(Resource.Id.bntLoginFacebook);
                    BntLoginFacebook.Visibility = ViewStates.Gone;
                }

                //#Google
                if (AppSettings.ShowGoogleLogin)
                {
                    // Configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DEFAULT_SIGN_IN.
                    var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                        .RequestIdToken(AppSettings.ClientId)
                        .RequestScopes(new Scope(Scopes.Profile))
                        .RequestScopes(new Scope(Scopes.PlusMe))
                        .RequestScopes(new Scope(Scopes.DriveAppfolder))
                        .RequestServerAuthCode(AppSettings.ClientId)
                        .RequestProfile().RequestEmail().Build();

                    MGoogleSignInClient = GoogleSignIn.GetClient(this, gso);

                    BntLoginGoogle = FindViewById<LinearLayout>(Resource.Id.bntLoginGoogle);
                    BntLoginGoogle.Click += GoogleSignInButtonOnClick;
                }
                else
                {
                    BntLoginGoogle = FindViewById<LinearLayout>(Resource.Id.bntLoginGoogle);
                    BntLoginGoogle.Visibility = ViewStates.Gone;
                }

                //#WoWonder 
                if (AppSettings.ShowWoWonderLogin)
                {
                    BntLoginWowonder = FindViewById<LinearLayout>(Resource.Id.bntLoginWowonder);
                    var txtWowonder = FindViewById<TextView>(Resource.Id.txtWowonder);
                    BntLoginWowonder.Click += BntLoginWowonderOnClick;

                    txtWowonder.Text = GetString(Resource.String.Lbl_LoginWith) + " " + AppSettings.AppNameWoWonder;
                    BntLoginWowonder.Visibility = ViewStates.Visible;
                }
                else
                {
                    BntLoginWowonder = FindViewById<LinearLayout>(Resource.Id.bntLoginWowonder);
                    BntLoginWowonder.Visibility = ViewStates.Gone;
                }
                  
                ContinueWithText = FindViewById<TextView>(Resource.Id.ContinueWithText);
                if (!AppSettings.ShowFacebookLogin && AppSettings.ShowGoogleLogin && AppSettings.ShowWoWonderLogin)
                    ContinueWithText.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Event Click login using WoWonder
        protected void BntLoginWowonderOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(WoWonderLoginActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BntLoginFacebookOnClick(object sender, EventArgs e)
        {
            try
            {
                LoginManager.Instance.LogInWithReadPermissions(this, new List<string>
                {
                    "email",
                    "public_profile"
                });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Abstract members

        protected abstract void ToggleVisibility(bool isLoginProgress);

        #endregion
         
        #region Social Logins

        private string FbAccessToken, GAccessToken, GServerCode;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                ToggleVisibility(false);

                //SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {

                ToggleVisibility(false);

                // Handle exception
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.Message, GetText(Resource.String.Lbl_Ok));

                //SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            try
            {
                //var loginResult = result as LoginResult;
                //var id = AccessToken.CurrentAccessToken.UserId;

                ToggleVisibility(false);

               //SetResult(Result.Ok);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            { 
                ToggleVisibility(true);

                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FbAccessToken = accessToken.Token;

                    //Login Api 
                    var (apiStatus, respond) = await RequestsAsync.Auth.SocialLoginAsync(FbAccessToken, "facebook", UserDetails.DeviceId);
                    if (apiStatus == 200)
                    {
                        if (respond is LoginObject auth)
                        {
                            if (auth.data != null)
                            {
                                if (AppSettings.EnableSmartLockForPasswords && !string.IsNullOrEmpty(json?.ToString()))
                                {
                                    var data = json.ToString();
                                    var result = JsonConvert.DeserializeObject<FacebookResult>(data);

                                    // Save Google Sign In to SmartLock
                                    Credential credential = new Credential.Builder(result?.Email)
                                        .SetAccountType(IdentityProviders.Facebook)
                                        .SetName(result?.Name)
                                        //.SetPassword(auth.AccessToken)
                                        .Build();

                                    SaveCredential(credential);
                                }

                                SetDataLogin(auth, "");

                                UserDetails.IsLogin = true;

                                StartActivity(new Intent(this, typeof(TabbedMainActivity)));

                                ToggleVisibility(false);
                                Finish();
                            }
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            ToggleVisibility(false);
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                error.errors.ErrorText, GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        ToggleVisibility(false);
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                            respond.ToString(), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ToggleVisibility(false);
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MprofileTrackerOnMOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            try
            {
                if (e.MProfile != null)
                    try
                    {
                        //var FbFirstName = e.MProfile.FirstName;
                        //var FbLastName = e.MProfile.LastName;
                        //var FbName = e.MProfile.Name;
                        //var FbProfileId = e.MProfile.Id;

                        var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                        var parameters = new Bundle();
                        parameters.PutString("fields", "id,name,age_range,email");
                        request.Parameters = parameters;
                        request.ExecuteAndWait();
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Methods.DisplayReportResultTrack(ex);
                    }
                //else
                //    Toast.MakeText(this, GetString(Resource.String.Lbl_Null_Data_User), ToastLength.Short)?.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
        #endregion

        //======================================================

        #region Google

        //Event Click login using google
        private void GoogleSignInButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (MGoogleSignInClient == null)
                {
                    // Configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DEFAULT_SIGN_IN.
                    var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                        .RequestIdToken(AppSettings.ClientId)
                        .RequestScopes(new Scope(Scopes.Profile))
                        .RequestScopes(new Scope(Scopes.PlusMe))
                        .RequestScopes(new Scope(Scopes.DriveAppfolder))
                        .RequestServerAuthCode(AppSettings.ClientId)
                        .RequestProfile().RequestEmail().Build();

                    MGoogleSignInClient ??= GoogleSignIn.GetClient(this, gso);
                }

                var signInIntent = MGoogleSignInClient.SignInIntent;
                StartActivityForResult(signInIntent, 0);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public async void SetContentGoogle(GoogleSignInAccount acct)
        {
            try
            {
                //Successful log in hooray!!
                if (acct != null)
                {
                    ToggleVisibility(true);

                    //var GAccountName = acct.Account.Name;
                    //var GAccountType = acct.Account.Type;
                    //var GDisplayName = acct.DisplayName;
                    //var GFirstName = acct.GivenName;
                    //var GLastName = acct.FamilyName;
                    //var GProfileId = acct.Id;
                    //var GEmail = acct.Email;
                    //var GImg = acct.PhotoUrl.Path;
                    GAccessToken = acct.IdToken;
                    GServerCode = acct.ServerAuthCode;
                    Console.WriteLine(GServerCode);

                    if (!string.IsNullOrEmpty(GAccessToken))
                    {
                        var (apiStatus, respond) = await RequestsAsync.Auth.SocialLoginAsync(GAccessToken, "google", UserDetails.DeviceId);
                        if (apiStatus == 200)
                        {
                            if (respond is LoginObject auth)
                            {
                                if (auth.data != null)
                                {
                                    if (AppSettings.EnableSmartLockForPasswords)
                                    {
                                        // Save Google Sign In to SmartLock
                                        Credential credential = new Credential.Builder(acct.Email)
                                            .SetAccountType(IdentityProviders.Google)
                                            .SetName(acct.DisplayName)
                                            .SetProfilePictureUri(acct.PhotoUrl)
                                            .Build();

                                        SaveCredential(credential);
                                    }
                                    
                                    SetDataLogin(auth, "");

                                    UserDetails.IsLogin = true;

                                    StartActivity(new Intent(this, typeof(TabbedMainActivity)));

                                    ToggleVisibility(false);
                                    Finish();
                                }
                            }
                        }
                        else if (apiStatus == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                ToggleVisibility(false);
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.errors.ErrorText, GetText(Resource.String.Lbl_Ok));
                            }
                            else if (respond is ErrorsGoogleObject errorsGoogle)
                            {
                                ToggleVisibility(false);
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorsGoogle.Errors.ErrorText.Message, GetText(Resource.String.Lbl_Ok));
                            }
                        }
                        else
                        {
                            ToggleVisibility(false);
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        ToggleVisibility(false);
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //======================================================

        #region WoWonder

        public async void LoginWoWonder(string woWonderAccessToken)
        {
            try
            {
                ToggleVisibility(true);

                if (!string.IsNullOrEmpty(woWonderAccessToken))
                {
                    //Login Api 
                    (int apiStatus, var respond) = await RequestsAsync.Auth.SocialLoginAsync(woWonderAccessToken, "wowonder", UserDetails.DeviceId);
                    if (apiStatus == 200)
                    {
                        if (respond is LoginObject auth)
                        {
                            SetDataLogin(auth, "");
                            ToggleVisibility(false);

                            UserDetails.IsLogin = true;

                            StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                            FinishAffinity();
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.errors.ErrorText, GetText(Resource.String.Lbl_Ok));
                        }

                        ToggleVisibility(false);
                    }
                    else if (apiStatus == 404)
                    {
                        ToggleVisibility(false);
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ToggleVisibility(false);
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #endregion

        #region Result

        //Result
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                // Logins Facebook
                MFbCallManager?.OnActivityResult(requestCode, (int)resultCode, data);
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 0)
                {
                    var task = await GoogleSignIn.GetSignedInAccountFromIntentAsync(data);
                    SetContentGoogle(task);
                }
                else if (requestCode == RcCredentialsRead)
                {
                    if (resultCode == Result.Ok)
                    {
                        var extra = data.GetParcelableExtra(Credential.ExtraKey);
                        if (extra != null && extra is Credential credential)
                        {
                            HandleCredential(credential, OnlyPasswords);
                        }
                    }
                }
                else if (requestCode == RcCredentialsSave)
                {
                    MIsResolving = false;
                    if (resultCode == Result.Ok)
                    {
                        //Saved
                    }
                    else
                    {
                        //Credential save failed
                    }
                }
                else if (requestCode == RcCredentialsHint)
                {
                    MIsResolving = false;
                    if (resultCode == Result.Ok)
                    {
                        var extra = data.GetParcelableExtra(Credential.ExtraKey);
                        if (extra != null && extra is Credential credential)
                        {
                            OnlyPasswords = true;
                            HandleCredential(credential, OnlyPasswords);
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
         
        #region Cross App Authentication

        private static readonly int RcCredentialsSave = 1;
        private static readonly int RcCredentialsRead = 2;
        private static readonly int RcCredentialsHint = 3;

        private bool OnlyPasswords;
        public bool MIsResolving;
        private Credential MCredential;
        private string CredentialType;

        private CredentialsClient MCredentialsClient;
        private GoogleSignInClient MSignInClient;

        public void BuildClients(string accountName)
        {
            try
            {
                var gsoBuilder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(AppSettings.ClientId)
                    .RequestScopes(new Scope(Scopes.Profile))
                    .RequestScopes(new Scope(Scopes.PlusMe))
                    .RequestScopes(new Scope(Scopes.DriveAppfolder))
                    .RequestServerAuthCode(AppSettings.ClientId)
                    .RequestProfile().RequestEmail();

                if (accountName != null)
                    gsoBuilder.SetAccountName(accountName);
                
                MCredentialsClient = Credentials.GetClient(this, CredentialsOptions.Default);
                MSignInClient = GoogleSignIn.GetClient(this, gsoBuilder.Build());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void LoadHintClicked()
        {
            try
            {
                HintRequest hintRequest = new HintRequest.Builder()
                    .SetHintPickerConfig(new CredentialPickerConfig.Builder()
                        .SetShowCancelButton(true)
                        .Build())
                    .SetIdTokenRequested(false)
                    .SetEmailAddressIdentifierSupported(true)
                    .SetAccountTypes(IdentityProviders.Google)
                    .Build();

                PendingIntent intent = MCredentialsClient.GetHintPickerIntent(hintRequest);
                StartIntentSenderForResult(intent.IntentSender, RcCredentialsHint, null, 0, 0, 0);
                MIsResolving = true;
            }
            catch (Exception e)
            {
                //Could not start hint picker Intent
                MIsResolving = false;
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void RequestCredentials(bool onlyPasswords)
        {
            try
            {
                OnlyPasswords = onlyPasswords;

                CredentialRequest.Builder crBuilder = new CredentialRequest.Builder()
                    .SetPasswordLoginSupported(true);

                if (!onlyPasswords)
                {
                    crBuilder.SetAccountTypes(IdentityProviders.Google);
                }

                CredentialType = "Request";

                MCredentialsClient.Request(crBuilder.Build()).AddOnCompleteListener(this, this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void HandleCredential(Credential credential, bool onlyPasswords)
        {
            try
            {
                // See "Handle successful credential requests"  
                MCredential = credential;

                //Log.d(TAG, "handleCredential:" + credential.getAccountType() + ":" + credential.getId());
                if (IdentityProviders.Google.Equals(credential.AccountType))
                {
                    // Google account, rebuild GoogleApiClient to set account name and then try
                    BuildClients(credential.Id);
                    GoogleSilentSignIn();
                }
                else if (!string.IsNullOrEmpty(credential?.Id) && !string.IsNullOrEmpty(credential?.Password))
                {
                    // Email/password account
                    Console.WriteLine("Signed in as {0}", credential.Id);

                    //ContinueButton.Text = GetString(Resource.String.Lbl_ContinueAs) + " " + credential.Id;
                    //ContinueButton.Visibility = ViewStates.Visible;

                    if (onlyPasswords)
                    {
                        //send api auth  
                        HideKeyboard();

                        ToggleVisibility(true);

                        await LoginActivity.Instance.AuthApi(credential.Id, credential.Password);
                    }
                }
                else
                {
                    //ContinueButton.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ResolveResult(ResolvableApiException rae, int requestCode)
        {
            try
            {
                if (!MIsResolving)
                {
                    try
                    {
                        rae.StartResolutionForResult(this, requestCode);
                        MIsResolving = true;
                    }
                    catch (IntentSender.SendIntentException e)
                    {
                        MIsResolving = false;
                        //Failed to send Credentials intent
                        Methods.DisplayReportResultTrack(e);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SaveCredential(Credential credential)
        {
            try
            {
                if (credential == null)
                {
                    //Log.w(TAG, "Ignoring null credential.");
                    return;
                }

                CredentialType = "Save";
                MCredentialsClient.Save(credential).AddOnCompleteListener(this, this).AddOnFailureListener(this, this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void OnGoogleRevokeClicked()
        {
            if (MCredential != null)
            {
                await MCredentialsClient.DeleteAsync(MCredential);
            }
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            try
            {
                if (CredentialType == "Request")
                {
                    if (task.IsSuccessful && task.Result is CredentialRequestResponse credential)
                    {
                        // Auto sign-in success
                        HandleCredential(credential.Credential, OnlyPasswords);
                        return;
                    }
                }
                else if (CredentialType == "Save")
                {
                    if (task.IsSuccessful)
                    {
                        return;
                    }
                }

                var ee = task.Exception;
                if (ee is ResolvableApiException rae)
                {
                    // Getting credential needs to show some UI, start resolution 
                    if (CredentialType == "Request")
                        ResolveResult(rae, RcCredentialsRead);

                    else if (CredentialType == "Save")
                        ResolveResult(rae, RcCredentialsSave);
                }
                else
                {
                    Console.WriteLine("request: not handling exception {0}", ee);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {

        }

        private async void GoogleSilentSignIn()
        {
            try
            {
                // Try silent sign-in with Google Sign In API
                GoogleSignInAccount silentSignIn = await MSignInClient.SilentSignInAsync();
                if (silentSignIn != null)
                {
                    SetContentGoogle(silentSignIn);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
         
        protected void HideKeyboard()
        {
            try
            {
                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager?.HideSoftInputFromWindow(CurrentFocus?.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SetDataLogin(LoginObject auth, string password)
        {
            try
            {
                UserDetails.Username = "";
                UserDetails.FullName = "";
                UserDetails.Password = password;
                UserDetails.AccessToken = Current.AccessToken = auth.data.SessionId;
                UserDetails.UserId = InitializePlayTube.UserId = auth.data.UserId.ToString();
                UserDetails.Status = "Active";
                UserDetails.Cookie = auth.data.Cookie;
                UserDetails.Email = "";

                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId,
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = "",
                    Password = password,
                    Status = "Active",
                    Lang = "",
                    DeviceId = UserDetails.DeviceId,
                };

                ListUtils.DataUserLoginList.Clear();
                ListUtils.DataUserLoginList.Add(user);

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateLogin_Credentials(user);

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetChannelData(this, UserDetails.UserId) });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        private void SetDataLogin(AuthObject auth, string password)
        {
            try
            {
                UserDetails.Username = "";
                UserDetails.FullName = "";
                UserDetails.Password = password;
                UserDetails.AccessToken = Current.AccessToken = auth.AccessToken;
                UserDetails.UserId = InitializePlayTube.UserId = auth.UserId;
                UserDetails.Status = "Active";
                UserDetails.Cookie = auth.AccessToken;
                UserDetails.Email = "";

                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId,
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = "",
                    Password = password,
                    Status = "Active",
                    Lang = "",
                    DeviceId = UserDetails.DeviceId,
                };

                ListUtils.DataUserLoginList.Clear();
                ListUtils.DataUserLoginList.Add(user);

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateLogin_Credentials(user);

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetChannelData(this, UserDetails.UserId), () => ApiRequest.GetSettings_Api(this) });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}