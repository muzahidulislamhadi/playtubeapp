using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.Util;
using PlayTubeClient.Classes.Playlist;
using Methods = PlayTube.Helpers.Utils.Methods;
using IList = System.Collections.IList;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Load.Engine;
using PlayTube.Helpers.CacheLoaders;

namespace PlayTube.Activities.Playlist.Adapters
{
    public class PlayListsRowAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<PlayListsRowAdapterClickEventArgs> ItemClick;
        public event EventHandler<PlayListsRowAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<PlayListVideoObject> PlayListsList = new ObservableCollection<PlayListVideoObject>();
        private readonly RequestOptions Options;

        public PlayListsRowAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
            Options = new RequestOptions().Apply(RequestOptions.CenterCropTransform()
                .Transform(new CenterCrop(), new RoundedCorners(8))
                .SetPriority(Priority.High)
                .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All).AutoClone()
                .Error(Resource.Drawable.ImagePlacholder)
                .Placeholder(Resource.Drawable.ImagePlacholder));
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_PlayListRowView
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_PlayListRowView, parent, false);
                var vh = new PlayListsRowAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is PlayListsRowAdapterViewHolder holder)
                {
                    var item = PlayListsList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Thumbnail, holder.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable, false, Options);
                        
                        holder.TxtPlayListName.Text = Methods.FunString.DecodeString(item.Name);
                        holder.TxtViewsCount.Text = item.Count == 0 ? ActivityContext.GetText(Resource.String.Lbl_NoVideos) : item.Count + " " + ActivityContext.GetText(Resource.String.Lbl_Videos); 
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
        
        public override int ItemCount => PlayListsList?.Count ?? 0;
        
        public PlayListVideoObject GetItem(int position)
        {
            return PlayListsList[position];
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

        void OnClick(PlayListsRowAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PlayListsRowAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);


        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            try
            {
                if (ActivityContext?.IsDestroyed != false)
                    return;

                if (holder is PlayListsRowAdapterViewHolder viewHolder)
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
                var item = PlayListsList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Thumbnail != "")
                {
                    d.Add(item.Thumbnail);
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

    public class PlayListsRowAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public ImageView VideoImage { get; private set; }
        public TextView TxtPlayListName { get; private set; }
        public TextView TxtViewsCount { get; private set; }

        #endregion

        public PlayListsRowAdapterViewHolder(View itemView, Action<PlayListsRowAdapterClickEventArgs> clickListener, Action<PlayListsRowAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                VideoImage = MainView.FindViewById<ImageView>(Resource.Id.PlayListIcon);
                TxtPlayListName = MainView.FindViewById<TextView>(Resource.Id.PlayListName);
                TxtViewsCount = MainView.FindViewById<TextView>(Resource.Id.PlayListCount);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new PlayListsRowAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new PlayListsRowAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class PlayListsRowAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}