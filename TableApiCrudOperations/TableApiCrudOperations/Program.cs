using Azure;
using Azure.Data.Tables;
using System;
using System.Collections;

namespace TableAPIDemo
{
    public record Product:ITableEntity
    {
        public string RowKey { get; set; } = default!;

        public string PartitionKey { get; set; } = default!;

        public string Name { get; init; } = default!;

        public int Quantity { get; init; }//init value can only be set inside the constructor 

        public bool sale { get; set; } = default!;

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    class Program
    {
       // public static readonly string _url = "<my connection>";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("This is the program structure");
            TableServiceClient client = new TableServiceClient(_url);
            Console.WriteLine("The connection established");

            //creating a table 
            TableClient tableclient = client.GetTableClient(tableName: "adventureworks");
            await tableclient.CreateIfNotExistsAsync();

            //adding an entity 

                var prod1 = new Product()
                {
                    RowKey = "6871945454",
                    PartitionKey = "gear-surf-surfboards",
                    Name = "my Surfboard",
                    Quantity = 20,
                    sale = true
                };

            var prod2 = new Product()
            {
                RowKey = "68719518388335",
                PartitionKey = "gear-surf-surfboards",
                Name = "Sea Surfboard",
                Quantity = 15,
                sale = true
            };

            var prod3= new Product()
            {
                RowKey = "68719518388",
                PartitionKey = "gear-surf-surfboards",
                Name = "Ocean Surfboard",
                Quantity = 8,
                sale = true
            };

   
            var resp = await tableclient.AddEntityAsync<Product>(prod1);
                Console.Out.WriteLine($"The entity is added successfully");

            var resp1 = await tableclient.AddEntityAsync<Product>(prod2);
             Console.Out.WriteLine($"The entity is added successfully");

            var resp2 = await tableclient.AddEntityAsync<Product>(prod3);
            Console.Out.WriteLine($"The entity is added successfully");


            //reading an entity 

            var pro = tableclient.GetEntityAsync<Product>(rowKey: "68719518388", partitionKey: "gear-surf-surfboards");
            Console.Out.WriteLine(pro.Result.Value.Name);

            //running a query 

            var products = tableclient.Query<Product>(x => x.PartitionKey == "gear-surf-surfboards");
            Console.WriteLine("Multiple products obtained");
            foreach(var product in products)
            {
                Console.Out.WriteLine(product.Name);
            }
        }
    }
}