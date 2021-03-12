using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class CLIParserTest5
    {
        private static bool hasRun;
        private static bool argsAreCorrect;
        private static bool testBool;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;
            testBool = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest5Command", "-flag1");

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void CLIParserTest5Command(bool flag1)
        {
            hasRun = true;
            argsAreCorrect = flag1 == true;
        }

        [TestMethod]
        public void test2()
        {
            hasRun = false;
            argsAreCorrect = false;
            testBool = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest5Command2", "-flag1");

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void CLIParserTest5Command2(bool flag1 = true)
        {
            hasRun = true;
            argsAreCorrect = flag1 == false;
        }

        [TestMethod]
        public void test3()
        {
            hasRun = false;
            argsAreCorrect = false;
            testBool = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest5Command3");

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void CLIParserTest5Command3(bool flag1 = false)
        {
            hasRun = true;
            argsAreCorrect = flag1 == false;
        }
    }
}