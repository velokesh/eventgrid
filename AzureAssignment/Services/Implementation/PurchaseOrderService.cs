using AzureAssignment.Exceptions;
using AzureAssignment.Models;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AzureAssignment.DTOs;
using System.Net.Http;
using Services;
using AzureAssignment.Repository.Interfaces;

namespace AzureAssignment.Services
{
    class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly KeyVaultService keyVaultService = new KeyVaultService();
        private IRepository<PurchaseOrder> _repository;

        public PurchaseOrderService(IRepository<PurchaseOrder> repository)
        {
            _repository = repository;
        }

        public async Task DeletePurchaseOrderById(int poId)
        {
            PurchaseOrder purchaseOrder = await _repository.Get(poId);
            if (purchaseOrder == null)
                throw new PurchaseOrderNotFoundException("Couldnt found the purchase order to delete.");
            await _repository.Delete(purchaseOrder);                  
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(int poId)
        {
            PurchaseOrder purchaseOrder = await _repository.Get(poId);
            if (purchaseOrder == null)
                throw new PurchaseOrderNotFoundException("No such purchase order.");
            return purchaseOrder;
        }

        public async Task<(string msg, bool isErr)> UpdatePurchaseOrder(int id, PurchaseOrder purchaseOrder)
        {
            if (await _repository.Get(id) == null)
                return ("Requested Purcahse Order doesn't exist", true);

            if (id != purchaseOrder.Id)
                return ("Id cannot be changed", true);

            if (purchaseOrder.Name == null)
                return ("Name of the Purchase Order cannot be null", true);

            purchaseOrder.Id = id;
            await _repository.Update(purchaseOrder);
            return ("Updated successfully", false);
        }
        public async Task<List<PurchaseOrder>> GetAllPurchaseOrders()
        {
            return await _repository.GetAll();
        }

        public async Task<(string msg, bool isErr)> CreatePurchaseOrder(HttpRequest req)
        {
            //Get request body data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<CreatePurchaseOrderDTO>(requestBody);


            //Check whether Name exists or not
            if (body.Name == null)
            {
                return ("Name is Required", true);
            }

            //Check whether Date exists or not
            if (body.Date == null)
            {
                return ("Date is Required", true);
            }

            //Check whether PO exists or not
            PurchaseOrder isPOPrsent = await _repository.Get(body.Id);

            if (isPOPrsent != null)
            {
                return ("Purchase Order Already present", true);
            }

            //Create Object from Dto
            PurchaseOrder purchaseOrder = new PurchaseOrder()
            {
                Id = body.Id,
                Name = body.Name,
                Date = body.Date
            };


            await _repository.Insert(purchaseOrder);

            return await onPurchaseOrderCreation(req, body.Name , body.Id);

        }

        private async Task<(string msg, bool isErr)> onPurchaseOrderCreation(HttpRequest req, string Name , int Id)
        {
            using (var client = new HttpClient())
            {
                var purchaseOrderName = new PurchaseOrderNameDto ()
                { 
                    Id = Id,
                    Name = Name
                };
                client.BaseAddress = new Uri(await keyVaultService.GetSecretValue("EventGridTopicPO"));
                var response = client.PostAsJsonAsync("api/publish-event/purchase-order-creation", purchaseOrderName).Result;
                if (response.IsSuccessStatusCode)
                {
                    return ("Purchase Order Created", false);
                }
                else
                    return ("Purchase Order Created, Error occurred in creating Appointment", true);
            }
        }
    }
}
