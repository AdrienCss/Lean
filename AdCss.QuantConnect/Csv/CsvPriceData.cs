using System;
using CsvHelper.Configuration.Attributes;

namespace AdCss.QC.Csv;

public class CsvPriceData // Used to export Data in CSV
{
    [Name("Date")] [Format("dd/MM/yyyy")] public DateTime Date { get; set; }

    [Name("CompanyName")] public string CompanyName { get; set; }
    [Name("Open")] public double? pOpen { get; set; }
    [Name("High")] public double? pHigh { get; set; }
    [Name("Last")] public double? pLast { get; set; }
    [Name("Low")] public double? pLow { get; set; }
    [Name("Close")] public double? pCloseGross { get; set; }
    [Name("AdjClose")] public double? pAdjClose { get; set; }
    [Name("Volume")] public double? Volume { get; set; }
    [Name("Currency")] public string Currency { get; set; }
    [Name("QuoteType")] public string QuoteType { get; set; }
    [Name("ExchangeName")] public string ExchangeName { get; set; }
    [Name("Ticker")] public string Ticker { get; set; }
    [Name("TimeZone")] public string TimeZone { get; set; }
    [Name("TimeZoneName")] public string TimeZoneName { get; set; }


    public CsvPriceData(
        double? low,
        double? closeGross,
        double? ajdClose,
        double? high,
        double? open,
        double? volume,
        DateTime date)
    {
        pLow = low;
        pHigh = high;
        pAdjClose = ajdClose;
        pCloseGross = closeGross;
        pOpen = open;
        Volume = volume;
        Date = date;
    }


    public CsvPriceData()
    {

    }
}
