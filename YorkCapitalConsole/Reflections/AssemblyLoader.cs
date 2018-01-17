using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;

namespace Reflections
{
    public sealed class AssemblyLoader
    {
        public static string Current()
        {
            string codeBase = Assembly.GetEntryAssembly().CodeBase;

            codeBase = codeBase.Substring(codeBase.LastIndexOf("/") + 1);
            codeBase = codeBase.Substring(0, codeBase.LastIndexOf("."));

            return codeBase;
        }

        public static string AssemblyDirectory()
        {
            string codeBase = Assembly.GetEntryAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string ConstructNamespace(string argNamespace, string argTypeName)
        {
            return string.Format("{0}.{1}", argNamespace, argTypeName);
        }

        public static object Load(Type T, string argFullyQualifiedTypeName) //with Namespace attached.
        {
            return Assembly.GetAssembly(T)
                            .CreateInstance(argFullyQualifiedTypeName,
                                            false,
                                            BindingFlags.CreateInstance, null, null, null, null);
        }


        public static object Instance(string argAssemblyName, string argFullyQualifiedTypeName)
        {
            ObjectHandle handle = Activator.CreateInstance(argAssemblyName, argFullyQualifiedTypeName);

            return handle.Unwrap();
        }

        public static object Instance(string argFullyQualifiedTypeName)
        {
            return Instance(Current(), argFullyQualifiedTypeName);
        }

        public static Type Type(string argAssemblyName, string argFullyQualifiedTypeName)
        {
            return Instance(argAssemblyName, argFullyQualifiedTypeName).GetType();
        }

        public static Type Type(string argFullyQualifiedTypeName)
        {
            return Instance(argFullyQualifiedTypeName).GetType();
        }


    }
}