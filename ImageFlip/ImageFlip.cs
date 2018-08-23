
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageFlip
{
    public static class ImageFlip
    {
        [FunctionName("ImageFlip")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            string flipDirection = req.Query["direction"].ToString().ToLower();

            if (flipDirection == "horizontal" || flipDirection == "vertical")
            {
                try
                {
                    using (Image<Rgba32> img = Image.Load(new StreamReader(req.Body).BaseStream))
                    {
                        var flipmode = flipDirection == "horizontal" ? FlipMode.Horizontal : FlipMode.Vertical;

                        using (var img2 = img.Clone(ctx => ctx.Flip(flipmode)))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                memoryStream.Position = 0;
                                img2.SaveAsPng(memoryStream);
                                var b = new FileContentResult(memoryStream.ToArray(), "image/png");
                                //var result = new OkObjectResult(System.Convert.ToBase64String(memoryStream.ToArray()));

                                return (ActionResult)b;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                    return (ActionResult)new BadRequestObjectResult("{\"error\" : \"Error processing image\"}");
                }
            }
            else
            {
                return (ActionResult)new BadRequestObjectResult(new ErrorData("Invalid direction specified. 'direction' param must be vertical or horizontal.").ToJson);
            }
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