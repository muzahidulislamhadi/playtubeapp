using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads.DoubleClick;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Auth;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
 
namespace PlayTube.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class ForgetPasswordActivity : AppCompatActivity
    {
        #region Variables Basic
        private PublisherAdView PublisherAdView;
        private AppCompatButton BtnSend;
        private EditText EmailEditext;
        private LinearLayout MainLinearLayout; 
        private ProgressBar ProgressBar;

        #endregion
         
        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                SetTheme(AppTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.ForgetPasswordLayout);

                //Get Value And Set Toolbar
                InitComponent();
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
                PublisherAdView?.Resume();
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
                PublisherAdView?.Pause();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
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
                PublisherAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                BtnSend = FindViewById<AppCompatButton>(Resource.Id.SendButton);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                MainLinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);

                PublisherAdView = FindViewById<PublisherAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitPublisherAdView(PublisherAdView);

                ProgressBar.Visibility = ViewStates.Invisible;
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
                    MainLinearLayout.Click += MainLinearLayoutOnClick;
                    BtnSend.Click += BtnSendOnClick;
                }
                else
                {
                    MainLinearLayout.Click -= MainLinearLayoutOnClick;
                    BtnSend.Click -= BtnSendOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private async void BtnSendOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailEditext.Text.Replace(" ", "")))
                {
                    if (Methods.CheckConnectivity())
                    {
                        var check = Methods.FunString.IsEmailValid(EmailEditext.Text);
                        if (!check)
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            ProgressBar.Visibility = ViewStates.Visible;
                            var (apiStatus, respond) = await RequestsAsync.Auth.ForgetPasswordAsync(EmailEditext.Text.Replace(" ", ""));
                            switch (apiStatus)
                            {
                                case 200:
                                {
                                    if (respond is ResetPasswordObject result)
                                    {
                                        Console.WriteLine(result.data);
                                        ProgressBar.Visibility = ViewStates.Invisible;
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationCorrect), GetText(Resource.String.Lbl_PasswordSent), GetText(Resource.String.Lbl_Ok));
                                    }

                                    break;
                                }
                                case 400:
                                {
                                    if (respond is ErrorObject error)
                                    {
                                        ProgressBar.Visibility = ViewStates.Invisible;
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_EmailNotFound), GetText(Resource.String.Lbl_Ok));
                                    }

                                    break;
                                }
                                default:
                                    Methods.DisplayReportResult(this, respond);
                                    break;
                            }
                            ProgressBar.Visibility = ViewStates.Invisible;
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Invisible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                ProgressBar.Visibility = ViewStates.Invisible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), exception.ToString(), GetText(Resource.String.Lbl_Ok));
            }
        }

        private void MainLinearLayoutOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager?.HideSoftInputFromWindow(CurrentFocus?.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

    }
}