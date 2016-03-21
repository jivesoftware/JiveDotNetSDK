using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
     public class Document: Content{
         public Person approvers {get;set;}
         public string authorship {get;set;}
         public Person editingBy {get;set;}
         public string fromQuest {get;set;}
         public Boolean restrictComments {get;set;}
         

     
    }
}
