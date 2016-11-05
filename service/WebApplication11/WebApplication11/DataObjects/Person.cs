using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootSteps987.Models
{
    public class Person:EntityData
    {
        public string Id { get; set; }
        public string name { get; set; }        
        public string phoneNo { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool isGloballyVisible { get; set; }        
    }
}