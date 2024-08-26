using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using MaterialDialogsCore; 
using PlayTube.Activities.Comments;
using PlayTube.Activities.Models;
using PlayTube.Helpers.Controller;
using PlayTubeClient.Classes.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using AndroidX.AppCompat.Widget;
using PlayTube.Library.Anjo.SuperTextLibrary;
using Newtonsoft.Json;
using PlayTube.Activities.SettingsPreferences;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.MediaPlayers;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using Google.Android.Material.AppBar;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using PlayTube.Activities.SettingsPreferences.General;
using YouTubePlayerAndroidX.Player;
using BaseActivity = PlayTube.Activities.Base.BaseActivity;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using Fragment = AndroidX.Fragment.App.Fragment;
using Exception = System.Exception;
using Math = System.Math;

namespace PlayTube.Activities.PlayersView
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode , LaunchMode = LaunchMode.SingleInstance ,ResizeableActivity = true, SupportsPictureInPicture = true)]
    public class GlobalPlayerActivity : BaseActivity, AppBarLayout.IOnOffsetChangedListener, IYouTubePlayerInitListener, IYouTubePlayerFullScreenListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, View.IOnClickListener
    {
        #region Variables Basic

        private TextView ShareIconView, AddToIconView, TextChannelName, EditIconView , TxtInfo;
        private ImageView ImageChannelView, LikeIconView, UnLikeIconView, ViewMoreCommentSection;
        private AppCompatButton SubscribeChannelButton;
        private LinearLayout VideoDescriptionLayout, LikeButton, UnLikeButton, ShareButton, AddToButton, EditButton;
        private TextView VideoTitle, VideoLikeCount, VideoUnLikeCount, VideoChannelViews, VideoPublishDate, VideoCategory, UpNextTextview, CountComments;
        private SuperTextView VideoDescription;
        private TextSanitizer TextSanitizerAutoLink;
        public VideoDataObject VideoDataHandler; 
        private VideoDataWithEventsLoader.VideoEnumTypes VideoType;
        private LinearLayout DonateButton, RentButton;
        private string TypeDialog;
        private Switch AutoNextswitch;
        private FrameLayout VideoButtomLayout, ShowMoreDescriptionIconView;
        private RelativeLayout CommentButtomLayout;
         
        public CommentsFragment CommentsFragment;
        private NextToFragment NextToFragment;
        public IYouTubePlayer YoutubePlayer { get; private set; }
        public LibrarySynchronizer LibrarySynchronizer;
        public VideoController VideoActionsController;
        private AppBarLayout AppBarLayoutView;
        private CoordinatorLayout CoordinatorLayoutView;
        public YouTubePlayerView TubePlayerView;
        public RestrictedVideoFragment RestrictedVideoPlayerFragment;
        private ThirdPartyPlayersFragment ThirdPartyPlayersFragment;
        private string VideoIdYoutube; 
       
        private static GlobalPlayerActivity Instance;
        private bool OnStopCalled;
        public static bool OnOpenPage;
         
        private FrameLayout MainVideoRoot;

        public YouTubePlayerEvents YouTubePlayerEvents;
        private string TypeYouTubePlayerFullScreen = "RequestedOrientation";

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
                SetContentView(Resource.Layout.VideoSliderLayout);

                Instance = this;
                OnStopCalled = false;
                OnOpenPage = true;

                SetVideoPlayerFragmentAdapters();

                var videoObject = Intent?.GetStringExtra("VideoObject");
                if (!string.IsNullOrEmpty(videoObject))
                    VideoDataHandler = JsonConvert.DeserializeObject<VideoDataObject>(videoObject);

                InitComponent();
                  
                if (VideoDataHandler == null)
                    return;
                
                LoadVideoData(VideoDataHandler); 
                StartPlayVideo(VideoDataHandler);
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

        protected override void OnStop()
        {
            try
            {
                OnStopCalled = true;
                base.OnStop();
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
                VideoActionsController?.ReleaseVideo();

                TubePlayerView?.Release();

                OnOpenPage = false;
                
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                base.OnNewIntent(intent);

                //Called onNewIntent
                VideoActionsController.SetStopvideo();

                if (YouTubePlayerEvents != null && YoutubePlayer != null && YouTubePlayerEvents.IsPlaying)
                    YoutubePlayer?.Pause();

                var videoObject = Intent?.GetStringExtra("VideoObject");
                if (!string.IsNullOrEmpty(videoObject))
                    VideoDataHandler = JsonConvert.DeserializeObject<VideoDataObject>(videoObject);

                InitComponent();

                if (VideoDataHandler == null)
                    return;

                LoadVideoData(VideoDataHandler);
                StartPlayVideo(VideoDataHandler);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Back Pressed
          
        private bool IsPipModeEnabled = true;
        public override void OnBackPressed()
        {
            try
            { 
                if (TubePlayerView.FullScreen)
                {
                    TubePlayerView?.ExitFullScreen(); 
                    return;
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N && PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture) && IsPipModeEnabled)
                {
                    switch (VideoType)
                    {
                        case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                            EnterPipMode();
                            break;
                        case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                            EnterPipMode();
                            //base.OnBackPressed();
                            break;
                    }
                }
                else
                {
                    base.OnBackPressed();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                base.OnBackPressed();
            }
        }

        private void EnterPipMode()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N && PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture))
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        Rational rational = new Rational(450, 250);
                        PictureInPictureParams.Builder builder = new PictureInPictureParams.Builder();
                        builder.SetAspectRatio(rational);
                        EnterPictureInPictureMode(builder.Build());
                    }
                    else
                    {
                        var param = new PictureInPictureParams.Builder().Build();
                        EnterPictureInPictureMode(param);
                    }

                    new Handler(Looper.MainLooper).PostDelayed(CheckPipPermission, 30);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void CheckPipPermission()
        {
            IsPipModeEnabled = IsInPictureInPictureMode;
            if (!IsInPictureInPictureMode)
            {
                OnBackPressed();
            }
        }

        private void BackIcon_Click(object sender, EventArgs e)
        {
            try
            {
                switch (VideoType)
                {
                    case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            var param = new PictureInPictureParams.Builder().Build();
                            EnterPictureInPictureMode(param);
                        }
                        break;
                    case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                        FinishActivityAndTask();
                        break;
                }
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
                MainVideoRoot = FindViewById<FrameLayout>(Resource.Id.Mainroot);

                CoordinatorLayoutView = FindViewById<CoordinatorLayout>(Resource.Id.parent);
                AppBarLayoutView = FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
                AppBarLayoutView.AddOnOffsetChangedListener(this);

                LikeIconView = FindViewById<ImageView>(Resource.Id.Likeicon);
                UnLikeIconView = FindViewById<ImageView>(Resource.Id.UnLikeicon);
                ShareIconView = FindViewById<TextView>(Resource.Id.Shareicon);
                AddToIconView = FindViewById<TextView>(Resource.Id.AddToicon);
                EditIconView = FindViewById<TextView>(Resource.Id.editIcon);

                ShowMoreDescriptionIconView = FindViewById<FrameLayout>(Resource.Id.video_ShowDiscription);
                VideoDescriptionLayout = FindViewById<LinearLayout>(Resource.Id.videoDescriptionLayout);
                ImageChannelView = FindViewById<ImageView>(Resource.Id.Image_Channel);
                TextChannelName = FindViewById<TextView>(Resource.Id.ChannelName);
                SubscribeChannelButton = FindViewById<AppCompatButton>(Resource.Id.SubcribeButton);
                
                LikeButton = FindViewById<LinearLayout>(Resource.Id.LikeButton);
                UnLikeButton = FindViewById<LinearLayout>(Resource.Id.UnLikeButton);
                ShareButton = FindViewById<LinearLayout>(Resource.Id.ShareButton);
                AddToButton = FindViewById<LinearLayout>(Resource.Id.AddToButton);
                EditButton = FindViewById<LinearLayout>(Resource.Id.editButton);

                LikeButton.Tag = "0";
                UnLikeButton.Tag = "0";

                RentButton = FindViewById<LinearLayout>(Resource.Id.RentButton);
                DonateButton = FindViewById<LinearLayout>(Resource.Id.DonateButton);

                RentButton.Visibility = ViewStates.Gone;
                DonateButton.Visibility = ViewStates.Gone;

                VideoTitle = FindViewById<TextView>(Resource.Id.video_Titile);
                TxtInfo = FindViewById<TextView>(Resource.Id.info);

                VideoLikeCount = FindViewById<TextView>(Resource.Id.LikeNumber);
                VideoUnLikeCount = FindViewById<TextView>(Resource.Id.UnLikeNumber);
                VideoChannelViews = FindViewById<TextView>(Resource.Id.Channelviews);
                VideoPublishDate = FindViewById<TextView>(Resource.Id.videoDate);
                VideoDescription = FindViewById<SuperTextView>(Resource.Id.videoDescriptionTextview);
                CountComments = FindViewById<TextView>(Resource.Id.countComments);
              
                VideoCategory = FindViewById<TextView>(Resource.Id.videoCategorytextview);

                VideoButtomLayout = FindViewById<FrameLayout>(Resource.Id.videoButtomLayout);
                CommentButtomLayout = FindViewById<RelativeLayout>(Resource.Id.CommentButtomLayout);
                UpNextTextview = FindViewById<TextView>(Resource.Id.UpNextTextview);
                ViewMoreCommentSection = FindViewById<ImageView>(Resource.Id.viewMoreCommentsection);
                AutoNextswitch = FindViewById<Switch>(Resource.Id.AutoNextswitch);
                AutoNextswitch.Checked = UserDetails.AutoNext;
                 
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShareIconView, IonIconsFonts.ShareAlt);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, AddToIconView, IonIconsFonts.AddCircle);
                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDown);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EditIconView, IonIconsFonts.Create);

                TextSanitizerAutoLink = new TextSanitizer(VideoDescription, this);

                VideoActionsController = new VideoController(this, "GlobalPlayer");
                VideoActionsController.ExoBackButton.Click += BackIcon_Click;

                LibrarySynchronizer = new LibrarySynchronizer(this); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
        public static GlobalPlayerActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    LikeButton.Click += LikeButtonOnClick;
                    UnLikeButton.Click += UnLikeButtonOnClick;
                    ShareButton.Click += ShareButtonOnClick;
                    AddToButton.Click += AddToButtonOnClick;
                    SubscribeChannelButton.Click += SubscribeChannelButtonOnClick;
                    TextChannelName.Click += ImageChannelViewOnClick;
                    VideoCategory.Click += VideoCategoryOnClick;
                    ImageChannelView.Click += ImageChannelViewOnClick;
                    ShowMoreDescriptionIconView.Click += ShowMoreDescriptionIconViewOnClick;
                    EditButton.Click += EditButtonOnClick;
                    RentButton.Click += RentButtonOnClick;
                    DonateButton.Click += DonateButtonOnClick;
                    CommentButtomLayout.Click += CommentButtomLayout_Click;
                    ViewMoreCommentSection.Click += ViewMoreCommentSectionClick;
                    UpNextTextview.Click += ViewMoreCommentSectionClick;
                    AutoNextswitch.CheckedChange += AutoNextswitchOnCheckedChange;
                }
                else
                {
                    LikeButton.Click -= LikeButtonOnClick;
                    UnLikeButton.Click -= UnLikeButtonOnClick;
                    ShareButton.Click -= ShareButtonOnClick;
                    AddToButton.Click -= AddToButtonOnClick;
                    SubscribeChannelButton.Click -= SubscribeChannelButtonOnClick;
                    TextChannelName.Click -= ImageChannelViewOnClick;
                    VideoCategory.Click -= VideoCategoryOnClick;
                    ImageChannelView.Click -= ImageChannelViewOnClick;
                    ShowMoreDescriptionIconView.Click -= ShowMoreDescriptionIconViewOnClick;
                    EditButton.Click -= EditButtonOnClick;
                    RentButton.Click -= RentButtonOnClick;
                    DonateButton.Click -= DonateButtonOnClick;
                    CommentButtomLayout.Click -= CommentButtomLayout_Click;
                    ViewMoreCommentSection.Click -= ViewMoreCommentSectionClick;
                    UpNextTextview.Click -= ViewMoreCommentSectionClick;
                    AutoNextswitch.CheckedChange -= AutoNextswitchOnCheckedChange;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events
        
        private void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent();
                intent.PutExtra("Open", "EditVideo");
                intent.PutExtra("ItemDataVideo", JsonConvert.SerializeObject(VideoDataHandler));
                SetResult(Result.Ok, intent);

                switch (VideoType)
                {
                    case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                        VideoActionsController.SetStopvideo();
                        break;
                    case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                        YoutubePlayer.Pause();
                        break;
                }

                FinishActivityAndTask();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void AutoNextswitchOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                UserDetails.AutoNext = AutoNextswitch.Checked;
                MainSettings.AutoNext?.Edit()?.PutBoolean(MainSettings.PrefKeyAutoNext, UserDetails.AutoNext)?.Commit();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void CommentButtomLayout_Click(object sender, EventArgs e)
        {
            try
            {
                UpNextTextview.Text = GetString(Resource.String.Lbl_Comments);
                UpNextTextview.Tag = GetString(Resource.String.Lbl_Comments);
                ViewMoreCommentSection.Visibility = ViewStates.Visible;
                AutoNextswitch.Visibility = ViewStates.Gone;
                CommentButtomLayout.Visibility = ViewStates.Gone;
                FragmentTransaction ftvideo = SupportFragmentManager.BeginTransaction();

                if (!CommentsFragment.IsAdded)
                {
                    ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                    ftvideo.AddToBackStack(null);
                    ftvideo.Add(VideoButtomLayout.Id, CommentsFragment, null)?.Commit();
                }
                else
                {
                    ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                    ftvideo.Hide(NextToFragment).Show(CommentsFragment)?.Commit();
                }

                CommentsFragment.StartApiService(VideoDataHandler.Id, "0");
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ViewMoreCommentSectionClick(object sender, EventArgs e)
        {
            try
            {
                HideCommentsAndShowNextTo();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void HideCommentsAndShowNextTo()
        {
            try
            {
                UpNextTextview.Text = GetString(Resource.String.Lbl_NextTo);
                UpNextTextview.Tag = GetString(Resource.String.Lbl_NextTo);
                ViewMoreCommentSection.Visibility = ViewStates.Gone;
                AutoNextswitch.Visibility = ViewStates.Visible;
                CommentButtomLayout.Visibility = ViewStates.Visible;
                FragmentTransaction ftvideo = SupportFragmentManager.BeginTransaction();
                ftvideo.AddToBackStack(null);
                ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                ftvideo.Hide(CommentsFragment).Show(NextToFragment)?.Commit();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
        
        private void AddToButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "AddTo";

                var dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                var arrayAdapter = new List<string> { GetString(Resource.String.Lbl_Addto_playlist) };

                var check = ListUtils.WatchLaterVideosList.FirstOrDefault(q => q.Videos?.VideoAdClass.Id == VideoDataHandler.Id);
                arrayAdapter.Add(check == null ? GetString(Resource.String.Lbl_Addto_watchlater) : GetString(Resource.String.Lbl_RemoveFromWatchLater));

                dialogList.Title(GetString(Resource.String.Lbl_Add_To));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetString(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ShareButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                LibrarySynchronizer?.ShareVideo(VideoDataHandler);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void UnLikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (UnLikeButton.Tag?.ToString() == "0")
                        {
                            UnLikeButton.Tag = "1";
                            UnLikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                            LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));

                            if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                            {
                                var x = Convert.ToDouble(VideoUnLikeCount.Text);
                                x++;
                                VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                            }

                            if (LikeButton.Tag?.ToString() == "1")
                            {
                                LikeButton.Tag = "0";
                                if (!VideoLikeCount.Text.Contains("K") && !VideoLikeCount.Text.Contains("M"))
                                {
                                    var x = Convert.ToDouble(VideoLikeCount.Text);
                                    if (x > 0)
                                    {
                                        x--;
                                    }
                                    else
                                    {
                                        x = 0;
                                    }

                                    VideoLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                                }
                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Video_UnLiked), ToastLength.Short)?.Show();

                            //Send API Request here for dislike
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.AddLikeDislikeVideosAsync(VideoDataHandler.Id, "dislike") });
                        }
                        else
                        {
                            UnLikeButton.Tag = "0";


                            UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                            var x = Convert.ToDouble(VideoUnLikeCount.Text);
                            if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                            {
                                if (x > 0)
                                {
                                    x--;
                                }
                                else
                                {
                                    x = 0;
                                }
                                VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                            }

                            //Send API Request here for Remove UNLike
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.AddLikeDislikeVideosAsync(VideoDataHandler.Id, "dislike") });

                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(this, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_sign_in_Dislike), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void LikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        try
                        {
                            //If User Liked
                            if (LikeButton.Tag?.ToString() == "0")
                            {
                                LikeButton.Tag = "1";
                                LikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));


                                UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                                if (!VideoLikeCount.Text.Contains("K") && !VideoLikeCount.Text.Contains("M"))
                                {
                                    var x = Convert.ToDouble(VideoLikeCount.Text);
                                    x++;
                                    VideoLikeCount.Text = x.ToString(CultureInfo.InvariantCulture);
                                }

                                if (UnLikeButton.Tag?.ToString() == "1")
                                {
                                    UnLikeButton.Tag = "0";
                                    if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                                    {
                                        var x = Convert.ToDouble(VideoUnLikeCount.Text);
                                        if (x > 0)
                                        {
                                            x--;
                                        }
                                        else
                                        {
                                            x = 0;
                                        }
                                        VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                                    }
                                }


                                //AddToLiked
                                LibrarySynchronizer.AddToLiked(VideoDataHandler);

                                Toast.MakeText(this, GetText(Resource.String.Lbl_Video_Liked), ToastLength.Short)?.Show();

                                //Send API Request here for Like
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.AddLikeDislikeVideosAsync(VideoDataHandler.Id, "like") });

                            }
                            else
                            {
                                LikeButton.Tag = "0";

                                LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                                UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                                if (!VideoLikeCount.Text.Contains("K") && !VideoLikeCount.Text.Contains("M"))
                                {
                                    var x = Convert.ToDouble(VideoLikeCount.Text);
                                    if (x > 0)
                                    {
                                        x--;
                                    }
                                    else
                                    {
                                        x = 0;
                                    }

                                    VideoLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                                }

                                Toast.MakeText(this, GetText(Resource.String.Lbl_Remove_Video_Liked), ToastLength.Short)?.Show();

                                //Send API Request here for Remove UNLike
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.AddLikeDislikeVideosAsync(VideoDataHandler.Id, "like") });
                            }
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(this, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_sign_in_Like), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void VideoCategoryOnClick(object sender, EventArgs e)
        {
            try
            {

                Intent intent = new Intent();
                intent.PutExtra("Open", "VideosByCategory");
                intent.PutExtra("CatId", VideoDataHandler.CategoryId);
                intent.PutExtra("CatName", VideoDataHandler.CategoryName);
                SetResult(Result.Ok, intent);

                switch (VideoType)
                {
                    case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                        VideoActionsController.SetStopvideo();
                        break;
                    case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                        YoutubePlayer.Pause();
                        break;
                }
                FinishActivityAndTask();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SubscribeChannelButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (!string.IsNullOrEmpty(VideoDataHandler.Owner?.OwnerClass?.SubscriberPrice) && VideoDataHandler.Owner?.OwnerClass?.SubscriberPrice != "0")
                        {
                            if (SubscribeChannelButton.Tag?.ToString() == "PaidSubscribe")
                            { 
                                //This channel is paid, You must pay to subscribe
                                var dialog = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                                dialog.Title(Resource.String.Lbl_PurchaseRequired);
                                dialog.Content(GetText(Resource.String.Lbl_ChannelIsPaid));
                                dialog.PositiveText(GetText(Resource.String.Lbl_Purchase)).OnPositive(async (materialDialog, action) =>
                                {
                                    try
                                    {
                                        if (AppTools.CheckWallet())
                                        {
                                            if (Methods.CheckConnectivity())
                                            {
                                                var (apiStatus, respond) = await RequestsAsync.Payment.PurchaseAsync("subscribe", VideoDataHandler.Owner?.OwnerClass.Id);
                                                if (apiStatus == 200)
                                                {
                                                    if (respond is MessageObject result)
                                                    {
                                                        Console.WriteLine(result.Message);

                                                        Toast.MakeText(this, GetText(Resource.String.Lbl_PurchasedSuccessfully), ToastLength.Long)?.Show();
                                                        SetSubscribeChannelWithPaid();
                                                    }
                                                }
                                                else Methods.DisplayReportResult(this, respond);
                                            }
                                            else
                                                Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                                        }
                                        else
                                        {
                                            var dialogBuilder = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                                            dialogBuilder.Title(GetText(Resource.String.Lbl_Wallet));
                                            dialogBuilder.Content(GetText(Resource.String.Lbl_Error_NoWallet));
                                            dialogBuilder.PositiveText(GetText(Resource.String.Lbl_AddWallet)).OnPositive((materialDialog, action) =>
                                            {
                                                try
                                                {
                                                    StartActivity(new Intent(this, typeof(WalletActivity)));
                                                }
                                                catch (Exception exception)
                                                {
                                                    Methods.DisplayReportResultTrack(exception);
                                                }
                                            });
                                            dialogBuilder.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                                            dialogBuilder.AlwaysCallSingleChoiceCallback();
                                            dialogBuilder.Build().Show();
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Methods.DisplayReportResultTrack(exception);
                                    }
                                });
                                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                                dialog.AlwaysCallSingleChoiceCallback();
                                dialog.Build().Show();
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribe);
                                //Color
                                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                //Drawable icon = GetDrawable(Resource.Drawable.SubcribeButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Remove The Video to Subscribed Videos Database
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.RemoveSubscriptionsChannel(VideoDataHandler.Owner?.OwnerClass?.Id);

                                //Send API Request here for UnSubscribed
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.AddSubscribeToChannelAsync(VideoDataHandler.Owner?.OwnerClass?.Id) });

                                // Toast.MakeText(this, this.GetText(Resource.String.Lbl_Channel_Removed_successfully, ToastLength.Short)?.Show();
                            }
                        }
                        else
                        {
                            if (SubscribeChannelButton.Tag?.ToString() == "Subscribe")
                            {
                                SubscribeChannelButton.Tag = "Subscribed";
                                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribed);

                                //Color
                                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                //Drawable icon = GetDrawable(Resource.Drawable.SubcribedButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Add The Video to  Subcribed Videos Database
                                Events_Insert_SubscriptionsChannel();

                                //Send API Request here for Subcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.AddSubscribeToChannelAsync(VideoDataHandler.Owner?.OwnerClass?.Id) });

                                Toast.MakeText(this, GetText(Resource.String.Lbl_Subscribed_successfully), ToastLength.Short)?.Show();
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribe);
                                //Color
                                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                //Drawable icon = GetDrawable(Resource.Drawable.SubcribeButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Remove The Video to Subcribed Videos Database
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.RemoveSubscriptionsChannel(VideoDataHandler.Owner?.OwnerClass?.Id);

                                //Send API Request here for UnSubcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.AddSubscribeToChannelAsync(VideoDataHandler.Owner?.OwnerClass?.Id) });

                                // Toast.MakeText(this, this.GetText(Resource.String.Lbl_Channel_Removed_successfully, ToastLength.Short)?.Show();
                            }
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(this, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_sign_in_Subcribed), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ImageChannelViewOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent();
                intent.PutExtra("Open", "UserProfile");
                intent.PutExtra("UserObject", JsonConvert.SerializeObject(VideoDataHandler.Owner));
                SetResult(Result.Ok, intent);

                FinishActivityAndTask();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ShowMoreDescriptionIconViewOnClick(object sender, EventArgs e)
        {
            try
            {
                if (VideoDescriptionLayout.Tag?.ToString() == "Open")
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDown);
                    VideoDescriptionLayout.Visibility = ViewStates.Gone;
                    VideoDescriptionLayout.Tag = "closed";
                    VideoTitle.Text = Methods.FunString.DecodeString(VideoDataHandler.Title);
                    VideoDescriptionLayout.Animate().Alpha(1).SetDuration(400);
                    TextChannelName.Animate().Alpha(1).SetDuration(300);
                    VideoChannelViews.Animate().Alpha(1).SetDuration(300);
                    VideoTitle.SetMaxLines(1);

                    ViewGroup parent = (ViewGroup)VideoDescription.Parent;
                    ViewGroup.LayoutParams par = parent.LayoutParameters;
                    par.Height = 200;
                    VideoDescriptionLayout.LayoutParameters = par;
                }
                else
                {
                    // VideoDescriptionLayout.LayoutParameters = ViewGroup.LayoutParams.WrapContent;
                    //LinearLayout.LayoutParams par = (LinearLayout.LayoutParams)VideoDescriptionLayout.LayoutParameters;
                    ViewGroup parent = (ViewGroup)VideoDescription.Parent;
                    ViewGroup.LayoutParams par = parent.LayoutParameters;
                    par.Height = ViewGroup.LayoutParams.WrapContent;
                    VideoDescriptionLayout.LayoutParameters = par;

                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowUp);
                    VideoDescriptionLayout.Visibility = ViewStates.Visible;
                    VideoDescriptionLayout.Tag = "Open";
                    VideoTitle.Text = Methods.FunString.DecodeString(VideoDataHandler.Title);
                    VideoDescriptionLayout.Animate().Alpha(1).SetDuration(500);
                    TextChannelName.Animate().Alpha(1).SetDuration(300);
                    VideoChannelViews.Animate().Alpha(1).SetDuration(300);
                    VideoTitle.SetMaxLines(4);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Event Menu >> WatchLater
        private async void OnMenuAddWatchLaterClick(VideoDataObject videoObject)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    if (Methods.CheckConnectivity())
                    {
                        var (apiStatus, respond) = await RequestsAsync.Video.AddToWatchLaterVideosAsync(VideoDataHandler.Id);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageCodeObject result)
                            {
                                if (result.SuccessType.Contains("Removed"))
                                {
                                    LibrarySynchronizer.RemovedFromWatchLater(videoObject);
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_RemovedFromWatchLater), ToastLength.Short)?.Show();
                                }
                                else if (result.SuccessType.Contains("Added"))
                                {
                                    LibrarySynchronizer.AddToWatchLater(videoObject);
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_AddedToWatchLater), ToastLength.Short)?.Show();
                                }
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                    }
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(this, videoObject, "Login");
                    dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_sign_in_WatchLater), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Event Menu >> Playlist
        private void OnMenuAddPlaylistClick(VideoDataObject videoObject)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        PopupDialogController dialog = new PopupDialogController(this, videoObject, "PlayList");
                        dialog.ShowPlayListDialog();
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(this, videoObject, "Login");
                        dialog.ShowNormalDialog(GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_sign_in_playlist), GetText(Resource.String.Lbl_Yes), GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void Events_Insert_SubscriptionsChannel()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                if (VideoDataHandler.Owner != null)
                    sqlEntity.Insert_One_SubscriptionChannel(VideoDataHandler.Owner?.OwnerClass);

                LibrarySynchronizer.AddToSubscriptions(VideoDataHandler.Owner?.OwnerClass);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Rent
        private void RentButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                dialog.Title(Resource.String.Lbl_purchaseVideo);
                dialog.Content(GetText(Resource.String.Lbl_RentVideo));
                dialog.PositiveText(GetText(Resource.String.Lbl_Purchase)).OnPositive(async (materialDialog, action) =>
                {
                    try
                    {
                        if (AppTools.CheckWallet())
                        {
                            if (Methods.CheckConnectivity())
                            {
                                var (apiStatus, respond) = await RequestsAsync.Payment.PurchaseAsync("rent", VideoDataHandler.Id);
                                if (apiStatus == 200)
                                {
                                    if (respond is MessageObject result)
                                    {
                                        Console.WriteLine(result.Message);

                                        Toast.MakeText(this, GetText(Resource.String.Lbl_PurchasedSuccessfully), ToastLength.Long)?.Show();
                                    }
                                }
                                else Methods.DisplayReportResult(this, respond);
                            }
                            else
                                Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                        }
                        else
                        {
                            var dialogBuilder = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                            dialogBuilder.Title(GetText(Resource.String.Lbl_Wallet));
                            dialogBuilder.Content(GetText(Resource.String.Lbl_Error_NoWallet));
                            dialogBuilder.PositiveText(GetText(Resource.String.Lbl_AddWallet)).OnPositive((materialDialog, action) =>
                            {
                                try
                                {
                                    StartActivity(new Intent(this, typeof(WalletActivity)));
                                }
                                catch (Exception exception)
                                {
                                    Methods.DisplayReportResultTrack(exception);
                                }
                            });
                            dialogBuilder.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                            dialogBuilder.AlwaysCallSingleChoiceCallback();
                            dialogBuilder.Build().Show();
                        }
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                });
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Donate
        private void DonateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(VideoDataHandler.Owner?.OwnerClass?.DonationPaypalEmail))
                {
                    var url = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=" + VideoDataHandler.Owner?.OwnerClass?.DonationPaypalEmail + "&lc=US&item_name=Donation+to+" + VideoDataHandler.Owner?.OwnerClass?.Name + "&no_note=0&cn=&currency_code=USD&bn=PP-DonationsBF:btn_donateCC_LG.gif:NonHosted";
                    new IntentController(this).OpenBrowserFromApp(url);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region PictureInPictur

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            try
            {
                var percentage = (float)Math.Abs(verticalOffset) / appBarLayout.TotalScrollRange;
                Console.WriteLine(percentage);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            try
            {
                CoordinatorLayoutView.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;

                switch (VideoType)
                {
                    case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                        if (VideoActionsController?.ControlView != null)
                            VideoActionsController.ControlView.Visibility = isInPictureInPictureMode ? ViewStates.Gone : ViewStates.Visible;
                        break;
                    case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                        if (isInPictureInPictureMode)
                            TubePlayerView?.PlayerUiController.ShowUi(false);
                        else
                            TubePlayerView.PlayerUiController.ShowUi(true);
                        break;
                }
                 
                if (isInPictureInPictureMode)
                {
                    // ...
                    switch (VideoType)
                    {
                        case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                            //VideoActionsController?.SetStopvideo();
                            break;
                        case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                            //YoutubePlayer.Play();
                            break;
                    }
                }
                else
                {
                    if (OnStopCalled)
                    {
                        switch (VideoType)
                        {
                            case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                                VideoActionsController?.SetStopvideo();
                                break;
                            case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                                YoutubePlayer.Pause();
                                TubePlayerView.PlayerUiController.ShowUi(true);
                                break;
                        }

                        FinishActivityAndTask();
                    }
                }

                base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnUserLeaveHint()
        {
            try
            {
                switch (VideoType)
                {
                    case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            var param = new PictureInPictureParams.Builder().Build();
                            EnterPictureInPictureMode(param);
                        }
                        base.OnUserLeaveHint();
                        break;
                    case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region YouTube Player

        //public void OnInitializationFailure(IYouTubePlayerProvider p0, YouTubeInitializationResult p1)
        //{
        //    try
        //    {
        //        if (AppSettings.DisableYouTubeInitializationFailureMessages)
        //            return;

        //        if (p1.IsUserRecoverableError)
        //            p1.GetErrorDialog(this, 1).Show();
        //        else
        //            Toast.MakeText(this, p1.ToString(), ToastLength.Short)?.Show();
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //    }
        //}

        public void OnInitSuccess(IYouTubePlayer player)
        {
            try
            {
                if (YoutubePlayer == null)
                {
                    YoutubePlayer = player;
                    YouTubePlayerEvents = new YouTubePlayerEvents(player, VideoIdYoutube);
                    YoutubePlayer.AddListener(YouTubePlayerEvents);
                    TubePlayerView.AddFullScreenListener(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnYouTubePlayerEnterFullScreen()
        {
            try
            {
                Intent intent = new Intent(this, typeof(YouTubePlayerFullScreenActivity));
                intent.PutExtra("type", TypeYouTubePlayerFullScreen);
                intent.PutExtra("VideoIdYoutube", VideoIdYoutube);
                intent.PutExtra("CurrentSecond", YouTubePlayerEvents.CurrentSecond);
                StartActivityForResult(intent, 2100);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnYouTubePlayerExitFullScreen()
        {
            try
            {
                TypeYouTubePlayerFullScreen = "RequestedOrientation";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Fun Video 

        public readonly List<Fragment> VideoFrameLayoutFragments = new List<Fragment>();

        private void SetVideoPlayerFragmentAdapters()
        {
            try
            {
                CommentsFragment = new CommentsFragment();
                NextToFragment = new NextToFragment();

                FragmentTransaction ftvideo = SupportFragmentManager.BeginTransaction();
                ftvideo.Add(Resource.Id.videoButtomLayout, NextToFragment, NextToFragment.Tag)?.Commit();

                TubePlayerView = FindViewById<YouTubePlayerView>(Resource.Id.youtube_player_view);
                if (TubePlayerView != null)
                {
                    TubePlayerView.Visibility = ViewStates.Gone;

                    // The player will automatically release itself when the activity is destroyed.
                    // The player will automatically pause when the activity is paused
                    // If you don't add YouTubePlayerView as a lifecycle observer, you will have to release it manually.
                    Lifecycle.AddObserver(TubePlayerView);

                    TubePlayerView.PlayerUiController.ShowMenuButton(false);

                    TubePlayerView.PlayerUiController.ShowCustomActionLeft1(true);
                    TubePlayerView.PlayerUiController.SetCustomActionLeft1(ContextCompat.GetDrawable(this, Resource.Drawable.ic_exo_icon_previous), this);

                    TubePlayerView.PlayerUiController.ShowCustomActionRight1(true);
                    TubePlayerView.PlayerUiController.SetCustomActionRight1(ContextCompat.GetDrawable(this, Resource.Drawable.ic_exo_icon_next), this);

                    TubePlayerView.PlayerUiController.ShowCustomActionLeft2(true);
                    TubePlayerView.PlayerUiController.SetCustomActionLeft2(ContextCompat.GetDrawable(this, Resource.Drawable.ic_exo_icon_rewind), this);

                    TubePlayerView.PlayerUiController.ShowCustomActionRight2(true);
                    TubePlayerView.PlayerUiController.SetCustomActionRight2(ContextCompat.GetDrawable(this, Resource.Drawable.ic_exo_icon_fastforward), this);

                    //TubePlayerView.PlayerUiController.Menu.AddItem(new MenuItem("example", Resource.Drawable.icon_settings_vector, (view)->Toast.makeText(this, "item clicked", Toast.LENGTH_SHORT).show()));
                    TubePlayerView.Initialize(this);
                }
                if (ThirdPartyPlayersFragment == null)
                {
                    FragmentTransaction ft1 = SupportFragmentManager.BeginTransaction();
                    ThirdPartyPlayersFragment = new ThirdPartyPlayersFragment();
                    ft1.Add(Resource.Id.root, ThirdPartyPlayersFragment, ThirdPartyPlayersFragment.Tag)?.Commit();

                    if (!VideoFrameLayoutFragments.Contains(ThirdPartyPlayersFragment))
                        VideoFrameLayoutFragments.Add(ThirdPartyPlayersFragment);
                }
                if (RestrictedVideoPlayerFragment == null)
                {
                    FragmentTransaction ft2 = SupportFragmentManager.BeginTransaction();
                    RestrictedVideoPlayerFragment = new RestrictedVideoFragment();
                    ft2.Add(Resource.Id.root, RestrictedVideoPlayerFragment, RestrictedVideoPlayerFragment.Tag)?.Commit();

                    if (!VideoFrameLayoutFragments.Contains(RestrictedVideoPlayerFragment))
                        VideoFrameLayoutFragments.Add(RestrictedVideoPlayerFragment);
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void LoadVideoData(VideoDataObject videoObject)
        {
            try
            {
                if (videoObject == null)
                    return;

                VideoDataHandler = videoObject;
                SetVideoType(VideoDataHandler);

                //Run fast data fetch from the server 
                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GetVideosInfoAsJson(videoObject.VideoId) });

                GlideImageLoader.LoadImage(this, videoObject.Owner?.OwnerClass?.Avatar, ImageChannelView, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);
                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);
                TextChannelName.Text = videoObject.Owner?.OwnerClass?.Username;
                VideoLikeCount.Text = videoObject.LikesPercent;
                VideoUnLikeCount.Text = videoObject.DislikesPercent;

                VideoPublishDate.Text = GetText(Resource.String.Lbl_Published_on) + " " + Methods.Time.ConvertToSpanishFormatIfNeeded(videoObject.TimeDate);
                VideoCategory.Text = CategoriesController.GetCategoryName(videoObject);
               
                //2M Views | 10K Shares | 500 Comments | 3 Months Ago
                TxtInfo.Text = CategoriesController.GetCategoryName(videoObject) + " | " + videoObject.Views + " " + GetText(Resource.String.Lbl_Views) + " | " + videoObject.TimeAgo;

                TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner?.OwnerClass?.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);

                if (videoObject.Owner != null && string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.SubscribeCount))
                    videoObject.Owner.Value.OwnerClass.SubscribeCount = "0";

                VideoChannelViews.Text = videoObject.Owner?.OwnerClass?.SubscribeCount + " " + GetText(Resource.String.Lbl_Subscribers);
              
                TextSanitizerAutoLink.Load(Methods.FunString.DecodeString(videoObject.Description));

                EditButton.Visibility = videoObject.IsOwner != null && videoObject.IsOwner.Value ? ViewStates.Visible : ViewStates.Gone;

                //Reset Views
                LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                LikeButton.Tag = "0";
                UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                UnLikeButton.Tag = "0";
                VideoLikeCount.Text = "0";
                VideoUnLikeCount.Text = "0";

                SubscribeChannelButton.Tag = "Subscribe";
                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribe);
                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                //Drawable icon = GetDrawable(Resource.Drawable.SubcribeButton);
                //icon.Bounds = new Rect(10, 10, 10, 7);
                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                //Close the description panel
                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDown);

                VideoDescriptionLayout.Tag = "closed";
                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);

                //Clear all data 
                if (CommentsFragment != null && CommentsFragment.MAdapter != null)
                {
                    CommentsFragment.MAdapter.CommentList?.Clear();
                    NextToFragment.MAdapter.VideoList?.Clear();
                    CommentsFragment.MAdapter.NotifyDataSetChanged();
                    NextToFragment.MAdapter.NotifyDataSetChanged();
                    CommentsFragment.StartApiService(videoObject.Id, "0");
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SetVideoType(VideoDataObject videoObject)
        {
            try
            {
                VideoType = VideoDataWithEventsLoader.VideoEnumTypes.Normal;
                var myDetails = ListUtils.MyChannelList?.FirstOrDefault();
                var age = string.IsNullOrWhiteSpace(myDetails?.Age) ? 0 : Convert.ToInt32(myDetails.Age);
                var isBelow18 = age > 0 && age < 18;

                if (videoObject.VideoLocation.Contains("Youtube") || videoObject.VideoLocation.Contains("youtu") || videoObject.VideoType == "VideoObject/youtube")
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.Youtube;
                else if (!string.IsNullOrEmpty(videoObject.Vimeo))
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.Vimeo;
                else if (!string.IsNullOrEmpty(videoObject.Daily))
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.DailyMotion;
                else if (!string.IsNullOrEmpty(videoObject.Ok))
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.Ok;
                else if (!string.IsNullOrEmpty(videoObject.Twitch))
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.Twitch;
                else if (!string.IsNullOrEmpty(videoObject.Facebook))
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.Facebook;
                else if (videoObject.IsOwner != null && videoObject.AgeRestriction == "2" && videoObject.IsOwner.Value == false && isBelow18)
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.AgeRestricted;
                else if (!string.IsNullOrEmpty(videoObject.GeoBlocking) && videoObject.IsOwner == false)
                    VideoType = VideoDataWithEventsLoader.VideoEnumTypes.GeoBlocked;

                videoObject.VideoType = VideoType.ToString();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task GetVideosInfoAsJson(string videoId)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Video.GetVideosDetailsAsync(videoId, UserDetails.AndroidId);
                if (apiStatus == 200)
                {
                    if (respond is GetVideosDetailsObject result)
                    {
                        var updater = ListUtils.GlobalViewsVideosList.FirstOrDefault(a => a.VideoId == videoId);
                        if (updater != null)
                        {
                            ListUtils.GlobalViewsVideosList.Add(updater);
                            SetNewDataVideo(updater);
                        }
                        else
                        {
                            ListUtils.GlobalViewsVideosList.Add(result.DataResult);
                            SetNewDataVideo(result.DataResult);
                        }

                        result.DataResult.SuggestedVideos = AppTools.ListFilter(result.DataResult.SuggestedVideos);
                        ListUtils.ArrayListPlay = new ObservableCollection<VideoDataObject>(result.DataResult.SuggestedVideos);
                        NextToFragment.LoadDataAsync(new ObservableCollection<VideoDataObject>(result.DataResult.SuggestedVideos));

                        if (ListUtils.AdsVideoList.Count > 0)
                        {
                            if (result.DataResult.VideoAd.VideoAdClass != null)
                                ListUtils.AdsVideoList.Add(result.DataResult.VideoAd.VideoAdClass);
                        }
                        else
                        {
                            ListUtils.AdsVideoList = new ObservableCollection<VideoAdDataObject>();

                            if (result.DataResult.VideoAd.VideoAdClass != null)
                                ListUtils.AdsVideoList.Add(result.DataResult.VideoAd.VideoAdClass);
                        }
                    }
                }
                else Methods.DisplayReportResult(this, respond);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void SetNewDataVideo(VideoDataObject videoObject)
        {
            try
            {
                if (videoObject == null)
                    return;

                VideoDataHandler = videoObject;
                SetVideoType(VideoDataHandler);

                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);
               
                //2M Views | 10K Shares | 500 Comments | 3 Months Ago
                TxtInfo.Text = CategoriesController.GetCategoryName(videoObject) + " | " + videoObject.Views + " " + GetText(Resource.String.Lbl_Views) + " | " + videoObject.TimeAgo;

                if (videoObject.IsLiked == "1") // true
                {

                    LikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                    UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    LikeButton.Tag = "1";
                }
                else
                {

                    LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    LikeButton.Tag = "0";
                }

                if (videoObject.IsDisliked == "1") // true
                {
                    LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    UnLikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                    UnLikeButton.Tag = "1";
                }
                else
                {
                    UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    UnLikeButton.Tag = "0";
                }


                var isOwner = videoObject.IsOwner != null && videoObject.IsOwner.Value;
                SubscribeChannelButton.Visibility = isOwner ? ViewStates.Invisible : ViewStates.Visible;
                EditButton.Visibility = isOwner ? ViewStates.Visible : ViewStates.Gone;

                VideoLikeCount.Text = videoObject.Likes;
                VideoUnLikeCount.Text = videoObject.Dislikes;
                CountComments.Text = ""; //wael add count comment after update 
                VideoPublishDate.Text = GetText(Resource.String.Lbl_Published_on) + " " + Methods.Time.ConvertToSpanishFormatIfNeeded(videoObject.TimeDate);
                VideoCategory.Text = CategoriesController.GetCategoryName(videoObject);

                //Verified 
                TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner?.OwnerClass?.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                TextChannelName.Text = AppTools.GetNameFinal(videoObject.Owner?.OwnerClass);
                GlideImageLoader.LoadImage(this, videoObject.Owner?.OwnerClass?.Avatar, ImageChannelView, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);

                if (videoObject.Owner != null && string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.SubscribeCount))
                    videoObject.Owner.Value.OwnerClass.SubscribeCount = "0";

                VideoChannelViews.Text = videoObject.Owner?.OwnerClass?.SubscribeCount + " " + GetText(Resource.String.Lbl_Subscribers);

                //Rent
                if (!string.IsNullOrEmpty(videoObject.RentPrice) && videoObject.RentPrice != "0" && AppSettings.RentVideosSystem)
                {
                    RentButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    RentButton.Visibility = ViewStates.Gone;
                }

                //Donate
                if (!string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.DonationPaypalEmail) && AppSettings.DonateVideosSystem)
                {
                    DonateButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    DonateButton.Visibility = ViewStates.Gone;
                }

                var file = VideoDownloadAsyncController.GetDownloadedDiskVideoUri(videoObject.Title);
                if (!string.IsNullOrEmpty(file))
                {
                    VideoActionsController.DownloadIcon.SetImageResource(Resource.Drawable.ic_checked_red);
                    VideoActionsController.DownloadIcon.Tag = "Downloaded";
                    LibrarySynchronizer.AddToWatchOffline(videoObject);
                }

                if (videoObject.Owner?.OwnerClass?.Id != UserDetails.UserId)
                {
                    UserDataObject channel = await ApiRequest.GetChannelData(this, videoObject.Owner?.OwnerClass?.Id);
                    if (channel != null)
                    {
                        videoObject.Owner = channel;

                        if (!string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.SubscriberPrice) && videoObject.Owner?.OwnerClass?.SubscriberPrice != "0")
                        {
                            if (videoObject.Owner?.OwnerClass?.AmISubscribed == "0")
                            {
                                //This channel is paid, You must pay to subscribe
                                SubscribeChannelButton.Tag = "PaidSubscribe";

                                //Color
                                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                //Drawable icon = GetDrawable(Resource.Drawable.SubcribeButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                                var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                                Console.WriteLine(currency);
                                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribe) + " " + currencyIcon + videoObject.Owner?.OwnerClass?.SubscriberPrice;
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribed";

                                SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribed);

                                //Color
                                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                //Drawable icon = GetDrawable(Resource.Drawable.SubcribedButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                            }
                        }
                        else
                        {
                            SubscribeChannelButton.Tag = videoObject.Owner?.OwnerClass?.AmISubscribed == "1" ? "Subscribed" : "Subscribe";

                            switch (SubscribeChannelButton.Tag?.ToString())
                            {
                                case "Subscribed":
                                {
                                    SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribed);

                                    //Color
                                    //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                    //icon
                                    //Drawable icon = GetDrawable(Resource.Drawable.SubcribedButton);
                                    //icon.Bounds = new Rect(10, 10, 10, 7);
                                    //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                                    break;
                                }
                                case "Subscribe":
                                {
                                    SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribe);

                                    //Color
                                    //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                    //icon
                                    //Drawable icon = GetDrawable(Resource.Drawable.SubcribeButton);
                                    //icon.Bounds = new Rect(10, 10, 10, 7);
                                    //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                                    break;
                                }
                            }
                        }

                        //Verified 
                        TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner?.OwnerClass?.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                    }
                }
                else
                {
                    UserDataObject channel = ListUtils.MyChannelList?.FirstOrDefault();
                    if (channel == null) return;
                    videoObject.Owner = channel;
                    //Verified 
                    TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner?.OwnerClass?.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void StartPlayVideo(VideoDataObject videoObject)
        {
            try
            {
                await Task.Delay(1500);

                RestrictedVideoPlayerFragment?.HideRestrictedInfo(true);
                UpdateMainRootDefaultSize();

                if (videoObject != null)
                {
                    VideoDataHandler = videoObject;

                    LoadVideoData(videoObject);
                    HideCommentsAndShowNextTo();

                    VideoActionsController.ExoBackButton.SetImageResource(Resource.Drawable.ic_action_arrow_down_sign);
                    VideoActionsController.ExoBackButton.Tag = "Open";

                    var userNotVideoOwner = videoObject.IsOwner != null && !videoObject.IsOwner.Value;
                    if (userNotVideoOwner &&
                        (!string.IsNullOrEmpty(videoObject.SellVideo) && videoObject.SellVideo != "0"
                         || !string.IsNullOrEmpty(videoObject.RentPrice) && videoObject.RentPrice != "0" && AppSettings.RentVideosSystem
                         || !string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.SubscriberPrice) && videoObject.Owner?.OwnerClass?.SubscriberPrice != "0" & videoObject.Owner?.OwnerClass?.AmISubscribed == "0"))
                    {
                        GlobalVideosRelease("All");
                        CustomNavigationController.BringFragmentToTop(RestrictedVideoPlayerFragment, SupportFragmentManager, VideoFrameLayoutFragments);

                        if (!string.IsNullOrEmpty(videoObject.SellVideo) && videoObject.SellVideo != "0")
                            RestrictedVideoPlayerFragment.LoadRestriction("purchaseVideo", videoObject.Thumbnail, videoObject);
                        else if (!string.IsNullOrEmpty(videoObject.RentPrice) && videoObject.RentPrice != "0" && AppSettings.RentVideosSystem)
                            RestrictedVideoPlayerFragment.LoadRestriction("RentVideo", videoObject.Thumbnail, videoObject);
                        else if (!string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.SubscriberPrice) && videoObject.Owner?.OwnerClass?.SubscriberPrice != "0" & videoObject.Owner?.OwnerClass?.AmISubscribed == "0")
                            RestrictedVideoPlayerFragment.LoadRestriction("Subscriber", videoObject.Thumbnail, videoObject);
                    }
                    else
                    {
                        switch (VideoType)
                        {
                            case VideoDataWithEventsLoader.VideoEnumTypes.AgeRestricted:
                                GlobalVideosRelease("All");
                                CustomNavigationController.BringFragmentToTop(RestrictedVideoPlayerFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                                RestrictedVideoPlayerFragment.LoadRestriction("AgeRestriction", videoObject.Thumbnail, videoObject);
                                break;
                            case VideoDataWithEventsLoader.VideoEnumTypes.GeoBlocked:
                                GlobalVideosRelease("All");
                                CustomNavigationController.BringFragmentToTop(RestrictedVideoPlayerFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                                RestrictedVideoPlayerFragment.LoadRestriction("GeoRestriction", videoObject.Thumbnail, videoObject);
                                break;
                            case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                                VideoIdYoutube = videoObject.VideoLocation.Split(new[] { "v=" }, StringSplitOptions.None).LastOrDefault();
                                GlobalVideosRelease("Youtube");
                                CustomNavigationController.BringFragmentToTop(null, SupportFragmentManager, VideoFrameLayoutFragments);

                                if (Lifecycle.CurrentState == Lifecycle.State.Resumed)
                                    YoutubePlayer.LoadVideo(VideoIdYoutube, 0);
                                else
                                    YoutubePlayer.CueVideo(VideoIdYoutube, 0);

                                break;
                            case VideoDataWithEventsLoader.VideoEnumTypes.Facebook:
                            case VideoDataWithEventsLoader.VideoEnumTypes.Twitch:
                            case VideoDataWithEventsLoader.VideoEnumTypes.DailyMotion:
                            case VideoDataWithEventsLoader.VideoEnumTypes.Ok:
                            case VideoDataWithEventsLoader.VideoEnumTypes.Vimeo:
                                GlobalVideosRelease("All");
                                CustomNavigationController.BringFragmentToTop(ThirdPartyPlayersFragment, SupportFragmentManager, VideoFrameLayoutFragments);
                                ThirdPartyPlayersFragment.SetVideoIframe(videoObject);
                                break;
                            default:
                                GlobalVideosRelease("exo");
                                CustomNavigationController.BringFragmentToTop(null, SupportFragmentManager, VideoFrameLayoutFragments);
                                VideoActionsController.PlayVideo(videoObject.VideoLocation, videoObject, RestrictedVideoPlayerFragment, this);
                                break;
                        }
                    }

                    TabbedMainActivity.GetInstance()?.SetOnWakeLock();
                    LibrarySynchronizer.AddToRecentlyWatched(videoObject);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        private void UpdateMainRootDefaultSize()
        {
            try
            {
                if (AppSettings.ShowVideoWithDynamicHeight)
                {
                    var p = MainVideoRoot.LayoutParameters;
                    p.Width = GetScreenWidth();
                    p.Height = VideoActionsController.IsPortraitVideo && VideoActionsController.PortraitHeight > 0 ? VideoActionsController.PortraitHeight : CovertDpToPixel(220);
                    MainVideoRoot.RequestLayout();
                    VideoActionsController?.FullWidthSetting();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void UpdateMainRootDefaultLandscapeSize()
        {
            try
            {
                var p = MainVideoRoot.LayoutParameters;
                p.Width = GetScreenWidth();
                p.Height = CovertDpToPixel(220);
                MainVideoRoot.RequestLayout();
                VideoActionsController?.SetFullWidthDefaultSetting();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private int GetScreenWidth()
        {
            try
            {
                var displayMetrics = Resources.System.DisplayMetrics;
                int width = displayMetrics.WidthPixels;
                return width;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        public int CovertDpToPixel(int dp)
        {
            try
            {
                var displayMetrics = Resources.System.DisplayMetrics;
                return (int)(dp * displayMetrics.Density);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        private void GlobalVideosRelease(string exepttype)
        {
            try
            {
                switch (exepttype)
                {
                    case "exo":
                    {
                        if (YouTubePlayerEvents != null && YoutubePlayer != null && YouTubePlayerEvents.IsPlaying)
                            YoutubePlayer?.Pause();

                        if (TubePlayerView != null) TubePlayerView.Visibility = ViewStates.Gone;

                        if (VideoActionsController.SimpleExoPlayerView.Visibility == ViewStates.Gone)
                            VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Visible;
                        break;
                    }
                    case "Youtube":
                    {
                        VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Gone;
                        VideoActionsController.ReleaseVideo();

                        if (TubePlayerView != null) TubePlayerView.Visibility = ViewStates.Visible;

                        break;
                    }
                    case "All":
                    {
                        if (YouTubePlayerEvents != null && YoutubePlayer != null && YouTubePlayerEvents.IsPlaying)
                            YoutubePlayer?.Pause();
                        if (TubePlayerView != null) TubePlayerView.Visibility = ViewStates.Gone;

                        VideoActionsController.SimpleExoPlayerView.Visibility = ViewStates.Gone;
                        VideoActionsController.ReleaseVideo();
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == Resource.Id.custom_action_left_button)
                {
                    YouTubePlayerEvents?.BtnPreviousOnClick();
                }
                else if (v.Id == Resource.Id.custom_action_right_button)
                {
                    YouTubePlayerEvents?.BtnNextOnClick(); 
                }
                else if (v.Id == Resource.Id.custom_action_left_button2)
                {
                    YouTubePlayerEvents?.BtnBackwardOnClick("GlobalPlayerActivity");
                }
                else if (v.Id == Resource.Id.custom_action_right_button2)
                {
                    YouTubePlayerEvents?.BtnForwardOnClick("GlobalPlayerActivity");
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "AddTo" when itemString == GetString(Resource.String.Lbl_Addto_playlist):
                        OnMenuAddPlaylistClick(VideoDataHandler);
                        break;
                    case "AddTo":
                    {
                        if (itemString == GetString(Resource.String.Lbl_Addto_watchlater) || itemString == GetString(Resource.String.Lbl_RemoveFromWatchLater))
                        {
                            OnMenuAddWatchLaterClick(VideoDataHandler);
                        }

                        break;
                    } 
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            { 
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
          
        #endregion

        #region Permissions && Result

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                
                switch (requestCode)
                {
                    case 2000 when resultCode == Result.Ok:
                    {
                        VideoActionsController.RestartPlayAfterShrinkScreen();
                        break;
                    }
                    case 2100 when resultCode == Result.Ok:
                    {
                        TubePlayerView?.ExitFullScreen();
                        break;
                    } 
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                switch (requestCode)
                { 
                    case 100 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        VideoActionsController?.DownloadVideo();
                        break;
                    case 100:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
        
        public async void SetSubscribeChannelWithPaid()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    SubscribeChannelButton.Tag = "Subscribed";
                    SubscribeChannelButton.Text = GetText(Resource.String.Lbl_Subscribed);

                    //Color
                    //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                    //icon
                    //Drawable icon = GetDrawable(Resource.Drawable.SubcribedButton);
                    //icon.Bounds = new Rect(10, 10, 10, 7);
                    //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                    //Add The Video to  Subscribed Videos Database
                    Events_Insert_SubscriptionsChannel();

                    //Send API Request here for Subscribed
                    var (apiStatus, respond) = await RequestsAsync.Global.AddSubscribeToChannelAsync(VideoDataHandler.Owner?.OwnerClass?.Id, "paid");
                    if (apiStatus == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Subscribed_successfully), ToastLength.Short)?.Show();
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void FinishActivityAndTask()
        {
            try
            {
                switch (VideoType)
                {
                    case VideoDataWithEventsLoader.VideoEnumTypes.Normal:
                        VideoActionsController.ReleaseVideo();
                        //MoveTaskToBack(true);
                        FinishAndRemoveTask();
                        Finish();
                        break;
                    case VideoDataWithEventsLoader.VideoEnumTypes.Youtube:
                        YoutubePlayer?.Pause();
                        Finish();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
    }
}