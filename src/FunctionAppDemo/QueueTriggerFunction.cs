using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionAppDemo
{
    public static class QueueTriggerFunction
    {
        [FunctionName("QueueTriggerFunction")]
        public static void Run(
            [QueueTrigger("outqueue", Connection = "AzureWebJobsDashboard")]CustomQueueMessage myQueueItem, 
            DateTimeOffset insertionTime,
            [DocumentDB("outDatabase", "MyCollection", ConnectionStringSetting = "functionsdemo_DOCUMENTDB")] out dynamic documentdb, 
            TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem.Name} inserted at {insertionTime}");

            // Save message data from queue to DocumentDB
            documentdb = new
            {
                name = myQueueItem.Name
            };
        }

        public class CustomQueueMessage
        {
            public string Name { get; set; }
        }
    }
}
