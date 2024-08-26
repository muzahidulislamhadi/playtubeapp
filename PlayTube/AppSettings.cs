//###############################################################
// Author >> Elin Doughouz
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System.Collections.Generic;
using PlayTube.Helpers.Models;

namespace PlayTube
{
    internal static class AppSettings
    {
        /// <summary>
        /// Deep Links To App Content
        /// you should add your website without http in the analytic.xml file >> ../values/analytic.xml .. line 5
        /// <string name="ApplicationUrlWeb">demo.playtubescript.com</string>
        /// </summary>
        public static readonly string TripleDesAppServiceProvider = "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";
       
        //Main Settings >>>>> 
        //********************************************************* 
        public static readonly string ApplicationName = "PlayTube";
        public static readonly string DatabaseName = "PlayTubeVideos"; 
        public static string Version = "3.1";

        //Main Colors >>
        //*********************************************************
        public static readonly string MainColor = "#0F64F7";

        public static readonly bool LinkWithTv = true; //#New

        public static readonly PlayerTheme PlayerTheme = PlayerTheme.Theme1; //#New
         
        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //Set Language User on site from phone 
        public static readonly bool SetLangUser = false;
        
        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "e06a3585-d0ac-44ef-b2df-0c24abc23682";

        //AdMob >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static readonly ShowAds ShowAds = ShowAds.UnProfessional; 

        //Three times after entering the ad is displayed
        public static readonly int ShowAdInterstitialCount = 3;
        public static readonly int ShowAdRewardedVideoCount = 3;
        public static readonly int ShowAdNativeCount = 5;
        public static readonly int ShowAdAppOpenCount = 2;
        
        public static readonly bool ShowAdMobBanner = true;
        public static readonly bool ShowAdMobInterstitial = true;
        public static readonly bool ShowAdMobRewardVideo = true;
        public static readonly bool ShowAdMobNative = true;
        public static readonly bool ShowAdMobAppOpen = true;  
        public static readonly bool ShowAdMobRewardedInterstitial = true; 

        public static readonly string AdInterstitialKey = "ca-app-pub-5135691635931982/6168068662";
        public static readonly string AdRewardVideoKey = "ca-app-pub-5135691635931982/4663415300";
        public static readonly string AdAdMobNativeKey = "ca-app-pub-5135691635931982/2619721801";
        public static readonly string AdAdMobAppOpenKey = "ca-app-pub-5135691635931982/4967593321"; 
        public static readonly string AdRewardedInterstitialKey = "ca-app-pub-5135691635931982/1850136085";  

        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static readonly bool ShowFbBannerAds = false; 
        public static readonly bool ShowFbInterstitialAds = false; 
        public static readonly bool ShowFbRewardVideoAds = false; 
        public static readonly bool ShowFbNativeAds = false; 

        //YOUR_PLACEMENT_ID
        public static readonly string AdsFbBannerKey = "250485588986218_554026418632132"; 
        public static readonly string AdsFbInterstitialKey = "250485588986218_554026125298828"; 
        public static readonly string AdsFbRewardVideoKey = "250485588986218_554072818627492";  
        public static readonly string AdsFbNativeKey = "250485588986218_554706301897477";

        //Colony Ads >> Please add the code ad in the Here 
        //*********************************************************  
        public static readonly bool ShowColonyBannerAds = true;  
        public static readonly bool ShowColonyInterstitialAds = true; 
        public static readonly bool ShowColonyRewardAds = true;  

        public static readonly string AdsColonyAppId = "app6fa8d492324841b9b5";  
        public static readonly string AdsColonyBannerId = "vz8f1309aa856842248e";  
        public static readonly string AdsColonyInterstitialId = "vzd4f625080a1b4bc0be";  
        public static readonly string AdsColonyRewardedId = "vzb00816befb614810ac";

        //*********************************************************   

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file or AndroidManifest.xml
        //Facebook >> ../values/analytic.xml  
        //Google >> ../Properties/AndroidManifest.xml .. line 27
        //*********************************************************
        public static readonly bool EnableSmartLockForPasswords = false; //#New
         
