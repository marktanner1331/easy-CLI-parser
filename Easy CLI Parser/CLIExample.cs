using System;
using System.Collections.Generic;
using System.Text;

namespace Easy_CLI_Parser
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CLIExample : Attribute
    {
        public readonly string Example;

        public CLIExample(string example)
        {
            Example = example;
        }
    }
}
