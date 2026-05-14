# Načtení textu ze schránky
$text = Get-Clipboard

$lines = $text -split "`r?`n" | Where-Object { $_.Trim() -ne "" }

# Odstranění hlavičky
$lines = $lines[1..($lines.Count - 1)]

# XML dokument
$xml = New-Object System.Xml.XmlDocument

# XML deklarace
$declaration = $xml.CreateXmlDeclaration("1.0", "utf-8", $null)
$xml.AppendChild($declaration) | Out-Null

# Root element
$root = $xml.CreateElement("Vehicles")
$xml.AppendChild($root) | Out-Null

# Záznamy
for ($i = 0; $i -lt $lines.Count; $i += 4) {

    $vehicle = $xml.CreateElement("Vehicle")

    # Model
    $model = $xml.CreateElement("Model")
    $model.InnerText = $lines[$i].Trim()
    $vehicle.AppendChild($model) | Out-Null

    # Datum
    $date = [datetime]::ParseExact(
        $lines[$i + 1].Trim(),
        "d.M.yyyy",
        [System.Globalization.CultureInfo]::InvariantCulture
    )

    $saleDate = $xml.CreateElement("SaleDate")
    $saleDate.InnerText = $date.ToString("yyyy-MM-ddTHH:mm:ss")
    $vehicle.AppendChild($saleDate) | Out-Null

    # Cena
    $priceText = $lines[$i + 2]

    $price = $priceText `
        -replace '\.', '' `
        -replace ',-', '' `
        -replace ',', '.'

    $priceElement = $xml.CreateElement("Price")
    $priceElement.InnerText = $price
    $vehicle.AppendChild($priceElement) | Out-Null

    # DPH
    $vat = $xml.CreateElement("VAT")
    $vat.InnerText = $lines[$i + 3].Trim()
    $vehicle.AppendChild($vat) | Out-Null

    # Přidání do root
    $root.AppendChild($vehicle) | Out-Null
}

# Uložení
$xml.Save(".\sales.xml")

Write-Host "XML created: sales.xml"