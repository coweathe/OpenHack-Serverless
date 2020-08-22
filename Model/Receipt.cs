using System;

namespace BFYOC
{
    public class Receipt
    {
        public int totalItems { get; set; }
        public decimal totalCost { get; set; }
        public Guid salesNumber { get; set; }
        public DateTime salesDate { get; set; }
        public string storeLocation { get; set; }
        public string receiptUrl { get; set; }
    }
}