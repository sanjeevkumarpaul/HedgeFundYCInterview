using PdfSharps.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PdfSharps
{
    public sealed class PdfAction : IDisposable
    {
        private PdfOptions Options;

        public PdfAction(PdfOptions options)
        {
            if (options == null) throw new Exception("Options can not be null");
            Options = options;
        }

        public void WriteFileNames()
        {
            StringBuilder _names = new StringBuilder();
            foreach (FileInfo file in new System.IO.DirectoryInfo(Options.Folder).GetFiles("*.pdf", Options.Subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ))
            {                
                _names.Append( Options.OutputText(file.Name));                
            }

            if (!_names.ToString().Empty())
            {
                using (var file = File.CreateText(Options.OutputFilePath()))
                {
                    file.WriteLine(_names.ToString());
                    file.Close();
                }
            }
        }

        public void Dispose()
        {

        }

    }
}
