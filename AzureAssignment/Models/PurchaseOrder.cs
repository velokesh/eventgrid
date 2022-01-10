using System;
using System.Collections.Generic;
using System.Text;

namespace AzureAssignment.Models
{
    public class PurchaseOrder : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
