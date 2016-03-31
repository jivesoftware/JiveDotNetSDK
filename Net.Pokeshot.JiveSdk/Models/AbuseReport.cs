using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class AbuseReport
    {
        public Person author { get; set; }
        public string category { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public string objectUri { get; set; }
        public DateTime published { get; set; }
        public Object resources { get; set; }
        public string type { get; set; }
    }
}
