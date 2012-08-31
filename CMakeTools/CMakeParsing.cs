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
        private static Dictionary<CMakeCommandId, int> _paramsBeforeVariable =
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
            NeedVariable
        }

        private static void AddVariableToList(List<string> vars, string variable)
        {
            // This is a private helper function to handle adding a variable to a list
            // if it is not already there and is not a standard CMake variable.
            if (!CMakeVariableDeclarations.IsStandardVariable(variable))
            {
                if (vars.FindIndex(x => x.ToUpper().Equals(variable.ToUpper())) < 0)
                {
                    vars.Add(variable);
                }
            }
        }

        /// <summary>
        /// Parse to find all variables defined in the code.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>A list containing all variables defined in the code.</returns>
        public static List<string> ParseForVariables(IEnumerable<string> lines)
        {
            CMakeScanner scanner = new CMakeScanner();
            List<string> vars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            bool advanceAtWhiteSpace = false;
            int paramsBeforeVariable = 0;
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
                        // We found the variable name.  Add it to the list if it's not
                        // already there and isn't a standard variable.
                        state = VariableParseState.NeedCommand;
                        AddVariableToList(vars, possibleVariable);
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
                    AddVariableToList(vars, possibleVariable);
                    possibleVariable = null;
                }
                else if (state == VariableParseState.NeedParen)
                {
                    // If we reached the end of the line without finding the opening
                    // parenthesis finding the command, then there is a syntax error and
                    // the variable declaration shouldn't be recognized.
                    state = VariableParseState.NeedCommand;
                }
            }
            return vars;
        }

        /// <summary>
        /// Parse to find all environment variables defined in the code.
        /// </summary>
        /// <param name="code">The code to parse.</param>
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
                        if (!CMakeVariableDeclarations.IsStandardVariable(possibleVariable))
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
            int col, out TokenInfo tokenInfo, out bool inParens)
        {
            CMakeScanner scanner = new CMakeScanner();
            int state = 0;
            int i = 0;
            tokenInfo = new TokenInfo();
            foreach (string line in lines)
            {
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (i == lineNum && tokenInfo.StartIndex <= col &&
                        tokenInfo.EndIndex >= col)
                    {
                        // We found the token.
                        inParens = CMakeScanner.InsideParens(state);
                        return true;
                    }
                }
                i++;
            }
            inParens = false;
            return false;
        }
    }
}
