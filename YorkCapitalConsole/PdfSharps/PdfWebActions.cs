using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfUtility
{
    public sealed partial class PdfWebActions
    {
        //TODO: mostly to understand more of pdfs.

        public void ConvertFromHtml()
        {
            //TODO: may be will be shifting to web section within pdfUtility
        }

        public object CreateStreamResponse(TextWriter writer, Stream outputStream)
        {
            using (StringReader sr = new StringReader(writer.ToString()))
            {
                using (Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f))
                {
                    using (PdfWriter pdfwriter = PdfWriter.GetInstance(pdfDoc, outputStream))
                    {
                        pdfDoc.Open();
                        XMLWorkerHelper.GetInstance().ParseXHtml(pdfwriter, pdfDoc, sr);
                        pdfDoc.Close();
                    }
                    return pdfDoc;
                }
            }
        }
    }
}
