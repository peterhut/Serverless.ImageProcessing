using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Serverless.StorageTrigger
{
    public static class ImageUploadedTrigger
    {
        [FunctionName("ProcessImage")]
        public static async Task Run(
            [BlobTrigger("images/{name}", Connection="StorageAccountConnection")] 
            Stream image,

            [Blob("thumbnails/{name}", FileAccess.Write, Connection="StorageAccountConnection")] 
            Stream thumbnailOut,            
            [CosmosDB(
                databaseName: "ImageUpload",
                collectionName: "Analysis",
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<AnalysisResult> analysisOut,
            string name,
            ILogger logger)
        {            
            logger.LogInformation($"Starting resize of {name}");
            using (Image<Rgba32> input = Image.Load<Rgba32>(image, out var format))
            {
                ResizeImage(input, thumbnailOut, format, 400);
            }

            // Reset stream
            image.Position = 0;

            logger.LogInformation($"Starting analysis of {name}");
            var analysis = await AnalyzeImage(image);
            logger.LogInformation("Analysis:" + analysis);
            await analysisOut.AddAsync(new AnalysisResult(name, analysis));
        }

        public static async Task<string> AnalyzeImage(Stream image)
        {
            var computerVisionEndpoint = System.Environment.GetEnvironmentVariable("VisionEndpoint", EnvironmentVariableTarget.Process);
            var computerVisionKey = System.Environment.GetEnvironmentVariable("VisionKey", EnvironmentVariableTarget.Process);

            var uri = computerVisionEndpoint + "vision/v3.0/analyze?visualFeatures=Categories,Description,Color";
           
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", computerVisionKey);

            HttpResponseMessage response;
            using (StreamContent content = new StreamContent(image))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }
            return await response.Content.ReadAsStringAsync();
        }

        public static void ResizeImage(Image<Rgba32> input, Stream output, IImageFormat format, int outputWidth)
        {     
            var divisor = input.Width / outputWidth;
            var height = Convert.ToInt32(Math.Round((decimal)(input.Height / divisor)));

            input.Mutate(x => x.Resize(outputWidth, height));
            input.Save(output, format);
        }

        public class AnalysisResult {
            public AnalysisResult(string name, string analysis) {
                Name = name;
                Analysis = JObject.Parse(analysis);
            }
            public string Name { get; set; }
            public JObject Analysis { get; set; }
        }
    }

}
