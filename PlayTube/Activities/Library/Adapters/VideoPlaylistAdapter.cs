using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using PlayTube.Activities.Models;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using Java.Util;
using IList = System.Collections.IList;
using AndroidX.RecyclerView.Widget;
using PlayTube.Activities.Videos.Adapters;
using PlayTubeClient.Classes.Playlist;
using PlayTube.Activities.Videos;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;

namespace PlayTube.Activities.Library.Adapters
{
    public class VideoPlaylistAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider, IVideoMenuListener
    {
        public event EventHandler<VideoAdapterClickEventArgs> ItemClick;
        public event EventHandler<VideoAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<DataVideoPlaylistObject> VideoList = new ObservableCollection<DataVideoPlaylistObject>();
        private readonly LibrarySynchronizer LibrarySynchronizer;
        private readonly TabbedMainActivity GlobalContext;

        public VideoPlaylistAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
            LibrarySynchronizer = new LibrarySynchronizer(context);
            GlobalContext = TabbedMainActivity.GetInstance();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Video_Big_View
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_VideoBigView, parent, false);
                var vh = new VideoPlaylistAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null!;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is VideoPlaylistAdapterViewHolder holder)
                {
                    var item = VideoList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Video.Thumbnail, holder.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        GlideImageLoader.LoadImage(ActivityContext, item.Video.Owner?.OwnerClass?.Avatar, holder.ChannelImage, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);

                        holder.TxtDuration.Text = Methods.Time.SplitStringDuration(item.Video.Duration);
                        holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Video.Title);

                        holder.TxtChannelName.Text = AppTools.GetNameFinal(item.Video.Owner?.OwnerClass);

                        holder.TxtViewsCount.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Video.Views)) + " " + ActivityContext.GetText(Resource.String.Lbl_Views);

                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.MenueView, IonIconsFonts.More);

                        //Verified 
                        if (item.Video.Owner?.OwnerClass?.Verified == "1")
                        {
                            holder.IconVerified.Visibility = ViewStates.Visible;
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.IconVerified, IonIconsFonts.CheckmarkCircle);
                        }
                        else
                        {
                            holder.IconVerified.Visibility = ViewStates.Gone;
                        }


                        if (!holder.MenueView.HasOnClickListeners)
                            holder.MenueView.Click += (sender, args) =>
                            {
                                var data = GetItem(holder.BindingAdapterPosition);
                                VideoMenuBottomSheets videoMenuBottomSheets = new VideoMenuBottomSheets(data.Video, this);
                                videoMenuBottomSheets.Show(GlobalContext.SupportFragmentManager, videoMenuBottomSheets.Tag);
                            };

                        //Set Badge on videos
                        AppTools.ShowGlobalBadgeSystem(holder.VideoType, item.Video);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        public override int ItemCount => VideoList?.Count ?? 0;

        public DataVideoPlaylistObject GetItem(int position)
        {
            return VideoList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        void OnClick(VideoAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(VideoAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            try
            {
                if (ActivityContext?.IsDestroyed != false)
                    return;

                if (holder is VideoPlaylistAdapterViewHolder viewHolder)
                {
                    Glide.With(ActivityContext).Clear(viewHolder.ChannelImage);
                    Glide.With(ActivityContext).Clear(viewHolder.VideoImage);
                }
                base.OnViewRecycled(holder);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = VideoList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Video.Thumbnail != "")
                {
                    d.Add(item.Video.Thumbnail);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Java.Lang.Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CenterCrop());
        }

        public void RemoveVideo(VideoDataObject data)
        {
            try
            {
                var check = VideoList.FirstOrDefault(a => a.VideoId == data.VideoId);
                if (check != null)
                {
                    var index = VideoList.IndexOf(check);
                    if (index != -1)
                    {
                        VideoList.Remove(check);
                        NotifyItemRemoved(index);
                        NotifyItemRangeChanged(index, ItemCount);

                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_Removed), ToastLength.Short)?.Show();

                        var dataObject = ListUtils.GlobalNotInterestedList.FirstOrDefault(a => a.Id == data.Id);
                        if (dataObject == null)
                        {
                            ListUtils.GlobalNotInterestedList.Add(data);
                        }

                    }
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.AddDeleteNotInterestedAsync(data.Id, true) });
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class VideoPlaylistAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public ImageView VideoImage { get; private set; }
        public TextView TxtDuration { get; private set; }
        public ImageView ChannelImage { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtChannelName { get; private set; }
        public TextView TxtViewsCount { get; private set; }
        public TextView IconVerified { get; private set; }
        public TextView MenueView { get; private set; }
        public TextView VideoType { get; private set; }
        #endregion

        public VideoPlaylistAdapterViewHolder(View itemView, Action<VideoAdapterClickEventArgs> clickListener, Action<VideoAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                VideoImage = MainView.FindViewById<ImageView>(Resource.Id.Imagevideo);
                TxtDuration = MainView.FindViewById<TextView>(Resource.Id.duration);
                ChannelImage = MainView.FindViewById<ImageView>(Resource.Id.Image_Channel);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.Title);
                TxtChannelName = MainView.FindViewById<TextView>(Resource.Id.ChannelName);
                TxtViewsCount = MainView.FindViewById<TextView>(Resource.Id.Views_Count);
                IconVerified = MainView.FindViewById<TextView>(Resource.Id.IconVerified);

                MenueView = MainView.FindViewById<TextView>(Resource.Id.videoMenue);
                VideoType = MainView.FindViewById<TextView>(Resource.Id.videoType);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new VideoAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, VideoStyle = VideoAdapterClickEventArgs.VideoType.BigVideo });
                itemView.LongClick += (sender, e) => longClickListener(new VideoAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }


}