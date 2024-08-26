using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Java.Lang;
using Newtonsoft.Json;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Messages;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;

namespace PlayTube.Service
{
    [Service(Exported = false, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class AppApiService : JobService
    {
        public static JobService Instance;
        public static JobParameters JobParameters;

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                //Toast.MakeText(Application.Context, "OnCreate", ToastLength.Short)?.Show();
                new Handler(Looper.MainLooper).PostDelayed(new ChatUpdaterHelper(new Handler(Looper.MainLooper)), AppSettings.RefreshChatActivitiesSeconds);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                base.OnStartCommand(intent, flags, startId);

                new Handler(Looper.MainLooper).PostDelayed(new ChatUpdaterHelper(new Handler(Looper.MainLooper)), AppSettings.RefreshChatActivitiesSeconds);
                //Toast.MakeText(Application.Context, "OnStartCommand", ToastLength.Short)?.Show();

                return StartCommandResult.Sticky;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return StartCommandResult.NotSticky;
            }
        }

        public override bool OnStartJob(JobParameters jobParams)
        {
            //Toast.MakeText(Application.Context, "On Start Job " + Methods.AppLifecycleObserver.AppState, ToastLength.Short)?.Show();

            Instance = this;
            JobParameters = jobParams;
            new Handler(Looper.MainLooper).PostDelayed(new ChatUpdaterHelper(new Handler(Looper.MainLooper)), AppSettings.RefreshChatActivitiesSeconds);

            // Our task will run in background, we will take care of notifying the finish.
            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            //Toast.MakeText(Application.Context, "On Stop Job 321" + Methods.AppLifecycleObserver.AppState, ToastLength.Short)?.Show();
            // I want it to reschedule so returned true, if we would have returned false, then job would have ended here.
            // It would not fire onStartJob() when constraints are re satisfied.

            new Handler(Looper.MainLooper).PostDelayed(new ChatUpdaterHelper(new Handler(Looper.MainLooper)), AppSettings.RefreshChatActivitiesSeconds);

            return false;
        }
    }

    public static class ChatJobInfo
    {
        public static void ScheduleJob(Context context)
        {
            try
            {
                ComponentName serviceComponent = new ComponentName(context, Class.FromType(typeof(AppApiService)));

                JobInfo jobInfo;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    JobInfo.Builder builder = new JobInfo.Builder(1, serviceComponent);
                    builder.SetMinimumLatency(AppSettings.RefreshChatActivitiesSeconds); // wait at least
                    builder.SetOverrideDeadline(100); // maximum delay
                    builder.SetRequiredNetworkType(NetworkType.Unmetered); // require unmetered network
                    //builder.SetRequiresDeviceIdle(true); // the device should be idle
                    builder.SetRequiresCharging(false); // we don't care if the device is charging or not 

                    //if (Build.VERSION.SdkInt == BuildVersionCodes.S)
                    //builder.SetExpedited(true);
                    //#pragma warning disable CS0618
                    //else if (Build.VERSION.SdkInt == BuildVersionCodes.R)
                    //builder.SetImportantWhileForeground(false);
                    //#pragma warning restore CS0618

                    builder.SetPersisted(true);
                    jobInfo = builder?.Build();
                }
                else
                {
                    jobInfo = new JobInfo.Builder(1, serviceComponent)?.SetPeriodic(AppSettings.RefreshChatActivitiesSeconds)?.Build();
                }

                var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
                if (jobInfo != null)
                    jobScheduler?.Schedule(jobInfo);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void StopJob(Context context)
        {
            try
            {
                var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
                jobScheduler?.CancelAll();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class ChatUpdaterHelper : Java.Lang.Object, IRunnable
    {
        private static Handler MainHandler;

        public ChatUpdaterHelper(Handler mainHandler)
        {
            MainHandler = mainHandler;
        }

        public void Run()
        {
            try
            {
                if (string.IsNullOrEmpty(Methods.AppLifecycleObserver.AppState))
                    Methods.AppLifecycleObserver.AppState = "Background";

                //Toast.MakeText(Application.Context, "Started", ToastLength.Short)?.Show(); 

                if (Methods.AppLifecycleObserver.AppState == "Background" && string.IsNullOrEmpty(Current.AccessToken))
                {
                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    var login = dbDatabase.Get_data_Login();
                    Console.WriteLine(login);
                }

                //ToastUtils.ShowToast(Application.Context, "AppState " + Methods.AppLifecycleObserver.AppState, ToastLength.Short);
                 
                if (string.IsNullOrEmpty(Current.AccessToken) || !UserDetails.IsLogin)
                    return;
                 
                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadChatAsync });

                MainHandler ??= new Handler(Looper.MainLooper);
                MainHandler?.PostDelayed(new ChatUpdaterHelper(new Handler(Looper.MainLooper)), AppSettings.RefreshChatActivitiesSeconds);
            }
            catch (Exception e)
            {
                //ToastUtils.ShowToast(Application.Context, "ResultSender failed",ToastLength.Short);
                MainHandler ??= new Handler(Looper.MainLooper);
                MainHandler?.PostDelayed(new ChatUpdaterHelper(new Handler(Looper.MainLooper)), AppSettings.RefreshChatActivitiesSeconds);
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static async Task LoadChatAsync()
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Messages.GetChats("20", "0");
                if (apiStatus != 200 || respond is not GetChatsObject result || result.data == null)
                {
                    // Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    //Toast.MakeText(Application.Context, "ResultSender 1 " + Methods.AppLifecycleObserver.AppState, ToastLength.Short)?.Show();
                    result.data.RemoveAll(a => a.Id == null || a.User == null);

                    if (result.data.Count > 0)
                    {
                        if (Methods.AppLifecycleObserver.AppState == "Foreground")
                        {
                            TabbedMainActivity.GetInstance()?.OnReceiveResult(JsonConvert.SerializeObject(result));
                        }
                        else
                        {
                            ListUtils.ChatList = new ObservableCollection<GetChatsObject.Data>(result.data);
                            //Insert All data users to database
                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.InsertOrReplaceLastChatTable(ListUtils.ChatList);
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