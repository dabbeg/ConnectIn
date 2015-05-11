using System;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ConnectIn.Controllers
{
    public class ImageResult : ActionResult
    {
        public String ContentType { get; set; }
        public byte[] ImageBytes { get; set; }

        public ImageResult(byte[] sourceStream, String contentType)
        {
            ImageBytes = sourceStream;
            ContentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = ContentType;

            var stream = new MemoryStream(ImageBytes);
            stream.WriteTo(response.OutputStream);
            stream.Dispose();
        }
    }
}