// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

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
            if (req.Reason == ParseReason.MemberSelect)
            {
                // Set an appropriate declarations object depending on the token that
                // triggered member selection.
                if (req.TokenInfo.Token == (int)CMakeToken.VariableStart)
                {
                    List<string> vars = ParseForVariables(req.Text);
                    scope.SetDeclarations(new CMakeVariableDeclarations(vars));
                }
                else if (req.TokenInfo.Token == (int)CMakeToken.OpenParen)
                {
                    CMakeCommandId id = ParseForTriggerCommandId(req);
                    scope.SetDeclarations(
                        CMakeSubcommandDeclarations.GetSubcommandDeclarations(id));
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

        public static List<string> ParseForVariables(string code)
        {
            // Parse to find all variables defined in the code.  This code is implemented
            // as a state machine.  It begins in state 0 and advances to state 1 upon
            // reading the SET command.  An opening parenthesis in state 1 will cause a
            // transition to state 2.  An identifier read while in state 2 will be added
            // as a variable unless it has already been added or is a standard variable.
            // All other tokens will cause a transition back to state 0.
            CMakeScanner scanner = new CMakeScanner();
            scanner.SetSource(code, 0);
            List<string> vars = new List<string>();
            TokenInfo tokenInfo = new TokenInfo();
            int scannerState = 0;
            int state = 0;
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref scannerState))
            {
                if (tokenInfo.Token == (int)CMakeToken.Keyword &&
                    code.Substring(tokenInfo.StartIndex,
                    tokenInfo.EndIndex - tokenInfo.StartIndex + 1).ToLower().Equals("set"))
                {
                    state = 1;
                }
                else if (state == 1 && tokenInfo.Token == (int)CMakeToken.OpenParen)
                {
                    state = 2;
                }
                else if (state == 2 && tokenInfo.Token == (int)CMakeToken.Identifier)
                {
                    state = 0;
                    string varName = code.Substring(tokenInfo.StartIndex,
                        tokenInfo.EndIndex - tokenInfo.StartIndex + 1);
                    if (!CMakeVariableDeclarations.IsStandardVariable(varName) &&
                        vars.FindIndex(x => x.ToUpper().Equals(varName.ToUpper())) < 0)
                    {
                        vars.Add(varName);
                    }
                }
                else
                {
                    state = 0;
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
    }
}
