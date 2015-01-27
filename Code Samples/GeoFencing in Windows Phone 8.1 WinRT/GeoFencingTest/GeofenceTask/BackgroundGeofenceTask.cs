using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Storage;
using Windows.UI.Notifications;

namespace GeofenceTask
{
    public sealed class BackgroundGeofenceTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var Reports = GeofenceMonitor.Current.ReadReports();
            var SelectedReport = 
                Reports.FirstOrDefault(report => (report.Geofence.Id == "My Home Geofence") && (report.NewState == GeofenceState.Entered || report.NewState == GeofenceState.Exited));

            if (SelectedReport==null)
            {
                return;
            }

            //We are creating a toast this time as this is running in Background
            var ToastContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);


           

            var TextNodes = ToastContent.GetElementsByTagName("text");

            if (SelectedReport.NewState == GeofenceState.Entered)
            {
                TextNodes[0].AppendChild(ToastContent.CreateTextNode("You are pretty close to your room"));
                TextNodes[1].AppendChild(ToastContent.CreateTextNode(SelectedReport.Geofence.Id));
            }
            else if(SelectedReport.NewState == GeofenceState.Exited)
            {
                TextNodes[0].AppendChild(ToastContent.CreateTextNode("You are pretty close to your room"));
                TextNodes[1].AppendChild(ToastContent.CreateTextNode(SelectedReport.Geofence.Id));
            }

            var settings = ApplicationData.Current.LocalSettings;

            if (settings.Values.ContainsKey("Status"))
            {
                settings.Values["Status"] = SelectedReport.NewState;
            }
            else
            {
                settings.Values.Add(new KeyValuePair<string, object>("Status", SelectedReport.NewState.ToString()));
            }


            var Toast = new ToastNotification(ToastContent);
            var ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            ToastNotifier.Show(Toast);


        }
    }
}
