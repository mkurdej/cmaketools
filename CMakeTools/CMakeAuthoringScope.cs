// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System.Collections.Generic;
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

        public void SetDeclarations(Declarations declarations)
        {
            _declarations = declarations;
        }

        public void SetMethods(Methods methods)
        {
            _methods = methods;
        }

        public void SetLines(IEnumerable<string> lines)
        {
            _lines = lines;
        }

        public void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
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
            if (cmd == VSConstants.VSStd97CmdID.GotoDefn ||
                cmd == VSConstants.VSStd97CmdID.GotoDecl)
            {
                // Parse for any identifier that may be at the cursor.  If the is one,
                // find the variable or function definition for that identifier and jump
                // to it.
                bool isVariable = false;
                string identifier = CMakeLanguageService.ParseForIdentifier(_lines, line,
                    col, out isVariable);
                if (identifier != null)
                {
                    if (isVariable)
                    {
                        if (CMakeLanguageService.ParseForVariableDefinition(_lines,
                            identifier, out span))
                        {
                            span.iEndIndex++;
                            return _fileName;
                        }
                    }
                    else
                    {
                        if (CMakeLanguageService.ParseForFunctionDefinition(_lines,
                            identifier, out span))
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
