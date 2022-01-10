using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureAssignment.Services.Interfaces;

namespace AzureAssignment.Controllers
{
    public class EventPublisherController
    {

        private readonly IEventPublisherOnPurchaseOrderService _eventPublisherService;
        public EventPublisherController(IEventPublisherOnPurchaseOrderService eventPublisherService)
        {
            _eventPublisherService = eventPublisherService;
        }

        [FunctionName("PublishEventOnPurchaseOrderCreation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "publish-event/purchase-order-creation")] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation("from control"+req.ToString());
                var (msg, isErr) = await _eventPublisherService.PublishEventOnPurchaseOrderCreation(req);
                if (isErr)
                {
                    log.LogError(msg);
                    return new BadRequestObjectResult(msg);
                }

                log.LogInformation(msg);
                return new OkObjectResult(msg);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new BadRequestResult();
            }

            
        }
    }
}
