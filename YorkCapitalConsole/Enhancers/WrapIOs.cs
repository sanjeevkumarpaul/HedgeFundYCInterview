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
        public static bool Exists(string filepath)
        {
            return File.Exists(filepath) || Directory.Exists(filepath);
        }

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

        public static void Create(string filepath, string text)
        {
            using (var file = File.CreateText( filepath ))
            {
                file.WriteLine(text);
                file.Close();
            }
        }

        public static string[] FindFiles(string path, bool includeSubfolders = false, string pattern="*.*")
        {
            if (System.IO.Directory.Exists(path))
                return System.IO.Directory.GetFiles(path, pattern, includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (System.IO.File.Exists(path))
                return new string[] { path };

            return null;
        }

        public static void UnZip(string path, string unzipPath =  "", bool deleteAfterExtraction = false)
        {
            try
            {
                if (unzipPath.Empty()) unzipPath = $@"{System.IO.Path.GetDirectoryName(path)}\{System.IO.Path.GetFileNameWithoutExtension(path)}";

                System.IO.Compression.ZipFile.ExtractToDirectory(path, unzipPath);
                if (deleteAfterExtraction) Delete(path);
            }
            catch { }

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

        public static bool Rename(string path, string searchPhrase, string replacePhrase)
        {
            if (!(searchPhrase == replacePhrase || searchPhrase.Empty()))
            {
                var _path = System.IO.Path.GetDirectoryName(path);
                var _file = System.IO.Path.GetFileName(path);

                if (_file.Contains(searchPhrase))
                {
                    var _newname = $"{_path}\\{_file.Replace(searchPhrase, replacePhrase.ToEmpty())}";

                    Rename(path, _newname);

                    return true;
                }
            }
            return false;
        }

        public static void Rename(string oldfile, string newfile)
        {
            if (oldfile == newfile) return; //Either complete path or just the filename is same.

            try { File.Delete(newfile); } catch { }
            try { File.Move(oldfile, newfile); } catch { }
        }

    }
}
