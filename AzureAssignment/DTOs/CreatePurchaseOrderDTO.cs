using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAssignment.DTOs
{
    class CreatePurchaseOrderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
