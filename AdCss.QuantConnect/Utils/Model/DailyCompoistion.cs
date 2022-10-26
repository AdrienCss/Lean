using QuantConnect;
using QuantConnect.Algorithm.Framework.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace AdCss.QC.Utils.Model
{
    public class compoDaily
    {
        public string name { get; set; }
        public string bbgCode { get; set; }
        public string ric { get; set; }
        public DateTime compositionDate { get; set; }
        public List<instrument> components { get; set; }


        public List<string> GetCompoTickersOnly(bool bbgCode)
        {

            var listTickers = new List<string>();

            if (bbgCode)
                listTickers = components.Select(i => i.bbgCode).ToList(); 
            else
                listTickers = components.Select(i => i.ric).ToList();

            return listTickers;
        }

        public void GetMembYahooFinanceTicker()
        {
            var mappingFolder = Path.Combine(Globals.DataFolder, "index", "AdCss", "Members", "Mapping");


            foreach( var memb in components)
            {
                memb.YahooFinanceTicker = "YahooTickerTaMere"; 
            }
        }
    }


    public class instrument
    {
        public string bbgCode { get; set; }
        public string ric { get; set; }
        public string currency { get; set; }
        public int volume { get; set; }
        public double weightCoefficient { get; set; }
        public double currencyExchangeRate { get; set; }
        public string YahooFinanceTicker { get; set; }


    
    }
}
