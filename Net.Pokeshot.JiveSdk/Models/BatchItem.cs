using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class BatchItem
    {
        public string key { get; set; }
        public BatchRequest request { get; set; }
    }

    public class BatchRequest
    {
        public string endpoint { get; set; }
        public string method { get; set; }
    }

    public class BatchResponse
    {
        //public ??? data { get; set; }
        //public ??? error { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<BatchResponse> result { get; set; }
        public int status { get; set; }
    }
}
