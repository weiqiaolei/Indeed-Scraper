using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program4
    {
        //class to hold parsed values
        static class Global
        {
            public static int IntTotalResult { get; set; }
            public static List<string> ListIndeedPagUrl = new List<string>();
            public static List<string> ListAllIndeedPostings = new List<string>();

            public static int IntTotalException { get; set; }

            public static List<Job> JobList = new List<Job>();
        }

        //-----------Job for EACH individual scraped link
        class Job
        {
            public string JobTitle { get; set; }
            public string CompanyName { get; set; }
            public string DatePosted { get; set; }
            public string Location { get; set; }
            public string UrlLink { get; set; }
            public string JobBody { get; set; }

            public List<string> Tags = new List<string>();
        }

        //-----------MAIN OF THE CONSOLE APP
        static void Main(string[] args)
        {
            var parser = new AngleSharp.Parser.Html.HtmlParser();
            var webClient = new WebClient();

            var url = "https://www.indeed.com/jobs?q=developer&l=los+angeles%2C+ca&sort=date";
            Global.ListIndeedPagUrl.Add(url);

            var html = webClient.DownloadString(url);
            var document = parser.Parse(html);
            var result = document.QuerySelector("div[id='searchCount']");
            var str = (string)result.TextContent;


            ////----------REGEX OUT "Jobs 1 to 10 of" & the "," then convert it to an INT
            //String SearchCount = Regex.Replace(str, @"\bJobs 1 to 10 of\b", String.Empty);
            //String iSearchCount = Regex.Replace(SearchCount, @",", String.Empty);
            //int iParsed = Int32.Parse(iSearchCount);
            //Global.IntTotalResult = RoundOff(iParsed);

            //Console.WriteLine("JOBS FOUND: " + Global.IntTotalResult);

            Global.IntTotalResult = 1000;

            //-----------Adds Each Pagineated Page || upto 1000 job listings
            //-----------i <= 1000 && i <= Global.IntTotalResult;
            for (var i = 10; i <= 21 && i <= Global.IntTotalResult; i += 10)
            {
                Global.ListIndeedPagUrl.Add("https://www.indeed.com/jobs?q=developer&l=los+angeles%2Cca&sort=date&start=" + i);
            }



            //-----------Grab all job links per each pagineated
            var countLink = 1;
            foreach (var listings in Global.ListIndeedPagUrl)
            {
                html = webClient.DownloadString(listings);
                document = parser.Parse(html);
                var queryJobLinks = document.QuerySelectorAll("div[data-tn-component='organicJob']");

                //----------1. After each jobs are gathered.
                //----------2. For each job with <div[organicjob]> linked gathered, cycle through each main <div>.
                foreach (var jobs in queryJobLinks)
                {
                    try
                    {
                        var scrapedJobTitle = jobs.QuerySelector("a[class='turnstileLink']").GetAttribute("title");
                        var scrapedCompanyName = jobs.QuerySelector("span[class='company']").TextContent;
                        var scrapedDatePosted = jobs.QuerySelector("span[class='date']").TextContent;
                        var scrapedLocation = jobs.QuerySelector("span[class='location']").TextContent;
                        var scrapedUrl = "https://www.indeed.com" + jobs.QuerySelector("a[class='turnstileLink']").GetAttribute("href");

                        string scrapedBody;
                        List<string> scrapedTags = new List<string>();

                        Console.WriteLine(scrapedCompanyName + "   <<<<<<<<<<");
                        Console.WriteLine(" ");
                        Console.WriteLine(scrapedJobTitle + " || " + scrapedLocation + " || " + scrapedDatePosted);
                        Console.WriteLine(scrapedUrl);

                        try
                        {
                            html = webClient.DownloadString(scrapedUrl);
                            document = parser.Parse(html);
                            var urlSelectedBody = document.QuerySelector("body").TextContent;
                            var bodyContent = urlSelectedBody.ToLower();

                            var withoutHtml = StripHTML(bodyContent);

       
                            scrapedBody = Regex.Replace(withoutHtml, "[+!?()_;:,\"\'/]", " ");

                            Console.WriteLine(scrapedBody);

                            string keyWords = @"\b(
                            |scrum|agile|

                            |developer|back(| |-)end|front(| |-)end|full stack|full-stack|programmer|
                            |quality assurance|software engineer|web developer|
                                                        
                            |html|html5|
                            |css|css3|css4|
                            |es6|es 6|ecma6|ecma 6|ecmascript6|ecmascript 6|javascript|javascript6|jquery|typescript|
                            |angular|angular.js|angular2|angular 2|angular4|angular 4|
                            |react(|js|.js)|
                            |swift|
                            |vue|vuejs|vue.js|
                            
                            |c\+\+|c#|java|
                            |obj-c|objectivec|objective(c| c|-c)|
                            |perl|php|python|ruby|ruby on rails|
                            |shell|

                            |express|ms sql|mysql|node|node.js|nosql|postgress(sql|)|sql|sql-server|

                            |asp.net|.net core|
                            |lamp|lamp-stack|
                            |mean-stack|mean stack|
                                
                            |android|ios|unix|linux|mac|windows|

                            )\b";

                            MatchCollection match = Regex.Matches(scrapedBody, keyWords, RegexOptions.Compiled);

                            var uniqueMatch = match
                                .OfType<Match>()
                                .Select(m => m.Value)
                                .Distinct();

                            foreach (var m in uniqueMatch)
                            {
                                Console.WriteLine(m);
                            }
                            Console.WriteLine("==================================================================================");
                            Console.WriteLine(" ");
                        }
                        catch (WebException e)
                        {
                            Console.WriteLine(e);
                            scrapedBody = null;
                            scrapedTags = null;
                        }


                        Global.JobList.Add(new Job
                        {
                            JobTitle = scrapedJobTitle,
                            CompanyName = scrapedCompanyName,
                            DatePosted = scrapedDatePosted,
                            Location = scrapedLocation,
                            UrlLink = scrapedUrl,
                            JobBody = scrapedBody,

                            Tags = null //List
                            //ADD THE LIST of TAGS TO THIS AS WELL!
                        });

                    }
                    catch (WebException e)
                    {
                        Console.WriteLine(e);
                        Global.IntTotalException++;
                    }

                    countLink++;
                    
                }
            }
            Console.WriteLine(Global.IntTotalException);
        }



        public static int RoundOff(int i)
        {
            return ((int)Math.Round(i / 10.0)) * 10;
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
