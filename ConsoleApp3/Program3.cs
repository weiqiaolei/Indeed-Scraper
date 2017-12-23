using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApp3
{
    class Program3
    {
        static class Global
        {
            public static int IntTotalResult { get; set; }
            public static List<string> ListIndeedPagUrl = new List<string>();
            public static List<string> ListAllIndeedPostings = new List<string>();

            public static int IntTotalException { get; set; }

            public static List<Job> JobList;
        }

        class Job
        {
            public string JobTitle { get; set; }
            public string CompanyName { get; set; }
            public string DatePosted { get; set; }
            public string Location { get; set; }
            public string UrlLink { get; set; }

            public List<string> Tags = new List<string>();
        }

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

            //----------REGEX OUT 'Jobs 1 to 10 of' & the ',' then conver to an INT
            String SearchCount = Regex.Replace(str, @"\bJobs 1 to 10 of\b", String.Empty);
            String iSearchCount = Regex.Replace(SearchCount, @",", String.Empty);
            Global.IntTotalResult = RoundOff(Int32.Parse(iSearchCount));

            Console.WriteLine("JOBS FOUND: " + Global.IntTotalResult);

            //-----------Adds the total Searchcount || upto 1000 job listings
            //-----------i <= 25 && i <= Global.IntTotalResult;
            for (var i = 10; i <= 1000 && i <= Global.IntTotalResult; i += 10)
            {
                Global.ListIndeedPagUrl.Add("https://www.indeed.com/jobs?q=developer&l=los+angeles%2Cca&sort=date&start=" + i);
            }

            //-----------Grabs every job posting for each of the pages.
            var countLink = 1;
            foreach (var listings in Global.ListIndeedPagUrl)
            {
                html = webClient.DownloadString(listings);
                document = parser.Parse(html);
                var queryJobLinks = document.QuerySelectorAll("a[class='turnstileLink']");

                foreach (var jobs in queryJobLinks)
                {
                    Global.ListAllIndeedPostings.Add("https://www.indeed.com" + jobs.GetAttribute("href"));

                    Console.WriteLine("getting " + countLink + ": " + ("https://www.indeed.com" + jobs.GetAttribute("href")));
                    countLink++;
                }
            }


            ////-----------Distinct the pages to reduce repetitions of URL Links
            //if (Global.ListAllIndeedPostings.Count != Global.ListAllIndeedPostings.Distinct().Count())
            //{
            //    Console.WriteLine("                   ##");
            //    Console.WriteLine("               ##########");
            //    Console.WriteLine("          ####################");
            //    Console.WriteLine("     ##############################");
            //    Console.WriteLine("########################################");
            //    Console.WriteLine("Duplicates Exists");
            //    Console.WriteLine("Non-Unique Total: " + Global.ListAllIndeedPostings.Count());
            //    Global.ListAllIndeedPostings = Global.ListAllIndeedPostings.Distinct().ToList();
            //    Console.WriteLine("Now Unique Total: " + Global.ListAllIndeedPostings.Count());
            //    Console.WriteLine("########################################");
            //}


            //----------Scrape Text from EVERY PAGE
            //----------Add a counter to EVERY KEYWORD
            var linkCount = 1;
            foreach (var links in Global.ListAllIndeedPostings)
            {
                try
                {
                    Console.WriteLine("Link Count " + linkCount + ": " + links);
                    linkCount++;

                    html = webClient.DownloadString(links);
                    document = parser.Parse(html);
                    var jobSummary = document.QuerySelector("body");

                    char[] delimiterChars = { ' ', ',', ':', '\t' };
                    string[] arrJobSum = jobSummary.TextContent.Split(delimiterChars);
                    arrJobSum = arrJobSum.Select(s => s.ToLowerInvariant()).ToArray();

                    foreach (string s in arrJobSum)
                    {
                        switch (s)
                        {
                            case "javascript":
                            case "html":
                            case "css":
                                Console.WriteLine("HTML/CSS/Javascript Found");
                                break;
                            case "angularjs":
                            case "angular.js":
                                Console.WriteLine("AngularJs found");
                                break;
                            case "knockoutjs":
                                Console.WriteLine("KnockOutJs found");
                                break;
                            case "react":
                            case "reactjs":
                                Console.WriteLine("ReactJs Found");
                                break;
                            case "vue":
                            case "vuejs":
                                Console.WriteLine("VueJS Found");
                                break;
                            case "vue.js":
                                Console.WriteLine("Vue.JS Found");
                                break;
                            case "python":
                                Console.WriteLine("Python Found");
                                break;
                            case "java":
                                Console.WriteLine("Java Found");
                                break;
                            case "asp":
                            case "asp.net":
                                Console.WriteLine("ASP Found");
                                break;
                            case "csharp":
                            case "c#":
                                Console.WriteLine("C# Found");
                                break;
                            case "sql":
                                Console.WriteLine("SQL Server Found");
                                break;
                            case "c++":
                                Console.WriteLine("C++ Found");
                                break;
                            case "c":
                                Console.WriteLine("C Found");
                                break;
                            case "perl":
                                Console.WriteLine("perl Found");
                                break;
                            case "mongodb":
                            case "mongo":
                                Console.WriteLine("mongoDB Found");
                                break;
                            case "mean":
                                Console.WriteLine("MeanStack Found");
                                break;
                            case "lamp":
                                Console.WriteLine("LampStack Found");
                                break;
                            case "express":
                                Console.WriteLine("Express Found");
                                break;
                            case "node":
                            case "nodejs":
                                Console.WriteLine("NodeJS Found");
                                break;
                            case "php":
                                Console.WriteLine("PHP Found");
                                break;
                            case "mysql":
                                Console.WriteLine("MySQL Found");
                                break;
                            case "ruby":
                                Console.WriteLine("Ruby Found");
                                break;
                            case "objective-c":
                                Console.WriteLine("Objective-C Found");
                                break;
                            case "oracle":
                                Console.WriteLine("Oracle Found");
                                break;
                            case "jquery":
                                Console.WriteLine("jQuery Found");
                                break;
                        }
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine(e);
                    Global.IntTotalException++;
                }
            }
            Console.WriteLine(" ");
            Console.WriteLine("***************************");
            Console.WriteLine("**********RESULTS**********");
            Console.WriteLine("Now Unique Total: " + Global.ListAllIndeedPostings.Count());
            Console.WriteLine("Total Links Scraped: " + linkCount);
            Console.WriteLine("Exception Total: " + Global.IntTotalException);
        }

        public static int RoundOff(int i)
        {
            return ((int)Math.Round(i / 10.0)) * 10;
        }
    }
}
