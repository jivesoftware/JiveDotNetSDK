using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Authorization
    {
        public string agent { get; set; }
        public List<string> appNames { get; set; }
        public DateTime created { get; set; }
        public DateTime expires { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Resources.Resources resources { get; set; }
        public string type { get; set; }
    }
}
