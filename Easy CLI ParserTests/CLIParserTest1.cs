using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class CLIParserTest1
    {
        private static bool success;

        [TestMethod]
        public void test1()
        {
            success = false;
            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest1Command");

            Assert.IsTrue(success);
        }

        [CLIMethod]
        private static void CLIParserTest1Command()
        {
            success = true;
        }
    }
}