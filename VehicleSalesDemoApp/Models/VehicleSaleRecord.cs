using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleSalesDemoApp.Models
{
    public class VehicleSaleRecord
    {
        public string? Model { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Price { get; set; }
        public decimal VAT { get; set; }

        public decimal PriceWithVAT => Price * (1 + VAT / 100);
    }
}
