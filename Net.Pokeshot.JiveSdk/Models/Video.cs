using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Video : Content
    {
        public int abuseCount { get; set; }
        public int favoriteCount { get; set; }
        public bool followed { get; set; }
        public bool promotedResult { get; set; }
    }
}
