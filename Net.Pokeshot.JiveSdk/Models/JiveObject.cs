using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    abstract class JiveObject
    {
        private JiveRetrieval retrieval = JiveRetrieval.Instance;
        public string Url { get; }
    }
}
