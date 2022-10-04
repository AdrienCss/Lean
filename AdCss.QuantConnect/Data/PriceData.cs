using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuantConnect;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using AdCss.QC;
using AdCss.QC.Csv;


namespace AdCss.QC.Data
{
    public class PriceData : TradeBar
    {
        public string CompanyName { get; set; }
        public string ISIN { get; set; }
        public string SP_Ticker { get; set; }
        public string Currency { get; set; }

        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            var fileName = CsvGenerator.GetFileName(config.Symbol.Value, IdentifierType.YahooFinance, Globals.DataFolder);

            var combine = Path.Combine(Globals.DataFolder, "equity", "AD", "Daily", $"{fileName}.csv");
            return new SubscriptionDataSource(combine, SubscriptionTransportMedium.LocalFile);
        }


        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            var index = new PriceData();

            if (line.StartsWith("Date"))
                return null;

            try
            {
                var data = line.Split(',');

                index.Symbol = config.Symbol;

                index.Time = DateTime.ParseExact(data[0], "dd/MM/yyyy", null);
                index.EndTime = DateTime.ParseExact(data[0], "dd/MM/yyyy", null).AddDays(1);

                decimal number;

                if (decimal.TryParse(data[2], out number))
                    index.Open = Convert.ToDecimal(data[2]);

                if (decimal.TryParse(data[3], out number))
                    index.High = Convert.ToDecimal(data[3]);

                if (decimal.TryParse(data[7], out number))
                    index.Close = Convert.ToDecimal(data[7]);

                if (decimal.TryParse(data[5], out number))
                    index.Low = Convert.ToDecimal(data[5]);

                //index.Volume = Convert.ToDecimal(data[8]);
                if (index.Open == 0 && index.High == 0 && index.Low == 0 && index.Close == 0)
                    return null;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Date Invalid: {e.Message}");
            }
            return index;
        }


        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
         {
        { "Total Trades", "1" },
        { "Average Win", "0%" },
        { "Average Loss", "0%" },
        { "Compounding Annual Return", "-0.010%" },
        { "Drawdown", "0.000%" },
        { "Expectancy", "0" },
        { "Net Profit", "-0.008%" },
        { "Sharpe Ratio", "-1.183" },
        { "Probabilistic Sharpe Ratio", "0.001%" },
        { "Loss Rate", "0%" },
        { "Win Rate", "0%" },
        { "Profit-Loss Ratio", "0" },
        { "Alpha", "0" },
        { "Beta", "0" },
        { "Annual Standard Deviation", "0" },
        { "Annual Variance", "0" },
        { "Information Ratio", "-1.183" },
        { "Tracking Error", "0" },
        { "Treynor Ratio", "0" },
        { "Total Fees", "$6.00" },
        { "Estimated Strategy Capacity", "$61000000000.00" },
        { "Lowest Capacity Asset", "YESBANK UL" },
        { "Fitness Score", "0" },
        { "Kelly Criterion Estimate", "0" },
        { "Kelly Criterion Probability Value", "0" },
        { "Sortino Ratio", "-0.247" },
        { "Return Over Maximum Drawdown", "-1.104" },
        { "Portfolio Turnover", "0" },
        { "Total Insights Generated", "0" },
        { "Total Insights Closed", "0" },
        { "Total Insights Analysis Completed", "0" },
        { "Long Insight Count", "0" },
        { "Short Insight Count", "0" },
        { "Long/Short Ratio", "100%" },
        { "Estimated Monthly Alpha Value", "₹0" },
        { "Total Accumulated Estimated Alpha Value", "₹0" },
        { "Mean Population Estimated Insight Value", "₹0" },
        { "Mean Population Direction", "0%" },
        { "Mean Population Magnitude", "0%" },
        { "Rolling Averaged Population Direction", "0%" },
        { "Rolling Averaged Population Magnitude", "0%" },
        { "OrderListHash", "6cc69218edd7bd461678b9ee0c575db5" }
         };

    }
}
