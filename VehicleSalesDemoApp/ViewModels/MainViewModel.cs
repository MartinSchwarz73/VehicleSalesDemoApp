using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using VehicleSalesDemoApp.Models;

namespace VehicleSalesDemoApp.ViewModels
{
public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SummaryItem> _summary = new();
        public ObservableCollection<VehicleSaleRecord> Sales { get; set; } = new();
        public ObservableCollection<SummaryItem> Summary
        {
            get => _summary;
            set
            {
                _summary = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private bool _showZeroItems = true;

        public bool ShowZeroItems
        {
            get => _showZeroItems;
            set
            {
                _showZeroItems = value;
                OnPropertyChanged();

                UpdateSummary();
            }
        }

        private string _fileName = "";

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public void LoadXml(string path)
        {
            try
            {
                var doc = XDocument.Load(path);

                Sales.Clear();

                var data = doc.Descendants("Vehicle")
                    .Select(x => new VehicleSaleRecord
                    {
                        Model = (string)x.Element("Model"),
                        SaleDate = (DateTime)x.Element("SaleDate"),
                        Price = (double)x.Element("Price"),
                        VAT = (double)x.Element("VAT")
                    });

                foreach (var item in data)
                    Sales.Add(item);

                FileName = "(" + Path.GetFileName(path) + ")";

                UpdateSummary();
            }
            catch (XmlException)
            {
                MessageBox.Show("XML soubor má špatný formát.");
            }
            catch (IOException)
            {
                MessageBox.Show("Soubor se nepodařilo načíst.");
            }
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


        private void UpdateSummary()
        {
            Summary.Clear();

            var summary = Sales
                .GroupBy(x => x.Model)
                .Select(g =>
                {
                    var weekendSales = g
                        .Where(x => x.SaleDate.DayOfWeek == DayOfWeek.Saturday
                                 || x.SaleDate.DayOfWeek == DayOfWeek.Sunday);

                    var total = weekendSales.Sum(x => x.Price);
                    var totalWithVAT = weekendSales.Sum(x => x.PriceWithVAT);

                    return new SummaryItem
                    {
                        Model = g.Key,
                        Total = total,
                        TotalWithVAT = totalWithVAT
                    };
                });

            if (!ShowZeroItems)
            {
                summary = summary.Where(x => x.Total > 0);
            }

            foreach (var item in summary)
                Summary.Add(item);
        }
    }

}
