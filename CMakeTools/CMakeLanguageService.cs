// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Language service for CMake.
    /// </summary>
    public class CMakeLanguageService : LanguageService
    {
        private LanguagePreferences _preferences;

        public override string GetFormatFilterList()
        {
            return "CMake Files (*.cmake)\n*.cmake";
        }

        public override string Name
        {
            get
            {
                return "CMake";
            }
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            CMakeAuthoringScope scope = new CMakeAuthoringScope();
            CMakeSource source = (CMakeSource)GetSource(req.FileName);
            if (req.Sink.HiddenRegions)
            {
                req.Sink.ProcessHiddenRegions = true;
                List<TextSpan> regions = ParseForFunctionBodies(source.GetLines());
                foreach (TextSpan textSpan in regions)
                {
                    req.Sink.AddHiddenRegion(textSpan);
                }
            }
            if (req.Reason == ParseReason.MemberSelect)
            {
                // Set an appropriate declarations object depending on the token that
                // triggered member selection.
                if (req.TokenInfo.Token == (int)CMakeToken.VariableStart)
                {
                    List<string> vars = ParseForVariables(req.Text);
                    scope.SetDeclarations(new CMakeVariableDeclarations(vars));
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.VariableStartEnv)
                {
                    List<string> vars = ParseForEnvVariables(req.Text);
                    scope.SetDeclarations(new CMakeVariableDeclarations(vars, true));
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.OpenParen)
                {
                    CMakeCommandId id = ParseForTriggerCommandId(req);
                    scope.SetDeclarations(
                        CMakeSubcommandDeclarations.GetSubcommandDeclarations(id));
                }
            }
            else if (req.Reason == ParseReason.MethodTip)
            {
                string commandText = ParseForParameterInfo(req);
                if (commandText != null)
                {
                    CMakeCommandId id = CMakeKeywords.GetCommandId(commandText);
                    if (id != CMakeCommandId.Unspecified)
                    {
                        scope.SetMethods(CMakeMethods.GetCommandParameters(id));
                    }
                }
            }
            return scope;
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            // Since Visual Studio handles language service associations by file
            // extension, CMakeLanguageService must handle all *.txt files in order to
            // handle CMakeLists.txt.  Detect if the buffer represents an ordinary text
            // files.  If so, disable syntax highlighting.  This is a kludge, but it's
            // the best that can be done here.
            bool textFile = true;
            string path = FilePathUtilities.GetFilePath(buffer);
            if (Path.GetExtension(path).ToLower() == ".cmake")
            {
                textFile = false;
            }
            else if (Path.GetExtension(path).ToLower() == ".txt")
            {
                if (Path.GetFileName(path).ToLower() == "cmakelists.txt")
                {
                    textFile = false;
                }
            }
            return new CMakeScanner(textFile);
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(Site,
                    typeof(CMakeLanguageService).GUID, Name);
                _preferences.Init();
            }
            return _preferences;
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            return new CMakeSource(this, buffer, GetColorizer(buffer));
        }

        public override void OnIdle(bool periodic)
        {
            // This code must be present in any language service that uses any of the
            // features that require a ParseReason of Check.
            Source source = GetSource(LastActiveTextView);
            if (source != null && source.LastParseTime >= Int32.MaxValue >> 12)
            {
                source.LastParseTime = 0;
            }
            base.OnIdle(periodic);
        }

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

        public static List<string> ParseForVariables(string code)
        {
            // Parse to find all variables defined in the code.
            CMakeScanner scanner = new CMakeScanner();
            scanner.SetSource(code, 0);
            List<string> vars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            bool advanceAtWhiteSpace = false;
            int paramsBeforeVariable = 0;
            string possibleVariable = null;
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
            {
                string tokenText = code.Substring(tokenInfo.StartIndex,
                    tokenInfo.EndIndex - tokenInfo.StartIndex + 1);
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
                    tokenInfo.Token == (int)CMakeToken.Identifier)
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
            return vars;
        }

        public static List<string> ParseForEnvVariables(string code)
        {
            // Parse to find all environment variables defined in the code.
            CMakeScanner scanner = new CMakeScanner();
            scanner.SetSource(code, 0);
            List<string> vars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            VariableParseState state = VariableParseState.NeedCommand;
            string possibleVariable = null;
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
            {
                string tokenText = code.Substring(tokenInfo.StartIndex,
                    tokenInfo.EndIndex - tokenInfo.StartIndex + 1);
                if (state == VariableParseState.NeedCommand &&
                    tokenInfo.Token == (int)CMakeToken.Keyword)
                {
                    if(CMakeKeywords.GetCommandId(tokenText) == CMakeCommandId.Set)
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
            return vars;
        }

        private CMakeCommandId ParseForTriggerCommandId(ParseRequest req)
        {
            // Parse to find the identifier of the command that triggered the current
            // member selection parse request.
            Source source = GetSource(req.FileName);
            int state = 0;
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            for (int lineNum = 0; lineNum <= req.Line; lineNum++)
            {
                string line = source.GetLine(lineNum);
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
                {
                    if (lineNum == req.Line)
                    {
                        if (tokenInfo.StartIndex == req.TokenInfo.StartIndex)
                        {
                            return CMakeScanner.GetLastCommand(state);
                        }
                    }
                }
            }
            return CMakeCommandId.Unspecified;
        }

        private string ParseForParameterInfo(ParseRequest req)
        {
            // Parse to find the needed information for the command that triggered the
            // current parameter information request.
            Source source = GetSource(req.FileName);
            int state = 0;
            CMakeScanner scanner = new CMakeScanner();
            TokenInfo tokenInfo = new TokenInfo();
            string line;
            for (int lineNum = 0; lineNum < req.Line; lineNum++)
            {
                line = source.GetLine(lineNum);
                scanner.SetSource(line, 0);
                while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state));
            }
            line = source.GetLine(req.Line);
            TextSpan lastCommandSpan = new TextSpan();
            string commandText = null;
            bool lastWasCommand = false;
            bool insideCommand = false;
            int parenDepth = 0;
            scanner.SetSource(line, 0);
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
            {
                if (tokenInfo.StartIndex > req.TokenInfo.EndIndex)
                {
                    // Stop parsing once we pass the token that triggered the parsing
                    // request.  Failure to do this will cause the arrow keys to not
                    // update the highlighted parameter properly.
                    break;
                }
                if (tokenInfo.Token == (int)CMakeToken.Keyword)
                {
                    if (!CMakeScanner.InsideParens(state))
                    {
                        lastCommandSpan.iStartLine = req.Line;
                        lastCommandSpan.iStartIndex = tokenInfo.StartIndex;
                        lastCommandSpan.iEndLine = req.Line;
                        lastCommandSpan.iEndIndex = tokenInfo.EndIndex;
                        commandText = line.Substring(tokenInfo.StartIndex,
                            tokenInfo.EndIndex - tokenInfo.StartIndex + 1).ToLower();
                        lastWasCommand = true;
                    }
                }
                else if (tokenInfo.Token == (int)CMakeToken.OpenParen)
                {
                    if (lastWasCommand)
                    {
                        CMakeCommandId id = CMakeKeywords.GetCommandId(commandText);
                        req.Sink.StartName(lastCommandSpan, commandText);
                        TextSpan parenSpan = new TextSpan();
                        parenSpan.iStartLine = req.Line;
                        parenSpan.iStartIndex = tokenInfo.StartIndex;
                        parenSpan.iEndLine = req.Line;
                        parenSpan.iEndIndex = tokenInfo.EndIndex;
                        req.Sink.StartParameters(parenSpan);
                        lastWasCommand = false;
                        insideCommand = true;
                    }
                    parenDepth++;
                }
                else if (tokenInfo.Token == (int)CMakeToken.WhiteSpace)
                {
                    if (parenDepth == 1 && insideCommand)
                    {
                        if ((tokenInfo.Trigger & TokenTriggers.ParameterNext) != 0)
                        {
                            TextSpan spaceSpan = new TextSpan();
                            spaceSpan.iStartIndex = req.Line;
                            spaceSpan.iStartIndex = tokenInfo.StartIndex;
                            spaceSpan.iEndLine = req.Line;
                            spaceSpan.iEndIndex = tokenInfo.EndIndex;
                            req.Sink.NextParameter(spaceSpan);
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
                            TextSpan parenSpan = new TextSpan();
                            parenSpan.iStartLine = req.Line;
                            parenSpan.iStartIndex = tokenInfo.StartIndex;
                            parenSpan.iEndLine = req.Line;
                            parenSpan.iEndIndex = tokenInfo.EndIndex;
                            req.Sink.EndParameters(parenSpan);
                            insideCommand = false;
                        }
                    }
                }
                else
                {
                    lastWasCommand = false;
                }
            }
            return commandText;
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
                    string tokenText = line.Substring(tokenInfo.StartIndex,
                        tokenInfo.EndIndex - tokenInfo.StartIndex + 1);
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
                i++;
            }
            return results;
        }
    }
}
