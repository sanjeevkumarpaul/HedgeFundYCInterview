using Downloads.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Downloads.Dependency
{
    public class WebContext
    {
        protected DownloadOptions Options;
        protected string FileName { get; set; }
       
        public WebContext(DownloadOptions options)
        {
            Options = options;
        }

        public HttpResponse Response { get { return HttpContext.Current.Response; } }
        
        public void Download(object content)
        {
            var _file = Options.GetFileName();

            Response.ContentType = FileTypeHelper.GetContentType(_file);  //"application/pdf";
            Response.AddHeader("content-disposition", $"attachment;filename={Options.GetFileName()}");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(content);
            Response.End();
        }

    }
}
