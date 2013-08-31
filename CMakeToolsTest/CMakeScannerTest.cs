/* ****************************************************************************
 * 
 * Copyright (C) 2012-2013 by David Golub.  All rights reserved.
 * 
 * This software is subject to the Microsoft Reciprocal License (Ms-RL).
 * A copy of the license can be found in the License.txt file included
 * in this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 * 
 * **************************************************************************/

using CMakeTools;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMakeTools
{
    /// <summary>
    /// Tests of the scanner for CMake code.
    /// </summary>
    [TestClass]
    public class CMakeScannerTest
    {
        /// <summary>
        /// Test the correctness of the scanner.
        /// </summary>
        [TestMethod]
        public void TestScanner()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("SET(FOO ${BAR} ABC) # comment", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex, 0);
            Assert.AreEqual(2, tokenInfo.EndIndex, 2);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(7, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(9, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableStart, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(10, tokenInfo.StartIndex);
            Assert.AreEqual(12, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Variable, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(13, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableEnd, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MatchBraces, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(14, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(15, tokenInfo.StartIndex);
            Assert.AreEqual(17, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(18, tokenInfo.StartIndex);
            Assert.AreEqual(18, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(19, tokenInfo.StartIndex);
            Assert.AreEqual(19, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(20, tokenInfo.StartIndex);
            Assert.AreEqual(28, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Comment, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("set( foo \"bar\")", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex, 4);
            Assert.AreEqual(4, tokenInfo.EndIndex, 4);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(5, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(9, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(14, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("$ENV{FOO}", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(4, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableStartEnv, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(5, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Variable, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableEnd, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MatchBraces, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test that a line ending with a backslash doesn't crash the scanner.
            scanner.SetSource("FOO\\", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.FileName, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test that the logic to handle a backslash at the end of a line doesn't
            // break handling of other strings with backslashes in them.
            scanner.SetSource("FOO\\BAR", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.FileName, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test the correctness of the scanner with respect to multiline commands.
        /// </summary>
        [TestMethod]
        public void TestScannerMultiLine()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("add_executable(foo", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(14, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(15, tokenInfo.StartIndex);
            Assert.AreEqual(17, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("foo.cpp WIN32)", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.FileName, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(7, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext | TokenTriggers.MemberSelect,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(12, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(13, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test correctness of the scanner with respect to multiline strings.
        /// </summary>
        [TestMethod]
        public void TestScannerMultiLineString()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("\"foo bar", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("\\\"foo bar\\\"", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(10, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("end\"", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test the correctness of the scanner with respect to commands to set
        /// environment variables.
        /// </summary>
        [TestMethod]
        public void TestScannerSetEnv()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("SET(ENV{FOO} BAR)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableStartSetEnv, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(10, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Variable, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(11, tokenInfo.StartIndex);
            Assert.AreEqual(11, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableEnd, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MatchBraces, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(12, tokenInfo.StartIndex);
            Assert.AreEqual(12, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(13, tokenInfo.StartIndex);
            Assert.AreEqual(15, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(16, tokenInfo.StartIndex);
            Assert.AreEqual(16, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // A space between the opening parenthesis and ENV{ shouldn't mess things up.
            scanner.SetSource("SET( ENV{FOO} BAR)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(4, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(5, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableStartSetEnv, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(9, tokenInfo.StartIndex);
            Assert.AreEqual(11, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Variable, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(12, tokenInfo.StartIndex);
            Assert.AreEqual(12, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableEnd, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MatchBraces, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(13, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(16, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(17, tokenInfo.StartIndex);
            Assert.AreEqual(17, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test that the recognition of command-specific keywords is case-sensitive.
        /// </summary>
        [TestMethod]
        public void TestScannerCaseSensitive()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("add_executable(foo win32", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(14, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(15, tokenInfo.StartIndex);
            Assert.AreEqual(17, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(18, tokenInfo.StartIndex);
            Assert.AreEqual(18, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext | TokenTriggers.MemberSelect,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(19, tokenInfo.StartIndex);
            Assert.AreEqual(23, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test that the scanner properly handles numeric identifier.
        /// </summary>
        [TestMethod]
        public void TestScannerNumericIdentifier()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            // Test setting a variable whose name is just a number.
            scanner.SetSource("set(8 abc)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(4, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.NumericIdentifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(5, tokenInfo.StartIndex);
            Assert.AreEqual(5, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(6, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(9, tokenInfo.StartIndex);
            Assert.AreEqual(9, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test setting a variable whose name starts with a number.
            scanner.SetSource("set(8xy abc)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.NumericIdentifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(7, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(10, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(11, tokenInfo.StartIndex);
            Assert.AreEqual(11, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test getting a variable whose name is just a number.
            scanner.SetSource("${8}", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(1, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableStart, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(2, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Variable, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableEnd, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MatchBraces, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test getting a variable whose name starts with a number.
            scanner.SetSource("${8xy}", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(1, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableStart, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(2, tokenInfo.StartIndex);
            Assert.AreEqual(4, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Variable, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(5, tokenInfo.StartIndex);
            Assert.AreEqual(5, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.VariableEnd, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MatchBraces, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test setting a variable whose name contains a hyphen.
            scanner.SetSource("set(foo-bar)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(10, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.NumericIdentifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(11, tokenInfo.StartIndex);
            Assert.AreEqual(11, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test setting a variable whose begins with a hyphen.
            scanner.SetSource("set(-foo)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(4, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.NumericIdentifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(8, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.CloseParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Test that a file name starting with a digit is identified as a file name
            // and not as a numeric identifier or something else.
            scanner.SetSource("8xy.txt", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.FileName, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test that member selection is properly triggered for functions, macros,
        /// and commands.
        /// </summary>
        [TestMethod]
        public void TestFunctionMemberSelection()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            // Check a single character triggers member selection.
            scanner.SetSource("a", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(0, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Check that a single character doesn't triggers member selection inside
            // parentheses.
            scanner.SetSource("add_executable(a", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(14, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(15, tokenInfo.StartIndex);
            Assert.AreEqual(15, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Check that whitespace at the beginning of a line doesn't prevent member
            // selection from being triggered.
            scanner.SetSource(" a", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(0, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(1, tokenInfo.StartIndex);
            Assert.AreEqual(1, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // Check that a multiline command doesn't trigger member selection.
            scanner.SetSource("set(", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(2, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(3, tokenInfo.StartIndex);
            Assert.AreEqual(3, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("a", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(0, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test that member selection is properly triggered for file names.
        /// </summary>
        [TestMethod]
        public void TestFileMemberSelection()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("add_executable(a b", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(13, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Keyword, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(14, tokenInfo.StartIndex);
            Assert.AreEqual(14, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.OpenParen, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterStart | TokenTriggers.MatchBraces,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(15, tokenInfo.StartIndex);
            Assert.AreEqual(15, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(16, tokenInfo.StartIndex);
            Assert.AreEqual(16, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.WhiteSpace, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.ParameterNext | TokenTriggers.MemberSelect,
                tokenInfo.Trigger);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(17, tokenInfo.StartIndex);
            Assert.AreEqual(17, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.Identifier, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }

        /// <summary>
        /// Test scanning strings that contain variable references.
        /// </summary>
        [TestMethod]
        public void TestVariablesInStrings()
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state;

            scanner.SetSource("\"FOO${", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(5, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("\"FOO$ENV{", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("\"FOO$CACHE{", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(10, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("\"FOO${BAR", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(8, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("\"FOO${\"", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("\"FOO\\${", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(6, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.None, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("\"FOO\\\\${", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(0, tokenInfo.StartIndex);
            Assert.AreEqual(7, tokenInfo.EndIndex);
            Assert.AreEqual(CMakeToken.String, (CMakeToken)tokenInfo.Token);
            Assert.AreEqual(TokenTriggers.MemberSelect, tokenInfo.Trigger);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }
    }
}
