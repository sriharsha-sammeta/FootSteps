using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication11.DataObjects
{
    public class Connect:EntityData
    {
        public string Id { get; set; }
        public string groupMapperId { get; set; }
        public bool status { get; set; }
        /*status
         * true->accepted
         * false->pending
         * tuple is not there then rejected or didnt even add
         */

    }
}