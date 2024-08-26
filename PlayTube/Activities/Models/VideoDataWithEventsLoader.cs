using MaterialDialogsCore;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using PlayTube.Library.Anjo.SuperTextLibrary;
using Com.Sothree.Slidinguppanel;
using Newtonsoft.Json;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils; 
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AndroidX.AppCompat.Widget;
using PlayTube.Activities.SettingsPreferences;
using PlayTube.Activities.SettingsPreferences.General;
using Exception = System.Exception;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using PlayTube.Helpers.ShimmerUtils;

namespace PlayTube.Activities.Models
{
    public class VideoDataWithEventsLoader : Java.Lang.Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback 
    {
        private readonly TabbedMainActivity ActivityContext;
        private TextView  TextChannelName, EditIconView, TxtInfo;
        private ImageView ImageChannelView, ShareIconView, AddToIconView, LikeIconView, UnLikeIconView, ViewMoreCommentSection;
        private AppCompatButton SubscribeChannelButton;
        private LinearLayout VideoDescriptionLayout, LikeButton, UnLikeButton, ShareButton, AddToButton, EditButton;
        private TextView VideoTitle, VideoLikeCount, VideoUnLikeCount, VideoChannelViews, VideoPublishDate, VideoCategory, UpNextTextview, CountComments;
        private SuperTextView VideoDescription;
        private TextSanitizer TextSanitizerAutoLink;
        private VideoDataObject VideoDataHandler;
        private readonly Activity Context;
        public VideoEnumTypes VideoType; 
        private LinearLayout DonateButton, RentButton;
        private string TypeDialog;
        private Switch AutoNextswitch;
        private FrameLayout VideoButtomLayout, ShowMoreDescriptionIconView;
        private RelativeLayout CommentButtomLayout;

        public ViewStub ShimmerPageLayout;
        private View InflatedShimmer;
        private TemplateShimmerInflater ShimmerInflater;

        public enum VideoEnumTypes
        {
            Youtube, Normal, DailyMotion, Vimeo, Ok, Twitch, Facebook, AgeRestricted, GeoBlocked
        }

        public VideoDataWithEventsLoader(Activity activityContext)
        {
            Context = activityContext;
            ActivityContext = (TabbedMainActivity)activityContext;
        }

        #region Functions

