using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class CLIParserTest3
    {
        private static bool hasRun;
        private static bool argsAreCorrect;
        private static int testInt;
        private static string testString;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;
            testInt = 5;
            testString = "hello";

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest3Command", "-param1=" + testString, "-param2=" + testInt);

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void CLIParserTest3Command(string param1, int param2)
        {
            hasRun = true;
            argsAreCorrect = param1 == testString && param2 == testInt;
        }
    }
}