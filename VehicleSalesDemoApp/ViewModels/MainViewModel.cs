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

        // Toto pole určuje, zda se v souhrnu zobrazí i modely s nulovými prodeji.
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

        // Toto pole uchovává název načteného souboru, který se zobrazuje v závorce vedle názvu aplikace.
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

        // Tato metoda načítá XML soubor a dynamicky mapuje jeho obsah na vlastnosti třídy VehicleSaleRecord pomocí reflexe.
        public void LoadXmlDynamic(string path)
        {
            try
            {
                var doc = XDocument.Load(path);

                if (doc.Root == null)
                    throw new Exception("XML nemá kořenový element.");

                Sales.Clear();

                IEnumerable<XElement> docElements = doc.Root.Elements();
                var nodes = docElements
                    .Select(currentNode =>
                    {
                        var record = new VehicleSaleRecord();

                        foreach (var element in currentNode.Elements())
                        {
                            // najde property podle názvu XML elementu
                            var property = typeof(VehicleSaleRecord)
                            .GetProperty(element.Name.LocalName);

                            // pokud property existuje
                            if (property != null)
                            {
                                // převede string z XML na správný typ
                                var convertedValue = Convert.ChangeType(
                                   element.Value,
                                   property.PropertyType);

                                // nastaví hodnotu property
                                property.SetValue(record, convertedValue);
                            }
                        }

                        return record;
                });

                foreach (var node in nodes)
                    Sales.Add(node);

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

        // Tato metoda načítá XML soubor a mapuje jeho obsah na vlastnosti třídy VehicleSaleRecord pomocí pevně definovaných názvů elementů.
        public void LoadXml(string path)
        {
            try
            {
                var doc = XDocument.Load(path);

                Sales.Clear();

                var data = doc.Descendants("Vehicle")
                    .Select(x => new VehicleSaleRecord
                    {
                        Model = (string)x.Element("Model") ?? "",
                        SaleDate = DateTime.TryParse((string)x.Element("SaleDate"), out var d) ? d : null,
                        Price = double.TryParse((string)x.Element("Price"), out var price) ? price : -1,
                        VAT = double.TryParse((string)x.Element("VAT"), out var vat) ? vat : -1 
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


        // Tato metoda vypočítává souhrn prodeje pouze pro víkendové prodeje a aktualizuje kolekci Summary.
        public void CalculateWeekendTotals()
        {
            Summary.Clear();

            var result = Sales
                .Where(s => s.SaleDate?.DayOfWeek == DayOfWeek.Saturday
                         || s.SaleDate?.DayOfWeek == DayOfWeek.Sunday)
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

        // Tato metoda aktualizuje souhrn prodeje pro všechny modely, přičemž zohledňuje nastavení ShowZeroItems.
        private void UpdateSummary()
        {
            Summary.Clear();

            var summary = Sales
                .GroupBy(x => x.Model)
                .Select(g =>
                {
                    var weekendSales = g
                        .Where(x => x.SaleDate?.DayOfWeek == DayOfWeek.Saturday
                                 || x.SaleDate?.DayOfWeek == DayOfWeek.Sunday);

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