        public void SetViews()
        {
            try
            {
                //IsVideoDescriptionExpended = false;

                LikeIconView = ActivityContext.FindViewById<ImageView>(Resource.Id.Likeicon);
                UnLikeIconView = ActivityContext.FindViewById<ImageView>(Resource.Id.UnLikeicon);
                ShareIconView = ActivityContext.FindViewById<ImageView>(Resource.Id.Shareicon);
                AddToIconView = ActivityContext.FindViewById<ImageView>(Resource.Id.AddToicon);
                EditIconView = ActivityContext.FindViewById<TextView>(Resource.Id.editIcon);

                ShowMoreDescriptionIconView = ActivityContext.FindViewById<FrameLayout>(Resource.Id.video_ShowDiscription);
                VideoDescriptionLayout = ActivityContext.FindViewById<LinearLayout>(Resource.Id.videoDescriptionLayout);
                ImageChannelView = ActivityContext.FindViewById<ImageView>(Resource.Id.Image_Channel);
                TextChannelName = ActivityContext.FindViewById<TextView>(Resource.Id.ChannelName);
                SubscribeChannelButton = ActivityContext.FindViewById<AppCompatButton>(Resource.Id.SubcribeButton);
                 
                LikeButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.LikeButton);
                UnLikeButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.UnLikeButton);
                ShareButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.ShareButton);
                AddToButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.AddToButton);
                EditButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.editButton);

                LikeButton.Tag = "0";
                UnLikeButton.Tag = "0";

                RentButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.RentButton);
                DonateButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.DonateButton);

                RentButton.Visibility = ViewStates.Gone;
                DonateButton.Visibility = ViewStates.Gone;

                VideoTitle = ActivityContext.FindViewById<TextView>(Resource.Id.video_Titile);
                TxtInfo = ActivityContext.FindViewById<TextView>(Resource.Id.info);

                VideoLikeCount = ActivityContext.FindViewById<TextView>(Resource.Id.LikeNumber);
                VideoUnLikeCount = ActivityContext.FindViewById<TextView>(Resource.Id.UnLikeNumber);
                VideoChannelViews = ActivityContext.FindViewById<TextView>(Resource.Id.Channelviews);
                VideoPublishDate = ActivityContext.FindViewById<TextView>(Resource.Id.videoDate);
                VideoDescription = ActivityContext.FindViewById<SuperTextView>(Resource.Id.videoDescriptionTextview);
                VideoCategory = ActivityContext.FindViewById<TextView>(Resource.Id.videoCategorytextview);
                CountComments = ActivityContext.FindViewById<TextView>(Resource.Id.countComments);

                VideoButtomLayout = ActivityContext.FindViewById<FrameLayout>(Resource.Id.videoButtomLayout);
                CommentButtomLayout = ActivityContext.FindViewById<RelativeLayout>(Resource.Id.CommentButtomLayout);
                UpNextTextview = ActivityContext.FindViewById<TextView>(Resource.Id.UpNextTextview);
                ViewMoreCommentSection = ActivityContext.FindViewById<ImageView>(Resource.Id.viewMoreCommentsection);
                AutoNextswitch = ActivityContext.FindViewById<Switch>(Resource.Id.AutoNextswitch);
                AutoNextswitch.Checked = UserDetails.AutoNext;
                  
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EditIconView, IonIconsFonts.Create);

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
               

                TextSanitizerAutoLink = new TextSanitizer(VideoDescription, Context, ActivityContext);
               
                FragmentTransaction ftvideo = ActivityContext.SupportFragmentManager.BeginTransaction();
                ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                ftvideo.AddToBackStack(null);
                ftvideo.Add(VideoButtomLayout.Id, ActivityContext.CommentsFragment, null).Hide(ActivityContext.CommentsFragment)?.Commit();
           
                InitShimmer(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitShimmer()
        {
            try
            {
                ShimmerPageLayout = ActivityContext.FindViewById<ViewStub>(Resource.Id.viewStubShimmer);
                InflatedShimmer ??= ShimmerPageLayout.Inflate();

                ShimmerInflater = new TemplateShimmerInflater();
                ShimmerInflater.InflateLayout(ActivityContext, InflatedShimmer, ShimmerTemplateStyle.VideoRowTemplate);
                ShimmerInflater.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

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
                UpNextTextview.Text = ActivityContext.GetString(Resource.String.Lbl_Comments);
                UpNextTextview.Tag = ActivityContext.GetString(Resource.String.Lbl_Comments);
                ViewMoreCommentSection.Visibility = ViewStates.Visible;
                AutoNextswitch.Visibility = ViewStates.Gone;
                CommentButtomLayout.Visibility = ViewStates.Gone;
                FragmentTransaction ftvideo = ActivityContext.SupportFragmentManager.BeginTransaction();

                if (!ActivityContext.CommentsFragment.IsAdded)
                {
                    ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                    ftvideo.AddToBackStack(null);
                    ftvideo.Add(VideoButtomLayout.Id, ActivityContext.CommentsFragment, null)?.Commit();
                }
                else
                {
                    ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                    ftvideo.Hide(ActivityContext.NextToFragment).Show(ActivityContext.CommentsFragment)?.Commit();
                }

                ActivityContext.CommentsFragment.StartApiService(VideoDataHandler.Id, "0");
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
                UpNextTextview.Text = ActivityContext.GetString(Resource.String.Lbl_NextTo);
                UpNextTextview.Tag = ActivityContext.GetString(Resource.String.Lbl_NextTo);
                ViewMoreCommentSection.Visibility = ViewStates.Gone;
                AutoNextswitch.Visibility = ViewStates.Visible;
                CommentButtomLayout.Visibility = ViewStates.Visible;
                FragmentTransaction ftvideo = ActivityContext.SupportFragmentManager.BeginTransaction();
                ftvideo.AddToBackStack(null);
                ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                ftvideo.Hide(ActivityContext.CommentsFragment).Show(ActivityContext.NextToFragment)?.Commit();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void DonateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(VideoDataHandler.Owner?.OwnerClass?.DonationPaypalEmail))
                {
                    var url = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=" + VideoDataHandler.Owner?.OwnerClass?.DonationPaypalEmail + "&lc=US&item_name=Donation+to+" + VideoDataHandler.Owner?.OwnerClass?.Name + "&no_note=0&cn=&currency_code=USD&bn=PP-DonationsBF:btn_donateCC_LG.gif:NonHosted";
                    new IntentController(ActivityContext).OpenBrowserFromApp(url);
                } 
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
                var dialog = new MaterialDialog.Builder(Context).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                dialog.Title(Resource.String.Lbl_PurchaseRequired);
                dialog.Content(Context.GetText(Resource.String.Lbl_RentVideo));
                dialog.PositiveText(Context.GetText(Resource.String.Lbl_Purchase)).OnPositive(async (materialDialog, action) =>
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

                                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_PurchasedSuccessfully), ToastLength.Long)?.Show();
                                    }
                                }
                                else Methods.DisplayReportResult(Context, respond);
                            }
                            else
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                        }
                        else
                        {
                            var dialogBuilder = new MaterialDialog.Builder(Context).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                            dialogBuilder.Title(Context.GetText(Resource.String.Lbl_Wallet));
                            dialogBuilder.Content(Context.GetText(Resource.String.Lbl_Error_NoWallet));
                            dialogBuilder.PositiveText(Context.GetText(Resource.String.Lbl_AddWallet)).OnPositive((materialDialog, action) =>
                            {
                                try
                                {
                                    Context.StartActivity(new Intent(Context, typeof(WalletActivity)));
                                }
                                catch (Exception exception)
                                {
                                    Methods.DisplayReportResultTrack(exception);
                                }
                            });
                            dialogBuilder.NegativeText(Context.GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                            dialogBuilder.AlwaysCallSingleChoiceCallback();
                            dialogBuilder.Build().Show();
                        }
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                });
                dialog.NegativeText(Context.GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("ItemDataVideo", JsonConvert.SerializeObject(VideoDataHandler));

                var editVideoFragment = new EditVideoFragment
                {
                    Arguments = bundle
                };

                ActivityContext.FragmentBottomNavigator.DisplayFragment(editVideoFragment);

                switch (VideoType)
                {
                    case VideoEnumTypes.Normal:
                        ActivityContext.VideoActionsController.SetStopvideo();
                        break;
                    case VideoEnumTypes.Youtube:
                        ActivityContext.YoutubePlayer.Pause();
                        break;
                }

                if (ActivityContext.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                    ActivityContext.SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
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

                var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                var arrayAdapter = new List<string> { ActivityContext.GetString(Resource.String.Lbl_Addto_playlist) };

                var check = ListUtils.WatchLaterVideosList.FirstOrDefault(q => q.Videos?.VideoAdClass.Id == VideoDataHandler.Id);
                arrayAdapter.Add(check == null ? ActivityContext.GetString(Resource.String.Lbl_Addto_watchlater) : ActivityContext.GetString(Resource.String.Lbl_RemoveFromWatchLater));

                dialogList.Title(ActivityContext.GetString(Resource.String.Lbl_Add_To));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(ActivityContext.GetString(Resource.String.Lbl_Close)).OnNegative(this);
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
                ActivityContext?.LibrarySynchronizer?.ShareVideo(VideoDataHandler);
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

                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_UnLiked), ToastLength.Short)?.Show();

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
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Dislike), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
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
                                ActivityContext.LibrarySynchronizer.AddToLiked(VideoDataHandler);

                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_Liked), ToastLength.Short)?.Show();

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

                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Remove_Video_Liked), ToastLength.Short)?.Show();

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
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Like), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
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
                Bundle bundle = new Bundle();
                bundle.PutString("CatId", VideoDataHandler.CategoryId);
                bundle.PutString("CatName", VideoDataHandler.CategoryName);

                var videoViewerFragment = new VideosByCategoryFragment
                {
                    Arguments = bundle
                };

                ActivityContext.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);


                switch (VideoType)
                {
                    case VideoEnumTypes.Normal:
                        ActivityContext.VideoActionsController.SetStopvideo();
                        break;
                    case VideoEnumTypes.Youtube:
                        ActivityContext.YoutubePlayer.Pause();
                        break;
                }

                if (ActivityContext.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                    ActivityContext.SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
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
                                ActivityContext.OpenDialog(VideoDataHandler.Owner?.OwnerClass);
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribe);

                                // background
                                SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribe);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#0F64F7"));
                                //icon
                                //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
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
                        else
                        {
                            if (SubscribeChannelButton.Tag?.ToString() == "Subscribe")
                            {
                                SubscribeChannelButton.Tag = "Subscribed";
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribed);

                                // background
                                SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribed);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#737884"));
                                //icon
                                //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Add The Video to  Subcribed Videos Database
                                Events_Insert_SubscriptionsChannel();

                                //Send API Request here for Subcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.AddSubscribeToChannelAsync(VideoDataHandler.Owner?.OwnerClass?.Id) });


                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Subscribed_successfully), ToastLength.Short)?.Show();
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribe);

                                // background
                                SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribe);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#0F64F7"));
                                //icon
                                //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
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
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Subcribed), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         

        private void ImageChannelViewOnClick(object sender, EventArgs e)
        {
            ActivityContext.ShowUserChannelFragment(VideoDataHandler.Owner?.OwnerClass, VideoDataHandler.Owner?.OwnerClass?.Id);
        }

        private void ShowMoreDescriptionIconViewOnClick(object sender, EventArgs e)
        {
            try
            {
                if (VideoDescriptionLayout.Tag?.ToString() == "Open")
                {
                    //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDown);

                    ShowMoreDescriptionIconView.Rotation = 0;
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

                    ShowMoreDescriptionIconView.Rotation = 180;

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
                    //Send API Request here for WatchLater
                    if (Methods.CheckConnectivity())
                    {
                        var (apiStatus, respond) = await RequestsAsync.Video.AddToWatchLaterVideosAsync(VideoDataHandler.Id);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageCodeObject result)
                            {
                                if (result.SuccessType.Contains("Removed"))
                                {
                                    ActivityContext.LibrarySynchronizer.RemovedFromWatchLater(videoObject);
                                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_RemovedFromWatchLater), ToastLength.Short)?.Show();
                                }
                                else if (result.SuccessType.Contains("Added"))
                                {
                                    ActivityContext.LibrarySynchronizer.AddToWatchLater(videoObject);
                                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_AddedToWatchLater), ToastLength.Short)?.Show();
                                }
                            }
                        }
                        else Methods.DisplayReportResult(ActivityContext, respond);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                    }
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(ActivityContext, videoObject, "Login");
                    dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_WatchLater), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
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
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, videoObject, "PlayList");
                        dialog.ShowPlayListDialog();
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, videoObject, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_playlist), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
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

                ActivityContext.LibrarySynchronizer.AddToSubscriptions(VideoDataHandler.Owner?.OwnerClass);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Fun Video

        private void SetVideoType(VideoDataObject videoObject)
        {
            var myDetails = ListUtils.MyChannelList?.FirstOrDefault();
            var age = string.IsNullOrWhiteSpace(myDetails?.Age) ? 0 : Convert.ToInt32(myDetails.Age);
            var isBelow18 = age > 0 && age < 18;

            try
            {
                VideoType = VideoEnumTypes.Normal;

                if (videoObject.VideoLocation.Contains("Youtube") || videoObject.VideoLocation.Contains("youtu") || videoObject.VideoType == "VideoObject/youtube")
                    VideoType = VideoEnumTypes.Youtube;
                else if (!string.IsNullOrEmpty(videoObject.Vimeo))
                    VideoType = VideoEnumTypes.Vimeo;
                else if (!string.IsNullOrEmpty(videoObject.Daily))
                    VideoType = VideoEnumTypes.DailyMotion;
                else if (!string.IsNullOrEmpty(videoObject.Ok))
                    VideoType = VideoEnumTypes.Ok;
                else if (!string.IsNullOrEmpty(videoObject.Twitch))
                    VideoType = VideoEnumTypes.Twitch;
                else if (!string.IsNullOrEmpty(videoObject.Facebook))
                    VideoType = VideoEnumTypes.Facebook;
                else if (videoObject.IsOwner != null && videoObject.AgeRestriction == "2" && videoObject.IsOwner.Value == false && isBelow18)
                    VideoType = VideoEnumTypes.AgeRestricted;
                else if (!string.IsNullOrEmpty(videoObject.GeoBlocking) && videoObject.IsOwner == false)
                    VideoType = VideoEnumTypes.GeoBlocked;

                videoObject.VideoType = VideoType.ToString();
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

                ShimmerInflater?.Show();

                VideoDataHandler = videoObject;
                SetVideoType(VideoDataHandler);

                //Run fast data fetch from the server 
                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GetVideosInfoAsJson(videoObject.VideoId) });

                GlideImageLoader.LoadImage(Context, videoObject.Owner?.OwnerClass?.Avatar, ImageChannelView, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);
                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);
                TextChannelName.Text = videoObject.Owner?.OwnerClass?.Username;
                VideoLikeCount.Text = videoObject.LikesPercent;
                VideoUnLikeCount.Text = videoObject.DislikesPercent;
               
                VideoPublishDate.Text = ActivityContext.GetText(Resource.String.Lbl_Published_on) + " " + Methods.Time.ConvertToSpanishFormatIfNeeded(videoObject.TimeDate);
                VideoCategory.Text = CategoriesController.GetCategoryName(videoObject);

                //2M Views | 10K Shares | 500 Comments | 3 Months Ago
                TxtInfo.Text = CategoriesController.GetCategoryName(videoObject) + " | " + videoObject.Views + " " + Context.GetText(Resource.String.Lbl_Views) + " | " + videoObject.TimeAgo;

                ActivityContext.VideoChannelText.Text = TextChannelName.Text;
                ActivityContext.VideoTitleText.Text = VideoTitle.Text;

                if (videoObject.Owner?.OwnerClass?.Verified == "1")
                {
                    ActivityContext.VideoChannelText.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);
                    TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);
                }
                else
                {
                    ActivityContext.VideoChannelText.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                    TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                }

                //IsVideoDescriptionExpended = false;
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
                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribe);

                // background
                SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribe);

                //Color
                SubscribeChannelButton.SetTextColor(Color.ParseColor("#0F64F7"));

                //SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                //icon.Bounds = new Rect(10, 10, 10, 7);
                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                //Close the description panel
                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDown);

                ShowMoreDescriptionIconView.Rotation = 0;

                VideoDescriptionLayout.Visibility = ViewStates.Gone;
                VideoDescriptionLayout.Tag = "closed";

                //Clear all data 
                if (ActivityContext.CommentsFragment != null && ActivityContext.CommentsFragment.MAdapter != null)
                {
                    ActivityContext.CommentsFragment.MAdapter.CommentList?.Clear();
                    ActivityContext.NextToFragment.MAdapter.VideoList?.Clear();
                    ActivityContext.CommentsFragment.MAdapter.NotifyDataSetChanged();
                    ActivityContext.NextToFragment.MAdapter.NotifyDataSetChanged();
                    ActivityContext.CommentsFragment.StartApiService(videoObject.Id, "0");
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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
                VideoPublishDate.Text = ActivityContext.GetText(Resource.String.Lbl_Published_on) + " " + Methods.Time.ConvertToSpanishFormatIfNeeded(videoObject.TimeDate);
                VideoCategory.Text = CategoriesController.GetCategoryName(videoObject);

                //Verified 
                TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner?.OwnerClass?.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                TextChannelName.Text = AppTools.GetNameFinal(videoObject.Owner?.OwnerClass);
                GlideImageLoader.LoadImage(ActivityContext, videoObject.Owner?.OwnerClass?.Avatar, ImageChannelView, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);

                if (videoObject.Owner != null && string.IsNullOrEmpty(videoObject.Owner?.OwnerClass?.SubscribeCount))
                    videoObject.Owner.Value.OwnerClass.SubscribeCount = "0";

                VideoChannelViews.Text = videoObject.Owner?.OwnerClass?.SubscribeCount + " " + Context.GetText(Resource.String.Lbl_Subscribers);

                //IsVideoDescriptionExpended = false;
                TextSanitizerAutoLink.Load(Methods.FunString.DecodeString(videoObject.Description));
               

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
                    ActivityContext.VideoActionsController.DownloadIcon.SetImageResource(Resource.Drawable.ic_checked_red);
                    ActivityContext.VideoActionsController.DownloadIcon.Tag = "Downloaded";
                    ActivityContext.LibrarySynchronizer.AddToWatchOffline(videoObject);
                }

                if (videoObject.Owner?.OwnerClass?.Id != UserDetails.UserId)
                {
                    UserDataObject channel = await ApiRequest.GetChannelData(ActivityContext, videoObject.Owner?.OwnerClass?.Id);
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
                                //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                                //icon.Bounds = new Rect(10, 10, 10, 7);
                                //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                                var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                                Console.WriteLine(currency);
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribe) + " " + currencyIcon + videoObject.Owner?.OwnerClass?.SubscriberPrice;
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribed";

                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribed);


                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#737884"));

                                // back ground
                                SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribed);


                                //icon
                                //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
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
                                    SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribed);

                                        //Color
                                        SubscribeChannelButton.SetTextColor(Color.ParseColor("#737884"));

                                        // back ground
                                        SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribed);

                                        //icon
                                        //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
                                        //icon.Bounds = new Rect(10, 10, 10, 7);
                                        //SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                                        break;
                                }
                                case "Subscribe":
                                {
                                    SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Lbl_Subscribe);

                                        //Color
                                        SubscribeChannelButton.SetTextColor(Color.ParseColor("#0F64F7"));

                                        // back ground
                                        SubscribeChannelButton.SetBackgroundResource(Resource.Drawable.pif_btn_subscribe);
                                        //icon
                                        //Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
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
                        ActivityContext.NextToFragment.LoadDataAsync(new ObservableCollection<VideoDataObject>(result.DataResult.SuggestedVideos));

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
                    ShimmerInflater?.Hide();
                } 
            }
            catch (Exception e)
            {
                ShimmerInflater?.Hide();
                Methods.DisplayReportResultTrack(e);
            } 
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "AddTo" when itemString == ActivityContext.GetString(Resource.String.Lbl_Addto_playlist):
                        OnMenuAddPlaylistClick(VideoDataHandler);
                        break;
                    case "AddTo":
                    {
                        if (itemString == ActivityContext.GetString(Resource.String.Lbl_Addto_watchlater) || itemString == ActivityContext.GetString(Resource.String.Lbl_RemoveFromWatchLater))
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

    }
}