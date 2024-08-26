//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using PlayTube.Activities.Base;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Playlist;
using PlayTubeClient.RestCalls;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace PlayTube.Activities.Playlist
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class CreatNewPlaylistActivity : BaseActivity
    {
        #region Variables Basic

        private EditText TxtNewplaylist, TxtDescription;
        private RadioButton RbPrivate, RbPublic;
        private TextView SaveTextView;
        private string Status = "", PlaylistId = "1";
        private AdView MAdView;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Methods.App.FullScreenApp(this);

                SetTheme(AppTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.CreatNewPlaylistLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();  
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
       protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);

                MAdView?.Resume();
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

                MAdView?.Pause();
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
                MAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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
                //Get values
                TxtNewplaylist = FindViewById<EditText>(Resource.Id.nameplaylist_Edit);
                TxtDescription = FindViewById<EditText>(Resource.Id.description_Edit);
                RbPrivate = (RadioButton)FindViewById(Resource.Id.radioPrivate);
                RbPublic = (RadioButton)FindViewById(Resource.Id.radioPublic);

                SaveTextView = FindViewById<TextView>(Resource.Id.toolbar_title);
                SaveTextView.Click += SaveTextViewOnClick;

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);
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
                //Set ToolBar
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_Creat_New_Playlist);
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
                    //Event  
                    RbPrivate.CheckedChange += RbPrivateOnCheckedChange;
                    RbPublic.CheckedChange += RbPublicOnCheckedChange;
                }
                else
                {
                    //Event  
                    RbPrivate.CheckedChange -= RbPrivateOnCheckedChange;
                    RbPublic.CheckedChange -= RbPublicOnCheckedChange;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void SaveTextViewOnClick(object sender, EventArgs e)
        {
            SaveDataButtonOnClick();
        }


        //Public
        private void RbPublicOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                bool isChecked = RbPublic.Checked;
                if (isChecked)
                {
                    RbPrivate.Checked = false;
                    Status = "1";
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void RbPrivateOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                bool isChecked = RbPrivate.Checked;
                if (isChecked)
                {
                    RbPublic.Checked = false;
                    Status = "0";
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void SaveDataButtonOnClick()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var (apiResult, respond) = await RequestsAsync.Playlist.CreatePlaylistAsync(TxtNewplaylist.Text, TxtDescription.Text, Status);

                    if (apiResult == 200)
                    {

                        if (respond is CreatePlaylistObject result)
                        {
                            PlaylistId = result.PlaylistId.ToString();

                            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                              
                            PlayListVideoObject playLists = new PlayListVideoObject
                            {
                                Id = Convert.ToInt32(PlaylistId),
                                ListId = PlaylistId,
                                UserId = Convert.ToInt32(UserDetails.UserId),
                                Name = TxtNewplaylist.Text,
                                Description = TxtDescription.Text,
                                Privacy = Convert.ToInt32(Status),
                                Views = 0,
                                Icon = "",
                                Time = Convert.ToInt32(unixTimestamp),
                                StyleTotalSubVideos = "0",
                                StyleImage = "lib_playlists",
                                StyleTypeVideo = "",
                                VideosList = new List<VideoDataObject>()
                            };

                            var adapter = TabbedMainActivity.GetInstance()?.LibraryFragment?.MAdapter;
                            if (adapter != null)
                            {
                                adapter.PlayListsList.Add(playLists);
                                adapter.NotifyDataSetChanged();
                            }

                            AndHUD.Shared.Dismiss(this);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Created_successfully_playlist), ToastLength.Short)?.Show();

                            Finish();
                        }
                    }
                    else Methods.DisplayAndHudErrorResult(this, respond);
                }
                else
                {
                    AndHUD.Shared.Dismiss(this);
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
           
    }
}