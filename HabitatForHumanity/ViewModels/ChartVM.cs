using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class ChartVM
    {
        public string _title { get; set; }
        public string _subtitle { get; set; }
        public string[] _categories { get; set; }
        public _Series[] _series { get; set; }
        public ChartVM()
        {
            _title = "My title";
            _subtitle = "sub title";
            _categories = new string[] { "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012" };
            _series = new _Series[3];
        
            _series[0] = (new _Series("Home Builds", new int[] { 812, 412, 628, 142, 460, 972, 204, 513, 315 }));
            _series[1] = (new _Series("Re-Store", new int[] { 312, 427, 928, 425, 660, 572, 604, 713, 115 }));
            _series[2] = (new _Series("ABWK", new int[] { 190, 895, 821, 113, 197, 198, 600, 764, 524 }));
        }
        public class _Series
        {
            public string _name { get; set; }
            public object[] _data { get; set; }

            public _Series(string name, int[] values)
            {
                _name = name;
                _data = new object[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    _data[i] = values[i];
                }
            }
        }

    }
}