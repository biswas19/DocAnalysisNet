using DocAnalysisNet.Entity;
using DocAnalysisNet.Util;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocAnalysisNet
{
    class TestArea
    {
        static string RAW_FOLDER_PATH = ConfigurationManager.AppSettings["RAW_FOLDER_PATH"];
        static string YML_FOLDER_PATH = ConfigurationManager.AppSettings["YML_FOLDER_PATH"];
        static StringBuilder AnroidRasFiles = new StringBuilder();
        static StringBuilder AnroidYmlFiles = new StringBuilder();
        public static void  getAnroidList()
        {
            string exepath = Utility.GetExepath();
            string folderNameAnroidRaw = exepath + RAW_FOLDER_PATH + "android";
            string folderNameAnroidYml = exepath + YML_FOLDER_PATH + "android";
            //if (Directory.Exists(folderNameAnroidRaw))
            //{
            //    ProcessDirectory(folderNameAnroidRaw, AnroidRasFiles);
            //}
            if (Directory.Exists(folderNameAnroidYml))
            {
                ProcessDirectory(folderNameAnroidYml, AnroidYmlFiles);
            }
           // string r = AnroidRasFiles.ToString();
            string y = AnroidYmlFiles.ToString();
        }
        public static void ProcessDirectory(string targetDirectory ,StringBuilder sb)
        {
            //string[] fileEntries = Directory.GetFiles(targetDirectory);
            DirectoryInfo di = new DirectoryInfo(targetDirectory);
            FileInfo[] fi = di.GetFiles();
            foreach (FileInfo f in fi)
                sb.Append(f.Name + Environment.NewLine);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory,sb);
        }
        public static List<APIDocumentParameter> test()
        {
            //string s = "Button(Context context, AttributeSet attrs, int defStyleAttr)";
            //string s = "Button(Context context)";
            //string s = "Button()";
            string s = "Button";
            int methodStartIndex, methodEndIndex;
            methodStartIndex = s.IndexOf("(");
            methodEndIndex = s.IndexOf(")");
            List<APIDocumentParameter> objList = new List<APIDocumentParameter>();
            if (methodStartIndex > -1 && methodEndIndex > -1)
            {
                string parameterArea = s.Substring(methodStartIndex + 1, methodEndIndex - methodStartIndex - 1);
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
                            p.DataType= Utility.RemoveHTMLComments(OneParamArray[0]);
                            p.ParameterName = Utility.RemoveHTMLComments(OneParamArray[1]);
                            objList.Add(p);
                        }
                    }
                }
            }
            return objList;
        }
    }
}
