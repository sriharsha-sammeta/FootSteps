using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootSteps987.DataObjects
{
    public class Group:EntityData
    {
       public string Id { get; set; }
       public string name { get; set; }
       public string theme { get; set; }
       public double destinationLatitude { get; set; }
       public double destinationLongitude { get; set; }

       public string destinationName { get; set; }
    }
}