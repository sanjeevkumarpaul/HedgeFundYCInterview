using PdfSharps.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using Wrappers;

//PDF Sharp Code Base
//https://www.csharpcodi.com/vs2/4577/NClass/lib/PdfSharp/PdfSharp/PdfSharp.Pdf/PdfPages.cs/

namespace PdfSharps
{

    using SharpDoc = PdfSharp.Pdf.PdfDocument;
    using SharpReader = PdfSharp.Pdf.IO.PdfReader;

    public sealed partial class PdfAction : IDisposable
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
            foreach (var file in Files())
            {                
                _names.Append( Options.OutputText(file.Name));
            }

            if (!_names.Empty())
                WrapIOs.Create(Options.OutputFilePath(false), _names.ToString());            
        }

        public void Merge()
        {
            using (SharpDoc outPdf = new SharpDoc())
            {
                foreach(var file in Files())
                {
                    try
                    {
                        using (SharpDoc one = SharpReader.Open(file.FullName, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)) { CopyPages(one, outPdf); }
                    }
                    catch { }
                }
                outPdf.Save(Options.OutputFilePath());
            }
        }

        public void RemovePages()
        {
            if (Options.File.Empty()) return;
            try
            {
                var _path = Options.CalculatedtFilePath();

                using (SharpDoc one = SharpReader.Open(_path, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify))
                {
                    Ranges(one).ForEach(p => { one.Pages.Remove(p); });

                    one.Save(_path);
                }
            }
            catch { }
        }

        /// <summary>
        /// Deletes PDF files from folders or all Sub folders.
        /// </summary>
        public void DeleteFiles()
        {
            foreach (var file in Files())
            {
                WrapIOs.Delete(file.FullName);
            }
        }

        public void Compress()
        {
            //TODO: PdfSharp is yet to do much with compression.
        }

        public void Dispose()
        {

        }

    }

    partial class PdfAction
    {
        private void CopyPages(SharpDoc from, SharpDoc to )
        {
            for(int i =0; i< from.PageCount; i++) to.AddPage(from.Pages[i]);            
        }

        private IList<FileInfo> Files()
        {
            var _files = new System.IO.DirectoryInfo(Options.Folder)
                             .GetFiles("*.pdf", Options.Subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                             .ToList();

            if (Options.ExcludeFileNames != null)
                Options.ExcludeFileNames.ToList().ForEach(f => 
                {
                    _files.RemoveAll(z => Path.GetFileName(z.FullName).Equals(f, StringComparison.CurrentCultureIgnoreCase) ||
                                          z.FullName.Equals(f, StringComparison.CurrentCultureIgnoreCase));
                });

            if (!Options.ExcludePattern.Empty())
                new System.IO.DirectoryInfo(Options.Folder)
                             .GetFiles(Options.ExcludePattern, Options.Subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                             .ToList().ForEach(f => 
                             {
                                 _files.RemoveAll(z => z.FullName.Equals(f.FullName, StringComparison.CurrentCultureIgnoreCase));
                             });

            return _files;
        }

        private List<PdfSharp.Pdf.PdfPage> Ranges(SharpDoc doc )
        {
            List<PdfSharp.Pdf.PdfPage> pages = new List<PdfSharp.Pdf.PdfPage>();

            foreach (var range in Options.Ranges.Where( r => r.Start > 0)  )
            {
                if (range.End > 0)
                {
                    for (int i = range.Start; i <= range.End; i++) pages.Add(doc.Pages[i - 1]);                    
                }
                else pages.Add(doc.Pages[range.Start - 1]);
            }

            return pages;
        }
    }
}
