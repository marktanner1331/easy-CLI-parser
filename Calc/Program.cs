using Easy_CLI_Parser;
using System;

namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            CLIParser.run();
        }

        [CLIMethod]
        [CLIDescription("a really long test description that should go really far past the end of the console edge and wrap onto the next line i hope")]
        static string test(string value)
        {
            return value;
        }

        [CLIMethod]
        [CLIShortName("+")]
        [CLIExample("add 1 2")]
        [CLIExample("+ 1.5 -4.8")]
        [CLIDescription("Adds two numbers together")]
        static double add(double value1, double value2)
        {
            return value1 + value2;
        }

        [CLIMethod]
        [CLIShortName("sub")]
        [CLIShortName("-")]
        [CLIExample("subtract 2 1")]
        [CLIExample("- 1.5 1")]
        static double subtract(double value1, double value2)
        {
            return value1 - value2;
        }

        [CLIMethod]
        [CLIShortName("tc")]
        [CLIExample("truncate 1.5")]
        [CLIDescription("Returns the integral part of the provided number")]
        static int truncate(double value)
        {
            return (int)Math.Truncate(value);
        }
    }
}
