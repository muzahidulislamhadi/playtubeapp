using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Google.Android.Material.TextField;
using PlayTube.Activities.Base;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using TheArtOfDev.Edmodo.Cropper;
using Console = System.Console;
using File = Java.IO.File;
using Uri = Android.Net.Uri;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace PlayTube.Activities.SettingsPreferences.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class VerificationActivity : BaseActivity
    {
        #region Variables Basic

        private ImageView PassportImage;
        private AppCompatButton BtnAddImage, BtnSubmit;
        private TextInputEditText FirstNameEdit, LastNameEdit, MessagesEdit;
        private string PathImage = "";
        private LinearLayout VerifiedLinear, NotVerifiedLinear;
        private TextView VerifiedIcon , TextTitileVerified;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                SetTheme(AppTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.VerificationLayout);
                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                Get_Data_User();
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
                PassportImage = FindViewById<ImageView>(Resource.Id.Image); 
                BtnAddImage = FindViewById<AppCompatButton>(Resource.Id.btn_AddPhoto);
                FirstNameEdit = FindViewById<TextInputEditText>(Resource.Id.FirstName_text);
                LastNameEdit = FindViewById<TextInputEditText>(Resource.Id.LastName_text);
                MessagesEdit = FindViewById<TextInputEditText>(Resource.Id.Messages_Edit);
                BtnSubmit = FindViewById<AppCompatButton>(Resource.Id.submitButton);

                TextTitileVerified = FindViewById<TextView>(Resource.Id.textTitileVerified);
                VerifiedIcon = FindViewById<TextView>(Resource.Id.verifiedIcon);
                VerifiedLinear = FindViewById<LinearLayout>(Resource.Id.verified);
                NotVerifiedLinear = FindViewById<LinearLayout>(Resource.Id.notVerified);
                  
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, VerifiedIcon, IonIconsFonts.CheckmarkCircle);
                VerifiedIcon.SetTextColor(Color.ParseColor(AppSettings.MainColor));
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
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = " ";

                    toolBar.SetTitleTextColor(AppTools.IsTabDark() ? Color.White : Color.Black);

                    SetSupportActionBar(toolBar);
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
         
        public void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnAddImage.Click += BtnAddImageOnClick;
                    BtnSubmit.Click += BtnSubmitOnClick;
                }
                else
                {
                    BtnAddImage.Click -= BtnAddImageOnClick;
                    BtnSubmit.Click -= BtnSubmitOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        #endregion

        #region Events

        private async void BtnSubmitOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {  
                    if (string.IsNullOrEmpty(FirstNameEdit.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_firstname), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(LastNameEdit.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_lastname), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(MessagesEdit.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_Messages), ToastLength.Short)?.Show();
                        return;
                    }
                     
                    if (string.IsNullOrEmpty(PathImage))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Image), ToastLength.Short)?.Show();
                    }
                    else
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");
 
                        var (apiStatus, respond) = await RequestsAsync.Global.VerificationAsync(FirstNameEdit.Text, LastNameEdit.Text, MessagesEdit.Text, PathImage);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageObject result)
                            {
                                Console.WriteLine(result.Message);
                                AndHUD.Shared.Dismiss(this);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_ErrorVerification), ToastLength.Short)?.Show();
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);
                       
                        AndHUD.Shared.Dismiss(this);
                    }
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnAddImageOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenGallery();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                PathImage = ""; 
                GlideImageLoader.LoadImage(this, "ImagePlacholder", PassportImage ,ImageStyle.CenterCrop,ImagePlaceholders.Drawable );
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        #endregion

        #region Permissions && Result

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //If its from Camera or Gallery  
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    CropImage.ActivityResult result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok && result.IsSuccessful)
                    {
                        Uri resultUri = result.Uri;

                        if (!string.IsNullOrEmpty(resultUri.Path))
                        {
                            PathImage = resultUri.Path;
                            File file2 = new File(resultUri.Path);
                            var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                            Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(PassportImage);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Short)?.Show();
                        }
                    } 
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 108) //Image Picker
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Open Image 
                        OpenGallery();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void OpenGallery()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpg"));
                        CropImage.Activity()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                            CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                        {
                            Methods.Path.Chack_MyFolder();

                            //Open Image 
                            var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpg"));
                            CropImage.Activity()
                                .SetInitialCropWindowPaddingRatio(0)
                                .SetAutoZoomEnabled(true)
                                .SetMaxZoom(4)
                                .SetGuidelines(CropImageView.Guidelines.On)
                                .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                                .SetOutputUri(myUri).Start(this);
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                }
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
                    await ApiRequest.GetChannelData(this,UserDetails.UserId);

                var local = ListUtils.MyChannelList?.FirstOrDefault();
                if (local != null)
                {
                    if (local.Verified == "1")
                    {
                        VerifiedLinear.Visibility = ViewStates.Visible;
                        NotVerifiedLinear.Visibility = ViewStates.Gone;
                        TextTitileVerified.Text = GetText(Resource.String.Lbl_WelcomeTo) + " " + AppSettings.ApplicationName;
                    }
                    else
                    {
                        VerifiedLinear.Visibility = ViewStates.Gone;
                        NotVerifiedLinear.Visibility = ViewStates.Visible; 
                        TextTitileVerified.Text = GetText(Resource.String.Lbl_Please_select_Image_passport);
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