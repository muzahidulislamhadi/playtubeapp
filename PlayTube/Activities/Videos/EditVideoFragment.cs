using System;
using System.Collections.Generic;
using System.Linq;
using MaterialDialogsCore;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AT.Markushi.UI;
using TheArtOfDev.Edmodo.Cropper;
using Java.IO;
using Newtonsoft.Json;
using PlayTube.Activities.PlayersView;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Console = System.Console;
using Exception = System.Exception;
using Uri = Android.Net.Uri;
using Fragment = AndroidX.Fragment.App.Fragment;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace PlayTube.Activities.Videos
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class EditVideoFragment : Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public ImageView Image;
        private CircleButton BtnClose;
        private AppCompatButton BtnSelectImage, BtnSave;
        private TextView IconTitle, IconDescription, IconTags, IconCategory, IconPrivacy, IconAgeRestriction, DeleteVideoButton;
        private EditText TitleEditText, DescriptionEditText, TagsEditText, CategoryEditText, PrivacyEditText, AgeRestrictionEditText;
        private string  Privacy = "0", TypeDialog = "", IdCategory = "", IdAgeRestriction = "";
        public string PathImage = "";
        private VideoDataObject VideoClass;
        private TabbedMainActivity GlobalContext;
        private static EditVideoFragment Instance;
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GlobalContext = TabbedMainActivity.GetInstance();
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater?.Inflate(Resource.Layout.EditVideoLayout, container, false);
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
                Instance = this;

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                AddOrRemoveEvent(true);

                SetData();
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
                Image = view.FindViewById<ImageView>(Resource.Id.Image);
                BtnClose = view.FindViewById<CircleButton>(Resource.Id.ImageCircle);
                BtnSelectImage = view.FindViewById<AppCompatButton>(Resource.Id.btn_AddPhoto);

                IconTitle = view.FindViewById<TextView>(Resource.Id.IconTitle);
                TitleEditText = view.FindViewById<EditText>(Resource.Id.TitleEditText);

                IconDescription = view.FindViewById<TextView>(Resource.Id.IconDescription);
                DescriptionEditText = view.FindViewById<EditText>(Resource.Id.DescriptionEditText);

                IconTags = view.FindViewById<TextView>(Resource.Id.IconTags);
                TagsEditText = view.FindViewById<EditText>(Resource.Id.TagsEditText);

                IconCategory = view.FindViewById<TextView>(Resource.Id.IconCategory);
                CategoryEditText = view.FindViewById<EditText>(Resource.Id.CategoryEditText);

                IconPrivacy = view.FindViewById<TextView>(Resource.Id.IconPrivacy);
                PrivacyEditText = view.FindViewById<EditText>(Resource.Id.PrivacyEditText);

                IconAgeRestriction = view.FindViewById<TextView>(Resource.Id.IconAgeRestriction);
                AgeRestrictionEditText = view.FindViewById<EditText>(Resource.Id.AgeRestrictionEditText);

                BtnSave = view.FindViewById<AppCompatButton>(Resource.Id.ApplyButton);
                DeleteVideoButton = view.FindViewById<TextView>(Resource.Id.DeleteVideoButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconTitle, FontAwesomeIcon.TextWidth);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconTags, FontAwesomeIcon.Tags);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconDescription, FontAwesomeIcon.AudioDescription);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconCategory, FontAwesomeIcon.LayerGroup);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconPrivacy, FontAwesomeIcon.ShieldAlt);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconAgeRestriction, FontAwesomeIcon.User);

                Methods.SetFocusable(CategoryEditText);
                Methods.SetFocusable(PrivacyEditText);
                Methods.SetFocusable(AgeRestrictionEditText); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar(View view)
        {
            try
            {
                var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    string title = GetString(Resource.String.Lbl_EditVideo); 
                    GlobalContext.SetToolBar(toolbar, title);
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
                    BtnClose.Click += BtnCloseOnClick;
                    BtnSelectImage.Click += BtnSelectImageOnClick;
                    BtnSave.Click += BtnSaveOnClick;
                    CategoryEditText.Touch += CategoryEditTextOnClick;
                    PrivacyEditText.Touch += PrivacyEditTextOnClick;
                    AgeRestrictionEditText.Touch += AgeRestrictionEditTextOnClick;
                    DeleteVideoButton.Click += DeleteVideoButtonOnClick;
                }
                else
                {
                    BtnClose.Click -= BtnCloseOnClick;
                    BtnSelectImage.Click -= BtnSelectImageOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
                    CategoryEditText.Touch -= CategoryEditTextOnClick;
                    PrivacyEditText.Touch -= PrivacyEditTextOnClick;
                    AgeRestrictionEditText.Touch -= AgeRestrictionEditTextOnClick;
                    DeleteVideoButton.Click -= DeleteVideoButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static EditVideoFragment GetInstance()
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

        private async void DeleteVideoButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    
                    //Show a progress
                    AndHUD.Shared.Show(Activity, Activity.GetString(Resource.String.Lbl_Loading));
 
                    var (apiStatus, respond) = await RequestsAsync.Video.DeleteVideosAsync(VideoClass.Id); //Sent api 
                    if (apiStatus.Equals(200))
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);

                            var mAdapter = TabbedMainActivity.GetInstance()?.MyChannelFragment?.VideosFragment?.MAdapter;
                            var data = mAdapter?.VideoList?.FirstOrDefault(a => a.Id == VideoClass.Id);
                            if (data != null)
                            {  
                                mAdapter?.VideoList.Remove(data);
                                mAdapter?.NotifyDataSetChanged();
                            } 

                            Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_Deleted), ToastLength.Short)?.Show();
                            AndHUD.Shared.Dismiss(Activity);

                            GlobalContext.FragmentNavigatorBack();
                        }
                    }
                    else
                    {
                        Methods.DisplayAndHudErrorResult(Activity, respond);
                    }

                    AndHUD.Shared.Dismiss(Activity);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss(Activity);
            }
        }

        //Click Save data  
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    if (string.IsNullOrEmpty(TitleEditText.Text))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PleaseEnterTitleVideo), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(DescriptionEditText.Text))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PleaseEnterDescriptionVideo), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TagsEditText.Text))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PleaseEnterTags), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(CategoryEditText.Text))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PleaseChooseCategory), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(PrivacyEditText.Text))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PleaseChoosePrivacy), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(AgeRestrictionEditText.Text))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_PleaseChooseAgeRestriction), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(PathImage))
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_Please_select_Image), ToastLength.Short)?.Show();
                        return;
                    }

                    //Show a progress
                    AndHUD.Shared.Show(Activity, Activity.GetString(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"title", TitleEditText.Text},
                        {"description", DescriptionEditText.Text},
                        {"tags", TagsEditText.Text},
                        {"category_id", IdCategory},
                        {"privacy", Privacy},
                        {"age_restriction", IdAgeRestriction},
                        //{"sub_category_id", ""},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Video.EditVideoAsync(VideoClass.Id, dictionary, PathImage); //Sent api 
                    if (apiStatus.Equals(200))
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);

                            var data = TabbedMainActivity.GetInstance()?.MyChannelFragment?.VideosFragment?.MAdapter?.VideoList?.FirstOrDefault(a => a.Id == VideoClass.Id);
                            if (data != null)
                            {
                                data.Title = TitleEditText.Text;
                                data.Description = DescriptionEditText.Text;
                                data.Tags = TagsEditText.Text;
                                data.CategoryName = CategoryEditText.Text;
                                data.CategoryId = IdCategory;
                                data.Privacy = Privacy;
                                data.AgeRestriction = IdAgeRestriction;
                                data.Thumbnail = PathImage;

                                TabbedMainActivity.GetInstance()?.MyChannelFragment?.VideosFragment?.MAdapter?.NotifyDataSetChanged();
                            }

                            GlobalPlayerActivity.GetInstance()?.SetNewDataVideo(data);
                            TabbedMainActivity.GetInstance()?.VideoDataWithEventsLoader?.SetNewDataVideo(data);

                            Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_YourVideoSuccessfullyEdited), ToastLength.Short)?.Show();
                            AndHUD.Shared.Dismiss(Activity);

                            GlobalContext.FragmentNavigatorBack();
                        }
                    }
                    else
                    {
                        Methods.DisplayAndHudErrorResult(Activity, respond);
                    }

                    AndHUD.Shared.Dismiss(Activity);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss(Activity);
            }
        }
         
        //Open Gallery 
        private void BtnSelectImageOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Remove image 
        private void BtnCloseOnClick(object sender, EventArgs e)
        {
            try
            {
                PathImage = "";
                GlideImageLoader.LoadImage(Activity, "Grey_Offline", Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Category
        private void CategoryEditTextOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "Category";

                var dialogList = new MaterialDialog.Builder(Activity).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                var arrayAdapter = CategoriesController.ListCategories.Select(item => Methods.FunString.DecodeString(item.Name)).ToList();

                dialogList.Title(GetText(Resource.String.Lbl_Category));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Privacy
        private void PrivacyEditTextOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "Privacy";

                var dialogList = new MaterialDialog.Builder(Activity).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                  
                var arrayAdapter = AppTools.GetPrivacyList(Activity).Select(item => item.Value).ToList();

                dialogList.Title(GetText(Resource.String.Lbl_Privacy));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
          
        //AgeRestriction
        private void AgeRestrictionEditTextOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "AgeRestriction";

                var dialogList = new MaterialDialog.Builder(Activity).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                var arrayAdapter = new List<string>
                {
                    GetString(Resource.String.Lbl_AgeRestrictionText0),
                    GetString(Resource.String.Lbl_AgeRestrictionText1)
                };

                dialogList.Title(GetText(Resource.String.Lbl_AgeRestriction));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion
         
        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                string text = itemString;

                switch (TypeDialog)
                {
                    case "Category":
                        IdCategory = CategoriesController.ListCategories[position]?.Id;
                        CategoryEditText.Text = text;
                        break;
                    case "Privacy":
                        Privacy = AppTools.GetPrivacyList(Activity)?.FirstOrDefault(a => a.Value == itemString).Key.ToString();
                        PrivacyEditText.Text = text;
                        break;
                    case "AgeRestriction":
                    {
                        if (text == GetString(Resource.String.Lbl_AgeRestrictionText0))
                        {
                            IdAgeRestriction = "1";
                        } 
                        else if (text == GetString(Resource.String.Lbl_AgeRestrictionText1))
                        {
                            IdAgeRestriction = "2";
                        }
                        AgeRestrictionEditText.Text = text;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private void SetData()
        {
            try
            {
                VideoClass = JsonConvert.DeserializeObject<VideoDataObject>(Arguments?.GetString("ItemDataVideo") ?? "");
                if (VideoClass == null) return;
                GlideImageLoader.LoadImage(Activity, VideoClass.Thumbnail, Image, ImageStyle.CenterCrop,ImagePlaceholders.Drawable);
                     
                TitleEditText.Text = Methods.FunString.DecodeString(VideoClass.Title);
                DescriptionEditText.Text = Methods.FunString.DecodeString(VideoClass.Description);
                TagsEditText.Text = VideoClass.Tags;
                CategoryEditText.Text = CategoriesController.GetCategoryName(VideoClass);

                AgeRestrictionEditText.Text = GetString(VideoClass.AgeRestriction == "1" ? Resource.String.Lbl_AgeRestrictionText0 : Resource.String.Lbl_AgeRestrictionText1);

                switch (VideoClass.Privacy)
                {
                    case "0": //Public 
                        PrivacyEditText.Text = GetText(Resource.String.Radio_public);
                        break;
                    case "1": //Private 
                        PrivacyEditText.Text = GetText(Resource.String.Radio_private);
                        break;
                    case "2": //Unlisted 
                        PrivacyEditText.Text = GetText(Resource.String.Lbl_Unlisted);
                        break;
                }

                Privacy = VideoClass.Privacy;
                IdCategory = VideoClass.CategoryId;
                IdAgeRestriction = VideoClass.AgeRestriction;
                PathImage = VideoClass.Thumbnail;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenDialogGallery()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
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
                            .SetOutputUri(myUri).Start(Activity);
                    }
                    else
                    {
                        if (!CropImage.IsExplicitCameraPermissionRequired(Activity) && Activity.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                            Activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && Activity.CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
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
                                .SetOutputUri(myUri).Start(Activity);
                        }
                        else
                        {
                            new PermissionsController(Activity).RequestPermission(108);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        } 
    }
}
 