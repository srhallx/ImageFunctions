
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using System;

namespace ImageInfo
{
    public static class ImageInfo
    {
        [FunctionName("ImageInfo")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                using (var img = Image.Load(new StreamReader(req.Body).BaseStream))
                {
                    var info = new ImageInfoData(img.Width, img.Height);
                    return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(info));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return (ActionResult)new BadRequestObjectResult(new ErrorData("Error processing image").ToJson);
            }
        }
    }

    public struct ImageInfoData
    {
        public int Width { get; }
        public int Height { get; }

        internal ImageInfoData(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    public struct ErrorData
    {
        public string ErrMsg { get; }
        public string ToJson { get { return JsonConvert.SerializeObject(ErrMsg); } }

        internal ErrorData(string errMsg)
        {
            ErrMsg = errMsg;
        }
    }
}
