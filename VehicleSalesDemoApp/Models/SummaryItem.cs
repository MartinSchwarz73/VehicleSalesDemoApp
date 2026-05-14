using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleSalesDemoApp.Models
{
    public class SummaryItem
    {
        public string? Model { get; set; }
        public decimal Total { get; set; }
        public decimal TotalWithVAT { get; set; }
    }
}
