using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Auth.Api.Credentials;
using Android.Graphics;
using Android.OS;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Auth;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Task = System.Threading.Tasks.Task;

namespace PlayTube.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class LoginActivity : SocialLoginBaseActivity
    {
        #region Variables Basic

        private EditText TxtUsername, TxtPassword;
        private TextView TxtForgotPassword;
        private AppCompatButton BtnLogin;
        private LinearLayout UsernameLayout, PasswordLayout;
        private ImageView BackIcon, UsernameIcon, PasswordIcon, ImageShowPass;
        public static LoginActivity Instance;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetTheme(AppTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                Window?.SetSoftInputMode(SoftInput.AdjustResize);

                InitializePlayTube.Initialize(AppSettings.TripleDesAppServiceProvider, PackageName, AppSettings.TurnSecurityProtocolType3072On, AppSettings.SetApisReportMode);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.LoginLayout);

                Instance = this;

                //Get Value And Set Toolbar
                InitComponent();
                InitSocialLogins();

                if (AppSettings.EnableSmartLockForPasswords)
                    BuildClients(null);
                  
                if (!MIsResolving && AppSettings.EnableSmartLockForPasswords)
                {
                    RequestCredentials(false);
                    LoadHintClicked();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
          
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                DestroyBasic();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Menu 

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                BackIcon = FindViewById<ImageView>(Resource.Id.backArrow);
                BackIcon.SetImageResource(AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                BackIcon.ImageTintList = ColorStateList.ValueOf(AppTools.IsTabDark() ? Color.White : Color.Black);

                UsernameLayout = FindViewById<LinearLayout>(Resource.Id.UsernameLayout);
                TxtUsername = FindViewById<EditText>(Resource.Id.etUsername);
                UsernameIcon = FindViewById<ImageView>(Resource.Id.imageUsername);

                PasswordLayout = FindViewById<LinearLayout>(Resource.Id.PasswordLayout);
                TxtPassword = FindViewById<EditText>(Resource.Id.etPassword);
                PasswordIcon = FindViewById<ImageView>(Resource.Id.imagePass);
                 
                BtnLogin = FindViewById<AppCompatButton>(Resource.Id.bntLogin);
                TxtForgotPassword = FindViewById<TextView>(Resource.Id.textForgotPassword);
                ImageShowPass = FindViewById<ImageView>(Resource.Id.imageShowPass);

                Methods.SetColorEditText(TxtUsername, AppTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtPassword, AppTools.IsTabDark() ? Color.White : Color.Black);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BackIcon.Click += BackIconOnClick;
                    TxtUsername.FocusChange += TxtUsernameOnFocusChange;
                    TxtPassword.FocusChange += TxtPasswordOnFocusChange;
                    BtnLogin.Click += BtnLoginOnClick;
                    TxtForgotPassword.Click += TxtForgotPasswordOnClick;
                    ImageShowPass.Touch += ImageShowPassOnTouch;
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    TxtUsername.FocusChange -= TxtUsernameOnFocusChange;
                    TxtPassword.FocusChange -= TxtPasswordOnFocusChange;
                    BtnLogin.Click -= BtnLoginOnClick;
                    TxtForgotPassword.Click -= TxtForgotPasswordOnClick;
                    ImageShowPass.Touch -= ImageShowPassOnTouch;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void DestroyBasic()
        {
            try
            {
                TxtUsername = null!;
                TxtPassword = null!;
                TxtForgotPassword = null!;
                BtnLogin = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Show Password 
        private void ImageShowPassOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                switch (e.Event?.Action)
                {
                    case MotionEventActions.Up: // hide password
                        TxtPassword.TransformationMethod = PasswordTransformationMethod.Instance;
                        ImageShowPass.SetImageResource(Resource.Drawable.ic_eye_hide);
                        break;
                    case MotionEventActions.Down: // show password
                        TxtPassword.TransformationMethod = HideReturnsTransformationMethod.Instance;
                        ImageShowPass.SetImageResource(Resource.Drawable.icon_eye);
                        break;
                    default:
                        return;
                } 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Forgot Password
        private void TxtForgotPasswordOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(ForgetPasswordActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //start login 
        private async void BtnLoginOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                    return;
                }

                if (string.IsNullOrEmpty(TxtUsername.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtPassword.Text))
                {
                    ToggleVisibility(false);
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    return;
                }

                HideKeyboard();

                ToggleVisibility(true);

                await AuthApi(TxtUsername.Text.Replace(" ", ""), TxtPassword.Text);
            }
            catch (Exception exception)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public async Task AuthApi(string username, string password)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Auth.RequestLoginAsync(username, password, UserDetails.DeviceId);
                switch (apiStatus)
                {
                    case 200 when respond is LoginObject result:

                        if (AppSettings.EnableSmartLockForPasswords)
                        {
                            // Save Google Sign In to SmartLock
                            Credential credential = new Credential.Builder(username)
                                .SetName(username)
                                .SetPassword(password)
                                .Build();

                            SaveCredential(credential);
                        }
                           
                        SetDataLogin(result, TxtPassword.Text);

                        UserDetails.IsLogin = true;

                        StartActivity(new Intent(this, typeof(TabbedMainActivity)));

                        ToggleVisibility(false);
                        Finish();
                        break;
                    case 200:
                        {
                            if (respond is AuthMessageObject messageObject)
                            {
                                UserDetails.Username = TxtUsername.Text;
                                //UserDetails.FullName = MEditTextUsername.Text;
                                UserDetails.Password = TxtPassword.Text;
                                UserDetails.UserId = messageObject.UserId;
                                UserDetails.Status = "Pending";
                                UserDetails.Email = TxtUsername.Text;

                                //Insert user data to database
                                var user = new DataTables.LoginTb
                                {
                                    UserId = UserDetails.UserId,
                                    AccessToken = "",
                                    Cookie = "",
                                    Username = TxtUsername.Text,
                                    Password = TxtPassword.Text,
                                    Status = "Pending",
                                    Lang = "",
                                    DeviceId = UserDetails.DeviceId,
                                };
                                ListUtils.DataUserLoginList.Add(user);

                                //var dbDatabase = new SqLiteDatabase();
                                // dbDatabase.InsertOrUpdateLogin_Credentials(user);

                                StartActivity(new Intent(this, typeof(VerificationCodeActivity)));
                            }

                            break;
                        }
                    case 400:
                        {
                            if (respond is ErrorObject error)
                            {
                                ToggleVisibility(false);
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.errors.ErrorText, GetText(Resource.String.Lbl_Ok));
                            }

                            break;
                        }
                    default:
                        ToggleVisibility(false);
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                        break;
                }
            }
            catch (Exception exception)
            {
                ToggleVisibility(false);
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception);
            }
        }


        private void BackIconOnClick(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtUsernameOnFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                InitEditTextsIconsColor();
                SetHighLight(e.HasFocus, UsernameLayout, UsernameIcon);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        } 
          
        private void TxtPasswordOnFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                InitEditTextsIconsColor();
                SetHighLight(e.HasFocus, PasswordLayout, PasswordIcon);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        #endregion
         
        private void InitEditTextsIconsColor()
        {
            try
            { 
                if (TxtUsername.Text != "")
                    UsernameIcon.ImageTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, AppTools.IsTabDark() ? Color.White : Resource.Color.textDark_color)));

                if (TxtPassword.Text != "")
                    PasswordIcon.ImageTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, AppTools.IsTabDark() ? Color.White : Resource.Color.textDark_color)));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        private void SetHighLight(bool state, LinearLayout layout, ImageView icon)
        {
            try
            {
                if (state)
                {
                    layout.SetBackgroundResource(Resource.Drawable.new_editbox_active);
                    icon.ImageTintList = ColorStateList.ValueOf(Color.ParseColor(AppSettings.MainColor));
                }
                else
                {
                    layout.SetBackgroundResource(Resource.Drawable.new_login_status);
                    icon.ImageTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(this, Resource.Color.text_color_in_between)));
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void ToggleVisibility(bool isLoginProgress)
        {
            try
            {
                if (isLoginProgress)
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                else
                    AndHUD.Shared.Dismiss(this);

                BtnLogin.Visibility = isLoginProgress ? ViewStates.Invisible : ViewStates.Visible;
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
                UserDetails.Username = TxtUsername.Text;
                UserDetails.FullName = TxtUsername.Text;
                UserDetails.Password = password;
                UserDetails.AccessToken = Current.AccessToken = auth.data.SessionId;
                UserDetails.UserId = InitializePlayTube.UserId = auth.data.UserId.ToString();
                UserDetails.Status = "Active";
                UserDetails.Cookie = auth.data.Cookie;
                UserDetails.Email = TxtUsername.Text;

                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId,
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = TxtUsername.Text,
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