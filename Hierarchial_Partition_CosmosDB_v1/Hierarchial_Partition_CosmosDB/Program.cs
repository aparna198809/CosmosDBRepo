using System;
using System.Drawing.Text;
using Microsoft.Azure.Cosmos;

namespace Hierarchial_Partition_CosmosDB
{
    class Program
    {
        private static readonly string uri = "https://testactru.documents.azure.com:443/";
        private static readonly string key = "ldBiqHg8u7QxyRtAgvq2gMUpQcDE4HlcxLvyTabuyX02t1zTasXXdOO2GX2QbRT83IMJdAiVGp3vACDbTERtmQ==";
        private static readonly string databasename = "test1";
        private static readonly string containername = "testcontainer";
        public static async Task Main(String[] args)
        {         
            //connecting to cosmos db
            CosmosClient client = new CosmosClient(uri, key);
            Console.WriteLine("Connection established");

            //creating the database  

            Database database = await client.CreateDatabaseIfNotExistsAsync(databasename);
            Console.WriteLine($"The database{database.Id} is successfully created ");

            //creating the collection with hierarchial partition key 
            List<string> subPartitionKeyPaths = new List<string> { "/TenantId", "/UserId", "/TransactionId" };

            ContainerProperties prop = new ContainerProperties(id : "containername",partitionKeyPaths: subPartitionKeyPaths);
            Container cont = await database.CreateContainerIfNotExistsAsync(prop, throughput: 1000);

            Console.WriteLine($"The container {cont.Id} is created successfully");
        }  
      
    }
}