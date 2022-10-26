using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuantConnect.Data;
using QuantConnect.Indicators;
using QuantConnect.Interfaces;
using QuantConnect.Orders;
using QuantConnect.Interfaces;
using QuantConnect.Algorithm;
using QuantConnect;
using AdCss.QC.Csv;
using AdCss.QC.Data;

namespace AdCss.QC.Strategies
{
    public class ci_TrendFollowingStrategies : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        /// 
        private MomentumPercent aaplMomentum;
        private MomentumPercent spidMomentum;

        public override void Initialize()
        {
            SetStartDate(2010, 8, 1);
            SetEndDate(2022, 01, 1);
            SetCash(1000000);





           var DataTickers = CsvGenerator.GetIndexComposition("^FCHI"); // Get securities of CAC40
            DataTickers.Add("^FCHI"); // CAC40

            // "^STOXX50E" Eurostoxx <= à ajouter pour les prochaines fois
            //     DataTickers = new HashSet<string>() { "OR.PA" };

            foreach (var ticker in DataTickers)
                AddData<PriceData>(ticker, Resolution.Daily);

         //   var cac40 = AddData<PriceData>("^FCHI", Resolution.Daily);
          //  SetBenchmark(x => cac40.Price);






            //AddEquity("spi", Resolution.Daily);
            //AddEquity("aapl", Resolution.Daily);

            //// S&P 500 ETF
            //aaplMomentum = MOMP("aapl", 50, Resolution.Daily);

            //// Vanguard Total Bond Market
            //spidMomentum = MOMP("spi", 50, Resolution.Daily);

          //  SetBenchmark("spy");
            
          //  SetWarmUp(50);
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="data">Slice object keyed by symbol containing the stock data</param>
        

        public override void OnData(Slice data)
        {
            var dateNow = Time.Date; // 21/05/2010, logiquement ca appartait 50 j apres

            if (IsWarmingUp)
                return;


            if (aaplMomentum == null || !aaplMomentum.IsReady)
            {
                Log("Ooutch your indicator on SPY is not ready");
                return;
            }
                

            if (spidMomentum == null || !spidMomentum.IsReady)
            {
                Log("Ooutch your indicator on Bond is not ready");
                return;
            }


          
            // Time Properties allow to know the current Time
            // We can use it here to limit trading activities, when for example it is friday
            if (Time.DayOfWeek == DayOfWeek.Wednesday)
            {
                return;
            }
            //1. If SPY has more upward momentum than BND, then we liquidate our holdings in BND and allocate 100% of our equity to SPY
            if (aaplMomentum > spidMomentum)
            {
                Liquidate("spi");
                SetHoldings("aapl", 1.0);
            }
            else
            {
                Liquidate("aapl");
                SetHoldings("spi", 1.0);
            }
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            if (orderEvent.Status.IsFill())
            {
                Debug($"Purchased Complete: {orderEvent.Symbol}");

                Console.WriteLine($"Trade Passé biatch !!!!! {orderEvent.Symbol} =>  {orderEvent.Quantity}");
            }

            if (orderEvent.Status.IsOpen())
            {
                Debug($"New oder Opened => : {orderEvent.Symbol}");
                Debug("Order direction is {}");
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp, Language.Python };

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

        public long DataPoints => throw new NotImplementedException();

        public int AlgorithmHistoryDataPoints => throw new NotImplementedException();
    }
}
