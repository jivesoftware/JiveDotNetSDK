using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    /// <summary>
    /// Contains the properties of all Content subclasses. Can be used when the type of content is unknown.
    /// </summary>
    public class GenericContent : Content
    {
        // Specific to Discussions
        public string answer { get; set; }
        public List<string> helpful { get; set; }
        public OnBehalfOf onBehalfOf { get; set; }
        public bool question { get; set; }
        public string resolved { get; set; }
        public bool restrictReplies { get; set; }

        // Specific to Documents
        public List<Person> approvers { get; set; }
        public string authorship { get; set; }
        public Person editingBy { get; set; }
        public string fromQuest { get; set; }
        public Boolean restrictComments { get; set; }

        // Specific to Favorite
        public Object favoriteObject { get; set; }
        public Boolean @private { get; set; }

        // Specific to File
        public List<Person> authors { get; set; }
        //public string authorship { get; set; }
        public string binaryURL { get; set; }
        public string contentType { get; set; }
        //public Boolean restrictComments { get; set; }
        public int size { get; set; }
        public Person users { get; set; }

        // Specific to Idea
        //public string authorship { get; set; }
        public string authorshipPolicy { get; set; }
        public int commentCount { get; set; }
        public int score { get; set; }
        public string stage { get; set; }
        //public List<Person> users { get; set; }
        public int voteCount { get; set; }
        public Boolean voted { get; set; }

        // Specific to Poll
        public List<String> options { get; set; }
        public List<PollOptionImage> optionsImages { get; set; }
        //public List<Person> users { get; set; }
        //public int voteCount { get; set; }
        public List<string> votes { get; set; }

        // Specific to Post
        public List<Attachment> attachments { get; set; }
        public DateTime publishDate { get; set; }
        //public Boolean restrictComments { get; set; }

        // Specific to Slide
        //public DateTime publishDate { get; set; }
        public int replyCount { get; set; }
        public int sortKey { get; set; }
        public string targetLink { get; set; }

        // Specific to Stage
        public string backgroundColor { get; set; }
        public string foregroundColor { get; set; }
        public int ideasCount { get; set; }
        public Boolean stageDefault { get; set; }
        public Boolean stageEnabled { get; set; }
        public Boolean votingEnabled { get; set; }

        // Specific to Task
        public Boolean completed { get; set; }
        public DateTime dueDate { get; set; }
        public string owner { get; set; }
        public string parentTask { get; set; }
        public List<string> subTasks { get; set; }

        // Specific to Update
        public string latitude { get; set; }
        public string longitude { get; set; }
        public Update repost { get; set; }
    }
}
