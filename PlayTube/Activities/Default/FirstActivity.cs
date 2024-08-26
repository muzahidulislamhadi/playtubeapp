using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.Library.OneSignalNotif;
using PlayTube.SQLite;
using PlayTubeClient;

namespace PlayTube.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class FirstActivity : AppCompatActivity
    {
        #region Variables Basic

        private AppCompatButton LoginButton, RegisterButton;
        private TextView SkipButton;

        #endregion

        #region General
         
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);
                
                SetContentView(Resource.Layout.FirstPageLayout);

                LoginButton = FindViewById<AppCompatButton>(Resource.Id.LoginButton);
                RegisterButton = FindViewById<AppCompatButton>(Resource.Id.RegisterButton);
                SkipButton = FindViewById<TextView>(Resource.Id.SkipButton);

                if (!AppSettings.ShowSkipButton)
                    SkipButton.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowRegisterButton)
                    RegisterButton.Visibility = ViewStates.Gone;

                //OneSignal Notification  
                //====================================== 
                if (string.IsNullOrEmpty(UserDetails.DeviceId))
                    OneSignalNotification.Instance.RegisterNotificationDevice(this); 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {

                    RegisterButton.Click += RegisterButton_Click;
                    LoginButton.Click += LoginButton_Click;
                    SkipButton.Click += SkipButton_Click;
                }
                else
                {

                    RegisterButton.Click -= RegisterButton_Click;
                    LoginButton.Click -= LoginButton_Click;
                    SkipButton.Click -= SkipButton_Click;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event
         
        private void SkipButton_Click(object sender, EventArgs e)
        {
            try
            {  
                UserDetails.Username = "";
                UserDetails.FullName = "";
                UserDetails.Password = "";
                UserDetails.AccessToken = "";
                UserDetails.UserId = InitializePlayTube.UserId = "0";
                UserDetails.Status = "Pending";
                UserDetails.Cookie = "";
                UserDetails.Email = "";

                DataTables.LoginTb login = new DataTables.LoginTb
                {
                    Username = "",
                    Password = "",
                    AccessToken = "",
                    UserId = "0",
                    Status = "Pending",
                    Cookie = "",
                    Email = "",
                    Lang = ""
                };

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertOrUpdateLogin_Credentials(login);

                UserDetails.IsLogin = false;
                StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            { 
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            { 
                StartActivity(new Intent(this, typeof(LoginActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        #endregion

    }
}