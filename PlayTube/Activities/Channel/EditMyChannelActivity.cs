using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads.DoubleClick;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.TextField;
using PlayTube.Activities.Base;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.RestCalls;
using TheArtOfDev.Edmodo.Cropper;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using PlayTube.Helpers.CacheLoaders;
using MaterialDialogsCore;
using AndroidHUD;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using File = Java.IO.File;
using Uri = Android.Net.Uri;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace PlayTube.Activities.Channel
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class EditMyChannelActivity : BaseActivity , MaterialDialog.IListCallback, MaterialDialog.IListCallbackMultiChoice
    {
        #region Variables Basic

        private ImageView ImageCover, ImageAvatar;
        private LinearLayout ChangeCoverLayout;
        private RelativeLayout ChangeAvatarLayout;

        private TextInputEditText TxtUsername, TxtFullName, TxtEmail, TxtAbout, TxtFavCategory, TxtGender, TxtAge, TxtCountry, TxtFacebook, TxtTwitter;
        private AppCompatButton SaveButton;

        private PublisherAdView PublisherAdView;
        private string ImageType, GenderStatus, Age, CountryId;
        private string CategoryId, CategoryName, DialogType;
        private List<string> CategorySelect = new List<string>();

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
                SetContentView(Resource.Layout.EditMyChannelLayout);
                 
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
                PublisherAdView?.Resume();
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
                PublisherAdView?.Pause();
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
                PublisherAdView?.Destroy();
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
                ImageCover = FindViewById<ImageView>(Resource.Id.imageCover);
                ChangeCoverLayout = FindViewById<LinearLayout>(Resource.Id.ChangeCoverLayout);
                 
                ImageAvatar = FindViewById<ImageView>(Resource.Id.imageAvatar);
                ChangeAvatarLayout = FindViewById<RelativeLayout>(Resource.Id.ChangeAvatarLayout);

                TxtUsername = FindViewById<TextInputEditText>(Resource.Id.usernameEdit);
                TxtFullName = FindViewById<TextInputEditText>(Resource.Id.FullNameEdit);
                TxtEmail = FindViewById<TextInputEditText>(Resource.Id.emailEdit);
                TxtAbout = FindViewById<TextInputEditText>(Resource.Id.aboutEdit);
                TxtFavCategory = FindViewById<TextInputEditText>(Resource.Id.favCategoryEdit);
                TxtGender = FindViewById<TextInputEditText>(Resource.Id.genderEdit);
                TxtAge = FindViewById<TextInputEditText>(Resource.Id.ageEdit);
                TxtCountry = FindViewById<TextInputEditText>(Resource.Id.countryEdit);
                TxtFacebook = FindViewById<TextInputEditText>(Resource.Id.facebookEdit);
                TxtTwitter = FindViewById<TextInputEditText>(Resource.Id.twitterEdit);
           
                SaveButton = FindViewById<AppCompatButton>(Resource.Id.SaveButton);

                PublisherAdView = FindViewById<PublisherAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitPublisherAdView(PublisherAdView);

                Methods.SetFocusable(TxtFavCategory);
                Methods.SetFocusable(TxtGender);
                Methods.SetFocusable(TxtAge);
                Methods.SetFocusable(TxtCountry);
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
                    ChangeCoverLayout.Click += ChangeCoverLayoutOnClick;
                    ChangeAvatarLayout.Click += ChangeAvatarLayoutOnClick;
                    TxtFavCategory.Touch += TxtFavCategoryOnTouch;
                    TxtGender.Touch += TxtGenderOnTouch;
                    TxtAge.Touch += TxtAgeOnTouch;
                    TxtCountry.Touch += TxtCountryOnTouch;
                    SaveButton.Click += SaveButtonOnClick;
                }
                else
                {
                    ChangeCoverLayout.Click -= ChangeCoverLayoutOnClick;
                    ChangeAvatarLayout.Click -= ChangeAvatarLayoutOnClick;
                    TxtFavCategory.Touch -= TxtFavCategoryOnTouch;
                    TxtGender.Touch -= TxtGenderOnTouch;
                    TxtAge.Touch -= TxtAgeOnTouch;
                    TxtCountry.Touch -= TxtCountryOnTouch;
                    SaveButton.Click -= SaveButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events
         
        private void ChangeAvatarLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenDialogGallery("Avatar");
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ChangeCoverLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenDialogGallery("Cover");
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtFavCategoryOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                DialogType = "FavCategory";

                var dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
              
                var arrayAdapter = CategoriesController.ListCategories.Select(item => Methods.FunString.DecodeString(item.Name)).ToList();
                var arrayIndexAdapter = new List<int>();
                if (CategorySelect?.Count > 0)
                {
                    arrayIndexAdapter.AddRange(CategorySelect.Select(t => CategoriesController.ListCategories.IndexOf(CategoriesController.ListCategories.FirstOrDefault(c => c.Id == t))));
                }
                else
                {
                    var local = ListUtils.MyChannelList?.FirstOrDefault();
                    if (local?.FavCategory?.Count > 0)
                    {
                        arrayIndexAdapter.AddRange(local?.FavCategory.Select(t => CategoriesController.ListCategories.IndexOf(CategoriesController.ListCategories.FirstOrDefault(c => c.Id == t))));
                    }
                }

                dialogList.Title(GetText(Resource.String.Lbl_ChooseFavCategory))
                    .Items(arrayAdapter)
                    .ItemsCallbackMultiChoice(arrayIndexAdapter.ToArray(), this)
                    .AlwaysCallMultiChoiceCallback()
                    .PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(new MyMaterialDialog())
                    .Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        private void TxtCountryOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                DialogType = "Country";
                 
                var dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                var countriesArray = AppTools.GetCountryList(this);
                var arrayAdapter = countriesArray.Select(item => item.Value).ToList();

                dialogList.Title(GetText(Resource.String.Lbl_Country));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtAgeOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                DialogType = "Age";

                var arrayAdapter = Enumerable.Range(1, 99).ToList();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                dialogList.Title(GetText(Resource.String.Lbl_Age));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtGenderOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                DialogType = "Gender";
                List<string> arrayAdapter = new List<string>();
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(this).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                arrayAdapter.Add(GetText(Resource.String.Radio_Male));
                arrayAdapter.Add(GetText(Resource.String.Radio_Female));

                dialogList.Title(GetText(Resource.String.Lbl_Gender));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        private async void SaveButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                  
                    string first = "", last = ""; 
                    var name = TxtFullName.Text?.Split(' ');
                    if (name?.Length > 0)
                    {
                        first = name.FirstOrDefault();
                        last = name.LastOrDefault();
                    }

                    var dictionary = new Dictionary<string, string>
                    {
                        {"settings_type", "general"},
                        {"username", TxtUsername.Text},
                        {"email", TxtEmail.Text},
                        {"first_name", first},
                        {"last_name", last},
                        {"about", TxtAbout.Text},
                        {"facebook", TxtFacebook.Text},
                        {"twitter", TxtTwitter.Text},
                        //{"google", TxtGoogle.Text},
                        {"gender", GenderStatus},
                        {"age", Age},
                        {"fav_category", CategoryId},
                        {"country", CountryId}
                    };

                    var (apiResult, respond) = await RequestsAsync.Global.UpdateUserDataGeneralAsync(dictionary);
                    if (apiResult == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            var local = ListUtils.MyChannelList?.FirstOrDefault();
                            if (local != null)
                            {
                                local.Username = UserDetails.Username = TxtUsername.Text;
                                local.Email = UserDetails.Email = TxtEmail.Text;
                                local.FirstName = first;
                                local.LastName = last;
                                local.About = TxtAbout.Text;
                                local.Gender = GenderStatus;
                                local.Facebook = TxtFacebook.Text;
                                local.Twitter = TxtTwitter.Text;
                                //local.Google = TxtGoogle.Text;
                                local.FavCategory = CategorySelect;
                                local.Age = Age;
                                local.CountryId = CountryId;
                                local.CountryName = TxtCountry.Text;

                                var database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyChannel(local);
                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short)?.Show();
                            AndHUD.Shared.Dismiss(this);

                            Intent intent = new Intent();
                            SetResult(Result.Ok, intent);
                            Finish();
                        }
                    }
                    else
                    {
                        Methods.DisplayAndHudErrorResult(this, respond);
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
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
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok && result.IsSuccessful)
                    {
                        var resultUri = result.Uri;

                        if (!string.IsNullOrEmpty(resultUri.Path))
                        { 
                            File file2 = new File(resultUri.Path);
                            var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);

                            if (ImageType == "Avatar")
                            {
                                Glide.With(this).Load(photoUri).Apply(GlideImageLoader.GetOptions(ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser)).Into(ImageAvatar);
                            }
                            else if (ImageType == "Cover")
                            {
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(ImageCover);
                            }

                            //Send image function
                            if (Methods.CheckConnectivity())
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.UpdateUserDataImageAsync(resultUri.Path, ImageType.ToLower()) });
                            else
                                Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
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
                        OpenDialogGallery(ImageType);
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

        private void OpenDialogGallery(string type)
        {
            try
            {
                ImageType = type;
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
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, DateTime.Now.ToShortDateString() + ".jpg"));
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
                            var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, DateTime.Now.ToShortDateString() + ".jpg"));
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
         
        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                switch (DialogType)
                {
                    case "Gender":
                        TxtGender.Text = itemString;
                        GenderStatus = position == 0 ? "male" : "female";
                        break;
                    case "Age":
                        TxtAge.Text = itemString;
                        Age = itemString;
                        break;
                    case "Country":
                        var countriesArray = AppTools.GetCountryList(this);
                        var check = countriesArray.FirstOrDefault(a => a.Value == itemString).Key;
                        if (check != null)
                        {
                            CountryId = check;
                        }

                        TxtCountry.Text = itemString;
                        break;
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public bool OnSelection(MaterialDialog dialog, int[] which, string[] text)
        {
            try
            {
                CategoryId = "";
                CategoryName = "";
                CategorySelect = new List<string>();

                foreach (var t in which)
                {
                    CategoryId += CategoriesController.ListCategories[t].Id + ",";
                    CategoryName += CategoriesController.ListCategories[t].Name + ",";

                    CategorySelect.Add(CategoryId);
                }

                TxtFavCategory.Text = CategoryName.Remove(CategoryName.Length - 1, 1);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return true;
            }
            return true;
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
                    GlideImageLoader.LoadImage(this, local.Avatar, ImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);
                    GlideImageLoader.LoadImage(this, local.Cover, ImageCover, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                     
                    if (local.Gender == "male" || local.Gender == "Male")
                    {
                        GenderStatus = "male";
                        TxtGender.Text = GetText(Resource.String.Radio_Male);
                    }
                    else
                    {

                        GenderStatus = "female";
                        TxtGender.Text = GetText(Resource.String.Radio_Female);
                    }

                    TxtUsername.Text = local.Username;
                    TxtEmail.Text = local.Email;
                    TxtFullName.Text = local.FirstName + " " + local.LastName;
                    TxtAbout.Text = local.About;
                    TxtFacebook.Text = local.Facebook;
                    TxtTwitter.Text = local.Twitter;

                    TxtAge.Text = local.Age == "0" ? GetText(Resource.String.Lbl_Age) : local.Age;
                    Age = local.Age;

                    TxtCountry.Text = local.CountryName;
                    CountryId = local.CountryId;

                    if (local?.FavCategory?.Count > 0)
                    {
                        CategorySelect = local.FavCategory;
                        foreach (var t in local.FavCategory)
                        {
                            CategoryId += t + ",";
                            CategoryName += CategoriesController.ListCategories.FirstOrDefault(q => q.Id == t)?.Name + ",";
                        }

                        TxtFavCategory.Text = CategoryName.Remove(CategoryName.Length - 1, 1);
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