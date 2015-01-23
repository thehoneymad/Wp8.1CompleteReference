using GalaSoft.MvvmLight;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace MVVMMaps.Model
{
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
        public List<object> weekday_text { get; set; }
    }

    public class Photo
    {
        public int height { get; set; }
        public List<object> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class FoodLocation : ObservableObject
    {

        public const string LocationPropertyName = "Location";

        private Geopoint _Location = null;

        public Geopoint Location
        {
            get
            {
                return _Location;
            }

            set
            {

                _Location = value;
                RaisePropertyChanged(LocationPropertyName);
            }
        }

        
        public Geometry _geometry;
        public Geometry geometry { get { return _geometry;  } set { _geometry = value; Location = new Geopoint(new BasicGeoposition() {Latitude=value.location.lat, Longitude=value.location.lng });  } }
        public string icon { get; set; }       
        public string name { get; set; }
        public OpeningHours opening_hours { get; set; }
        public string place_id { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public string scope { get; set; }
        public List<string> types { get; set; }
        public string vicinity { get; set; }
        public int? price_level { get; set; }
        public List<Photo> photos { get; set; }

        
    }

    public class FoodLocations : ObservableObject
    {
        public List<object> html_attributions { get; set; }
        public string next_page_token { get; set; }

      
        public const string resultsPropertyName = "results";

        private List<FoodLocation> _results = null;

        
        public List<FoodLocation> results
        {
            get
            {
                return _results;
            }

            set
            {
               

                _results = value;
                RaisePropertyChanged(resultsPropertyName);
            }
        }

        
        public string status { get; set; }
    }
}
