using System;
using System.Collections.Generic;
using System.Text;

namespace Easy_CLI_Parser
{
    internal class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {

        }
    }
}
