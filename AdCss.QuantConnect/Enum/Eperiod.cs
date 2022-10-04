using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdCss.QC.Enum
{
    public enum Eperiod
    {
        None = 0,
        [Description("1y")]
        _1y,
        [Description("5y")]
        _5y,
        [Description("10y")]
        _10y 
    }
}
