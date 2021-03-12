using System;
using System.Collections.Generic;
using System.Text;

namespace Easy_CLI_Parser
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CLIShortName : Attribute
    {
        public readonly string ShortName;

        public CLIShortName(string shortName)
        {
            ShortName = shortName;
        }
    }
}
