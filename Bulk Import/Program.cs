using System;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using Azure.Identity;
using Bogus;

public class Item {
    public string id {get; set;}
    public string pk {get; set; }
    public string username {get; set;}
}

namespace Bulk_Import
{
    class Program
    {

        private static readonly string uri = "https://testactru.documents.azure.com:443/";
        private static readonly string _databasename ="test1";
        private static readonly string _containername ="testcontainer";
        

        public static async Task Main(String[] args)
        {
            //establishing connection with cosmos db 

            //   CosmosClient cosmosclient = new CosmosClient(_uri,_passkey, new CosmosClientOptions(){AllowBulkExecution = true});
            var tokenCredential = new DefaultAzureCredential();
            CosmosClient cosmosclient = new CosmosClient(uri,tokenCredential);
            Console.WriteLine("The connection established successfully");

            //creating a database 

            Database database = await cosmosclient.CreateDatabaseIfNotExistsAsync(_databasename);
            Console.WriteLine($"The database {database.Id} is successfully created");

            //creating a collection with no indexes defined

            List<string> keypaths = new List<String> { "/TenantId", "/UserId", "/TransactionId" };

            ContainerProperties prop = new ContainerProperties(id: _containername, partitionKeyPaths: keypaths);

            Container container = await database.CreateContainerIfNotExistsAsync(prop,throughput:4000);

            Console.WriteLine($"The container {container.Id} is successfully created");


            try{

                

                //getting a list of items to insert 

                Console.WriteLine("Getting a list of items using Bogus library");
                IReadOnlyCollection<Item> itemresp = getitemtoinsert();

                //get the collection name and start the stopwatch 

                Container cont = database.GetContainer(_containername);
                Stopwatch watch = Stopwatch.StartNew();

                //get a list of tasks 
                List<Task> task = new List<Task>(40000);
                foreach (Item item in itemresp){
                    task.Add(cont.CreateItemAsync(item,new PartitionKey(item.pk)));
                }

                await Task.WhenAll(task);
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                
            
            }
            catch(Exception e){
                Console.WriteLine(e);
            }
    
            
        }


        public static IReadOnlyCollection<Item> getitemtoinsert(){
            var Tenantslist = new string[] { "Microsoft", "Google", "Contoso", "Amazon" };
            return new Bogus.Faker<Item>()
            .RuleFor(o=>o.id,f=>Guid.NewGuid().ToString())
            .RuleFor(o=>o.TenantId, f=>f.PickRandom(Tenantslist))
            .RuleFor(o=>o.UserId, f=>f.Internet.UserName())
            .RuleFor(o=>o.TransactionId,f=>)
            .Generate(400000);
        }
    }
}
