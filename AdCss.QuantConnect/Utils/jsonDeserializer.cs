using AdCss.QC.Utils.Model;
using QuantConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace AdCss.QC.Utils
{
    /// <summary>
    /// Export
    /// </summary>
    public  class jsonDeserializer
    {
        public static List<compoDaily> GetSX5EComposition()
        {
            var dataFolder  = Path.Combine(Globals.DataFolder, "index", "AdCss", "Members");
            var compoList = new List<compoDaily>();

            foreach (var jsonFile in Directory.GetFiles(dataFolder))
            {
                var compo = File.ReadAllText(jsonFile);
                var yearlyComp = JsonSerializer.Deserialize<List<compoDaily>>(compo);
           //     yearlyComp.GetYahooTicker();
                compoList.AddRange(yearlyComp);
            }
            return compoList;
        }
    }
}
