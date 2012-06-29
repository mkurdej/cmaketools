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

        /// <summary>
        /// Test parsing for identifiers at given locations.
        /// </summary>
        [TestMethod]
        public void TestParseForIdentifiers()
        {
            // Test recognition of variable names.
            List<string> lines = new List<string>();
            bool isVariable = false;
            lines.Add("set(foo ${bar}");
            string identifier = CMakeLanguageService.ParseForIdentifier(lines, 0, 11,
                out isVariable);
            Assert.AreEqual(identifier, "bar");
            Assert.AreEqual(isVariable, true);
            identifier = CMakeLanguageService.ParseForIdentifier(lines, 0, 1,
                out isVariable);
            Assert.AreEqual(identifier, null);

            // Test recognition of function/macro names.
            lines.Clear();
            lines.Add("foo(bar)");
            identifier = CMakeLanguageService.ParseForIdentifier(lines, 0, 1,
                out isVariable);
            Assert.AreEqual(identifier, "foo");
            Assert.AreEqual(isVariable, false);
        }

        /// <summary>
        /// Test parsing for the definition of a given variable.
        /// </summary>
        [TestMethod]
        public void TestParseForVariableDefinition()
        {
            List<string> lines = new List<string>();
            TextSpan span = new TextSpan();
            lines.Add("set(foo bar)");
            Assert.IsTrue(CMakeLanguageService.ParseForVariableDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 0);
            Assert.AreEqual(span.iStartIndex, 4);
            Assert.AreEqual(span.iEndLine, 0);
            Assert.AreEqual(span.iEndIndex, 6);
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration with extra whitespace.
            lines.Clear();
            lines.Add("set ( foo  bar )");
            Assert.IsTrue(CMakeLanguageService.ParseForVariableDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 0);
            Assert.AreEqual(span.iStartIndex, 6);
            Assert.AreEqual(span.iEndLine, 0);
            Assert.AreEqual(span.iEndIndex, 8);
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration spread across multiple lines.
            lines.Clear();
            lines.Add("set(");
            lines.Add("  foo");
            lines.Add("  bar");
            lines.Add(")");
            Assert.IsTrue(CMakeLanguageService.ParseForVariableDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 1);
            Assert.AreEqual(span.iStartIndex, 2);
            Assert.AreEqual(span.iEndLine, 1);
            Assert.AreEqual(span.iEndIndex, 4);
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "set",
                out span));

            // Test an arbitrary command.
            lines.Clear();
            lines.Add("message(foo)");
            Assert.IsFalse(CMakeLanguageService.ParseForVariableDefinition(lines, "foo",
                out span));
        }

        /// <summary>
        /// Test parsing for the definition of a given function or macro.
        /// </summary>
        [TestMethod]
        public void TestParseForFunctionDefinition()
        {
            // Test a function definition.
            List<string> lines = new List<string>();
            TextSpan span = new TextSpan();
            lines.Add("function(foo bar)");
            Assert.IsTrue(CMakeLanguageService.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 0);
            Assert.AreEqual(span.iStartIndex, 9);
            Assert.AreEqual(span.iEndLine, 0);
            Assert.AreEqual(span.iEndIndex, 11);
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines,
                "function", out span));

            // Test a macro definition.
            lines.Clear();
            lines.Add("macro(foo bar)");
            Assert.IsTrue(CMakeLanguageService.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 0);
            Assert.AreEqual(span.iStartIndex, 6);
            Assert.AreEqual(span.iEndLine, 0);
            Assert.AreEqual(span.iEndIndex, 8);
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines,
                "macro", out span));

            // Test a definition with extra whitespace.
            lines.Clear();
            lines.Add("function ( foo bar )");
            Assert.IsTrue(CMakeLanguageService.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 0);
            Assert.AreEqual(span.iStartIndex, 11);
            Assert.AreEqual(span.iEndLine, 0);
            Assert.AreEqual(span.iEndIndex, 13);
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines,
                "function", out span));

            // Test a definition spread across multiple lines.
            lines.Clear();
            lines.Add("function(");
            lines.Add("  foo");
            lines.Add("  bar");
            lines.Add(")");
            Assert.IsTrue(CMakeLanguageService.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(span.iStartLine, 1);
            Assert.AreEqual(span.iStartIndex, 2);
            Assert.AreEqual(span.iEndLine, 1);
            Assert.AreEqual(span.iEndIndex, 4);
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeLanguageService.ParseForFunctionDefinition(lines,
                "function", out span));
        }
    }
}
