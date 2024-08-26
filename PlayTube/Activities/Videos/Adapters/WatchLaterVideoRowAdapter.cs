using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.Util;
using PlayTube.Activities.Models;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;
using IList = System.Collections.IList;

namespace PlayTube.Activities.Videos.Adapters
{
    public class WatchLaterVideoRowAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider, IVideoMenuListener
    {
        public event EventHandler<WatchLaterVideoAdapterClickEventArgs> ItemClick;
        public event EventHandler<WatchLaterVideoAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<DataWatchLaterVideos> VideoList = new ObservableCollection<DataWatchLaterVideos>();
        private readonly LibrarySynchronizer LibrarySynchronizer;
        private readonly string Type;
        private readonly TabbedMainActivity GlobalContext;

        public WatchLaterVideoRowAdapter(Activity context, string type = "Normal")
        {
            HasStableIds = true;
            ActivityContext = context;
            LibrarySynchronizer = new LibrarySynchronizer(context);
            Type = type;
            GlobalContext = TabbedMainActivity.GetInstance();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Video_Row_View
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_VideoRowView, parent, false);
                var vh = new WatchLaterVideoRowAdapterViewHolder(itemView, OnClick, OnLongClick);

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
                if (viewHolder is WatchLaterVideoRowAdapterViewHolder holder)
                {
                    var item = VideoList[position];
                    if (item?.Videos != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Videos?.VideoAdClass.Thumbnail, holder.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        GlideImageLoader.LoadImage(ActivityContext, item.Videos?.VideoAdClass.Owner?.OwnerClass?.Avatar, holder.ChannelImage, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);

                        holder.TxtDuration.Text = Methods.Time.SplitStringDuration(item.Videos?.VideoAdClass.Duration);
                        holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Videos?.VideoAdClass.Title);

                        holder.TxtChannelName.Text = AppTools.GetNameFinal(item.Videos?.VideoAdClass.Owner?.OwnerClass);
                        holder.TxtChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, item.Videos?.VideoAdClass.Owner?.OwnerClass?.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);

                        holder.TxtViewsCount.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Videos?.VideoAdClass.Views)) + " " + ActivityContext.GetText(Resource.String.Lbl_Views);

                        if (!holder.MenuView.HasOnClickListeners)
                        {
                            holder.MenuView.Click += (sender, args) =>
                            {
                                var data = GetItem(holder.BindingAdapterPosition);
                                VideoMenuBottomSheets videoMenuBottomSheets = new VideoMenuBottomSheets(data.Videos?.VideoAdClass, this);
                                videoMenuBottomSheets.Show(GlobalContext.SupportFragmentManager, videoMenuBottomSheets.Tag);
                            };

                            holder.InfoContainer.Click += (sender, args) =>
                            {
                                var data = GetItem(holder.BindingAdapterPosition);
                                GlobalContext?.ShowUserChannelFragment(data.Videos?.VideoAdClass.Owner?.OwnerClass, data.Videos?.VideoAdClass.Owner?.OwnerClass.Id);
                            };
                        }

                        //Set Badge on videos
                        AppTools.ShowGlobalBadgeSystem(holder.VideoType, item.Videos?.VideoAdClass);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
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

        public override int ItemCount => VideoList?.Count ?? 0;
        
        public DataWatchLaterVideos GetItem(int position)
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
        void OnClick(WatchLaterVideoAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(WatchLaterVideoAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            try
            {
                if (ActivityContext?.IsDestroyed != false)
                    return;

                if (holder is WatchLaterVideoRowAdapterViewHolder viewHolder)
                {
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

                if (item.Videos?.VideoAdClass.Thumbnail != "")
                {
                    d.Add(item.Videos?.VideoAdClass.Thumbnail);
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
    }

    public class WatchLaterVideoRowAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView VideoImage { get; private set; }
        public TextView TxtDuration { get; private set; }
        public ImageView ChannelImage { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtChannelName { get; private set; }
        public TextView TxtViewsCount { get; private set; }
        public TextView VideoType { get; private set; }
        public TextView MenuView { get; private set; }
        public LinearLayout InfoContainer { get; private set; }

        #endregion

        public WatchLaterVideoRowAdapterViewHolder(View itemView, Action<WatchLaterVideoAdapterClickEventArgs> clickListener,Action<WatchLaterVideoAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                VideoImage = MainView.FindViewById<ImageView>(Resource.Id.Imagevideo);
                VideoType = MainView.FindViewById<TextView>(Resource.Id.videoType);
                TxtDuration = MainView.FindViewById<TextView>(Resource.Id.duration);

                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.Title);
                MenuView = MainView.FindViewById<TextView>(Resource.Id.videoMenu);

                InfoContainer = MainView.FindViewById<LinearLayout>(Resource.Id.info_container);
                ChannelImage = MainView.FindViewById<ImageView>(Resource.Id.Image_Channel);
                TxtChannelName = MainView.FindViewById<TextView>(Resource.Id.ChannelName);
                TxtViewsCount = MainView.FindViewById<TextView>(Resource.Id.Views_Count);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, MenuView, FontAwesomeIcon.EllipsisV);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new WatchLaterVideoAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new WatchLaterVideoAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class WatchLaterVideoAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}