using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

using Downloads.Dependency;

namespace Downloads
{
    public sealed class AspDownload : WebContext
    {
        public static void ControlContent()
        {
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    //GridView1.RenderControl(hw);
                    StringReader sr = new StringReader(sw.ToString());

                    //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                    //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    //pdfDoc.Open();
                    //XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    //pdfDoc.Close();                    
                }
            }
        }
    }


}
