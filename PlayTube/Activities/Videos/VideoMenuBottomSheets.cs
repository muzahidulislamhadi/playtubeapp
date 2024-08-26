using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using PlayTube.Activities.Models;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using Exception = System.Exception;

namespace PlayTube.Activities.Videos
{
    public class VideoMenuBottomSheets : BottomSheetDialogFragment
    {
        #region Variables Basic

        private TabbedMainActivity GlobalContext;
        private LinearLayout MenuAddWatchLater, MenuDownload, MenuAddPlaylist, MenuRemove, MenuShare, MenuReport; 

        private LibrarySynchronizer LibrarySynchronizer;
        private readonly VideoDataObject DataObject;
        private readonly IVideoMenuListener Listener;

        #endregion

        #region General

        public VideoMenuBottomSheets(VideoDataObject item, IVideoMenuListener listener)
        {
            DataObject = item;
            Listener = listener;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = TabbedMainActivity.GetInstance(); 
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                Context contextThemeWrapper = AppTools.IsTabDark() ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);
                View view = localInflater?.Inflate(Resource.Layout.PopupVideoMoreLayout, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);
                InitComponent(view);
                LibrarySynchronizer = new LibrarySynchronizer(GlobalContext);
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
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {  
                MenuAddWatchLater = view.FindViewById<LinearLayout>(Resource.Id.menu_AddWatchLater);
                MenuDownload = view.FindViewById<LinearLayout>(Resource.Id.menu_download);
                MenuAddPlaylist = view.FindViewById<LinearLayout>(Resource.Id.menu_AddPlaylist);
                MenuRemove = view.FindViewById<LinearLayout>(Resource.Id.menu_Remove);
                MenuShare = view.FindViewById<LinearLayout>(Resource.Id.menu_Share);
                MenuReport = view.FindViewById<LinearLayout>(Resource.Id.menu_Report);

                if (DataObject.IsOwner != null && DataObject.IsOwner.Value)
                {
                    MenuRemove.Visibility = ViewStates.Gone;
                    MenuReport.Visibility = ViewStates.Invisible;
                }

                if (DataObject.Source == "Uploaded")
                {
                    MenuDownload.Visibility = ViewStates.Visible;
                }
                else
                {
                    MenuDownload.Visibility = ViewStates.Gone;
                }

                MenuAddWatchLater.Click += MenuAddWatchLaterOnClick;
                MenuDownload.Click += MenuDownloadOnClick;
                MenuAddPlaylist.Click += MenuAddPlaylistOnClick;
                MenuRemove.Click += MenuRemoveOnClick;
                MenuShare.Click += MenuShareOnClick;
                MenuReport.Click += MenuReportOnClick;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

        private void MenuReportOnClick(object sender, EventArgs e)
        {
            try
            {
                LibrarySynchronizer.AddReportVideo(DataObject);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MenuShareOnClick(object sender, EventArgs e)
        {
            try
            {
                LibrarySynchronizer.ShareVideo(DataObject);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MenuRemoveOnClick(object sender, EventArgs e)
        {
            try
            {
                Listener?.RemoveVideo(DataObject);
                Dismiss(); 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MenuAddPlaylistOnClick(object sender, EventArgs e)
        {
            try
            {
                LibrarySynchronizer.AddToPlaylist(DataObject);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MenuDownloadOnClick(object sender, EventArgs e)
        {
            try
            {
                if (PermissionsController.CheckPermissionStorage())
                {
                    GlobalContext?.VideoActionsController?.DownloadVideo();
                }
                else
                {
                    new PermissionsController(GlobalContext).RequestPermission(100);
                }
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MenuAddWatchLaterOnClick(object sender, EventArgs e)
        {
            try
            {
                LibrarySynchronizer.AddToWatchLater(DataObject);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion
    }
}