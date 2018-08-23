
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace ImageRotate
{
    public static class ImageRotate
    {
        [FunctionName("ImageRotate")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string rotationAmount = req.Query["degrees"];
            float degrees = 0.0F;

            if (float.TryParse(rotationAmount, out degrees) && degrees >= 0 && degrees <= 360)
            {
                try
                {
                    using (Image<Rgba32> img = Image.Load(new StreamReader(req.Body).BaseStream))
                    {

                        using (var img2 = img.Clone(ctx => ctx.Rotate(degrees)))
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
                return (ActionResult)new BadRequestObjectResult(new ErrorData("Invalid rotation degrees param.").ToJson);
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
