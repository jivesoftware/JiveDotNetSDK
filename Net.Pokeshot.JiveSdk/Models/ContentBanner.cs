using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    /// <summary>
    /// This object defines the configuration for displaying a content banner.
    /// </summary>
    public class ContentBanner
    {
        public string bgColor { get; set; }
        public string id { get; set; }
        public string imageURI { get; set; }
        public string position { get; set; }
        public string repeat { get; set; }
        public string textColor { get; set; }
    }
}
