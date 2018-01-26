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
        public void Download(object content)
        {
            var Response = HttpContext.Current.Response;

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(content);
            Response.End();
        }

    }
}
