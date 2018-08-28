using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ServerlessPrj
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, 
            [DocumentDB("serverlessdb","serverlesscol",ConnectionStringSetting = "MyAccount_COSMOSDB", CreateIfNotExists =true)] IAsyncCollector<Object> outputDocument, 
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();
            string userId = data?.userId;
            string productId = data?.productId;
            string locationName = data?.locationName;
            int? rating = data?.rating;
            string userNotes = data?.userNotes;
            Guid id = Guid.NewGuid();
            DateTime timestamp = DateTime.UtcNow;

            HttpClient client = new HttpClient();
            string result=await client.GetStringAsync($"http://serverlessohproduct.trafficmanager.net/api/GetProduct?productid={productId}");

            if (result.Contains("Please pass a valid productId on the query string"))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "ProductId is incorrect");
            }

            result = await client.GetStringAsync($"http://serverlessohuser.trafficmanager.net/api/GetUser?userid={userId}");

            if (result.Contains("Please pass a valid userId on the query string"))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "UserId is incorrect");
            }

            if ((rating==null)||(locationName==null)||(userNotes==null))
            {
                req.CreateResponse(HttpStatusCode.BadRequest, "Request body is invalid");
            }

            var obj=JsonConvert.SerializeObject(
                new
                {
                    userId,
                    productId,
                    locationName,
                    rating,
                    userNotes,
                    id,
                    timestamp
                });

            await outputDocument.AddAsync(obj);

            return req.CreateResponse(HttpStatusCode.OK, obj);
        }
    }
}
