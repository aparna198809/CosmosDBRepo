using Microsoft.Azure.Cosmos;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net.WebSockets;
using Azure.Identity;

namespace prefregions
{
    class Program
    {
        public static readonly string uri = "https://testactru.documents.azure.com:443/";       
        public static readonly string databasename = "sampleDB";
        public static readonly string containername = "sampleContainer";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("This is a c# program");
            CosmosClientOptions copt = new CosmosClientOptions();

            //the request is routed to francecentral or Japaneast in this case .if both regions are unavailable the SDK will use the read/write region
            //copt.ApplicationPreferredRegions = new List<string> { Regions.FranceCentral, Regions.JapanEast };

            //if you want SDK to automatically form the preferredregions ,then just configure the priamry region where the appl is hosted.If this region
            //IS unavailable SDK will automatically route the reads to the next region which is close to this region
            copt.ApplicationRegion = Regions.WestUS;

            //connection 
            CosmosClient client = new CosmosClient(uri,new DefaultAzureCredential(),copt);

            var database = client.GetDatabase(databasename);
            var container = database.GetContainer(containername);

            Console.WriteLine("Connected to Cosmos DB");
            QueryDefinition query1 = new QueryDefinition("select * from c");
            using (FeedIterator<item> iterator = container.GetItemQueryIterator<item>(query1))
            {
                while (iterator.HasMoreResults)
                {
                    FeedResponse<item> response = await iterator.ReadNextAsync();
                    foreach(var itemval in response)
                    {
                        Console.WriteLine($"The item id is {itemval.id} and the firstname is {itemval.FirstName}");
                    }
                    //checking which region endpoint is been used 
                    Console.WriteLine($"The response diagnostics are ${response.Diagnostics}");

                }


            }
            
        }
    }

    public class item
    {
       public string id{get; set;}

       public string isAlive { get; set; }

       public string FirstName { get; set; }

       public string LastName { get; set; }



    }
}