using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureAssignment.Services.Implementation
{
    public class EventTriggerService : IEventTriggerService
    {
        private ILogger<EventTriggerService> _log;
        private readonly KeyVaultService keyVaultService = new KeyVaultService();
        private static HttpClient httpClient = new HttpClient();

        public EventTriggerService(ILogger<EventTriggerService> log)
        {
            _log = log;
        }

        public async Task<(string msg, bool isErr)> TriggerEventOnPurchaseOrderCreation(object eventData)
        {
            var jsondata = JsonConvert.SerializeObject(eventData);
            var tmp = new { Id = "", Name = "" };
            var data = JsonConvert.DeserializeAnonymousType(jsondata, tmp);

     

            if (data.Name == null || data.Id == null)
            {
                return ("Name and Id are required", true);
            }

            string createAppointmentUrl = await keyVaultService.GetSecretValue("createAppointmentUrlEventGrid");
            var response = await httpClient.PostAsJsonAsync(createAppointmentUrl, new
            {
                Id = data.Id,
                Name = data.Name
            });

            if (!response.IsSuccessStatusCode)
            {
                return ("Error occurced appointment not creadted", true);
            }

     
            return ("Event Triggered", false);
        }

    }
}
