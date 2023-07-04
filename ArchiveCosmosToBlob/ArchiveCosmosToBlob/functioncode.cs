using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

using System.Threading.Tasks;
using Azure.Storage.Blobs;
using ChangeFeedSample.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChangeFeedSample
{
    public static class CosmosTrigger
    {
        [FunctionName("CosmosTrigger")]
        public static void Run(
            [CosmosDBTrigger(
               databaseName:  "test1",
               containerName:  "mv-src",
                Connection = "CosmosDBConnectionString",
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true,
            StartFromBeginning = true)]
            IReadOnlyList<Payment> documents,
            ILogger log)
        {
            if (documents != null && documents.Count > 0)
            {
                log.LogInformation("Documents modified: " + documents.Count);

                BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=csg10032001cc580621;AccountKey=dkfFGxMvFxt0gOHvBPc0LoF7KH94qyzafAvDdGaceYh/rwjD/wuL/GhQ5GYG3ZHvlK/JEeI0Z1+ZG1khSF5B5Q==;EndpointSuffix=core.windows.net");

                string containerName = "quickstartblobs" + Guid.NewGuid().ToString();
                var connectionString = "DefaultEndpointsProtocol=https;AccountName=csg10032001cc580621;AccountKey=dkfFGxMvFxt0gOHvBPc0LoF7KH94qyzafAvDdGaceYh/rwjD/wuL/GhQ5GYG3ZHvlK/JEeI0Z1+ZG1khSF5B5Q==;EndpointSuffix=core.windows.net";

                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                container.Create();
                foreach (var document in documents)
                {
                    string paymentString = JsonSerializer.Serialize<Payment>(document);
                    log.LogInformation(paymentString);
                    var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(paymentString));
                    container.UploadBlobAsync(document.Id, memoryStream);
                }
            }
        }
    }
    
}

