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

    public class AndroidFileReader
    {
        //static string allFramework = @"C:\Pravangsu\MS\DocAnalysisNet\DocAnalysisNet\repository\raw\ios\AllFrameworks.html";

        static int iCount = 0;
        static string RAW_FOLDER_PATH = ConfigurationManager.AppSettings["RAW_FOLDER_PATH"];
        static string ANDROID_BASE_URL = ConfigurationManager.AppSettings["ANDROID_BASE_URL"];
        static string allFramework = RAW_FOLDER_PATH + @"AllFramework\AllFrameworks_Android.html";
        static string YML_FOLDER_PATH = ConfigurationManager.AppSettings["YML_FOLDER_PATH"]; //@"C:\Pravangsu\MS\DocAnalysisNet\DocAnalysisNet\repository\raw\";


        public static void extractAnroid()
        {
            string exepath = AppDomain.CurrentDomain.BaseDirectory;
            exepath = exepath.Replace("\\bin", "");
            exepath = exepath.Replace("\\Debug", "");
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

                //List<HtmlNode> trlist = doc.DocumentNode.SelectSingleNode("//table[@id='documentsTable']").Descendants("tr").Skip(1).ToList();// skip one for the header 
                //List<HtmlNode> trlist = doc.DocumentNode.SelectSingleNode("//table").Descendants("tr").Skip(1).ToList();// skip one for the header 
                List<HtmlNode> trlist = doc.DocumentNode.SelectNodes("//table").Descendants("tr").ToList();// skip one for the header 
                int TotalCount = trlist.Count;
                int Processing = 0;
                foreach (var node in trlist)
                {
                    List<HtmlNode> tdList = node.Descendants("td").ToList();


                    if (tdList.Count > 1)
                    {

                        // string resourceType = tdList[1].InnerText;
                        //var links = tdList[3].Descendants("a").ToList();
                        //if (resourceType == "Reference")
                        //{
                        Processing++;
                        var links = tdList[0].Descendants("a").ToList();
                        string fileName = tdList[0].InnerText.Replace("&nbsp;", "");
                        Console.WriteLine("Processing:" + fileName + "--> " + Processing.ToString() + " of " + TotalCount.ToString());
                        fileName = Util.Utility.RemoveIlligalChar(fileName);
                        //string folderName = tdList[3].InnerText;
                        if (links != null)
                        {
                            var href = links[0].GetAttributeValue("href", string.Empty);
                            href = href.Replace("../", ANDROID_BASE_URL);

                            string fileContent = Util.Utility.extractDoc(href);
                            string folderName = exepath + RAW_FOLDER_PATH + "Android" + "\\ ";
                            fileName = folderName + "\\" + fileName + ".xml";

                            if (!Directory.Exists(folderName))
                            {
                                Directory.CreateDirectory(folderName);
                            }
                            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
                            file.WriteLine(fileContent);
                            file.Close();
                            // }
                        }
                    }
                }
            }
        }

        public static void CreateYML()
        {
            string exepath = Utility.GetExepath();
            string folderName = exepath + RAW_FOLDER_PATH + "android";
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
            HtmlNode nodeClassname = doc.DocumentNode.SelectSingleNode("//h1[@class='api-title']");
            if (nodeClassname != null)
            {
                ApiDoc.ClassName = nodeClassname.InnerText;
                Console.WriteLine("Processed file '{0}: {1}'.", iCount, ApiDoc.ClassName);
            }
            string criteria1 = "//div[@class='api apilevel-1']";
            string criteria2 = "//div[@class='api apilevel-11']";
            List<HtmlNode> plist = null;
            if (doc.DocumentNode.SelectSingleNode(criteria1) != null && doc.DocumentNode.SelectSingleNode(criteria1).Descendants("p") != null)
            {
                plist = doc.DocumentNode.SelectSingleNode(criteria1).Descendants("p").ToList();
            }
            else if (doc.DocumentNode.SelectSingleNode(criteria2) != null && doc.DocumentNode.SelectSingleNode(criteria2).Descendants("p") != null)
            {
                plist = doc.DocumentNode.SelectSingleNode(criteria2).Descendants("p").ToList();
            }

                if (plist != null && plist.Count > 1)
                {
                    ApiDoc.ClassDesc = plist[1].InnerText;
                    if (ApiDoc.ClassDesc == "" && plist.Count > 4)// Some Document type class Desc are found in p no 4. No class name associate so rely on Index
                    {
                        ApiDoc.ClassDesc = plist[4].InnerText;
                    }
                    
                }
                iCount++;
                ApiDoc.Methods = getMethods(doc);

            SaveYml(ApiDoc);
        }
        private static void SaveYml(APIDocument ApiDoc)
        {
            Serializer s = new Serializer();
            string exepath = Utility.GetExepath();
            string folderName = exepath + YML_FOLDER_PATH + "android" + "\\ ";
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
         private static List<APIDocumentMethod> getMethods(HtmlDocument doc)
        {
              List<APIDocumentMethod> objList = new List<APIDocumentMethod>();
           //string criteria = "//table[@id='inhattrs'] | //table[@id='inhconstants'] | //table[@id='inhfields'] | //table[@id='pubctors'] | //table[@id='pubmethods']  | //table[@id='inhmethods']";
              //TODO: Some method will not have parameter , like XML attrubute , but some will habe like method and constrauctor, in getMethods1 pass a parameter to extract parameter
             getMethods(doc,"inhattrs", objList,MethodType.INHERIT_XML);
             getMethods(doc,"inhconstants", objList,MethodType.INHERIT_CONSTANT);
             getMethods(doc,"inhfields", objList,MethodType.INHERIT_FIELD);
             getMethods(doc,"pubctors", objList,MethodType.PUBLIC_CONSTRAUCTOR);
             getMethods(doc,"pubmethods", objList,MethodType.PUBLIC_METHOD);
             getMethods(doc,"inhmethods", objList,MethodType.INHERIT_METHOD);
             return objList;
        }
         private static List<APIDocumentMethod> getMethods(HtmlDocument doc, string tableId, List<APIDocumentMethod> objList, string methodType)
         {

             string criteria = "//table[@id='" + tableId + "']";
             if (doc.DocumentNode.SelectNodes(criteria) != null)
             {
                 HtmlNode tableSection = doc.DocumentNode.SelectSingleNode(criteria);

                 if (tableSection != null)
                 {
                     HtmlAgilityPack.HtmlDocument objTableDoc = new HtmlAgilityPack.HtmlDocument();
                     objTableDoc.LoadHtml(tableSection.InnerHtml);
                     //<table class="jd-sumtable-expando">
                     string criteria2 = "//table[@class='jd-sumtable-expando'] | //table[@class='jd-sumtable-expando responsive'] "; // there may be mul;tiple table of this
                     if (objTableDoc.DocumentNode.SelectNodes(criteria2) != null && objTableDoc.DocumentNode.SelectNodes(criteria2).Descendants("tr").ToList() != null)
                     {
                         List<HtmlNode> trList = objTableDoc.DocumentNode.SelectNodes(criteria2).Descendants("tr").ToList();
                         foreach (HtmlNode tr in trList)
                         {
                             APIDocumentMethod method = new APIDocumentMethod();
                             method.MethodType = methodType;
                             List<HtmlNode> tdList =tr.Descendants("td").ToList();
                             if (tdList != null && tdList.Count > 1)
                             {
                                 if (methodType == MethodType.INHERIT_METHOD || methodType == MethodType.PUBLIC_METHOD)
                                 {
                                     int methodStartIndex ;
                                     string methodName =tdList[1].InnerText ;
                                     methodStartIndex = methodName.IndexOf("(");
                                     methodName = Utility.RemoveIlligalChar(methodName.Substring(0, methodStartIndex));
                                     method.MethodName = Utility.RemoveSpace(methodName);
                                     
                                 }
                                 else
                                 {
                                     method.MethodName = Utility.RemoveHTMLComments(tdList[0].InnerText);
                                 }
                                 string md = tdList[1].InnerText.Replace("\n", "");
                                 md = Utility.RemoveIlligalChar(md);
                                 method.MethodDesc = Utility.RemoveSpace( md);
                                 method.Parameters = Utility.getParametersFromMethodString(md);
                                 
                                
                                 objList.Add(method);
                             }

                         }
                     }
                 }

             }
             return objList;
         }

        
        private static void getMethodName(HtmlNode n, APIDocumentMethod method)
        {
            List<APIDocumentMethod> objList = new List<APIDocumentMethod>();
            //HtmlNode objNode = n.SelectSingleNode("//a[@class='x-api-property-task-list instance-method Objective-C']");
            List<HtmlNode> objNode = n.Descendants("a").ToList();//[@class='x-api-property-task-list instance-method Objective-C']");
            foreach (HtmlNode a in objNode)
            {
                if (a.Attributes["class"] != null
                    && (a.Attributes["class"].Value == "x-api-property-task-list instance-method Objective-C"
                    || a.Attributes["class"].Value == "x-api-property-task-list function Objective-C"))
                    method.MethodName = Utility.RemoveHTMLComments(a.InnerText);
            }
            //List<HtmlNode> objNodelist = doc.DocumentNode.SelectNodes("//section[@class='section method']").ToList();
            //if (objNode != null && objNode.Count>3)
            //{
            //    method.MethodName = Utility.RemoveHTMLComments(objNode[2].InnerText);
            //}
            //else
            //{
            //    method.MethodName = Utility.RemoveHTMLComments(objNode[2].InnerText);
            //}

        }

    }
}