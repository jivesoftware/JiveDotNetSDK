using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class IdeaVote
    {
        public Idea idea { get; set; }
        public bool promote { get; set; }
        public string type { get; set; }
        public Person voter { get; set; }
    }
}
