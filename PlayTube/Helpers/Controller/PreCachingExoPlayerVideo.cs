using Android.Content;
using System;
using Com.Google.Android.Exoplayer2.Database;
using Com.Google.Android.Exoplayer2.Upstream.Cache;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using PlayTube.Helpers.Utils;
using PlayTube.MediaPlayers;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;
using Java.Util.Concurrent.Atomic;

namespace PlayTube.Helpers.Controller
{
    public class PreCachingExoPlayerVideo : Object, CacheUtil.IProgressListener
    {
        public static SimpleCache Cache;
        private readonly long ExoPlayerCacheSize = 90 * 1024 * 1024;
        private CacheDataSourceFactory CacheDataSourceFactory;
        private IDataSource xacheDataSource;
        
        public PreCachingExoPlayerVideo(Context context)
        {
            try
            {
                //LeastRecentlyUsedCacheEvictor evictor = new LeastRecentlyUsedCacheEvictor(ExoPlayerCacheSize);
                //var exoDatabaseProvider = new ExoDatabaseProvider(context);
                //Cache = new SimpleCache(context.CacheDir, evictor, exoDatabaseProvider);
                var httpDataSourceFactory = new DefaultHttpDataSourceFactory(Util.GetUserAgent(context, AppSettings.ApplicationName));
                var defaultDataSourceFactory = new DefaultDataSourceFactory(context, httpDataSourceFactory);

                Cache ??= new SimpleCache(context.CacheDir, new NoOpCacheEvictor(), new ExoDatabaseProvider(context));

                CacheDataSourceFactory = new CacheDataSourceFactory(Cache, httpDataSourceFactory);
                xacheDataSource =  CacheDataSourceFactory.CreateDataSource(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        public void CacheVideosFiles(Context context, Uri videoUrl)
        {
            try
            {
                if (!PlayerSettings.EnableOfflineMode)
                    return;
                
                if (videoUrl.Path != null && videoUrl.Path.Contains(".mp4"))
                {
                    CacheUtil.Cache(new DataSpec(videoUrl), Cache, xacheDataSource, this,new AtomicBoolean(false));
                    //new Thread(() =>
                    //{
                    //    try
                    //    {
                    //        int type = Util.InferContentType(videoUrl, videoUrl.Path.Split('.').Last());
                    //        string typeVideo = type switch
                    //        {
                    //            C.TypeSs => DownloadRequest.TypeSs,
                    //            C.TypeDash => DownloadRequest.TypeDash,
                    //            C.TypeHls => DownloadRequest.TypeHls,
                    //            C.TypeOther => DownloadRequest.TypeProgressive,
                    //            _ => DownloadRequest.TypeProgressive
                    //        };

                    //        DownloadManager downloadManager = new DownloadManager(context, new DefaultDatabaseProvider(new ExoDatabaseProvider(context)), Cache, cacheDataSourceFactory);
                    //        downloadManager.AddDownload(new DownloadRequest("id", typeVideo, videoUrl, /* streamKeys= */new List<StreamKey>(), null!, /* data= */ null!));
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Methods.DisplayReportResultTrack(e);
                    //    }
                    //}).Start(); 
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        public void Destroy()
        {
            try
            {
               // Cache = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public class MyCacheKeyFactory : Object, ICacheKeyFactory
        {
            public string Key { set; get; }

            public string BuildCacheKey(DataSpec dataSpec)
            {
                Key = dataSpec.Key;
                return dataSpec.Key;
            }
        }

        public void OnProgress(long requestLength, long bytesCached, long newBytesCached)
        {
            var downloadPercentage = (bytesCached * 100.0 / requestLength);
            Console.WriteLine("downloadPercentage " + downloadPercentage);
        } 
    }
}