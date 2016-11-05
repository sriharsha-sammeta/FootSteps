using System;
using System.Collections.Generic;
using System.Linq;
using test.DataModels;

namespace test.DataModels
{
    public class Person
    {
        public string deviceID { get; set; }
        public string phoneNo { get; set; }
        public string name { get; set; }
        public bool groupDirty { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}