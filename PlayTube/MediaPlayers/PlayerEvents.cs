using System;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using Object = Java.Lang.Object;

namespace PlayTube.MediaPlayers
{
    public class PlayerEvents : Object, IPlayerEventListener, PlayerControlView.IVisibilityListener
    {
        private readonly Activity ActContext;
        private readonly ProgressBar LoadingProgressBar;
        private readonly ImageButton VideoPlayButton;
        private readonly ImageButton VideoResumeButton;
        private readonly VideoController VideoPlayerController;

        public PlayerEvents(VideoController videoController, Activity act, PlayerControlView controlView)
        {
            try
            {
                VideoPlayerController = videoController;
                ActContext = act;

                if (controlView != null)
                {
                    VideoPlayButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_play);
                    VideoResumeButton = controlView.FindViewById<ImageButton>(Resource.Id.exo_pause);
                    LoadingProgressBar = act.FindViewById<ProgressBar>(Resource.Id.progress_bar);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnIsPlayingChanged(bool isPlaying)
        {
             
        }

        public void OnLoadingChanged(bool p0)
        {

        }

        public void OnPlaybackParametersChanged(PlaybackParameters p0)
        {

        }

        public void OnPlaybackSuppressionReasonChanged(int playbackSuppressionReason)
        {
            
        }

        public void OnPlayerError(ExoPlaybackException p0)
        {

        }

        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            try
            {
                if (playbackState == IPlayer.StateEnded || playbackState == IPlayer.StateIdle && !playWhenReady)
                {
                    TabbedMainActivity.GetInstance().SetOffWakeLock();
                    VideoPlayerController.ToggleExoPlayerKeepScreenOnFeature(false);
                    OnVideoEnded(); 
                } 
                else
                { // STATE_IDLE, STATE_ENDED
                  // This prevents the screen from getting dim/lock
                    TabbedMainActivity.GetInstance().SetOnWakeLock();
                    VideoPlayerController.ToggleExoPlayerKeepScreenOnFeature(true);
                }

                if (VideoResumeButton == null || VideoPlayButton == null || LoadingProgressBar == null)
                    return;

                switch (playbackState)
                {
                    case IPlayer.StateEnded when VideoPlayerController.ShowRestrictedVideo:
                    {
                        var videoData = VideoPlayerController.VideoData;
                        if (!string.IsNullOrEmpty(videoData.SellVideo) && videoData.SellVideo != "0")
                            VideoPlayerController.ShowRestrictedVideoFragment(null, ActContext, "purchaseVideo");
                        else if (!string.IsNullOrEmpty(videoData.RentPrice) && videoData.RentPrice != "0" && AppSettings.RentVideosSystem)
                            VideoPlayerController.ShowRestrictedVideoFragment(null, ActContext, "RentVideo");

                        VideoPlayerController.ShowRestrictedVideo = false;
                        return;
                    }
                    case IPlayer.StateEnded when playWhenReady == false:
                        VideoResumeButton.Visibility = ViewStates.Visible;
                        break;
                    case IPlayer.StateEnded:
                        VideoResumeButton.Visibility = ViewStates.Gone;
                        VideoPlayButton.Visibility = ViewStates.Visible;
                        break;
                    case IPlayer.StateReady:
                    {
                        //Allen On Next Update

                        //if (VideoController.Player.VideoFormat.Height > 650)
                        //    VideoController.Player.VideoFormat.Height = (int)(VideoController.Player.VideoFormat.Height / 1.8);

                        //float videoRatio = (float)VideoController.Player.VideoFormat.Width / VideoController.Player.VideoFormat.Height ;

                        //var display = ActContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay;
                        //var size = new Point();
                        //display.GetSize(size);

                        //float displayRatio = (float)size.X / size.Y;

                        //if (videoRatio > displayRatio)
                        //{
                        //    VideoPlayerController.MainVideoFrameLayout.LayoutParameters.Height = (int)Math.Round(VideoPlayerController.MainVideoFrameLayout.MeasuredWidth / videoRatio);
                        //    VideoPlayerController.MainVideoFrameLayout.RequestLayout();
                        //}
                        //else
                        //{
                        //    LinearLayout.LayoutParams Params = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 220);
                        //    Params.Gravity = GravityFlags.Center;
                        //    VideoPlayerController.MainVideoFrameLayout.LayoutParameters = Params;
                        //}

                        if (playWhenReady == false)
                        {
                            VideoResumeButton.Visibility = ViewStates.Gone;
                            VideoPlayButton.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            VideoResumeButton.Visibility = ViewStates.Visible;
                        }

                        LoadingProgressBar.Visibility = ViewStates.Invisible;

                        bool canNext = ListUtils.ArrayListPlay.Count > 0;
                        VideoPlayerController.BtnNext.Enabled = canNext;
                        VideoPlayerController.BtnNext.Alpha = canNext ? 1f : 0.3f;

                        break;
                    }
                    case IPlayer.StateBuffering:
                        LoadingProgressBar.Visibility = ViewStates.Visible;
                        VideoResumeButton.Visibility = ViewStates.Invisible;
                        break;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        public void OnPositionDiscontinuity(int p0)
        {
            try
            {
                // Load metadata for mediaId and update the UI.
                var tag = VideoPlayerController.SimpleExoPlayerView.Player.CurrentTag ?? "";
                switch (tag?.ToString())
                {
                    case "Ads":
                        break;
                    case "normal":
                        VideoPlayerController.ExoTopLayout.Visibility = ViewStates.Visible;
                        VideoPlayerController.ExoEventButton.Visibility = ViewStates.Visible;
                        VideoPlayerController.BtnSkipIntro.Visibility = ViewStates.Gone;
                        VideoPlayerController.ExoTopAds.Visibility = ViewStates.Gone;
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnRepeatModeChanged(int p0)
        {

        }

        public void OnSeekProcessed()
        {

        }

        public void OnShuffleModeEnabledChanged(bool p0)
        {

        }

        public void OnTimelineChanged(Timeline timeline, int reason)
        {
            try
            {
                var tag = VideoPlayerController.SimpleExoPlayerView.Player.CurrentTag ?? "";
                if (reason == 0 || tag?.ToString() == "normal")
                {
                    //Methods.DisplayReportResult(ActContext, "OnTimelineChanged >> normal");
                    VideoPlayerController.ExoTopLayout.Visibility = ViewStates.Visible;
                    VideoPlayerController.ExoEventButton.Visibility = ViewStates.Visible;
                    VideoPlayerController.BtnSkipIntro.Visibility = ViewStates.Gone;
                    VideoPlayerController.ExoTopAds.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1)
        {

        }

        public void OnVisibilityChange(int p0)
        {

        }

        public void OnVideoEnded()
        {
            try
            {
                if (ListUtils.ArrayListPlay.Count > 0 && UserDetails.AutoNext)
                {
                    var data = ListUtils.ArrayListPlay.FirstOrDefault();
                    if (data != null)
                    {
                        ListUtils.LessonList.Add(data);
                        TabbedMainActivity.GetInstance()?.StartPlayVideo(data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}