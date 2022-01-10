using Azure.Messaging.EventGrid;
using AzureAssignment.DTOs;
using AzureAssignment.Models;
using AzureAssignment.Repository;
using AzureAssignment.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Services;

namespace AzureAssignment.Services.Implementation
{
    public class EventPublisherOnPurchaseOrderService : IEventPublisherOnPurchaseOrderService
    {
        private ILogger<EventPublisherOnPurchaseOrderService> _log;
        private readonly KeyVaultService keyVaultService = new KeyVaultService();
        private readonly EventPublisherService eventPublisherService = new EventPublisherService();


        public EventPublisherOnPurchaseOrderService(ILogger<EventPublisherOnPurchaseOrderService> log)
        {
            _log = log;
        }
        public async Task<(string msg, bool isErr)> PublishEventOnPurchaseOrderCreation(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<PurchaseOrderNameDto>(requestBody);

            //Check whether Name exists or not
            if (body.Name == null)
            {
                return ("Name is Required", true);
            }
          

            //Extracting topic secrets from key vault
            string topicEndpoint = await keyVaultService.GetSecretValue("PurchaseOrderEventTopicEndpoint");
            string topicAccessKey = await keyVaultService.GetSecretValue("PurchaseOrderEventTopicAccessKey");

            var (msg, isErr) = await eventPublisherService.PublishEvent(body, topicEndpoint, topicAccessKey, "onPurchaseOrder");

            if (isErr)
            {
                return (msg, true);
            }

            return ("Event Published", false);
        }
    }
}
