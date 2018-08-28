using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ServerlessPrj
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            [DocumentDB("serverlessdb", "serverlesscol", ConnectionStringSetting = "MyAccount_COSMOSDB", CreateIfNotExists = true)] DocumentClient client,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string userid = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "userId", true) == 0)
                .Value;

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("serverlessdb", "serverlesscol");

            IDocumentQuery<DocItem> query = client.CreateDocumentQuery<DocItem>(collectionUri)
                .Where(p =>p.userId.Equals(userid))
                .AsDocumentQuery();

            List<DocItem> outputDocument = new List<DocItem>();
            while (query.HasMoreResults)
            {
                foreach (DocItem result in await query.ExecuteNextAsync())
                {
                    outputDocument.Add(result);
                }
            }

            if (outputDocument.Count()==0)
                return req.CreateResponse(HttpStatusCode.NotFound, "No objects found");

            return req.CreateResponse(HttpStatusCode.OK, outputDocument);
        }
    }
}
