using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using VehicleSalesDemoApp.Models;

namespace VehicleSalesDemoApp.ViewModel
{
public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<VehicleDetail> Sales { get; set; } = new();
        public ObservableCollection<SummaryItem> Summary { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        public void LoadXml(string path)
        {
            var doc = XDocument.Load(path);

            Sales.Clear();

            var data = doc.Descendants("Vehicle")
                .Select(x => new VehicleDetail
                {
                    Model = (string)x.Element("Model"),
                    SaleDate = (DateTime)x.Element("SaleDate"),
                    Price = (double)x.Element("Price"),
                    VAT = (double)x.Element("VAT")
                });

            foreach (var item in data)
                Sales.Add(item);
        }

        public void CalculateWeekendTotals()
        {
            Summary.Clear();

            var result = Sales
                .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Saturday
                         || s.SaleDate.DayOfWeek == DayOfWeek.Sunday)
                .GroupBy(s => s.Model)
                .Select(g => new SummaryItem
                {
                    Model = g.Key,
                    Total = g.Sum(x => x.Price),
                    TotalinclVAT = g.Sum(x => x.PriceWithVAT)
                });

            foreach (var item in result)
                Summary.Add(item);
        }
    }

    public class SummaryItem
    {
        public string? Model { get; set; }
        public double Total { get; set; }
        public double TotalinclVAT { get; set; }
    }
}
