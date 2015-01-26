using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Geolocator geolocator = new Geolocator();
        CancellationTokenSource CancellationTokenSrc = new CancellationTokenSource();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            InitializeGeoFence();
            
        }


        async private void InitializeGeoFence()
        {
            try
            {
                
                geolocator = new Geolocator();

                
                CancellationTokenSrc = new CancellationTokenSource();
                CancellationToken token = CancellationTokenSrc.Token;

                await geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)).AsTask(token);
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

        private async void ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}
