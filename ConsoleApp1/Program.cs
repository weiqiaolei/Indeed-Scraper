using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using AngleSharp.Parser.Html;
using AngleSharp.Extensions;
using AngleSharp.Dom.Html;
using AngleSharp;
using System.Text.RegularExpressions;

namespace JobWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var webClient = new WebClient();
            var html = webClient.DownloadString("https://www.indeed.com/jobs?q=developer&l=los+angeles%2Cca");

            //char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            var parser = new HtmlParser();
            var document = parser.Parse(html);

            var str = "aSdF HtML";
            var str2 = " ,asdf python, perl, angularjs, yeah!?#$%^&*()";
            str2 = str2.Replace(",", "");
            var str3 = Regex.Replace(str2, @",","");

            string keyWords = @"\b(python|perl|angularjs)\b";
            MatchCollection str4 = Regex.Matches(str2, keyWords, RegexOptions.Compiled);

            Console.WriteLine(str.ToLower());
            Console.WriteLine(str2);
            Console.WriteLine("STR3: " + str3);

            foreach (var k in str4) { 
            Console.WriteLine("STR4: " + k);
            }
            //Grabs all links on indeed with anchor tag and contains "jobtitle" as class
            //var links = document.QuerySelectorAll("*").Where(m => m.LocalName == "a" && m.ClassList.Contains("jobtitle"));

            /*
            var links = document.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
            var links2 = links.Cast<IHtmlAnchorElement>()
                     .Select(m => m.Href)
                     .ToList();
            */


            //var jobSummary = document.QuerySelector("#job_summary");
            //string[] arrJobSum = jobSummary.TextContent.Split(delimiterChars); //DELIMS the string to array
            //arrJobSum = arrJobSum.Select(s => s.ToLowerInvariant()).ToArray(); //Replaces array as lower case
            /*
            foreach (string s in arrJobSum) {
                switch (s)
                {
                    case "angularjs":
                        Console.WriteLine("AngularJs found");
                        break;
                    case "knockoutjs":
                        Console.WriteLine("KnockOutJs found");
                        break;
                    case "react":
                    case "reacjs":
                        Console.WriteLine("ReactJs Found");
                        break;
                    case "vue":
                    case "vuejs":
                        Console.WriteLine("VueJS Found");
                        break;

                }
            }
            */




            //// Setup the configuration to support document loading
            //var config = Configuration.Default.WithDefaultLoader();
            //// Load the names of all The Big Bang Theory episodes from Wikipedia
            //var address = "https://en.wikipedia.org/wiki/List_of_The_Big_Bang_Theory_episodes";
            //// Asynchronously get the document in a new context using the configuration
            //var document = await BrowsingContext.New(config).OpenAsync(address);
            //// This CSS selector gets the desired content
            //var cellSelector = "tr.vevent td:nth-child(3)";
            //// Perform the query to get all cells with the content
            //var cells = document.QuerySelectorAll(cellSelector);
            //// We are only interested in the text - select it with LINQ
            //var titles = cells.Select(m => m.TextContent);

        }
    }
}
