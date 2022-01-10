using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Services;
using Azure.Messaging.EventGrid;

namespace AzureAssignment.Services.Implementation
{
    public class EventPublisherService
    {
  
        public async Task<(string msg, bool isErr)> PublishEvent(object body , string topicEndpoint , string topicAccessKey , string eventType)
        {
            
            EventGridPublisherClient client = new EventGridPublisherClient(new Uri(topicEndpoint),
                new Azure.AzureKeyCredential(topicAccessKey));

          

            //Creating an event with Subject, Eventtype, dataVersion and data
            EventGridEvent egEvent = new EventGridEvent("Subject", eventType, "1.0", body);


            try
            {
                await client.SendEventAsync(egEvent);
             
            }
            catch (Exception err)
            {
                return (err.Message, true);          
            }

            return ("event published" , false);
        }
    }
}
