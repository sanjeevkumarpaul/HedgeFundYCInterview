using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Downloads.Dependency;

namespace DownloadHandler
{
    /// <summary>
    /// USAGE
    ///public ImageResult GetImage(int imageId)
    ///{
    ///    // Database fetch of image details
    ///    var imageInfo = repository.Get<ImageInfo>(imageId);
    ///    return new ImageResult(imageInfo.FullFilename);
    ///}
    /// </summary>

    public class ImageResult : ActionResult
    {
        public string SourceFilename { get; set; }
        public MemoryStream SourceStream { get; set; }
        public string ContentType { get; set; }
        public ImageResult(string sourceFilename)
        {
            SourceFilename = sourceFilename;
            ContentType = FileTypeHelper.GetContentType(SourceFilename);
        }
        public ImageResult(MemoryStream sourceStream, string contentType)
        {
            SourceStream = sourceStream;
            ContentType = contentType;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var res = context.HttpContext.Response;
            res.Clear();
            res.Cache.SetCacheability(HttpCacheability.NoCache);
            res.ContentType = ContentType;

            if (SourceStream != null)
            {
                SourceStream.WriteTo(res.OutputStream);

            }
            else
            {
                res.TransmitFile(SourceFilename);
            }

        }
    }
}
