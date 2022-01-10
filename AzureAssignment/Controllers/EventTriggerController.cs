// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using AzureAssignment.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureAssignment.Controllers
{
    public class EventTriggerController
    {

        private readonly IEventTriggerService _eventTriggerService;
        public EventTriggerController(IEventTriggerService eventTriggerService)
        {
            _eventTriggerService = eventTriggerService;
        }
     

        [FunctionName("EventTrigger")]
        public async void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
      
            try
            {
                var (msg, isErr) = await _eventTriggerService.TriggerEventOnPurchaseOrderCreation(eventGridEvent.Data);
                
                if (isErr)
                {
                    log.LogError(msg);
           
                }

                log.LogInformation(msg);
            
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
             
            }
        }
    }
}