        public static readonly bool ShowFacebookLogin = true;
        public static readonly bool ShowGoogleLogin = true; 
        public static readonly bool ShowWoWonderLogin = true; 

        public static readonly string AppNameWoWonder = "WoWonder"; 
        public static readonly string WoWonderDomainUri = "https://demo.wowonder.com"; 
        public static readonly string WoWonderAppKey = "131c471c8b4edf662dd0ebf7adf3c3d7365838b9"; 

        public static readonly string ClientId = "404363570731-j48d139m31tgaq2tj0gamg8ah430botj.apps.googleusercontent.com";

        //First Page
        //*********************************************************
        public static readonly bool ShowSkipButton = true; 
         
        public static readonly bool ShowRegisterButton = true; 
        public static readonly bool EnablePhoneNumber = false; 

        //Set Theme Full Screen App
        //*********************************************************
        public static readonly bool EnableFullScreenApp = false;
        public static readonly bool EnablePictureToPictureMode = false; //>> Not Working >> Next update 

        //Data Channal Users >> About
        //*********************************************************
        public static readonly bool ShowEmailAccount = true;
        public static readonly bool ShowActivities = true; 

        //Tab >> 
        //*********************************************************
        public static readonly bool ShowArticle = true;
        public static readonly bool ShowMovies = true;
        public static readonly bool ShowShorts = true; //#New 
        public static readonly bool ShowChannelPopular = true; //#New 
         
        //how in search 
        public static readonly List<string> LastKeyWordList = new List<string>() { "Music", "Party", "Nature", "Snow", "Entertainment", "Holidays", "Covid19", "Comedy", "Politics", "Suspense" }; //#New 

        //Offline Watched Videos >>  
        //*********************************************************
        public static readonly bool AllowOfflineDownload = true;
        public static readonly bool AllowDownloadProUser = true;
        public static readonly bool AllowWatchLater = true; 
        public static readonly bool AllowRecentlyWatched = true; 
        public static readonly bool AllowPlayLists = true; 
        public static readonly bool AllowLiked = true; 
        public static readonly bool AllowShared = true; 
        public static readonly bool AllowPaid = true; 

        //Import && Upload Videos >>  
        //*********************************************************
        public static bool ShowButtonImport { get; set; } = true;
        public static bool ShowButtonUpload { get; set; } = true;

        //Last_Messages Page >>
        ///********************************************************* 
        public static readonly bool RunSoundControl = true;
        public static readonly int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static readonly int MessageRequestSpeed = 3000; // 3 Seconds
         
        public static readonly int ShowButtonSkip = 10; // 6 Seconds 
         
        //Set Theme App >> Color - Tab
        //*********************************************************
        public static TabTheme SetTabDarkTheme = TabTheme.Light;

        public static readonly bool SetYoutubeTypeBadgeIcon = true;
        public static readonly bool SetVimeoTypeBadgeIcon = true;
        public static readonly bool SetDailyMotionTypeBadgeIcon = true;
        public static readonly bool SetTwichTypeBadgeIcon = true;
        public static readonly bool SetOkTypeBadgeIcon = true;
        public static readonly bool SetFacebookTypeBadgeIcon = true;

        //Bypass Web Errors 
        ///*********************************************************
        public static readonly bool TurnTrustFailureOnWebException = true;
        public static readonly bool TurnSecurityProtocolType3072On = true;

        //*********************************************************
        public static readonly bool RenderPriorityFastPostLoad = true;

        //Error Report Mode
        //*********************************************************
        public static readonly bool SetApisReportMode = false;
         
        public static readonly int AvatarSize = 60; 
        public static readonly int ImageSize = 400;

        //Home Page 
        //*********************************************************
        public static readonly bool ShowStockVideo = true;
        
        public static readonly int CountVideosTop = 10;  
        public static readonly int CountVideosLatest = 10;  
        public static readonly int CountVideosFav = 10;
        public static readonly int CountVideosLive = 13;
        public static readonly int CountVideosStock= 10;

