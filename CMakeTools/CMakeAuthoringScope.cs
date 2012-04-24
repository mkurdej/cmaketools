// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    class CMakeAuthoringScope : AuthoringScope
    {
        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        public override Declarations GetDeclarations(IVsTextView view, int line, int col,
            TokenInfo info, ParseReason reason)
        {
            return null;
        }

        public override Methods GetMethods(int line, int col, string name)
        {
            return null;
        }

        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView,
            int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }
    }
}
