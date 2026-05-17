using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleSalesDemoApp.Models
{
    public class SummaryItem
    {
        public string? Model { get; set; }
        public double Total { get; set; }
        public double TotalWithVAT { get; set; }
    }
}
