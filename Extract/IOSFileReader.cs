using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Configuration;
using DocAnalysisNet.Util;
using DocAnalysisNet.Entity;
using YamlDotNet.Serialization;
namespace DocAnalysisNet.Extract
{

    public class IOSFileReader
    {
        //static string allFramework = @"C:\Pravangsu\MS\DocAnalysisNet\DocAnalysisNet\repository\raw\ios\AllFrameworks.html";
        static int iCount = 0;

        static string RAW_FOLDER_PATH = ConfigurationManager.AppSettings["RAW_FOLDER_PATH"]; //@"C:\Pravangsu\MS\DocAnalysisNet\DocAnalysisNet\repository\raw\";
        static string YML_FOLDER_PATH = ConfigurationManager.AppSettings["YML_FOLDER_PATH"]; //@"C:\Pravangsu\MS\DocAnalysisNet\DocAnalysisNet\repository\raw\";
        
        //static string IOS_ALL_FRAMEWORK = ConfigurationManager.AppSettings["IOS_ALL_FRAMEWORK"]; // @"android\AllFrameworks.html";
        static string allFramework = RAW_FOLDER_PATH + @"AllFramework\AllFrameworks_IOS.html";
        static string IOS_BASE_URL = ConfigurationManager.AppSettings["IOS_BASE_URL"]; //"https://developer.apple.com/library/ios/";
        //static string ANDROID_ALL_FRAMEWORK  = ConfigurationManager.AppSettings["ANDROID_ALL_FRAMEWORK"]; //@"android\AllFrameworks.html";

        public static void extractIOS()
        {
            //string IosBaseUrl = "https://developer.apple.com/library/ios/documentation/Miscellaneous/Conceptual/iPhoneOSTechOverview/";
            //string IosBaseUrl = "https://developer.apple.com/library/ios/";
            //WebClient webClient = new WebClient();
            // webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            //string page = webClient.DownloadString(Url);
            string exepath = Utility.GetExepath(); 
            allFramework = exepath + allFramework;
            string allFrameworkContent = String.Empty;
            if (File.Exists(allFramework))
            {
                allFrameworkContent = File.ReadAllText(allFramework);
            }

            if (allFrameworkContent != null && allFrameworkContent.Length > 0)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(allFrameworkContent);

                //List<HtmlNode> trlist = doc.DocumentNode.SelectSingleNode("//table[@class='graybox']").Descendants("tr").Skip(1).ToList();
                List<HtmlNode> trlist = doc.DocumentNode.SelectSingleNode("//table[@id='documentsTable']").Descendants("tr").Skip(1).ToList();// skip one for the header 
                //
                //.Where(tr => tr.Elements("td").Count() > 1)
                //.Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())

                foreach (var node in trlist)
                {
                    List<HtmlNode> tdList = node.Descendants("td").ToList();
                    if (tdList.Count > 3)
                    {

                        string resourceType = tdList[1].InnerText;
                        //var links = tdList[3].Descendants("a").ToList();
                        if (resourceType == "Reference")
                        {
                            var links = tdList[0].Descendants("a").ToList();
                            string fileName = tdList[0].InnerText.Replace("&nbsp;", "");
                            //fileName = fileName.Replace(":", "");
                            //fileName = fileName.Replace("?", "");
                            fileName = Util.Utility.RemoveIlligalChar(fileName);
                            string folderName = tdList[3].InnerText;
                            if (links != null)
                            {
                                var href = links[0].GetAttributeValue("href", string.Empty);
                                href = href.Replace("../", IOS_BASE_URL);

                                string fileContent = Util.Utility.extractDoc(href);
                                folderName = exepath + RAW_FOLDER_PATH + "ios" + "\\ " + folderName;
                                fileName = folderName + "\\" + fileName + ".xml";

                                if (!Directory.Exists(folderName))
                                {
                                    Directory.CreateDirectory(folderName);
                                }
                                System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
                                file.WriteLine(fileContent);
                                file.Close();
                            }
                        }
                    }
                }
            }
        }

       

