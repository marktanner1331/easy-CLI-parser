using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class QuickVariableNameTest
    {
        private static bool hasRun;
        private static bool argsAreCorrect;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("QuickVariableNameTestCommand", "param1=cats");

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [TestMethod]
        public void test2()
        {
            hasRun = false;
            argsAreCorrect = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("QuickVariableNameTestCommand", "param1=cats", "param2");

            //in this case we dont want it to run, as switches should start with an escape character
            Assert.IsFalse(hasRun, "has run command when it shouldnt");
        }

        [CLIMethod]
        private static void QuickVariableNameTestCommand(string param1, bool param2)
        {
            hasRun = true;
            argsAreCorrect = param1 == "cats";
        }
    }
}