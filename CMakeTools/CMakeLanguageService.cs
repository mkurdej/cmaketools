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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Language service for CMake.
    /// </summary>
    [Guid(CMakeGuids.guidCMakeLanguageService)]
    public class CMakeLanguageService : LanguageService
    {
        private LanguagePreferences _preferences;

        // Delegate to parse for a particular type of error.
        private delegate List<CMakeErrorInfo> ParseForErrorMethod(
            IEnumerable<string> lines);

        // Delegate whether an error is enabled.
        private delegate bool ErrorEnabledMethod();

        // Array of error parsing methods.
        private readonly ParseForErrorMethod[] _parseForErrorMethods =
            new ParseForErrorMethod[]
        {
            CMakeParsing.ParseForBadVariableRefs,
            CMakeParsing.ParseForUnmatchedParens,
            CMakeParsing.ParseForBadCommands,
            CMakeParsing.ParseForDeprecatedCommands,
            CMakeParsing.ParseForInvalidEscapeSequences
        };

        // Map from error codes to text strings.
        private Dictionary<CMakeError, string> _errorStrings =
            new Dictionary<CMakeError, string>()
        {
            { CMakeError.InvalidVariableRef,    CMakeStrings.InvalidVariableRef },
            { CMakeError.UnmatchedParen,        CMakeStrings.UnmatchedParen },
            { CMakeError.ExpectedCommand,       CMakeStrings.ExpectedCommand },
            { CMakeError.ExpectedEOL,           CMakeStrings.ExpectedEOL },
            { CMakeError.ExpectedOpenParen,     CMakeStrings.ExpectedOpenParen },
            { CMakeError.DeprecatedCommand,     CMakeStrings.DeprecatedCommand },
            { CMakeError.InvalidEscapeSequence, CMakeStrings.InvalidEscapeSequence }
        };

        // Map from error codes to function checking if the errors are enabled.
        private Dictionary<CMakeError, ErrorEnabledMethod> _enabledMethods =
            new Dictionary<CMakeError, ErrorEnabledMethod>()
        {
            { CMakeError.DeprecatedCommand,     CMakePackage.IsDeprecatedWarningEnabled }
        };

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
            if (!CMakeSource.IsCMakeFile(req.FileName))
            {
                // Don't do IntelliSense parsing for ordinary text files.
                return scope;
            }
            CMakeSource source = (CMakeSource)GetSource(req.FileName);
            if (req.Sink.HiddenRegions)
            {
                req.Sink.ProcessHiddenRegions = true;
                List<TextSpan> regions = CMakeParsing.ParseForFunctionBodies(
                    source.GetLines());
                foreach (TextSpan textSpan in regions)
                {
                    req.Sink.AddHiddenRegion(textSpan);
                }
            }
            if (req.Sink.BraceMatching)
            {
                List<CMakeParsing.SpanPair> pairs = null;
                switch ((CMakeToken)req.TokenInfo.Token)
                {
                case CMakeToken.OpenParen:
                case CMakeToken.CloseParen:
                    pairs = CMakeParsing.ParseForParens(source.GetLines());
                    break;
                case CMakeToken.VariableStart:
                case CMakeToken.VariableStartEnv:
                case CMakeToken.VariableStartCache:
                case CMakeToken.VariableStartSetEnv:
                case CMakeToken.VariableEnd:
                    pairs = CMakeParsing.ParseForVariableBraces(source.GetLines(),
                        req.Line);
                    break;
                }
                if (pairs != null)
                {
                    foreach (CMakeParsing.SpanPair pair in pairs)
                    {
                        req.Sink.MatchPair(pair.First, pair.Second, 0);
                    }
                }
            }
            if (req.Reason == ParseReason.MemberSelect ||
                req.Reason == ParseReason.MemberSelectAndHighlightBraces)
            {
                // Set an appropriate declarations object depending on the token that
                // triggered member selection.
                CMakeToken token = (CMakeToken)req.TokenInfo.Token;
                if (token == CMakeToken.String)
                {
                    // If the token is a string and the user has began to reference a
                    // variable inside the string, treat the string as if it was the
                    // appropriate type of variable start token and display member
                    // selection for variables.
                    string line = source.GetLine(req.Line);
                    string tokenText = line.ExtractToken(req.TokenInfo);
                    if (tokenText.EndsWith("${"))
                    {
                        token = CMakeToken.VariableStart;
                    }
                    else if (tokenText.EndsWith("$ENV{"))
                    {
                        token = CMakeToken.VariableStartEnv;
                    }
                    else if (tokenText.EndsWith("$CACHE{"))
                    {
                        token = CMakeToken.VariableStartCache;
                    }
                }
                if (token == CMakeToken.VariableStart)
                {
                    List<string> vars = CMakeParsing.ParseForVariables(
                        source.GetLines(), req.Line);
                    CMakeVariableDeclarations decls = new CMakeVariableDeclarations(vars,
                        CMakeVariableType.Variable);
                    decls.AddItems(source.GetIncludeCacheVariables(),
                        CMakeItemDeclarations.ItemType.Variable);
                    string functionName = CMakeParsing.ParseForCurrentFunction(
                        source.GetLines(), req.Line);
                    if (functionName != null)
                    {
                        List<string> paramNames = CMakeParsing.ParseForParameterNames(
                            source.GetLines(), functionName);
                        paramNames.Add("ARGN");
                        decls.AddItems(paramNames,
                            CMakeItemDeclarations.ItemType.Variable);
                    }
                    scope.SetDeclarations(decls);
                }
                else if (token == CMakeToken.VariableStartEnv)
                {
                    List<string> vars = CMakeParsing.ParseForEnvVariables(
                        source.GetLines());
                    CMakeVariableDeclarations decls = new CMakeVariableDeclarations(vars,
                        CMakeVariableType.EnvVariable);
                    decls.AddItems(source.GetIncludeCacheEnvVariables(),
                        CMakeItemDeclarations.ItemType.Variable);
                    scope.SetDeclarations(decls);
                }
                else if (token == CMakeToken.VariableStartCache)
                {
                    List<string> vars = CMakeParsing.ParseForCacheVariables(
                        source.GetLines());
                    CMakeVariableDeclarations decls = new CMakeVariableDeclarations(vars,
                        CMakeVariableType.CacheVariable);
                    decls.AddItems(source.GetIncludeCacheCacheVariables(),
                        CMakeItemDeclarations.ItemType.Variable);
                    scope.SetDeclarations(decls);
                }
                else if (token == CMakeToken.Identifier)
                {
                    CMakeParsing.TokenData tokenData;
                    CMakeParsing.ParseForToken(source.GetLines(), req.Line,
                        req.TokenInfo.StartIndex, out tokenData);
                    if (!tokenData.InParens)
                    {
                        CMakeItemDeclarations decls = new CMakeItemDeclarations();
                        IEnumerable<string> commands = CMakeKeywords.GetAllCommands(
                            CMakePackage.Instance.CMakeOptionPage.ShowDeprecated);
                        if (!CMakePackage.Instance.CMakeOptionPage.CommandsLower)
                        {
                            commands = commands.Select(x => x.ToUpper());
                        }
                        decls.AddItems(commands, CMakeItemDeclarations.ItemType.Command);
                        decls.AddItems(
                            CMakeParsing.ParseForFunctionNames(source.GetLines(), false),
                            CMakeItemDeclarations.ItemType.Function);
                        decls.AddItems(
                            CMakeParsing.ParseForFunctionNames(source.GetLines(), true),
                            CMakeItemDeclarations.ItemType.Macro);
                        decls.AddItems(source.GetIncludeCacheFunctions(),
                            CMakeItemDeclarations.ItemType.Function);
                        decls.AddItems(source.GetIncludeCacheMacros(),
                            CMakeItemDeclarations.ItemType.Macro);
                        scope.SetDeclarations(decls);
                    }
                    else
                    {
                        Declarations decls = CMakeDeclarationsFactory.CreateDeclarations(
                            tokenData.Command, req, source,
                            tokenData.ParameterIndex > 0 ? tokenData.PriorParameters : null);
                        scope.SetDeclarations(decls);
                    }
                }
                else if (token == CMakeToken.OpenParen)
                {
                    CMakeCommandId id = CMakeParsing.ParseForTriggerCommandId(
                        source.GetLines(), req.Line, req.TokenInfo.StartIndex);
                    Declarations decls = CMakeDeclarationsFactory.CreateDeclarations(
                        id, req, source);
                    scope.SetDeclarations(decls);
                }
                else if (token == CMakeToken.WhiteSpace)
                {
                    CMakeParsing.TokenData tokenData;
                    CMakeParsing.ParseForToken(source.GetLines(), req.Line,
                        req.TokenInfo.StartIndex, out tokenData);
                    Declarations decls = CMakeDeclarationsFactory.CreateDeclarations(
                        tokenData.Command, req, source,
                        tokenData.ParameterIndex > 0 ? tokenData.PriorParameters : null);
                    scope.SetDeclarations(decls);
                }
            }
            else if (req.Reason == ParseReason.MethodTip)
            {
                CMakeParsing.ParameterInfoResult result =
                    CMakeParsing.ParseForParameterInfo(source.GetLines(), req.Line,
                    req.TokenInfo.EndIndex);
                if (result.CommandName != null && result.CommandSpan.HasValue)
                {
                    if (result.SubcommandName == null)
                    {
                        req.Sink.StartName(result.CommandSpan.Value, result.CommandName);
                    }
                    else
                    {
                        req.Sink.StartName(result.CommandSpan.Value,
                            result.CommandSpan + "(" + result.SubcommandName);
                    }
                    if (result.BeginSpan.HasValue)
                    {
                        req.Sink.StartParameters(result.BeginSpan.Value);
                    }
                    foreach (TextSpan span in result.SeparatorSpans)
                    {
                        req.Sink.NextParameter(span);
                    }
                    if (result.EndSpan.HasValue)
                    {
                        req.Sink.EndParameters(result.EndSpan.Value);
                    }
                    CMakeCommandId id = CMakeKeywords.GetCommandId(result.CommandName);
                    if (id == CMakeCommandId.Unspecified)
                    {
                        // If it's a user-defined function or macro, parse to try to find
                        // its parameters.
                        List<string> parameters = CMakeParsing.ParseForParameterNames(
                            source.GetLines(), result.CommandName);
                        if (parameters == null)
                        {
                            parameters = source.GetParametersFromIncludeCache(
                                result.CommandName);
                        }
                        if (parameters != null)
                        {
                            scope.SetMethods(new CMakeUserMethods(result.CommandName,
                                parameters));
                        }
                    }
                    else
                    {
                        scope.SetMethods(CMakeMethods.GetCommandParameters(id,
                            result.SubcommandName));
                    }
                }
            }
            else if (req.Reason == ParseReason.Goto)
            {
                scope.SetLines(source.GetLines());
                scope.SetFileName(req.FileName);
            }
            else if (req.Reason == ParseReason.QuickInfo)
            {
                scope.SetLines(source.GetLines());
            }
            else if (req.Reason == ParseReason.Check)
            {
                foreach (ParseForErrorMethod method in _parseForErrorMethods)
                {
                    List<CMakeErrorInfo> info = method(source.GetLines());
                    foreach (CMakeErrorInfo item in info)
                    {
                        CMakeError err = item.ErrorCode;
                        if (_errorStrings.ContainsKey(err) &&
                            (!_enabledMethods.ContainsKey(err) || _enabledMethods[err]()))
                        {
                            req.Sink.AddError(req.FileName, _errorStrings[err], item.Span,
                                item.Warning ? Severity.Warning : Severity.Error);
                        }
                    }
                }
                if (CMakePackage.Instance.CMakeOptionPage.ParseIncludedFiles)
                {
                    source.BuildIncludeCache(source.GetLines());
                    source.UpdateIncludeCache();
                    source.PruneIncludeCache();
                }
                else
                {
                    source.ClearIncludeCache();
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
            string path = FilePathUtilities.GetFilePath(buffer);
            return new CMakeScanner(!CMakeSource.IsCMakeFile(path));
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

        public override ExpansionFunction CreateExpansionFunction(
            ExpansionProvider provider, string functionName)
        {
            if (functionName == "ToCommandCase")
            {
                return new ToCommandCaseExpansionFunction(provider);
            }
            return base.CreateExpansionFunction(provider, functionName);
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            return new CMakeSource(this, buffer, GetColorizer(buffer));
        }

        public override ViewFilter CreateViewFilter(CodeWindowManager mgr,
            IVsTextView newView)
        {
            return new CMakeViewFilter(mgr, newView);
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

        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line,
            int col, TextSpan[] pCodeSpan)
        {
            // Block all breakpoints in CMake, since debugging is not supported.
            return VSConstants.S_FALSE;
        }
    }
}
