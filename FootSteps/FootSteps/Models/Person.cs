using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootSteps.Models
{
    public class Person:EntityData
    {
       //public string deviceID{get;set;}
       public string phoneNo { get; set; }
       //public string name { get; set; }
       //public bool groupDirty { get; set; }
       public string latitude { get; set; }
       public string longitude { get; set; }
    }
}