using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace GeoFencingTest
{
    
    public sealed partial class MainPage : Page
    {
        Geolocator geolocator = new Geolocator();
        CancellationTokenSource CancellationTokenSrc = new CancellationTokenSource();

        Geofence geofence = null;
        int DefaultDwellTimeInSeconds = 0;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            InitializeLocation();
            
        }


        async private void InitializeLocation()
        {
            try
            {
                
                geolocator = new Geolocator();

                
                CancellationTokenSrc = new CancellationTokenSource();
                CancellationToken token = CancellationTokenSrc.Token;

                var position = await geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)).AsTask(token);

                Longitude.Text = position.Coordinate.Point.Position.Longitude.ToString();
                Latitude.Text = position.Coordinate.Point.Position.Latitude.ToString();
                Altitude.Text = position.Coordinate.Point.Position.Altitude.ToString();
            }
            
            catch (Exception)
            {
                if (geolocator.LocationStatus == PositionStatus.Disabled)
                {
                    ShowMessage("Location Services are turned off");
                }

            }
            finally
            {
                CancellationTokenSrc = null;
            }

        }


        private void SetupGeofence()
        {
            //Set up a unique key for the geofence
            string GeofenceKey = "My Home Geofence";
            BasicGeoposition GeoFenceRootPosition = GetMyHomeLocation();

            double Georadius = 500;

            // the geocircle is the circular region that defines the geofence
            Geocircle geocircle = new Geocircle(GeoFenceRootPosition, Georadius);

            bool singleUse = (bool)SingleUse.IsChecked;

            //Selecting a subset of the events we need to interact with the geofence
            MonitoredGeofenceStates GeoFenceStates = 0;

            GeoFenceStates |= MonitoredGeofenceStates.Entered;
            GeoFenceStates |= MonitoredGeofenceStates.Exited;

            // setting up how long you need to be in geofence for enter event to fire
            TimeSpan dwellTime;

            dwellTime = DwellTime.Text!=""? ParseTimeSpan(DwellTime.Text) : TimeSpan.FromSeconds(DefaultDwellTimeInSeconds);

            // setting up how long the geofence should be active
            TimeSpan GeoFenceDuration;
            GeoFenceDuration = TimeSpan.FromDays(10);

            // setting up the start time of the geofence
            DateTimeOffset GeoFenceStartTime = DateTimeOffset.Now;

            geofence = new Geofence(GeofenceKey, geocircle, GeoFenceStates, singleUse, dwellTime, GeoFenceStartTime, GeoFenceDuration);
            
            //Add the geofence to the GeofenceMonitor
            GeofenceMonitor.Current.Geofences.Add(geofence);

            //GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;

        }

        private async void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var Reports = sender.ReadReports();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (GeofenceStateChangeReport report in Reports)
                {
                    GeofenceState state = report.NewState;

                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Entered)
                    {
                        ShowMessage("You are pretty close to your room");

                    }
                    else if (state == GeofenceState.Exited)
                    {

                        ShowMessage("You have left pretty far away from your room");
                    }
                }
            });
        }

        private TimeSpan ParseTimeSpan(string DwellTimeInSeconds)
        {
            return TimeSpan.FromSeconds(Double.Parse(DwellTimeInSeconds));
            

        }

        private static BasicGeoposition GetMyHomeLocation()
        {
            BasicGeoposition GeoFenceRootPosition;
            GeoFenceRootPosition.Latitude = 23.742766;
            GeoFenceRootPosition.Longitude = 90.417566;
            GeoFenceRootPosition.Altitude = 0.0;
            return GeoFenceRootPosition;
        }

        private async void ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void CreateGeoFencingButton_Click(object sender, RoutedEventArgs e)
        {
            SetupGeofence();
            

        }

        private async Task RegisterBackgroundTask()
        {
            BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            var geofenceTaskBuilder = new BackgroundTaskBuilder
            {
                Name = "My Home Geofence",
                TaskEntryPoint = "GeofenceTask.BackgroundGeofenceTask"
            };

            var trigger = new LocationTrigger(LocationTriggerType.Geofence);
            geofenceTaskBuilder.SetTrigger(trigger);

            var geofenceTask = geofenceTaskBuilder.Register();
            geofenceTask.Completed += GeofenceTask_Completed;

            switch (backgroundAccessStatus)
            {
                case BackgroundAccessStatus.Unspecified:
                case BackgroundAccessStatus.Denied:
                    ShowMessage("This application is denied of the background task ");
                    break;
            }

        }

        private async void GeofenceTask_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            if (sender != null)
            {
                // Update the UI with progress reported by the background task
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception 
                        args.CheckResult();

                        var settings = ApplicationData.Current.LocalSettings;

                        // get status
                        if (settings.Values.ContainsKey("Status"))
                        {
                            Status.Text = settings.Values["Status"].ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message);
                    }
                });
            }
            

        }

        public static bool IsTaskRegistered(string TaskName)
        {
            var Registered = false;
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(keyval => keyval.Value.Name == TaskName);

            if (entry.Value != null)
                Registered = true;

            return Registered;
        }

        public static void Unregister(string TaskName)
        {
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(keyval => keyval.Value.Name == TaskName);

            if (entry.Value != null)
                entry.Value.Unregister(true);
        }

        private async void CreateBackgroundServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsTaskRegistered("My Home Geofence"))
            {
                Unregister("My Home Geofence");
            }
            await RegisterBackgroundTask();
        }
    }
}
