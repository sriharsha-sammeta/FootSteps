using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps
{
    public sealed class LocalPerson
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string name { get; set; }
        public string phoneNo { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
