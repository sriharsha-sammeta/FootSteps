using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.ViewModels
{
    public class Person
    {
        public string Id { get; set; }
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string phoneNo { get; set; }
        public bool isGloballyVisible { get; set; }
        public bool isInMeeting { get; set; }
    }
}
