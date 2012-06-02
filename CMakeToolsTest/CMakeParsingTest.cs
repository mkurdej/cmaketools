// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using CMakeTools;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMakeTools
{
    /// <summary>
    /// Tests of parsing operations on CMake code.
    /// </summary>
    [TestClass]
    public class CMakeParsingTest
    {
        /// <summary>
        /// Test the correctness parsing for names of defined variables.
        /// </summary>
        [TestMethod]
        public void TestParseForVariables()
        {
            List<string> vars = CMakeLanguageService.ParseForVariables(
                "set(foo 1)\nset(bar 0)\n");
            Assert.AreEqual(vars.Count, 2);
            Assert.AreEqual(vars[0], "foo");
            Assert.AreEqual(vars[1], "bar");
            vars = CMakeLanguageService.ParseForVariables(
                "SET(FOO 1)\nSET(BAR 0)\n");
            Assert.AreEqual(vars.Count, 2);
            Assert.AreEqual(vars[0], "FOO");
            Assert.AreEqual(vars[1], "BAR");
            vars = CMakeLanguageService.ParseForVariables("SET( FOO 1 )");
            Assert.AreEqual(vars.Count, 1);
            Assert.AreEqual(vars[0], "FOO");
            vars = CMakeLanguageService.ParseForVariables("SET(\nFOO\n1)");
            Assert.AreEqual(vars.Count, 1);
            Assert.AreEqual(vars[0], "FOO");
            vars = CMakeLanguageService.ParseForVariables(
                "option(FOO \"Description.\" ON)");
            Assert.AreEqual(vars.Count, 1);
            Assert.AreEqual(vars[0], "FOO");
            vars = CMakeLanguageService.ParseForVariables(
                "aux_source_directory(dir FOO)");
            Assert.AreEqual(vars.Count, 1);
            Assert.AreEqual(vars[0], "FOO");
            vars = CMakeLanguageService.ParseForVariables(
                "get_test_property(some_test SOME_PROPERTY FOO)");
            Assert.AreEqual(vars.Count, 1);
            Assert.AreEqual(vars[0], "FOO");
            vars = CMakeLanguageService.ParseForVariables(
                "add_executable(foo file1.cpp file2.cpp)");
            Assert.AreEqual(vars.Count, 0);
            vars = CMakeLanguageService.ParseForVariables(
                "set(${foo} ${bar})");
            Assert.AreEqual(vars.Count, 0);
        }
    }
}
