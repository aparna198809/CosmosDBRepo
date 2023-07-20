
    using System;
    using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
using Azure.Identity;

namespace Paginationtest
{

    class Program
    {
        //Read configuration
        private static readonly string CosmosDatabaseId = "RetailIngest";
        private static readonly string containerId = "WebsiteMetrics";

        private static Database cosmosDatabase = null;

        private static readonly string uri = "https://testactru.documents.azure.com:443/";
       

        public static async Task Main(string[] args)
        {
            var tokenCredential = new DefaultAzureCredential();
            CosmosClient client = new CosmosClient(uri, tokenCredential);
            Console.WriteLine("Connection established");

            Database database = client.GetDatabase(CosmosDatabaseId);
            Container container = database.GetContainer(containerId);


            await Program.QueryWithContinuationTokens(container);
        }
        private static async Task QueryWithContinuationTokens(Container container)
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");
            string continuation = null;

            List<Items> results = new List<Items>();
            using (FeedIterator<Items> resultSetIterator = container.GetItemQueryIterator<Items>(
                query,
                requestOptions: new QueryRequestOptions()
                { MaxItemCount = 50 })) //1000 documents returned in one iteration 
            {
              while (resultSetIterator.HasMoreResults)
                {
                    FeedResponse<Items> response = await resultSetIterator.ReadNextAsync();
                    results.AddRange(response);
                    foreach(Items res1 in results)
                    {
                        Console.Out.WriteLine($"found Country :{res1.Country},Latitude:{res1.Latitude},Longitude : {res1.Longitude} ");
                    }
                    Console.WriteLine($"the continuation token is {response.ContinuationToken}");
                    

                }
            }
        }

     }


        class Items
        {
            public int CartID { get; set; } 

            public string Action { get; set; }

            public string Item { get; set; }

            public float Price { get; set; }

            public string UserName { get; set; }

            public string Country { get; set; }

            public string EventDate { get; set; }

            public string Year { get; set; }

            public string Latitude { get; set; }

            public string Longitude { get; set; }

            public string Address { get; set; }
    }
}
