using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace GeoLocationTrackingTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Geolocator locator = null;
        private CoreDispatcher dispatcher;

        public MainPage()
        {
            this.InitializeComponent();
            dispatcher = Window.Current.CoreWindow.Dispatcher;
        }
        

        

        async private void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Geoposition geoPosition = e.Position;
                LatitudeText.Text = geoPosition.Coordinate.Point.Position.Latitude.ToString();
                LongitudeText.Text = geoPosition.Coordinate.Point.Position.Longitude.ToString();
                AccuracyText.Text = geoPosition.Coordinate.Accuracy.ToString();
            });
        }

        private void TrackLocationButton_Click(object sender, RoutedEventArgs e)
        {
            if (locator == null)
            {
                locator = new Geolocator();
            }
            if (locator != null)
            {
                locator.MovementThreshold = 3;

                locator.PositionChanged +=
                    new TypedEventHandler<Geolocator,
                        PositionChangedEventArgs>(locator_PositionChanged);

                
            }

            TrackLocationButton.IsEnabled = false;
            StoptrackingButton.IsEnabled = true;

        }

        private void StoptrackingButton_Click(object sender, RoutedEventArgs e)
        {
            if (locator != null)
            {
                locator.PositionChanged -= new TypedEventHandler<Geolocator, PositionChangedEventArgs>(locator_PositionChanged);
            }

            StoptrackingButton.IsEnabled = false;
            TrackLocationButton.IsEnabled = true;
        }
    }
}
