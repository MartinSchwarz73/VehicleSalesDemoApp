# Vehicle Sales Demo (WPF)

This is a simple WPF desktop application built using the MVVM pattern.
The app loads vehicle sales data from an XML file and displays it in a structured way.

## Features

* Load vehicle sales data from an XML file
* Display all sales records in a table
* Calculate weekend sales totals per vehicle model
* Show totals:
  * excl. VAT
  * incl. VAT
* Option to include/exclude models with zero weekend sales

## Technologies

* C#
* WPF (.NET)
* MVVM pattern
* Data binding
* LINQ (data processing)

## Data Format (XML)

Each record contains:

* Model (string)
* SaleDate (DateTime)
* Price (double)
* VAT (double)

## How to Use

1. Click **Load XML**
2. Select a valid XML file
3. The data will be displayed automatically
4. Weekend sales summary is calculated instantly

## Notes

* Dates and numbers are formatted using Czech locale (cs-CZ)
* Weekend is defined as Saturday and Sunday
* The UI is designed with simplicity and clarity in mind

## Purpose

This project was created as a learning exercise to practice:

* WPF UI design
* MVVM architecture
* Working with XML data
* Data transformation and presentation

---

## Data Preparation (PowerShell)

To simplify working with input data, a PowerShell script was created to convert tabular data into XML format.

### Workflow

1. Copy a table from a PDF document (e.g. using Adobe Acrobat) to the clipboard
2. Run the PowerShell script
3. The script automatically processes the clipboard content
4. A properly structured XML file is generated and ready to be loaded into the application

### Benefits

* No manual XML editing required
* Fast and simple data preparation
* Reduces human errors
* Demonstrates basic automation skills

> Note: This script was created as a helper tool and is not part of the main WPF application. <br>
See /PowerShell/ConvertToXml.ps1 for implementation.


