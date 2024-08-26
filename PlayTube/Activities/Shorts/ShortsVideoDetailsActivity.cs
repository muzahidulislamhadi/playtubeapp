using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using Newtonsoft.Json;
using PlayTube.Activities.Shorts.Adapters;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls; 

namespace PlayTube.Activities.Shorts
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class ShortsVideoDetailsActivity : AppCompatActivity
    {
        #region Variables Basic
         
        private readonly string KeySelectedPage = "KEY_SELECTED_PAGE_Shorts";

        public ViewPager2 Pager;

        public ShortsVideoPagerAdapter MAdapter;
        private int SelectedPage, VideosCount;
        private string Type;
        private ObservableCollection<VideoDataObject> DataVideos;
        private static ShortsVideoDetailsActivity Instance;
        public string OpenEvent;
        public UserDataObject UserDataObjectOpenEvent;
        public PreCachingExoPlayerVideo PreCachingExoPlayerVideo;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this, true);

                Instance = this;
                if (savedInstanceState != null)
                {
                    SelectedPage = savedInstanceState.GetInt(KeySelectedPage);
                }

                // Create your application here
                SetContentView(Resource.Layout.ShortsVideoLayout);

                TabbedMainActivity.GetInstance()?.SetOnWakeLock();

                PreCachingExoPlayerVideo = new PreCachingExoPlayerVideo(this);

                if (Intent != null)
                {
                    Type = Intent?.GetStringExtra("Type") ?? "";
                    VideosCount = Intent?.GetIntExtra("VideosCount", 0) ?? 0;
                    SelectedPage = Intent?.GetIntExtra("IndexItem", 0) ?? 0;
                    DataVideos = Type == "VideoShorts" ? ListUtils.VideoShortsList : JsonConvert.DeserializeObject<ObservableCollection<VideoDataObject>>(Intent?.GetStringExtra("DataItem") ?? "");
                }

                //Get Value And Set Toolbar
                InitComponent();

                if (Type == "VideoShorts")
                    Task.Factory.StartNew(StartApiService);
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
                var instance = ViewShortsVideoFragment.GetInstance();
                instance?.StopVideo();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                var instance = ViewShortsVideoFragment.GetInstance();
                instance?.StopVideo();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            try
            {
                base.OnSaveInstanceState(outState);
                outState.PutInt(KeySelectedPage, Pager.CurrentItem);
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
                if (OpenEvent == "Profile")
                {
                    TabbedMainActivity globalContext = TabbedMainActivity.GetInstance();
                    globalContext.ShowUserChannelFragment(UserDataObjectOpenEvent, UserDataObjectOpenEvent.Id);
                }

                TabbedMainActivity.GetInstance()?.SetOffWakeLock();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

                var instance = ViewShortsVideoFragment.GetInstance();
                instance?.Destroy();
                PreCachingExoPlayerVideo?.Destroy();
                DestroyBasic();

                base.OnDestroy();
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
                Pager = FindViewById<ViewPager2>(Resource.Id.viewpager);

                MAdapter = new ShortsVideoPagerAdapter(this, VideosCount, DataVideos);

                //Pager.CurrentItem = MAdapter.ItemCount;
                //Pager.OffscreenPageLimit = 0;

                Pager.Orientation = ViewPager2.OrientationVertical;
                //Pager.SetPageTransformer(new CustomViewPageTransformer(TransformType.Flow));
                Pager.RegisterOnPageChangeCallback(new MyOnPageChangeCallback(this));
                Pager.Adapter = MAdapter;
                Pager.Adapter.NotifyDataSetChanged();

                Pager.SetCurrentItem(SelectedPage, false);
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
                Pager = null!; 
                MAdapter = null!;
                Instance = null!;
                SelectedPage = 0; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static ShortsVideoDetailsActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion
        
        private class MyOnPageChangeCallback : ViewPager2.OnPageChangeCallback
        {
            private readonly ShortsVideoDetailsActivity Activity;

            public MyOnPageChangeCallback(ShortsVideoDetailsActivity activity)
            {
                try
                {
                    Activity = activity;
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }
            private int LastPosition = -1;
            public override void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                try
                {
                    base.OnPageScrolled(position, positionOffset, positionOffsetPixels);

                    if (LastPosition == -1)
                    {
                        LastPosition = position;
                    }
                    else
                    {
                        LastPosition = position;
                        var instance = ViewShortsVideoFragment.GetInstance();
                        instance?.StopVideo();
                    }
                     
                    if (position > Activity.DataVideos.Count - 5 && Activity.Type == "VideoShorts")
                        Task.Factory.StartNew(Activity.StartApiService);
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }
        }

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { GetAllShortsVideo });
        }

        private bool ApiRun;
        private async Task GetAllShortsVideo()
        {
            try
            {
                if (!Methods.CheckConnectivity() || ApiRun)
                    return;

                ApiRun = true;

                var offset = ListUtils.VideoShortsViewsList.Aggregate("", (current, dataObject) => current + ("," + dataObject.Id)) ?? "";
                if (!string.IsNullOrEmpty(offset))
                {
                    offset = offset.Remove(0, 1);
                }

                var (apiStatus, respond) = await RequestsAsync.Video.GetShortsAsync(offset);
                if (apiStatus != 200 || respond is not GetVideosListDataObject result || result.VideoList == null)
                {
                    Methods.DisplayReportResult(this, respond);
                }
                else
                {
                    var respondList = result.VideoList?.Count;
                    if (respondList > 0)
                    {
                        result.VideoList = AppTools.ListFilter(result.VideoList);

                        foreach (var item in from item in result.VideoList let check = ListUtils.VideoShortsList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            var checkViewed = ListUtils.VideoShortsViewsList.FirstOrDefault(a => a.Id == item.Id);
                            if (checkViewed == null)
                            { 
                                ListUtils.VideoShortsList.Add(item);
                            }
                        }
                    }

                    RunOnUiThread(() =>
                    {
                        try
                        {
                            DataVideos = ListUtils.VideoShortsList;
                            MAdapter.UpdateShortsVideoPager(DataVideos.Count, DataVideos);
                            Pager.Adapter.NotifyDataSetChanged();
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });

                    ApiRun = false;
                }
            }
            catch (Exception e)
            {
                ApiRun = false;
                Methods.DisplayReportResultTrack(e);
            }
        }
         
    }
}