using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDesignPlus.Net.Logger.Options
{
    public class LoggerOptions
    {
        public const string Section = "Logger";

        public bool Enable { get; set; }

        public string OTelEndpoint { get; set; }
    }
}
