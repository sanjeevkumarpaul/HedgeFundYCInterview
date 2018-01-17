using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers
{
    using Extensions;

    public static partial class WrapIOs
    {

        public static string CurrentFolder
        {
            get
            {
                return AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            }
        }

    }


    public static partial class WrapIOs
    {

        public static string AbsolutePath(string relativepath)
        {
            return Path.Combine(CurrentFolder, relativepath);
        }

        public static FileInfo InfoFromRelativePath(string relativepath)
        {
            return new FileInfo(AbsolutePath(relativepath));
        }

        public static List<T> ReadTabDelimited<T>(string filePath) where T : class, new()
        {
            string content = null;
            List<T> rec = new List<T>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    content = reader.ReadToEnd();
                }

                if (!content.Empty())
                {
                    var properties = (new T()).PropertyNames();

                    foreach (string record in content.SplitEx(Environment.NewLine))
                        rec.Add(CreateObjectFromTabDelimitedRow(record, new T(), properties));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return rec;
        }

        public static T CreateObjectFromTabDelimitedRow<T>(string rowText, T obj, List<string> properties) where T : class
        {
            string[] info = rowText.SplitEx('\t', false);

            if (info.Length > 0)
            {
                int index = 0;
                foreach (var prop in properties)
                {
                    if (!info[index].Empty())
                    {
                        try { obj.SetProperty(prop, info[index]); }
                        catch { }
                    }
                    index++;
                }
            }

            return obj;
        }

        public static string[] ReadAllLines(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(@filePath);

            return lines;
        }


        public static void WriteRecords(string[] records, string filePath)
        {
            using (StreamWriter writer = File.CreateText(filePath))
            {
                foreach (var record in records)
                {
                    writer.WriteLine(record);
                }
            }
        }

        public static void CreateIfNotexists(string filePath, string header)
        {
            if (File.Exists(filePath)) return;

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(header);
            }
        }

        public static void AppendRecords(string[] records, string filePath)
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                foreach (var record in records)
                {
                    writer.WriteLine(record);
                }
            }
        }


        public static void Delete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
                //Nolog
            }

        }

    }
}
