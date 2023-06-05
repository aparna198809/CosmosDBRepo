using System;
using Microsoft.Azure.Cosmos;
using Azure.Identity;
using System.Collections.ObjectModel;

namespace ComputedPropertiesCosmos
{ 
public class computedpropresult
    {
        public string id { get; set; }
        public string name { get; set; }
    }
class Program
{
    private static readonly string uri = "https://testactru.documents.azure.com:443/";
    private static readonly string databasename = "test1";
    



    public static async Task Main(String[] args)
    {
        //connecting to cosmos db

        var tokenCredential = new DefaultAzureCredential();
        CosmosClient client = new CosmosClient(uri, tokenCredential);
        Console.WriteLine("Connection established");

        //creating the database  

        Database database = await client.CreateDatabaseIfNotExistsAsync(databasename);
        Console.WriteLine($"The database{database.Id} is successfully created ");


        //creating a container with computed properties 

        ContainerProperties contprop = new ContainerProperties("computedpropcont", "/pk")
        {
            ComputedProperties = new Collection<ComputedProperty>
            {
                new ComputedProperty
                {
                    Name ="cp_lowername",
                    Query ="select VALUE LOWER(c.name) from c "
                },
                new ComputedProperty
                {
                    Name="cp_uppername",
                    Query ="select VALUE UPPER(c.name) from c"
                },
                new ComputedProperty
                {
                    Name ="cp_20percentdiscount",
                    Query="select VALUE (c.price *0.2) from c "
                },

                new ComputedProperty
                {
                    Name ="cp_primarycategory",
                    Query= "select VALUE SUBSTRING(c.categoryName,0,INDEX_OF(c.categoryName,',')) from c"
                }
            }
        };

        Container cont = await database.CreateContainerIfNotExistsAsync(contprop);
        Console.Out.WriteLine($"The container{contprop.Id} is created successfully");

        //querying the container for data without using computed properties 

        QueryDefinition query1 = new QueryDefinition("Select c.id,LOWER(c.name) as name from c");
        using (FeedIterator<computedpropresult> iterator = cont.GetItemQueryIterator<computedpropresult>(query1))
        {
            while (iterator.HasMoreResults)
            {
                FeedResponse<computedpropresult> resp = await iterator.ReadNextAsync();
                foreach (var item in resp)
                {
                    Console.Out.WriteLine($"{item.name} is read with request charge {resp.RequestCharge}");
                }

            }
        }

        //querying the container with the computed properties 

            QueryDefinition query2 = new QueryDefinition("select c.id,c.cp_lowername as name from c");
            using (FeedIterator<computedpropresult> iterator1= cont.GetItemQueryIterator<computedpropresult>(query2))
                {
                while (iterator1.HasMoreResults)
                {
                    FeedResponse<computedpropresult> resp1 = await iterator1.ReadNextAsync();
                    foreach(var item1 in resp1)
                    {
                        Console.Out.WriteLine($"{item1.name} with request charge{resp1.RequestCharge}");
                    }
                    
                }
            }
            //primarycategory when ran the full query in portal its consuming more RU ,but when using computedproperties it is not consuming any RU"
            QueryDefinition query3 = new QueryDefinition("SELECT c.cp_primarycategory as name FROM c");
            using (FeedIterator<computedpropresult> iterator2 = cont.GetItemQueryIterator<computedpropresult>(query3))
            {
                while(iterator2.HasMoreResults)
                {
                    FeedResponse<computedpropresult> resp2 = await iterator2.ReadNextAsync();
                    foreach(var item2 in resp2)
                    {
                        Console.Out.WriteLine($"{item2.name} returned with RU {resp2.RequestCharge}");
                    }

                }
            }

    }
}
}