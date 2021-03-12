using Microsoft.VisualStudio.TestTools.UnitTesting;
using Easy_CLI_Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Easy_CLI_Parser.Tests
{
    [TestClass]
    public class FilePathTest
    {
        private static bool hasRun;
        private static bool argsAreCorrect;

        [TestMethod]
        public void test1()
        {
            hasRun = false;
            argsAreCorrect = false;

            CLIParser.setCustomAssembly(Assembly.GetExecutingAssembly());

            CLIParser.run("FilePathTestCommand", "C:\\Users\\Mark Tanner\\Downloads\\PublishDotNetUniversalTester (1)\\PublishDotNetUniversalTester\\bin\\UniversalTester.dll");

            Assert.IsTrue(hasRun, "has not run command");
            Assert.IsTrue(argsAreCorrect, "args are not correct");
        }

        [CLIMethod]
        private static void FilePathTestCommand(FileInfo fileInfo)
        {
            hasRun = true;
            argsAreCorrect = fileInfo != null;
        }
    }
}