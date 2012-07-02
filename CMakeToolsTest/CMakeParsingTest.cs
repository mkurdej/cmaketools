/* ****************************************************************************
 * 
 * Copyright (C) 2012 by David Golub.  All rights reserved.
 * 
 * This software is subject to the Microsoft Reciprocal License (Ms-RL).
 * A copy of the license can be found in the License.txt file included
 * in this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 * 
 * **************************************************************************/

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
            List<string> vars = CMakeParsing.ParseForVariables(
                "set(foo 1)\nset(bar 0)\n");
            Assert.AreEqual(2, vars.Count);
            Assert.AreEqual("foo", vars[0]);
            Assert.AreEqual("bar", vars[1]);
            vars = CMakeParsing.ParseForVariables(
                "SET(FOO 1)\nSET(BAR 0)\n");
            Assert.AreEqual(2, vars.Count, 2);
            Assert.AreEqual("FOO", vars[0]);
            Assert.AreEqual("BAR", vars[1]);
            vars = CMakeParsing.ParseForVariables("SET( FOO 1 )");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            vars = CMakeParsing.ParseForVariables("SET(\nFOO\n1)");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            vars = CMakeParsing.ParseForVariables(
                "option(FOO \"Description.\" ON)");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            vars = CMakeParsing.ParseForVariables(
                "aux_source_directory(dir FOO)");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            vars = CMakeParsing.ParseForVariables(
                "aux_source_directory(${DIRNAME} FOO)");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            vars = CMakeParsing.ParseForVariables(
                "get_test_property(some_test SOME_PROPERTY FOO)");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            vars = CMakeParsing.ParseForVariables(
                "add_executable(foo file1.cpp file2.cpp)");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForVariables(
                "set(${foo} ${bar})");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForVariables(
                "set(${foo} bar)");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForVariables(
                "set(foo_${bar} abc)");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForVariables(
                "set(${foo}_bar abc)");
            Assert.AreEqual(0, vars.Count);
        }

        /// <summary>
        /// Test the correctness of parsing for names of defined variables when there
        /// are syntax errors.
        /// </summary>
        [TestMethod]
        public void TestParseForVariablesErrors()
        {
            List<string> vars = CMakeParsing.ParseForVariables("set foo(bar abc)");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForVariables("set ${foo}(bar abc)");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForVariables("foo set(bar abc)");
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("bar", vars[0]);
        }

        /// <summary>
        /// Test the correctness of parsing for names of defined environment variables.
        /// </summary>
        [TestMethod]
        public void TestParseForEnvVariables()
        {
            List<string> vars = CMakeParsing.ParseForEnvVariables(
                "set(ENV{foo} abc)\nset(ENV{bar} def)");
            Assert.AreEqual(2, vars.Count);
            Assert.AreEqual("foo", vars[0]);
            Assert.AreEqual("bar", vars[1]);
            
            // Ensure that ENV is case-sensitive.
            vars = CMakeParsing.ParseForEnvVariables("set(env{foo} abc");
            Assert.AreEqual(0, vars.Count);
            
            // Ensure that SET(ENV{}) is distinguished from SET($ENV{}).
            vars = CMakeParsing.ParseForEnvVariables("set($ENV{foo} bar");
            Assert.AreEqual(0, vars.Count);

            // Ensure that ENV{} elsewhere doesn't define a variable.
            vars = CMakeParsing.ParseForEnvVariables("set(foo ENV{bar})");
            Assert.AreEqual(0, vars.Count);

            // Test handling of environment variables defined in terms of other
            // variables or environment variables.
            vars = CMakeParsing.ParseForEnvVariables("set(ENV{foo_${bar}} abc");
            Assert.AreEqual(0, vars.Count);
            vars = CMakeParsing.ParseForEnvVariables("set(ENV{foo_$ENV{bar}} abc");
            Assert.AreEqual(0, vars.Count);
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
            List<TextSpan> regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(0, regions[0].iStartLine);
            Assert.AreEqual(13, regions[0].iStartIndex);
            Assert.AreEqual(2, regions[0].iEndLine);
            Assert.AreEqual(16, regions[0].iEndIndex);

            // Test a simple macro body.
            lines.Clear();
            lines.Add("macro(foo)");
            lines.Add("set(abc def)");
            lines.Add("endmacro(foo)");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(0, regions[0].iStartLine);
            Assert.AreEqual(10, regions[0].iStartIndex);
            Assert.AreEqual(2, regions[0].iEndLine);
            Assert.AreEqual(13, regions[0].iEndIndex);

            // Test a function body with extra whitespace.
            lines.Clear();
            lines.Add("function   ( foo )");
            lines.Add("set ( abc )");
            lines.Add("endfunction ( foo )");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(0, regions[0].iStartLine);
            Assert.AreEqual(18, regions[0].iStartIndex);
            Assert.AreEqual(2, regions[0].iEndLine);
            Assert.AreEqual(19, regions[0].iEndIndex);

            // Test a function body with extra line breaks.
            lines.Clear();
            lines.Add("function(");
            lines.Add("  foo");
            lines.Add(")");
            lines.Add("set(abc)");
            lines.Add("endfunction(");
            lines.Add("  foo");
            lines.Add(")");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(2, regions[0].iStartLine);
            Assert.AreEqual(1, regions[0].iStartIndex);
            Assert.AreEqual(6, regions[0].iEndLine);
            Assert.AreEqual(1, regions[0].iEndIndex);

            // Test a malformed function body.
            lines.Clear();
            lines.Add("function foo(bar)");
            lines.Add("set(abc def)");
            lines.Add("endfunction(foo)");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(0, regions.Count);

            // Test an incomplete function body.
            lines.Clear();
            lines.Add("function(foo)");
            lines.Add("set(abc)");
            lines.Add("set(def)");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(0, regions.Count);

            // Test a function body with a double header.
            lines.Clear();
            lines.Add("function(foo)");
            lines.Add("function(bar)");
            lines.Add("set(abc)");
            lines.Add("endfunction(bar)");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(1, regions[0].iStartLine);
            Assert.AreEqual(13, regions[0].iStartIndex);
            Assert.AreEqual(3, regions[0].iEndLine);
            Assert.AreEqual(16, regions[0].iEndIndex);

            // Test a function body with a double footer.
            lines.Clear();
            lines.Add("function(foo)");
            lines.Add("set(abc)");
            lines.Add("endfunction(foo)");
            lines.Add("endfunction(bar)");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(1, regions.Count, 1);
            Assert.AreEqual(0, regions[0].iStartLine);
            Assert.AreEqual(13, regions[0].iStartIndex);
            Assert.AreEqual(2, regions[0].iEndLine);
            Assert.AreEqual(16, regions[0].iEndIndex);
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
            string identifier = CMakeParsing.ParseForIdentifier(lines, 0, 11,
                out isVariable);
            Assert.AreEqual("bar", identifier);
            Assert.IsTrue(isVariable);
            identifier = CMakeParsing.ParseForIdentifier(lines, 0, 1, out isVariable);
            Assert.IsNull(identifier);

            // Test recognition of function/macro names.
            lines.Clear();
            lines.Add("foo(bar)");
            identifier = CMakeParsing.ParseForIdentifier(lines, 0, 1, out isVariable);
            Assert.AreEqual("foo", identifier);
            Assert.IsFalse(isVariable);
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
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "foo",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(4, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(6, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration with extra whitespace.
            lines.Clear();
            lines.Add("set ( foo  bar )");
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "foo",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(6, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(8, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration spread across multiple lines.
            lines.Clear();
            lines.Add("set(");
            lines.Add("  foo");
            lines.Add("  bar");
            lines.Add(")");
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "foo",
                out span));
            Assert.AreEqual(1, span.iStartLine);
            Assert.AreEqual(2, span.iStartIndex);
            Assert.AreEqual(1, span.iEndLine);
            Assert.AreEqual(4, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
                out span));

            // Test an arbitrary command.
            lines.Clear();
            lines.Add("message(foo)");
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "foo",
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
            Assert.IsTrue(CMakeParsing.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(9, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(11, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "function",
                out span));

            // Test a macro definition.
            lines.Clear();
            lines.Add("macro(foo bar)");
            Assert.IsTrue(CMakeParsing.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(6, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(8, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines,
                "macro", out span));

            // Test a definition with extra whitespace.
            lines.Clear();
            lines.Add("function ( foo bar )");
            Assert.IsTrue(CMakeParsing.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(11, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(13, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "function",
                out span));

            // Test a definition spread across multiple lines.
            lines.Clear();
            lines.Add("function(");
            lines.Add("  foo");
            lines.Add("  bar");
            lines.Add(")");
            Assert.IsTrue(CMakeParsing.ParseForFunctionDefinition(lines, "foo",
                out span));
            Assert.AreEqual(1, span.iStartLine);
            Assert.AreEqual(2, span.iStartIndex);
            Assert.AreEqual(1, span.iEndLine);
            Assert.AreEqual(4, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "bar",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "function",
                out span));
        }
    }
}
