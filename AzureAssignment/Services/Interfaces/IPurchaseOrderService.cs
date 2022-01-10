using AzureAssignment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAssignment.Services
{
    interface IPurchaseOrderService
    {
        public Task<PurchaseOrder> GetPurchaseOrderById(int poId);

        public Task DeletePurchaseOrderById(int poId);
        public Task<(string msg, bool isErr)> UpdatePurchaseOrder(int id, PurchaseOrder purchaseOrder);

        public Task<List<PurchaseOrder>> GetAllPurchaseOrders();
        Task<(string msg, bool isErr)> CreatePurchaseOrder(HttpRequest req);
    }
}
