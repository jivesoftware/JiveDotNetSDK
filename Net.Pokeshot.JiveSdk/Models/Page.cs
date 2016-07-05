using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Page : Content
    {
        public int displayOrder { get; set; }
        public string layout { get; set; }
        public string name { get; set; }
        public string pageType { get; set; }
        public List<JiveTileInstance> tiles { get; set; }
    }
}
