using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialDialogsCore;
using Android.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using PlayTube.Activities.PlayersView;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Comment;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;

namespace PlayTube.Activities.Comments
{
    public class CommentClickListener : Java.Lang.Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        private readonly Activity Context;
        private readonly TabbedMainActivity MainContext;
        private CommentDataObject CommentObject;
        private ReplyDataObject ReplyObject;
        private string TypeDialog;
        private readonly string TypeClass;

        public CommentClickListener(Activity context, string typeClass)
        {
            Context = context;
            MainContext = TabbedMainActivity.GetInstance();
            TypeClass = typeClass;
        }
         
        public void MoreCommentPostClick(CommentDataObject item)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "MoreComment";
                    CommentObject = item;

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(MainContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                    arrayAdapter.Add(Context.GetString(Resource.String.Lbl_CopeText));

                    if (CommentObject != null && (CommentObject.IsCommentOwner || CommentObject?.CommentUserData?.Id == UserDetails.UserId))
                    {
                        arrayAdapter.Add(Context.GetString(Resource.String.Lbl_Edit));
                        arrayAdapter.Add(Context.GetString(Resource.String.Lbl_Delete));
                    }

                    dialogList.Title(Context.GetString(Resource.String.Lbl_More));
                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(MainContext.GetText(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Event Menu >> Delete Comment
        private void DeleteCommentEvent(CommentDataObject item)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "DeleteComment";
                    CommentObject = item;

                    var dialog = new MaterialDialog.Builder(MainContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                    dialog.Title(MainContext.GetText(Resource.String.Lbl_DeleteComment));
                    dialog.Content(MainContext.GetText(Resource.String.Lbl_AreYouSureDeleteComment));
                    dialog.PositiveText(MainContext.GetText(Resource.String.Lbl_Yes)).OnPositive(this);
                    dialog.NegativeText(MainContext.GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Event Menu >> Edit Comment
        private void EditCommentEvent(CommentDataObject item)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "EditComment";
                    CommentObject = item;

                    var dialog = new MaterialDialog.Builder(MainContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                    dialog.Title(Resource.String.Lbl_Edit);
                    dialog.Input(Context.GetString(Resource.String.Lbl_Write_comment), Methods.FunString.DecodeString(item.Text), this);

                    dialog.InputType(InputTypes.TextFlagImeMultiLine);
                    dialog.PositiveText(MainContext.GetText(Resource.String.Lbl_Update)).OnPositive(this);
                    dialog.NegativeText(MainContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                    dialog.Build().Show();
                    dialog.AlwaysCallSingleChoiceCallback();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        public void MoreReplyPostClick(ReplyDataObject item)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "MoreComment";
                    ReplyObject = item;

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(MainContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                    arrayAdapter.Add(Context.GetString(Resource.String.Lbl_CopeText));

                    if (ReplyObject != null && (ReplyObject.IsReplyOwner || ReplyObject?.ReplyUserData?.Id == UserDetails.UserId))
                    {
                        arrayAdapter.Add(Context.GetString(Resource.String.Lbl_Edit));
                        arrayAdapter.Add(Context.GetString(Resource.String.Lbl_Delete));
                    }

                    dialogList.Title(Context.GetString(Resource.String.Lbl_More));
                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(MainContext.GetText(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Event Menu >> Delete Reply
        private void DeleteReplyEvent(ReplyDataObject item)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "DeleteComment";
                    ReplyObject = item;

                    var dialog = new MaterialDialog.Builder(MainContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                    dialog.Title(MainContext.GetText(Resource.String.Lbl_DeleteComment));
                    dialog.Content(MainContext.GetText(Resource.String.Lbl_AreYouSureDeleteComment));
                    dialog.PositiveText(MainContext.GetText(Resource.String.Lbl_Yes)).OnPositive(this);
                    dialog.NegativeText(MainContext.GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Event Menu >> Edit Reply
        private void EditReplyEvent(ReplyDataObject item)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "EditComment";
                    ReplyObject = item;

                    var dialog = new MaterialDialog.Builder(MainContext).Theme(AppTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                    dialog.Title(Resource.String.Lbl_Edit);
                    dialog.Input(Context.GetString(Resource.String.Lbl_Write_comment), Methods.FunString.DecodeString(item.Text), this);

                    dialog.InputType(InputTypes.TextFlagImeMultiLine);
                    dialog.PositiveText(MainContext.GetText(Resource.String.Lbl_Update)).OnPositive(this);
                    dialog.NegativeText(MainContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                    dialog.Build().Show();
                    dialog.AlwaysCallSingleChoiceCallback();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                string text = itemString;
                if (text == Context.GetString(Resource.String.Lbl_CopeText))
                {
                    switch (TypeClass)
                    {
                        case "Comment":
                            Methods.CopyToClipboard(MainContext, Methods.FunString.DecodeString(CommentObject.Text));
                            break;
                        case "Reply":
                            Methods.CopyToClipboard(MainContext, Methods.FunString.DecodeString(ReplyObject.Text));
                            break;
                    }
                }
                else if (text == Context.GetString(Resource.String.Lbl_Edit))
                {
                    switch (TypeClass)
                    {
                        case "Comment":
                            EditCommentEvent(CommentObject);
                            break;
                        case "Reply":
                            EditReplyEvent(ReplyObject);
                            break;
                    }
                }
                else if (text == Context.GetString(Resource.String.Lbl_Delete))
                {
                    switch (TypeClass)
                    {
                        case "Comment":
                            DeleteCommentEvent(CommentObject);
                            break;
                        case "Reply":
                            DeleteReplyEvent(ReplyObject);
                            break;
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
                if (p1 == DialogAction.Positive)
                {
                    if (TypeDialog == "DeleteComment")
                    {
                        MainContext?.RunOnUiThread(() =>
                        {
                            try
                            {
                                switch (TypeClass)
                                {
                                    case "Comment":
                                    {
                                        //TypeClass
                                        var adapterGlobal = MainContext?.CommentsFragment?.MAdapter;
                                        var dataGlobal = adapterGlobal?.CommentList?.FirstOrDefault(a => a.Id == CommentObject?.Id);
                                        if (dataGlobal != null)
                                        {
                                            var index = adapterGlobal.CommentList.IndexOf(dataGlobal);
                                            if (index > -1)
                                            {
                                                adapterGlobal.CommentList.RemoveAt(index);
                                                adapterGlobal.NotifyItemRemoved(index);
                                            }
                                        }

                                        var adapterGlobal1 = GlobalPlayerActivity.GetInstance()?.CommentsFragment?.MAdapter;
                                        var dataGlobal1 = adapterGlobal1?.CommentList?.FirstOrDefault(a => a.Id == CommentObject?.Id);
                                        if (dataGlobal1 != null)
                                        {
                                            var index = adapterGlobal1.CommentList.IndexOf(dataGlobal1);
                                            if (index > -1)
                                            {
                                                adapterGlobal1.CommentList.RemoveAt(index);
                                                adapterGlobal1.NotifyItemRemoved(index);
                                            }
                                        }
                                     
                                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.DeleteCommentAsync(CommentObject.Id.ToString()) });
                                        break;
                                    }
                                    case "Reply":
                                    {
                                        //TypeClass
                                        var adapterGlobal = ReplyCommentBottomSheet.GetInstance()?.ReplyAdapter;
                                        var dataGlobal = adapterGlobal?.ReplyList?.FirstOrDefault(a => a.Id == ReplyObject?.Id);
                                        if (dataGlobal != null)
                                        {
                                            var index = adapterGlobal.ReplyList.IndexOf(dataGlobal);
                                            if (index > -1)
                                            {
                                                adapterGlobal.ReplyList.RemoveAt(index);
                                                adapterGlobal.NotifyItemRemoved(index);
                                            }
                                        }

                                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.DeleteReplyAsync(ReplyObject.Id.ToString()) });
                                        break;
                                    }
                                }

                                Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CommentSuccessfullyDeleted), ToastLength.Short)?.Show();
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }
                    else
                    {
                        if (p1 == DialogAction.Positive)
                        {
                        }
                        else if (p1 == DialogAction.Negative)
                        {
                            p0.Dismiss();
                        }
                    }
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

        public void OnInput(MaterialDialog dialog, string p1)
        {
            try
            {
                if (p1.Length > 0)
                {
                    var strName = p1;

                    if (!Methods.CheckConnectivity())
                    {
                        Toast.MakeText(MainContext, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                    else
                    {
                        switch (TypeClass)
                        {
                            case "Comment":
                            {
                                //TypeClass
                                var adapterGlobal = MainContext?.CommentsFragment?.MAdapter;
                                var dataGlobal = adapterGlobal?.CommentList?.FirstOrDefault(a => a.Id == CommentObject?.Id);
                                if (dataGlobal != null)
                                {
                                    dataGlobal.Text = strName;
                                    var index = adapterGlobal.CommentList.IndexOf(dataGlobal);
                                    if (index > -1)
                                    {
                                        adapterGlobal.NotifyItemChanged(index);
                                    }
                                }

                                var adapterGlobal1 = GlobalPlayerActivity.GetInstance()?.CommentsFragment?.MAdapter;
                                var dataGlobal1 = adapterGlobal1?.CommentList?.FirstOrDefault(a => a.Id == CommentObject?.Id);
                                if (dataGlobal1 != null)
                                {
                                    dataGlobal1.Text = strName;
                                    var index = adapterGlobal1.CommentList.IndexOf(dataGlobal1);
                                    if (index > -1)
                                    {
                                        adapterGlobal1.NotifyItemChanged(index);
                                    }
                                }
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.EditCommentAsync(CommentObject.Id.ToString(), strName) });
                                break;
                            }
                            case "Reply":
                            {
                                //TypeClass
                                var adapterGlobal = ReplyCommentBottomSheet.GetInstance()?.ReplyAdapter;
                                var dataGlobal = adapterGlobal?.ReplyList?.FirstOrDefault(a => a.Id == ReplyObject?.Id);
                                if (dataGlobal != null)
                                {
                                    dataGlobal.Text = strName;
                                    var index = adapterGlobal.ReplyList.IndexOf(dataGlobal);
                                    if (index > -1)
                                    {
                                        adapterGlobal.NotifyItemChanged(index);
                                    }
                                }

                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.EditCommentReplyAsync(ReplyObject.Id.ToString(), strName) });
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion MaterialDialog 
    } 
}