        /// <summary>
        /// if Radius you can select how much Radius in the parameter #CardPlayerViewRadius
        /// </summary>
        public static readonly CardPlayerView CardPlayerView  = CardPlayerView.Square; 
        public static readonly float CardPlayerViewRadius = 10F;  

        public static readonly bool ShowGoLive = true;
        public static readonly string AppIdAgoraLive = "9471c47b589c4a35abf3f7338ef18629";

        //Settings 
        //*********************************************************
        public static readonly bool ShowEditPassword = true; 
        public static readonly bool ShowMonetization = true; //(Withdrawals)
        public static readonly bool ShowVerification = true; 
        public static readonly bool ShowBlockedUsers = true; 
        public static readonly bool ShowPoints = true; 
        public static readonly bool ShowSettingsTwoFactor = true;   
        public static readonly bool ShowSettingsManageSessions = true;   

        public static readonly bool ShowSettingsRateApp = true;  
        public static readonly int ShowRateAppCount = 5;  
         
        public static readonly bool ShowSettingsUpdateManagerApp = false; 

        public static readonly bool ShowGoPro = true; 

        public static readonly bool ShowClearHistory = true; 
        public static readonly bool ShowClearCache = true; 
         
        public static readonly bool ShowHelp = true; 
        public static readonly bool ShowTermsOfUse = true; 
        public static readonly bool ShowAbout = true; 
        public static readonly bool ShowDeleteAccount = true;

        //*********************************************************
        /// <summary>
        /// Currency
        /// CurrencyStatic = true : get currency from app not api 
        /// CurrencyStatic = false : get currency from api (default)
        /// </summary>
        public static readonly bool CurrencyStatic = false; 
        public static readonly string CurrencyIconStatic = "$"; 
        public static readonly string CurrencyCodeStatic = "USD"; 

        //********************************************************* 
        public static readonly bool RentVideosSystem = true; 
        
        //*********************************************************  
        public static readonly bool DonateVideosSystem = true;  
         
        //*********************************************************  
        /// <summary>
        /// Paypal and google pay using Braintree Gateway https://www.braintreepayments.com/
        /// 
        /// Add info keys in Payment Methods : https://prnt.sc/1z5bffc & https://prnt.sc/1z5b0yj
        /// To find your merchant ID :  https://prnt.sc/1z59dy8
        ///
        /// Tokenization Keys : https://prnt.sc/1z59smv
        /// </summary>
        public static readonly bool ShowPaypal = true;
        public static readonly string MerchantAccountId = "test"; 

        public static readonly string SandboxTokenizationKey = "sandbox_kt2f6mdh_hf4c******"; 
        public static readonly string ProductionTokenizationKey = "production_t2wns2y2_dfy45******";  

        public static readonly bool ShowCreditCard = true;
        public static readonly bool ShowBankTransfer = true;  

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static readonly bool ShowInAppBilling = false;

        //*********************************************************   
        public static readonly bool ShowCashFree = true;   
         
        /// <summary>
        /// Currencies : http://prntscr.com/u600ok
        /// </summary>
        public static readonly string CashFreeCurrency = "INR";  

        /// <summary>
        /// Currencies : https://razorpay.com/accept-international-payments
        /// </summary>
        public static readonly string RazorPayCurrency = "INR"; 

        /// <summary>
        /// If you want RazorPay you should change id key in the analytic.xml file
        /// razorpay_api_Key >> .. line 18 
        /// </summary>
        public static readonly bool ShowRazorPay = true;  

        public static readonly bool ShowPayStack = true;  
        public static readonly bool ShowPaySera = true;   
        public static readonly bool ShowSecurionPay = true; //#New
        public static readonly bool ShowAuthorizeNet = true; //#New 
        public static readonly bool ShowIyziPay = true; //#New 
         
        //*********************************************************  

        public static readonly bool ShowVideoWithDynamicHeight = true;

        //********************************************************* 
        public static readonly bool ShowTextWithSpace = true; 

    }
}