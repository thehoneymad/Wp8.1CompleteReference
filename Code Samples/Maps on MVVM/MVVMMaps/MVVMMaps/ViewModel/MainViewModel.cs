using GalaSoft.MvvmLight;
using Windows.Devices.Geolocation;
using System;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using MVVMMaps.Utility;
using System.Collections.ObjectModel;
using MVVMMaps.Model;

namespace MVVMMaps.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
       
        public const string MyLocationPropertyName = "MyLocation";

        private Geoposition _myLocation = null;

        public Geoposition MyLocation
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


        public const string NearbyFoodPlacesPropertyName = "NearbyFoodPlaces";

        private ObservableCollection<FoodLocation> _NearbyFoodPlaces = null;

        public ObservableCollection<FoodLocation> NearbyFoodPlaces
        {
            get
            {
                return _NearbyFoodPlaces;
            }

            set
            {
                if (_NearbyFoodPlaces == value)
                {
                    return;
                }

                _NearbyFoodPlaces = value;
                RaisePropertyChanged(NearbyFoodPlacesPropertyName);
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




        public const string FindNearestFoodPlacesCommandPropertyName = "FindNearestFoodPlacesCommand";

        private RelayCommand _FindNearestFoodPlacesCommand = null;
        public RelayCommand FindNearestFoodPlacesCommand
        {
            get
            {
                return _FindNearestFoodPlacesCommand;
            }

            set
            {
                if (_FindNearestFoodPlacesCommand == value)
                {
                    return;
                }

                _FindNearestFoodPlacesCommand = value;
                RaisePropertyChanged(FindNearestFoodPlacesCommandPropertyName);
            }
        }

        
        public const string ShowFoodLocationDetailsPropertyName = "ShowFoodLocationDetails";

        private RelayCommand<FoodLocation> _ShowFoodLocationDetails = null;

       
        public RelayCommand<FoodLocation> ShowFoodLocationDetails
        {
            get
            {
                return _ShowFoodLocationDetails;
            }

            set
            {
                if (_ShowFoodLocationDetails == value)
                {
                    return;
                }

                _ShowFoodLocationDetails = value;
                RaisePropertyChanged(ShowFoodLocationDetailsPropertyName);
            }
        }


        public MainViewModel()
        {
            
            GetMyLocationCommand = new RelayCommand(GetMyLocation);
            FindNearestFoodPlacesCommand = new RelayCommand(FindNearestFoodPlaces);
            ShowFoodLocationDetails = new RelayCommand<FoodLocation>(ShowFoodLocationDetailsAction);
            
        }

        private void ShowFoodLocationDetailsAction(FoodLocation location)
        {
            MessageDialog Dialog = new MessageDialog(location.name+ "-"+location.vicinity);
            Dialog.ShowAsync();
        }

        private async void FindNearestFoodPlaces()
        {
            ToggleProgressBar(true, "Looking for food places around");
            try
            {
                
                if (MyLocation != null)
                {
                    FoodSearcher Searcher = new FoodSearcher();
                    var FoodPlaces = await Searcher.SearchForFoodPlaces(MyLocation.Coordinate.Point, 500);

                    NearbyFoodPlaces = new ObservableCollection<FoodLocation>(FoodPlaces.results);
                }


            }
            catch (Exception ex)
            {
                MessageDialog Dialog = new MessageDialog("Nearby Food Places Search Failed");
                Dialog.ShowAsync();

            }

            ToggleProgressBar(false);
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
                MyLocation = location;

                Messenger.Default.Send<Geopoint>(MyLocation.Coordinate.Point, Constants.SetMapViewToken);

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