using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAssignment.Services.Interfaces
{
    public interface IEventPublisherOnPurchaseOrderService
    {
        Task<(string msg, bool isErr)> PublishEventOnPurchaseOrderCreation(HttpRequest req);
    }
}
