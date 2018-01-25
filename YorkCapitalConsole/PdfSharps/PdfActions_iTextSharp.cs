using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extensions;
using Wrappers;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using PdfSharps.SystemIO;

namespace PdfSharps
{
    partial class PdfAction
    {
        public void Compress()
        {
            if (Options.File.Empty()) return;
            try
            {
                CompressFile(Options.CalculatedtFilePath());
            }
            catch { }
        }

        public void CompressSelection(bool singleFileToo = false)
        {
            if (singleFileToo) Compress();

            foreach (var file in PdfIOs.Files())
            {
                try
                {
                    CompressFile(file.FullName);
                }
                catch { }
            }
        }

        public void ConvertFromHtml()
        {
            //TODO: may be will be shifting to web section within pdfUtility
        }
    }

    partial class PdfAction
    {
        public void CompressFile(string filepath)
        {
            var _path = PdfIOs.AppendToFileName(filepath, postfix:"_Compressed"); 

            using (iTextSharp.text.Document doc = new iTextSharp.text.Document()) 
            {
                using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(filepath))
                {                    
                    PdfStamper stamper = new PdfStamper(reader, new FileStream(_path, FileMode.Create), PdfWriter.VERSION_1_5);

                    stamper.FormFlattening = true;
                    stamper.SetFullCompression();

                    stamper.Close();
                    reader.Close();
                }
            }
            //check if same file is to be saved.            
            if (!Options.CompressToCopy)  WrapIOs.Rename(_path, filepath);
        }
    }
}
