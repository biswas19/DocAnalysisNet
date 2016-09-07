using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocAnalysisNet.Entity
{

    public class APIDocument
    {
   
        public string ClassName { get; set; }
        public string ClassDesc { get; set; }
        public List<APIDocumentMethod> Methods { get; set; }
        public string ClassType { get; set; }
        public List<String> AdoptedBy { get; set; }
        public List<APIDeclaration> Declarations { get; set; }

    }
    public class APIDeclaration
    {
        public string DeclarationDesc { get; set; }
        public List<APIDocumentParameter> Parameters { get; set; }
    }

    public class APIDocumentMethod
    {
        public string MethodName { get; set; }
        public string MethodDesc { get; set; }
        public string Returns { get; set; }
        public List<APIDocumentParameter> Parameters { get; set; }
        public string SwiftDeclaration { get; set; }
        public string ObjCDeclaration { get; set; }
        public string MethodType { get; set; }
        public string InheritFrom { get; set; }
    }
    public class APIDocumentParameter
    {
        public string ParameterName { get; set; }
        public string DataType { get; set; }
        public string ParameterDesc { get; set; }
    }
    public static class MethodType
    {
        public static string INHERIT_XML = "INHERIT_XML";
        public static string INHERIT_CONSTANT = "INHERIT_CONSTANT";
        public static string INHERIT_FIELD = "INHERIT_FIELD";
        public static string PUBLIC_CONSTRAUCTOR = "PUBLIC_CONSTRAUCTOR";
        public static string PUBLIC_METHOD = "PUBLIC_METHOD";
        public static string INHERIT_METHOD = "INHERIT_METHOD";


    }
}
