using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MapTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //SetTileSourceToOSM();
            GetMyLocation();
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void ToggleProgressBar( bool toggle, string message="")
        {
            StatusBarProgressIndicator progressbar = StatusBar.GetForCurrentView().ProgressIndicator;
            if(toggle)
            {
                progressbar.Text = message;
                await progressbar.ShowAsync();
            }
            else
            {
                await progressbar.HideAsync();
            }
            
        }

        private void SetTileSourceToOSM()
        {
            var next = "a";
            var httpsource = new HttpMapTileDataSource("http://a.tile.openstreetmap.org/{zoomlevel}/{x}/{y}.png");

            httpsource.UriRequested += (src, args) => {
                next = GetNextDomain(next);
                args.Request.Uri = new Uri("http://" + next + ".tile.openstreetmap.org/" +
                    args.ZoomLevel + "/" + args.X + "/" + args.Y + ".png");

            };

            var tilesource = new MapTileSource(httpsource)
            {
                Layer = MapTileLayer.BackgroundOverlay
            };

           MyMap.Style = MapStyle.None;
           MyMap.TileSources.Add(tilesource);
        }

        private string GetNextDomain(string next)
        {
            string[] domains={"a", "b", "c"};
            
            for(int count=0; count<domains.Length; count++)
            {
                if (domains[count] == next)
                    return domains[(count + 1) % domains.Length];
            }

            return next;
        }

        private async void GetMyLocation()
        {
            ToggleProgressBar(true,"Loading");

            try
            {
                Geolocator locator = new Geolocator();
                Geoposition position = await locator.GetGeopositionAsync();

                //MapIcon icon = new MapIcon();
                //icon.Location = position.Coordinate.Point;
                //icon.Title = "My Location";
                //icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/ImageIcon.png"));
                //MyMap.MapElements.Add(icon);

                var pushpin = new Windows.UI.Xaml.Shapes.Ellipse();
                pushpin.Fill=new SolidColorBrush(Colors.Red);
                pushpin.Height=50;
                pushpin.Width=50;

                MyMap.Children.Add(pushpin);
                //MyMap.Center = position.Coordinate.Point;
                MyMap.DesiredPitch = 0;


                MapControl.SetLocation(pushpin,position.Coordinate.Point);
                MapControl.SetNormalizedAnchorPoint(pushpin, new Point(0.5, 0.5));

                await MyMap.TrySetViewAsync(position.Coordinate.Point, 15,0,0,MapAnimationKind.Bow);
            }
            catch (Exception ex)
            {
                
                ShowMessage(ex.Message);
            }

            ToggleProgressBar(false);
 
        }

        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GetMyLocation();
            
        }

        private void MapColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapColorBox!=null)
            {
                string selectedColor = ((ComboBoxItem)MapColorBox.SelectedItem).Content.ToString();

                switch (selectedColor)
                {
                    case "Light":
                        MyMap.ColorScheme = MapColorScheme.Light;
                        break;
                    case "Dark":
                        MyMap.ColorScheme = MapColorScheme.Dark;
                        break;
                    default:
                        MyMap.ColorScheme = MapColorScheme.Light;
                        break;
                } 
            }

        }

        private void MapStyleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapStyleBox != null)
            {
                string selectedStyle = ((ComboBoxItem)MapStyleBox.SelectedItem).Content.ToString();

                switch (selectedStyle)
                {
                    case "None":
                        MyMap.Style = MapStyle.None;
                        break;
                    case "Road":
                        MyMap.Style = MapStyle.Road;
                        break;
                    case "Aerial":
                        MyMap.Style = MapStyle.Aerial;
                        break;
                    case "AerialWithRoads":
                        MyMap.Style = MapStyle.AerialWithRoads;
                        break;
                    case "Terrain":
                        MyMap.Style = MapStyle.Terrain;
                        break;
                    default:
                        MyMap.Style = MapStyle.None;
                        break;
                }
            }
        }

        private async void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Geopoint point = new Geopoint(args.Location.Position);

           
            MapLocationFinderResult FinderResult =
                await MapLocationFinder.FindLocationsAtAsync(point);

            String format = "{0}, {1}, {2}";


            if (FinderResult.Status == MapLocationFinderStatus.Success)
            {
                var selectedLocation=FinderResult.Locations.First();

                string message= String.Format(format, selectedLocation.Address.Town, selectedLocation.Address.District, selectedLocation.Address.Country);
                await ShowMessage(message);

            }

        }

        private async Task ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        

 

        
    }
}
