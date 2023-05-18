using System;
using System.Drawing.Text;
using Azure;
using Microsoft.Azure.Cosmos;
using Azure.Identity;



namespace Hierarchial_Partition_CosmosDB
{
    public class PaymentEvent
    {
        public string id { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public string TransactionId { get; set; }
    }
    class Program
    {
        private static readonly string uri = "https://testactru.documents.azure.com:443/";
      //  private static readonly string key = "ldBiqHg8u7QxyRtAgvq2gMUpQcDE4HlcxLvyTabuyX02t1zTasXXdOO2GX2QbRT83IMJdAiVGp3vACDbTERtmQ==";
        private static readonly string databasename = "test1";
        private static readonly string containername = "testcontainer";


        public static async Task Main(String[] args)
        {
            //connecting to cosmos db

            var tokenCredential = new DefaultAzureCredential();
            CosmosClient client = new CosmosClient(uri, tokenCredential);
            Console.WriteLine("Connection established");

            //creating the database  

            Database database = await client.CreateDatabaseIfNotExistsAsync(databasename);
            Console.WriteLine($"The database{database.Id} is successfully created ");

            //creating the collection with hierarchial partition key 
            List<string> subPartitionKeyPaths = new List<string> { "/TenantId", "/UserId", "/TransactionId" };

            ContainerProperties prop = new ContainerProperties(id : containername,partitionKeyPaths: subPartitionKeyPaths);
            Container cont = await database.CreateContainerIfNotExistsAsync(prop, throughput: 1000);

            Console.WriteLine($"The container {cont.Id} is created successfully");

            //Inserting data into the container 

            PaymentEvent paym = new PaymentEvent()
            {
                id = Guid.NewGuid().ToString(),
                TenantId = "Contoso",
                UserId = "Alice",
                TransactionId = Guid.NewGuid().ToString()
            };

            //Adding the full partition path when inserting the document 

            PartitionKey part = new PartitionKeyBuilder().Add(paym.TenantId).
                Add(paym.UserId).Add(paym.TransactionId).Build();

            ItemResponse<PaymentEvent> resp = await cont.CreateItemAsync(paym, part);

            Console.WriteLine($"The data is inserted via partitionkeybuilder class with RU {resp.RequestCharge}");


            PaymentEvent paym2 = new PaymentEvent()
            {
                id = Guid.NewGuid().ToString(),
                TenantId = "Contoso",
                UserId = "Michael",
                TransactionId = "1001"

            };

            ItemResponse<PaymentEvent> itemresp2 = await cont.CreateItemAsync(paym2);


            Console.WriteLine($"The data is inserted without partitionbuilder class with RU {resp.RequestCharge}");

            //reading an item with the full partitionkeypath 

            Container container = client.GetDatabase("test1").GetContainer("testcontainer");
            Console.WriteLine($"The container is {container.Id}");
            // Create new item

            var id = "cf660be1-464b-4c50-9928-884a6773d4e8";
            var partitionkeypath = new PartitionKeyBuilder().Add("Contoso").Add("Michael").Add("1001").Build();

            var itemResponse = await container.ReadItemAsync<dynamic>(id, partitionkeypath);

            Console.WriteLine($"The item {itemResponse.Resource.TenantId} read from the database with RU {itemResponse.RequestCharge}");

            //querying different permutation combination 

            QueryDefinition query1 = new QueryDefinition("select * from c where c.TenantId = @TenantIdinput and c.UserId = @UserIdinput and c.TransactionId = @TransactionIdinput")
                .WithParameter("@TenantIdinput", "Contoso")
                .WithParameter("@UserIdinput", "Michael")
                .WithParameter("@TransactionIdinput", "1001");            
            using (FeedIterator<PaymentEvent> resultset = container.GetItemQueryIterator<PaymentEvent>(query1))
            {
                while(resultset.HasMoreResults)
                {
                    FeedResponse<PaymentEvent> response = await resultset.ReadNextAsync();
                    PaymentEvent resultEvent = response.First();
                    Console.WriteLine($"\nFound item with TenantId: {resultEvent.TenantId}; UserId: {resultEvent.UserId};TransactionId : {resultEvent.TransactionId}");
                    Console.WriteLine($"The RU charge is {response.RequestCharge}");

                }
            }
            List<PaymentEvent> allPaymentEvents = new List<PaymentEvent>();
            QueryDefinition query2 = new QueryDefinition("select * from c where c.TenantId = @Tenantidinput and c.UserId = @UserIdinput ")
                                        .WithParameter("@Tenantidinput", "Contoso")
                                        .WithParameter("@UserIdinput", "Alice");

            using (FeedIterator<PaymentEvent> iterator = container.GetItemQueryIterator<PaymentEvent>(query2))
            {
                while(iterator.HasMoreResults)
                {
                    FeedResponse<PaymentEvent> response =await iterator.ReadNextAsync();
                    foreach (var item in response)
                    {
                        Console.WriteLine($"\nFound item with TenantId: {item.TenantId}; UserId: {item.UserId}");
                        Console.WriteLine($"The request charge is {response.RequestCharge}");
                        allPaymentEvents.AddRange(response);
                    }

                }
            }

            QueryDefinition query3 = new QueryDefinition("select * from c where c.TenantId = @TenantIdinput")
                .WithParameter("@TenantIdinput", "Contoso");

            using (FeedIterator<PaymentEvent> iterator = container.GetItemQueryIterator<PaymentEvent>(query3))
            {
                while (iterator.HasMoreResults)
                {
                    FeedResponse<PaymentEvent> response = await iterator.ReadNextAsync();
                    foreach (var item in response)
                    {
                        Console.WriteLine($"\nFound item with TenantId: {item.TenantId}");
                        Console.WriteLine($"The request charge is {response.RequestCharge}");
                    }

                }
            }

            QueryDefinition query4 = new QueryDefinition("select * from c where c.UserId =@UserIdinput")
                .WithParameter("@UserIdinput", "Alice");

            using (FeedIterator<PaymentEvent> iterator = container.GetItemQueryIterator<PaymentEvent>(query4))
            {
                while(iterator.HasMoreResults)
                {
                    FeedResponse<PaymentEvent> response = await iterator.ReadNextAsync();
                    foreach(var item in response)
                    {
                        Console.WriteLine($"\nFound item with UserId: {item.UserId}");
                        Console.WriteLine($"The request charge is {response.RequestCharge}");
                    }
                }    
            }



            

        }

    }
}