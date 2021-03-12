using System;
using System.Collections.Generic;
using System.Text;

namespace Easy_CLI_Parser
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CLIMethod : Attribute
    {
       
    }
}
