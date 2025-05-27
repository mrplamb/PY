using System.Collections.Generic;

namespace HolidayManagerWeb.Models // Make sure this namespace matches your project setup
{
    public class CitySuggestion
    {
        public string CityName { get; set; }

        public List<PlaceDetail> PlacesToVisit { get; set; }
        public List<PlaceDetail> PlacesToEat { get; set; }

        public CitySuggestion()
        {
            PlacesToVisit = new List<PlaceDetail>();
            PlacesToEat = new List<PlaceDetail>();
        }
    }
}