using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Metadata
{
    public class TimeZone
    {
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public string id { get; set; }
        public int rawOffset { get; set; }
    }
}
