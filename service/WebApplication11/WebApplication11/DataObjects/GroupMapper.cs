using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FootSteps987.DataObjects
{
    public class GroupMapper:EntityData
    {
        public string Id { get; set; }
        public string groupId { get; set; }
        public string personId { get; set; }
        public bool isPersonVisibleInGroup { get; set; }        
    }
}