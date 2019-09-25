using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Func01
{
    public static class ReceiveOrder
    {
        [FunctionName("ReceiveOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "")]
            HttpRequest req,
            ILogger log
            , [Queue("orderqueue")] IAsyncCollector<Order> asyncCollector, IBinder binder)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var temp = await binder.BindAsync<IAsyncCollector<Order>>(new QueueAttribute("AzureWebJobsStorage1") {Connection = "AzureWebJobsStorage1" });


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<Order>(requestBody);
            await temp.AddAsync(data);

            return new OkObjectResult(data);
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProductId { get; set; }
    }
}