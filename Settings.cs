using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Timer
{
    static class Settings
    {
        // the default time for the timer
        public static TimeSpan DefaultTime
        {
            get { return (TimeSpan)Properties.Settings.Default["defaultTime"]; }
            set { Properties.Settings.Default["defaultTime"] = value; }
        }

        public static TimeSpan TimeStep { get { return TimeSpan.FromMinutes(5); } }
    }
}
