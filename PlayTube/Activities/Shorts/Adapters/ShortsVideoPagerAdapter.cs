using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Runtime;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;
using Newtonsoft.Json;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace PlayTube.Activities.Shorts.Adapters
{
    public class ShortsVideoPagerAdapter : FragmentStateAdapter
    {
        private int CountVideo;
        private ObservableCollection<VideoDataObject> DataVideos;
         
        public ShortsVideoPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ShortsVideoPagerAdapter(Fragment fragment) : base(fragment)
        {
        }

        public ShortsVideoPagerAdapter(FragmentActivity fragmentActivity) : base(fragmentActivity)
        {
        }

        public ShortsVideoPagerAdapter(FragmentManager fragmentManager, Lifecycle lifecycle) : base(fragmentManager, lifecycle)
        {
        }

        public ShortsVideoPagerAdapter(FragmentActivity fragmentActivity, int size, ObservableCollection<VideoDataObject> dataVideos) : base(fragmentActivity)
        {
            try
            {
                CountVideo = size;
                DataVideos = dataVideos; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
        public void UpdateShortsVideoPager(int size, ObservableCollection<VideoDataObject> dataVideos) 
        {
            try
            {
                CountVideo = size;
                DataVideos = dataVideos; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
        public override int ItemCount => CountVideo;

        public override Fragment CreateFragment(int position)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutInt("position", position);

                VideoDataObject dataItem = DataVideos[position];
                if (dataItem != null)
                    bundle.PutString("DataItem", JsonConvert.SerializeObject(dataItem));

                ViewShortsVideoFragment viewShortsVideoFragment = new ViewShortsVideoFragment { Arguments = bundle };
                return viewShortsVideoFragment;
            }
            catch (Exception a)
            {
                Methods.DisplayReportResultTrack(a);
                return null!;
            }
        } 
    }
}