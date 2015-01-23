using GalaSoft.MvvmLight;
using Windows.Devices.Geolocation;
using System;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;

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


        public const string GetMyLocationCommandPropertyName = "GetMyLocationCommand";

        private RelayCommand _getMyLocationCommand = null;

        public RelayCommand GetMyLocationCommand
        {
            get
            {
                return _getMyLocationCommand;
            }

            set
            {
                if (_getMyLocationCommand == value)
                {
                    return;
                }

               
                _getMyLocationCommand = value;
                RaisePropertyChanged(GetMyLocationCommandPropertyName);
            }
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
            GetMyLocationCommand = new RelayCommand(GetMyLocation);
            
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
                
                var location = await locator.GetGeopositionAsync(TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(60));
                MyLocation = location.Coordinate.Point;

                Messenger.Default.Send<Geopoint>(MyLocation, Constants.SetMapViewToken);

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