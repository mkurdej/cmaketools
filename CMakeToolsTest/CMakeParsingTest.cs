// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using CMakeTools;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
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
                "aux_source_directory(${DIRNAME} FOO)");
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
            vars = CMakeLanguageService.ParseForVariables(
                "set(${foo} bar)");
            Assert.AreEqual(vars.Count, 0);
            vars = CMakeLanguageService.ParseForVariables(
                "set(foo_${bar} abc)");
            Assert.AreEqual(vars.Count, 0);
            vars = CMakeLanguageService.ParseForVariables(
                "set(${foo}_bar abc)");
            Assert.AreEqual(vars.Count, 0);
        }

        /// <summary>
        /// Test the correctness of parsing for names of defined variables when there
        /// are syntax errors.
        /// </summary>
        [TestMethod]
        public void TestParseForVariablesErrors()
        {
            List<string> vars = CMakeLanguageService.ParseForVariables(
                "set foo(bar abc)");
            Assert.AreEqual(vars.Count, 0);
            vars = CMakeLanguageService.ParseForVariables("set ${foo}(bar abc)");
            Assert.AreEqual(vars.Count, 0);
            vars = CMakeLanguageService.ParseForVariables("foo set(bar abc)");
            Assert.AreEqual(vars.Count, 1);
            Assert.AreEqual(vars[0], "bar");
        }

        /// <summary>
        /// Test the correctness of parsing for names of defined environment variables.
        /// </summary>
        [TestMethod]
        public void TestParseForEnvVariables()
        {
            List<string> vars = CMakeLanguageService.ParseForEnvVariables(
                "set(ENV{foo} abc)\nset(ENV{bar} def)");
            Assert.AreEqual(vars.Count, 2);
            Assert.AreEqual(vars[0], "foo");
            Assert.AreEqual(vars[1], "bar");
            
            // Ensure that ENV is case-sensitive.
            vars = CMakeLanguageService.ParseForEnvVariables("set(env{foo} abc");
            Assert.AreEqual(vars.Count, 0);
            
            // Ensure that SET(ENV{}) is distinguished from SET($ENV{}).
            vars = CMakeLanguageService.ParseForEnvVariables("set($ENV{foo} bar");
            Assert.AreEqual(vars.Count, 0);

            // Ensure that ENV{} elsewhere doesn't define a variable.
            vars = CMakeLanguageService.ParseForEnvVariables("set(foo ENV{bar})");
            Assert.AreEqual(vars.Count, 0);

            // Test handling of environment variables defined in terms of other
            // variables or environment variables.
            vars = CMakeLanguageService.ParseForEnvVariables("set(ENV{foo_${bar}} abc");
            Assert.AreEqual(vars.Count, 0);
            vars = CMakeLanguageService.ParseForEnvVariables(
                "set(ENV{foo_$ENV{bar}} abc");
            Assert.AreEqual(vars.Count, 0);
        }

        /// <summary>
        /// Test parsing for function bodies.
        /// </summary>
        [TestMethod]
        public void TestParseForFunctionBodies()
        {
            // Test a simple function body.
            List<string> lines = new List<string>();
            lines.Add("function(foo)");
            lines.Add("set(abc def)");
            lines.Add("endfunction(foo)");
            List<TextSpan> regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 1);
            Assert.AreEqual(regions[0].iStartLine, 0);
            Assert.AreEqual(regions[0].iStartIndex, 13);
            Assert.AreEqual(regions[0].iEndLine, 2);
            Assert.AreEqual(regions[0].iEndIndex, 16);

            // Test a simple macro body.
            lines.Clear();
            lines.Add("macro(foo)");
            lines.Add("set(abc def)");
            lines.Add("endmacro(foo)");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 1);
            Assert.AreEqual(regions[0].iStartLine, 0);
            Assert.AreEqual(regions[0].iStartIndex, 10);
            Assert.AreEqual(regions[0].iEndLine, 2);
            Assert.AreEqual(regions[0].iEndIndex, 13);

            // Test a function body with extra whitespace.
            lines.Clear();
            lines.Add("function   ( foo )");
            lines.Add("set ( abc )");
            lines.Add("endfunction ( foo )");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 1);
            Assert.AreEqual(regions[0].iStartLine, 0);
            Assert.AreEqual(regions[0].iStartIndex, 18);
            Assert.AreEqual(regions[0].iEndLine, 2);
            Assert.AreEqual(regions[0].iEndIndex, 19);

            // Test a function body with extra line breaks.
            lines.Clear();
            lines.Add("function(");
            lines.Add("  foo");
            lines.Add(")");
            lines.Add("set(abc)");
            lines.Add("endfunction(");
            lines.Add("  foo");
            lines.Add(")");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 1);
            Assert.AreEqual(regions[0].iStartLine, 2);
            Assert.AreEqual(regions[0].iStartIndex, 1);
            Assert.AreEqual(regions[0].iEndLine, 6);
            Assert.AreEqual(regions[0].iEndIndex, 1);

            // Test a malformed function body.
            lines.Clear();
            lines.Add("function foo(bar)");
            lines.Add("set(abc def)");
            lines.Add("endfunction(foo)");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 0);

            // Test an incomplete function body.
            lines.Clear();
            lines.Add("function(foo)");
            lines.Add("set(abc)");
            lines.Add("set(def)");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 0);

            // Test a function body with a double header.
            lines.Clear();
            lines.Add("function(foo)");
            lines.Add("function(bar)");
            lines.Add("set(abc)");
            lines.Add("endfunction(bar)");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 1);
            Assert.AreEqual(regions[0].iStartLine, 1);
            Assert.AreEqual(regions[0].iStartIndex, 13);
            Assert.AreEqual(regions[0].iEndLine, 3);
            Assert.AreEqual(regions[0].iEndIndex, 16);

            // Test a function body with a double footer.
            lines.Clear();
            lines.Add("function(foo)");
            lines.Add("set(abc)");
            lines.Add("endfunction(foo)");
            lines.Add("endfunction(bar)");
            regions = CMakeLanguageService.ParseForFunctionBodies(lines);
            Assert.AreEqual(regions.Count, 1);
            Assert.AreEqual(regions[0].iStartLine, 0);
            Assert.AreEqual(regions[0].iStartIndex, 13);
            Assert.AreEqual(regions[0].iEndLine, 2);
            Assert.AreEqual(regions[0].iEndIndex, 16);
        }
    }
}
