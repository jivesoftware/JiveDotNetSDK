using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    /// <summary>
    /// Jive Core API entity representing an event.
    /// </summary>
    public class Event : Content
    {
        public int abuseCount { get; set; }
        public List<Person> authors { get; set; }
        public string authorship { get; set; }
        public ContentBanner banner { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public DateTime endDate { get; set; }
        public string eventAccess { get; set; }
        public int eventAccessID { get; set; }
        public EventCategoryType eventCategory { get; set; }
        public int favoriteCount { get; set; }
        public bool followed { get; set; }
        public string language { get; set; }
        public string location { get; set; }
        public int maxAttendees { get; set; }
        public string phone { get; set; }
        public bool promotedResult { get; set; }
        public bool spansMultipleDays { get; set; }
        public DateTime startDate { get; set; }
        public string street { get; set; }
        public string url { get; set; }
        public List<Person> users { get; set; }
    }

    public class EventCalendarCount
    {
        public DateTime calendarDate { get; set; }
        public int count { get; set; }
        public string id { get; set; }
        public Resources.Resources resources { get; set; }
    }

    public class EventCategoryType
    {
        public int followerCount { get; set; }
        public string hexColor { get; set; }
        public string id { get; set; }
        public int likeCount { get; set; }
        public string localizedDisplayName { get; set; }
        public string name { get; set; }
        public DateTime published { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
        public bool followed { get; set; }
    }
}
