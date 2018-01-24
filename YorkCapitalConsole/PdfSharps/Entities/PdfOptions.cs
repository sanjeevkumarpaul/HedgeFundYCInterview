using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extensions;
using System.IO;

namespace PdfSharps.Entities
{
    public class PdfOptions
    {
        public string Folder { get; set; }
        public string File { get; set; }  
        public string OutputFile { get; set; }
        public List<PageRange> Ranges { get; set; }

        #region ^Custom
        public bool Subfolders { get; set; }
        public string OutText { get; set; }

        #endregion

        #region ^Methods
        internal string CalculatedtFilePath(bool isOutputFile = false)
        {
            if (!isOutputFile && File.Empty()) return "";

            var _path = Path.GetDirectoryName(isOutputFile ? OutputFile : File ); if (_path.Empty()) _path = Folder.TrimEx(@"\");
            var _file = Path.GetFileName(isOutputFile ? OutputFile : File);

            return $@"{_path}\{_file}";
        }

        internal string OutputFilePath(bool pdfExtension = true)
        {
            if (OutputFile.Empty()) return $@"{Folder}\Output.{ ( pdfExtension ? "pdf" : "txt" ) }";

            return CalculatedtFilePath(true);
        }

        /// <summary>
        /// Here the output to the file names are Generated. Every file name will be put into a new line.
        /// #serial# serial numbers as prefix.
        /// #name# filename
        /// #date# current date only without slashes (/)
        /// #bigdate# current date spelled withoutslashes
        /// #time# current time only without colons (:)
        /// </summary>
        /// <returns></returns>
        internal string OutputText(string filename, int serial = 0)
        {
            if (OutText.Empty()) return $@"{filename}{Environment.NewLine}";

            string _str = OutText;

            string[] _notation = new string[] { "#serial#", "#name#", "#date#", "#bigdate#", "#time#" };

            if (_str.IndexOf("#name#") < 0) _str += $" {filename}";

            foreach(var not in _notation)
            {
                string value = "";
                switch (not)
                {
                    case "#serial#": value= ""; break;
                    case "#name#": value = filename; break;
                    case "#date#": value = DateTime.Today.ToString("MMddyyyy"); break;
                    case "#bigdate#": value = DateTime.Today.ToString("ddddMMMddyyyy"); break;
                    case "#time#": value = DateTime.Now.ToString("hhmmss"); break;
                }

                _str = _str.Replace(not, value);
            }
                       
            
            return _str + Environment.NewLine;
        }
        #endregion
    }
}
