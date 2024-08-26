//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using PlayTube.Activities.PlayersView;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using Environment = Android.OS.Environment;

namespace PlayTube.Helpers.Controller
{ 
    public class VideoDownloadAsyncController
    {
        private readonly DownloadManager Downloadmanager;
        private readonly DownloadManager.Request Request;
        public static readonly string FilePath = Android.OS.Environment.DirectoryDownloads + "/" + AppSettings.ApplicationName;
        private readonly string Filename;
        private long DownloadId;
        private string FromActivity;
        private VideoDataObject VideoData;
        private static Activity ActivityContext;

        public VideoDownloadAsyncController(string url, string filename, Activity contextActivity, string fromActivity)
        {
            try
            {
                switch (fromActivity)
                {
                    case "Main":
                        ActivityContext = TabbedMainActivity.GetInstance();
                        break;
                    case "GlobalPlayer":
                        ActivityContext = GlobalPlayerActivity.GetInstance();
                        break;
                    default:
                        ActivityContext = contextActivity;
                        break;
                }

                if (!filename.Contains(".mp4") || !filename.Contains(".Mp4") || !filename.Contains(".MP4"))
                    Filename = filename + ".mp4";
                else
                    Filename = filename;

                Downloadmanager = (DownloadManager)ActivityContext.GetSystemService(Context.DownloadService);
                Request = new DownloadManager.Request(Android.Net.Uri.Parse(url));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void StartDownloadManager(string title, VideoDataObject video, string fromActivity)
        {
            try
            {
                if (video != null && !string.IsNullOrEmpty(title))
                {
                    VideoData = video;
                    FromActivity = fromActivity;

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Insert_WatchOfflineVideos(video);

                    Request.SetTitle(title);
                    Request.SetAllowedNetworkTypes(DownloadNetwork.Mobile | DownloadNetwork.Wifi);

                    Request.SetDestinationInExternalPublicDir(Environment.DirectoryDownloads + "/" + AppSettings.ApplicationName, Filename);

                    Request.SetNotificationVisibility(DownloadVisibility.Visible);
                    Request.SetAllowedOverRoaming(true);
                    DownloadId = Downloadmanager.Enqueue(Request);

                    var onDownloadComplete = new OnDownloadComplete
                    {
                        ActivityContext = ActivityContext,
                        TypeActivity = fromActivity,
                        video = video
                    };

                    ActivityContext.RegisterReceiver(onDownloadComplete, new IntentFilter(DownloadManager.ActionDownloadComplete));
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Download_faileds), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void StopDownloadManager()
        {
            try
            {
                Downloadmanager.Remove(DownloadId);
                RemoveDiskVideoFile(Filename, VideoData.Id);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public bool RemoveDiskVideoFile(string filename, string id)
        {
            try
            {
                string path;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    var directories = Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads + "/" + AppSettings.ApplicationName);
                    path = new Java.IO.File(directories, filename + ".mp4").Path;
                }
                else
                {
                    path = new Java.IO.File(Methods.Path.GetDirectoryDcim() + "/" + Environment.DirectoryDownloads + "/" + AppSettings.ApplicationName, filename + ".mp4").Path;
                }

                if (File.Exists(path))
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Remove_WatchOfflineVideos(id);
                    File.Delete(path);
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }

        public bool CheckDownloadLinkIfExits()
        {
            try
            {
                if (File.Exists(FilePath + Filename))
                    return true;

                return false;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }

        public static string GetDownloadedDiskVideoUri(string filename)
        {
            try
            {
                Java.IO.File file;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    var directories = Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads + "/" + AppSettings.ApplicationName);
                    file = new Java.IO.File(directories, filename + ".mp4");
                }
                else
                {
                    file = new Java.IO.File(Methods.Path.GetDirectoryDcim() + "/" + Environment.DirectoryDownloads + "/" + AppSettings.ApplicationName, filename + ".mp4");
                }

                //Hbh14ktZ3i4frTd  
                if (file.Exists())
                {
                    return file.Path;
                }

                return "";
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return "";
            }
        }

        [BroadcastReceiver(Exported = false)]
        [IntentFilter(new[] { DownloadManager.ActionDownloadComplete })]
        public class OnDownloadComplete : BroadcastReceiver
        {
            public Context ActivityContext;
            public string TypeActivity;
            public VideoDataObject video;

            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    if (intent.Action == DownloadManager.ActionDownloadComplete)
                    {
                        if (ActivityContext == null)
                            return;

                        DownloadManager downloadManagerExcuter = (DownloadManager)ActivityContext.GetSystemService(Context.DownloadService);
                        long downloadId = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);
                        DownloadManager.Query query = new DownloadManager.Query();
                        query.SetFilterById(downloadId);
                        ICursor c = downloadManagerExcuter.InvokeQuery(query);
                        var sqlEntity = new SqLiteDatabase();

                        if (c.MoveToFirst())
                        {
                            int columnIndex = c.GetColumnIndex(DownloadManager.ColumnStatus);
                            if (c.GetInt(columnIndex) == (int)DownloadStatus.Successful)
                            {
                                string downloadedPath = c.GetString(c.GetColumnIndex(DownloadManager.ColumnLocalUri));

                                ActivityManager.RunningAppProcessInfo appProcessInfo = new ActivityManager.RunningAppProcessInfo();
                                ActivityManager.GetMyMemoryState(appProcessInfo);
                                if (appProcessInfo.Importance == Importance.Foreground || appProcessInfo.Importance == Importance.Background)
                                {
                                    sqlEntity.Update_WatchOfflineVideos(video.Id, downloadedPath);
                                    if (TypeActivity == "Main")
                                    {
                                        if (ActivityContext is TabbedMainActivity tabbedMain)
                                        {
                                            tabbedMain.VideoActionsController.DownloadIcon.Tag = "Downloaded";
                                            tabbedMain.VideoActionsController.DownloadIcon.SetImageResource(Resource.Drawable.ic_check_circle);
                                            tabbedMain.VideoActionsController.DownloadIcon.SetColorFilter(Color.Red);
                                            tabbedMain.LibrarySynchronizer.AddToWatchOffline(video);

                                           // tabbedMain.VideoActionsController.ProgressBarDownload.Visibility = ViewStates.Invisible;
                                           // tabbedMain.VideoActionsController.BtnIconDownload.Visibility = ViewStates.Visible;
                                        }
                                        else if (ActivityContext is GlobalPlayerActivity GlobalPlayer)
                                        {
                                            GlobalPlayer.VideoActionsController.DownloadIcon.Tag = "Downloaded";
                                            GlobalPlayer.VideoActionsController.DownloadIcon.SetImageResource(Resource.Drawable.ic_check_circle);
                                            GlobalPlayer.VideoActionsController.DownloadIcon.SetColorFilter(Color.Red);
                                            GlobalPlayer.LibrarySynchronizer.AddToWatchOffline(video);

                                           // GlobalPlayer.VideoActionsController.ProgressBarDownload.Visibility = ViewStates.Invisible;
                                           // GlobalPlayer.VideoActionsController.BtnIconDownload.Visibility = ViewStates.Visible;
                                        }
                                        else if (ActivityContext is FullScreenVideoActivity fullScreen)
                                        {
                                            fullScreen.VideoActionsController.DownloadIcon.SetImageResource(Resource.Drawable.ic_check_circle);
                                            fullScreen.VideoActionsController.DownloadIcon.SetColorFilter(Color.Red);
                                            fullScreen.VideoActionsController.DownloadIcon.Tag = "Downloaded";
                                            //fullScreen tabbedMain.LibrarySynchronizer.OfflineVideoList.Add(Video);
                                        }
                                    }
                                }
                                else
                                {
                                    sqlEntity.Update_WatchOfflineVideos(video.Id, downloadedPath);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            }
        }
    }
}