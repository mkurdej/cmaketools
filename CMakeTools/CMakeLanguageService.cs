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

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio;
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
        private CMakePackage _package;

        public CMakeLanguageService(CMakePackage package)
        {
            _package = package;
        }

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
                List<TextSpan> regions = CMakeParsing.ParseForFunctionBodies(
                    source.GetLines());
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
                    List<string> vars = CMakeParsing.ParseForVariables(
                        source.GetLines());
                    scope.SetDeclarations(new CMakeVariableDeclarations(vars));
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.VariableStartEnv)
                {
                    List<string> vars = CMakeParsing.ParseForEnvVariables(
                        source.GetLines());
                    scope.SetDeclarations(new CMakeVariableDeclarations(vars, true));
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.Identifier)
                {
                    List<string> functions = CMakeParsing.ParseForFunctionNames(
                        source.GetLines(), false);
                    List<string> macros = CMakeParsing.ParseForFunctionNames(
                        source.GetLines(), true);
                    bool commandsLower = _package.CMakeOptionPage.CommandsLower;
                    scope.SetDeclarations(new CMakeFunctionDeclarations(functions,
                        macros, commandsLower));
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.OpenParen)
                {
                    CMakeCommandId id = CMakeParsing.ParseForTriggerCommandId(
                        source.GetLines(), req.Line, req.TokenInfo.StartIndex);
                    if (id == CMakeCommandId.Include)
                    {
                        scope.SetDeclarations(new CMakeIncludeDeclarations(
                            req.FileName));
                    }
                    else if (id == CMakeCommandId.FindPackage)
                    {
                        scope.SetDeclarations(new CMakePackageDeclarations(
                            req.FileName));
                    }
                    else if (id == CMakeCommandId.AddSubdirectory)
                    {
                        bool requireCMakeLists =
                            (_package.CMakeOptionPage.ShowSubdirectories ==
                            CMakeOptionPage.SubdirectorySetting.CMakeListsOnly);
                        scope.SetDeclarations(new CMakeSubdirectoryDeclarations(
                            req.FileName, requireCMakeLists));
                    }
                    else
                    {
                        scope.SetDeclarations(
                            CMakeSubcommandDeclarations.GetSubcommandDeclarations(id));
                    }
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.WhiteSpace)
                {
                    scope.SetDeclarations(new CMakeSourceDeclarations(req.FileName));
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

        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line,
            int col, TextSpan[] pCodeSpan)
        {
            // Block all breakpoints in CMake, since debugging is not supported.
            return VSConstants.S_FALSE;
        }
    }
}
