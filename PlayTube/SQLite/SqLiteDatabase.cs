//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Newtonsoft.Json;
using PlayTube.Activities.Chat;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Messages;
using SQLite;

namespace PlayTube.SQLite
{
    public class SqLiteDatabase
    {
        //############# DON'T MODIFY HERE #############

        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string PathCombine = System.IO.Path.Combine(Folder, AppSettings.DatabaseName + "_.db");
         
        //############# CONNECTION #############

        #region DataBase Functions

        private SQLiteConnection OpenConnection()
        {
            try
            {
               var connection = new SQLiteConnection(PathCombine);
                return connection;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    OpenConnection();
                else
                    Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        public void Connect()
        {
            try
            {
                var connection = OpenConnection();
                connection?.CreateTable<DataTables.MySettingsTb>();
                connection?.CreateTable<DataTables.LoginTb>();
                connection?.CreateTable<DataTables.ChannelTb>();
                connection?.CreateTable<DataTables.WatchOfflineVideosTb>();
                connection?.CreateTable<DataTables.SubscriptionsChannelTb>();
                connection?.CreateTable<DataTables.LibraryItemTb>();
                connection?.CreateTable<DataTables.SharedVideosTb>();
                connection?.CreateTable<DataTables.LastChatTb>();
                connection?.CreateTable<DataTables.MessageTb>();

                //Connection?.Dispose();
                //Connection?.Close();
                AddLibrarySectionViews(); 
            }
            catch (SQLiteException e)
            {
                if (e.Message.Contains("database is locked"))
                    Connect();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }
          
        //Delete table 
        public void DropAll()
        {
            try
            {
                var connection = OpenConnection();
                connection.DropTable<DataTables.MySettingsTb>();
                connection.DropTable<DataTables.LoginTb>();
                connection.DropTable<DataTables.ChannelTb>();
                connection.DropTable<DataTables.WatchOfflineVideosTb>();
                connection.DropTable<DataTables.SubscriptionsChannelTb>();
                connection.DropTable<DataTables.LibraryItemTb>();
                connection.DropTable<DataTables.SharedVideosTb>();
                connection.DropTable<DataTables.LastChatTb>();
                connection.DropTable<DataTables.MessageTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DropAll();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        //############# CONNECTION #############

        #region Settings

        //Insert data Settings
        public void InsertOrUpdate_Settings(GetSettingsObject.SiteSettings data)
        {
            try
            {
                var connection = OpenConnection();
                var resultChannelTb = connection.Table<DataTables.MySettingsTb>().FirstOrDefault();
                if (resultChannelTb == null)
                { 
                    var db = new DataTables.MySettingsTb()
                    {
                        SmtpOrMail = data.SmtpOrMail,
                        SmtpHost = data.SmtpHost,
                        SmtpUsername = data.SmtpUsername,
                        SmtpPassword = data.SmtpPassword,
                        SmtpEncryption = data.SmtpEncryption,
                        SmtpPort = data.SmtpPort,
                        Theme = data.Theme,
                        LinkedinLogin = data.LinkedinLogin,
                        VkontakteLogin = data.VkontakteLogin,
                        FacebookAppId = data.FacebookAppId,
                        FacebookAppKey = data.FacebookAppKey,
                        GoogleAppId = data.GoogleAppId,
                        GoogleAppKey = data.GoogleAppKey,
                        TwitterAppId = data.TwitterAppId,
                        TwitterAppKey = data.TwitterAppKey,
                        LinkedinAppId = data.LinkedinAppId,
                        LinkedinAppKey = data.LinkedinAppKey,
                        VkontakteAppId = data.VkontakteAppId,
                        VkontakteAppKey = data.VkontakteAppKey,
                        InstagramAppId = data.InstagramAppId,
                        InstagramAppkey = data.InstagramAppkey,
                        InstagramLogin = data.InstagramLogin,
                        PaypalId = data.PaypalId,
                        PaypalSecret = data.PaypalSecret,
                        PaypalMode = data.PaypalMode,
                        LastBackup = data.LastBackup,
                        AmazoneS3Key = data.AmazoneS3Key,
                        AmazoneS3SKey = data.AmazoneS3SKey,
                        Region = data.Region,
                        StripeSecret = data.StripeSecret,
                        StripeId = data.StripeId,
                        PushId = data.PushId,
                        PushKey = data.PushKey,
                        DeleteAccount = data.DeleteAccount,
                        UserRegistration = data.UserRegistration,
                        MaxUpload = data.MaxUpload,
                        WowonderLogin = data.WowonderLogin,
                        WowonderAppId = data.WowonderAppId,
                        WowonderAppKey = data.WowonderAppKey,
                        WowonderDomainUri = data.WowonderDomainUri,
                        BankTransferNote = data.BankTransferNote,
                        PayseraProjectId = data.PayseraProjectId,
                        MaintenanceMode = data.MaintenanceMode,
                        BankDescription = data.BankDescription,
                        Version = data.Version,
                        Push = data.Push,
                        YtApi = data.YtApi,
                        Seo = data.Seo,
                        CheckoutPayment = data.CheckoutPayment,
                        CheckoutMode = data.CheckoutMode,
                        CheckoutCurrency = data.CheckoutCurrency,
                        CheckoutSellerId = data.CheckoutSellerId,
                        CheckoutPublishableKey = data.CheckoutPublishableKey,
                        CheckoutPrivateKey = data.CheckoutPrivateKey,
                        CashfreePayment = data.CashfreePayment,
                        CashfreeMode = data.CashfreeMode,
                        CashfreeClientKey = data.CashfreeClientKey,
                        CashfreeSecretKey = data.CashfreeSecretKey,
                        IyzipayPayment = data.IyzipayPayment,
                        IyzipayMode = data.IyzipayMode,
                        IyzipayKey = data.IyzipayKey,
                        IyzipayBuyerId = data.IyzipayBuyerId,
                        IyzipaySecretKey = data.IyzipaySecretKey,
                        IyzipayBuyerName = data.IyzipayBuyerName,
                        IyzipayBuyerSurname = data.IyzipayBuyerSurname,
                        IyzipayBuyerGsmNumber = data.IyzipayBuyerGsmNumber,
                        IyzipayBuyerEmail = data.IyzipayBuyerEmail,
                        IyzipayIdentityNumber = data.IyzipayIdentityNumber,
                        IyzipayAddress = data.IyzipayAddress,
                        IyzipayCity = data.IyzipayCity,
                        IyzipayCountry = data.IyzipayCountry,
                        IyzipayZip = data.IyzipayZip,
                        PayuPayment = data.PayuPayment,
                        PayuMode = data.PayuMode,
                        PayuMerchantId = data.PayuMerchantId,
                        PayuSecretKey = data.PayuSecretKey,
                        PayuBuyerName = data.PayuBuyerName,
                        PayuBuyerSurname = data.PayuBuyerSurname,
                        PayuBuyerGsmNumber = data.PayuBuyerGsmNumber,
                        PayuBuyerEmail = data.PayuBuyerEmail,
                        PreventSystem = data.PreventSystem,
                        BadLoginLimit = data.BadLoginLimit,
                        LockTime = data.LockTime,
                        PaystackPayment = data.PaystackPayment,
                        PaystackSecretKey = data.PaystackSecretKey,
                        LiveVideo = data.LiveVideo,
                        LiveVideoSave = data.LiveVideoSave,
                        AgoraLiveVideo = data.AgoraLiveVideo,
                        AgoraAppId = data.AgoraAppId,
                        AgoraAppCertificate = data.AgoraAppCertificate,
                        AgoraCustomerId = data.AgoraCustomerId,
                        AgoraCustomerCertificate = data.AgoraCustomerCertificate,
                        AmazoneS32 = data.AmazoneS32,
                        BucketName2 = data.BucketName2,
                        AmazoneS3Key2 = data.AmazoneS3Key2,
                        AmazoneS3SKey2 = data.AmazoneS3SKey2,
                        Region2 = data.Region2,
                        QqAppId = data.QqAppId,
                        QqAppkey = data.QqAppkey,
                        WeChatAppId = data.WeChatAppId,
                        WeChatAppkey = data.WeChatAppkey,
                        DiscordAppId = data.DiscordAppId,
                        DiscordAppkey = data.DiscordAppkey,
                        MailruAppId = data.MailruAppId,
                        MailruAppkey = data.MailruAppkey,
                        QqLogin = data.QqLogin,
                        DiscordLogin = data.DiscordLogin,
                        MailruLogin = data.MailruLogin,
                        AuthorizePayment = data.AuthorizePayment,
                        AuthorizeLoginId = data.AuthorizeLoginId,
                        AuthorizeTransactionKey = data.AuthorizeTransactionKey,
                        AuthorizeTestMode = data.AuthorizeTestMode,
                        SecurionpayPayment = data.SecurionpayPayment,
                        SecurionpayPublicKey = data.SecurionpayPublicKey,
                        SecurionpaySecretKey = data.SecurionpaySecretKey,
                        InviteLinksSystem = data.InviteLinksSystem,
                        UserLinksLimit = data.UserLinksLimit,
                        ExpireUserLinks = data.ExpireUserLinks,
                        Name = data.Name,
                        AamarpayMode = data.AamarpayMode,
                        AamarpayPayment = data.AamarpayPayment,
                        AamarpaySignatureKey = data.AamarpaySignatureKey,
                        AamarpayStoreId = data.AamarpayStoreId,
                        AdCPrice = data.AdCPrice,
                        AdminComRentVideos = data.AdminComRentVideos,
                        AdminComSellVideos = data.AdminComSellVideos,
                        AdminComSubscribers = data.AdminComSubscribers,
                        AllCreateArticles = data.AllCreateArticles,
                        ApproveVideos = data.ApproveVideos,
                        AppsApiId = data.AppsApiId,
                        AppsApiKey = data.AppsApiKey,
                        ArticleSystem = data.ArticleSystem,
                        AutoApprove = data.AutoApprove,
                        AutoSubscribe = data.AutoSubscribe,
                        AutoUsername = data.AutoUsername,
                        AutoplaySystem = data.AutoplaySystem,
                        BankPayment = data.BankPayment,
                        BlockSystem = data.BlockSystem,
                        AdVPrice = data.AdVPrice,
                        CensoredWords = data.CensoredWords,
                        CoinbaseKey = data.CoinbaseKey,
                        CoinbasePayment = data.CoinbasePayment,
                        Coinpayments = data.Coinpayments,
                        CoinpaymentsCoin = data.CoinpaymentsCoin,
                        CoinpaymentsCoins = data.CoinpaymentsCoins,
                        CoinpaymentsPublicKey = data.CoinpaymentsPublicKey,
                        CoinpaymentsSecret = data.CoinpaymentsSecret,
                        ComType = data.ComType,
                        CommentSystem = data.CommentSystem,
                        CommentsDefaultNum = data.CommentsDefaultNum,
                        CommentsPoint = data.CommentsPoint,
                        ConvertSpeed = data.ConvertSpeed,
                        CreditCard = data.CreditCard,
                        DailymotionId = data.DailymotionId,
                        DateStyle = data.DateStyle,
                        DemoVideo = data.DemoVideo,
                        Description = data.Description,
                        DislikesPoint = data.DislikesPoint,
                        DollarToPointCost = data.DollarToPointCost,
                        DonateSystem = data.DonateSystem,
                        DownloadVideos = data.DownloadVideos,
                        EarnFromAds = data.EarnFromAds,
                        Email = data.Email,
                        EmbedSystem = data.EmbedSystem,
                        FacebookImport = data.FacebookImport,
                        FavCategory = data.FavCategory,
                        FbLogin = data.FbLogin,
                        FfmpegBinaryFile = data.FfmpegBinaryFile,
                        FfmpegSystem = data.FfmpegSystem,
                        FortumoPayment = data.FortumoPayment,
                        FortumoServiceId = data.FortumoServiceId,
                        FreeDayLimit = data.FreeDayLimit,
                        FtpEndpoint = data.FtpEndpoint,
                        FtpHost = data.FtpHost,
                        FtpPassword = data.FtpPassword,
                        FtpPath = data.FtpPath,
                        FtpPort = data.FtpPort,
                        FtpUpload = data.FtpUpload,
                        FtpUsername = data.FtpUsername,
                        GeoBlocking = data.GeoBlocking,
                        GifSystem = data.GifSystem,
                        GoPro = data.GoPro,
                        Google = data.Google,
                        HistorySystem = data.HistorySystem,
                        Hostname = data.Hostname,
                        ImportSystem = data.ImportSystem,
                        IsOk = data.IsOk,
                        Keyword = data.Keyword,
                        LangModal = data.LangModal,
                        Language = data.Language,
                        LastAdminCollection = data.LastAdminCollection,
                        LastCreatedSitemap = data.LastCreatedSitemap,
                        LikesPoint = data.LikesPoint,
                        MainCurrency = data.MainCurrency,
                        MaxUploadAllUsers = data.MaxUploadAllUsers,
                        MaxUploadFreeUsers = data.MaxUploadFreeUsers,
                        MaxUploadProUsers = data.MaxUploadProUsers,
                        MoviesVideos = data.MoviesVideos,
                        NgeniusApiKey = data.NgeniusApiKey,
                        NgeniusMode = data.NgeniusMode,
                        NgeniusOutletId = data.NgeniusOutletId,
                        NgeniusPayment = data.NgeniusPayment,
                        NightMode = data.NightMode,
                        OkImport = data.OkImport,
                        P1080 = data.P1080,
                        P2048 = data.P2048,
                        P240 = data.P240,
                        P360 = data.P360,
                        P4096 = data.P4096,
                        P480 = data.P480,
                        P720 = data.P720,
                        PasswordComplexitySystem = data.PasswordComplexitySystem,
                        PayedSubscribers = data.PayedSubscribers,
                        PaymentCurrency = data.PaymentCurrency,
                        Paypal = data.Paypal,
                        PaypalCurrency = data.PaypalCurrency,
                        PayseraMode = data.PayseraMode,
                        PayseraPayment = data.PayseraPayment,
                        PayseraSignPassword = data.PayseraSignPassword,
                        PlayerType = data.PlayerType,
                        PlaylistSubscribe = data.PlaylistSubscribe,
                        PlusLogin = data.PlusLogin,
                        PointAllowWithdrawal = data.PointAllowWithdrawal,
                        PointLevelSystem = data.PointLevelSystem,
                        PopUp18 = data.PopUp18,
                        PopularChannels = data.PopularChannels,
                        PostSystem = data.PostSystem,
                        ProDayLimit = data.ProDayLimit,
                        ProGoogle = data.ProGoogle,
                        ProPkgPrice = data.ProPkgPrice,
                        PubPrice = data.PubPrice,
                        QueueCount = data.QueueCount,
                        RazorpayKeyId = data.RazorpayKeyId,
                        RazorpayKeySecret = data.RazorpayKeySecret,
                        RazorpayPayment = data.RazorpayPayment,
                        Recaptcha = data.Recaptcha,
                        RecaptchaKey = data.RecaptchaKey,
                        RememberDevice = data.RememberDevice,
                        RentVideosSystem = data.RentVideosSystem,
                        ReportCopyright = data.ReportCopyright,
                        RequireLogin = data.RequireLogin,
                        RequireSubcription = data.RequireSubcription,
                        ResizeVideo = data.ResizeVideo,
                        RestrictEmbeddingSystem = data.RestrictEmbeddingSystem,
                        RssExport = data.RssExport,
                        S3BucketName = data.S3BucketName,
                        S3Upload = data.S3Upload,
                        ScriptVersion = data.ScriptVersion,
                        SellVideosSystem = data.SellVideosSystem,
                        SeoLink = data.SeoLink,
                        Server = data.Server,
                        ServerKey = data.ServerKey,
                        ServerPort = data.ServerPort,
                        ShortsDuration = data.ShortsDuration,
                        ShortsSystem = data.ShortsSystem,
                        ShowArticles = data.ShowArticles,
                        SiteUrl = data.SiteUrl,
                        SpaceName = data.SpaceName,
                        SpaceRegion = data.SpaceRegion,
                        Spaces = data.Spaces,
                        SpacesKey = data.SpacesKey,
                        SpacesSecret = data.SpacesSecret,
                        StickyVideo = data.StickyVideo,
                        StockVideos = data.StockVideos,
                        StripeCurrency = data.StripeCurrency,
                        ThemeUrl = data.ThemeUrl,
                        Time18 = data.Time18,
                        Title = data.Title,
                        TotalComments = data.TotalComments,
                        TotalDislikes = data.TotalDislikes,
                        TotalLikes = data.TotalLikes,
                        TotalSaved = data.TotalSaved,
                        TotalSubs = data.TotalSubs,
                        TotalUsers = data.TotalUsers,
                        TotalVideos = data.TotalVideos,
                        TotalViews = data.TotalViews,
                        TrailerSystem = data.TrailerSystem,
                        TwLogin = data.TwLogin,
                        TwitchApi = data.TwitchApi,
                        TwitchImport = data.TwitchImport,
                        TwoFactorSetting = data.TwoFactorSetting,
                        UploadPoint = data.UploadPoint,
                        UploadSystem = data.UploadSystem,
                        UploadSystemType = data.UploadSystemType,
                        UserAds = data.UserAds,
                        UserMaxImport = data.UserMaxImport,
                        UserMaxUpload = data.UserMaxUpload,
                        UserMonApprove = data.UserMonApprove,
                        UserStatics = data.UserStatics,
                        UsrVMon = data.UsrVMon,
                        Validation = data.Validation,
                        VerificationBadge = data.VerificationBadge,
                        VideoTextSystem = data.VideoTextSystem,
                        VideosLoadLimit = data.VideosLoadLimit,
                        VideosStatics = data.VideosStatics,
                        VideosUploadLimit = data.VideosUploadLimit,
                        VimeoApiKey = data.VimeoApiKey,
                        WasabiAccessKey = data.WasabiAccessKey,
                        WasabiBucketName = data.WasabiBucketName,
                        WasabiBucketRegion = data.WasabiBucketRegion,
                        WasabiSecretKey = data.WasabiSecretKey,
                        WasabiStorage = data.WasabiStorage,
                        WatchingPoint = data.WatchingPoint,
                        Watermark = data.Watermark,
                        WechatLogin = data.WechatLogin,
                        WhoSell = data.WhoSell,
                        WhoUpload = data.WhoUpload,
                        WhoUseLive = data.WhoUseLive,
                        WowonderImg = data.WowonderImg,
                        WowonderSiteName = data.WowonderSiteName,
                        YoomoneyNotificationsSecret = data.YoomoneyNotificationsSecret,
                        YoomoneyPayment = data.YoomoneyPayment,
                        YoomoneyWalletId = data.YoomoneyWalletId,
                        AmazonEndpoint = data.AmazonEndpoint, 
                        BackblazeAccessKey = data.BackblazeAccessKey, 
                        BackblazeAccessKeyId = data.BackblazeAccessKeyId, 
                        BackblazeBucketId = data.BackblazeBucketId, 
                        BackblazeBucketName = data.BackblazeBucketName, 
                        BackblazeEndpoint = data.BackblazeEndpoint, 
                        BackblazeRegion = data.BackblazeRegion, 
                        BackblazeStorage = data.BackblazeStorage, 
                        ChunkSize = data.ChunkSize, 
                        CronjobLastRun = data.CronjobLastRun, 
                        DeveloperMode = data.DeveloperMode,
                        LogoCache = data.LogoCache,
                        MWithdrawal = data.MWithdrawal,
                        MainPaymentCurrency = data.MainPaymentCurrency,
                        SpacesEndpoint = data.SpacesEndpoint,
                        TiktokImport = data.TiktokImport,
                        TwitchAccessToken = data.TwitchAccessToken,
                        WasabiEndpoint = data.WasabiEndpoint,
                        YoutubePlayer = data.YoutubePlayer,
                    };

                    db.CurrencyArray = JsonConvert.SerializeObject(data.CurrencyArray);
                    db.CurrencySymbolArray = JsonConvert.SerializeObject(data.CurrencySymbolArray);
                    db.Continents = JsonConvert.SerializeObject(data.Continents);
                    db.Categories = JsonConvert.SerializeObject(data.Categories);
                    if (data.SubCategories != null)
                        db.SubCategories = JsonConvert.SerializeObject(data.SubCategories?.SubCategoriessList);
                    db.MoviesCategories = JsonConvert.SerializeObject(data.MoviesCategories);
                    db.ImportSystem = data.ImportSystem;
                    db.UploadSystem = data.UploadSystem;

                    connection.Insert(db);
                }
                else
                {

                    resultChannelTb.SmtpOrMail = data.SmtpOrMail;
                    resultChannelTb.SmtpHost = data.SmtpHost;
                    resultChannelTb.SmtpUsername = data.SmtpUsername;
                    resultChannelTb.SmtpPassword = data.SmtpPassword;
                    resultChannelTb.SmtpEncryption = data.SmtpEncryption;
                    resultChannelTb.SmtpPort = data.SmtpPort;
                    resultChannelTb.Theme = data.Theme;
                    resultChannelTb.LinkedinLogin = data.LinkedinLogin;
                    resultChannelTb.VkontakteLogin = data.VkontakteLogin;
                    resultChannelTb.FacebookAppId = data.FacebookAppId;
                    resultChannelTb.FacebookAppKey = data.FacebookAppKey;
                    resultChannelTb.GoogleAppId = data.GoogleAppId;
                    resultChannelTb.GoogleAppKey = data.GoogleAppKey;
                    resultChannelTb.TwitterAppId = data.TwitterAppId;
                    resultChannelTb.TwitterAppKey = data.TwitterAppKey;
                    resultChannelTb.LinkedinAppId = data.LinkedinAppId;
                    resultChannelTb.LinkedinAppKey = data.LinkedinAppKey;
                    resultChannelTb.VkontakteAppId = data.VkontakteAppId;
                    resultChannelTb.VkontakteAppKey = data.VkontakteAppKey;
                    resultChannelTb.InstagramAppId = data.InstagramAppId;
                    resultChannelTb.InstagramAppkey = data.InstagramAppkey;
                    resultChannelTb.InstagramLogin = data.InstagramLogin;
                    resultChannelTb.PaypalId = data.PaypalId;
                    resultChannelTb.PaypalSecret = data.PaypalSecret;
                    resultChannelTb.PaypalMode = data.PaypalMode;
                    resultChannelTb.LastBackup = data.LastBackup;
                    resultChannelTb.AmazoneS3Key = data.AmazoneS3Key;
                    resultChannelTb.AmazoneS3SKey = data.AmazoneS3SKey;
                    resultChannelTb.Region = data.Region;
                    resultChannelTb.StripeSecret = data.StripeSecret;
                    resultChannelTb.StripeId = data.StripeId;
                    resultChannelTb.PushId = data.PushId;
                    resultChannelTb.PushKey = data.PushKey;
                    resultChannelTb.DeleteAccount = data.DeleteAccount;
                    resultChannelTb.UserRegistration = data.UserRegistration;
                    resultChannelTb.MaxUpload = data.MaxUpload;
                    resultChannelTb.WowonderLogin = data.WowonderLogin;
                    resultChannelTb.WowonderAppId = data.WowonderAppId;
                    resultChannelTb.WowonderAppKey = data.WowonderAppKey;
                    resultChannelTb.WowonderDomainUri = data.WowonderDomainUri;
                    resultChannelTb.BankTransferNote = data.BankTransferNote;
                    resultChannelTb.PayseraProjectId = data.PayseraProjectId;
                    resultChannelTb.MaintenanceMode = data.MaintenanceMode;
                    resultChannelTb.BankDescription = data.BankDescription;
                    resultChannelTb.Version = data.Version;
                    resultChannelTb.Push = data.Push;
                    resultChannelTb.YtApi = data.YtApi;
                    resultChannelTb.Seo = data.Seo;
                    resultChannelTb.CheckoutPayment = data.CheckoutPayment;
                    resultChannelTb.CheckoutMode = data.CheckoutMode;
                    resultChannelTb.CheckoutCurrency = data.CheckoutCurrency;
                    resultChannelTb.CheckoutSellerId = data.CheckoutSellerId;
                    resultChannelTb.CheckoutPublishableKey = data.CheckoutPublishableKey;
                    resultChannelTb.CheckoutPrivateKey = data.CheckoutPrivateKey;
                    resultChannelTb.CashfreePayment = data.CashfreePayment;
                    resultChannelTb.CashfreeMode = data.CashfreeMode;
                    resultChannelTb.CashfreeClientKey = data.CashfreeClientKey;
                    resultChannelTb.CashfreeSecretKey = data.CashfreeSecretKey;
                    resultChannelTb.IyzipayPayment = data.IyzipayPayment;
                    resultChannelTb.IyzipayMode = data.IyzipayMode;
                    resultChannelTb.IyzipayKey = data.IyzipayKey;
                    resultChannelTb.IyzipayBuyerId = data.IyzipayBuyerId;
                    resultChannelTb.IyzipaySecretKey = data.IyzipaySecretKey;
                    resultChannelTb.IyzipayBuyerName = data.IyzipayBuyerName;
                    resultChannelTb.IyzipayBuyerSurname = data.IyzipayBuyerSurname;
                    resultChannelTb.IyzipayBuyerGsmNumber = data.IyzipayBuyerGsmNumber;
                    resultChannelTb.IyzipayBuyerEmail = data.IyzipayBuyerEmail;
                    resultChannelTb.IyzipayIdentityNumber = data.IyzipayIdentityNumber;
                    resultChannelTb.IyzipayAddress = data.IyzipayAddress;
                    resultChannelTb.IyzipayCity = data.IyzipayCity;
                    resultChannelTb.IyzipayCountry = data.IyzipayCountry;
                    resultChannelTb.IyzipayZip = data.IyzipayZip;
                    resultChannelTb.PayuPayment = data.PayuPayment;
                    resultChannelTb.PayuMode = data.PayuMode;
                    resultChannelTb.PayuMerchantId = data.PayuMerchantId;
                    resultChannelTb.PayuSecretKey = data.PayuSecretKey;
                    resultChannelTb.PayuBuyerName = data.PayuBuyerName;
                    resultChannelTb.PayuBuyerSurname = data.PayuBuyerSurname;
                    resultChannelTb.PayuBuyerGsmNumber = data.PayuBuyerGsmNumber;
                    resultChannelTb.PayuBuyerEmail = data.PayuBuyerEmail;
                    resultChannelTb.PreventSystem = data.PreventSystem;
                    resultChannelTb.BadLoginLimit = data.BadLoginLimit;
                    resultChannelTb.LockTime = data.LockTime;
                    resultChannelTb.PaystackPayment = data.PaystackPayment;
                    resultChannelTb.PaystackSecretKey = data.PaystackSecretKey;
                    resultChannelTb.LiveVideo = data.LiveVideo;
                    resultChannelTb.LiveVideoSave = data.LiveVideoSave;
                    resultChannelTb.AgoraLiveVideo = data.AgoraLiveVideo;
                    resultChannelTb.AgoraAppId = data.AgoraAppId;
                    resultChannelTb.AgoraAppCertificate = data.AgoraAppCertificate;
                    resultChannelTb.AgoraCustomerId = data.AgoraCustomerId;
                    resultChannelTb.AgoraCustomerCertificate = data.AgoraCustomerCertificate;
                    resultChannelTb.AmazoneS32 = data.AmazoneS32;
                    resultChannelTb.BucketName2 = data.BucketName2;
                    resultChannelTb.AmazoneS3Key2 = data.AmazoneS3Key2;
                    resultChannelTb.AmazoneS3SKey2 = data.AmazoneS3SKey2;
                    resultChannelTb.Region2 = data.Region2;
                    resultChannelTb.QqAppId = data.QqAppId;
                    resultChannelTb.QqAppkey = data.QqAppkey;
                    resultChannelTb.WeChatAppId = data.WeChatAppId;
                    resultChannelTb.WeChatAppkey = data.WeChatAppkey;
                    resultChannelTb.DiscordAppId = data.DiscordAppId;
                    resultChannelTb.DiscordAppkey = data.DiscordAppkey;
                    resultChannelTb.MailruAppId = data.MailruAppId;
                    resultChannelTb.MailruAppkey = data.MailruAppkey;
                    resultChannelTb.QqLogin = data.QqLogin;
                    resultChannelTb.DiscordLogin = data.DiscordLogin;
                    resultChannelTb.MailruLogin = data.MailruLogin;
                    resultChannelTb.AuthorizePayment = data.AuthorizePayment;
                    resultChannelTb.AuthorizeLoginId = data.AuthorizeLoginId;
                    resultChannelTb.AuthorizeTransactionKey = data.AuthorizeTransactionKey;
                    resultChannelTb.AuthorizeTestMode = data.AuthorizeTestMode;
                    resultChannelTb.SecurionpayPayment = data.SecurionpayPayment;
                    resultChannelTb.SecurionpayPublicKey = data.SecurionpayPublicKey;
                    resultChannelTb.SecurionpaySecretKey = data.SecurionpaySecretKey;
                    resultChannelTb.InviteLinksSystem = data.InviteLinksSystem;
                    resultChannelTb.UserLinksLimit = data.UserLinksLimit;
                    resultChannelTb.ExpireUserLinks = data.ExpireUserLinks;
                    resultChannelTb.Name = data.Name;
                    resultChannelTb.AamarpayMode = data.AamarpayMode;
                    resultChannelTb.AamarpayPayment = data.AamarpayPayment;
                    resultChannelTb.AamarpaySignatureKey = data.AamarpaySignatureKey;
                    resultChannelTb.AamarpayStoreId = data.AamarpayStoreId;
                    resultChannelTb.AdCPrice = data.AdCPrice;
                    resultChannelTb.AdminComRentVideos = data.AdminComRentVideos;
                    resultChannelTb.AdminComSellVideos = data.AdminComSellVideos;
                    resultChannelTb.AdminComSubscribers = data.AdminComSubscribers;
                    resultChannelTb.AllCreateArticles = data.AllCreateArticles;
                    resultChannelTb.ApproveVideos = data.ApproveVideos;
                    resultChannelTb.AppsApiId = data.AppsApiId;
                    resultChannelTb.AppsApiKey = data.AppsApiKey;
                    resultChannelTb.ArticleSystem = data.ArticleSystem;
                    resultChannelTb.AutoApprove = data.AutoApprove;
                    resultChannelTb.AutoSubscribe = data.AutoSubscribe;
                    resultChannelTb.AutoUsername = data.AutoUsername;
                    resultChannelTb.AutoplaySystem = data.AutoplaySystem;
                    resultChannelTb.BankPayment = data.BankPayment;
                    resultChannelTb.BlockSystem = data.BlockSystem;
                    resultChannelTb.AdVPrice = data.AdVPrice;
                    resultChannelTb.CensoredWords = data.CensoredWords;
                    resultChannelTb.CoinbaseKey = data.CoinbaseKey;
                    resultChannelTb.CoinbasePayment = data.CoinbasePayment;
                    resultChannelTb.Coinpayments = data.Coinpayments;
                    resultChannelTb.CoinpaymentsCoin = data.CoinpaymentsCoin;
                    resultChannelTb.CoinpaymentsCoins = data.CoinpaymentsCoins;
                    resultChannelTb.CoinpaymentsPublicKey = data.CoinpaymentsPublicKey;
                    resultChannelTb.CoinpaymentsSecret = data.CoinpaymentsSecret;
                    resultChannelTb.ComType = data.ComType;
                    resultChannelTb.CommentSystem = data.CommentSystem;
                    resultChannelTb.CommentsDefaultNum = data.CommentsDefaultNum;
                    resultChannelTb.CommentsPoint = data.CommentsPoint;
                    resultChannelTb.ConvertSpeed = data.ConvertSpeed;
                    resultChannelTb.CreditCard = data.CreditCard;
                    resultChannelTb.DailymotionId = data.DailymotionId;
                    resultChannelTb.DateStyle = data.DateStyle;
                    resultChannelTb.DemoVideo = data.DemoVideo;
                    resultChannelTb.Description = data.Description;
                    resultChannelTb.DislikesPoint = data.DislikesPoint;
                    resultChannelTb.DollarToPointCost = data.DollarToPointCost;
                    resultChannelTb.DonateSystem = data.DonateSystem;
                    resultChannelTb.DownloadVideos = data.DownloadVideos;
                    resultChannelTb.EarnFromAds = data.EarnFromAds;
                    resultChannelTb.Email = data.Email;
                    resultChannelTb.EmbedSystem = data.EmbedSystem;
                    resultChannelTb.FacebookImport = data.FacebookImport;
                    resultChannelTb.FavCategory = data.FavCategory;
                    resultChannelTb.FbLogin = data.FbLogin;
                    resultChannelTb.FfmpegBinaryFile = data.FfmpegBinaryFile;
                    resultChannelTb.FfmpegSystem = data.FfmpegSystem;
                    resultChannelTb.FortumoPayment = data.FortumoPayment;
                    resultChannelTb.FortumoServiceId = data.FortumoServiceId;
                    resultChannelTb.FreeDayLimit = data.FreeDayLimit;
                    resultChannelTb.FtpEndpoint = data.FtpEndpoint;
                    resultChannelTb.FtpHost = data.FtpHost;
                    resultChannelTb.FtpPassword = data.FtpPassword;
                    resultChannelTb.FtpPath = data.FtpPath;
                    resultChannelTb.FtpPort = data.FtpPort;
                    resultChannelTb.FtpUpload = data.FtpUpload;
                    resultChannelTb.FtpUsername = data.FtpUsername;
                    resultChannelTb.GeoBlocking = data.GeoBlocking;
                    resultChannelTb.GifSystem = data.GifSystem;
                    resultChannelTb.GoPro = data.GoPro;
                    resultChannelTb.Google = data.Google;
                    resultChannelTb.HistorySystem = data.HistorySystem;
                    resultChannelTb.Hostname = data.Hostname;
                    resultChannelTb.ImportSystem = data.ImportSystem;
                    resultChannelTb.IsOk = data.IsOk;
                    resultChannelTb.Keyword = data.Keyword;
                    resultChannelTb.LangModal = data.LangModal;
                    resultChannelTb.Language = data.Language;
                    resultChannelTb.LastAdminCollection = data.LastAdminCollection;
                    resultChannelTb.LastCreatedSitemap = data.LastCreatedSitemap;
                    resultChannelTb.LikesPoint = data.LikesPoint;
                    resultChannelTb.MainCurrency = data.MainCurrency;
                    resultChannelTb.MaxUploadAllUsers = data.MaxUploadAllUsers;
                    resultChannelTb.MaxUploadFreeUsers = data.MaxUploadFreeUsers;
                    resultChannelTb.MaxUploadProUsers = data.MaxUploadProUsers;
                    resultChannelTb.MoviesVideos = data.MoviesVideos;
                    resultChannelTb.NgeniusApiKey = data.NgeniusApiKey;
                    resultChannelTb.NgeniusMode = data.NgeniusMode;
                    resultChannelTb.NgeniusOutletId = data.NgeniusOutletId;
                    resultChannelTb.NgeniusPayment = data.NgeniusPayment;
                    resultChannelTb.NightMode = data.NightMode;
                    resultChannelTb.OkImport = data.OkImport;
                    resultChannelTb.P1080 = data.P1080;
                    resultChannelTb.P2048 = data.P2048;
                    resultChannelTb.P240 = data.P240;
                    resultChannelTb.P360 = data.P360;
                    resultChannelTb.P4096 = data.P4096;
                    resultChannelTb.P480 = data.P480;
                    resultChannelTb.P720 = data.P720;
                    resultChannelTb.PasswordComplexitySystem = data.PasswordComplexitySystem;
                    resultChannelTb.PayedSubscribers = data.PayedSubscribers;
                    resultChannelTb.PaymentCurrency = data.PaymentCurrency;
                    resultChannelTb.Paypal = data.Paypal;
                    resultChannelTb.PaypalCurrency = data.PaypalCurrency;
                    resultChannelTb.PayseraMode = data.PayseraMode;
                    resultChannelTb.PayseraPayment = data.PayseraPayment;
                    resultChannelTb.PayseraSignPassword = data.PayseraSignPassword;
                    resultChannelTb.PlayerType = data.PlayerType;
                    resultChannelTb.PlaylistSubscribe = data.PlaylistSubscribe;
                    resultChannelTb.PlusLogin = data.PlusLogin;
                    resultChannelTb.PointAllowWithdrawal = data.PointAllowWithdrawal;
                    resultChannelTb.PointLevelSystem = data.PointLevelSystem;
                    resultChannelTb.PopUp18 = data.PopUp18;
                    resultChannelTb.PopularChannels = data.PopularChannels;
                    resultChannelTb.PostSystem = data.PostSystem;
                    resultChannelTb.ProDayLimit = data.ProDayLimit;
                    resultChannelTb.ProGoogle = data.ProGoogle;
                    resultChannelTb.ProPkgPrice = data.ProPkgPrice;
                    resultChannelTb.PubPrice = data.PubPrice;
                    resultChannelTb.QueueCount = data.QueueCount;
                    resultChannelTb.RazorpayKeyId = data.RazorpayKeyId;
                    resultChannelTb.RazorpayKeySecret = data.RazorpayKeySecret;
                    resultChannelTb.RazorpayPayment = data.RazorpayPayment;
                    resultChannelTb.Recaptcha = data.Recaptcha;
                    resultChannelTb.RecaptchaKey = data.RecaptchaKey;
                    resultChannelTb.RememberDevice = data.RememberDevice;
                    resultChannelTb.RentVideosSystem = data.RentVideosSystem;
                    resultChannelTb.ReportCopyright = data.ReportCopyright;
                    resultChannelTb.RequireLogin = data.RequireLogin;
                    resultChannelTb.RequireSubcription = data.RequireSubcription;
                    resultChannelTb.ResizeVideo = data.ResizeVideo;
                    resultChannelTb.RestrictEmbeddingSystem = data.RestrictEmbeddingSystem;
                    resultChannelTb.RssExport = data.RssExport;
                    resultChannelTb.S3BucketName = data.S3BucketName;
                    resultChannelTb.S3Upload = data.S3Upload;
                    resultChannelTb.ScriptVersion = data.ScriptVersion;
                    resultChannelTb.SellVideosSystem = data.SellVideosSystem;
                    resultChannelTb.SeoLink = data.SeoLink;
                    resultChannelTb.Server = data.Server;
                    resultChannelTb.ServerKey = data.ServerKey;
                    resultChannelTb.ServerPort = data.ServerPort;
                    resultChannelTb.ShortsDuration = data.ShortsDuration;
                    resultChannelTb.ShortsSystem = data.ShortsSystem;
                    resultChannelTb.ShowArticles = data.ShowArticles;
                    resultChannelTb.SiteUrl = data.SiteUrl;
                    resultChannelTb.SpaceName = data.SpaceName;
                    resultChannelTb.SpaceRegion = data.SpaceRegion;
                    resultChannelTb.Spaces = data.Spaces;
                    resultChannelTb.SpacesKey = data.SpacesKey;
                    resultChannelTb.SpacesSecret = data.SpacesSecret;
                    resultChannelTb.StickyVideo = data.StickyVideo;
                    resultChannelTb.StockVideos = data.StockVideos;
                    resultChannelTb.StripeCurrency = data.StripeCurrency;
                    resultChannelTb.ThemeUrl = data.ThemeUrl;
                    resultChannelTb.Time18 = data.Time18;
                    resultChannelTb.Title = data.Title;
                    resultChannelTb.TotalComments = data.TotalComments;
                    resultChannelTb.TotalDislikes = data.TotalDislikes;
                    resultChannelTb.TotalLikes = data.TotalLikes;
                    resultChannelTb.TotalSaved = data.TotalSaved;
                    resultChannelTb.TotalSubs = data.TotalSubs;
                    resultChannelTb.TotalUsers = data.TotalUsers;
                    resultChannelTb.TotalVideos = data.TotalVideos;
                    resultChannelTb.TotalViews = data.TotalViews;
                    resultChannelTb.TrailerSystem = data.TrailerSystem;
                    resultChannelTb.TwLogin = data.TwLogin;
                    resultChannelTb.TwitchApi = data.TwitchApi;
                    resultChannelTb.TwitchImport = data.TwitchImport;
                    resultChannelTb.TwoFactorSetting = data.TwoFactorSetting;
                    resultChannelTb.UploadPoint = data.UploadPoint;
                    resultChannelTb.UploadSystem = data.UploadSystem;
                    resultChannelTb.UploadSystemType = data.UploadSystemType;
                    resultChannelTb.UserAds = data.UserAds;
                    resultChannelTb.UserMaxImport = data.UserMaxImport;
                    resultChannelTb.UserMaxUpload = data.UserMaxUpload;
                    resultChannelTb.UserMonApprove = data.UserMonApprove;
                    resultChannelTb.UserStatics = data.UserStatics;
                    resultChannelTb.UsrVMon = data.UsrVMon;
                    resultChannelTb.Validation = data.Validation;
                    resultChannelTb.VerificationBadge = data.VerificationBadge;
                    resultChannelTb.VideoTextSystem = data.VideoTextSystem;
                    resultChannelTb.VideosLoadLimit = data.VideosLoadLimit;
                    resultChannelTb.VideosStatics = data.VideosStatics;
                    resultChannelTb.VideosUploadLimit = data.VideosUploadLimit;
                    resultChannelTb.VimeoApiKey = data.VimeoApiKey;
                    resultChannelTb.WasabiAccessKey = data.WasabiAccessKey;
                    resultChannelTb.WasabiBucketName = data.WasabiBucketName;
                    resultChannelTb.WasabiBucketRegion = data.WasabiBucketRegion;
                    resultChannelTb.WasabiSecretKey = data.WasabiSecretKey;
                    resultChannelTb.WasabiStorage = data.WasabiStorage;
                    resultChannelTb.WatchingPoint = data.WatchingPoint;
                    resultChannelTb.Watermark = data.Watermark;
                    resultChannelTb.WechatLogin = data.WechatLogin;
                    resultChannelTb.WhoSell = data.WhoSell;
                    resultChannelTb.WhoUpload = data.WhoUpload;
                    resultChannelTb.WhoUseLive = data.WhoUseLive;
                    resultChannelTb.WowonderImg = data.WowonderImg;
                    resultChannelTb.WowonderSiteName = data.WowonderSiteName;
                    resultChannelTb.YoomoneyNotificationsSecret = data.YoomoneyNotificationsSecret;
                    resultChannelTb.YoomoneyPayment = data.YoomoneyPayment;
                    resultChannelTb.YoomoneyWalletId = data.YoomoneyWalletId;
                    resultChannelTb.AmazonEndpoint = data.AmazonEndpoint;
                    resultChannelTb.BackblazeAccessKey = data.BackblazeAccessKey;
                    resultChannelTb.BackblazeAccessKeyId = data.BackblazeAccessKeyId;
                    resultChannelTb.BackblazeBucketId = data.BackblazeBucketId;
                    resultChannelTb.BackblazeBucketName = data.BackblazeBucketName;
                    resultChannelTb.BackblazeEndpoint = data.BackblazeEndpoint;
                    resultChannelTb.BackblazeRegion = data.BackblazeRegion;
                    resultChannelTb.BackblazeStorage = data.BackblazeStorage;
                    resultChannelTb.ChunkSize = data.ChunkSize;
                    resultChannelTb.CronjobLastRun = data.CronjobLastRun;
                    resultChannelTb.DeveloperMode = data.DeveloperMode;
                    resultChannelTb.LogoCache = data.LogoCache;
                    resultChannelTb.MWithdrawal = data.MWithdrawal;
                    resultChannelTb.MainPaymentCurrency = data.MainPaymentCurrency;
                    resultChannelTb.SpacesEndpoint = data.SpacesEndpoint;
                    resultChannelTb.TiktokImport = data.TiktokImport;
                    resultChannelTb.TwitchAccessToken = data.TwitchAccessToken;
                    resultChannelTb.WasabiEndpoint = data.WasabiEndpoint;
                    resultChannelTb.YoutubePlayer = data.YoutubePlayer;

                    resultChannelTb.CurrencyArray = JsonConvert.SerializeObject(data.CurrencyArray);
                    resultChannelTb.CurrencySymbolArray = JsonConvert.SerializeObject(data.CurrencySymbolArray);
                    resultChannelTb.Continents = JsonConvert.SerializeObject(data.Continents);
                    resultChannelTb.Categories = JsonConvert.SerializeObject(data.Categories);
                    if (data.SubCategories != null)
                        resultChannelTb.SubCategories = JsonConvert.SerializeObject(data.SubCategories?.SubCategoriessList);
                    resultChannelTb.MoviesCategories = JsonConvert.SerializeObject(data.MoviesCategories);
                    resultChannelTb.ImportSystem = data.ImportSystem;
                    resultChannelTb.UploadSystem = data.UploadSystem;

                    connection.Update(resultChannelTb);

                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdate_Settings(data);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data setting 
        public GetSettingsObject.SiteSettings Get_Settings()
        {
            try
            {
                var connection = OpenConnection();
                var data = connection.Table<DataTables.MySettingsTb>().FirstOrDefault();
                if (data != null)
                {
                    var db = new GetSettingsObject.SiteSettings()
                    {
                        SmtpOrMail = data.SmtpOrMail,
                        SmtpHost = data.SmtpHost,
                        SmtpUsername = data.SmtpUsername,
                        SmtpPassword = data.SmtpPassword,
                        SmtpEncryption = data.SmtpEncryption,
                        SmtpPort = data.SmtpPort,
                        Theme = data.Theme,
                        LinkedinLogin = data.LinkedinLogin,
                        VkontakteLogin = data.VkontakteLogin,
                        FacebookAppId = data.FacebookAppId,
                        FacebookAppKey = data.FacebookAppKey,
                        GoogleAppId = data.GoogleAppId,
                        GoogleAppKey = data.GoogleAppKey,
                        TwitterAppId = data.TwitterAppId,
                        TwitterAppKey = data.TwitterAppKey,
                        LinkedinAppId = data.LinkedinAppId,
                        LinkedinAppKey = data.LinkedinAppKey,
                        VkontakteAppId = data.VkontakteAppId,
                        VkontakteAppKey = data.VkontakteAppKey,
                        InstagramAppId = data.InstagramAppId,
                        InstagramAppkey = data.InstagramAppkey,
                        InstagramLogin = data.InstagramLogin,
                        PaypalId = data.PaypalId,
                        PaypalSecret = data.PaypalSecret,
                        PaypalMode = data.PaypalMode,
                        LastBackup = data.LastBackup,
                        AmazoneS3Key = data.AmazoneS3Key,
                        AmazoneS3SKey = data.AmazoneS3SKey,
                        Region = data.Region,
                        StripeSecret = data.StripeSecret,
                        StripeId = data.StripeId,
                        PushId = data.PushId,
                        PushKey = data.PushKey,
                        DeleteAccount = data.DeleteAccount,
                        UserRegistration = data.UserRegistration,
                        MaxUpload = data.MaxUpload,
                        WowonderLogin = data.WowonderLogin,
                        WowonderAppId = data.WowonderAppId,
                        WowonderAppKey = data.WowonderAppKey,
                        WowonderDomainUri = data.WowonderDomainUri,
                        BankTransferNote = data.BankTransferNote,
                        PayseraProjectId = data.PayseraProjectId,
                        MaintenanceMode = data.MaintenanceMode,
                        BankDescription = data.BankDescription,
                        Version = data.Version,
                        Push = data.Push,
                        YtApi = data.YtApi,
                        Seo = data.Seo,
                        CheckoutPayment = data.CheckoutPayment,
                        CheckoutMode = data.CheckoutMode,
                        CheckoutCurrency = data.CheckoutCurrency,
                        CheckoutSellerId = data.CheckoutSellerId,
                        CheckoutPublishableKey = data.CheckoutPublishableKey,
                        CheckoutPrivateKey = data.CheckoutPrivateKey,
                        CashfreePayment = data.CashfreePayment,
                        CashfreeMode = data.CashfreeMode,
                        CashfreeClientKey = data.CashfreeClientKey,
                        CashfreeSecretKey = data.CashfreeSecretKey,
                        IyzipayPayment = data.IyzipayPayment,
                        IyzipayMode = data.IyzipayMode,
                        IyzipayKey = data.IyzipayKey,
                        IyzipayBuyerId = data.IyzipayBuyerId,
                        IyzipaySecretKey = data.IyzipaySecretKey,
                        IyzipayBuyerName = data.IyzipayBuyerName,
                        IyzipayBuyerSurname = data.IyzipayBuyerSurname,
                        IyzipayBuyerGsmNumber = data.IyzipayBuyerGsmNumber,
                        IyzipayBuyerEmail = data.IyzipayBuyerEmail,
                        IyzipayIdentityNumber = data.IyzipayIdentityNumber,
                        IyzipayAddress = data.IyzipayAddress,
                        IyzipayCity = data.IyzipayCity,
                        IyzipayCountry = data.IyzipayCountry,
                        IyzipayZip = data.IyzipayZip,
                        PayuPayment = data.PayuPayment,
                        PayuMode = data.PayuMode,
                        PayuMerchantId = data.PayuMerchantId,
                        PayuSecretKey = data.PayuSecretKey,
                        PayuBuyerName = data.PayuBuyerName,
                        PayuBuyerSurname = data.PayuBuyerSurname,
                        PayuBuyerGsmNumber = data.PayuBuyerGsmNumber,
                        PayuBuyerEmail = data.PayuBuyerEmail,
                        PreventSystem = data.PreventSystem,
                        BadLoginLimit = data.BadLoginLimit,
                        LockTime = data.LockTime,
                        PaystackPayment = data.PaystackPayment,
                        PaystackSecretKey = data.PaystackSecretKey,
                        LiveVideo = data.LiveVideo,
                        LiveVideoSave = data.LiveVideoSave,
                        AgoraLiveVideo = data.AgoraLiveVideo,
                        AgoraAppId = data.AgoraAppId,
                        AgoraAppCertificate = data.AgoraAppCertificate,
                        AgoraCustomerId = data.AgoraCustomerId,
                        AgoraCustomerCertificate = data.AgoraCustomerCertificate,
                        AmazoneS32 = data.AmazoneS32,
                        BucketName2 = data.BucketName2,
                        AmazoneS3Key2 = data.AmazoneS3Key2,
                        AmazoneS3SKey2 = data.AmazoneS3SKey2,
                        Region2 = data.Region2,
                        QqAppId = data.QqAppId,
                        QqAppkey = data.QqAppkey,
                        WeChatAppId = data.WeChatAppId,
                        WeChatAppkey = data.WeChatAppkey,
                        DiscordAppId = data.DiscordAppId,
                        DiscordAppkey = data.DiscordAppkey,
                        MailruAppId = data.MailruAppId,
                        MailruAppkey = data.MailruAppkey,
                        QqLogin = data.QqLogin,
                        DiscordLogin = data.DiscordLogin,
                        MailruLogin = data.MailruLogin,
                        AuthorizePayment = data.AuthorizePayment,
                        AuthorizeLoginId = data.AuthorizeLoginId,
                        AuthorizeTransactionKey = data.AuthorizeTransactionKey,
                        AuthorizeTestMode = data.AuthorizeTestMode,
                        SecurionpayPayment = data.SecurionpayPayment,
                        SecurionpayPublicKey = data.SecurionpayPublicKey,
                        SecurionpaySecretKey = data.SecurionpaySecretKey,
                        InviteLinksSystem = data.InviteLinksSystem,
                        UserLinksLimit = data.UserLinksLimit,
                        ExpireUserLinks = data.ExpireUserLinks,
                        Name = data.Name,
                        AamarpayMode = data.AamarpayMode,
                        AamarpayPayment = data.AamarpayPayment,
                        AamarpaySignatureKey = data.AamarpaySignatureKey,
                        AamarpayStoreId = data.AamarpayStoreId,
                        AdCPrice = data.AdCPrice,
                        AdminComRentVideos = data.AdminComRentVideos,
                        AdminComSellVideos = data.AdminComSellVideos,
                        AdminComSubscribers = data.AdminComSubscribers,
                        AllCreateArticles = data.AllCreateArticles,
                        ApproveVideos = data.ApproveVideos,
                        AppsApiId = data.AppsApiId,
                        AppsApiKey = data.AppsApiKey,
                        ArticleSystem = data.ArticleSystem,
                        AutoApprove = data.AutoApprove,
                        AutoSubscribe = data.AutoSubscribe,
                        AutoUsername = data.AutoUsername,
                        AutoplaySystem = data.AutoplaySystem,
                        BankPayment = data.BankPayment,
                        BlockSystem = data.BlockSystem,
                        AdVPrice = data.AdVPrice,
                        CensoredWords = data.CensoredWords,
                        CoinbaseKey = data.CoinbaseKey,
                        CoinbasePayment = data.CoinbasePayment,
                        Coinpayments = data.Coinpayments,
                        CoinpaymentsCoin = data.CoinpaymentsCoin,
                        CoinpaymentsCoins = data.CoinpaymentsCoins,
                        CoinpaymentsPublicKey = data.CoinpaymentsPublicKey,
                        CoinpaymentsSecret = data.CoinpaymentsSecret,
                        ComType = data.ComType,
                        CommentSystem = data.CommentSystem,
                        CommentsDefaultNum = data.CommentsDefaultNum,
                        CommentsPoint = data.CommentsPoint,
                        ConvertSpeed = data.ConvertSpeed,
                        CreditCard = data.CreditCard,
                        DailymotionId = data.DailymotionId,
                        DateStyle = data.DateStyle,
                        DemoVideo = data.DemoVideo,
                        Description = data.Description,
                        DislikesPoint = data.DislikesPoint,
                        DollarToPointCost = data.DollarToPointCost,
                        DonateSystem = data.DonateSystem,
                        DownloadVideos = data.DownloadVideos,
                        EarnFromAds = data.EarnFromAds,
                        Email = data.Email,
                        EmbedSystem = data.EmbedSystem,
                        FacebookImport = data.FacebookImport,
                        FavCategory = data.FavCategory,
                        FbLogin = data.FbLogin,
                        FfmpegBinaryFile = data.FfmpegBinaryFile,
                        FfmpegSystem = data.FfmpegSystem,
                        FortumoPayment = data.FortumoPayment,
                        FortumoServiceId = data.FortumoServiceId,
                        FreeDayLimit = data.FreeDayLimit,
                        FtpEndpoint = data.FtpEndpoint,
                        FtpHost = data.FtpHost,
                        FtpPassword = data.FtpPassword,
                        FtpPath = data.FtpPath,
                        FtpPort = data.FtpPort,
                        FtpUpload = data.FtpUpload,
                        FtpUsername = data.FtpUsername,
                        GeoBlocking = data.GeoBlocking,
                        GifSystem = data.GifSystem,
                        GoPro = data.GoPro,
                        Google = data.Google,
                        HistorySystem = data.HistorySystem,
                        Hostname = data.Hostname,
                        ImportSystem = data.ImportSystem,
                        IsOk = data.IsOk,
                        Keyword = data.Keyword,
                        LangModal = data.LangModal,
                        Language = data.Language,
                        LastAdminCollection = data.LastAdminCollection,
                        LastCreatedSitemap = data.LastCreatedSitemap,
                        LikesPoint = data.LikesPoint,
                        MainCurrency = data.MainCurrency,
                        MaxUploadAllUsers = data.MaxUploadAllUsers,
                        MaxUploadFreeUsers = data.MaxUploadFreeUsers,
                        MaxUploadProUsers = data.MaxUploadProUsers,
                        MoviesVideos = data.MoviesVideos,
                        NgeniusApiKey = data.NgeniusApiKey,
                        NgeniusMode = data.NgeniusMode,
                        NgeniusOutletId = data.NgeniusOutletId,
                        NgeniusPayment = data.NgeniusPayment,
                        NightMode = data.NightMode,
                        OkImport = data.OkImport,
                        P1080 = data.P1080,
                        P2048 = data.P2048,
                        P240 = data.P240,
                        P360 = data.P360,
                        P4096 = data.P4096,
                        P480 = data.P480,
                        P720 = data.P720,
                        PasswordComplexitySystem = data.PasswordComplexitySystem,
                        PayedSubscribers = data.PayedSubscribers,
                        PaymentCurrency = data.PaymentCurrency,
                        Paypal = data.Paypal,
                        PaypalCurrency = data.PaypalCurrency,
                        PayseraMode = data.PayseraMode,
                        PayseraPayment = data.PayseraPayment,
                        PayseraSignPassword = data.PayseraSignPassword,
                        PlayerType = data.PlayerType,
                        PlaylistSubscribe = data.PlaylistSubscribe,
                        PlusLogin = data.PlusLogin,
                        PointAllowWithdrawal = data.PointAllowWithdrawal,
                        PointLevelSystem = data.PointLevelSystem,
                        PopUp18 = data.PopUp18,
                        PopularChannels = data.PopularChannels,
                        PostSystem = data.PostSystem,
                        ProDayLimit = data.ProDayLimit,
                        ProGoogle = data.ProGoogle,
                        ProPkgPrice = data.ProPkgPrice,
                        PubPrice = data.PubPrice,
                        QueueCount = data.QueueCount,
                        RazorpayKeyId = data.RazorpayKeyId,
                        RazorpayKeySecret = data.RazorpayKeySecret,
                        RazorpayPayment = data.RazorpayPayment,
                        Recaptcha = data.Recaptcha,
                        RecaptchaKey = data.RecaptchaKey,
                        RememberDevice = data.RememberDevice,
                        RentVideosSystem = data.RentVideosSystem,
                        ReportCopyright = data.ReportCopyright,
                        RequireLogin = data.RequireLogin,
                        RequireSubcription = data.RequireSubcription,
                        ResizeVideo = data.ResizeVideo,
                        RestrictEmbeddingSystem = data.RestrictEmbeddingSystem,
                        RssExport = data.RssExport,
                        S3BucketName = data.S3BucketName,
                        S3Upload = data.S3Upload,
                        ScriptVersion = data.ScriptVersion,
                        SellVideosSystem = data.SellVideosSystem,
                        SeoLink = data.SeoLink,
                        Server = data.Server,
                        ServerKey = data.ServerKey,
                        ServerPort = data.ServerPort,
                        ShortsDuration = data.ShortsDuration,
                        ShortsSystem = data.ShortsSystem,
                        ShowArticles = data.ShowArticles,
                        SiteUrl = data.SiteUrl,
                        SpaceName = data.SpaceName,
                        SpaceRegion = data.SpaceRegion,
                        Spaces = data.Spaces,
                        SpacesKey = data.SpacesKey,
                        SpacesSecret = data.SpacesSecret,
                        StickyVideo = data.StickyVideo,
                        StockVideos = data.StockVideos,
                        StripeCurrency = data.StripeCurrency,
                        ThemeUrl = data.ThemeUrl,
                        Time18 = data.Time18,
                        Title = data.Title,
                        TotalComments = data.TotalComments,
                        TotalDislikes = data.TotalDislikes,
                        TotalLikes = data.TotalLikes,
                        TotalSaved = data.TotalSaved,
                        TotalSubs = data.TotalSubs,
                        TotalUsers = data.TotalUsers,
                        TotalVideos = data.TotalVideos,
                        TotalViews = data.TotalViews,
                        TrailerSystem = data.TrailerSystem,
                        TwLogin = data.TwLogin,
                        TwitchApi = data.TwitchApi,
                        TwitchImport = data.TwitchImport,
                        TwoFactorSetting = data.TwoFactorSetting,
                        UploadPoint = data.UploadPoint,
                        UploadSystem = data.UploadSystem,
                        UploadSystemType = data.UploadSystemType,
                        UserAds = data.UserAds,
                        UserMaxImport = data.UserMaxImport,
                        UserMaxUpload = data.UserMaxUpload,
                        UserMonApprove = data.UserMonApprove,
                        UserStatics = data.UserStatics,
                        UsrVMon = data.UsrVMon,
                        Validation = data.Validation,
                        VerificationBadge = data.VerificationBadge,
                        VideoTextSystem = data.VideoTextSystem,
                        VideosLoadLimit = data.VideosLoadLimit,
                        VideosStatics = data.VideosStatics,
                        VideosUploadLimit = data.VideosUploadLimit,
                        VimeoApiKey = data.VimeoApiKey,
                        WasabiAccessKey = data.WasabiAccessKey,
                        WasabiBucketName = data.WasabiBucketName,
                        WasabiBucketRegion = data.WasabiBucketRegion,
                        WasabiSecretKey = data.WasabiSecretKey,
                        WasabiStorage = data.WasabiStorage,
                        WatchingPoint = data.WatchingPoint,
                        Watermark = data.Watermark,
                        WechatLogin = data.WechatLogin,
                        WhoSell = data.WhoSell,
                        WhoUpload = data.WhoUpload,
                        WhoUseLive = data.WhoUseLive,
                        WowonderImg = data.WowonderImg,
                        WowonderSiteName = data.WowonderSiteName,
                        YoomoneyNotificationsSecret = data.YoomoneyNotificationsSecret,
                        YoomoneyPayment = data.YoomoneyPayment,
                        YoomoneyWalletId = data.YoomoneyWalletId,
                        AmazonEndpoint = data.AmazonEndpoint,
                        BackblazeAccessKey = data.BackblazeAccessKey,
                        BackblazeAccessKeyId = data.BackblazeAccessKeyId,
                        BackblazeBucketId = data.BackblazeBucketId,
                        BackblazeBucketName = data.BackblazeBucketName,
                        BackblazeEndpoint = data.BackblazeEndpoint,
                        BackblazeRegion = data.BackblazeRegion,
                        BackblazeStorage = data.BackblazeStorage,
                        ChunkSize = data.ChunkSize,
                        CronjobLastRun = data.CronjobLastRun,
                        DeveloperMode = data.DeveloperMode,
                        LogoCache = data.LogoCache,
                        MWithdrawal = data.MWithdrawal,
                        MainPaymentCurrency = data.MainPaymentCurrency,
                        SpacesEndpoint = data.SpacesEndpoint,
                        TiktokImport = data.TiktokImport,
                        TwitchAccessToken = data.TwitchAccessToken,
                        WasabiEndpoint = data.WasabiEndpoint,
                        YoutubePlayer = data.YoutubePlayer,
                    };

                    if (db != null)
                    { 
                        db.CurrencyArray = new List<string>();
                        db.CurrencySymbolArray = new GetSettingsObject.CurrencySymbolArray();
                        db.Continents = new List<string>();
                        db.Categories = new Dictionary<string, string>();
                        db.SubCategories = new GetSettingsObject.SubCategoriesUnion();
                        db.MoviesCategories = new Dictionary<string, string>();

                        if (!string.IsNullOrEmpty(data.CurrencyArray))
                            db.CurrencyArray = JsonConvert.DeserializeObject<List<string>>(data.CurrencyArray);

                        if (!string.IsNullOrEmpty(data.CurrencySymbolArray))
                            db.CurrencySymbolArray = JsonConvert.DeserializeObject<GetSettingsObject.CurrencySymbolArray>(data.CurrencySymbolArray);

                        if (!string.IsNullOrEmpty(data.Continents))
                            db.Continents = JsonConvert.DeserializeObject<List<string>>(data.Continents);

                        if (!string.IsNullOrEmpty(data.Categories))
                            db.Categories = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.Categories);

                        if (!string.IsNullOrEmpty(data.SubCategories))
                            db.SubCategories = new GetSettingsObject.SubCategoriesUnion
                            {
                                SubCategoriessList = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(data.SubCategories),
                            };

                        if (!string.IsNullOrEmpty(data.MoviesCategories))
                            db.MoviesCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.MoviesCategories);

                        //Categories
                        var listCategories = db.Categories.Select(cat => new Classes.Category
                        {
                            Id = cat.Key,
                            Name = Methods.FunString.DecodeString(cat.Value),
                            Color = "#212121",
                            Image = CategoriesController.GetImageCategory(cat.Value),
                            SubList = new List<Classes.Category>()
                        }).ToList();

                        CategoriesController.ListCategories.Clear();
                        CategoriesController.ListCategories = new ObservableCollection<Classes.Category>(listCategories);

                        if (db.SubCategories?.SubCategoriessList?.Count > 0)
                        {
                            //Sub Categories
                            foreach (var sub in db?.SubCategories?.SubCategoriessList)
                            {
                                var subCategories = ListUtils.MySettingsList?.SubCategories?.SubCategoriessList?.FirstOrDefault(a => a.Key == sub.Key).Value;
                                if (subCategories?.Count > 0)
                                {
                                    var cat = CategoriesController.ListCategories.FirstOrDefault(a => a.Id == sub.Key);
                                    if (cat != null)
                                    {
                                        foreach (var pairs in subCategories.SelectMany(pairs => pairs))
                                        {
                                            cat.SubList.Add(new Classes.Category
                                            {
                                                Id = pairs.Key,
                                                Name = Methods.FunString.DecodeString(pairs.Value),
                                                Color = "#212121",
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        //Movies Categories
                        var listMovies = db.MoviesCategories.Select(cat => new Classes.Category
                        {
                            Id = cat.Key,
                            Name = Methods.FunString.DecodeString(cat.Value),
                            Color = "#212121",
                            SubList = new List<Classes.Category>()
                        }).ToList();

                        CategoriesController.ListCategoriesMovies.Clear();
                        CategoriesController.ListCategoriesMovies = new ObservableCollection<Classes.Category>(listMovies);

                        AppSettings.Lang = data.Language;

                        AppSettings.OneSignalAppId = data.PushId;
                        //AppSettings.ShowButtonImport = string.IsNullOrWhiteSpace(data.ImportSystem) ? AppSettings.ShowButtonImport : data.ImportSystem == "on";
                        //AppSettings.ShowButtonUpload = string.IsNullOrWhiteSpace(data.UploadSystem) ? AppSettings.ShowButtonUpload : data.UploadSystem == "on";

                        return db;
                    }

                    return null!;
                }

                return null!;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_Settings();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                }
            }
        }

        #endregion

        #region Login

        //Get data Login
        public DataTables.LoginTb Get_data_Login()
        {
            try
            {
                var connection = OpenConnection();
                var dataUser = connection.Table<DataTables.LoginTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    UserDetails.Username = dataUser.Username;
                    UserDetails.FullName = dataUser.Username;
                    UserDetails.Password = dataUser.Password;
                    UserDetails.UserId = InitializePlayTube.UserId = dataUser.UserId;
                    UserDetails.AccessToken = Current.AccessToken = dataUser.AccessToken;
                    UserDetails.Status = dataUser.Status;
                    UserDetails.Cookie = dataUser.Cookie;
                    UserDetails.Email = dataUser.Email;
                    AppSettings.Lang = dataUser.Lang;

                    if (!string.IsNullOrEmpty(UserDetails.AccessToken))
                        UserDetails.IsLogin = true;

                    ListUtils.DataUserLoginList.Add(dataUser);
                    
                    return dataUser;
                }
                
                return null!;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_data_Login();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                var connection = OpenConnection();
                var dataUser = connection.Table<DataTables.LoginTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    dataUser.UserId = UserDetails.UserId;
                    dataUser.AccessToken = UserDetails.AccessToken;
                    dataUser.Cookie = UserDetails.Cookie;
                    dataUser.Username = UserDetails.Username;
                    dataUser.Password = UserDetails.Password;
                    dataUser.Status = UserDetails.Status;
                    dataUser.Lang = AppSettings.Lang;
                    dataUser.DeviceId = UserDetails.DeviceId;
                    dataUser.Email = UserDetails.Email;

                    connection.Update(dataUser);
                }
                else
                {
                    connection.Insert(db);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateLogin_Credentials(db);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region MyChannel

        //Insert Or Update data MyChannel

        public void InsertOrUpdate_DataMyChannel(UserDataObject channel)
        {
            try
            {
                var connection = OpenConnection();
                var resultChannelTb = connection.Table<DataTables.ChannelTb>().FirstOrDefault();
                if (resultChannelTb != null)
                {
                    resultChannelTb = new DataTables.ChannelTb
                    {
                        Id = channel.Id,
                        Username = channel.Username,
                        Email = channel.Email,
                        IpAddress = channel.IpAddress,
                        FirstName = channel.FirstName,
                        LastName = channel.LastName,
                        Gender = channel.Gender,
                        EmailCode = channel.EmailCode,
                        DeviceId = channel.DeviceId,
                        Language = channel.Language,
                        Avatar = channel.Avatar,
                        Cover = channel.Cover,
                        Src = channel.Src,
                        CountryId = channel.CountryId,
                        Age = channel.Age,
                        About = channel.About,
                        Google = channel.Google,
                        Facebook = channel.Facebook,
                        Twitter = channel.Twitter,
                        Instagram = channel.Instagram,
                        Active = channel.Active,
                        Admin = channel.Admin,
                        Verified = channel.Verified,
                        LastActive = channel.LastActive,
                        Registered = channel.Registered,
                        IsPro = channel.IsPro,
                        Imports = channel.Imports,
                        Uploads = channel.Uploads,
                        Wallet = channel.Wallet,
                        Balance = channel.Balance,
                        VideoMon = channel.VideoMon,
                        AgeChanged = channel.AgeChanged,
                        DonationPaypalEmail = channel.DonationPaypalEmail,
                        UserUploadLimit = channel.UserUploadLimit,
                        TwoFactor = channel.TwoFactor,
                        LastMonth = channel.LastMonth,
                        ActiveTime = channel.ActiveTime,
                        ActiveExpire = channel.ActiveExpire,
                        PhoneNumber = channel.PhoneNumber,
                        Address = channel.Address,
                        City = channel.City,
                        State = channel.State,
                        Zip = channel.Zip,
                        SubscriberPrice = channel.SubscriberPrice,
                        Monetization = channel.Monetization,
                        NewEmail = channel.NewEmail,
                        TotalAds = channel.TotalAds,
                        SuspendUpload = channel.SuspendUpload,
                        SuspendImport = channel.SuspendImport,
                        PaystackRef = channel.PaystackRef,
                        ConversationId = channel.ConversationId,
                        PointDayExpire = channel.PointDayExpire,
                        Points = channel.Points,
                        DailyPoints = channel.DailyPoints,
                        Name = channel.Name,
                        ExCover = channel.ExCover,
                        Url = channel.Url,
                        AboutDecoded = channel.AboutDecoded,
                        FullCover = channel.FullCover,
                        BalanceOr = channel.BalanceOr,
                        NameV = channel.NameV,
                        CountryName = channel.CountryName,
                        GenderText = channel.GenderText,
                        AmISubscribed = channel.AmISubscribed,
                        SubscribeCount = channel.SubscribeCount,
                        IsSubscribedToChannel = channel.IsSubscribedToChannel,
                        Time = channel.Time,
                        InfoFile = channel.InfoFile,
                        GoogleTrackingCode = channel.GoogleTrackingCode,
                        Newsletters = channel.Newsletters,
                        ChannelNotify = channel.ChannelNotify,
                        AamarpayTranId = channel.AamarpayTranId,
                        CoinbaseCode = channel.CoinbaseCode,
                        CoinbaseHash = channel.CoinbaseHash,
                        CoinpaymentsTxnId = channel.CoinpaymentsTxnId,
                        Discord = channel.Discord,
                        FortumoHash = channel.FortumoHash,
                        LinkedIn = channel.LinkedIn,
                        Mailru = channel.Mailru,
                        NgeniusRef = channel.NgeniusRef,
                        PauseHistory = channel.PauseHistory,
                        Qq = channel.Qq,
                        SecurionpayKey = channel.SecurionpayKey,
                        StripeSessionId = channel.StripeSessionId,
                        TvCode = channel.TvCode,
                        Vk = channel.Vk,
                        Wechat = channel.Wechat,
                        YoomoneyHash = channel.YoomoneyHash,
                        FavCategory = JsonConvert.SerializeObject(channel.FavCategory),
                    };
                     
                    if (resultChannelTb != null)
                    {
                        UserDetails.Avatar = resultChannelTb.Avatar;
                        UserDetails.Cover = resultChannelTb.Cover;
                        UserDetails.Username = resultChannelTb.Username;
                        UserDetails.FullName = AppTools.GetNameFinal(resultChannelTb);

                        connection.Update(resultChannelTb);
                    }
                }
                else
                {
                    var db = new DataTables.ChannelTb
                    {
                        Id = channel.Id,
                        Username = channel.Username,
                        Email = channel.Email,
                        IpAddress = channel.IpAddress,
                        FirstName = channel.FirstName,
                        LastName = channel.LastName,
                        Gender = channel.Gender,
                        EmailCode = channel.EmailCode,
                        DeviceId = channel.DeviceId,
                        Language = channel.Language,
                        Avatar = channel.Avatar,
                        Cover = channel.Cover,
                        Src = channel.Src,
                        CountryId = channel.CountryId,
                        Age = channel.Age,
                        About = channel.About,
                        Google = channel.Google,
                        Facebook = channel.Facebook,
                        Twitter = channel.Twitter,
                        Instagram = channel.Instagram,
                        Active = channel.Active,
                        Admin = channel.Admin,
                        Verified = channel.Verified,
                        LastActive = channel.LastActive,
                        Registered = channel.Registered,
                        IsPro = channel.IsPro,
                        Imports = channel.Imports,
                        Uploads = channel.Uploads,
                        Wallet = channel.Wallet,
                        Balance = channel.Balance,
                        VideoMon = channel.VideoMon,
                        AgeChanged = channel.AgeChanged,
                        DonationPaypalEmail = channel.DonationPaypalEmail,
                        UserUploadLimit = channel.UserUploadLimit,
                        TwoFactor = channel.TwoFactor,
                        LastMonth = channel.LastMonth,
                        ActiveTime = channel.ActiveTime,
                        ActiveExpire = channel.ActiveExpire,
                        PhoneNumber = channel.PhoneNumber,
                        Address = channel.Address,
                        City = channel.City,
                        State = channel.State,
                        Zip = channel.Zip,
                        SubscriberPrice = channel.SubscriberPrice,
                        Monetization = channel.Monetization,
                        NewEmail = channel.NewEmail,
                        TotalAds = channel.TotalAds,
                        SuspendUpload = channel.SuspendUpload,
                        SuspendImport = channel.SuspendImport,
                        PaystackRef = channel.PaystackRef,
                        ConversationId = channel.ConversationId,
                        PointDayExpire = channel.PointDayExpire,
                        Points = channel.Points,
                        DailyPoints = channel.DailyPoints,
                        Name = channel.Name,
                        ExCover = channel.ExCover,
                        Url = channel.Url,
                        AboutDecoded = channel.AboutDecoded,
                        FullCover = channel.FullCover,
                        BalanceOr = channel.BalanceOr,
                        NameV = channel.NameV,
                        CountryName = channel.CountryName,
                        GenderText = channel.GenderText,
                        AmISubscribed = channel.AmISubscribed,
                        SubscribeCount = channel.SubscribeCount,
                        IsSubscribedToChannel = channel.IsSubscribedToChannel,
                        Time = channel.Time,
                        InfoFile = channel.InfoFile,
                        GoogleTrackingCode = channel.GoogleTrackingCode,
                        Newsletters = channel.Newsletters,
                        ChannelNotify = channel.ChannelNotify,
                        AamarpayTranId = channel.AamarpayTranId,
                        CoinbaseCode = channel.CoinbaseCode,
                        CoinbaseHash = channel.CoinbaseHash,
                        CoinpaymentsTxnId = channel.CoinpaymentsTxnId,
                        Discord = channel.Discord,
                        FortumoHash = channel.FortumoHash,
                        LinkedIn = channel.LinkedIn,
                        Mailru = channel.Mailru,
                        NgeniusRef = channel.NgeniusRef,
                        PauseHistory = channel.PauseHistory,
                        Qq = channel.Qq,
                        SecurionpayKey = channel.SecurionpayKey,
                        StripeSessionId = channel.StripeSessionId,
                        TvCode = channel.TvCode,
                        Vk = channel.Vk,
                        Wechat = channel.Wechat,
                        YoomoneyHash = channel.YoomoneyHash,
                        FavCategory = JsonConvert.SerializeObject(channel.FavCategory),
                    };
                     
                    if (db != null)
                    {
                        connection.Insert(db);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdate_DataMyChannel(channel);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get Data My Channel
        public UserDataObject GetDataMyChannel()
        {
            try
            {
                var connection = OpenConnection();
                var channel = connection.Table<DataTables.ChannelTb>().FirstOrDefault();
                if (channel != null)
                { 
                    var db = new UserDataObject
                    {
                        Id = channel.Id,
                        Username = channel.Username,
                        Email = channel.Email,
                        IpAddress = channel.IpAddress,
                        FirstName = channel.FirstName,
                        LastName = channel.LastName,
                        Gender = channel.Gender,
                        EmailCode = channel.EmailCode,
                        DeviceId = channel.DeviceId,
                        Language = channel.Language,
                        Avatar = channel.Avatar,
                        Cover = channel.Cover,
                        Src = channel.Src,
                        CountryId = channel.CountryId,
                        Age = channel.Age,
                        About = channel.About,
                        Google = channel.Google,
                        Facebook = channel.Facebook,
                        Twitter = channel.Twitter,
                        Instagram = channel.Instagram,
                        Active = channel.Active,
                        Admin = channel.Admin,
                        Verified = channel.Verified,
                        LastActive = channel.LastActive,
                        Registered = channel.Registered,
                        IsPro = channel.IsPro,
                        Imports = channel.Imports,
                        Uploads = channel.Uploads,
                        Wallet = channel.Wallet,
                        Balance = channel.Balance,
                        VideoMon = channel.VideoMon,
                        AgeChanged = channel.AgeChanged,
                        DonationPaypalEmail = channel.DonationPaypalEmail,
                        UserUploadLimit = channel.UserUploadLimit,
                        TwoFactor = channel.TwoFactor,
                        LastMonth = channel.LastMonth,
                        ActiveTime = channel.ActiveTime,
                        ActiveExpire = channel.ActiveExpire,
                        PhoneNumber = channel.PhoneNumber,
                        Address = channel.Address,
                        City = channel.City,
                        State = channel.State,
                        Zip = channel.Zip,
                        SubscriberPrice = channel.SubscriberPrice,
                        Monetization = channel.Monetization,
                        NewEmail = channel.NewEmail,
                        TotalAds = channel.TotalAds,
                        SuspendUpload = channel.SuspendUpload,
                        SuspendImport = channel.SuspendImport,
                        PaystackRef = channel.PaystackRef,
                        ConversationId = channel.ConversationId,
                        PointDayExpire = channel.PointDayExpire,
                        Points = channel.Points,
                        DailyPoints = channel.DailyPoints,
                        Name = channel.Name,
                        ExCover = channel.ExCover,
                        Url = channel.Url,
                        AboutDecoded = channel.AboutDecoded,
                        FullCover = channel.FullCover,
                        BalanceOr = channel.BalanceOr,
                        NameV = channel.NameV,
                        CountryName = channel.CountryName,
                        GenderText = channel.GenderText,
                        AmISubscribed = channel.AmISubscribed,
                        SubscribeCount = channel.SubscribeCount,
                        IsSubscribedToChannel = channel.IsSubscribedToChannel,
                        Time = channel.Time,
                        InfoFile = channel.InfoFile,
                        GoogleTrackingCode = channel.GoogleTrackingCode,
                        Newsletters = channel.Newsletters,
                        ChannelNotify = channel.ChannelNotify,
                        AamarpayTranId = channel.AamarpayTranId,
                        CoinbaseCode = channel.CoinbaseCode,
                        CoinbaseHash = channel.CoinbaseHash,
                        CoinpaymentsTxnId = channel.CoinpaymentsTxnId,
                        Discord = channel.Discord,
                        FortumoHash = channel.FortumoHash,
                        LinkedIn = channel.LinkedIn,
                        Mailru = channel.Mailru,
                        NgeniusRef = channel.NgeniusRef,
                        PauseHistory = channel.PauseHistory,
                        Qq = channel.Qq,
                        SecurionpayKey = channel.SecurionpayKey,
                        StripeSessionId = channel.StripeSessionId,
                        TvCode = channel.TvCode,
                        Vk = channel.Vk,
                        Wechat = channel.Wechat,
                        YoomoneyHash = channel.YoomoneyHash,
                        FavCategory = new List<string>(),
                    };
                     
                    if (db != null)
                    {
                        if (!string.IsNullOrEmpty(channel.FavCategory))
                            db.FavCategory = JsonConvert.DeserializeObject<List<string>>(channel.FavCategory);
                        UserDetails.Avatar = db.Avatar;
                        UserDetails.Cover = db.Cover;
                        UserDetails.Username = db.Username;
                        UserDetails.FullName = AppTools.GetNameFinal(db);

                        ListUtils.MyChannelList?.Clear();
                        ListUtils.MyChannelList?.Add(db);

                        return channel;
                    }
                     
                    return null!;
                }

                return null!;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetDataMyChannel();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        #endregion

        #region SubscriptionsChannel Videos

        //Insert SubscriptionsChannel Videos
        public void Insert_One_SubscriptionChannel(UserDataObject channel)
        {
            try
            {
                var connection = OpenConnection();
                if (channel != null)
                {
                    var select = connection.Table<DataTables.SubscriptionsChannelTb>().FirstOrDefault(a => a.Id == channel.Id);
                    if (select == null)
                    {
                        var db = ClassMapper.Mapper?.Map<DataTables.SubscriptionsChannelTb>(channel);
                        if (db != null)
                        {
                            db.FavCategory = JsonConvert.SerializeObject(channel.FavCategory);

                            connection.Insert(db);
                        }
                    }
                    else
                    {
                        select = ClassMapper.Mapper?.Map<DataTables.SubscriptionsChannelTb>(channel);
                        if (select != null)
                        {
                            select.FavCategory = JsonConvert.SerializeObject(channel.FavCategory);

                            connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_One_SubscriptionChannel(channel);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Insert SubscriptionsChannel Videos
        public void InsertAllSubscriptionsChannel(ObservableCollection<UserDataObject> channelsList)
        {
            try
            {
                var connection = OpenConnection();
                var result = connection.Table<DataTables.SubscriptionsChannelTb>().ToList();

                var list = new List<DataTables.SubscriptionsChannelTb>();
                foreach (var info in channelsList)
                {
                    var db = ClassMapper.Mapper?.Map<DataTables.SubscriptionsChannelTb>(info);
                    if (db != null)
                    {
                        db.FavCategory = JsonConvert.SerializeObject(info.FavCategory);

                        list.Add(db);
                    }

                    var update = result.FirstOrDefault(a => a.Id == info.Id);
                    if (update != null)
                    {
                        update = db;

                        update.FavCategory = JsonConvert.SerializeObject(info.FavCategory);

                        connection.Update(update);
                    }
                }

                if (list.Count <= 0) return;

                connection.BeginTransaction();
                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                if (newItemList.Count > 0)
                    connection.InsertAll(newItemList);

                result = connection.Table<DataTables.SubscriptionsChannelTb>().ToList();
                var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                if (deleteItemList.Count > 0)
                    foreach (var delete in deleteItemList)
                        connection.Delete(delete);

                connection?.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertAllSubscriptionsChannel(channelsList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Remove SubscriptionsChannel Videos
        public void RemoveSubscriptionsChannel(string subscriptionsChannelId)
        {
            try
            {
                var connection = OpenConnection();
                if (!string.IsNullOrEmpty(subscriptionsChannelId))
                {
                    var select = connection.Table<DataTables.SubscriptionsChannelTb>().FirstOrDefault(a => a.Id == subscriptionsChannelId);
                    if (select != null)
                    {
                        connection.Delete(select);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    RemoveSubscriptionsChannel(subscriptionsChannelId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get SubscriptionsChannel Videos
        public ObservableCollection<UserDataObject> GetSubscriptionsChannel()
        {
            try
            {
                var connection = OpenConnection();
                var select = connection.Table<DataTables.SubscriptionsChannelTb>().ToList();
                if (select.Count > 0)
                {
                    var list = new ObservableCollection<UserDataObject>();
                    foreach (var channel in select)
                    {
                        var db = new UserDataObject
                        {
                            Id = channel.Id,
                            Username = channel.Username,
                            Email = channel.Email,
                            IpAddress = channel.IpAddress,
                            FirstName = channel.FirstName,
                            LastName = channel.LastName,
                            Gender = channel.Gender,
                            EmailCode = channel.EmailCode,
                            DeviceId = channel.DeviceId,
                            Language = channel.Language,
                            Avatar = channel.Avatar,
                            Cover = channel.Cover,
                            Src = channel.Src,
                            CountryId = channel.CountryId,
                            Age = channel.Age,
                            About = channel.About,
                            Google = channel.Google,
                            Facebook = channel.Facebook,
                            Twitter = channel.Twitter,
                            Instagram = channel.Instagram,
                            Active = channel.Active,
                            Admin = channel.Admin,
                            Verified = channel.Verified,
                            LastActive = channel.LastActive,
                            Registered = channel.Registered,
                            IsPro = channel.IsPro,
                            Imports = channel.Imports,
                            Uploads = channel.Uploads,
                            Wallet = channel.Wallet,
                            Balance = channel.Balance,
                            VideoMon = channel.VideoMon,
                            AgeChanged = channel.AgeChanged,
                            DonationPaypalEmail = channel.DonationPaypalEmail,
                            UserUploadLimit = channel.UserUploadLimit,
                            TwoFactor = channel.TwoFactor,
                            LastMonth = channel.LastMonth,
                            ActiveTime = channel.ActiveTime,
                            ActiveExpire = channel.ActiveExpire,
                            PhoneNumber = channel.PhoneNumber,
                            Address = channel.Address,
                            City = channel.City,
                            State = channel.State,
                            Zip = channel.Zip,
                            SubscriberPrice = channel.SubscriberPrice,
                            Monetization = channel.Monetization,
                            NewEmail = channel.NewEmail,
                            TotalAds = channel.TotalAds,
                            SuspendUpload = channel.SuspendUpload,
                            SuspendImport = channel.SuspendImport,
                            PaystackRef = channel.PaystackRef,
                            ConversationId = channel.ConversationId,
                            PointDayExpire = channel.PointDayExpire,
                            Points = channel.Points,
                            DailyPoints = channel.DailyPoints,
                            Name = channel.Name,
                            ExCover = channel.ExCover,
                            Url = channel.Url,
                            AboutDecoded = channel.AboutDecoded,
                            FullCover = channel.FullCover,
                            BalanceOr = channel.BalanceOr,
                            NameV = channel.NameV,
                            CountryName = channel.CountryName,
                            GenderText = channel.GenderText,
                            AmISubscribed = channel.AmISubscribed,
                            SubscribeCount = channel.SubscribeCount,
                            IsSubscribedToChannel = channel.IsSubscribedToChannel,
                            Time = channel.Time,
                            InfoFile = channel.InfoFile,
                            GoogleTrackingCode = channel.GoogleTrackingCode,
                            Newsletters = channel.Newsletters,
                            ChannelNotify = channel.ChannelNotify,
                            AamarpayTranId = channel.AamarpayTranId,
                            CoinbaseCode = channel.CoinbaseCode,
                            CoinbaseHash = channel.CoinbaseHash,
                            CoinpaymentsTxnId = channel.CoinpaymentsTxnId,
                            Discord = channel.Discord,
                            FortumoHash = channel.FortumoHash,
                            LinkedIn = channel.LinkedIn,
                            Mailru = channel.Mailru,
                            NgeniusRef = channel.NgeniusRef,
                            PauseHistory = channel.PauseHistory,
                            Qq = channel.Qq,
                            SecurionpayKey = channel.SecurionpayKey,
                            StripeSessionId = channel.StripeSessionId,
                            TvCode = channel.TvCode,
                            Vk = channel.Vk,
                            Wechat = channel.Wechat,
                            YoomoneyHash = channel.YoomoneyHash,
                            FavCategory = new List<string>(),
                        };

                        if (db != null)
                        {
                            if (!string.IsNullOrEmpty(channel.FavCategory))
                                db.FavCategory = JsonConvert.DeserializeObject<List<string>>(channel.FavCategory);

                            list.Add(db);
                        } 
                    }

                    return list;
                }

                return new ObservableCollection<UserDataObject>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetSubscriptionsChannel();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<UserDataObject>();
                } 
            }
        }

        #endregion

        #region WatchOffline Videos

        //Insert WatchOffline Videos
        public void Insert_WatchOfflineVideos(VideoDataObject video)
        {
            try
            {
                var connection = OpenConnection();
                if (video != null)
                {
                    var select = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == video.Id);
                    if (select == null)
                    {
                        var db = ClassMapper.Mapper?.Map<DataTables.WatchOfflineVideosTb>(video);
                        db.Owner = JsonConvert.SerializeObject(video.Owner?.OwnerClass);
                        connection.Insert(db);
                    }
                    else
                    {
                        select = ClassMapper.Mapper?.Map<DataTables.WatchOfflineVideosTb>(video);
                        select.Owner = JsonConvert.SerializeObject(video.Owner?.OwnerClass);
                        connection.Update(select);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_WatchOfflineVideos(video);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Remove WatchOffline Videos
        public void Remove_WatchOfflineVideos(string watchOfflineVideosId)
        {
            try
            {
                var connection = OpenConnection();
                if (!string.IsNullOrEmpty(watchOfflineVideosId))
                {
                    var select = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == watchOfflineVideosId);
                    if (select != null)
                    {
                        connection.Delete(select);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Remove_WatchOfflineVideos(watchOfflineVideosId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get WatchOffline Videos
        public ObservableCollection<VideoDataObject> Get_WatchOfflineVideos()
        {
            try
            {
                var connection = OpenConnection();
                var select = connection.Table<DataTables.WatchOfflineVideosTb>().ToList();
                if (select.Count > 0)
                {
                    var list = new ObservableCollection<VideoDataObject>();
                    foreach (var item in select)
                    {
                        var db = new VideoDataObject
                        {
                            Id = item.Id,
                            VideoId = item.VideoId,
                            UserId = item.UserId,
                            ShortId = item.ShortId,
                            Title = item.Title,
                            Description = item.Description,
                            Thumbnail = item.Thumbnail,
                            VideoLocation = item.VideoLocation,
                            Youtube = item.Youtube,
                            Vimeo = item.Vimeo,
                            Daily = item.Daily,
                            Facebook = item.Facebook,
                            Time = item.Time,
                            TimeDate = item.TimeDate,
                            Active = item.Active,
                            Tags = item.Tags,
                            Duration = item.Duration,
                            Size = item.Size,
                            Converted = item.Converted,
                            CategoryId = item.CategoryId,
                            Views = item.Views,
                            Featured = item.Featured,
                            Registered = item.Registered,
                            Type = item.Type,
                            Approved = item.Approved,
                            OrgThumbnail = item.OrgThumbnail,
                            VideoType = item.VideoType,
                            Source = item.Source,
                            Url = item.Url,
                            EditDescription = item.EditDescription,
                            MarkupDescription = item.MarkupDescription,
                            IsLiked = item.IsLiked,
                            IsDisliked = item.IsDisliked,
                            IsOwner = item.IsOwner,
                            TimeAlpha = item.TimeAlpha,
                            TimeAgo = item.TimeAgo,
                            CategoryName = item.CategoryName,
                            Likes = item.Likes,
                            Dislikes = item.Dislikes,
                            LikesPercent = item.LikesPercent,
                            DislikesPercent = item.DislikesPercent,
                            AgeRestriction = item.AgeRestriction,
                            Country = item.Country,
                            Demo = item.Demo,
                            GeoBlocking = item.GeoBlocking,
                            Gif = item.Gif,
                            IsMovie = item.IsMovie,
                            IsPurchased = item.IsPurchased,
                            MovieRelease = item.MovieRelease,
                            Ok = item.Ok,
                            PlaylistLink = item.PlaylistLink,
                            Privacy = item.Privacy,
                            Producer = item.Producer,
                            Quality = item.Quality,
                            Rating = item.Rating,
                            RentPrice = item.RentPrice,
                            SellVideo = item.SellVideo,
                            Stars = item.Stars,
                            SubCategory = item.SubCategory,
                            TrId = item.TrId,
                            Twitch = item.Twitch,
                            TwitchType = item.TwitchType,
                            DataVideoId = item.DataVideoId,
                            IsSubscribed = item.IsSubscribed,
                            Monetization = item.Monetization,
                            The1080P = item.The1080P,
                            The2048P = item.The2048P,
                            The240P = item.The240P,
                            The360P = item.The360P,
                            The4096P = item.The4096P,
                            The480P = item.The480P,
                            The720P = item.The720P,
                            AgoraResourceId = item.AgoraResourceId,
                            AgoraSid = item.AgoraSid,
                            HistoryId = item.HistoryId,
                            IsStock = item.IsStock,
                            License = item.License,
                            LiveEnded = item.LiveEnded,
                            LiveTime = item.LiveTime,
                            AgoraToken = item.AgoraToken,
                            AjaxUrl = item.AjaxUrl,
                            Embedding = item.Embedding,
                            IsShort = item.IsShort,
                            LiveChating = item.LiveChating,
                            PausedTime = item.PausedTime,
                            PublicationDate = item.PublicationDate,
                            Trailer = item.Trailer,
                            QualityVideoSelect = item.QualityVideoSelect,
                            StreamName = item.StreamName,
                            Video1080P = item.Video1080P,
                            Video2048P = item.Video2048P,
                            Video240P = item.Video240P,
                            Video360P = item.Video360P,
                            Video4096P = item.Video4096P,
                            Video480P = item.Video480P,
                            Video720P = item.Video720P,
                            VideoAuto = item.VideoAuto,
                            Owner = new UserDataObject(),
                        };

                        if (!string.IsNullOrEmpty(item.Owner))
                            db.Owner = new OwnerUnion
                            {
                                OwnerClass = JsonConvert.DeserializeObject<UserDataObject>(item.Owner)
                            };

                        list.Add(db);
                    }

                    return list;
                }

                return new ObservableCollection<VideoDataObject>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_WatchOfflineVideos();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<VideoDataObject>();
                } 
            }
        }
   
        public VideoDataObject Get_LatestWatchOfflineVideos(string id)
        {
            try
            {
                var connection = OpenConnection();
                var item = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == id);
                if (item != null)
                {
                    var db = new VideoDataObject
                    {
                        Id = item.Id,
                        VideoId = item.VideoId,
                        UserId = item.UserId,
                        ShortId = item.ShortId,
                        Title = item.Title,
                        Description = item.Description,
                        Thumbnail = item.Thumbnail,
                        VideoLocation = item.VideoLocation,
                        Youtube = item.Youtube,
                        Vimeo = item.Vimeo,
                        Daily = item.Daily,
                        Facebook = item.Facebook,
                        Time = item.Time,
                        TimeDate = item.TimeDate,
                        Active = item.Active,
                        Tags = item.Tags,
                        Duration = item.Duration,
                        Size = item.Size,
                        Converted = item.Converted,
                        CategoryId = item.CategoryId,
                        Views = item.Views,
                        Featured = item.Featured,
                        Registered = item.Registered,
                        Type = item.Type,
                        Approved = item.Approved,
                        OrgThumbnail = item.OrgThumbnail,
                        VideoType = item.VideoType,
                        Source = item.Source,
                        Url = item.Url,
                        EditDescription = item.EditDescription,
                        MarkupDescription = item.MarkupDescription,
                        IsLiked = item.IsLiked,
                        IsDisliked = item.IsDisliked,
                        IsOwner = item.IsOwner,
                        TimeAlpha = item.TimeAlpha,
                        TimeAgo = item.TimeAgo,
                        CategoryName = item.CategoryName,
                        Likes = item.Likes,
                        Dislikes = item.Dislikes,
                        LikesPercent = item.LikesPercent,
                        DislikesPercent = item.DislikesPercent,
                        AgeRestriction = item.AgeRestriction,
                        Country = item.Country,
                        Demo = item.Demo,
                        GeoBlocking = item.GeoBlocking,
                        Gif = item.Gif,
                        IsMovie = item.IsMovie,
                        IsPurchased = item.IsPurchased,
                        MovieRelease = item.MovieRelease,
                        Ok = item.Ok,
                        PlaylistLink = item.PlaylistLink,
                        Privacy = item.Privacy,
                        Producer = item.Producer,
                        Quality = item.Quality,
                        Rating = item.Rating,
                        RentPrice = item.RentPrice,
                        SellVideo = item.SellVideo,
                        Stars = item.Stars,
                        SubCategory = item.SubCategory,
                        TrId = item.TrId,
                        Twitch = item.Twitch,
                        TwitchType = item.TwitchType,
                        DataVideoId = item.DataVideoId,
                        IsSubscribed = item.IsSubscribed,
                        Monetization = item.Monetization,
                        The1080P = item.The1080P,
                        The2048P = item.The2048P,
                        The240P = item.The240P,
                        The360P = item.The360P,
                        The4096P = item.The4096P,
                        The480P = item.The480P,
                        The720P = item.The720P,
                        AgoraResourceId = item.AgoraResourceId,
                        AgoraSid = item.AgoraSid,
                        HistoryId = item.HistoryId,
                        IsStock = item.IsStock,
                        License = item.License,
                        LiveEnded = item.LiveEnded,
                        LiveTime = item.LiveTime,
                        AgoraToken = item.AgoraToken,
                        AjaxUrl = item.AjaxUrl,
                        Embedding = item.Embedding,
                        IsShort = item.IsShort,
                        LiveChating = item.LiveChating,
                        PausedTime = item.PausedTime,
                        PublicationDate = item.PublicationDate,
                        Trailer = item.Trailer,
                        QualityVideoSelect = item.QualityVideoSelect,
                        StreamName = item.StreamName,
                        Video1080P = item.Video1080P,
                        Video2048P = item.Video2048P,
                        Video240P = item.Video240P,
                        Video360P = item.Video360P,
                        Video4096P = item.Video4096P,
                        Video480P = item.Video480P,
                        Video720P = item.Video720P,
                        VideoAuto = item.VideoAuto,
                        Owner = new UserDataObject(),
                    };

                    if (!string.IsNullOrEmpty(item.Owner))
                        db.Owner = new OwnerUnion
                        {
                            OwnerClass = JsonConvert.DeserializeObject<UserDataObject>(item.Owner)
                        };

                    return db;
                }

                return null;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_LatestWatchOfflineVideos(id);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                } 
            }
        }

        public void Update_WatchOfflineVideos(string videoid, string videopath)
        {
            try
            {
                var connection = OpenConnection();
                var select = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == videoid);
                if (select != null)
                {
                    select.VideoLocation = videopath;
                    connection.Update(select);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Update_WatchOfflineVideos(videoid, videopath);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Library Item

        private void AddLibrarySectionViews()
        {
            try
            {
                var connection = OpenConnection();
                var check = connection.Table<DataTables.LibraryItemTb>().ToList();

                if (check == null || check.Count == 0)
                {
                    //translate text in the adapter
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "1",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_Subscriptions),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_subscriptions_vector,
                        BackgroundColor = "#9C27B0",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "2",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_WatchLater),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_time_vector,
                        BackgroundColor = "#2196F3",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "3",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_RecentlyWatched),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_recently_vector,
                        BackgroundColor = "#E91E63",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "4",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_WatchOffline),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_download_vector,
                        BackgroundColor = "#009688",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "5",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_PlayLists),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_playList_vector,
                        BackgroundColor = "#F44336",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "6",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_Liked),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_likefilled_video_vector,
                        BackgroundColor = "#3F51B5",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "7",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_Shared),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_share_post_vector,
                        BackgroundColor = "#B71C1C",
                    });
                    ListUtils.LibraryList.Add(new Classes.LibraryItem
                    {
                        SectionId = "8",
                        SectionText = Application.Context.GetText(Resource.String.Lbl_Paid),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault",
                        Icon = Resource.Drawable.icon_dollars_vector,
                        BackgroundColor = "#6D4C41",
                    });

                    InsertLibraryItem(ListUtils.LibraryList);
                }
                else
                {
                    ListUtils.LibraryList = new ObservableCollection<Classes.LibraryItem>();
                    foreach (var item in check)
                    { 
                        ListUtils.LibraryList.Add(new Classes.LibraryItem
                        {
                            SectionId = item.SectionId,
                            SectionText = item.SectionText,
                            VideoCount = item.VideoCount,
                            BackgroundImage = item.BackgroundImage,
                            Icon = item.Icon,
                            BackgroundColor = item.BackgroundColor,
                        }); 
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Insert data LibraryItem
        public void InsertLibraryItem(Classes.LibraryItem libraryItem)
        {
            try
            {
                var connection = OpenConnection();
                if (libraryItem == null)
                    return;
                var select = connection.Table<DataTables.LibraryItemTb>().FirstOrDefault(a => a.SectionId == libraryItem.SectionId);
                if (select != null)
                {
                    select.VideoCount = libraryItem.VideoCount;
                    select.BackgroundImage = libraryItem.BackgroundImage;
                    connection.Update(select);
                }
                else
                {
                    var item = new DataTables.LibraryItemTb
                    {
                        SectionId = libraryItem.SectionId,
                        SectionText = libraryItem.SectionText,
                        VideoCount = libraryItem.VideoCount,
                        BackgroundImage = libraryItem.BackgroundImage,
                        Icon = libraryItem.Icon,
                        BackgroundColor = libraryItem.BackgroundColor,
                    };
                    connection.Insert(item);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertLibraryItem(libraryItem);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Insert data LibraryItem
        public void InsertLibraryItem(ObservableCollection<Classes.LibraryItem> libraryList)
        {
            try
            {
                var connection = OpenConnection();
                if (libraryList?.Count == 0)
                    return;
                if (libraryList != null)
                {
                    foreach (var libraryItem in libraryList)
                    {
                        var select = connection.Table<DataTables.LibraryItemTb>().FirstOrDefault(a => a.SectionId == libraryItem.SectionId);
                        if (select != null)
                        {
                            select.SectionId = libraryItem.SectionId;
                            select.SectionText = libraryItem.SectionText;
                            select.VideoCount = libraryItem.VideoCount;
                            select.BackgroundImage = libraryItem.BackgroundImage;
                            select.Icon = libraryItem.Icon;
                            select.BackgroundColor = libraryItem.BackgroundColor;

                            connection.Update(select);
                        }
                        else
                        {
                            var item = new DataTables.LibraryItemTb
                            {
                                SectionId = libraryItem.SectionId,
                                SectionText = libraryItem.SectionText,
                                VideoCount = libraryItem.VideoCount,
                                BackgroundImage = libraryItem.BackgroundImage,
                                Icon = libraryItem.Icon,
                                BackgroundColor = libraryItem.BackgroundColor,
                            };
                            connection.Insert(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertLibraryItem(libraryList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data LibraryItem
        public ObservableCollection<DataTables.LibraryItemTb> Get_LibraryItem()
        {
            try
            {
                var connection = OpenConnection();
                var select = connection.Table<DataTables.LibraryItemTb>().OrderBy(a => a.SectionId).ToList();
                if (select.Count > 0)
                {
                    return new ObservableCollection<DataTables.LibraryItemTb>(select);
                }

                return new ObservableCollection<DataTables.LibraryItemTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_LibraryItem();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<DataTables.LibraryItemTb>();
                } 
            }
        }

        #endregion

        #region Shared Videos

        //Insert Shared Videos
        public void Insert_SharedVideos(VideoDataObject video)
        {
            try
            {
                var connection = OpenConnection();
                if (video != null)
                {
                    var select = connection.Table<DataTables.SharedVideosTb>().FirstOrDefault(a => a.VideoId == video.VideoId);
                    if (select == null)
                    {
                        var db = ClassMapper.Mapper?.Map<DataTables.SharedVideosTb>(video);
                        db.Owner = JsonConvert.SerializeObject(video.Owner?.OwnerClass);
                        connection.Insert(db);
                    }
                    else
                    {
                        select = ClassMapper.Mapper?.Map<DataTables.SharedVideosTb>(video);
                        select.Owner = JsonConvert.SerializeObject(video.Owner?.OwnerClass);
                        connection.Update(select);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_SharedVideos(video);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get Shared Videos
        public ObservableCollection<VideoDataObject> Get_SharedVideos()
        {
            try
            {
                var connection = OpenConnection();
                var select = connection.Table<DataTables.SharedVideosTb>().ToList();
                if (select.Count > 0)
                {
                    var list = new ObservableCollection<VideoDataObject>();
                    foreach (var item in select)
                    {
                        var db = new VideoDataObject
                        {
                            Id = item.Id,
                            VideoId = item.VideoId,
                            UserId = item.UserId,
                            ShortId = item.ShortId,
                            Title = item.Title,
                            Description = item.Description,
                            Thumbnail = item.Thumbnail,
                            VideoLocation = item.VideoLocation,
                            Youtube = item.Youtube,
                            Vimeo = item.Vimeo,
                            Daily = item.Daily,
                            Facebook = item.Facebook,
                            Time = item.Time,
                            TimeDate = item.TimeDate,
                            Active = item.Active,
                            Tags = item.Tags,
                            Duration = item.Duration,
                            Size = item.Size,
                            Converted = item.Converted,
                            CategoryId = item.CategoryId,
                            Views = item.Views,
                            Featured = item.Featured,
                            Registered = item.Registered,
                            Type = item.Type,
                            Approved = item.Approved,
                            OrgThumbnail = item.OrgThumbnail,
                            VideoType = item.VideoType,
                            Source = item.Source,
                            Url = item.Url,
                            EditDescription = item.EditDescription,
                            MarkupDescription = item.MarkupDescription,
                            IsLiked = item.IsLiked,
                            IsDisliked = item.IsDisliked,
                            IsOwner = item.IsOwner,
                            TimeAlpha = item.TimeAlpha,
                            TimeAgo = item.TimeAgo,
                            CategoryName = item.CategoryName,
                            Likes = item.Likes,
                            Dislikes = item.Dislikes,
                            LikesPercent = item.LikesPercent,
                            DislikesPercent = item.DislikesPercent,
                            AgeRestriction = item.AgeRestriction,
                            Country = item.Country,
                            Demo = item.Demo,
                            GeoBlocking = item.GeoBlocking,
                            Gif = item.Gif,
                            IsMovie = item.IsMovie,
                            IsPurchased = item.IsPurchased,
                            MovieRelease = item.MovieRelease,
                            Ok = item.Ok,
                            PlaylistLink = item.PlaylistLink,
                            Privacy = item.Privacy,
                            Producer = item.Producer,
                            Quality = item.Quality,
                            Rating = item.Rating,
                            RentPrice = item.RentPrice,
                            SellVideo = item.SellVideo,
                            Stars = item.Stars,
                            SubCategory = item.SubCategory,
                            TrId = item.TrId,
                            Twitch = item.Twitch,
                            TwitchType = item.TwitchType,
                            DataVideoId = item.DataVideoId,
                            IsSubscribed = item.IsSubscribed,
                            Monetization = item.Monetization,
                            The1080P = item.The1080P,
                            The2048P = item.The2048P,
                            The240P = item.The240P,
                            The360P = item.The360P,
                            The4096P = item.The4096P,
                            The480P = item.The480P,
                            The720P = item.The720P,
                            AgoraResourceId = item.AgoraResourceId,
                            AgoraSid = item.AgoraSid,
                            HistoryId = item.HistoryId,
                            IsStock = item.IsStock,
                            License = item.License,
                            LiveEnded = item.LiveEnded,
                            LiveTime = item.LiveTime,
                            QualityVideoSelect = item.QualityVideoSelect,
                            StreamName = item.StreamName,
                            Video1080P = item.Video1080P,
                            Video2048P = item.Video2048P,
                            Video240P = item.Video240P,
                            Video360P = item.Video360P,
                            Video4096P = item.Video4096P,
                            Video480P = item.Video480P,
                            Video720P = item.Video720P,
                            VideoAuto = item.VideoAuto,
                            AgoraToken = item.AgoraToken,
                            AjaxUrl = item.AjaxUrl,
                            Embedding = item.Embedding,
                            IsShort = item.IsShort,
                            LiveChating = item.LiveChating,
                            PausedTime = item.PausedTime,
                            PublicationDate = item.PublicationDate,
                            Trailer = item.Trailer,
                            Owner = new UserDataObject(),
                        };

                        if (!string.IsNullOrEmpty(item.Owner))
                            db.Owner = new OwnerUnion
                            {
                                OwnerClass = JsonConvert.DeserializeObject<UserDataObject>(item.Owner)
                            };

                        list.Add(db);
                    }

                    return list;
                }

                return new ObservableCollection<VideoDataObject>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_SharedVideos();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<VideoDataObject>();
                } 
            }
        }

        #endregion
        
        #region Last Chat

        //Insert data To Last Chat Table
        public void InsertOrReplaceLastChatTable(ObservableCollection<GetChatsObject.Data> usersContactList)
        {
            try
            {
                var connection = OpenConnection();
                var result = connection.Table<DataTables.LastChatTb>().ToList();
                List<DataTables.LastChatTb> list = new List<DataTables.LastChatTb>();
                foreach (var info in usersContactList)
                {
                    var user = new DataTables.LastChatTb
                    {
                        Id = info.Id,
                        UserOne = info.UserOne,
                        UserTwo = info.UserTwo,
                        Time = info.Time,
                        TextTime = info.TextTime,
                        GetCountSeen = info.GetCountSeen,
                        UserDataJson = JsonConvert.SerializeObject(info.User),
                        GetLastMessageJson = JsonConvert.SerializeObject(info.GetLastMessage),
                    };

                    list.Add(user);

                    var update = result.FirstOrDefault(a => a.Id == info.Id);
                    if (update != null)
                    {
                        update = user;
                        connection.Update(update);
                    }
                }

                if (list.Count > 0)
                {
                    connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (newItemList.Count > 0)
                    {
                        connection.InsertAll(newItemList);
                    }

                    var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (deleteItemList.Count > 0)
                    {
                        foreach (var delete in deleteItemList)
                        {
                            connection.Delete(delete);
                        }
                    }

                    connection?.Commit();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrReplaceLastChatTable(usersContactList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To LastChat Table
        public ObservableCollection<GetChatsObject.Data> GetAllLastChat()
        {
            try
            {
                var connection = OpenConnection();
                var select = connection.Table<DataTables.LastChatTb>().ToList();
                if (select.Count > 0)
                {
                    var list = select.Select(user => new GetChatsObject.Data
                    {
                        Id = user.Id,
                        UserOne = user.UserOne,
                        UserTwo = user.UserTwo,
                        Time = user.Time,
                        TextTime = user.TextTime,
                        GetCountSeen = user.GetCountSeen,
                        User = JsonConvert.DeserializeObject<UserDataObject>(user.UserDataJson),
                        GetLastMessage =
                            JsonConvert.DeserializeObject<GetChatsObject.GetLastMessage>(user.GetLastMessageJson),
                    }).ToList();

                    return new ObservableCollection<GetChatsObject.Data>(list);
                }

                return new ObservableCollection<GetChatsObject.Data>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetAllLastChat();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<GetChatsObject.Data>();
                } 
            }
        }

        // Get data To LastChat Table By Id >> Load More
        public ObservableCollection<GetChatsObject.Data> GetLastChatById(int id, int nSize)
        {
            try
            {
                var connection = OpenConnection();
                var query = connection.Table<DataTables.LastChatTb>().Where(w => w.AutoIdLastChat >= id)
                    .OrderBy(q => q.AutoIdLastChat).Take(nSize).ToList();
                if (query.Count > 0)
                {
                    var list = query.Select(user => new GetChatsObject.Data
                    {
                        Id = user.Id,
                        UserOne = user.UserOne,
                        UserTwo = user.UserTwo,
                        Time = user.Time,
                        TextTime = user.TextTime,
                        GetCountSeen = user.GetCountSeen,
                        User = JsonConvert.DeserializeObject<UserDataObject>(user.UserDataJson),
                        GetLastMessage =
                            JsonConvert.DeserializeObject<GetChatsObject.GetLastMessage>(user.GetLastMessageJson),
                    }).ToList();

                    if (list.Count > 0)
                        return new ObservableCollection<GetChatsObject.Data>(list);
                    return null!;
                }

                return new ObservableCollection<GetChatsObject.Data>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetLastChatById(id, nSize);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<GetChatsObject.Data>();
                } 
            }
        }

        //Remove data To LastChat Table
        public void DeleteUserLastChat(string userId)
        {
            try
            {
                var connection = OpenConnection();
                var user = connection.Table<DataTables.LastChatTb>()
                    .FirstOrDefault(c => c.UserTwo.ToString() == userId);
                if (user != null)
                {
                    connection.Delete(user);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteUserLastChat(userId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Clear All data LastChat
        public void ClearLastChat()
        {
            try
            {
                var connection = OpenConnection();
                connection.DeleteAll<DataTables.LastChatTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearLastChat();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Message

        //Insert data To Message Table
        public void InsertOrReplaceMessages(ObservableCollection<GetUserMessagesObject.Message> messageList)
        {
            try
            {
                var connection = OpenConnection();
                var listOfDatabaseForInsert = new List<DataTables.MessageTb>();

                // get data from database
                var resultMessage = connection.Table<DataTables.MessageTb>().ToList();
                var listAllMessage = resultMessage.Select(messages => new GetUserMessagesObject.Message
                {
                    Id = messages.Id,
                    FromId = messages.FromId,
                    ToId = messages.ToId,
                    Text = messages.Text,
                    Seen = messages.Seen,
                    Time = messages.Time,
                    FromDeleted = messages.FromDeleted,
                    ToDeleted = messages.ToDeleted,
                    TextTime = messages.TextTime,
                    Position = messages.Position,
                }).ToList();

                foreach (var messages in messageList)
                {
                    var maTb = new DataTables.MessageTb
                    {
                        Id = messages.Id,
                        FromId = messages.FromId,
                        ToId = messages.ToId,
                        Text = messages.Text,
                        Seen = messages.Seen,
                        Time = messages.Time,
                        FromDeleted = messages.FromDeleted,
                        ToDeleted = messages.ToDeleted,
                        TextTime = messages.TextTime,
                        Position = messages.Position,
                    };

                    var dataCheck = listAllMessage.FirstOrDefault(a => a.Id == messages.Id);
                    if (dataCheck != null)
                    {
                        var checkForUpdate = resultMessage.FirstOrDefault(a => a.Id == dataCheck.Id);
                        if (checkForUpdate != null)
                        {
                            checkForUpdate.Id = messages.Id;
                            checkForUpdate.FromId = messages.FromId;
                            checkForUpdate.ToId = messages.ToId;
                            checkForUpdate.Text = messages.Text;
                            checkForUpdate.Seen = messages.Seen;
                            checkForUpdate.Time = messages.Time;
                            checkForUpdate.FromDeleted = messages.FromDeleted;
                            checkForUpdate.ToDeleted = messages.ToDeleted;
                            checkForUpdate.TextTime = messages.TextTime;
                            checkForUpdate.Position = messages.Position;

                            connection.Update(checkForUpdate);
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }
                    else
                    {
                        listOfDatabaseForInsert.Add(maTb);
                    }
                }

                connection.BeginTransaction();

                //Bring new  
                if (listOfDatabaseForInsert.Count > 0)
                {
                    connection.InsertAll(listOfDatabaseForInsert);
                }

                connection?.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrReplaceMessages(messageList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Update one Messages Table
        public void InsertOrUpdateToOneMessages(GetUserMessagesObject.Message messages)
        {
            try
            {
                var connection = OpenConnection();
                var data = connection.Table<DataTables.MessageTb>().FirstOrDefault(a => a.Id == messages.Id);
                if (data != null)
                {
                    data.Id = messages.Id;
                    data.FromId = messages.FromId;
                    data.ToId = messages.ToId;
                    data.Text = messages.Text;
                    data.Seen = messages.Seen;
                    data.Time = messages.Time;
                    data.FromDeleted = messages.FromDeleted;
                    data.ToDeleted = messages.ToDeleted;
                    data.TextTime = messages.TextTime;
                    data.Position = messages.Position;

                    connection.Update(data);
                }
                else
                {
                    var mdb = new DataTables.MessageTb
                    {
                        Id = messages.Id,
                        FromId = messages.FromId,
                        ToId = messages.ToId,
                        Text = messages.Text,
                        Seen = messages.Seen,
                        Time = messages.Time,
                        FromDeleted = messages.FromDeleted,
                        ToDeleted = messages.ToDeleted,
                        TextTime = messages.TextTime,
                        Position = messages.Position,
                    };

                    //Insert  one Messages Table
                    connection.Insert(mdb);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateToOneMessages(messages);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To Messages
        public string GetMessagesList(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                var connection = OpenConnection();
                var beforeQ = "";
                if (beforeMessageId != "0")
                {
                    beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                }

                var query = connection.Query<DataTables.MessageTb>(
                    "SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" +
                    toId + " and ToId=" + fromId + ")) " + beforeQ);
                var queryList = query
                    .Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId)
                    .OrderBy(q => q.Time).TakeLast(35).ToList();
                if (queryList.Count > 0)
                {
                    foreach (var messages in queryList)
                    {
                        var m = new GetUserMessagesObject.Message
                        {
                            Id = messages.Id,
                            FromId = messages.FromId,
                            ToId = messages.ToId,
                            Text = messages.Text,
                            Seen = messages.Seen,
                            Time = messages.Time,
                            FromDeleted = messages.FromDeleted,
                            ToDeleted = messages.ToDeleted,
                            TextTime = messages.TextTime,
                            Position = messages.Position,
                        };

                        if (beforeMessageId == "0")
                        {
                            MessagesBoxActivity.MAdapter?.Add(m);
                        }
                        else
                        {
                            MessagesBoxActivity.MAdapter?.Insert(m, beforeMessageId);
                        }
                    }

                    return "1";
                }

                return "0";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetMessagesList(fromId, toId, beforeMessageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return "0";
                }  
            }
        }

        //Get data To where first Messages >> load more
        public List<DataTables.MessageTb> GetMessageList(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                var connection = OpenConnection();
                var beforeQ = "";
                if (beforeMessageId != "0")
                {
                    beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                }

                var query = connection.Query<DataTables.MessageTb>(
                    "SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" +
                    toId + " and ToId=" + fromId + ")) " + beforeQ);
                var queryList = query
                    .Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId)
                    .OrderBy(q => q.Time).TakeLast(35).ToList();
                return queryList;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetMessageList(fromId, toId, beforeMessageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(string messageId)
        {
            try
            {
                var connection = OpenConnection();
                var user = connection.Table<DataTables.MessageTb>().FirstOrDefault(c => c.Id == messageId);
                if (user != null)
                {
                    connection.Delete(user);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Delete_OneMessageUser(messageId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteAllMessagesUser(string fromId, string toId)
        {
            try
            {
                var connection = OpenConnection();
                var query = connection.Query<DataTables.MessageTb>(
                    "Delete FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" +
                    toId + " and ToId=" + fromId + "))");
                Console.WriteLine(query);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllMessagesUser(fromId, toId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Remove All data To Messages Table
        public void ClearAll_Messages()
        {
            try
            {
                var connection = OpenConnection();
                connection.DeleteAll<DataTables.MessageTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearAll_Messages();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
    }
}