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
using Microsoft.VisualStudio.Package;
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
            List<string> lines = new List<string>();
            lines.Add("set(foo 1)");
            lines.Add("set(bar 0)");
            List<string> vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(2, vars.Count);
            Assert.AreEqual("foo", vars[0]);
            Assert.AreEqual("bar", vars[1]);
            lines.Clear();
            lines.Add("SET(FOO 1)");
            lines.Add("SET(BAR 0)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(2, vars.Count, 2);
            Assert.AreEqual("FOO", vars[0]);
            Assert.AreEqual("BAR", vars[1]);
            lines.Clear();
            lines.Add("SET( FOO 1 )");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("SET(");
            lines.Add("FOO");
            lines.Add("1)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("option(FOO \"Description.\" ON)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("aux_source_directory(dir FOO)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("aux_source_directory(${DIRNAME} FOO)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("get_test_property(some_test SOME_PROPERTY FOO)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("add_executable(foo file1.cpp file2.cpp)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(${foo} ${bar})");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(${foo} bar)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(foo_${bar} abc)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(${foo}_bar abc)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(8 abc)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("8", vars[0]);
            lines.Clear();
            lines.Add("set(8xy abc)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("8xy", vars[0]);
        }

        /// <summary>
        /// Test the correctness of parsing for names of defined variables when there
        /// are syntax errors.
        /// </summary>
        [TestMethod]
        public void TestParseForVariablesErrors()
        {
            List<string> lines = new List<string>();
            lines.Add("set foo(bar abc)");
            List<string> vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set ${foo}(bar abc)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("foo set(bar abc)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("bar", vars[0]);
            lines.Clear();
            lines.Add("set(foo.txt)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(8xy.txt)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set");
            lines.Add("(foo bar)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(0, vars.Count);
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
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(0, regions[0].iStartLine);
            Assert.AreEqual(13, regions[0].iStartIndex);
            Assert.AreEqual(2, regions[0].iEndLine);
            Assert.AreEqual(16, regions[0].iEndIndex);

            // Test a function body with an illegal line break.  This should fail.
            lines.Clear();
            lines.Add("function");
            lines.Add("(foo)");
            lines.Add("endfunction(foo)");
            regions = CMakeParsing.ParseForFunctionBodies(lines);
            Assert.AreEqual(0, regions.Count);
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

            // Test a declaration of a variable whose name is just a number.
            lines.Clear();
            lines.Add("set(8 abc)");
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "8", out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(4, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(4, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "abc",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration of a variable whose name starts with a number.
            lines.Clear();
            lines.Add("set(8xy abc)");
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "8xy",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(4, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(6, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "abc",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
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

        /// <summary>
        /// Test parsing for the command that triggered a parse request.
        /// </summary>
        [TestMethod]
        public void TestParseForTriggerCommandId()
        {
            List<string> lines = new List<string>();
            lines.Add("add_executable(foo foo.cpp bar.cpp)");
            Assert.AreEqual(CMakeCommandId.AddExecutable,
                CMakeParsing.ParseForTriggerCommandId(lines, 0, 14));
            lines.Clear();
            lines.Add("add_executable ( foo foo.cpp bar.cpp )");
            Assert.AreEqual(CMakeCommandId.AddExecutable,
                CMakeParsing.ParseForTriggerCommandId(lines, 0, 15));
        }

        /// <summary>
        /// Test parsing for parameter names.
        /// </summary>
        [TestMethod]
        public void TestParseForParameterNames()
        {
            List<string> lines = new List<string>();
            lines.Add("function(foo a b c)");
            lines.Add("something()");
            lines.Add("endfunction(foo)");
            lines.Add("function(bar x y z w)");
            lines.Add("something_else()");
            lines.Add("endfunction(bar)");
            List<string> result = CMakeParsing.ParseForParameterNames(lines, "foo");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
            result = CMakeParsing.ParseForParameterNames(lines, "bar");
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("x", result[0]);
            Assert.AreEqual("y", result[1]);
            Assert.AreEqual("z", result[2]);
            Assert.AreEqual("w", result[3]);
            result = CMakeParsing.ParseForParameterNames(lines, "something");
            Assert.IsNull(result);

            // Test a macro.
            lines.Clear();
            lines.Add("macro(foo a b c)");
            lines.Add("something()");
            lines.Add("endmacro(foo)");
            result = CMakeParsing.ParseForParameterNames(lines, "foo");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);

            // Test a function definition spread across multiple lines.
            lines.Clear();
            lines.Add("function(");
            lines.Add("  foo");
            lines.Add("  a # first parameter");
            lines.Add("  b # second parameter");
            lines.Add("  c)");
            result = CMakeParsing.ParseForParameterNames(lines, "foo");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);

            // Test a function definition with an illegal line break.  This should fail.
            lines.Clear();
            lines.Add("function");
            lines.Add("(foo)");
            lines.Add("endfunction(foo)");
            result = CMakeParsing.ParseForParameterNames(lines, "foo");
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test parsing for parameter information.
        /// </summary>
        [TestMethod]
        public void TestParseForParameterInfo()
        {
            List<string> lines = new List<string>();
            lines.Add("add_executable(foo foo.cpp bar.cpp)");
            CMakeParsing.ParameterInfoResult result = CMakeParsing.ParseForParameterInfo(
                lines, 0, 34);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("add_executable", result.CommandName);
            Assert.IsNull(result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(13, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(14, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(14, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(1, result.SeparatorSpans.Count);
            Assert.AreEqual(0, result.SeparatorSpans[0].iStartLine);
            Assert.AreEqual(18, result.SeparatorSpans[0].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[0].iEndLine);
            Assert.AreEqual(18, result.SeparatorSpans[0].iEndIndex);
            Assert.IsTrue(result.EndSpan.HasValue);
            Assert.AreEqual(0, result.EndSpan.Value.iStartLine);
            Assert.AreEqual(34, result.EndSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.EndSpan.Value.iEndLine);
            Assert.AreEqual(34, result.EndSpan.Value.iEndIndex);

            // Test that moving back the trigger token location prevents tokens after
            // the trigger token from being included in the results.
            result = CMakeParsing.ParseForParameterInfo(lines, 0, 14);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("add_executable", result.CommandName);
            Assert.IsNull(result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(13, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(14, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(14, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(0, result.SeparatorSpans.Count);
            Assert.IsFalse(result.EndSpan.HasValue);

            // Test a command with extra whitespace.
            lines.Clear();
            lines.Add("add_executable( foo  foo.cpp  bar.cpp )");
            result = CMakeParsing.ParseForParameterInfo(lines, 0, 38);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("add_executable", result.CommandName);
            Assert.IsNull(result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(13, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(14, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(14, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(1, result.SeparatorSpans.Count);
            Assert.AreEqual(0, result.SeparatorSpans[0].iStartLine);
            Assert.AreEqual(19, result.SeparatorSpans[0].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[0].iEndLine);
            Assert.AreEqual(20, result.SeparatorSpans[0].iEndIndex);
            Assert.IsTrue(result.EndSpan.HasValue);
            Assert.AreEqual(0, result.EndSpan.Value.iStartLine);
            Assert.AreEqual(38, result.EndSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.EndSpan.Value.iEndLine);
            Assert.AreEqual(38, result.EndSpan.Value.iEndIndex);

            // Test a user-defined function.
            lines.Clear();
            lines.Add("foo(a b c)");
            result = CMakeParsing.ParseForParameterInfo(lines, 0, 9);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("foo", result.CommandName);
            Assert.IsNull(result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(2, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(3, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(3, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(2, result.SeparatorSpans.Count);
            Assert.AreEqual(0, result.SeparatorSpans[0].iStartLine);
            Assert.AreEqual(5, result.SeparatorSpans[0].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[0].iEndLine);
            Assert.AreEqual(5, result.SeparatorSpans[0].iEndIndex);
            Assert.AreEqual(0, result.SeparatorSpans[1].iStartLine);
            Assert.AreEqual(7, result.SeparatorSpans[1].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[1].iEndLine);
            Assert.AreEqual(7, result.SeparatorSpans[1].iEndIndex);
            Assert.IsTrue(result.EndSpan.HasValue);
            Assert.AreEqual(0, result.EndSpan.Value.iStartLine);
            Assert.AreEqual(9, result.EndSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.EndSpan.Value.iEndLine);
            Assert.AreEqual(9, result.EndSpan.Value.iEndIndex);

            // Test that passing a negative number for the column causes the entire line
            // to be scanned.
            result = CMakeParsing.ParseForParameterInfo(lines, 0, -1);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("foo", result.CommandName);
            Assert.IsNull(result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(2, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(3, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(3, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(2, result.SeparatorSpans.Count);
            Assert.AreEqual(0, result.SeparatorSpans[0].iStartLine);
            Assert.AreEqual(5, result.SeparatorSpans[0].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[0].iEndLine);
            Assert.AreEqual(5, result.SeparatorSpans[0].iEndIndex);
            Assert.AreEqual(0, result.SeparatorSpans[1].iStartLine);
            Assert.AreEqual(7, result.SeparatorSpans[1].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[1].iEndLine);
            Assert.AreEqual(7, result.SeparatorSpans[1].iEndIndex);
            Assert.IsTrue(result.EndSpan.HasValue);
            Assert.AreEqual(0, result.EndSpan.Value.iStartLine);
            Assert.AreEqual(9, result.EndSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.EndSpan.Value.iEndLine);
            Assert.AreEqual(9, result.EndSpan.Value.iEndIndex);

            // Test a subcommand.
            lines.Clear();
            lines.Add("FILE(COPY foo.txt bar.txt)");
            result = CMakeParsing.ParseForParameterInfo(lines, 0, 25);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("FILE", result.CommandName);
            Assert.IsNotNull(result.SubcommandName);
            Assert.AreEqual("COPY", result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(8, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(9, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(9, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(1, result.SeparatorSpans.Count);
            Assert.AreEqual(0, result.SeparatorSpans[0].iStartLine);
            Assert.AreEqual(17, result.SeparatorSpans[0].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[0].iEndLine);
            Assert.AreEqual(17, result.SeparatorSpans[0].iEndIndex);
            Assert.IsTrue(result.EndSpan.HasValue);
            Assert.AreEqual(0, result.EndSpan.Value.iStartLine);
            Assert.AreEqual(25, result.EndSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.EndSpan.Value.iEndLine);
            Assert.AreEqual(25, result.EndSpan.Value.iEndIndex);

            // Ensure that we can parse for names only.
            result = CMakeParsing.ParseForParameterInfo(lines, 0, -1, true);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("FILE", result.CommandName);
            Assert.IsNotNull(result.SubcommandName);
            Assert.AreEqual("COPY", result.SubcommandName);
            Assert.IsFalse(result.CommandSpan.HasValue);
            Assert.IsFalse(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.SeparatorSpans.Count);
            Assert.IsFalse(result.EndSpan.HasValue);
        }

        /// <summary>
        /// Test parsing for the token at a given position.
        /// </summary>
        [TestMethod]
        public void TestParseForToken()
        {
            List<string> lines = new List<string>();
            lines.Add("SET(ABC");
            lines.Add("  X");
            lines.Add("  Y");
            lines.Add("  Z)");

            bool inParens;
            TokenInfo tokenInfo;
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 1, out tokenInfo,
                out inParens));
            Assert.IsFalse(inParens);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 4, out tokenInfo,
                out inParens));
            Assert.IsTrue(inParens);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.IsFalse(CMakeParsing.ParseForToken(lines, 0, 10, out tokenInfo,
                out inParens));
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 1, 2, out tokenInfo,
                out inParens));
            Assert.IsTrue(inParens);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(2, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.IsFalse(CMakeParsing.ParseForToken(lines, 10, 0, out tokenInfo,
                out inParens));
        }

        /// <summary>
        /// Test parsing for function and macro names.
        /// </summary>
        [TestMethod]
        public void TestParseForFunctionNames()
        {
            // Test parsing a function declaration.
            List<string> lines = new List<string>();
            lines.Add("function(foo)");
            List<string> functions = CMakeParsing.ParseForFunctionNames(lines, false);
            Assert.IsNotNull(functions);
            Assert.AreEqual(1, functions.Count);
            Assert.AreEqual("foo", functions[0]);
            functions = CMakeParsing.ParseForFunctionNames(lines, true);
            Assert.IsNotNull(functions);
            Assert.AreEqual(0, functions.Count);

            // Test parsing a macro declaration.
            lines.Clear();
            lines.Add("macro(bar)");
            functions = CMakeParsing.ParseForFunctionNames(lines, false);
            Assert.IsNotNull(functions);
            Assert.AreEqual(0, functions.Count);
            functions = CMakeParsing.ParseForFunctionNames(lines, true);
            Assert.IsNotNull(functions);
            Assert.AreEqual(1, functions.Count);
            Assert.AreEqual("bar", functions[0]);

            // Test parsing a declaration with extra whitespace.
            lines.Clear();
            lines.Add("function ( foo )");
            functions = CMakeParsing.ParseForFunctionNames(lines, false);
            Assert.IsNotNull(functions);
            Assert.AreEqual(1, functions.Count);
            Assert.AreEqual("foo", functions[0]);

            // Test parsing a declaration with line breaks and comments.
            lines.Clear();
            lines.Add("function( # comment");
            lines.Add("  foo)");
            functions = CMakeParsing.ParseForFunctionNames(lines, false);
            Assert.IsNotNull(functions);
            Assert.AreEqual(1, functions.Count);
            Assert.AreEqual("foo", functions[0]);

            // Test parsing a declaration with an illegal line break.  This should fail.
            lines.Clear();
            lines.Add("function");
            lines.Add("(foo)");
            functions = CMakeParsing.ParseForFunctionNames(lines, false);
            Assert.IsNotNull(functions);
            Assert.AreEqual(0, functions.Count);
        }
    }
}
