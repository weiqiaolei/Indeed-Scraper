using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp2
{
    class Program2
    {
        static void Main(string[] args)
        {
            List<string> paginationUrl = new List<string>();
            List<string> uniqueIndeedUrls = new List<string>();
            List<string> jobDesLinks = new List<string>();

            var webClient = new WebClient();
            var html = webClient.DownloadString("https://www.indeed.com/q-developer-l-los-angeles,ca-jobs.html");
            var parser = new AngleSharp.Parser.Html.HtmlParser();
            var document = parser.Parse(html);

            //GRABS LINKS for EACH FIRST PAGE
            var queryJobLinks = document.QuerySelectorAll("a[class='turnstileLink']");
            foreach (var item in queryJobLinks)
            {
                jobDesLinks.Add("https://www.indeed.com" + item.GetAttribute("href"));
            }


            //INDEED PAGINATION LINK SCRAPER
            var queryNextLink = document.QuerySelectorAll("a[onmousedown='addPPUrlParam && addPPUrlParam(this);']");
            foreach (var next in queryNextLink)
            {
                html = webClient.DownloadString("https://www.indeed.com" + next.GetAttribute("href"));
                paginationUrl.Add("https://www.indeed.com" + next.GetAttribute("href"));
            }

            if (paginationUrl.Count != paginationUrl.Distinct().Count())
            {
                //Console.WriteLine("Duplicates Exists");
                uniqueIndeedUrls = paginationUrl.Distinct().ToList();
                //uniqueIndeedUrls.ForEach(i => Console.WriteLine("{0}\t", i));

            }

            //FOR EACH PAGINATION, CYCLE THROUGH AND GRAB THE LINKS
            foreach (var url in uniqueIndeedUrls)
            {
                Console.WriteLine("URL:         " + url);
                html = webClient.DownloadString(url);
                document = parser.Parse(html);

                //GRABS ALL LINKS FROM THAT PAGE
                queryJobLinks = document.QuerySelectorAll("a[class='turnstileLink']");
                foreach (var item in queryJobLinks)
                {
                    jobDesLinks.Add("https://www.indeed.com" + item.GetAttribute("href"));
                }
            }



            jobDesLinks.ForEach(i => Console.WriteLine("{0}\t", i));
            Console.WriteLine("Total Links {0}", jobDesLinks.Count);

            DoStuff();

            File.WriteAllText("D:/filename.html", html);



        }
        public static void DoStuff()
        {
            Console.WriteLine("Do Stuff !!!!!!!!!!");
        }
    }
}



/*
 *      static void Main(string[] args)
        {
            var webClient = new WebClient();
            var html = webClient.DownloadString("https://www.indeed.com/q-developer-l-los-angeles,ca-jobs.html");

            var parser = new AngleSharp.Parser.Html.HtmlParser();
            var document = parser.Parse(html);


            File.WriteAllText("D:/filename.html", html);

            //LINQ equivalent CSS selector example
            var queryItems = document.QuerySelectorAll("a[class='turnstileLink']");
            //print href attributes value to console
            foreach (var item in queryItems)
            {
                Console.WriteLine(item.GetAttribute("href"));
            }
        }

 
     
     */
