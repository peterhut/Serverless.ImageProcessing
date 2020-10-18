using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QRCoder;

namespace CGI.Functions
{
    public static class HttpTriggerFunctions
    {
        [FunctionName("MyFirstFunction")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult("Hello World!");
        }

        
        [FunctionName("GenerateQRCode")]
        public static IActionResult GenerateQRCode(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, 
            ILogger log)
        {
            log.LogInformation("GenerateQRCode recieved a request.");

            var data = (string)req.Query["data"] ?? "https://www.cgi.com";
            
            var generator = new QRCodeGenerator();
            var encoded = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(encoded);
            var image = qrCode.GetGraphic(20);
            return new FileContentResult(image, "image/png");
        }

        
        [FunctionName("CalculateNthPrime")]
        public static IActionResult CalculateNthPrime(
                    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "CalculateNthPrime/{n}")] HttpRequest req,
                    int n,
                    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            long nthPrime = FindPrimeNumber(n);

            return new OkObjectResult(new
            {
                Result = $"Calculated {n}th prime number: {nthPrime}"
            });
        }

        public static long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
    }
}
