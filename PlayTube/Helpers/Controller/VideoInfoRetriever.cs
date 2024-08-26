//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayTube.Helpers.Utils;
using YoutubeExtractor;

namespace PlayTube.Helpers.Controller
{
    public static class VideoInfoRetriever
    {
        private static IEnumerable<YoutubeExtractor.VideoInfo> VideoInfos;
        private static YoutubeExtractor.VideoInfo VideoSelected;

        public static string VideoDownloadstring;


        public class VideoInfo
        {
            public string Quality { get; set; }
            public string Videourl { get; set; }
        }

        public static async Task<List<VideoInfo>> Get_Youtube_Video(string url)
        {
            try
            {
                List < VideoInfo > listVideos = new List<VideoInfo>();
                using var client = new HttpClient();
                var response = await client.GetAsync("https://you-link.herokuapp.com/?url="+ url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                  
                JArray videos = JArray.Parse(json);
                if (videos.Count >= 0)
                {
                    foreach (var key in videos)
                    {
                        var quality = key["quality"].ToString();
                        var videourl = key["url"].ToString();
                        VideoInfo vid = new VideoInfo();
                        vid.Videourl = videourl;
                        vid.Quality = quality;
                        listVideos.Add(vid);
                    }
                        
                    return listVideos;
                }

                return null!;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null!;
            }
        }


        public static async Task<string> GetEmbededVideo(string url)
        {
            try
            {
                if (url.Contains("youtube") || url.Contains("youtu"))
                {
                    VideoInfos =  await DownloadUrlResolver.GetDownloadUrlsAsync(url);

                    var videoInfos = VideoInfos.ToList();
                    if (videoInfos.Count() > 1)
                    {
                        YoutubeExtractor.VideoInfo video =  videoInfos.FirstOrDefault(info => info.VideoType == VideoType.Mp4 && info.Resolution == 720);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 480);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 240);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 144);

                        if (video != null)
                        {
                            if (video.RequiresDecryption)
                                DownloadUrlResolver.DecryptDownloadUrl(video);
                            VideoSelected = video;
                            VideoDownloadstring = video.DownloadUrl;

                            return VideoDownloadstring;
                        }

                        return null!;
                    }

                    var result = await Get_Youtube_Video(url);
                    if (result != null)
                    {
                        VideoDownloadstring = result[0].Videourl;
                        return VideoDownloadstring;
                    }

                    return null!;
                }

                return null!;

            }
            catch (IOException exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return "Invalid";
            }
        }
    }
}