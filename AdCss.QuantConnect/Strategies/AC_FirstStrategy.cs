using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QLNet;
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Indicators;
using QuantConnect.Interfaces;
using QuantConnect.Orders;
using QuantConnect.Orders.Fees;
using QuantConnect.Securities;
using QuantConnect.Algorithm.Framework;
using QuantConnect.Indicators;
using QuantConnect;
using AdCss.QC.Csv;
using AdCss.QC.Data;
using QuantConnect.Algorithm;
using QuantConnect;

namespace AdCss.QC.Strategies
{
    public class AC_FirstStrategy : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        ///
        HashSet<string> DataTickers;

        public static string StrategyDescription { get; }



        //private Dictionary<string, Dictionary<DateTime, SMAP_Data>> Smap_Score;

        public override void Initialize()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            SetStartDate(2015, 01, 01); //Set Start Date
            var endDate = new DateTime(2022, 08, 20);
            SetEndDate(endDate);
            SetCash(1_000_000); //Set Strategy Cash

            DataTickers = CsvGenerator.GetIndexComposition("^FCHI"); // Get securities of CAC40
            DataTickers.Add("^FCHI"); // CAC40

            // "^STOXX50E" Eurostoxx <= à ajouter pour les prochaines fois


            foreach (var ticker in DataTickers)
                AddData<PriceData>(ticker, Resolution.Daily);

            var cac40 = AddData<PriceData>("^FCHI", Resolution.Daily);
            SetBenchmark(x => cac40.Price);

            Debug("Intialization Done");
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="data">Slice object keyed by symbol containing the stock data</param>
        public override void OnData(Slice data)
        {
            var date = Time.Date; // Fais gaffe mirey les date sont affiché en mm/dd/yyyy
            var portfolio8 = Portfolio;
            var securities = Securities;



            if (!Portfolio.Invested)
            {
                SetHoldings("ENGI.PA", 0.5);  
            }


            #region old

            //if (PreviousMOnth != Time.Month)
            //{
            //    var lastReleaseSmapDate = ReleaseDates.Where(d => d <= Time.Date).Max(i => i);
            //    var potentialTarget = new HashSet<string>();

            //    if (data.Keys.Count < 100)
            //        return;

            //    foreach (var ticker in data)
            //    {
            //        if (Smap_Score.ContainsKey(ticker.Key.Value))
            //        {
            //            var smapScoreList = Smap_Score[ticker.Key.Value];

            //            if (smapScoreList.ContainsKey(lastReleaseSmapDate))
            //            {
            //                var smapScore = smapScoreList[lastReleaseSmapDate];

            //                double? mktCap = string.IsNullOrEmpty(smapScore.MarketCap) ? null : Convert.ToDouble(smapScore.MarketCap);

            //                double SectorMomentum;
            //                double analystRecommandation;

            //                var MomSucced = double.TryParse(smapScore.MaSectorMomentum, out SectorMomentum);
            //                var RecoSucced = double.TryParse(smapScore.AnalystRecomandationAvg, out analystRecommandation);

            //                if (mktCap is null || mktCap < 300_000_000)
            //                    continue;

            //                if (MomSucced == false || RecoSucced == false)
            //                    continue;


            //                if (smapScore.Smap >= 0.90 && SectorMomentum > 0.90 && analystRecommandation >= 0.8)
            //                {
            //                    potentialTarget.Add(ticker.Key.Value);
            //                }
            //            }
            //        }
            //    }

            //    Log($"Time : {date.ToString("d")}, Available securities: {data.Keys.Count}");

            //    var equalWeight = Convert.ToDouble(100) / Convert.ToDouble(5) * 0.01;

            //    var CurrentPosition = Portfolio.Securities.Where(i => i.Value.HoldStock == true).ToList();
            //    var positionstoLiquidate = new List<string>();

            //    foreach (var pos in CurrentPosition)
            //    {
            //        if (!potentialTarget.Contains(pos.Key.Value))
            //        {
            //            positionstoLiquidate.Add(pos.Key.Value);
            //        }
            //    }

            //    foreach (var position in positionstoLiquidate)
            //    {
            //        var quantitytoTrade = CalculateOrderQuantity(position, 0.0);
            //        var feesUSD = Math.Abs(quantitytoTrade * securities[position].Price * tradingFees);
            //        securities[position].FeeModel = new ConstantFeeModel(feesUSD);

            //        Liquidate(position);
            //    }


            //    foreach (var target in potentialTarget)
            //    {
            //        var quantitytoTrade = CalculateOrderQuantity(target, equalWeight);
            //        var feesUSD = quantitytoTrade * securities[target].Price * tradingFees;
            //        securities[target].FeeModel = new ConstantFeeModel(feesUSD);

            //        SetHoldings(target, equalWeight);
            //    }

            //    PreviousMOnth = Time.Month;
            //}


            //var CurrentPosition2 = Portfolio.Securities.Where(i => i.Value.HoldStock == true).ToList();

            //if (CurrentPosition2.Count == 0)
            //{
            //    Console.WriteLine("stop");
            //} 
            #endregion
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            if (orderEvent.Status.IsFill())
            {
                Console.WriteLine($"Trade {orderEvent.Symbol} ({orderEvent.Direction}) =>price={orderEvent.FillPrice} , qty = {orderEvent.Quantity} , tradingfees = {orderEvent.OrderFee.Value.Amount}({Time.Date:d}) ");
                var fee = Portfolio[orderEvent.Symbol].TotalFees;
            }

            if (orderEvent.OrderFee.Value.Amount != 0)
            {

            }
        }

    

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp };

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
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

        public long DataPoints { get; }

        public int AlgorithmHistoryDataPoints { get; }
    }
}
