using PdfSharps.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using Wrappers;
using PdfSharp.Drawing;
using PdfSharps.SystemIO;

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
            PdfIOs.Options = Options;
        }

        public void WriteFileNames()
        {
            StringBuilder _names = new StringBuilder();
            foreach (var file in PdfIOs.Files())
            {                
                _names.Append( Options.OutputText(file.Name));
            }

            if (!_names.Empty())
                WrapIOs.Create(Options.OutputFilePath(false), _names.ToString());            
        }

        public void AddSamplePage()
        {
            //Help: http://csharp.net-informations.com/file/txttopdf.htm

            var _path = Options.CalculatedtFilePath();

            using (SharpDoc pdf = SharpReader.Open(_path, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify))
            {
                var page = pdf.AddPage();

                XGraphics graph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                graph.DrawString("-- SAMPLE --", font, XBrushes.Black, new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                pdf.Save(_path);
            }
        }

        /// <summary>
        /// Merge all files and produce an output file.
        /// </summary>
        public void Merge()
        {
            using (SharpDoc outPdf = new SharpDoc())
            {
                foreach(var file in PdfIOs.Files())
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

        /// <summary>
        /// Removes Pages from the only file specially specified.
        /// </summary>
        public void RemovePages()
        {
            if (Options.File.Empty()) return;
            try
            {
               RemovePages(Options.CalculatedtFilePath());
            }
            catch { }
        }

        /// <summary>
        /// Removes ranges from all files selected in a folder or subfolders
        /// </summary>
        public void RemovePagesSelection(bool singleFileToo = false)
        {
            if (singleFileToo) RemovePages();

            foreach (var file in PdfIOs.Files())
            {
                try
                {
                    RemovePages(file.FullName);
                }
                catch { }
            }
        }

        public void Divide()
        {
            if ( ( Options.DivisionPageSize <= 0 && !Options.DivisionOnRanges ) || Options.File.Empty()) return;

            try
            {
                Divide(Options.CalculatedtFilePath());
            }
            catch { }
        }

        public void DivideSelection(bool singleFileToo = false)
        {
            if (singleFileToo) Divide();

            foreach (var file in PdfIOs.Files())
            {
                try
                {
                    Divide(file.FullName);
                }
                catch { }
            }
        }

        /// <summary>
        /// Deletes PDF files from folders or all Sub folders.
        /// </summary>
        public void DeleteFiles()
        {
            foreach (var file in PdfIOs.Files())
            {
                WrapIOs.Delete(file.FullName);
            }
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

        private void RemovePages(string filePath)
        {
            using (SharpDoc one = SharpReader.Open(filePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify))
            {
                Ranges(one).ForEach(p => { one.Pages.Remove(p); });

                one.Save(filePath);
            }
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

        private void Divide(string filePath)
        {
            using (SharpDoc pdf = SharpReader.Open(filePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import))
            {
                if (Options.DivisionOnRanges)
                {
                    var _range = Ranges(pdf);
                    var _pages = pdf.Pages;
                    _pages.Cast<PdfSharp.Pdf.PdfPage>().ToList().ForEach(p => { if ( !_range.Any(r => r == p) ) pdf.Pages.Remove(p); });                
                }

                Divide(filePath, pdf.Pages);
            }
        }

        private void Divide(string filePath, PdfSharp.Pdf.PdfPages pages)
        {
            var _pCount = pages.Count;
            var _pageSize = Options.DivisionPageSize;
            if (Options.DivisionOnRanges && Options.DivisionPageSize <= 0) _pageSize = _pCount;
            if (_pCount < _pageSize) return;

            Int32 index = 1;
            Int32 pageNo = 0;
            bool pageExists = true;
            do
            {
                using (SharpDoc outPdf = new SharpDoc())
                {
                    for (var iPage = 1; iPage <= _pageSize; iPage++)
                    {
                        if (pageNo > (_pCount - 1)) { pageExists = false; break; }
                        outPdf.Pages.Add(pages[pageNo++]);
                    }

                    if (outPdf.Pages.Count > 0)
                    {
                        var _path = PdfIOs.AppendToFileName(filePath, postfix: $"_{index++}");
                        outPdf.Save(_path);
                    }
                }

            } while (pageExists);
        }
    }
}
