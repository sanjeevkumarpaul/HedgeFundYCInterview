using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

using Downloads.Dependency;
using PdfUtility;

namespace Downloads
{
    public sealed class AspDownload : WebContext
    {
        public AspDownload(Downloads.Entities.DownloadOptions options) : base(options) { } //Default copy constructor

        public void ControlContent(Control uiControl)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    uiControl.RenderControl(hw);

                    switch (Options.FileType)
                    {
                        case Entities.FileTypes.PDF: Download(new PdfWebActions().CreateStreamResponse(hw, Response.OutputStream)); break;
                        case Entities.FileTypes.EXCEL:
                        case Entities.FileTypes.CSV: Download(sw.ToString()); break;


                        default: break;
                    }
                }                
            }
        }
    }
}
