using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Resources
{
    public abstract class Resource
    {
        public List<string> allowed { get; set; }
        public string @ref { get; set; }
    }
}
