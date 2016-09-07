using DocAnalysisNet.Entity;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocAnalysisNet.Util
{
    public class Utility
    {
        public static string RemoveIlligalChar(string fileName)
        {
            //string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            fileName = r.Replace(fileName, "");

            return fileName;
        }
        public  static string RemoveSpace(string md)
        {
            md = md.Replace(Environment.NewLine, "");
            md = md.Replace("  ", "");
            return md;
        }

        public static string extractDoc(string Url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string result = webClient.DownloadString(Url);
            return result;
        }
        //public static string RemovedComment(string s)
        //{
        //    int CommentStart = s.IndexOf("<!--");
        //    int CommentEnd = s.IndexOf("-->");
        //   string result = webClient.DownloadString(Url);
        //    return result;
        //}
        public static string RemoveHTMLComments(string input)
        {
            string output = string.Empty;
            string[] temp = System.Text.RegularExpressions.Regex.Split(input, "<!--");
            foreach (string s in temp)
            {
                string str = string.Empty;
                if (!s.Contains("-->"))
                {
                    str = s;
                }
                else
                {
                    str = s.Substring(s.IndexOf("-->") + 3);
                }
                if (str.Trim() != string.Empty)
                {
                    output = output + str.Trim();
                }
            }
            return output;
        }
        public static HtmlDocument LoadHtmlDoc(string path)
        {
            String FileContent = string.Empty;

            if (File.Exists(path))
            {
                FileContent = File.ReadAllText(path);
            }

            if (FileContent != null && FileContent.Length > 0)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(FileContent);
                return doc;
            }
            else
            {
                return null;
            }
        }
        public static string GetExepath()
        {
            string exepath = AppDomain.CurrentDomain.BaseDirectory;
            exepath = exepath.Replace("\\bin", "");
            exepath = exepath.Replace("\\Debug", "");
            return exepath;
        }
        public static List<APIDocumentParameter> getParametersFromMethodString_XXX(string methodDesc)
        {
            int methodStartIndex, methodEndIndex;
            methodStartIndex = methodDesc.IndexOf("(");
            methodEndIndex = methodDesc.IndexOf(")");
            List<APIDocumentParameter> objList = new List<APIDocumentParameter>();
            if (methodStartIndex > -1 && methodEndIndex > -1)
            {
                string parameterArea = methodDesc.Substring(methodStartIndex + 1, methodEndIndex - methodStartIndex - 1);
                if (parameterArea.Length > 3)// Any paramenter and data type will be atleast 3 char 
                {

                    Char delimiter = ',';
                    String[] paramNamevalue = parameterArea.Split(delimiter);
                    foreach (var OneParam in paramNamevalue)
                    {
                        Char paramDelimiter = ' ';
                        String[] OneParamArray = OneParam.Trim().Split(paramDelimiter);
                        if (OneParamArray != null && OneParamArray.Length > 1)
                        {
                            APIDocumentParameter p = new APIDocumentParameter();
                            p.DataType = Utility.RemoveHTMLComments(OneParamArray[0]);
                            p.ParameterName = Utility.RemoveHTMLComments(OneParamArray[1]);
                            objList.Add(p);
                        }
                    }
                }
            }
            return objList;
        }
        public static List<APIDocumentParameter> getParametersFromMethodString(string methodDesc)
        {
            int methodStartIndex, methodEndIndex;
            methodStartIndex = methodDesc.IndexOf("(");
            methodEndIndex = methodDesc.IndexOf(")");
            List<APIDocumentParameter> objList = new List<APIDocumentParameter>();
            if (methodStartIndex > -1 && methodEndIndex > -1)
            {
                string parameterArea = methodDesc.Substring(methodStartIndex + 1, methodEndIndex - methodStartIndex - 1);
                if (parameterArea.Length > 3)// Any paramenter and data type will be atleast 3 char 
                {

                    Char delimiter = ',';
                    String[] paramNamevalue = parameterArea.Split(delimiter);
                    foreach (var OneParam in paramNamevalue)
                    {
                        Char paramDelimiter = ' ';
                        String[] OneParamArray = OneParam.Trim().Split(paramDelimiter);
                        if (OneParamArray != null && OneParamArray.Length ==2)
                        {
                            APIDocumentParameter p = new APIDocumentParameter();
                            p.DataType = Utility.RemoveHTMLComments(OneParamArray[0]);
                            p.ParameterName = Utility.RemoveHTMLComments(OneParamArray[1]);
                            objList.Add(p);
                        }
                        else if (OneParamArray != null && OneParamArray.Length == 3)
                        {
                            APIDocumentParameter p = new APIDocumentParameter();
                            p.DataType = Utility.RemoveHTMLComments(OneParamArray[1]);
                            p.ParameterName = Utility.RemoveHTMLComments(OneParamArray[2]);
                            objList.Add(p);
                        }

                    }
                }
            }
            return objList;
        }
    }
}
