using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class SpaceSeperatedArgTest
    {
        private static bool hasRun;
        private static bool argsAreCorrect;
        private static int testInt;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;
            testInt = 5;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("SpaceSeperatedArgTestCommand", "-param1", testInt.ToString());

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void SpaceSeperatedArgTestCommand(int param1)
        {
            hasRun = true;
            argsAreCorrect = param1 == testInt;
        }
    }
}