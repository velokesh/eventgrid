using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using System.Threading.Tasks;

namespace AzureAssignment.Services.Implementation
{
    public interface IEventTriggerService
    {
        Task<(string msg, bool isErr)> TriggerEventOnPurchaseOrderCreation(object eventData);
    }
}