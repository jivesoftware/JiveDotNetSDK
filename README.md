JiveDotNetSDK
=============

.Net SDK for working with Jive. This is a work in progress. Many Clients have still not been implemented.

See Blog Post Announcement for More Details

https://community.jivesoftware.com/community/developer/blog/2014/08/21/jive-net-sdk-on-github

## Installation
To include JiveDotNetSDK in your .NET project, follow these steps.
 1. Clone the source to your project root directory.
  - If you have the git command line tools for Windows installed, simply navigate to your project root and run the command:
  ```git
  git clone https://github.com/jivesoftware/JiveDotNetSDK
  ```
 2. Build the JiveDotNetSDK project
  - Open the solution file in from the JiveDotNetSDK source.
  - Once the project is open in Visual Studio, change the build mode to "Release" and then click Build > Build Solution (F6) to build the project into a dll.
 3. Add a reference to the JiveDotNetSDK to your project.
  - Open the project in which you want to use JiveDotNetSDK.
  - In Visual Studio, right click "References" and "Add Reference".
  - In the Refenece Manager tool, click "Browse" and navigate to the folder where you cloned JiveDotNetSDK.
  - From there, select the file at ```Net.Pokeshot.JiveSdk\bin\Release\Net.Pokeshot.JiveSdk.dll```.
  - Click "Ok" in the Reference Manager and the reference will be added to your project.
 4. Add ```using Net.Pokeshot.JiveSdk.<Auth>|<Clients>|<Models>|<Util>``` to the .cs files where you want to use the SDK.
 
## Usage
 The SDK is modeled closely after [Jive's REST API v3.14](https://developers.jivesoftware.com/api/v3/cloud/rest/index.html).
 
### Models
 The Models in the SDK are simply the Types defined by the REST API Documentation implemented in C# classes.
 
### Clients
 The Clients in the SDK are a C# wrapper around the services defined by the REST API Documentation. To use a client, you must first instantiate one using your Jive instance URL and credentials. Then you can make Web requests to your instance using the Client's methods.


## Examples
Here are some examples of using the JiveDotNetSDK to interface with your Jive instance.

### Getting Users
```C#
using System.Collections.Generic;
using Net.Pokeshot.JiveSdk.Models;
using Net.Pokeshot.JiveSdk.Clients;
using System.Net;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string jiveUrl = "https://example.jiveon.com";
            var creds = new NetworkCredential("username", "password");

            PeopleClient p = new PeopleClient(jiveUrl, creds);

            // Gets a list of all the Users on your instance of Jive.
            List<Person> people = p.GetPeople();
        }
    }
}
```

### Printing the names of the recent Authors
```C#
using System.Collections.Generic;
using Net.Pokeshot.JiveSdk.Models;
using Net.Pokeshot.JiveSdk.Clients;
using System.Net;
using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string jiveUrl = "https://example.jiveon.com";
            var creds = new NetworkCredential("username", "password");

            ContentsClient c = new ContentsClient(jiveUrl, creds);

            // Get all Content created in the last day.
            List<GenericContent> content = c.GetContents(new Tuple<DateTime, DateTime>(DateTime.Now.AddDays(-1), DateTime.Now), null, int.MaxValue);

            foreach (var item in content)
            {
                Console.WriteLine(item.author.displayName);
            }
        }
    }
}
```
