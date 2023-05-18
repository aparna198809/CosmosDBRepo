using System;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.Json;

public class Item {
    public string id {get; set;}
    public string pk {get; set; }
    public string username {get; set;}
}

namespace Bulk_Import
{
    class Program
    {

        private static readonly string _uri = "https://mycosmosdbfnapp1809.documents.azure.com:443/";
        private static readonly string _passkey = "h6TjkmopAczL1TotbMhMN5WFnfoCxP80k0TwYdktV2LarI9kmAA8xys2S4zMrDuUdB0k393KJ2gRACDbyaZxRg==";
        private static readonly string _databasename ="bulkimporttest";
        private static readonly string _containername ="bulkimporttest";
        

        public static async Task Main(String[] args)
        {
            //establishing connection with cosmos db 

         //   CosmosClient cosmosclient = new CosmosClient(_uri,_passkey, new CosmosClientOptions(){AllowBulkExecution = true});
            CosmosClient cosmosclient = new CosmosClient(_uri,_passkey);
            Console.WriteLine("The connection established successfully");

            //creating a database 

            Database database = await cosmosclient.CreateDatabaseIfNotExistsAsync(_databasename);
            Console.WriteLine($"The database {database.Id} is successfully created");

            //creating a collection with no indexes defined

            Container container = await database.DefineContainer(_containername,"/pk")
                                                .WithIndexingPolicy()
                                                .WithIndexingMode(IndexingMode.Consistent)
                                                .WithIncludedPaths()
                                                .Attach()
                                                .WithExcludedPaths()
                                                .Path("/*")
                                                .Attach()
                                            .Attach()
                                            .CreateAsync(20000);

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
            var occupations = new string[] { "gardener", "teacher", "writer", "programmer" };
            return new Bogus.Faker<Item>()
            .RuleFor(o=>o.id,f=>Guid.NewGuid().ToString())
            .RuleFor(o=>o.pk, f=>f.PickRandom(occupations))
            .RuleFor(o=>o.username,f=>f.Internet.UserName())
            .Generate(400000);
        }
    }
}
