// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

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

        public void SetDeclarations(Declarations declarations)
        {
            _declarations = declarations;
        }

        public void SetMethods(Methods methods)
        {
            _methods = methods;
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
            span = new TextSpan();
            return null;
        }
    }
}
