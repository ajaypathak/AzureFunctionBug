using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Func01
{
    public static class QueueTriggerFunction
    {
        [FunctionName("QueueTriggerFunction")]
        public static void Run(
            [QueueTrigger("orderQueue", Connection = "AzureWebJobsStorage")]
            Order order,
            [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {order}");
            outputBlob.WriteLine($"OrderId: {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {DateTime.UtcNow}");
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(
                Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");
        }
    }
}