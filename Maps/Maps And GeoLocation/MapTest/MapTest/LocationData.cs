using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace MapTest
{
    public class LocationData: DependencyObject
    {
        public Geopoint Location { get; set; }
        public string Name { get; set; }
    }
}
