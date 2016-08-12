using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Metadata
{
    public class Resource
    {
        public string availability { get; set; }
        public string description { get; set; }
        public string example { get; set; }
        public bool hasBody { get; set; }
        public string jsMethod { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string since { get; set; }
        public bool unpublished { get; set; }
        public string verb { get; set; }
    }
}
