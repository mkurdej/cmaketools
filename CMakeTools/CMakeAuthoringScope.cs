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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Authoring scope object representing the result of a parse operation.
    /// </summary>
    class CMakeAuthoringScope : AuthoringScope
    {
        private Declarations _declarations;
        private Methods _methods;
        private IEnumerable<string> _lines;
        private string _fileName;

        internal void SetDeclarations(Declarations declarations)
        {
            _declarations = declarations;
        }

        internal void SetMethods(Methods methods)
        {
            _methods = methods;
        }

        internal void SetLines(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        internal void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            CMakeParsing.TokenData tokenData;
            if (_lines != null &&
                CMakeParsing.ParseForToken(_lines, line, col, out tokenData) &&
                !tokenData.InParens)
            {
                if (tokenData.TokenInfo.Token == (int)CMakeToken.Keyword)
                {
                    // Get a Quick Info tip for the command at the cursor.
                    span = tokenData.TokenInfo.ToTextSpan(line);
                    string lineText = _lines.ToList()[line];
                    string tokenText = lineText.ExtractToken(tokenData.TokenInfo);
                    CMakeCommandId id = CMakeKeywords.GetCommandId(tokenText);
                    if (CMakeSubcommandMethods.HasSubcommands(id))
                    {
                        // Parse to get the subcommand, if there is one.
                        CMakeParsing.ParameterInfoResult result =
                            CMakeParsing.ParseForParameterInfo(_lines, line, -1, true);
                        return CMakeSubcommandMethods.GetSubcommandQuickInfoTip(id,
                            result.SubcommandName);                        
                    }
                    return CMakeMethods.GetCommandQuickInfoTip(id);
                }
                else if (tokenData.TokenInfo.Token == (int)CMakeToken.Identifier)
                {
                    // Get a Quick Info tip for the function called at the cursor, if
                    // there is one.
                    string lineText = _lines.ToList()[line];
                    string tokenText = lineText.ExtractToken(tokenData.TokenInfo);
                    List<string> parameters = CMakeParsing.ParseForParameterNames(_lines,
                        tokenText);
                    if (parameters != null)
                    {
                        span = tokenData.TokenInfo.ToTextSpan(line);
                        return string.Format("{0}({1})", tokenText,
                            string.Join(" ", parameters));
                    }
                }
            }
            span = new TextSpan();
            return null;
        }

        public override Declarations GetDeclarations(IVsTextView view, int line, int col,
            TokenInfo info, ParseReason reason)
        {
            return _declarations;
        }

        public override Methods GetMethods(int line, int col, string name)
        {
            return _methods;
        }

        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView,
            int line, int col, out TextSpan span)
        {
            if (_lines != null && (cmd == VSConstants.VSStd97CmdID.GotoDefn ||
                cmd == VSConstants.VSStd97CmdID.GotoDecl))
            {
                // Parse for any identifier that may be at the cursor.  If the is one,
                // find the variable or function definition for that identifier and jump
                // to it.
                bool isVariable = false;
                string identifier = CMakeParsing.ParseForIdentifier(_lines, line, col,
                    out isVariable);
                if (identifier != null)
                {
                    if (isVariable)
                    {
                        if (CMakeParsing.ParseForVariableDefinition(_lines, identifier,
                            out span))
                        {
                            span.iEndIndex++;
                            return _fileName;
                        }
                    }
                    else
                    {
                        if (CMakeParsing.ParseForFunctionDefinition(_lines, identifier,
                            out span))
                        {
                            span.iEndIndex++;
                            return _fileName;
                        }
                    }
                }
            }
            span = new TextSpan();
            return null;
        }
    }
}
