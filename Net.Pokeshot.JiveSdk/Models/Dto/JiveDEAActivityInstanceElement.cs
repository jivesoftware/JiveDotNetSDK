using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAActivityInstanceElement
    {
        public JiveDEAActor actor { get; set; }
        public string action { get; set; }
        public JiveDEAActionObject actionObject { get; set; }
        public long activityTime { get; set; }
        public JiveDEADestination destination { get; set; }

    }
}
