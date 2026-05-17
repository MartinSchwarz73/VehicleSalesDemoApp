using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleSalesDemoApp.Models
{
    public class VehicleSaleRecord
    {
        public string? Model { get; set; }
        public DateTime SaleDate { get; set; }
        public double Price { get; set; }
        public double VAT { get; set; }

        public double PriceWithVAT => Price * (1 + VAT / 100);
    }
}
