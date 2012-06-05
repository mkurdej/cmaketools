// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

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
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 2);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Keyword);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 3);
            Assert.AreEqual(tokenInfo.EndIndex, 3);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.OpenParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterStart);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 4);
            Assert.AreEqual(tokenInfo.EndIndex, 6);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Identifier);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 7);
            Assert.AreEqual(tokenInfo.EndIndex, 7);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterNext);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 8);
            Assert.AreEqual(tokenInfo.EndIndex, 9);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableStart);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.MemberSelect);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 10);
            Assert.AreEqual(tokenInfo.EndIndex, 12);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Variable);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 13);
            Assert.AreEqual(tokenInfo.EndIndex, 13);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableEnd);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 14);
            Assert.AreEqual(tokenInfo.EndIndex, 14);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 15);
            Assert.AreEqual(tokenInfo.EndIndex, 17);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Identifier);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 18);
            Assert.AreEqual(tokenInfo.EndIndex, 18);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.CloseParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterEnd);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 19);
            Assert.AreEqual(tokenInfo.EndIndex, 19);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 20);
            Assert.AreEqual(tokenInfo.EndIndex, 28);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Comment);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("set( foo \"bar\")", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 2);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Keyword);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 3);
            Assert.AreEqual(tokenInfo.EndIndex, 3);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.OpenParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterStart);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 4);
            Assert.AreEqual(tokenInfo.EndIndex, 4);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 5);
            Assert.AreEqual(tokenInfo.EndIndex, 7);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Identifier);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 8);
            Assert.AreEqual(tokenInfo.EndIndex, 8);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterNext);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 9);
            Assert.AreEqual(tokenInfo.EndIndex, 13);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.String);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 14);
            Assert.AreEqual(tokenInfo.EndIndex, 14);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.CloseParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterEnd);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            scanner.SetSource("$ENV{FOO}", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 4);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableStartEnv);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.MemberSelect);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 5);
            Assert.AreEqual(tokenInfo.EndIndex, 7);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Variable);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 8);
            Assert.AreEqual(tokenInfo.EndIndex, 8);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableEnd);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
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
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 13);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Keyword);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 14);
            Assert.AreEqual(tokenInfo.EndIndex, 14);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.OpenParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterStart);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 15);
            Assert.AreEqual(tokenInfo.EndIndex, 17);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Identifier);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("foo.cpp WIN32)", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 6);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.FileName);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 7);
            Assert.AreEqual(tokenInfo.EndIndex, 7);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterNext);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 8);
            Assert.AreEqual(tokenInfo.EndIndex, 12);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Keyword);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 13);
            Assert.AreEqual(tokenInfo.EndIndex, 13);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.CloseParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterEnd);
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
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 7);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.String);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("\\\"foo bar\\\"", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 10);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.String);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            scanner.SetSource("end\"", 0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 3);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.String);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
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
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 2);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Keyword);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 3);
            Assert.AreEqual(tokenInfo.EndIndex, 3);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.OpenParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterStart);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 4);
            Assert.AreEqual(tokenInfo.EndIndex, 7);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableStartSetEnv);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 8);
            Assert.AreEqual(tokenInfo.EndIndex, 10);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Variable);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 11);
            Assert.AreEqual(tokenInfo.EndIndex, 11);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableEnd);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 12);
            Assert.AreEqual(tokenInfo.EndIndex, 12);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterNext);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 13);
            Assert.AreEqual(tokenInfo.EndIndex, 15);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Identifier);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 16);
            Assert.AreEqual(tokenInfo.EndIndex, 16);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.CloseParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterEnd);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));

            // A space between the opening parenthesis and ENV{ shouldn't mess things up.
            scanner.SetSource("SET( ENV{FOO} BAR)", 0);
            state = 0;
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 0);
            Assert.AreEqual(tokenInfo.EndIndex, 2);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Keyword);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 3);
            Assert.AreEqual(tokenInfo.EndIndex, 3);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.OpenParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterStart);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 4);
            Assert.AreEqual(tokenInfo.EndIndex, 4);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 5);
            Assert.AreEqual(tokenInfo.EndIndex, 8);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableStartSetEnv);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 9);
            Assert.AreEqual(tokenInfo.EndIndex, 11);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Variable);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 12);
            Assert.AreEqual(tokenInfo.EndIndex, 12);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.VariableEnd);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 13);
            Assert.AreEqual(tokenInfo.EndIndex, 13);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.WhiteSpace);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterNext);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 14);
            Assert.AreEqual(tokenInfo.EndIndex, 16);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.Identifier);
            Assert.AreEqual(tokenInfo.Trigger, (TokenTriggers)0);
            Assert.IsTrue(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            Assert.AreEqual(tokenInfo.StartIndex, 17);
            Assert.AreEqual(tokenInfo.EndIndex, 17);
            Assert.AreEqual(tokenInfo.Token, (int)CMakeToken.CloseParen);
            Assert.AreEqual(tokenInfo.Trigger, TokenTriggers.ParameterEnd);
            Assert.IsFalse(scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
        }
    }
}
