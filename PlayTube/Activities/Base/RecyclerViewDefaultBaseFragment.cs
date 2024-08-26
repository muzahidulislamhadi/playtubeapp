using System;
using Android.Gms.Ads;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Com.Adcolony.Sdk;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Utils;

namespace PlayTube.Activities.Base
{
    public class RecyclerViewDefaultBaseFragment : Fragment
    {
        protected void ShowGoogleAds(View view, RecyclerView recyclerView)
        {
            try
            {
                var adView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(adView, recyclerView);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected void ShowFacebookAds(View view, RecyclerView recyclerView)
        {
            try
            { 
                var containerLayout = view.FindViewById<LinearLayout>(Resource.Id.bannerContainer);

                if (AppSettings.ShowFbBannerAds)
                    AdsFacebook.InitAdView(Activity, containerLayout, recyclerView);
                else
                    AdsColony.InitBannerAd(Activity, containerLayout, AdColonyAdSize.Banner, recyclerView); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}