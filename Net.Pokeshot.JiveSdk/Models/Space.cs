using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Space : Place
    {
        public int childCount { get; set; }
        public string displayName { get; set; }
        public string locale { get; set; }
        public string parent { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
    }
}
