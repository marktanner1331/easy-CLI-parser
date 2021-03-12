using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class CLIParserTest2
    {
        private static bool hasRun;
        private static bool argsAreCorrect;
        private static Guid testGuid;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;
            testGuid = Guid.NewGuid();

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest2Command", "-param=" + testGuid.ToString());

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [TestMethod]
        public void test2()
        {
            hasRun = false;
            argsAreCorrect = false;
            testGuid = Guid.NewGuid();

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest2Command", "--param=" + testGuid.ToString());

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [TestMethod]
        public void test3()
        {
            hasRun = false;
            argsAreCorrect = false;
            testGuid = Guid.NewGuid();

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest2Command", "/param=" + testGuid.ToString());

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [TestMethod]
        public void test4()
        {
            hasRun = false;
            argsAreCorrect = false;
            testGuid = Guid.NewGuid();

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("CLIParserTest2Command", "~param=" + testGuid.ToString());

            Assert.IsFalse(hasRun, "has run command when it shouldnt");
        }

        [CLIMethod]
        private static void CLIParserTest2Command(Guid param)
        {
            hasRun = true;
            argsAreCorrect = testGuid == param;
        }
    }
}