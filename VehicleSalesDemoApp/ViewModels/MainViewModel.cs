using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using VehicleSalesDemoApp.Models;

namespace VehicleSalesDemoApp.ViewModels
{
public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<VehicleSaleRecord> Sales { get; set; } = new();
        public ObservableCollection<SummaryItem> Summary { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        public void LoadXml(string path)
        {
            var doc = XDocument.Load(path);

            Sales.Clear();

            var data = doc.Descendants("Vehicle")
                .Select(x => new VehicleSaleRecord
                {
                    Model = (string)x.Element("Model"),
                    SaleDate = (DateTime)x.Element("SaleDate"),
                    Price = (decimal)x.Element("Price"),
                    VAT = (decimal)x.Element("VAT")
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
                    TotalWithVAT = g.Sum(x => x.PriceWithVAT)
                });

            foreach (var item in result)
                Summary.Add(item);
        }
    }

}
