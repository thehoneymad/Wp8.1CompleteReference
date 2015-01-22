using GalaSoft.MvvmLight;
using Windows.Devices.Geolocation;
using System;
using Windows.UI.Popups;

namespace MVVMMaps.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
       
        public const string MyLocationPropertyName = "MyLocation";

        private Geopoint _myLocation = null;

        public Geopoint MyLocation
        {
            get
            {
                return _myLocation;
            }

            set
            {
                if (_myLocation == value)
                {
                    return;
                }

                
                _myLocation = value;
                RaisePropertyChanged(MyLocationPropertyName);
            }
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            GetMyLocation();
            //_testLocation = new Geopoint(new BasicGeoposition() { Latitude = 37.127330, Longitude = 33.123779});
        }

        private async void ToggleProgressBar(bool toggle, string message="")
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            

            if (toggle)
            {
                statusBar.ProgressIndicator.Text = message;
                await statusBar.ProgressIndicator.ShowAsync();
            }
            else
            {
                await statusBar.ProgressIndicator.HideAsync();
            }
        }

        private async void GetMyLocation()
        {
            try
            {
                ToggleProgressBar(true, "Getting your location");

                Geolocator locator = new Geolocator();
                
                var location = await locator.GetGeopositionAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60));
                MyLocation = location.Coordinate.Point;

                ToggleProgressBar(false);
            }
            catch (Exception ex)
            {

                MessageDialog Dialog = new MessageDialog("Location detection failed");
                Dialog.ShowAsync();
            }
        }
    }
}