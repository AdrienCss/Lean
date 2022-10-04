using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using QLNet;
using QuantConnect;
using QuantConnect.Logging;
using Path = System.IO.Path;
using AdCss.QC.API.YahooFinance;
using AdCss.QC.Enum;

namespace AdCss.QC.Csv
{

    public static class CsvGenerator
    {
        public static Dictionary<string, bool> IsCheck = new Dictionary<string, bool>();

        public static string GetFileName(string ticker, IdentifierType idType, string path)
        {
            if (IsCheck.ContainsKey(ticker))
                return ticker;

            var identifierProvider = String.Empty;

            if (idType == IdentifierType.Bloomberg)
            {
                if (ticker != "SX5E")
                    identifierProvider = ticker.Replace("_", " ") + " Equity";
                else
                    identifierProvider = ticker + " INDEX";
            }
            else
            {
                identifierProvider = ticker;
            }


            var pathFileHistoryPrice = Path.Combine(path, "equity", "AdCss", "Daily", $"{ticker}.csv");

            if (!File.Exists(pathFileHistoryPrice))
                ExportData(identifierProvider, pathFileHistoryPrice);
            else
            {   // to be reimported because of adjusted prices.
                //UpdateData(identifierProvider, pathFileHistoryPrice);
            }

            IsCheck[ticker] = true;
            return ticker;
        }

        private static void UpdateData(string identifierProvider, string path)
        {
            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var data = csv.EnumerateRecords(new CsvPriceData()).ToList();
                    var maxDate = data.Max(i => i.Date);

                    if (maxDate >= DateTime.Now.Date.AddDays(-7))
                    {
                        Log.Trace(
                            $"__________________ IMPORTING PRICES : {identifierProvider} is UpToDate. Last date = {maxDate}");
                        csv.Dispose();
                        return;
                    }

                    var prices = YahooFinanceAPI.GetHistoPrice(identifierProvider, Eperiod._1y, maxDate);

                    Task.WaitAll(prices);

                    if (prices.Result.Count() != 0)
                    {
                        csv.Dispose();
                        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                        {
                            HasHeaderRecord = false,
                        };

                        using (var stream = File.Open(path, FileMode.Append))
                        using (var writer = new StreamWriter(stream))
                        using (var csv_ = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv_.WriteRecords(prices.Result);
                        }
                    }
                    else
                    {
                        Log.Trace(
                            $"__________________ IMPORTING PRICES WARNING: {identifierProvider} is UpToDate. Last date = {maxDate}");
                    }
                }
            }
            catch (Exception e)
            {


            }
        }

        private static void ExportData(string identifierProvider, string path)
        {
            var result = YahooFinanceAPI.GetHistoPrice(identifierProvider, Eperiod._10y);

            Task.WaitAll(result);

            if (result.Result.Count() != 0)
            {
                using (var writer = new StreamWriter(path))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(result.Result);
                }
                Log.Trace($"__________________ IMPORTING PRICES : {identifierProvider} has been imported for the first time. From {result.Result.Min(i => i.Date).ToString("d")} to {result.Result.Min(i => i.Date).ToString("d")}");
            }
            else
            {
                Log.Trace($"__________________ IMPORTING PRICES ERROR : {identifierProvider} has no time Series !!!!!!!!! _________________");
            }
        }


        /// <summary>
        /// Index composition need to be imported Manually before. 
        /// /!\ Must be imported systematically later.
        /// </summary>
        /// <param name="YahooTicker"></param>
        /// <returns></returns>
        public static HashSet<string> GetIndexComposition(string YahooTicker)
        {
            var csvPath = Path.Combine(Globals.DataFolder, "index", "AdCss", $"{YahooTicker}.csv");


            var comp = new HashSet<string>();

            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                comp = csv.EnumerateRecords(new IndexComp()).Select(i => i.Ticker).ToHashSet();
            }
            return comp;
        }
    }

    public enum IdentifierType
    {
        None = 0,
        Bloomberg = 1,
        SP_CapIQ = 2,
        ISIN = 3,
        YahooFinance
    }

}

public class IndexComp
{
    [Name("Ticker")]
    public string Ticker { get; set; }
}

