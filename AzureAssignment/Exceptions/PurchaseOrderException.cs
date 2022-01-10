using System;

namespace AzureAssignment.Exceptions
{
    public class PurchaseOrderNotFoundException : Exception
    {
        public PurchaseOrderNotFoundException(string m) : base(m) { }
    }
}
