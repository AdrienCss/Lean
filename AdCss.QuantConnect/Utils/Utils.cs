using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using QuantConnect.Configuration;


namespace AdCss.QC.Utils
{
    public static class Utils
    {

        public static void WhriteStrategyDescriptionInConfigFile(string strategyDescription)
        {
            Config.Set("strategy-description", strategyDescription);
        }
    }
}
