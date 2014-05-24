/* ****************************************************************************
 * 
 * Copyright (C) 2012-2014 by David Golub.  All rights reserved.
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
        /// Test the correctness of parsing for names of defined variables.
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

            // Test parsing for local variables.
            lines.Clear();
            lines.Add("set(a)");
            lines.Add("function(f)");
            lines.Add("set(local1)");
            lines.Add("set(local2)");
            lines.Add("endfunction(f)");
            lines.Add("set(b)");
            vars = CMakeParsing.ParseForVariables(lines);
            Assert.AreEqual(2, vars.Count);
            Assert.AreEqual("a", vars[0]);
            Assert.AreEqual("b", vars[1]);
            vars = CMakeParsing.ParseForVariables(lines, 4);
            Assert.AreEqual(3, vars.Count);
            Assert.AreEqual("a", vars[0]);
            Assert.AreEqual("local1", vars[1]);
            Assert.AreEqual("local2", vars[2]);
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
            List<string> lines = new List<string>();
            lines.Clear();
            lines.Add("set(ENV{foo} abc)");
            lines.Add("set(ENV{bar} def)");
            List<string> vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(2, vars.Count);
            Assert.AreEqual("foo", vars[0]);
            Assert.AreEqual("bar", vars[1]);
            
            // Ensure that ENV is case-sensitive.
            lines.Clear();
            lines.Add("set(env{foo} abc");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(0, vars.Count);
            
            // Ensure that SET(ENV{}) is distinguished from SET($ENV{}).
            lines.Clear();
            lines.Add("set($ENV{foo} bar");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(0, vars.Count);

            // Ensure that ENV{} elsewhere doesn't define a variable.
            lines.Clear();
            lines.Add("set(foo ENV{bar})");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(0, vars.Count);

            // Test handling of environment variables defined in terms of other
            // variables or environment variables.
            lines.Clear();
            lines.Add("set(ENV{foo_${bar}} abc");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("set(ENV{foo_$ENV{bar}} abc");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(0, vars.Count);

            // Test an environment variable declaration with an illegal line break.  This
            // should fail.
            lines.Clear();
            lines.Add("set");
            lines.Add("(ENV{foo} bar)");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(0, vars.Count);

            // Test an environment variable with the same name as a standard CMake
            // variable.
            lines.Clear();
            lines.Add("set(ENV{CMAKE_CURRENT_BINARY_DIR} foo)");
            vars = CMakeParsing.ParseForEnvVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("CMAKE_CURRENT_BINARY_DIR", vars[0]);
        }

        /// <summary>
        /// Test the correctness of parsing for names of defined cache variables.
        /// </summary>
        [TestMethod]
        public void TestParseForCacheVariables()
        {
            List<string> lines = new List<string>();
            lines.Add("OPTION(FOO \"Foo\")");
            List<string> vars = CMakeParsing.ParseForCacheVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
            lines.Clear();
            lines.Add("SET(FOO)");
            vars = CMakeParsing.ParseForCacheVariables(lines);
            Assert.AreEqual(0, vars.Count);
            lines.Clear();
            lines.Add("SET(FOO VAL CACHE FILEPATH \"Foo\")");
            vars = CMakeParsing.ParseForCacheVariables(lines);
            Assert.AreEqual(1, vars.Count);
            Assert.AreEqual("FOO", vars[0]);
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

            // Test that fragments of variable names aren't treated as identifiers.
            lines.Clear();
            lines.Add("${foo${bar}foo}");
            Assert.IsNull(CMakeParsing.ParseForIdentifier(lines, 0, 3, out isVariable));
            Assert.IsNull(CMakeParsing.ParseForIdentifier(lines, 0, 12, out isVariable));
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
            
            // Test a declaration of a variable whose name contains a hyphen.
            lines.Clear();
            lines.Add("set(foo-bar abc)");
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "foo-bar",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(4, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(10, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "abc",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration of a variable whose name begins with a hyphen.
            lines.Clear();
            lines.Add("set(-foo abc)");
            Assert.IsTrue(CMakeParsing.ParseForVariableDefinition(lines, "-foo",
                out span));
            Assert.AreEqual(0, span.iStartLine);
            Assert.AreEqual(4, span.iStartIndex);
            Assert.AreEqual(0, span.iEndLine);
            Assert.AreEqual(7, span.iEndIndex);
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "abc",
                out span));
            Assert.IsFalse(CMakeParsing.ParseForVariableDefinition(lines, "set",
                out span));

            // Test a declaration with an illegal line break.  This should fail.
            lines.Clear();
            lines.Add("set");
            lines.Add("(foo bar)");
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

            // Test a definition with illegal line breaks.  This should fail.
            lines.Clear();
            lines.Add("function");
            lines.Add("(foo)");
            lines.Add("endfunction(foo)");
            Assert.IsFalse(CMakeParsing.ParseForFunctionDefinition(lines, "foo",
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
            lines.Add("  b #[[ second parameter");
            lines.Add("         bracket comment");
            lines.Add("         end]]");
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

            // Test a command with a bracket comment in the middle.
            lines.Clear();
            lines.Add("add_executable(foo #[[bracket comment]] foo.cpp bar.cpp)");
            result = CMakeParsing.ParseForParameterInfo(lines, 0, 55);
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
            lines.Add("FILE(WRITE foo.txt \"bar\")");
            result = CMakeParsing.ParseForParameterInfo(lines, 0, 25);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("FILE", result.CommandName);
            Assert.IsNotNull(result.SubcommandName);
            Assert.AreEqual("WRITE", result.SubcommandName);
            Assert.IsTrue(result.CommandSpan.HasValue);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartLine);
            Assert.AreEqual(0, result.CommandSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.CommandSpan.Value.iEndLine);
            Assert.AreEqual(9, result.CommandSpan.Value.iEndIndex);
            Assert.IsTrue(result.BeginSpan.HasValue);
            Assert.AreEqual(0, result.BeginSpan.Value.iStartLine);
            Assert.AreEqual(10, result.BeginSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.BeginSpan.Value.iEndLine);
            Assert.AreEqual(10, result.BeginSpan.Value.iEndIndex);
            Assert.AreEqual(1, result.SeparatorSpans.Count);
            Assert.AreEqual(0, result.SeparatorSpans[0].iStartLine);
            Assert.AreEqual(18, result.SeparatorSpans[0].iStartIndex);
            Assert.AreEqual(0, result.SeparatorSpans[0].iEndLine);
            Assert.AreEqual(18, result.SeparatorSpans[0].iEndIndex);
            Assert.IsTrue(result.EndSpan.HasValue);
            Assert.AreEqual(0, result.EndSpan.Value.iStartLine);
            Assert.AreEqual(24, result.EndSpan.Value.iStartIndex);
            Assert.AreEqual(0, result.EndSpan.Value.iEndLine);
            Assert.AreEqual(24, result.EndSpan.Value.iEndIndex);

            // Ensure that we can parse for names only.
            result = CMakeParsing.ParseForParameterInfo(lines, 0, -1, true);
            Assert.IsNotNull(result.CommandName);
            Assert.AreEqual("FILE", result.CommandName);
            Assert.IsNotNull(result.SubcommandName);
            Assert.AreEqual("WRITE", result.SubcommandName);
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

            CMakeParsing.TokenData tokenData;
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 1, out tokenData));
            Assert.IsFalse(tokenData.InParens);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenData.TokenInfo.Token);
            Assert.AreEqual(0, tokenData.TokenInfo.StartIndex);
            Assert.AreEqual(2, tokenData.TokenInfo.EndIndex);
            Assert.AreEqual(0, tokenData.PriorParameters.Count);
            Assert.AreEqual(CMakeCommandId.Unspecified, tokenData.Command);
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 4, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(0, tokenData.ParameterIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenData.TokenInfo.Token);
            Assert.AreEqual(4, tokenData.TokenInfo.StartIndex);
            Assert.AreEqual(6, tokenData.TokenInfo.EndIndex);
            Assert.AreEqual(0, tokenData.PriorParameters.Count);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);
            Assert.IsFalse(CMakeParsing.ParseForToken(lines, 0, 10, out tokenData));
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 1, 2, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(1, tokenData.ParameterIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenData.TokenInfo.Token);
            Assert.AreEqual(2, tokenData.TokenInfo.StartIndex);
            Assert.AreEqual(2, tokenData.TokenInfo.EndIndex);
            Assert.AreEqual(1, tokenData.PriorParameters.Count);
            Assert.AreEqual("ABC", tokenData.PriorParameters[0]);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);
            Assert.IsFalse(CMakeParsing.ParseForToken(lines, 10, 0, out tokenData));

            // Test on a single line.
            lines.Clear();
            lines.Add("SET(ABC X)");
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 4, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(0, tokenData.ParameterIndex);
            Assert.AreEqual(0, tokenData.PriorParameters.Count);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 8, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(1, tokenData.ParameterIndex);
            Assert.AreEqual(1, tokenData.PriorParameters.Count);
            Assert.AreEqual("ABC", tokenData.PriorParameters[0]);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);

            // Test with extra whitespace.
            lines.Clear();
            lines.Add("SET( ABC X )");
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 5, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(0, tokenData.ParameterIndex);
            Assert.AreEqual(0, tokenData.PriorParameters.Count);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 9, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(1, tokenData.ParameterIndex);
            Assert.AreEqual(1, tokenData.PriorParameters.Count);
            Assert.AreEqual("ABC", tokenData.PriorParameters[0]);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);

            // Test on multiple lines with no whitespace.
            lines.Clear();
            lines.Add("SET(");
            lines.Add("ABC");
            lines.Add("X)");
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 1, 0, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(0, tokenData.ParameterIndex);
            Assert.AreEqual(0, tokenData.PriorParameters.Count);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 2, 0, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(1, tokenData.ParameterIndex);
            Assert.AreEqual(1, tokenData.PriorParameters.Count);
            Assert.AreEqual("ABC", tokenData.PriorParameters[0]);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);

            // Test a parameter involving multiple tokens.
            lines.Clear();
            lines.Add("SET(ABC${FOO} X)");
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 14, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(1, tokenData.ParameterIndex);
            Assert.AreEqual(1, tokenData.PriorParameters.Count);
            Assert.AreEqual("ABC${FOO}", tokenData.PriorParameters[0]);
            Assert.AreEqual(CMakeCommandId.Set, tokenData.Command);

            // Test a different command.
            lines.Clear();
            lines.Add("UNSET(FOO)");
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 6, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(0, tokenData.ParameterIndex);
            Assert.AreEqual(0, tokenData.PriorParameters.Count);
            Assert.AreEqual(CMakeCommandId.Unset, tokenData.Command);

            // Test a user-defined function or macro.
            lines.Clear();
            lines.Add("BAR(FOO)");
            Assert.IsTrue(CMakeParsing.ParseForToken(lines, 0, 4, out tokenData));
            Assert.IsTrue(tokenData.InParens);
            Assert.AreEqual(0, tokenData.ParameterIndex);
            Assert.AreEqual(CMakeCommandId.Unspecified, tokenData.Command);
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

        /// <summary>
        /// Test parsing for target names.
        /// </summary>
        [TestMethod]
        public void TestParseForTargetNames()
        {
            // Test parsing an executable and a library.
            List<string> lines = new List<string>();
            lines.Add("add_executable(foo foo.cpp)");
            lines.Add("add_library(bar bar.cpp)");
            lines.Add("add_test(test test.cpp)");
            List<string> targets = CMakeParsing.ParseForTargetNames(lines);
            Assert.IsNotNull(targets);
            Assert.AreEqual(2, targets.Count);
            Assert.AreEqual("foo", targets[0]);
            Assert.AreEqual("bar", targets[1]);
            targets = CMakeParsing.ParseForTargetNames(lines, true);
            Assert.IsNotNull(targets);
            Assert.AreEqual(1, targets.Count);
            Assert.AreEqual("test", targets[0]);

            // Test parsing a target with extra whitespace.
            lines.Clear();
            lines.Add("add_executable( foo foo.cpp )");
            targets = CMakeParsing.ParseForTargetNames(lines);
            Assert.IsNotNull(targets);
            Assert.AreEqual(1, targets.Count);
            Assert.AreEqual("foo", targets[0]);

            // Test parsing a target with line breaks and comments.
            lines.Clear();
            lines.Add("add_executable( # comment");
            lines.Add("  foo # another comment");
            lines.Add("  foo.cpp)");
            targets = CMakeParsing.ParseForTargetNames(lines);
            Assert.IsNotNull(targets);
            Assert.AreEqual(1, targets.Count);
            Assert.AreEqual("foo", targets[0]);

            // Test parsing a target with bracket comments in the middle.
            lines.Clear();
            lines.Add("add_executable(#[[comment]] foo #[[comment]] foo.cpp)");
            targets = CMakeParsing.ParseForTargetNames(lines);
            Assert.IsNotNull(targets);
            Assert.AreEqual(1, targets.Count);
            Assert.AreEqual("foo", targets[0]);

            // Test parsing a target with an illegal line break.  This should fail.
            lines.Clear();
            lines.Add("add_executable");
            lines.Add("(foo foo.cpp)");
            targets = CMakeParsing.ParseForTargetNames(lines);
            Assert.IsNotNull(targets);
            Assert.AreEqual(0, targets.Count);
        }

        /// <summary>
        /// Test parsing for matching pairs of parentheses.
        /// </summary>
        [TestMethod]
        public void TestParseForParens()
        {
            // Test a single pair.
            List<string> lines = new List<string>();
            lines.Add("set(foo)");
            List<CMakeParsing.SpanPair> pairs = CMakeParsing.ParseForParens(lines);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(1, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(3, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(4, pairs[0].First.iEndIndex);
            Assert.AreEqual(0, pairs[0].Second.iStartLine);
            Assert.AreEqual(7, pairs[0].Second.iStartIndex);
            Assert.AreEqual(0, pairs[0].Second.iEndLine);
            Assert.AreEqual(8, pairs[0].Second.iEndIndex);

            // Test nested parentheses.
            lines.Clear();
            lines.Add("set(foo (abc))");
            pairs = CMakeParsing.ParseForParens(lines);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(2, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(8, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(9, pairs[0].First.iEndIndex);
            Assert.AreEqual(0, pairs[0].Second.iStartLine);
            Assert.AreEqual(12, pairs[0].Second.iStartIndex);
            Assert.AreEqual(0, pairs[0].Second.iEndLine);
            Assert.AreEqual(13, pairs[0].Second.iEndIndex);
            Assert.AreEqual(0, pairs[1].First.iStartLine);
            Assert.AreEqual(3, pairs[1].First.iStartIndex);
            Assert.AreEqual(0, pairs[1].First.iEndLine);
            Assert.AreEqual(4, pairs[1].First.iEndIndex);
            Assert.AreEqual(0, pairs[1].Second.iStartLine);
            Assert.AreEqual(13, pairs[1].Second.iStartIndex);
            Assert.AreEqual(0, pairs[1].Second.iEndLine);
            Assert.AreEqual(14, pairs[1].Second.iEndIndex);

            // Test a pair of parentheses split across multiple lines.
            lines.Clear();
            lines.Add("set(foo");
            lines.Add("  abc");
            lines.Add(")");
            pairs = CMakeParsing.ParseForParens(lines);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(1, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(3, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(4, pairs[0].First.iEndIndex);
            Assert.AreEqual(2, pairs[0].Second.iStartLine);
            Assert.AreEqual(0, pairs[0].Second.iStartIndex);
            Assert.AreEqual(2, pairs[0].Second.iEndLine);
            Assert.AreEqual(1, pairs[0].Second.iEndIndex);

            // Test an unmatched opening parenthesis.
            lines.Clear();
            lines.Add("set(");
            pairs = CMakeParsing.ParseForParens(lines);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(0, pairs.Count);

            // Test an unmatched closing parenthesis.
            lines.Clear();
            lines.Add(")");
            pairs = CMakeParsing.ParseForParens(lines);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(0, pairs.Count);
        }

        /// <summary>
        /// Test for matching pair of curly braces denoting variable references.
        /// </summary>
        [TestMethod]
        public void TestParseForVariableBraces()
        {
            // Test a single pair.
            List<string> lines = new List<string>();
            lines.Add("${FOO}");
            List<CMakeParsing.SpanPair> pairs = CMakeParsing.ParseForVariableBraces(
                lines, 0);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(1, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(0, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(2, pairs[0].First.iEndIndex);
            Assert.AreEqual(0, pairs[0].Second.iStartLine);
            Assert.AreEqual(5, pairs[0].Second.iStartIndex);
            Assert.AreEqual(0, pairs[0].Second.iEndLine);
            Assert.AreEqual(6, pairs[0].Second.iEndIndex);
            lines.Clear();
            lines.Add("$ENV{FOO}");
            pairs = CMakeParsing.ParseForVariableBraces(lines, 0);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(1, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(0, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(5, pairs[0].First.iEndIndex);
            Assert.AreEqual(0, pairs[0].Second.iStartLine);
            Assert.AreEqual(8, pairs[0].Second.iStartIndex);
            Assert.AreEqual(0, pairs[0].Second.iEndLine);
            Assert.AreEqual(9, pairs[0].Second.iEndIndex);
            lines.Clear();
            lines.Add("$CACHE{FOO}");
            pairs = CMakeParsing.ParseForVariableBraces(lines, 0);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(1, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(0, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(7, pairs[0].First.iEndIndex);
            Assert.AreEqual(0, pairs[0].Second.iStartLine);
            Assert.AreEqual(10, pairs[0].Second.iStartIndex);
            Assert.AreEqual(0, pairs[0].Second.iEndLine);
            Assert.AreEqual(11, pairs[0].Second.iEndIndex);

            // Test two nested pairs.
            lines.Clear();
            lines.Add("${${FOO}}");
            pairs = CMakeParsing.ParseForVariableBraces(lines, 0);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(2, pairs.Count);
            Assert.AreEqual(0, pairs[0].First.iStartLine);
            Assert.AreEqual(2, pairs[0].First.iStartIndex);
            Assert.AreEqual(0, pairs[0].First.iEndLine);
            Assert.AreEqual(4, pairs[0].First.iEndIndex);
            Assert.AreEqual(0, pairs[0].Second.iStartLine);
            Assert.AreEqual(7, pairs[0].Second.iStartIndex);
            Assert.AreEqual(0, pairs[0].Second.iEndLine);
            Assert.AreEqual(8, pairs[0].Second.iEndIndex);
            Assert.AreEqual(0, pairs[1].First.iStartLine);
            Assert.AreEqual(0, pairs[1].First.iStartIndex);
            Assert.AreEqual(0, pairs[1].First.iEndLine);
            Assert.AreEqual(2, pairs[1].First.iEndIndex);
            Assert.AreEqual(0, pairs[1].Second.iStartLine);
            Assert.AreEqual(8, pairs[1].Second.iStartIndex);
            Assert.AreEqual(0, pairs[1].Second.iEndLine);
            Assert.AreEqual(9, pairs[1].Second.iEndIndex);

            // Test an unmatched pair.
            lines.Clear();
            lines.Add("${FOO");
            pairs = CMakeParsing.ParseForVariableBraces(lines, 0);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(0, pairs.Count);

            // Test a matched pair with an illegal character in the middle.
            lines.Clear();
            lines.Add("${FOO BAR}");
            pairs = CMakeParsing.ParseForVariableBraces(lines, 0);
            Assert.IsNotNull(pairs);
            Assert.AreEqual(0, pairs.Count);
        }

        /// <summary>
        /// Test parsing for include files.
        /// </summary>
        [TestMethod]
        public void TestParseForIncludes()
        {
            List<string> lines = new List<string>();
            lines.Add("include(A)");
            lines.Add("include ( B )");
            lines.Add("include(");
            lines.Add("  C)");
            lines.Add("include( # comment");
            lines.Add("  D)");
            lines.Add("include(#[[comment]] E #[[comment]])");
            lines.Add("find_package(Foo)");
            List<string> includes = CMakeParsing.ParseForIncludes(lines);
            Assert.AreEqual(6, includes.Count);
            Assert.AreEqual("A", includes[0]);
            Assert.AreEqual("B", includes[1]);
            Assert.AreEqual("C", includes[2]);
            Assert.AreEqual("D", includes[3]);
            Assert.AreEqual("E", includes[4]);
            Assert.AreEqual("FindFoo", includes[5]);
        }

        /// <summary>
        /// Test parsing to determine whether a line should be indented.
        /// </summary>
        [TestMethod]
        public void TestIndentation()
        {
            List<string> lines = new List<string>();
            int lineToMatch;
            lines.Add("# foo");
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 0));
            Assert.IsFalse(CMakeParsing.ShouldUnindent(lines, 0, out lineToMatch));
            lines.Clear();
            lines.Add("if(foo)");
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 0));
            Assert.IsFalse(CMakeParsing.ShouldUnindent(lines, 0, out lineToMatch));
            lines.Clear();
            lines.Add("set(foo");
            lines.Add("  bar)");
            Assert.IsTrue(CMakeParsing.ShouldIndent(lines, 0));
            Assert.IsFalse(CMakeParsing.ShouldUnindent(lines, 0, out lineToMatch));
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 1));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 1, out lineToMatch));
            Assert.AreEqual(0, lineToMatch);
            lines.Clear();
            lines.Add("set(");
            lines.Add("  foo");
            lines.Add("  \"abc");
            lines.Add("def");
            lines.Add("ghi\"");
            lines.Add(")");
            Assert.IsTrue(CMakeParsing.ShouldIndent(lines, 0));
            Assert.IsFalse(CMakeParsing.ShouldUnindent(lines, 0, out lineToMatch));
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 1));
            Assert.IsFalse(CMakeParsing.ShouldUnindent(lines, 1, out lineToMatch));
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 2));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 2, out lineToMatch));
            Assert.AreEqual(-1, lineToMatch);
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 3));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 3, out lineToMatch));
            Assert.AreEqual(-1, lineToMatch);
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 4));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 4, out lineToMatch));
            Assert.AreEqual(2, lineToMatch);
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 5));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 5, out lineToMatch));
            lines.Clear();
            lines.Add("set(foo \"abc");
            lines.Add("def\")");
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 0));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 0, out lineToMatch));
            Assert.AreEqual(-1, lineToMatch);
            Assert.IsFalse(CMakeParsing.ShouldIndent(lines, 1));
            Assert.IsTrue(CMakeParsing.ShouldUnindent(lines, 1, out lineToMatch));
            Assert.AreEqual(0, lineToMatch);
        }

        /// <summary>
        /// Test finding the indentation level of a line.
        /// </summary>
        [TestMethod]
        public void TestIndentationLevel()
        {
            Assert.AreEqual(0, CMakeParsing.GetIndentationLevel("set(foo)", '\t'));
            Assert.AreEqual(0, CMakeParsing.GetIndentationLevel("set(foo)", ' '));
            Assert.AreEqual(1, CMakeParsing.GetIndentationLevel("\tset(foo)", '\t'));
            Assert.AreEqual(2, CMakeParsing.GetIndentationLevel("  set(foo)", ' '));
        }

        /// <summary>
        /// Test finding the last non-empty line up to a given line.
        /// </summary>
        [TestMethod]
        public void TestLastNonEmptyLines()
        {
            List<string> lines = new List<string>();
            lines.Add("set(foo)");
            lines.Add("    ");
            lines.Add("\t");
            lines.Add("set(bar)");
            Assert.AreEqual(0, CMakeParsing.GetLastNonEmptyLine(lines, 0));
            Assert.AreEqual(0, CMakeParsing.GetLastNonEmptyLine(lines, 1));
            Assert.AreEqual(0, CMakeParsing.GetLastNonEmptyLine(lines, 2));
            Assert.AreEqual(3, CMakeParsing.GetLastNonEmptyLine(lines, 3));
        }

        /// <summary>
        /// Test parsing for bad variable references.
        /// </summary>
        [TestMethod]
        public void TestParseForBadVariableRefs()
        {
            // Check that there are no false positives.
            List<string> lines = new List<string>();
            lines.Add("${FOO}");
            lines.Add("$ENV{FOO}");
            lines.Add("$CACHE{FOO}");
            lines.Add("${${FOO}}");
            lines.Add("${FOO${FOO}}");
            lines.Add("${${FOO}FOO}");
            lines.Add("${FOO${FOO}FOO}");
            lines.Add("$ENV{${FOO}}");
            lines.Add("${FOO-BAR${FOO-BAR}FOO-BAR}");
            lines.Add("${8${8}8}");
            lines.Add("SET(ENV{${FOO}})");
            List<CMakeErrorInfo> info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(0, info.Count);

            // Check that errors are found.
            lines.Clear();
            lines.Add("${FOO }");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(2, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(5, info[0].Span.iEndIndex);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[1].ErrorCode);
            Assert.AreEqual(0, info[1].Span.iStartLine);
            Assert.AreEqual(6, info[1].Span.iStartIndex);
            Assert.AreEqual(0, info[1].Span.iEndLine);
            Assert.AreEqual(7, info[1].Span.iEndIndex);
            lines.Clear();
            lines.Add("${");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(2, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("${}");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(3, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("$ENV{");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(5, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("$CACHE{");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(7, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("SET(ENV{");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(4, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(8, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("${${FOO");
            info = CMakeParsing.ParseForBadVariableRefs(lines);
            Assert.AreEqual(2, info.Count);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(2, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(7, info[0].Span.iEndIndex);
            Assert.AreEqual(CMakeError.InvalidVariableRef, info[1].ErrorCode);
            Assert.AreEqual(0, info[1].Span.iStartLine);
            Assert.AreEqual(0, info[1].Span.iStartIndex);
            Assert.AreEqual(0, info[1].Span.iEndLine);
            Assert.AreEqual(7, info[1].Span.iEndIndex);
        }

        /// <summary>
        /// Test parsing for unmatched parentheses.
        /// </summary>
        [TestMethod]
        public void TestParseForUnmatchedParens()
        {
            // Test that there are no false positives.
            List<string> lines = new List<string>();
            lines.Add("()");
            lines.Add("(())");
            lines.Add("((");
            lines.Add(")");
            lines.Add(")");
            List<CMakeErrorInfo> info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(0, info.Count);

            // Test that unmatched parentheses are properly detected.
            lines.Clear();
            lines.Add("(");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(1, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add(")");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(1, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("(()");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(1, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("())");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(2, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(3, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("((");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(2, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(1, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(2, info[0].Span.iEndIndex);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[1].ErrorCode);
            Assert.AreEqual(0, info[1].Span.iStartLine);
            Assert.AreEqual(0, info[1].Span.iStartIndex);
            Assert.AreEqual(0, info[1].Span.iEndLine);
            Assert.AreEqual(1, info[1].Span.iEndIndex);
            lines.Clear();
            lines.Add("))");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(2, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(1, info[0].Span.iEndIndex);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[1].ErrorCode);
            Assert.AreEqual(0, info[1].Span.iStartLine);
            Assert.AreEqual(1, info[1].Span.iStartIndex);
            Assert.AreEqual(0, info[1].Span.iEndLine);
            Assert.AreEqual(2, info[1].Span.iEndIndex);
            lines.Clear();
            lines.Add("(");
            lines.Add("()");
            info = CMakeParsing.ParseForUnmatchedParens(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.UnmatchedParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(1, info[0].Span.iEndIndex);
        }

        /// <summary>
        /// Test parsing for bad commands.
        /// </summary>
        [TestMethod]
        public void TestParseForBadCommands()
        {
            // Test that there are no false positives
            List<string> lines = new List<string>();
            lines.Add("SET(FOO)");
            lines.Add("SET(FOO BAR)");
            lines.Add("# comment");
            lines.Add("SET(FOO) # comment");
            lines.Add("SET(#[[comment]] FOO)");
            List<CMakeErrorInfo> info = CMakeParsing.ParseForBadCommands(lines);
            Assert.AreEqual(0, info.Count);

            // Test that invalid commands are properly detected.
            lines.Clear();
            lines.Add("${FOO}()");
            info = CMakeParsing.ParseForBadCommands(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.ExpectedCommand, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(2, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("FOO.EXE()");
            info = CMakeParsing.ParseForBadCommands(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.ExpectedCommand, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(0, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(7, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("SET(FOO) BAR");
            info = CMakeParsing.ParseForBadCommands(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.ExpectedEOL, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(9, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(12, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("SET FOO");
            info = CMakeParsing.ParseForBadCommands(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.ExpectedOpenParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(4, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(7, info[0].Span.iEndIndex);
            lines.Clear();
            lines.Add("SET");
            info = CMakeParsing.ParseForBadCommands(lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.ExpectedOpenParen, info[0].ErrorCode);
            Assert.AreEqual(0, info[0].Span.iStartLine);
            Assert.AreEqual(3, info[0].Span.iStartIndex);
            Assert.AreEqual(0, info[0].Span.iEndLine);
            Assert.AreEqual(4, info[0].Span.iEndIndex);
        }

        /// <summary>
        /// Test parsing for invalid escape sequences.
        /// </summary>
        [TestMethod]
        public void TestParseForInvalidEscapeSequences()
        {
            List<string> lines = new List<string>();
            lines.Add("\"\\\\\\\"\\ \\#\\(\\)\\$\\@\\^\\;\\t\\n\\r\\0\"");
            lines.Add("\"\\\\a\"");
            lines.Add("    \"\\a\"");
            List<CMakeErrorInfo> info = CMakeParsing.ParseForInvalidEscapeSequences(
                lines);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual(CMakeError.InvalidEscapeSequence, info[0].ErrorCode);
            Assert.AreEqual(2, info[0].Span.iStartLine);
            Assert.AreEqual(5, info[0].Span.iStartIndex);
            Assert.AreEqual(2, info[0].Span.iEndLine);
            Assert.AreEqual(7, info[0].Span.iEndIndex);
            Assert.IsFalse(info[0].Warning);
        }

        /// <summary>
        /// Test parsing for the current function name.
        /// </summary>
        [TestMethod]
        public void TestParseForCurrentFunction()
        {
            List<string> lines = new List<string>();
            lines.Add("SET(FOO)");
            lines.Add("FUNCTION(ABC PARAM1 PARAM2)");
            lines.Add("  ADD_EXECUTABLE(${PARAM1} ${PARAM2})");
            lines.Add("ENDFUNCTION(ABC)");
            lines.Add("SET(BAR)");
            lines.Add("MACRO(DEF)");
            lines.Add("  MESSAGE(STATUS \"Test\")");
            lines.Add("ENDMACRO()");
            lines.Add("SET(XYZ)");
            Assert.IsNull(CMakeParsing.ParseForCurrentFunction(lines, 0));
            Assert.AreEqual("ABC", CMakeParsing.ParseForCurrentFunction(lines, 2));
            Assert.IsNull(CMakeParsing.ParseForCurrentFunction(lines, 4));
            Assert.AreEqual("DEF", CMakeParsing.ParseForCurrentFunction(lines, 6));
            Assert.IsNull(CMakeParsing.ParseForCurrentFunction(lines, 8));
        }
    }
}
