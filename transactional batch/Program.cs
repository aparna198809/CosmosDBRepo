using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Azure.Identity;


public class Product
            {
                public string id { get; set; }
                public string productid { get; set; }
                public string category { get; set; }
                public string name { get; set; }
            }

    public class Part 
            {
                public string id { get; set; }
                public string category { get; set; }
                public string name { get; set; }
                public string productid { get; set; }
            }

  namespace transactionalbatch
 {

    class Program
    {
 

   private static readonly string _uri ="https://sqlapiserverlessdevtstaparna.documents.azure.com:443/";
    

    private CosmosClient client ;
    private Database database;
    private Container container ;

    const string databasename = "cosmicworks";
    const string containername = "Product";  
         

        private static async Task Main(string[] args)
        {
            var tokenCredential = new DefaultAzureCredential();

         CosmosClient client = new CosmosClient(_uri,tokenCredential);
         Console.WriteLine("The connection established");
         Database database = client.GetDatabase(databasename);
         Container container = database.GetContainer(containername);

         PartitionKey partitionkey = new PartitionKey("abcd");

         Console.WriteLine($"The partition key is {partitionkey}");

         Product bike = new(){id ="12345",productid="123",category= "abcd",name ="toyota"};

         Part part = new(){id = "9999",category = "abcd",name = "cars",productid= "123"};

         TransactionalBatch batch = container.CreateTransactionalBatch(partitionkey);
         batch.CreateItem<Product>(bike);
         batch.CreateItem<Part>(part);

        using TransactionalBatchResponse resp = await batch.ExecuteAsync();

         if (resp.IsSuccessStatusCode){
            Console.WriteLine("The operation is successful");
            TransactionalBatchOperationResult<Product> productresp ;
            productresp=  resp.GetOperationResultAtIndex<Product>(0);
            Console.WriteLine($"The result is {productresp}");

            TransactionalBatchOperationResult<Part> partresp ;
            partresp= resp.GetOperationResultAtIndex<Part>(0);
            Console.WriteLine($"The result of part is : {partresp}");
         }
         else
         {
            Console.WriteLine($"error in execution {resp.StatusCode}");
         }
           

        }
    }

 }