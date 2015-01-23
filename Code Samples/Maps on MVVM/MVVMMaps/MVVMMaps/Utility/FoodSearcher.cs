using MVVMMaps.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MVVMMaps.Utility
{
    internal class FoodSearcher
    {
        private string BaseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/";
        private string Format = "json";
        private string LocationKey = "location";
        private string RadiusKey = "radius";
        private string TypesKey = "types";
        private string Type = "food";
        private string KeyKey = "key";
        private string Key = "AIzaSyDSuWOakAmCKEzeDfPq3fRfuISKu0nhjmU";

        public async Task<FoodLocations> SearchForFoodPlaces(Geopoint point, int radius)
        {
            try
            {
                string searchUrlFormat = string.Concat(BaseUrl, Format, "?", LocationKey, "=", "{0}", "&", "radius={1}", "&", TypesKey, "=", Type, "&", KeyKey, "=", Key);
                string searchUrl = string.Format(searchUrlFormat, GetLocationParam(point), radius);

                HttpClient client = new HttpClient();
                var data = await client.GetStringAsync(searchUrl);

                return JsonConvert.DeserializeObject<FoodLocations>(data);
            }
            catch (System.Exception ex)
            {
                return null;              
            }
        }

        private string GetLocationParam(Geopoint point)
        {
            return string.Concat(point.Position.Latitude.ToString(), ",", point.Position.Longitude.ToString());
        }

    }
}
