using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class QuotedValueTest
    {
        private static bool hasRun;
        private static bool argsAreCorrect;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("QuotedValueTestCommand", "-param1=\"cats\"");

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void QuotedValueTestCommand(string param1)
        {
            hasRun = true;
            argsAreCorrect = param1 == "cats";
        }
    }
}