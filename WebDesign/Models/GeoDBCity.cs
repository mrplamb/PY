using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Models
{
    internal class GeoDBCity
    {
        public class GeoDBCityResponse
        {
            public List<CityData> Data { get; set; }
        }

        public class CityData
        {
            public string City { get; set; }
            public string Country { get; set; }
        }

    }
}