        public static void CreateIOSYML()
        {
            string exepath = Utility.GetExepath(); 
            string folderName = exepath + RAW_FOLDER_PATH + "ios";
            if (Directory.Exists(folderName))
            {
                ProcessDirectory(folderName);
            }

        }
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        public static void ProcessFile(string path)
        {
            APIDocument ApiDoc = new APIDocument();

           
            HtmlDocument doc = Utility.LoadHtmlDoc(path);
            HtmlNode nodeClassName = doc.DocumentNode.SelectSingleNode("//h1[@class='chapter-name']");
            if (nodeClassName != null)
            {
                ApiDoc.ClassName = nodeClassName.InnerText;
                iCount++;
                Console.WriteLine("Processed file '{0}: {1}'.", iCount ,ApiDoc.ClassName);
            }

            HtmlNode nodeClassTypeParent = doc.DocumentNode.SelectSingleNode("//code[@class='code-voice']");
            if (nodeClassTypeParent != null && nodeClassTypeParent.ChildNodes.Count > 1)
            {
                string nodeClassType = nodeClassTypeParent.ChildNodes[1].InnerText;
                ApiDoc.ClassType = nodeClassType;
            }

            ApiDoc.ClassDesc = getClassDesc(doc);
            ApiDoc.AdoptedBy = getAdoptedBy(doc);
            ApiDoc.Methods=  getMethods(doc);

            SaveYml(ApiDoc);
        }
        private static void SaveYml(APIDocument ApiDoc)
        {
            Serializer s = new Serializer();
            string exepath = Utility.GetExepath();
            string folderName = exepath + YML_FOLDER_PATH + "ios" + "\\ ";
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            using (System.IO.StreamWriter writeFile = new System.IO.StreamWriter(folderName + ApiDoc.ClassName + ".yaml", false))
            {
                s.Serialize(writeFile, ApiDoc);
            }

            //System.IO.TextWriter writeFile = new StreamWriter(folderName + ApiDoc.ClassName + ".yaml");
          
        }
        private static string getClassDesc(HtmlDocument doc)
        {
            //HtmlNode nodeProtocolDesc = doc.DocumentNode.SelectSingleNode("//section[@class='z-protocol-description'] | //section[@class='z-class-introduction section'] | //section[@class='z-class-description section'] | //section[@class='intro section']");//
            HtmlNode nodeProtocolDesc = doc.DocumentNode.SelectSingleNode("//section[@class='z-protocol-description'] | //section[@class='z-class-introduction section'] | //section[@class='z-class-description section'] | //section[@class='intro section'] | //section[@class='z-protocol-description section']");//
            if (nodeProtocolDesc != null && nodeProtocolDesc.ChildNodes.Count > 0)
            {
                HtmlNode nodeDesc = nodeProtocolDesc.Descendants("p").FirstOrDefault();
                if (nodeDesc != null)
                {
                    return Utility.RemoveHTMLComments(nodeDesc.InnerText);
                }
                else
                    return string.Empty;
            }
            return string.Empty;
        }
        //private static string getClassDesc(HtmlDocument doc)
        //{
        //    HtmlNode nodeProtocolDesc = doc.DocumentNode.SelectSingleNode("//code[@class='code-voice']");//
        //    if (nodeProtocolDesc != null)
        //        return nodeProtocolDesc.InnerText;
        //    else
        //        return string.Empty;
        //}


        //

        private static List<string> getAdoptedBy(HtmlDocument doc)
        {
            List<string> adoptedby = new List<string>();
            HtmlNode nodeAdoptedBy = doc.DocumentNode.SelectSingleNode("//div[@class='adopted-by']");
            if (nodeAdoptedBy != null && nodeAdoptedBy.ChildNodes.Count > 1)
            {
                List<HtmlNode> nodes = nodeAdoptedBy.Descendants("code").ToList();
                foreach (HtmlNode n in nodes)
                {
                    string s = Utility.RemoveHTMLComments(n.InnerText);
                    adoptedby.Add(s);
                }
            }
            return adoptedby;
        }

        private static List<APIDocumentMethod> getMethods(HtmlDocument doc)
        {
            List<APIDocumentMethod> objList = new List<APIDocumentMethod>();
            HtmlNodeCollection objNode = doc.DocumentNode.SelectNodes("//ul[@class='task-group-list']");
            //List<HtmlNode> objNodelist = doc.DocumentNode.SelectNodes("//section[@class='section method']").ToList();
            if (objNode != null && objNode.Count > 0)
            {
                List<HtmlNode> UL = objNode.ToList();
                if (UL != null && UL.Count > 0)
                {
                    //foreach (HtmlNode UlList in UL)
                    //{
                    string liCriteria = "//li[@class='item symbol'] | //li[@class='item symbol obj-c-only on']";
                    if (doc.DocumentNode.SelectNodes(liCriteria) != null)
                    {
                        List<HtmlNode> objIls = doc.DocumentNode.SelectNodes(liCriteria).ToList();

                        foreach (HtmlNode il in objIls)
                        {
                            APIDocumentMethod method = new APIDocumentMethod();
                            bool isparameterExtracted= getMethodName(il, method);
                            if (method.MethodName != null && method.MethodName.Length > 0)
                            {
                                getDeclaration(il, method);
                                if (isparameterExtracted == false)
                                {
                                    getParametera(il, method);
                                }
                                objList.Add(method);
                            }
                        }
                    }

                    //}
                }
            }
            return objList;
        }

