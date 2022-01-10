using AzureAssignment.Models;
using AzureAssignment.Exceptions;
using AzureAssignment.Repository;
using AzureAssignment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzureAssignment.Controllers
{
    class PurchaseOrderController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [FunctionName("UpdatePurchaseOrder")]
        public async Task<IActionResult> UpdatePurchaseOrder(
              [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "purchase-order/{id}")] HttpRequest req, int id,
              ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                PurchaseOrder Body = JsonConvert.DeserializeObject<PurchaseOrder>(requestBody);

                var (msg, isErr) = await _purchaseOrderService.UpdatePurchaseOrder(id, Body);
                if (isErr == false)
                {
                    log.LogInformation(msg);
                    return new OkObjectResult(msg);
                }
                else
                {
                    log.LogInformation(msg);
                    return new OkObjectResult(msg);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new BadRequestResult();
            }
        }

        [FunctionName("GetAllPurchaseOrders")]
        public async Task<IActionResult> GetAllAppointment(
              [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "purchase-order")] HttpRequest req,
              ILogger log)
        {
            var allPurchaseOrders = await _purchaseOrderService.GetAllPurchaseOrders();


            return new OkObjectResult(allPurchaseOrders);
        }

        [FunctionName("CreatePurchaseOrder")]
        public async Task<IActionResult> CreatePurchaseOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "purchase-order")] HttpRequest req,
            ILogger log)
        {

            try
            {
                var (msg, isErr) = await _purchaseOrderService.CreatePurchaseOrder(req);
                if (isErr)
                {
                    log.LogError(msg);
                    return new BadRequestObjectResult(msg);
                }
                log.LogInformation(msg);
                return new OkObjectResult(msg);

            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }


        [FunctionName("GetPurchaseOrderById")]
        public async Task<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "purchaseOrder/{id}")] HttpRequest req, int id,
               ILogger logger)
        {
            try
            {
                PurchaseOrder purchaseOrder = await _purchaseOrderService.GetPurchaseOrderById(id);
                logger.LogInformation("Purchase order with id = " + id + " retrieved successfully!");
                return new OkObjectResult(purchaseOrder);
            }
            catch (PurchaseOrderNotFoundException p)
            {
                logger.LogError("Error while retriveing purchase order:" + p.Message);
                return new BadRequestObjectResult(p.Message);
            }
            catch (Exception e)
            {
                logger.LogError("Error retriving PO:" + e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("DeletePurchaseOrderById")]
        public async Task<IActionResult> DeleteById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "purchaseOrder/{id}")] HttpRequest req, int id, ILogger logger)
        {
            try
            {
                await _purchaseOrderService.DeletePurchaseOrderById(id);
                logger.LogInformation("Purchase order with id = " + id + " deleted successfully from database!");
                return new OkObjectResult(StatusCodes.Status200OK);
            }
            catch (PurchaseOrderNotFoundException p)
            {
                logger.LogError("Error while deleting a record from purchase order:" + p.Message);
                return new BadRequestObjectResult(p.Message);
            }
            catch (Exception e)
            {
                logger.LogError("Error deleting PO:" + e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
