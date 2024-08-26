using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Graphics;
using Android.Text;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using Com.Razorpay;
using Google.Android.Material.TextField;
using IyziPay;
using IyziPay.Lib.Model;
using MaterialDialogsCore;
using Newtonsoft.Json;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.Payment;
using PlayTubeClient;
using PlayTubeClient.Classes.Payment;
using PlayTubeClient.RestCalls; 
using SecurionPay;
using BaseActivity = PlayTube.Activities.Base.BaseActivity;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace PlayTube.Activities.SettingsPreferences.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class WalletActivity : BaseActivity, MaterialDialog.IListCallback, IPaymentResultWithDataListener, ISecurionPayPaymentListener, IIyziPayPaymentListener
    {
        #region Variables Basic

        private ImageView Avatar;
        private TextView TxtProfileName, TxtUsername;

        private TextView TxtMyBalance;
        public TextInputEditText TxtAmount;
        private AppCompatButton BtnReplenish;
        private InitPayPalPayment InitPayPalPayment;
        private InitPayStackPayment PayStackPayment;
        private InitCashFreePayment CashFreePayment;
        private InitRazorPayPayment InitRazorPay;
        private InitPaySeraPayment PaySeraPayment;
        private InitSecurionPayPayment SecurionPayPayment;
        private InitIyziPayPayment IyziPayPayment;
        private string Price;
        private static WalletActivity Instance;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Methods.App.FullScreenApp(this);

                SetTheme(AppTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.WalletLayout);

                Instance = this;

                //Get Value And Set Toolbar
                InitBuy();
                InitComponent();
                InitToolbar();
                Get_Data_User();

                AdsGoogle.Ad_AdMobNative(this);
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

        protected override void OnDestroy()
        {
            try
            {
                if (AppSettings.ShowRazorPay) InitRazorPay?.StopRazorPay();
                if (AppSettings.ShowPayStack) PayStackPayment?.StopPayStack();
                if (AppSettings.ShowCashFree) CashFreePayment?.StopCashFree();
                if (AppSettings.ShowPaySera) PaySeraPayment?.StopPaySera();
                if (AppSettings.ShowSecurionPay) SecurionPayPayment?.StopSecurionPay();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitBuy()
        {
            try
            {
                if (AppSettings.ShowPaypal) InitPayPalPayment ??= new InitPayPalPayment(this);
                if (AppSettings.ShowRazorPay) InitRazorPay ??= new InitRazorPayPayment(this);
                if (AppSettings.ShowPayStack) PayStackPayment ??= new InitPayStackPayment(this);
                if (AppSettings.ShowCashFree) CashFreePayment ??= new InitCashFreePayment(this);
                if (AppSettings.ShowPaySera) PaySeraPayment ??= new InitPaySeraPayment(this);
                if (AppSettings.ShowSecurionPay) SecurionPayPayment ??= new InitSecurionPayPayment(this, this, ListUtils.MySettingsList?.SecurionpayPublicKey);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitComponent()
        {
            try
            {
                Avatar = FindViewById<ImageView>(Resource.Id.avatar);
                TxtProfileName = FindViewById<TextView>(Resource.Id.name);
                TxtUsername = FindViewById<TextView>(Resource.Id.tv_subname);

                TxtMyBalance = FindViewById<TextView>(Resource.Id.myBalance);

                TxtAmount = FindViewById<TextInputEditText>(Resource.Id.AmountEditText);
                BtnReplenish = FindViewById<AppCompatButton>(Resource.Id.ReplenishButton);

                Methods.SetColorEditText(TxtAmount, AppTools.IsTabDark() ? Color.White : Color.Black);

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
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
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
                    BtnReplenish.Click += BtnReplenishOnClick;
                }
                else
                {
                    BtnReplenish.Click -= BtnReplenishOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static WalletActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        #endregion

        #region Events

        private void BtnReplenishOnClick(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtAmount.Text) || string.IsNullOrWhiteSpace(TxtAmount.Text) || Convert.ToInt32(TxtAmount.Text) == 0)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_PleaseEnterAmount), ToastLength.Long)?.Show();
                    return;
                }

                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                    return;
                }

                Price = TxtAmount.Text;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                if (AppSettings.ShowPaypal) arrayAdapter.Add(GetString(Resource.String.Lbl_Paypal));
                if (AppSettings.ShowCreditCard) arrayAdapter.Add(GetString(Resource.String.Lbl_CreditCard));
                if (AppSettings.ShowBankTransfer) arrayAdapter.Add(GetString(Resource.String.Lbl_BankTransfer));
                if (AppSettings.ShowRazorPay) arrayAdapter.Add(GetString(Resource.String.Lbl_RazorPay));
                if (AppSettings.ShowPayStack) arrayAdapter.Add(GetString(Resource.String.Lbl_PayStack));
                if (AppSettings.ShowCashFree) arrayAdapter.Add(GetString(Resource.String.Lbl_CashFree));
                if (AppSettings.ShowPaySera) arrayAdapter.Add(GetString(Resource.String.Lbl_PaySera));
                if (AppSettings.ShowSecurionPay) arrayAdapter.Add(GetString(Resource.String.Lbl_SecurionPay));
                if (AppSettings.ShowAuthorizeNet) arrayAdapter.Add(GetString(Resource.String.Lbl_AuthorizeNet));
                if (AppSettings.ShowIyziPay) arrayAdapter.Add(GetString(Resource.String.Lbl_IyziPay));
                 
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public async void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                string text = itemString;
                if (text == GetString(Resource.String.Lbl_Paypal))
                {
                    InitPayPalPayment.BtnPaypalOnClick(Price);
                }
                else if (text == GetString(Resource.String.Lbl_CreditCard))
                {
                    OpenIntentCreditCard();
                }
                else if (text == GetString(Resource.String.Lbl_BankTransfer))
                {
                    OpenIntentBankTransfer();
                }
                else if (text == GetString(Resource.String.Lbl_RazorPay))
                {
                    InitRazorPay?.BtnRazorPayOnClick(Price);
                }
                else if (text == GetString(Resource.String.Lbl_PayStack))
                {
                    OpenPayStackDialog();
                }
                else if (text == GetString(Resource.String.Lbl_CashFree))
                {
                    OpenCashFreeDialog();
                }
                else if (text == GetString(Resource.String.Lbl_PaySera))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_wait), ToastLength.Long)?.Show();

                    await PaySera();
                }
                else if (text == GetString(Resource.String.Lbl_SecurionPay))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_wait), ToastLength.Long)?.Show();

                    await SecurionPay();
                }
                else if (text == GetString(Resource.String.Lbl_AuthorizeNet))
                {
                    OpenIntentAuthorizeNet();
                } 
                else if (text == GetString(Resource.String.Lbl_IyziPay))
                {
                    IyziPay();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OpenIntentCreditCard()
        {
            try
            {
                Intent intent = new Intent(this, typeof(PaymentCardDetailsActivity));
                intent.PutExtra("Price", Price);
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OpenIntentBankTransfer()
        {
            try
            {
                Intent intent = new Intent(this, typeof(PaymentLocalActivity));
                intent.PutExtra("Id", "");
                intent.PutExtra("Price", Price);
                intent.PutExtra("payType", "wallet");
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
        private void OpenIntentAuthorizeNet()
        {
            try
            {
                Intent intent = new Intent(this, typeof(AuthorizeNetPaymentActivity));
                intent.PutExtra("Price", Price);
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Result

        //Result 
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 2325)
                {
                    if (resultCode == Result.Ok)
                    {
                        //if (data != null)
                        //{
                        //    var token = RU.Yoomoney.Sdk.Kassa.Payments.Checkout.CreateTokenizationResult(data).PaymentToken;
                        //    Toast.MakeText(this, "tokenization_success", ToastLength.Short)?.Show(); 
                        //}
                        //else
                        //{
                        //    Toast.MakeText(this, "tokenization_canceled", ToastLength.Short)?.Show();
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region RazorPay
          
        public void OnPaymentError(int code, string response, PaymentData p2)
        {
            try
            {
                Console.WriteLine("razorpay : Payment failed: " + code + " " + response);
                Toast.MakeText(this, "Payment failed: " + response, ToastLength.Long)?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void OnPaymentSuccess(string razorpayPaymentId, PaymentData p1)
        {
            try
            {
                Console.WriteLine("razorpay : Payment Successful:" + razorpayPaymentId);

                if (!string.IsNullOrEmpty(razorpayPaymentId) && Methods.CheckConnectivity())
                {
                    var priceInt = Convert.ToInt32(Price) * 100;

                    var (apiStatus, respond) = await RequestsAsync.Payment.RazorPayAsync(razorpayPaymentId, "wallet", priceInt.ToString());
                    if (apiStatus == 200) 
                    {
                        TxtAmount.Text = string.Empty;
                        Toast.MakeText(this, GetText(Resource.String.Lbl_PaymentSuccessfully), ToastLength.Long)?.Show();
                    }
                    else
                        Methods.DisplayReportResult(this, respond);
                }
                else if (!string.IsNullOrEmpty(razorpayPaymentId))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region CashFree

        private EditText TxtName, TxtEmail, TxtPhone;
        private void OpenCashFreeDialog()
        {
            try
            {
                var dialog = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light)
                    .Title(GetText(Resource.String.Lbl_CashFree)).TitleColorRes(Resource.Color.primary)
                    .CustomView(Resource.Layout.CashFreePaymentLayout, true)
                    .PositiveText(GetText(Resource.String.Lbl_PayNow)).OnPositive(async (materialDialog, action) =>
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrWhiteSpace(TxtName.Text))
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_name), ToastLength.Short)?.Show();
                                return;
                            }

                            var check = Methods.FunString.IsEmailValid(TxtEmail.Text.Replace(" ", ""));
                            switch (check)
                            {
                                case false:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                                    return;
                            }

                            if (string.IsNullOrEmpty(TxtPhone.Text) || string.IsNullOrWhiteSpace(TxtPhone.Text))
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_your_data), ToastLength.Short)?.Show();
                                return;
                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Please_wait), ToastLength.Short)?.Show();

                            await CashFree(TxtName.Text, TxtEmail.Text, TxtPhone.Text);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    })
                    .NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog())
                    .Build();

                var iconName = dialog.CustomView.FindViewById<TextView>(Resource.Id.IconName);
                TxtName = dialog.CustomView.FindViewById<EditText>(Resource.Id.NameEditText);

                var iconEmail = dialog.CustomView.FindViewById<TextView>(Resource.Id.IconEmail);
                TxtEmail = dialog.CustomView.FindViewById<EditText>(Resource.Id.EmailEditText);

                var iconPhone = dialog.CustomView.FindViewById<TextView>(Resource.Id.IconPhone);
                TxtPhone = dialog.CustomView.FindViewById<EditText>(Resource.Id.PhoneEditText);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, iconName, FontAwesomeIcon.User);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, iconEmail, FontAwesomeIcon.PaperPlane);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, iconPhone, FontAwesomeIcon.Mobile);

                Methods.SetColorEditText(TxtName, AppTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtEmail, AppTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtPhone, AppTools.IsTabDark() ? Color.White : Color.Black);

                var local = ListUtils.MyChannelList?.FirstOrDefault();
                if (local != null)
                {
                    TxtName.Text = AppTools.GetNameFinal(local);
                    TxtEmail.Text = local.Email;
                    TxtPhone.Text = local.PhoneNumber;
                }

                dialog.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task CashFree(string name, string email, string phone)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var keyValues = new Dictionary<string, string>
                    {
                        {"name", name},
                        {"phone", phone},
                        {"email", email},
                        {"amount", Price},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Payment.InitializeCashFreeAsync("wallet", AppSettings.CashFreeCurrency, ListUtils.MySettingsList?.CashfreeSecretKey ?? "", ListUtils.MySettingsList?.CashfreeMode, keyValues);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                switch (respond)
                                {
                                    case CashFreeObject result:
                                        CashFreePayment ??= new InitCashFreePayment(this);
                                        CashFreePayment.DisplayCashFreePayment(result, Price);
                                        break;
                                }

                                break;
                            }
                        default:
                            Methods.DisplayReportResult(this, respond);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region PayStack

        private void OpenPayStackDialog()
        {
            try
            {
                var dialogBuilder = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                dialogBuilder.Title(Resource.String.Lbl_PayStack).TitleColorRes(Resource.Color.primary);
                dialogBuilder.Input(Resource.String.Lbl_Email, 0, false, async (materialDialog, s) =>
                {
                    try
                    {
                        switch (s.Length)
                        {
                            case <= 0:
                                return;
                        }

                        var check = Methods.FunString.IsEmailValid(s.ToString().Replace(" ", ""));
                        switch (check)
                        {
                            case false:
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                                return;
                            default:
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Please_wait), ToastLength.Long)?.Show();

                                await PayStack(s.ToString());
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });
                dialogBuilder.InputType(InputTypes.TextVariationEmailAddress);
                dialogBuilder.PositiveText(GetText(Resource.String.Lbl_PayNow)).OnPositive((materialDialog, action) =>
                {

                });
                dialogBuilder.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialogBuilder.AlwaysCallSingleChoiceCallback();
                dialogBuilder.Build().Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task PayStack(string email)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var priceInt = Convert.ToInt32(Price) * 100;

                    var keyValues = new Dictionary<string, string>
                    {
                        {"email", email},
                        {"amount", priceInt.ToString()},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Payment.InitializePayStackAsync("wallet", keyValues);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                switch (respond)
                                {
                                    case InitializePaymentObject result:
                                        PayStackPayment ??= new InitPayStackPayment(this);
                                        PayStackPayment.DisplayPayStackPayment(result.Url, priceInt.ToString());
                                        break;
                                }

                                break;
                            }
                        default:
                            Methods.DisplayReportResult(this, respond);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region PaySera

        private async Task PaySera()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var keyValues = new Dictionary<string, string>
                    {
                        {"amount", Price},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Payment.InitializePaySeraAsync("wallet", keyValues);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                switch (respond)
                                {
                                    case InitializePaymentObject result:
                                        PaySeraPayment ??= new InitPaySeraPayment(this);
                                        PaySeraPayment.DisplayPaySeraPayment(result.Url, Price);
                                        break;
                                }

                                break;
                            }
                        default:
                            Methods.DisplayReportResult(this, respond);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region SecurionPay

        private async Task SecurionPay()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Payment.InitializeSecurionPayAsync("securionpay_token", Price);
                    switch (apiStatus)
                    {
                        case 200:
                            {
                                switch (respond)
                                {
                                    case InitializeSecurionPayObject result:
                                        SecurionPayPayment ??= new InitSecurionPayPayment(this, this, ListUtils.MySettingsList?.SecurionpayPublicKey);
                                        SecurionPayPayment.DisplaySecurionPayPayment(result.Token, Price, AppTools.IsTabDark() ? Resource.Style.MyDialogThemeDark : Resource.Style.MyDialogTheme);
                                        break;
                                }

                                break;
                            }
                        default:
                            Methods.DisplayReportResult(this, respond);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task SecurionPay(string request, string charge)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Payment.SecurionPayAsync(request, charge);
                    switch (apiStatus)
                    {
                        case 200:
                            Toast.MakeText(this, GetText(Resource.String.Lbl_PaymentSuccessfully), ToastLength.Long)?.Show();

                            break;
                        default:
                            Methods.DisplayReportResult(this, respond);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnPaymentError(string error)
        {
            try
            {

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnPaymentSuccess(SecurionPayResult result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result?.Charge?.Id))
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => SecurionPay("securionpay_handle", JsonConvert.SerializeObject(result.Charge)) });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region IyziPay

        private void IyziPay()
        {
            try
            {
                IyziPayPaymentObject request = new IyziPayPaymentObject()
                {
                    Locale = Locale.TR,

                    Id = ListUtils.MySettingsList.IyzipayBuyerId,
                    Name = ListUtils.MySettingsList.IyzipayBuyerName,
                    Surname = ListUtils.MySettingsList.IyzipayBuyerSurname,
                    GsmNumber = ListUtils.MySettingsList.IyzipayBuyerGsmNumber,
                    Email = ListUtils.MySettingsList.IyzipayBuyerEmail,
                    IdentityNumber = ListUtils.MySettingsList.IyzipayIdentityNumber,
                    Address = ListUtils.MySettingsList.IyzipayAddress,
                    City = ListUtils.MySettingsList.IyzipayCity,
                    Country = ListUtils.MySettingsList.IyzipayCountry,
                    Zip = ListUtils.MySettingsList.IyzipayZip,
                    
                    Price = Price,
                    Currency = Currency.TRY,
                    CallbackUrl = InitializePlayTube.WebsiteUrl + "/requests.php?f=iyzipay&s=success&amount="+ Price,

                    ApiKey = ListUtils.MySettingsList.IyzipayKey,
                    SecretKey = ListUtils.MySettingsList.IyzipaySecretKey,
                    BaseUrl = ListUtils.MySettingsList.IyzipayMode == "0" ? "https://merchant.iyzipay.com/" : "https://sandbox-api.iyzipay.com/"
                };
                 
                IyziPayPayment ??= new InitIyziPayPayment(this,this, request);
                IyziPayPayment.DisplayIyziPayPayment(Price, AppTools.IsTabDark() ? Resource.Style.MyDialogThemeDark : Resource.Style.MyDialogTheme);
                //string Token = IyziPayPayment.CheckoutFormInitialize?.Token; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
          
        public void OnIyziPayPaymentSuccess(CheckoutFormInitialize result)
        {
            try
            { 
                if (!string.IsNullOrEmpty(result?.Token))
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => IyziPay(result?.Token) }); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task IyziPay(string token)
        {
            try
            {
                if (Methods.CheckConnectivity())
                { 
                    var (apiStatus, respond) = await RequestsAsync.Payment.IyziPayAsync("wallet", token, Price);
                    switch (apiStatus)
                    {
                        case 200:
                            Toast.MakeText(this, GetText(Resource.String.Lbl_PaymentSuccessfully), ToastLength.Long)?.Show();

                            IyziPayPayment.StopIyziPay();
                            break;
                        default:
                            Methods.DisplayReportResult(this, respond);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region YooMoney

        //private async Task YooMoney()
        //{
        //    try
        //    {
        //        if (Methods.CheckConnectivity())
        //        {
        //            var (apiStatus, respond) = await RequestsAsync.Payment.InitializeYooMoneyAsync("create_yoomoney", Price);
        //            switch (apiStatus)
        //            {
        //                case 200:
        //                {
        //                    switch (respond)
        //                    {
        //                        case InitializeYooMoneyObject result:
        //                           // OpenIntentYooMoney();
        //                            break;
        //                    } 
        //                    break;
        //                }
        //                default:
        //                    Methods.DisplayReportResult(this, respond);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //    }
        //}

        private void OpenIntentYooMoney()
        {
            try
            {
                //var amount = new Amount(BigDecimal.ValueOf(10.0), Currency.GetInstance("RUB"));
                //var title = "product name";
                //var subtitle = "product description";
                //var clientApplicationKey = "";
                //var shopId = "";
                //var savePaymentMethod = SavePaymentMethod.Off;
                //var paymentMethodTypes = new List<PaymentMethodType>() { PaymentMethodType.GooglePay, PaymentMethodType.BankCard, PaymentMethodType.Sberbank, PaymentMethodType.YooMoney };
                //var gatewayId = "";
                //var customReturnUrl = "test_redirect_url";
                //var userPhoneNumber = "test_phone_number";
                //var googlePayParameters = new GooglePayParameters();
                //var authCenterClientId = "";
                 
                //var paymentParameters = new PaymentParameters(amount, title, subtitle, clientApplicationKey, shopId, savePaymentMethod, paymentMethodTypes, gatewayId, customReturnUrl, userPhoneNumber, googlePayParameters, authCenterClientId);
                //var intent = RU.Yoomoney.Sdk.Kassa.Payments.Checkout.CreateTokenizeIntent(this, paymentParameters, new TestParameters(true));
                //StartActivityForResult(intent, 2325);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        #endregion

        private async void Get_Data_User()
        {
            try
            {
                if (ListUtils.MyChannelList?.Count == 0)
                    await ApiRequest.GetChannelData(this, UserDetails.UserId);

                var local = ListUtils.MyChannelList?.FirstOrDefault();
                if (local != null)
                {
                    GlideImageLoader.LoadImage(this, local.Avatar, Avatar, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);
                    TxtProfileName.Text = AppTools.GetNameFinal(local);
                    TxtUsername.Text = "@" + local.Username;

                    var success = double.TryParse(local.Wallet, out var number);
                    if (success)
                    {
                        TxtMyBalance.Text = number.ToString("F");
                    }
                    else
                    {
                        TxtMyBalance.Text = local.Wallet;
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