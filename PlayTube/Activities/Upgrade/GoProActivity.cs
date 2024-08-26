using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialDialogsCore;
using Android.App;
using Android.BillingClient.Api;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using InAppBilling.Lib;
using PlayTube.Activities.SettingsPreferences.General;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTube.PaymentGoogle;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using BaseActivity = PlayTube.Activities.Base.BaseActivity;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using AndroidX.AppCompat.Widget; 

namespace PlayTube.Activities.Upgrade
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode | ConfigChanges.Locale)]
    public class GoProActivity : BaseActivity, IBillingPaymentListener, MaterialDialog.IListCallback
    {
        #region Variables Basic
         
        private TextView HeadText, PriceProMember, PriceFreeMember;
        private AppCompatButton BtnUpgrade;

        private BillingSupport BillingSupport;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.GoProLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar(); 

                if (AppSettings.ShowInAppBilling && InitializePlayTube.IsExtended)
                    BillingSupport = new BillingSupport(this, InAppBillingGoogle.ProductId, AppSettings.TripleDesAppServiceProvider, InAppBillingGoogle.ListProductSku, this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
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

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions
        
        private void InitComponent()
        {
            try
            { 
                HeadText = FindViewById<TextView>(Resource.Id.headText);
                PriceFreeMember = FindViewById<TextView>(Resource.Id.priceFreeMember);
                PriceProMember = FindViewById<TextView>(Resource.Id.priceProMember);

                HeadText.Text = GetText(Resource.String.Lbl_Title_Pro1) + " " + AppSettings.ApplicationName + " " + GetText(Resource.String.Lbl_Title_Pro2);

                BtnUpgrade = FindViewById<AppCompatButton>(Resource.Id.btn_upgrade);
                 
                var idCurrency = ListUtils.MySettingsList?.PaypalCurrency ?? "USD";
                var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                Console.WriteLine(currency);

                PriceProMember.Text = currencyIcon + ListUtils.MySettingsList?.ProPkgPrice;
                PriceFreeMember.Text = currencyIcon + "0";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = " ";
                    toolbar.SetTitleTextColor(AppTools.IsTabDark() ? Color.White : Color.Black);

                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                    icon?.SetTint(AppTools.IsTabDark() ? Color.White : Color.Black);
                    SupportActionBar.SetHomeAsUpIndicator(icon);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -= 
                if (addEvent)
                {
                    BtnUpgrade.Click += UpgradeButtonOnClick;
                }
                else
                {
                    BtnUpgrade.Click -= UpgradeButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void UpgradeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.ShowInAppBilling && InitializePlayTube.IsExtended)
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                    arrayAdapter.Add(GetString(Resource.String.Lbl_Wallet));

                    arrayAdapter.Add(GetString(Resource.String.Lbl_GooglePlay));

                    dialogList.Title(GetText(Resource.String.Lbl_PurchaseRequired));
                    dialogList.Items(arrayAdapter);
                    dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    var dialog = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                    dialog.Title(Resource.String.Lbl_PurchaseRequired);
                    dialog.Content(GetText(Resource.String.Lbl_Go_Pro));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Purchase)).OnPositive(async (materialDialog, action) =>
                    {
                        try
                        {
                            if (AppTools.CheckWallet())
                            {
                                if (Methods.CheckConnectivity())
                                {
                                    var (apiStatus, respond) = await RequestsAsync.Payment.PurchaseAsync("pro");
                                    if (apiStatus == 200)
                                    {
                                        if (respond is MessageObject result)
                                        {
                                            Console.WriteLine(result.Message);

                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { SetProApi });
                                        }
                                    }
                                    else Methods.DisplayReportResult(this, respond);
                                }
                                else
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                            }
                            else
                            {
                                var dialogBuilder = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                                dialogBuilder.Title(GetText(Resource.String.Lbl_Wallet));
                                dialogBuilder.Content(GetText(Resource.String.Lbl_Error_NoWallet));
                                dialogBuilder.PositiveText(GetText(Resource.String.Lbl_AddWallet)).OnPositive((materialDialog, action) =>
                                {
                                    try
                                    {
                                        StartActivity(new Intent(this, typeof(WalletActivity)));
                                    }
                                    catch (Exception exception)
                                    {
                                        Methods.DisplayReportResultTrack(exception);
                                    }
                                });
                                dialogBuilder.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                                dialogBuilder.AlwaysCallSingleChoiceCallback();
                                dialogBuilder.Build().Show();
                            }
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }
                    });
                    dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion
         
        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string text)
        {
            try
            {
                if (text == GetString(Resource.String.Lbl_Wallet))
                {
                    var dialogBuilder = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                    dialogBuilder.Title(Resource.String.Lbl_PurchaseRequired);
                    dialogBuilder.Content(GetText(Resource.String.Lbl_Go_Pro));
                    dialogBuilder.PositiveText(GetText(Resource.String.Lbl_Purchase)).OnPositive(async (materialDialog, action) =>
                    {
                        try
                        {
                            if (AppTools.CheckWallet())
                            {
                                if (Methods.CheckConnectivity())
                                {
                                    var (apiStatus, respond) = await RequestsAsync.Payment.PurchaseAsync("pro");
                                    if (apiStatus == 200)
                                    {
                                        if (respond is MessageObject result)
                                        {
                                            Console.WriteLine(result.Message);

                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { SetProApi });
                                        }
                                    }
                                    else Methods.DisplayReportResult(this, respond);
                                }
                                else
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                            }
                            else
                            {
                                var dialogTheme = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                                dialogTheme.Title(GetText(Resource.String.Lbl_Wallet));
                                dialogTheme.Content(GetText(Resource.String.Lbl_Error_NoWallet));
                                dialogTheme.PositiveText(GetText(Resource.String.Lbl_AddWallet)).OnPositive((materialDialog, action) =>
                                {
                                    try
                                    {
                                        StartActivity(new Intent(this, typeof(WalletActivity)));
                                    }
                                    catch (Exception exception)
                                    {
                                        Methods.DisplayReportResultTrack(exception);
                                    }
                                });
                                dialogTheme.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                                dialogTheme.AlwaysCallSingleChoiceCallback();
                                dialogTheme.Build().Show();
                            }
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }
                    });
                    dialogBuilder.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                    dialogBuilder.AlwaysCallSingleChoiceCallback();
                    dialogBuilder.Build().Show();
                }
                else if (text == GetString(Resource.String.Lbl_GooglePlay))
                {
                    BillingSupport?.PurchaseNow("membership");
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Billing

        public void OnPaymentError(string error)
        {
            Console.WriteLine(error);
        }

        public void OnPaymentSuccess(IList<Purchase> result)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { SetProApi });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private async Task SetProApi()
        {
            if (Methods.CheckConnectivity())
            {
                var (apiStatus, respond) = await RequestsAsync.Global.UpgradeAsync();
                if (apiStatus == 200)
                {
                    var dataUser = ListUtils.MyChannelList?.FirstOrDefault();
                    if (dataUser != null)
                    {
                        dataUser.IsPro = "1";

                        var sqlEntity = new SqLiteDatabase();
                        sqlEntity.InsertOrUpdate_DataMyChannel(dataUser);
                    }

                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long)?.Show();
                    Finish();
                }
                else Methods.DisplayReportResult(this, respond);
            }
            else
            {
                Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
            }
        }
    }
}