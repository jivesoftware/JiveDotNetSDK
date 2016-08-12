using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Metadata
{
    public class Object
    {
        public bool associatable { get; set; }
        public string availability { get; set; }
        public bool commentable { get; set; }
        public bool content { get; set; }
        public string description { get; set; }
        public string example { get; set; }
        public List<Field> fields { get; set; }
        public string name { get; set; }
        public int objectType { get; set; }
        public List<OutcomeType> outcomeTypes { get; set; }
        public bool place { get; set; }
        public string plural { get; set; }
        public Metadata.Resource resourceLinks { get; set; }
        public string since { get; set; }
        public bool likeable { get; set; }
    }
}
