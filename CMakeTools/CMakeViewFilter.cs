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
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    class CMakeViewFilter : ViewFilter
    {
        public CMakeViewFilter(CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view) {}

        protected override int QueryCommandStatus(ref Guid guidCmdGroup, uint nCmdId)
        {
            if (guidCmdGroup == VSConstants.VSStd2K &&
                CMakeSource.IsCMakeFile(Source.GetFilePath()))
            {
                if (nCmdId == (uint)VSConstants.VSStd2KCmdID.INSERTSNIPPET)
                {
                    // Show and enable the Insert Snippet command.
                    return (int)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
                }
                else if (nCmdId == (uint)VSConstants.VSStd2KCmdID.OPENFILE)
                {
                    // Show and enable the Open File command if the current token is a
                    // file name.
                    string extraSearchPath;
                    if (GetCurrentTokenFileName(out extraSearchPath) != null)
                    {
                        return (int)(OLECMDF.OLECMDF_SUPPORTED |
                            OLECMDF.OLECMDF_ENABLED);
                    }
                }
            }
            else if (guidCmdGroup == VSConstants.GUID_VSStandardCommandSet97 &&
                !CMakeSource.IsCMakeFile(Source.GetFilePath()))
            {
                // Visual Studio does not show these commands for ordinary text files.
                // All text files get associated with the CMake language service in order
                // to facilitate handling CMakeLists.txt.  When the current file is an
                // ordinary text file, hide these commands to match what Visual Studio
                // would otherwise do.
                if (nCmdId == (uint)VSConstants.VSStd97CmdID.GotoDecl ||
                    nCmdId == (uint)VSConstants.VSStd97CmdID.GotoDefn ||
                    nCmdId == (uint)VSConstants.VSStd97CmdID.GotoRef)
                {
                    return (int)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE);
                }
            }
            return base.QueryCommandStatus(ref guidCmdGroup, nCmdId);
        }

        public override bool HandlePreExec(ref Guid guidCmdGroup, uint nCmdId,
            uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (guidCmdGroup == VSConstants.VSStd2K)
            {
                if (nCmdId == (uint)VSConstants.VSStd2KCmdID.INSERTSNIPPET)
                {
                    // Handle the Insert Snippet command by showing the UI to insert a snippet.
                    ExpansionProvider ep = GetExpansionProvider();
                    if (ep != null && TextView != null)
                    {
                        ep.DisplayExpansionBrowser(TextView, CMakeStrings.CMakeSnippet, null,
                            false, null, false);
                    }
                    return true;
                }
                else if (nCmdId == (uint)VSConstants.VSStd2KCmdID.OPENFILE)
                {
                    // Handle the Open File by opening the file specified by the current
                    // token.
                    string extraSearchPath;
                    string fileName = GetCurrentTokenFileName(out extraSearchPath);
                    if (fileName == null)
                    {
                        return false;
                    }
                    string curFileDir = Path.GetDirectoryName(Source.GetFilePath());
                    string filePath = Path.Combine(curFileDir, fileName);
                    if (!File.Exists(filePath))
                    {
                        filePath = null;
                        if (extraSearchPath != null)
                        {
                            filePath = Path.Combine(extraSearchPath, fileName);
                            if (!File.Exists(filePath))
                            {
                                filePath = null;
                            }
                        }
                    }
                    if (filePath != null)
                    {
                        VsShellUtilities.OpenDocument(ServiceProvider.GlobalProvider,
                            filePath);
                    }
                    else
                    {
                        MessageBox.Show(string.Format(CMakeStrings.FileNotFound,
                            fileName), CMakeStrings.MessageBoxTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    return true;
                }
                else if (nCmdId == (uint)VSConstants.VSStd2KCmdID.TAB)
                {
                    // Handle the tab key when the caret is positioned immediately after
                    // the name of a snippet by inserting that snippet.
                    ExpansionProvider ep = GetExpansionProvider();
                    if (ep == null || TextView == null || ep.InTemplateEditingMode)
                    {
                        return false;
                    }
                    int line;
                    int col;
                    if (TextView.GetCaretPos(out line, out col) != VSConstants.S_OK)
                    {
                        return false;
                    }
                    TokenInfo tokenInfo = Source.GetTokenInfo(line, col);
                    if (tokenInfo.StartIndex == tokenInfo.EndIndex)
                    {
                        return false;
                    }
                    TextSpan span = tokenInfo.ToTextSpan(line);
                    string shortcut = Source.GetText(span);
                    if (string.IsNullOrEmpty(shortcut))
                    {
                        return false;
                    }
                    string title;
                    string path;
                    if (ep.FindExpansionByShortcut(TextView, shortcut, span, true,
                        out title, out path) != VSConstants.S_OK)
                    {
                        return false;
                    }
                    return ep.InsertNamedExpansion(TextView, title, path, span, false);
                }
                else if (nCmdId == (uint)VSConstants.VSStd2KCmdID.RETURN)
                {
                    // Dismiss method tips when the user presses enter.  They interfere
                    // with the proper functioning of smart indentation.
                    if (Source.IsCompletorActive && !Source.CompletionSet.IsDisplayed)
                    {
                        Source.DismissCompletor();
                    }
                }
            }
            else if (guidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                if (nCmdId == (uint)VSConstants.VSStd97CmdID.F1Help)
                {
                    DoContextHelp();
                    return true;
                }
            }
            return base.HandlePreExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn,
                pvaOut);
        }

        private string GetCurrentTokenFileName(out string extraSearchPath)
        {
            // Obtain the name of the file referenced by the current token if there is
            // one or null otherwise.
            int line;
            int col;
            extraSearchPath = null;
            if (TextView == null ||
                TextView.GetCaretPos(out line, out col) != VSConstants.S_OK)
            {
                return null;
            }
            TokenInfo tokenInfo = Source.GetTokenInfo(line, col);
            if (tokenInfo == null ||
                (tokenInfo.Token != (int)CMakeToken.FileName &&
                tokenInfo.Token != (int)CMakeToken.Identifier))
            {
                return null;
            }
            string text = Source.GetText(tokenInfo.ToTextSpan(line));
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            // If the string specifies a file name with an extension, just return it
            // regardless or where it is found.
            if (tokenInfo.Token == (int)CMakeToken.FileName &&
                text.IndexOf('.') >= 0)
            {
                return text;
            }

            // An identifier or path may reference a file if appears as a parameter to
            // one of certain commands, such as INCLUDE or FIND_PACKAGE.  Parse to find
            // the command to which the token is a parameter, if any, and handle it
            // accordingly.
            CMakeParsing.TokenData tokenData;
            if (!CMakeParsing.ParseForToken(Source.GetLines(), line, col,
                out tokenData) || !tokenData.InParens)
            {
                return null;
            }
            switch (tokenData.Command)
            {
            case CMakeCommandId.Include:
                if (tokenData.ParameterIndex == 0)
                {
                    extraSearchPath = CMakePath.FindCMakeModules();
                    return string.Format("{0}.cmake",
                        Source.GetText(tokenInfo.ToTextSpan(line)));
                }
                break;
            case CMakeCommandId.FindPackage:
                if (tokenData.ParameterIndex == 0)
                {
                    extraSearchPath = CMakePath.FindCMakeModules();
                    return string.Format("Find{0}.cmake",
                        Source.GetText(tokenInfo.ToTextSpan(line)));
                }
                break;
            case CMakeCommandId.AddSubdirectory:
                if (tokenData.ParameterIndex == 0)
                {
                    return Path.Combine(Source.GetText(tokenInfo.ToTextSpan(line)),
                        "CMakeLists.txt");
                }
                break;
            }
            return null;
        }

        private void DoContextHelp()
        {
            // Open CMake help for the command, standard variables, or standard module
            // at the caret.
            int line;
            int col;
            CMakeParsing.TokenData tokenData;
            if (TextView != null &&
                TextView.GetCaretPos(out line, out col) == VSConstants.S_OK &&
                CMakeParsing.ParseForToken(Source.GetLines(), line, col, out tokenData))
            {
                string text = Source.GetText(tokenData.TokenInfo.ToTextSpan(line));
                string[] htmlFiles = null;
                switch ((CMakeToken)tokenData.TokenInfo.Token)
                {
                case CMakeToken.Keyword:
                    if (!tokenData.InParens)
                    {
                        if (CMakeKeywords.IsDeprecated(CMakeKeywords.GetCommandId(text)))
                        {
                            htmlFiles = new string[]
                            {
                                "html\\command\\" + text.ToLower() + ".html",
                                "cmake-compatcommands.html#command:" + text.ToLower()
                            };
                        }
                        else
                        {
                            htmlFiles = new string[]
                            {
                                "html\\command\\" + text.ToLower() + ".html",
                                "cmake-commands.html#command:" + text.ToLower()
                            };
                        }
                    }
                    break;
                case CMakeToken.Variable:
                    if (CMakeVariableDeclarations.IsStandardVariable(text,
                        CMakeVariableType.Variable))
                    {
                        htmlFiles = new string[]
                        {
                            "html\\variable\\" + text + ".html",
                            "cmake-variables.html#variable:" + text
                        };
                    }
                    break;
                case CMakeToken.Identifier:
                    if (tokenData.Command == CMakeCommandId.Include)
                    {
                        htmlFiles = new string[]
                        {
                            "html\\module\\" + text + ".html",
                            "cmake-modules.html#module:" + text
                        };
                    }
                    else if (tokenData.Command == CMakeCommandId.FindPackage)
                    {
                        htmlFiles = new string[]
                        {
                            "html\\module\\Find" + text + ".html",
                            "cmake-modules.html#module:Find" + text
                        };
                    }
                    break;
                }
                if (htmlFiles != null)
                {
                    CMakePackage.Instance.OpenCMakeHelpPage(htmlFiles);
                }
            }
        }

        public override bool HandleSmartIndent()
        {
            // If the line contains an outer opening parenthesis, intent by one level.
            // If the line contains an outer closing parenthesis, unindent to match the
            // level of the corresponding opening parenthesis.  In all other cases,
            // match the indentation level of the previous line.
            LanguagePreferences prefs = Source.LanguageService.Preferences;
            char indentChar = prefs.InsertTabs ? '\t' : ' ';
            int line;
            int col;
            TextView.GetCaretPos(out line, out col);
            List<string> lines = Source.GetLines().ToList();
            int prevLine = CMakeParsing.GetLastNonEmptyLine(lines, line - 1);
            int level = CMakeParsing.GetIndentationLevel(lines[prevLine], indentChar);
            int lineToMatch;
            if (CMakeParsing.ShouldIndent(lines, prevLine))
            {
                level += prefs.InsertTabs ? 1 : prefs.IndentSize;
            }
            else if (CMakeParsing.ShouldUnindent(lines, prevLine, out lineToMatch))
            {
                if (lineToMatch < 0)
                {
                    level = 0;
                }
                else
                {
                    level = CMakeParsing.GetIndentationLevel(lines[lineToMatch], indentChar);
                }
            }
            int oldIndentLen = 0;
            while (oldIndentLen < lines[line].Length &&
                lines[line][oldIndentLen] == indentChar)
            {
                oldIndentLen++;
            }
            Source.SetText(line, 0, line, oldIndentLen, new string(indentChar, level));
            TextView.PositionCaretForEditing(line, level);
            return true;
        }
    }
}
