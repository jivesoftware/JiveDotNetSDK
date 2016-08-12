using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Outcome
    {
        public DateTime creationDate { get; set; }
        public string id { get; set; }
        public string note { get; set; }
        public OutcomeType outcomeType { get; set; }
        public string parent { get; set; }
        public string predecessorOutcome { get; set; }
        public Resources.Resources resources { get; set; }
        public string root { get; set; }
        public string status { get; set; }
        public List<OutcomeType> successorOutcomesTypes { get; set; }
        public DateTime updated { get; set; }
        public Person user { get; set; }
    }
}
