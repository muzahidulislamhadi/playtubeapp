using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using AndroidX.AppCompat.App;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;

namespace PlayTube.Activities.Videos
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode | ConfigChanges.Locale)]
    public class FullScreenVideoActivity : AppCompatActivity
    {
        public VideoController VideoActionsController;
        private static VideoDataObject VideoData;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this, true);
                 
                var type = Intent?.GetStringExtra("type") ?? "";
                if (type == "RequestedOrientation")
                {
                    //ScreenOrientation.Portrait >>  Make to run your application only in portrait mode
                    //ScreenOrientation.Landscape >> Make to run your application only in LANDSCAPE mode 
                    RequestedOrientation = ScreenOrientation.Landscape;
                }

                SetContentView(Resource.Layout.FullScreenDialogLayout);

                VideoActionsController = new VideoController(this, "FullScreen"); 
                VideoActionsController.PlayFullScreen(VideoData);
               
                if (Intent?.GetStringExtra("Downloaded") == "Downloaded")
                    VideoActionsController.DownloadIcon.SetImageDrawable(GetDrawable(Resource.Drawable.ic_checked_red)); 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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
        
        public override void OnBackPressed()
        {
            VideoActionsController.InitFullscreenDialog("","Close");
            base.OnBackPressed();
        }

        public static void SetVideoData(VideoDataObject videoObject)
        {
            VideoData = videoObject;
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            { 
                if (newConfig.Orientation == Orientation.Landscape)
                {
                }
                else if (newConfig.Orientation == Orientation.Portrait)
                {
                    VideoActionsController.InitFullscreenDialog("","Close");
                }
                base.OnConfigurationChanged(newConfig);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


    }
}