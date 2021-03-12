using System;
using System.Collections.Generic;
using System.Text;

namespace Easy_CLI_Parser
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CLIDescription : Attribute
    {
        public readonly string Description;

        public CLIDescription(string description)
        {
            Description = description;
        }
    }
}
