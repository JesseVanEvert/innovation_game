using Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardDeck.BLL.Service
{    
    public class QueueService
    {
        private static readonly string? connection = "UseDevelopmentStorage=true";//Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static readonly string? queueName = "card-deck-queue";//Environment.GetEnvironmentVariable("QueueName");

        public static QueueClient GetQueueClient()
        {
            //creating and returning the queue with base64 encoding, so the QueueTrigger can receive the message properly.
            var queue = new QueueClient(connection, queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            queue.CreateIfNotExists();

            return queue;
        }

        public static async Task AddMessageAsJsonAsync<T>(T objectToAdd, QueueClient queue)
        {
            string messageAsJson = JsonConvert.SerializeObject(objectToAdd);
            await queue.SendMessageAsync(messageAsJson);
        }

    }
}
