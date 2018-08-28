using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ServerlessPrj
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            [DocumentDB("serverlessdb", "serverlesscol", ConnectionStringSetting = "MyAccount_COSMOSDB", CreateIfNotExists = true,Id ="{Query.RatingId}")] object outputDocument,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            if (outputDocument==null)
                return req.CreateResponse(HttpStatusCode.NotFound, "id is not found");

            return req.CreateResponse(HttpStatusCode.OK, outputDocument);
        }
    }
}
