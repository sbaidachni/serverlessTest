using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ServerlessPrj
{
    public static class GetProductDetails
    {
        [FunctionName("GetProductDetails")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string productid = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "productid", true) == 0)
                .Value;

            return productid == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a productid on the query string")
                : req.CreateResponse(HttpStatusCode.OK, string.Format($"The product name for your product id {productid} is Grape Ice Cream"));
        }
    }
}
