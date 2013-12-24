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

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Helper routines to parse CMake code.
    /// </summary>
    public static class CMakeParsing
    {
        // Map from commands that define variables to the number of parameter before the
        // variable defined.
        private static readonly Dictionary<CMakeCommandId, int> _paramsBeforeVariable =
            new Dictionary<CMakeCommandId, int>
        {
            { CMakeCommandId.AuxSourceDirectory,    1 },
            { CMakeCommandId.BuildCommand,          0 },
            { CMakeCommandId.FindFile,              0 },
            { CMakeCommandId.FindLibrary,           0 },
            { CMakeCommandId.FindPath,              0 },
            { CMakeCommandId.FindProgram,           0 },
            { CMakeCommandId.ForEach,               0 },
            { CMakeCommandId.GetCMakeProperty,      0 },
            { CMakeCommandId.GetDirectoryProperty,  0 },
            { CMakeCommandId.GetFileNameComponent,  0 },
            { CMakeCommandId.GetProperty,           0 },
            { CMakeCommandId.GetSourceFileProperty, 0 },
            { CMakeCommandId.GetTargetProperty,     0 },
            { CMakeCommandId.GetTestProperty,       2 },
            { CMakeCommandId.Math,                  1 },
            { CMakeCommandId.Option,                0 },
            { CMakeCommandId.SeparateArguments,     0 },
            { CMakeCommandId.Set,                   0 },
            { CMakeCommandId.SiteName,              0 }
        };

        // Internal states of the variable parsing mechanism
        private enum VariableParseState
        {
            NeedCommand,
            NeedParen,
            NeedSetEnv,
            NeedVariable,
            NeedCache
        }

        private static void AddVariableToList(List<string> vars, string variable)
        {
            // This is a private helper function to handle adding a variable to a list
            // if it is not already there and is not a standard CMake variable.
            if (!CMakeVariableDeclarations.IsStandardVariable(variable,
                CMakeVariableType.Variable))
            {
                if (vars.FindIndex(x => x.ToUpper().Equals(variable.ToUpper())) < 0)
                {
                    vars.Add(variable);
                }
            }
        }

        /// <summary>
        /// Parse to find all variables defined in the code up to a specified line,
        /// including local variables for the function that the line is part of but not
        /// including local variables for other functions.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <param name="lineNum">Line number up to which to parse.</param>
        /// <returns>A list containing all variables defined in the code.</returns>
        public static List<string> ParseForVariables(IEnumerable<string> lines,
            int lineNum = -1)
        {
            CMakeScanner scanner = new CMakeScanner();
            List<string> vars = new List<string>();
            List<string> localVars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            bool advanceAtWhiteSpace = false;
            bool insideFunction = false;
            int paramsBeforeVariable = 0;
            string possibleVariable = null;
            int i = 0;
            foreach (string line in lines)
            {
                if (i == lineNum)
                {
                    break;
                }
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    if (state == VariableParseState.NeedCommand &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                        if (_paramsBeforeVariable.ContainsKey(id))
                        {
                            // We found the name of a command that defines a variable.  Now,
                            // look for an opening parenthesis.
                            state = VariableParseState.NeedParen;
                            advanceAtWhiteSpace = false;
                            paramsBeforeVariable = _paramsBeforeVariable[id];
                        }
                        else if (id == CMakeCommandId.Function)
                        {
                            insideFunction = true;
                        }
                        else if (id == CMakeCommandId.EndFunction)
                        {
                            insideFunction = false;
                            localVars.Clear();
                        }
                    }
                    else if (state == VariableParseState.NeedParen &&
                        tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        // We found the opening parenthesis after the command name.  Now,
                        // look for the variable name, possibly after some other parameters.
                        state = VariableParseState.NeedVariable;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        possibleVariable != null &&
                        (tokenInfo.Token == (int)CMakeToken.WhiteSpace ||
                        tokenInfo.Token == (int)CMakeToken.CloseParen))
                    {
                        // We found the variable name.  Add it to the list if it's not
                        // already there and isn't a standard variable.
                        state = VariableParseState.NeedCommand;
                        AddVariableToList(insideFunction ? localVars : vars,
                            possibleVariable);
                        possibleVariable = null;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        (tokenInfo.Token == (int)CMakeToken.Identifier ||
                        tokenInfo.Token == (int)CMakeToken.NumericIdentifier))
                    {
                        if (paramsBeforeVariable == 0)
                        {
                            // We found an identifier token where the variable name is
                            // expected.  If it isn't followed by a variable start token, we
                            // will add it to the list.
                            possibleVariable = tokenText;
                        }
                        else
                        {
                            // We found a parameter.
                            advanceAtWhiteSpace = true;
                        }
                    }
                    else if (tokenInfo.Token == (int)CMakeToken.WhiteSpace)
                    {
                        if (advanceAtWhiteSpace)
                        {
                            // We found whitespace after a parameter.  Advance to the next
                            // parameter.
                            advanceAtWhiteSpace = false;
                            if (paramsBeforeVariable > 0)
                            {
                                paramsBeforeVariable--;
                            }
                        }
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        (tokenInfo.Token == (int)CMakeToken.VariableStart ||
                        tokenInfo.Token == (int)CMakeToken.Variable ||
                        tokenInfo.Token == (int)CMakeToken.VariableEnd))
                    {
                        if (paramsBeforeVariable > 0)
                        {
                            // We found a variable as a parameter.  Advance to the next
                            // parameter at the next whitespace token.
                            advanceAtWhiteSpace = true;
                        }
                        else
                        {
                            // We the variable name, and it is itself the value of another
                            // variable.  Don't add anything to the list.
                            state = VariableParseState.NeedCommand;
                            possibleVariable = null;
                        }
                    }
                    else
                    {
                        state = VariableParseState.NeedCommand;
                        possibleVariable = null;
                    }
                }
                if (state == VariableParseState.NeedVariable && possibleVariable != null)
                {
                    // If we reached the end of the line and have a variable name, accept
                    // it.  Any tokens on the next line won't be considered part of the
                    // variable name.
                    state = VariableParseState.NeedCommand;
                    AddVariableToList(insideFunction ? localVars : vars,
                        possibleVariable);
                    possibleVariable = null;
                }
                else if (state == VariableParseState.NeedParen)
                {
                    // If we reached the end of the line without finding the opening
                    // parenthesis finding the command, then there is a syntax error and
                    // the variable declaration shouldn't be recognized.
                    state = VariableParseState.NeedCommand;
                }
                i++;
            }

            // If we've finished and there are local variables, add them to the list.
            foreach (string localVar in localVars)
            {
                AddVariableToList(vars, localVar);
            }
            return vars;
        }

        /// <summary>
        /// Parse to find all environment variables defined in the code.
        /// </summary>
        /// <param name="lines">The code to parse.</param>
        /// <returns>
        /// A list containing all environment variables defined in the code.
        /// </returns>
        public static List<string> ParseForEnvVariables(IEnumerable<string> lines)
        {
            CMakeScanner scanner = new CMakeScanner();
            List<string> vars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            string possibleVariable = null;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    if (state == VariableParseState.NeedCommand &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        if (CMakeKeywords.GetCommandId(tokenText) == CMakeCommandId.Set)
                        {
                            // We found the name of a command that may define an environment
                            // variable.  Now, look for an opening parenthesis.
                            state = VariableParseState.NeedParen;
                        }
                    }
                    else if (state == VariableParseState.NeedParen &&
                        tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        // We found the opening parenthesis after the command name.  Now,
                        // look for ENV{.
                        state = VariableParseState.NeedSetEnv;
                    }
                    else if (state == VariableParseState.NeedSetEnv &&
                        tokenInfo.Token == (int)CMakeToken.VariableStartSetEnv)
                    {
                        // We found ENV{ after the opening parenthesis.  Now, look for the
                        // environment variable name.
                        state = VariableParseState.NeedVariable;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        possibleVariable != null &&
                        tokenInfo.Token == (int)CMakeToken.VariableEnd)
                    {
                        // We found the variable name.  Add it to the list if it's not
                        // already there and isn't a standard variable.
                        state = VariableParseState.NeedCommand;
                        if (!CMakeVariableDeclarations.IsStandardVariable(
                            possibleVariable, CMakeVariableType.EnvVariable))
                        {
                            if (vars.FindIndex(x => x.ToUpper().Equals(
                                possibleVariable.ToUpper())) < 0)
                            {
                                vars.Add(possibleVariable);
                            }
                        }
                        possibleVariable = null;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        tokenInfo.Token == (int)CMakeToken.Variable)
                    {
                        // We found an identifier token where the variable name is
                        // expected.  If it's followed by a variable end token, we
                        // will add it to the list.
                        possibleVariable = tokenText;
                    }
                    else if (tokenInfo.Token == (int)CMakeToken.WhiteSpace)
                    {
                        possibleVariable = null;
                    }
                    else
                    {
                        possibleVariable = null;
                        state = VariableParseState.NeedCommand;
                    }
                }
                if (state == VariableParseState.NeedParen)
                {
                    // If we reached the end of the line without finding the opening
                    // parenthesis following the command, then there is a syntax error
                    // and the variable declaration shouldn't be recognized.
                    state = VariableParseState.NeedCommand;
                    possibleVariable = null;
                }
            }
            return vars;
        }

        /// <summary>
        /// Parse to find all cache variables defined in the code.
        /// </summary>
        /// <param name="lines">The code to parse.</param>
        /// <returns>
        /// A list containing all cache variables defined in the code.
        /// </returns>
        public static List<string> ParseForCacheVariables(IEnumerable<string> lines)
        {
            CMakeScanner scanner = new CMakeScanner();
            List<string> vars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            string possibleVariable = null;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    if (state == VariableParseState.NeedCommand &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                        if (id == CMakeCommandId.Set || id == CMakeCommandId.Option)
                        {
                            // We found the name of a command that may define a cache
                            // variable.  Now, look for an opening parenthesis.
                            state = VariableParseState.NeedParen;
                        }
                    }
                    else if (state == VariableParseState.NeedParen &&
                        tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        // We found the opening parenthesis after the command name.  Now,
                        // look for the variable name.
                        state = VariableParseState.NeedVariable;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        tokenInfo.Token == (int)CMakeToken.Identifier)
                    {
                        // We found the variable name.  For the SET command, remember it
                        // and look for the CACHE keyword.  For the OPTION command, just
                        // go ahead and add it.
                        if (CMakeScanner.GetLastCommand(scannerState) == CMakeCommandId.Set)
                        {
                            possibleVariable = tokenText;
                            state = VariableParseState.NeedCache;
                        }
                        else
                        {
                            vars.Add(tokenText);
                            state = VariableParseState.NeedCommand;
                        }
                    }
                    else if (state == VariableParseState.NeedCache &&
                        tokenInfo.Token == (int)CMakeToken.Keyword &&
                        tokenText == "CACHE")
                    {
                        // We found the CACHE keyword.  Add the variable to the list and
                        // begin looking for the next one.
                        vars.Add(possibleVariable);
                        possibleVariable = null;
                        state = VariableParseState.NeedCommand;
                    }
                    else if (tokenInfo.Token == (int)CMakeToken.CloseParen)
                    {
                        possibleVariable = null;
                        state = VariableParseState.NeedCommand;
                    }
                }
            }
            return vars;
        }

        /// <summary>
        /// Parse to find the definition of a given variable.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <param name="variable">The name of the variable to find.</param>
        /// <param name="textSpan">
        /// A text span object that will be set to the range of text containing the name
        /// of the variable in its definition.
        /// </param>
        /// <returns>
        /// True if the definition of the variable was found or false otherwise.
        /// </returns>
        public static bool ParseForVariableDefinition(IEnumerable<string> lines,
            string variable, out TextSpan textSpan)
        {
            textSpan = new TextSpan();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            bool advanceAtWhiteSpace = false;
            int paramsBeforeVariable = 0;
            string possibleVariable = null;
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    if (state == VariableParseState.NeedCommand &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                        if (_paramsBeforeVariable.ContainsKey(id))
                        {
                            // We found the name of a command that defines a variable.  Now,
                            // look for an opening parenthesis.
                            state = VariableParseState.NeedParen;
                            advanceAtWhiteSpace = false;
                            paramsBeforeVariable = _paramsBeforeVariable[id];
                        }
                    }
                    else if (state == VariableParseState.NeedParen &&
                        tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        // We found the opening parenthesis after the command name.  Now,
                        // look for the variable name, possibly after some other parameters.
                        state = VariableParseState.NeedVariable;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        possibleVariable != null &&
                        (tokenInfo.Token == (int)CMakeToken.WhiteSpace ||
                        tokenInfo.Token == (int)CMakeToken.CloseParen))
                    {
                        // We found the variable name.  Return and pass its span back
                        // to the caller.
                        state = VariableParseState.NeedCommand;
                        return true;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        (tokenInfo.Token == (int)CMakeToken.Identifier ||
                        tokenInfo.Token == (int)CMakeToken.NumericIdentifier))
                    {
                        if (paramsBeforeVariable == 0)
                        {
                            if (tokenText.Equals(variable))
                            {
                                // We found what appears to be the variable defintion.
                                // If it isn't followed by a variable start token, we
                                // will return success.  Store the span.
                                possibleVariable = tokenText;
                                textSpan.iStartLine = i;
                                textSpan.iStartIndex = tokenInfo.StartIndex;
                                textSpan.iEndLine = i;
                                textSpan.iEndIndex = tokenInfo.EndIndex;
                            }
                            else
                            {
                                // It's the wrong variable, so start over.
                                state = VariableParseState.NeedCommand;
                            }
                        }
                        else
                        {
                            // We found a parameter.
                            advanceAtWhiteSpace = true;
                        }
                    }
                    else if (tokenInfo.Token == (int)CMakeToken.WhiteSpace)
                    {
                        if (advanceAtWhiteSpace)
                        {
                            // We found whitespace after a parameter.  Advance to the next
                            // parameter.
                            advanceAtWhiteSpace = false;
                            if (paramsBeforeVariable > 0)
                            {
                                paramsBeforeVariable--;
                            }
                        }
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        (tokenInfo.Token == (int)CMakeToken.VariableStart ||
                        tokenInfo.Token == (int)CMakeToken.Variable ||
                        tokenInfo.Token == (int)CMakeToken.VariableEnd))
                    {
                        if (paramsBeforeVariable > 0)
                        {
                            // We found a variable as a parameter.  Advance to the next
                            // parameter at the next whitespace token.
                            advanceAtWhiteSpace = true;
                        }
                        else
                        {
                            // We found the variable name, and it is itself the value of
                            // another variable.  Don't add anything to the list.
                            state = VariableParseState.NeedCommand;
                            possibleVariable = null;
                        }
                    }
                    else
                    {
                        state = VariableParseState.NeedCommand;
                        possibleVariable = null;
                    }
                }
                if (state == VariableParseState.NeedParen)
                {
                    // If we reached the end of the line without finding the opening
                    // parenthesis following the command, then there is a syntax error
                    // and the variable declaration shouldn't be recognized.
                    state = VariableParseState.NeedCommand;
                    possibleVariable = null;
                }
                i++;
            }
            return false;
        }

        /// <summary>
        /// Parse to find the definition of a given function or macro.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <param name="function">The name of the function or macro to find.</param>
        /// <param name="textSpan">
        /// A text span object that will be set to the range of text containing the name
        /// of the function or macro in its definition.
        /// </param>
        /// <returns>
        /// True if the definition of the function or macro was found or false otherwise.
        /// </returns>
        public static bool ParseForFunctionDefinition(IEnumerable<string> lines,
            string function, out TextSpan textSpan)
        {
            textSpan = new TextSpan();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    if (state == VariableParseState.NeedCommand &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                        if (id == CMakeCommandId.Function || id == CMakeCommandId.Macro)
                        {
                            // We found the name of a command that defines a function or
                            // macro.  Now, look for an opening parenthesis.
                            state = VariableParseState.NeedParen;
                        }
                    }
                    else if (state == VariableParseState.NeedParen &&
                        tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        // We found the opening parenthesis after the command name.  Now,
                        // look for the function or macro name.
                        state = VariableParseState.NeedVariable;
                    }
                    else if (state == VariableParseState.NeedVariable &&
                        tokenInfo.Token == (int)CMakeToken.Identifier)
                    {
                        if (tokenText.Equals(function))
                        {
                            // We found the function or macro definition.
                            textSpan.iStartLine = i;
                            textSpan.iStartIndex = tokenInfo.StartIndex;
                            textSpan.iEndLine = i;
                            textSpan.iEndIndex = tokenInfo.EndIndex;
                            return true;
                        }
                        state = VariableParseState.NeedCommand;
                    }
                    else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace)
                    {
                        state = VariableParseState.NeedCommand;
                    }
                }
                if (state == VariableParseState.NeedParen)
                {
                    // If we reached the end of the line without finding the opening
                    // parenthesis following the command, then there is a syntax error
                    // and the function declaration shouldn't be recognized.
                    state = VariableParseState.NeedCommand;
                }
                i++;
            }
            return false;
        }

        /// <summary>
        /// Parse to find the identifier of the command that triggered a member
        /// selection parse request.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <param name="lineNum">
        /// The line number of the token that triggered the parse request.
        /// </param>
        /// <param name="startIndex">
        /// The start index of the token that triggered the parse request.
        /// </param>
        /// <returns>
        /// The command identifier of the command that triggered the parse request.
        /// </returns>
        public static CMakeCommandId ParseForTriggerCommandId(IEnumerable<string> lines,
            int lineNum, int startIndex)
        {
            int state = 0;
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (i == lineNum)
                    {
                        if (tokenInfo.StartIndex == startIndex)
                        {
                            return CMakeScanner.GetLastCommand(state);
                        }
                    }
                }
                i++;
            }
            return CMakeCommandId.Unspecified;
        }

        /// <summary>
        /// Structure to hold the results of a parameter information parsing operation.
        /// </summary>
        public struct ParameterInfoResult
        {
            /// <summary>
            /// The name of the command whose parameters were parsed.
            /// </summary>
            public string CommandName;

            /// <summary>
            /// The name of the subcommand whose parameters were parsed.
            /// </summary>
            public string SubcommandName;

            /// <summary>
            /// The range of text containing the name of the command.
            /// </summary>
            public TextSpan? CommandSpan;

            /// <summary>
            /// The range of text containing the opening parenthesis denoting the
            /// beginning of the command's parameters.
            /// </summary>
            public TextSpan? BeginSpan;

            /// <summary>
            /// A list of text span objects for the ranges of text containing the
            /// whitespace tokens separating the command's parameter.
            /// </summary>
            public List<TextSpan> SeparatorSpans;

            /// <summary>
            /// The range of text containing the closing parenthesis denoting the
            /// end of the command's parameters.
            /// </summary>
            public TextSpan? EndSpan;
        }

        // Internal states of the parameter name parsing mechanism
        private enum ParameterParseState
        {
            NeedCommand,
            NeedParen,
            NeedFunction,
            NeedParams
        }

        /// <summary>
        /// Parse to find the names of the parameters to a given function.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="function">
        /// The name of the function for which to find parameters.
        /// </param>
        /// <returns>
        /// A list of the function's parameters or null if the function could not be
        /// found.
        /// </returns>
        public static List<string> ParseForParameterNames(IEnumerable<string> lines,
            string function)
        {
            List<string> parameters = new List<string>();
            int scannerState = 0;
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            ParameterParseState state = ParameterParseState.NeedCommand;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    if (state == ParameterParseState.NeedCommand &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                        if (id == CMakeCommandId.Function || id == CMakeCommandId.Macro)
                        {
                            // We found the FUNCTION keyword or the MACRO keyword.  Now,
                            // look for the opening parenthesis.
                            state = ParameterParseState.NeedParen;
                        }
                    }
                    else if (state == ParameterParseState.NeedParen &&
                        tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        // We found the opening parenthesis of a function or macro
                        // definition.  Now, look for the function or macro name.
                        state = ParameterParseState.NeedFunction;
                    }
                    else if (state == ParameterParseState.NeedFunction &&
                        tokenInfo.Token == (int)CMakeToken.Identifier)
                    {
                        // We found the function or macro name.  If it's the one that
                        // we're looking for, look for parameters.  Otherwise, ignore the
                        // rest of the function or macro definition.
                        if (tokenText.ToUpper().Equals(function.ToUpper()))
                        {
                            state = ParameterParseState.NeedParams;
                        }
                        else
                        {
                            state = ParameterParseState.NeedCommand;
                        }
                    }
                    else if (state == ParameterParseState.NeedParams &&
                        tokenInfo.Token == (int)CMakeToken.Identifier)
                    {
                        // We found a parameter.  Add it to the list and continue.
                        parameters.Add(tokenText);
                    }
                    else if (state == ParameterParseState.NeedParams &&
                        tokenInfo.Token == (int)CMakeToken.CloseParen)
                    {
                        // We found the closing parenthesis marking the end of the
                        // function or macro definition.  All the parameters have now
                        // been parsed, so return them.
                        return parameters;
                    }
                    else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                        tokenInfo.Token != (int)CMakeToken.Comment)
                    {
                        state = ParameterParseState.NeedCommand;
                    }
                }
                if (state == ParameterParseState.NeedParen)
                {
                    // There may not be a line break between a command and the opening
                    // parenthesis following.  If there is, don't recognize this as a
                    // valid function definition.
                    state = ParameterParseState.NeedCommand;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse to find the needed information for the command that triggered a
        /// parameter information parse request.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <param name="lineNum">
        /// The line number of the token that triggered the parse request.
        /// </param>
        /// <param name="endIndex">
        /// The end index of the token that triggered the parse request.
        /// </param>
        /// <param name="namesOnly">
        /// Flag indicating whether to find only the command and subcommand names,
        /// skipping all other information.
        /// </param>
        /// <returns>A structure containing the result of the parse operation.</returns>
        public static ParameterInfoResult ParseForParameterInfo(
            IEnumerable<string> lines, int lineNum, int endIndex,
            bool namesOnly = false)
        {
            ParameterInfoResult result = new ParameterInfoResult();
            result.SeparatorSpans = new List<TextSpan>();
            int state = 0;
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int i = 0;
            string lineFound = null;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                if (i == lineNum)
                {
                    lineFound = line;
                    break;
                }
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
                i++;
            }
            if (i != lineNum)
            {
                return result;
            }
            TextSpan lastCommandSpan = new TextSpan();
            string commandText = null;
            string subcommandText = null;
            bool lastWasCommand = false;
            bool insideCommand = false;
            bool needSubcommand = false;
            int parenDepth = 0;
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
            {
                if (endIndex >= 0 && tokenInfo.StartIndex > endIndex)
                {
                    // Stop parsing once we pass the token that triggered the parsing
                    // request.  Failure to do this will cause the arrow keys to not
                    // update the highlighted parameter properly.
                    break;
                }
                if (tokenInfo.Token == (int)CMakeToken.Keyword ||
                    tokenInfo.Token == (int)CMakeToken.Identifier)
                {
                    // Handle commands and subcommands.
                    if (!CMakeScanner.InsideParens(state))
                    {
                        lastCommandSpan.iStartLine = lineNum;
                        lastCommandSpan.iStartIndex = tokenInfo.StartIndex;
                        lastCommandSpan.iEndLine = lineNum;
                        lastCommandSpan.iEndIndex = tokenInfo.EndIndex;
                        commandText = lineFound.ExtractToken(tokenInfo);
                        lastWasCommand = true;
                    }
                    else if (needSubcommand && subcommandText == null)
                    {
                        lastCommandSpan.iEndLine = lineNum;
                        lastCommandSpan.iEndIndex = tokenInfo.EndIndex;
                        subcommandText = lineFound.ExtractToken(tokenInfo);
                    }
                }
                else if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                {
                    if (lastWasCommand)
                    {
                        // If the command takes subcommands, the subcommand will appear
                        // after the opening parenthesis before the parameters.
                        // Otherwise, the opening parenthesis marks the beginning of the
                        // parameters.
                        if (CMakeSubcommandMethods.HasSubcommands(
                            CMakeScanner.GetLastCommand(state)))
                        {
                            subcommandText = null;
                            needSubcommand = true;
                        }
                        else
                        {
                            result.CommandName = commandText;
                            if (namesOnly)
                            {
                                break;
                            }
                            result.CommandSpan = lastCommandSpan;
                            result.BeginSpan = new TextSpan()
                            {
                                iStartLine = lineNum,
                                iStartIndex = tokenInfo.StartIndex,
                                iEndLine = lineNum,
                                iEndIndex = tokenInfo.EndIndex
                            };
                            needSubcommand = false;
                        }
                        lastWasCommand = false;
                        insideCommand = true;
                    }
                    parenDepth++;
                }
                else if (tokenInfo.Token == (int)CMakeToken.WhiteSpace)
                {
                    if (parenDepth == 1 && insideCommand)
                    {
                        // Whitespace following a subcommand name marks the beginning
                        // of the parameters.  Otherwise, it may be a parameter
                        // separator.
                        if ((tokenInfo.Trigger & TokenTriggers.ParameterNext) != 0)
                        {
                            TextSpan spaceSpan = new TextSpan()
                            {
                                iStartLine = lineNum,
                                iStartIndex = tokenInfo.StartIndex,
                                iEndLine = lineNum,
                                iEndIndex = tokenInfo.EndIndex
                            };
                            result.SeparatorSpans.Add(spaceSpan);
                        }
                        else if ((tokenInfo.Trigger & TokenTriggers.ParameterStart) != 0)
                        {
                            result.CommandName = commandText;
                            result.SubcommandName = subcommandText;
                            if (namesOnly)
                            {
                                break;
                            }
                            result.CommandSpan = lastCommandSpan;
                            result.BeginSpan = new TextSpan()
                            {
                                iStartLine = lineNum,
                                iStartIndex = tokenInfo.StartIndex,
                                iEndLine = lineNum,
                                iEndIndex = tokenInfo.EndIndex
                            };
                            needSubcommand = false;
                        }
                    }
                }
                else if (tokenInfo.Token == (int)CMakeToken.CloseParen)
                {
                    if (parenDepth > 0)
                    {
                        parenDepth--;
                        if (parenDepth == 0 && insideCommand)
                        {
                            result.EndSpan = new TextSpan()
                            {
                                iStartLine = lineNum,
                                iStartIndex = tokenInfo.StartIndex,
                                iEndLine = lineNum,
                                iEndIndex = tokenInfo.EndIndex
                            };
                            insideCommand = false;
                        }
                    }
                }
                else
                {
                    lastWasCommand = false;
                }
            }
            return result;
        }

        // Internal states of the function parsing mechanism
        private enum FunctionParseState
        {
            NotInFunction,
            NeedFunctionArgs,
            NeedMacroArgs,
            InsideFunctionArgs,
            InsideMacroArgs,
            InsideFunction,
            InsideMacro,
            NeedEndFunctionArgs,
            NeedEndMacroArgs,
            InsideEndFunctionArgs,
            InsideEndMacroArgs
        }

        /// <summary>
        /// Parse for the bodies of functions and macros defined in the given code.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <returns>
        /// A list of text span objects for the ranges of text containg the function
        /// bodies in the code.
        /// </returns>
        public static List<TextSpan> ParseForFunctionBodies(IEnumerable<string> lines)
        {
            // Parse for the bodies of functions and add them as hidden regions.
            List<TextSpan> results = new List<TextSpan>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            FunctionParseState state = FunctionParseState.NotInFunction;
            int startLine = -1;
            int startPos = -1;
            int i = 0;
            foreach (string line in lines)
            {
                int scannerState = 0;
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    switch (state)
                    {
                    case FunctionParseState.NotInFunction:
                        if (tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                            if (id == CMakeCommandId.Function)
                            {
                                state = FunctionParseState.NeedFunctionArgs;
                            }
                            else if (id == CMakeCommandId.Macro)
                            {
                                state = FunctionParseState.NeedMacroArgs;
                            }
                        }
                        break;
                    case FunctionParseState.NeedFunctionArgs:
                    case FunctionParseState.NeedMacroArgs:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = (state == FunctionParseState.NeedMacroArgs) ?
                                FunctionParseState.InsideMacroArgs :
                                FunctionParseState.InsideFunctionArgs;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace)
                        {
                            state = FunctionParseState.NotInFunction;
                        }
                        break;
                    case FunctionParseState.InsideFunctionArgs:
                    case FunctionParseState.InsideMacroArgs:
                        if (tokenInfo.Token == (int)CMakeToken.CloseParen &&
                            !CMakeScanner.InsideParens(scannerState))
                        {
                            state = (state == FunctionParseState.InsideMacroArgs) ?
                                FunctionParseState.InsideMacro :
                                FunctionParseState.InsideFunction;
                            startLine = i;
                            startPos = tokenInfo.EndIndex + 1;
                        }
                        break;
                    case FunctionParseState.InsideFunction:
                    case FunctionParseState.InsideMacro:
                        if (tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                            if (id == CMakeCommandId.EndFunction &&
                                state == FunctionParseState.InsideFunction)
                            {
                                state = FunctionParseState.NeedEndFunctionArgs;
                            }
                            else if (id == CMakeCommandId.EndMacro &&
                                state == FunctionParseState.InsideMacro)
                            {
                                state = FunctionParseState.NeedEndMacroArgs;
                            }
                            else if (id == CMakeCommandId.Function)
                            {
                                // Ignore incomplete function or macro.
                                state = FunctionParseState.NeedFunctionArgs;
                            }
                            else if (id == CMakeCommandId.Macro)
                            {
                                // Ignore incomplete function or macro.
                                state = FunctionParseState.NeedMacroArgs;
                            }
                        }
                        break;
                    case FunctionParseState.NeedEndFunctionArgs:
                    case FunctionParseState.NeedEndMacroArgs:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = (state == FunctionParseState.NeedEndMacroArgs) ?
                                FunctionParseState.InsideEndMacroArgs :
                                FunctionParseState.InsideEndFunctionArgs;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace)
                        {
                            state = FunctionParseState.NotInFunction;
                        }
                        break;
                    case FunctionParseState.InsideEndFunctionArgs:
                    case FunctionParseState.InsideEndMacroArgs:
                        if (tokenInfo.Token == (int)CMakeToken.CloseParen &&
                            !CMakeScanner.InsideParens(scannerState))
                        {
                            state = FunctionParseState.NotInFunction;
                            results.Add(new TextSpan()
                            {
                                iStartLine = startLine,
                                iStartIndex = startPos,
                                iEndLine = i,
                                iEndIndex = tokenInfo.EndIndex + 1
                            });
                        }
                        break;
                    }
                }
                if (state == FunctionParseState.NeedFunctionArgs ||
                    state == FunctionParseState.NeedMacroArgs ||
                    state == FunctionParseState.NeedEndFunctionArgs ||
                    state == FunctionParseState.NeedEndMacroArgs)
                {
                    // There must not be a line break between a command and the opening
                    // parenthesis following it.  If there is, the command is invalid and
                    // shouldn't be recognized by IntelliSense
                    state = FunctionParseState.NotInFunction;
                }
                i++;
            }
            return results;
        }

        // Internal states of the function name parsing mechanism
        private enum FunctionNameParseState
        {
            NeedKeyword,
            NeedOpenParen,
            NeedIdentifier
        }

        /// <summary>
        /// Parse for the names of all functions or macros defined in the given code.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <param name="findMacros">
        /// A Boolean value indicating whether to parse for the names of macros instead
        /// of functions.
        /// </param>
        /// <returns>A list of function of macro names.</returns>
        public static List<string> ParseForFunctionNames(IEnumerable<string> lines,
            bool findMacros)
        {
            CMakeCommandId commandSought = findMacros ? CMakeCommandId.Macro :
                CMakeCommandId.Function;
            List<string> results = new List<string>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            FunctionNameParseState state = FunctionNameParseState.NeedKeyword;
            int scannerState = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    switch (state)
                    {
                    case FunctionNameParseState.NeedKeyword:
                        if (tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                            if (id == commandSought)
                            {
                                state = FunctionNameParseState.NeedOpenParen;
                            }
                        }
                        break;
                    case FunctionNameParseState.NeedOpenParen:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = FunctionNameParseState.NeedIdentifier;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            state = FunctionNameParseState.NeedKeyword;
                        }
                        break;
                    case FunctionNameParseState.NeedIdentifier:
                        if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            if (tokenInfo.Token == (int)CMakeToken.Identifier)
                            {
                                results.Add(tokenText);
                            }
                            state = FunctionNameParseState.NeedKeyword;
                        }
                        break;
                    }
                }
                if (state == FunctionNameParseState.NeedOpenParen)
                {
                    // A line break may not appear between the command and the opening
                    // parenthesis that follows it.  If there is one, the function
                    // definition is illegal and should be ignored.
                    state = FunctionNameParseState.NeedKeyword;
                }
            }
            return results;
        }

        /// <summary>
        /// Parse for the names of all targets in the given code.
        /// </summary>
        /// <param name="lines">A collection of lines of code to parse.</param>
        /// <param name="findTests">
        /// Boolean value indicating whether to parse for tests instead of ordinary
        /// targets.
        /// </param>
        /// <returns>A list of target names.</returns>
        public static List<string> ParseForTargetNames(IEnumerable<string> lines,
            bool findTests = false)
        {
            List<string> results = new List<string>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            FunctionNameParseState state = FunctionNameParseState.NeedKeyword;
            int scannerState = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    switch (state)
                    {
                    case FunctionNameParseState.NeedKeyword:
                        if (tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                            if (findTests ? id == CMakeCommandId.AddTest :
                                (id == CMakeCommandId.AddExecutable ||
                                id == CMakeCommandId.AddLibrary))
                            {
                                state = FunctionNameParseState.NeedOpenParen;
                            }
                        }
                        break;
                    case FunctionNameParseState.NeedOpenParen:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = FunctionNameParseState.NeedIdentifier;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            state = FunctionNameParseState.NeedKeyword;
                        }
                        break;
                    case FunctionNameParseState.NeedIdentifier:
                        if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            if (tokenInfo.Token == (int)CMakeToken.Identifier)
                            {
                                results.Add(tokenText);
                            }
                            state = FunctionNameParseState.NeedKeyword;
                        }
                        break;
                    }
                }
                if (state == FunctionNameParseState.NeedOpenParen)
                {
                    // A line break may not appear between the command and the opening
                    // parenthesis that follows it.  If there is one, there is a syntax
                    // error and the target should be ignored.
                    state = FunctionNameParseState.NeedKeyword;
                }
            }
            return results;
        }

        /// <summary>
        /// Parse to see if there is an identifier at a given position.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="lineNum">
        /// The line number at which to look for an identifier.
        /// </param>
        /// <param name="col">The column at which to look for an identifier.</param>
        /// <param name="isVariable">
        /// A Boolean value that will be set to true is the identifier names a variable
        /// or false if it names a function or macro.
        /// </param>
        /// <returns>
        /// The text of the identifier if one was found or null otherwise.
        /// </returns>
        public static string ParseForIdentifier(IEnumerable<string> lines, int lineNum,
            int col, out bool isVariable)
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (i == lineNum && tokenInfo.StartIndex <= col &&
                        tokenInfo.EndIndex >= col)
                    {
                        // We found the token.
                        string tokenText = line.ExtractToken(tokenInfo);
                        if (tokenInfo.Token == (int)CMakeToken.Variable)
                        {
                            if (tokenInfo.StartIndex <= 0 ||
                                tokenInfo.EndIndex >= line.Length - 1 ||
                                line[tokenInfo.StartIndex - 1] != '{' ||
                                line[tokenInfo.EndIndex + 1] != '}')
                            {
                                isVariable = false;
                                return null;
                            }
                            isVariable = true;
                            return tokenText;
                        }
                        else if (tokenInfo.Token == (int)CMakeToken.Identifier &&
                            !CMakeScanner.InsideParens(state))
                        {
                            isVariable = false;
                            return tokenText;
                        }
                        break;
                    }
                }
                i++;
            }
            isVariable = false;
            return null;
        }

        /// <summary>
        /// Structure to hold the results of a token parsing operation.
        /// </summary>
        public struct TokenData
        {
            /// <summary>
            /// Information on the token found.
            /// </summary>
            public TokenInfo TokenInfo;

            /// <summary>
            /// Boolean value indicating whether the token is in parentheses.
            /// </summary>
            public bool InParens;

            /// <summary>
            /// The parameter index of the token, if it is a parameter.
            /// </summary>
            public int ParameterIndex;

            /// <summary>
            /// List of all parameters to the same command appearing prior to the token,
            /// if it is a parameter.
            /// </summary>
            public List<string> PriorParameters;

            /// <summary>
            /// Identifier of the command to which this token is a parameter, if it is a
            /// parameter.
            /// </summary>
            public CMakeCommandId Command;
        }

        /// <summary>
        /// Parse to find the token at a given position.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="lineNum">The line number at which to look for a token.</param>
        /// <param name="col">The column at which to look for a token.</param>
        /// <param name="tokenInfo">
        /// A token information structure that will be set to contain information on the
        /// token found.
        /// </param>
        /// <param name="inParens">
        /// A Boolean value that will be set to true if a token is found or false
        /// otherwise.
        /// </param>
        /// <returns>True if a token was found or false otherwise.</returns>
        public static bool ParseForToken(IEnumerable<string> lines, int lineNum,
            int col, out TokenData tokenData)
        {
            CMakeScanner scanner = new CMakeScanner();
            int state = 0;
            int i = 0;
            bool foundParameter = false;
            string parameterText = "";
            tokenData = new TokenData();
            tokenData.TokenInfo = new TokenInfo();
            tokenData.PriorParameters = new List<string>();
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenData.TokenInfo,
                    ref state))
                {
                    tokenData.InParens = CMakeScanner.InsideParens(state);
                    if (tokenData.InParens)
                    {
                        CMakeToken token = (CMakeToken)tokenData.TokenInfo.Token;
                        tokenData.Command = CMakeScanner.GetLastCommand(state);
                        if (token != CMakeToken.WhiteSpace &&
                            token != CMakeToken.Comment &&
                            token != CMakeToken.OpenParen)
                        {
                            foundParameter = true;
                            parameterText += line.ExtractToken(tokenData.TokenInfo);
                        }
                        else if (foundParameter && token == CMakeToken.WhiteSpace)
                        {
                            tokenData.PriorParameters.Add(parameterText);
                            parameterText = "";
                            ++tokenData.ParameterIndex;
                            foundParameter = false;
                        }
                    }
                    else
                    {
                        tokenData.PriorParameters.Clear();
                        parameterText = "";
                        foundParameter = false;
                        tokenData.ParameterIndex = 0;
                        tokenData.Command = CMakeCommandId.Unspecified;
                    }
                    if (i == lineNum && tokenData.TokenInfo.StartIndex <= col &&
                        tokenData.TokenInfo.EndIndex >= col)
                    {
                        // We found the token.
                        return true;
                    }
                }
                if (foundParameter)
                {
                    // Handle the case where the parameters are separated by newlines
                    // without any whitespace.  (It's ugly code, but it's syntactically
                    // correct, so it needs to be handled properly.)
                    tokenData.PriorParameters.Add(parameterText);
                    parameterText = "";
                    ++tokenData.ParameterIndex;
                    foundParameter = false;
                }
                i++;
            }
            tokenData.InParens = false;
            return false;
        }

        /// <summary>
        /// A pair of matched text spans.
        /// </summary>
        public struct SpanPair
        {
            /// <summary>
            /// The first text span.
            /// </summary>
            public TextSpan First;

            /// <summary>
            /// The second text span.
            /// </summary>
            public TextSpan Second;
        }

        /// <summary>
        /// Parse for pairs of matching parentheses.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>A list of pairs of matching parentheses.</returns>
        public static List<SpanPair> ParseForParens(IEnumerable<string> lines)
        {
            List<SpanPair> pairs = new List<SpanPair>();
            Stack<TextSpan> stack = new Stack<TextSpan>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                    {
                        stack.Push(tokenInfo.ToTextSpan(i));
                    }
                    else if (tokenInfo.Token == (int)CMakeToken.CloseParen &&
                        stack.Count > 0)
                    {
                        pairs.Add(new SpanPair()
                        {
                            First = stack.Pop(),
                            Second = tokenInfo.ToTextSpan(i)
                        });
                    }
                }
                i++;
            }
            return pairs;
        }

        /// <summary>
        /// Parse for pairs of matching braces denoting references to variables.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="lineNum">The number of the line on which to find braces.</param>
        /// <returns>A list of pairs of matching braces.</returns>
        public static List<SpanPair> ParseForVariableBraces(IEnumerable<string> lines,
            int lineNum)
        {
            List<SpanPair> pairs = new List<SpanPair>();
            Stack<TextSpan> stack = new Stack<TextSpan>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                if (i < lineNum)
                {
                    while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
                }
                else
                {
                    while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                    {
                        switch ((CMakeToken)tokenInfo.Token)
                        {
                        case CMakeToken.VariableStart:
                        case CMakeToken.VariableStartEnv:
                        case CMakeToken.VariableStartCache:
                        case CMakeToken.VariableStartSetEnv:
                            stack.Push(tokenInfo.ToTextSpan(i));
                            break;
                        case CMakeToken.VariableEnd:
                            if (stack.Count > 0)
                            {
                                pairs.Add(new SpanPair()
                                {
                                    First = stack.Pop(),
                                    Second = tokenInfo.ToTextSpan(i)
                                });
                            }
                            break;
                        case CMakeToken.Variable:
                            break;
                        default:
                            stack.Clear();
                            break;
                        }
                    }
                }
                i++;
            }
            return pairs;
        }

        private enum IncludeParseState
        {
            BeforeCommand,
            BeforeParen,
            BeforeParenPackage,
            InsideParens,
            InsideParensPackage
        }

        /// <summary>
        /// Parse for all files include by the specified lines.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>A list of include files.</returns>
        public static List<string> ParseForIncludes(IEnumerable<string> lines)
        {
            List<string> results = new List<string>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            IncludeParseState state = IncludeParseState.BeforeCommand;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    switch (state)
                    {
                    case IncludeParseState.BeforeCommand:
                        if (!CMakeScanner.InsideParens(scannerState) &&
                            tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            CMakeCommandId id = CMakeKeywords.GetCommandId(
                                line.ExtractToken(tokenInfo));
                            if (id == CMakeCommandId.Include)
                            {
                                state = IncludeParseState.BeforeParen;
                            }
                            else if (id == CMakeCommandId.FindPackage)
                            {
                                state = IncludeParseState.BeforeParenPackage;
                            }
                        }
                        break;
                    case IncludeParseState.BeforeParen:
                    case IncludeParseState.BeforeParenPackage:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = (state == IncludeParseState.BeforeParenPackage) ?
                                IncludeParseState.InsideParensPackage :
                                IncludeParseState.InsideParens;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace)
                        {
                            state = IncludeParseState.BeforeCommand;
                        }
                        break;
                    case IncludeParseState.InsideParens:
                    case IncludeParseState.InsideParensPackage:
                        if (tokenInfo.Token == (int)CMakeToken.Identifier ||
                            tokenInfo.Token == (int)CMakeToken.FileName)
                        {
                            string prefix =
                                (state == IncludeParseState.InsideParensPackage) ?
                                "Find" : "";
                            results.Add(prefix + line.ExtractToken(tokenInfo));
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            state = IncludeParseState.BeforeCommand;
                        }
                        break;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Check whether the line following the specified line should be indented.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="lineNum">The number of the line to check.</param>
        /// <returns>
        /// True if the following line should be indented, or false otherwise.
        /// </returns>
        public static bool ShouldIndent(IEnumerable<string> lines, int lineNum)
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int i = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                if (i < lineNum)
                {
                    while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
                }
                else
                {
                    if (CMakeScanner.InsideParens(state) ||
                        CMakeScanner.GetStringFlag(state))
                    {
                        return false;
                    }
                    while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
                    return CMakeScanner.InsideParens(state) &&
                        !CMakeScanner.GetStringFlag(state);
                }
                i++;
            }
            return false;
        }

        /// <summary>
        /// Check whether the line following the specified line should be unindented.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="lineNum">The number of the line to check.</param>
        /// <param name="lineToMatch">
        /// Variable to receive the number of the line that the following line should be
        /// unindented to match.
        /// </param>
        /// <returns>
        /// True if the following line should be unindented, or false otherwise.
        /// </returns>
        public static bool ShouldUnindent(IEnumerable<string> lines, int lineNum,
            out int lineToMatch)
        {
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int i = 0;
            int openParenLine = -1;
            int openStringLine = -1;
            lineToMatch = -1;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                if (i < lineNum)
                {
                    bool wasInParens = CMakeScanner.InsideParens(state);
                    bool wasInString = CMakeScanner.GetStringFlag(state);
                    while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
                    if (!wasInParens && CMakeScanner.InsideParens(state))
                    {
                        openParenLine = i;
                    }
                    if (!wasInString && CMakeScanner.GetStringFlag(state))
                    {
                        openStringLine = i;
                    }
                }
                else
                {
                    bool wasInParens = CMakeScanner.InsideParens(state);
                    bool wasInString = CMakeScanner.GetStringFlag(state);
                    while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
                    if (CMakeScanner.GetStringFlag(state))
                    {
                        // Inside multiline strings, always unindent all the way.
                        lineToMatch = -1;
                        return true;
                    }
                    if (wasInParens && !CMakeScanner.InsideParens(state))
                    {
                        lineToMatch = openParenLine;
                        return true;
                    }
                    if (wasInString)
                    {
                        lineToMatch = openStringLine;
                        return true;
                    }
                    return false;
                }
                i++;
            }
            return false;
        }

        /// <summary>
        /// Determine the indentation level of the given line.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <param name="indentChar">
        /// The character used for indentation, either a space or a tab.
        /// </param>
        /// <returns>The indentation level of the specified line.</returns>
        public static int GetIndentationLevel(string line, char indentChar)
        {
            int i = 0;
            while (i < line.Length)
            {
                if (line[i] != indentChar)
                {
                    break;
                }
                i++;
            }
            return i;
        }

        /// <summary>
        /// Find the last line up to and including the specified line that is not empty.
        /// </summary>
        /// <param name="lines">A list of lines.</param>
        /// <param name="lineNum">The line number of the last line to consider.</param>
        /// <returns>The line number of the last line that is not empty.</returns>
        public static int GetLastNonEmptyLine(List<string> lines, int lineNum)
        {
            for (int i = lineNum; i > 0; i--)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        // Internal states of the bad variable reference parsing mechanism
        private enum BadVariableRefParseState
        {
            NeedStart,
            NeedName,
            NeedEnd
        }

        /// <summary>
        /// Parse for syntax errors in variable references.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>
        /// A list of error information.
        /// </returns>
        public static List<CMakeErrorInfo> ParseForBadVariableRefs(IEnumerable<string> lines)
        {
            List<CMakeErrorInfo> results = new List<CMakeErrorInfo>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            BadVariableRefParseState state = BadVariableRefParseState.NeedStart;
            Stack<TextSpan> stack = new Stack<TextSpan>();
            int scannerState = 0;
            int lineNum = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    switch ((CMakeToken)tokenInfo.Token)
                    {
                    case CMakeToken.VariableStart:
                    case CMakeToken.VariableStartEnv:
                    case CMakeToken.VariableStartCache:
                    case CMakeToken.VariableStartSetEnv:
                        stack.Push(tokenInfo.ToTextSpan(lineNum));
                        state = BadVariableRefParseState.NeedName;
                        break;
                    case CMakeToken.Variable:
                    case CMakeToken.Identifier:
                        if (state == BadVariableRefParseState.NeedName)
                        {
                            state = BadVariableRefParseState.NeedEnd;
                        }
                        break;
                    case CMakeToken.VariableEnd:
                        if (state == BadVariableRefParseState.NeedEnd)
                        {
                            stack.Pop();
                            state = stack.Count > 0 ? BadVariableRefParseState.NeedEnd :
                                BadVariableRefParseState.NeedStart;
                        }
                        else if (state == BadVariableRefParseState.NeedStart)
                        {
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.InvalidVariableRef,
                                Span = tokenInfo.ToTextSpan(lineNum)
                            });
                        }
                        else if (state == BadVariableRefParseState.NeedName)
                        {
                            TextSpan span = stack.Pop();
                            span.iEndIndex = tokenInfo.EndIndex + 1;
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.InvalidVariableRef,
                                Span = span
                            });
                        }
                        break;
                    default:
                        HandlePossibleBadVariableRef(ref state, tokenInfo.StartIndex,
                            stack, results);
                        break;
                    }
                }
                HandlePossibleBadVariableRef(ref state, line.Length, stack, results);
                lineNum++;
            }
            return results;
        }

        private static void HandlePossibleBadVariableRef(
            ref BadVariableRefParseState state, int curIndex,
            Stack<TextSpan> stack, List<CMakeErrorInfo> results)
        {
            if (state != BadVariableRefParseState.NeedStart)
            {
                while (stack.Count > 0)
                {
                    TextSpan span = stack.Pop();
                    span.iEndIndex = curIndex;
                    results.Add(new CMakeErrorInfo()
                    {
                        ErrorCode = CMakeError.InvalidVariableRef,
                        Span = span
                    });
                }
                state = BadVariableRefParseState.NeedStart;
            }
        }

        /// <summary>
        /// Parse for unmatched parentheses.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>
        /// A list of error information.
        /// </returns>
        public static List<CMakeErrorInfo> ParseForUnmatchedParens(
            IEnumerable<string> lines)
        {
            List<CMakeErrorInfo> results = new List<CMakeErrorInfo>();
            Stack<TextSpan> stack = new Stack<TextSpan>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int lineNum = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    switch ((CMakeToken)tokenInfo.Token)
                    {
                    case CMakeToken.OpenParen:
                        stack.Push(tokenInfo.ToTextSpan(lineNum));
                        break;
                    case CMakeToken.CloseParen:
                        if (stack.Count > 0)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.UnmatchedParen,
                                Span = tokenInfo.ToTextSpan(lineNum)
                            });
                        }
                        break;
                    }
                }
                lineNum++;
            }
            while (stack.Count > 0)
            {
                results.Add(new CMakeErrorInfo()
                {
                    ErrorCode = CMakeError.UnmatchedParen,
                    Span = stack.Pop()
                });
            }
            return results;
        }

        private enum BadCommandParseState
        {
            BeforeCommand,
            BeforeParen,
            InsideParens,
            AfterParen
        }

        /// <summary>
        /// Parse for bad commands.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>A list of error information.</returns>
        public static List<CMakeErrorInfo> ParseForBadCommands(IEnumerable<string> lines)
        {
            List<CMakeErrorInfo> results = new List<CMakeErrorInfo>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            int lineNum = 0;
            BadCommandParseState state = BadCommandParseState.BeforeCommand;
            foreach (string line in lines)
            {
                bool lineHasError = false;
                if (state != BadCommandParseState.InsideParens)
                {
                    state = BadCommandParseState.BeforeCommand;
                }
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    switch (state)
                    {
                    case BadCommandParseState.BeforeCommand:
                        if (tokenInfo.Token == (int)CMakeToken.Identifier ||
                            tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            state = BadCommandParseState.BeforeParen;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment &&
                            !lineHasError)
                        {
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.ExpectedCommand,
                                Span = tokenInfo.ToTextSpan(lineNum)
                            });
                            lineHasError = true;
                        }
                        break;
                    case BadCommandParseState.BeforeParen:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = BadCommandParseState.InsideParens;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace)
                        {
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.ExpectedOpenParen,
                                Span = tokenInfo.ToTextSpan(lineNum)
                            });
                            lineHasError = true;
                        }
                        break;
                    case BadCommandParseState.InsideParens:
                        if (!CMakeScanner.InsideParens(scannerState))
                        {
                            state = BadCommandParseState.AfterParen;
                        }
                        break;
                    case BadCommandParseState.AfterParen:
                        if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment &&
                            !lineHasError)
                        {
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.ExpectedEOL,
                                Span = tokenInfo.ToTextSpan(lineNum)
                            });
                            lineHasError = true;
                        }
                        break;
                    }
                }
                if (state == BadCommandParseState.BeforeParen)
                {
                    // If we reached the end of a line while looking for the opening
                    // parenthesis, show an error, since it must appear on the same line
                    // as the command name to not have a syntax error.
                    if (!lineHasError)
                    {
                        results.Add(new CMakeErrorInfo()
                        {
                            ErrorCode = CMakeError.ExpectedOpenParen,
                            Span = new TextSpan()
                            {
                                iStartLine = lineNum,
                                iStartIndex = tokenInfo.EndIndex + 1,
                                iEndLine = lineNum,
                                iEndIndex = tokenInfo.EndIndex + 2
                            }
                        });
                    }
                    state = BadCommandParseState.BeforeCommand;
                }
                lineNum++;
            }
            return results;
        }
        
        /// <summary>
        /// Parse for uses of deprecated commands.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>A list of error information.</returns>
        public static List<CMakeErrorInfo> ParseForDeprecatedCommands(
            IEnumerable<string> lines)
        {
            List<CMakeErrorInfo> results = new List<CMakeErrorInfo>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int lineNum = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (!CMakeScanner.InsideParens(state) &&
                        tokenInfo.Token == (int)CMakeToken.Keyword)
                    {
                        string tokenText = line.ExtractToken(tokenInfo);
                        CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                        if (CMakeKeywords.IsDeprecated(id))
                        {
                            results.Add(new CMakeErrorInfo()
                            {
                                ErrorCode = CMakeError.DeprecatedCommand,
                                Span = tokenInfo.ToTextSpan(lineNum),
                                Warning = true
                            });
                        }
                    }
                }
                lineNum++;
            }
            return results;
        }

        /// <summary>
        /// Parse for invalid escape sequences.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <returns>A list of error information.</returns>
        public static List<CMakeErrorInfo> ParseForInvalidEscapeSequences(
            IEnumerable<string> lines)
        {
            List<CMakeErrorInfo> results = new List<CMakeErrorInfo>();
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            int state = 0;
            int lineNum = 0;
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (tokenInfo.Token == (int)CMakeToken.String)
                    {
                        string tokenText = line.ExtractToken(tokenInfo);
                        for (int i = 0; i < tokenText.Length - 1; i++)
                        {
                            if (tokenText[i] == '\\')
                            {
                                char nextChar = tokenText[i + 1];
                                switch (nextChar)
                                {
                                case '\\':
                                case '"':
                                case ' ':
                                case '#':
                                case '(':
                                case ')':
                                case '$':
                                case '@':
                                case '^':
                                case ';':
                                case 't':
                                case 'n':
                                case 'r':
                                case '0':
                                    // It's one of the characters enumerated in
                                    // cmCommandArgumentParserHelper.cxx in the CMake
                                    // source code and there a valid escape sequence.
                                    i++;
                                    break;
                                default:
                                    // Otherwise, it's an invalid escape sequence.
                                    results.Add(new CMakeErrorInfo()
                                    {
                                        ErrorCode = CMakeError.InvalidEscapeSequence,
                                        Span = new TextSpan()
                                        {
                                            iStartLine = lineNum,
                                            iStartIndex = tokenInfo.StartIndex + i,
                                            iEndLine = lineNum,
                                            iEndIndex = tokenInfo.StartIndex + i + 2
                                        }
                                    });
                                    break;
                                }
                            }
                        }
                    }
                }
                lineNum++;
            }
            return results;
        }

        /// <summary>
        /// Parse to find the name of the function that the specified line is part of.
        /// </summary>
        /// <param name="lines">A collection of lines to parse.</param>
        /// <param name="lineNum">The line number of the line to check.</param>
        /// <returns>
        /// The name of the function or macro that the specified line is part of, or null
        /// if the line is at global scope.
        /// </returns>
        public static string ParseForCurrentFunction(IEnumerable<string> lines,
            int lineNum)
        {
            string result = null;
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            FunctionNameParseState state = FunctionNameParseState.NeedKeyword;
            int scannerState = 0;
            int curLineNum = 0;
            foreach (string line in lines)
            {
                if (curLineNum == lineNum)
                {
                    return result;
                }
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo,
                    ref scannerState))
                {
                    string tokenText = line.ExtractToken(tokenInfo);
                    switch (state)
                    {
                    case FunctionNameParseState.NeedKeyword:
                        if (tokenInfo.Token == (int)CMakeToken.Keyword)
                        {
                            CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                            if (id == CMakeCommandId.Function ||
                                id == CMakeCommandId.Macro)
                            {
                                state = FunctionNameParseState.NeedOpenParen;
                            }
                            else if (id == CMakeCommandId.EndFunction ||
                                id == CMakeCommandId.EndMacro)
                            {
                                result = null;
                            }
                        }
                        break;
                    case FunctionNameParseState.NeedOpenParen:
                        if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                        {
                            state = FunctionNameParseState.NeedIdentifier;
                        }
                        else if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            state = FunctionNameParseState.NeedKeyword;
                        }
                        break;
                    case FunctionNameParseState.NeedIdentifier:
                        if (tokenInfo.Token != (int)CMakeToken.WhiteSpace &&
                            tokenInfo.Token != (int)CMakeToken.Comment)
                        {
                            result = tokenText;
                        }
                        state = FunctionNameParseState.NeedKeyword;
                        break;
                    }
                }
                if (state == FunctionNameParseState.NeedOpenParen)
                {
                    // A line break may not appear between the command and the opening
                    // parenthesis that follows it.  If there is one, the function
                    // definition is illegal and should be ignored.
                    state = FunctionNameParseState.NeedKeyword;
                }
                curLineNum++;
            }
            return null;
        }
    }
}
