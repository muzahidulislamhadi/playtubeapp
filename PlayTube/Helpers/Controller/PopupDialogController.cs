//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialDialogsCore;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using PlayTube.Activities.Default;
using PlayTube.Activities.Models;
using PlayTube.Activities.Playlist;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace PlayTube.Helpers.Controller
{
    public class PopupDialogController : Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        private readonly Activity ActivityContext;
        private readonly VideoDataObject Videodata;
        private readonly string TypeDialog;

        public PopupDialogController(Activity activity, VideoDataObject videoobje, string typeDialog)
        {
            ActivityContext = activity;
            Videodata = videoobje;
            TypeDialog = typeDialog;
        }

        public async void ShowPlayListDialog()
        {
            try
            {
                MaterialDialog.Builder progressDialog = new MaterialDialog.Builder(ActivityContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                progressDialog.Title(ActivityContext.GetText(Resource.String.Lbl_Checking_PlayLists_info));
                progressDialog.Content(ActivityContext.GetText(Resource.String.Lbl_Please_wait));
                progressDialog.Progress(true, 0);
                progressDialog.ProgressIndeterminateStyle(true);

                MaterialDialog dialog = progressDialog.Build();
                dialog.Show();

                if (ListUtils.PlayListVideoObjectList.Count == 0)
                {
                    await ApiRequest.PlayListsVideosApi(ActivityContext);
                }

                dialog.Dismiss();

                List<string> arrayAdapter = new List<string>();
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                var count = ListUtils.PlayListVideoObjectList.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!string.IsNullOrEmpty(ListUtils.PlayListVideoObjectList[i].Name))
                            arrayAdapter.Add(ListUtils.PlayListVideoObjectList[i].Name);
                    }
                }

                dialogList.Title(ActivityContext.GetText(Resource.String.Lbl_Select_One_Name));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Creat_new)).OnPositive(this);
                dialogList.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void ShowNormalDialog(string title, string content = null, string positiveText = null, string negativeText = null)
        {
            try
            {
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                if (!string.IsNullOrEmpty(title))
                    dialogList.Title(title);

                if (!string.IsNullOrEmpty(content))
                    dialogList.Content(content);

                if (!string.IsNullOrEmpty(negativeText))
                {
                    dialogList.NegativeText(negativeText);
                    dialogList.OnNegative(this);
                }

                if (!string.IsNullOrEmpty(positiveText))
                {
                    dialogList.PositiveText(positiveText);
                    dialogList.OnPositive(this);
                }

                dialogList.Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void ShowEditTextDialog(string title, string content = null, string positiveText = null, string negativeText = null)
        {
            try
            {
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                if (!string.IsNullOrEmpty(title))
                    dialogList.Title(title);

                if (!string.IsNullOrEmpty(content))
                    dialogList.Content(content);

                if (!string.IsNullOrEmpty(negativeText))
                {
                    dialogList.NegativeText(negativeText);
                    dialogList.OnNegative(this);
                }

                if (!string.IsNullOrEmpty(positiveText))
                {
                    dialogList.PositiveText(positiveText);
                    dialogList.OnPositive(this);
                }

                dialogList.InputType(InputTypes.ClassText | InputTypes.TextFlagMultiLine);
                dialogList.Input("", "", this);
                dialogList.Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                if (TypeDialog == "PlayList")
                {
                    var dataPlaylist = ListUtils.PlayListVideoObjectList.FirstOrDefault(a => a.Name == itemString.ToString());
                    if (dataPlaylist != null)
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Playlist.AddToListAsync(Videodata.Id, dataPlaylist.ListId) });

                        new LibrarySynchronizer(ActivityContext).AddToPlaylistVideo(Videodata);
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_added), ToastLength.Short)?.Show();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "PlayList" when p1 == DialogAction.Positive:
                        ActivityContext.StartActivity(new Intent(ActivityContext, typeof(CreatNewPlaylistActivity)));
                        break;
                    case "PlayList":
                        {
                            if (p1 == DialogAction.Negative)
                            {
                                p0.Dismiss();
                            }

                            break;
                        }
                    case "Login" when p1 == DialogAction.Positive:
                        TabbedMainActivity.GetInstance()?.StopFragmentVideo();
                        ActivityContext.StartActivity(new Intent(ActivityContext, typeof(LoginActivity)));
                        break;
                    case "Login":
                        {
                            if (p1 == DialogAction.Negative)
                            {
                                p0.Dismiss();
                            }

                            break;
                        } 
                    case "Logout" when p1 == DialogAction.Positive:
                        TabbedMainActivity.GetInstance()?.VideoActionsController.ReleaseVideo();

                        ApiRequest.Logout(ActivityContext);
                        break;
                    case "Logout":
                        {
                            if (p1 == DialogAction.Negative)
                            {
                                p0.Dismiss();
                            }

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnInput(MaterialDialog dialog, string p1)
        {
            try
            {
                if (TypeDialog == "Report")
                {
                    if (p1.Length > 0)
                    {
                        if (Methods.CheckConnectivity())
                        {
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.ReportVideoAsync(Videodata.Id, p1) });

                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_received_your_report), ToastLength.Short)?.Show();
                        }
                        else
                        {
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_The_name_can_not_be_blank), ToastLength.Short)?.Show();
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