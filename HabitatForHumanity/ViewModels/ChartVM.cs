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

        public ChartVM(string title, string[] cats, int[] restoreHrs, int[] awbkHrs, int[] homesHrs)
        {
            _title = title;
            _subtitle = "";
            _categories = cats;
            _series = new _Series[3];
            _series[0] = (new _Series("Re-Store", restoreHrs));
            _series[1] = (new _Series("ABWK", awbkHrs));
            _series[2] = (new _Series("Home Builds", homesHrs));
          
           
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