        private static bool getMethodName(HtmlNode n, APIDocumentMethod method)
        {
            bool isparameterExtracted = false;
            List<APIDocumentMethod> objList = new List<APIDocumentMethod>();
            List<HtmlNode> objNode = n.Descendants("a").ToList();//[@class='x-api-property-task-list instance-method Objective-C']");
            foreach (HtmlNode a in objNode)
            {
                if (a.Attributes["class"] != null 
                    && (a.Attributes["class"].Value == "x-api-property-task-list instance-method Objective-C"
                    || a.Attributes["class"].Value == "x-api-property-task-list function Objective-C"
                    || a.Attributes["class"].Value == "x-api-property-task-list class-method Objective-C"))
                    method.MethodName = Utility.RemoveHTMLComments(a.InnerText);
            }
            if (method.MethodName == "" || method.MethodName==null)
            {
                string criSecond = "//section[@class='section method']";
                if(  n.SelectNodes(criSecond) != null)
                {
                    List<HtmlNode> objMethods = n.SelectNodes(criSecond).ToList();
                    foreach (HtmlNode md in objMethods)
                    {
                        HtmlAgilityPack.HtmlDocument objSingleMethod = new HtmlAgilityPack.HtmlDocument();
                        objSingleMethod.LoadHtml(md.InnerHtml);
                        //class=""
                        HtmlNode m = objSingleMethod.DocumentNode.SelectSingleNode("//div[@class='declaration']");
                        if (m != null)
                        {
                            

                            HtmlNode Mymethod = m.SelectSingleNode("//div[@class='']");
                            if (Mymethod != null)
                            {
                                method.MethodName = Mymethod.InnerText.Replace("\n", "");
                            }
                            HtmlNode MymethodDesc = m.SelectSingleNode("//p[@class='para']");

                            if (method.MethodName == "" || method.MethodName == null)
                            {
                                // Make the 3rd attempt
                                string mName = m.InnerText.Replace("Declaration", "");
                                method.MethodName = mName.Replace("\n", ""); ;
                            }
                            string MethodDesc = MymethodDesc.InnerText.Replace("\n", "");
                            MethodDesc = Utility.RemoveIlligalChar(MethodDesc);
                            method.MethodDesc = Utility.RemoveSpace(MethodDesc);

                            method.Parameters = Utility.getParametersFromMethodString(method.MethodName);
                            isparameterExtracted = true;
                            
                        }
                        //

                    }
                }
            }
            return isparameterExtracted;
        }

       

        private static void getDeclaration(HtmlNode n, APIDocumentMethod method)
        {
            List<APIDocumentMethod> objList = new List<APIDocumentMethod>();
            //HtmlNode objDeclaration = n.SelectSingleNode("//div[@class='declaration']");
            //HtmlNode objDeclaration = n.SelectSingleNode("//div[@class='declaration']");
            HtmlAgilityPack.HtmlDocument objLiDoc = new HtmlAgilityPack.HtmlDocument();
            objLiDoc.LoadHtml(n.InnerHtml);
            HtmlNode objDeclaration = objLiDoc.DocumentNode.SelectSingleNode("//div[@class='declaration']");
            HtmlNode objDiscussion = objLiDoc.DocumentNode.SelectSingleNode("//div[@class='discussion']");

            if (objDiscussion != null)
            {
                method.MethodDesc = Utility.RemoveHTMLComments(objDiscussion.InnerText.Replace(System.Environment.NewLine, ""));
                method.MethodDesc =  method.MethodDesc.Replace("\n", "");
            }
            if (objDeclaration != null)
            {
                List<HtmlNode> dList = objDeclaration.Descendants("code").ToList();
                if (dList != null && dList.Count > 2)
                {
                    method.SwiftDeclaration = Utility.RemoveHTMLComments(dList[0].InnerText);
                    method.ObjCDeclaration = Utility.RemoveHTMLComments(dList[2].InnerText);
                }
            }
        }

        private static void getParametera(HtmlNode LiMethodNode, APIDocumentMethod method)
        {
            List<APIDocumentParameter> objList = new List<APIDocumentParameter>();
            HtmlAgilityPack.HtmlDocument objLiDoc = new HtmlAgilityPack.HtmlDocument();
            objLiDoc.LoadHtml(LiMethodNode.InnerHtml);
            if (objLiDoc.DocumentNode.SelectNodes("//table") != null && objLiDoc.DocumentNode.SelectNodes("//table").Descendants("tr") != null)
            {
                // IF null the Method Have zero parameter
                List<HtmlNode> trlist = objLiDoc.DocumentNode.SelectNodes("//table").Descendants("tr").ToList();
                foreach (var node in trlist)
                {
                    List<HtmlNode> tdList = node.Descendants("td").ToList();
                    if (tdList.Count > 1)
                    {
                        APIDocumentParameter p = new APIDocumentParameter();
                        p.ParameterName = Utility.RemoveHTMLComments(tdList[0].InnerText);
                        p.ParameterDesc = Utility.RemoveHTMLComments(tdList[1].InnerText);
                        objList.Add(p);
                    }
                    method.Parameters = objList;
                }
            }
        }
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
    }
}