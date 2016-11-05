using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps.DataModels
{
    public class Location
    {
        public int locationId { get; set; }
        public string locationName { get; set; }
        public double locationLat { get; set; }
        public double locationLong { get; set; }
    }
}
