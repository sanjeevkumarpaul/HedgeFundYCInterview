using System.IO;
using System.Web.UI;

using Downloads.Dependency;
using PdfUtility;

namespace Downloads
{
    public sealed class AspDownload : WebContext
    {
        public AspDownload(Downloads.Entities.DownloadOptions options) : base(options) { } //Default copy constructor

        /// <summary>
        /// Renders any control 
        /// If you get an error like wise - 
        /// Server Error in 'ASP.Net' Application.
        ///     RegisterForEventValidation can only be called during Render();
        ///     Description: An unhandled exception occurred during the execution of the current web request.Please review the stack trace for more information about the error and where it originated in the code. 
        ///     Exception Details: System.InvalidOperationException: RegisterForEventValidation can only be called during Render();
        ///Use
        ///   <pages enableEventValidation ="false"></pages> -- In web.config
        ///   OR
        ///   <%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation = "false" -- Inside each page.
        /// </summary>
        /// <param name="uiControl"></param>
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
