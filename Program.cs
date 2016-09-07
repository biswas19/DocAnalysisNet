using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocAnalysisNet
{
    class Program
    {
        //static string allFramework = @"C:\Pravangsu\MS\DocAnalysisNet\DocAnalysisNet\repository\raw\ios\AllFrameworks.html";
        //static string IOS_RAW_REPO_PATH = @"\repository\raw\ios";
        static void Main(string[] args)
        {

            //string str = "https://developer.apple.com/library/ios/documentation/Miscellaneous/Conceptual/iPhoneOSTechOverview/iPhoneOSFrameworks/iPhoneOSFrameworks.html";
           // string str = "https://developer.apple.com/library/ios/navigation/";


            //Extract.IOSFileReader.extractIOS();
            //Extract.AndroidFileReader.extractAnroid();
           //Extract.IOSFileReader.CreateIOSYML();
            Extract.AndroidFileReader.CreateYML();
            //TestArea.getAnroidList();
            Console.ReadLine();
           
        }

        //private static void extractIOS(string Url, bool doRecussion)
        //{
        //    //string IosBaseUrl = "https://developer.apple.com/library/ios/documentation/Miscellaneous/Conceptual/iPhoneOSTechOverview/";
        //    //string IosBaseUrl = "https://developer.apple.com/library/ios/";
        //    //WebClient webClient = new WebClient();
        //    //webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //    //string page = webClient.DownloadString(Url);

        //    string allFrameworkContent = String.Empty;
        //    if (File.Exists(allFramework))
        //    {
        //        allFrameworkContent = File.ReadAllText(allFramework);
        //    }

        //    if (allFrameworkContent != null && allFrameworkContent.Length > 0)
        //    {
        //        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //        doc.LoadHtml(allFrameworkContent);

        //        //List<HtmlNode> trlist = doc.DocumentNode.SelectSingleNode("//table[@class='graybox']").Descendants("tr").Skip(1).ToList();
        //        List<HtmlNode> trlist = doc.DocumentNode.SelectSingleNode("//table[@id='documentsTable']").Descendants("tr").Skip(1).ToList();// skip one for the header 
        //        //
        //        //.Where(tr => tr.Elements("td").Count() > 1)
        //        //.Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())

        //        foreach (var node in trlist)
        //        {
        //            List<HtmlNode> tdList = node.Descendants("td").ToList();
        //            if (tdList.Count > 3)
        //            {

        //                string resourceType = tdList[1].InnerText;
        //                //var links = tdList[3].Descendants("a").ToList();
        //                if (resourceType == "Reference")
        //                {
        //                    var links = tdList[0].Descendants("a").ToList();
        //                    string fileName = tdList[0].InnerText.Replace("&nbsp;", "");
        //                    fileName = fileName.Replace(":", "");
        //                    fileName = fileName.Replace("?", "");
        //                    string folderName = tdList[3].InnerText;
        //                    if (links != null)
        //                    {
        //                        var href = links[0].GetAttributeValue("href", string.Empty);
        //                        href = href.Replace("../", IosBaseUrl);
        //                        string exepath = AppDomain.CurrentDomain.BaseDirectory;
        //                        exepath = exepath.Replace("\\bin", "");
        //                        exepath = exepath.Replace("\\Debug", "");
        //                        string fileContent = extractDoc(href, false);
        //                        folderName = exepath + IOS_RAW_REPO_PATH + "\\ " + folderName;
        //                        fileName = folderName + "\\" + fileName + ".xml";
        //                        if (!Directory.Exists(folderName))
        //                        {
        //                            Directory.CreateDirectory(folderName);
        //                        }
        //                        System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
        //                        file.WriteLine(fileContent);
        //                        file.Close();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //private static string extractDoc(string Url, bool doRecussion)
        //{
        //    //HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
        //    //myRequest.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //    //myRequest.Method = "GET";
        //    //WebResponse myResponse = myRequest.GetResponse();
        //    //StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
        //    //string result = sr.ReadToEnd();
        //    //sr.Close();
        //    //myResponse.Close();
        //    WebClient webClient = new WebClient();
           
        //    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //    string result = webClient.DownloadString(Url);

        //    return result;
        //}
        ////public static string StripHTML(string HTMLText, bool decode = true)
        ////{
        ////    Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
        ////    var stripped = reg.Replace(HTMLText, "");
        ////    return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
        ////}

    }
}
