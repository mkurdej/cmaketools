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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    class CMakeViewFilter : ViewFilter
    {
        public CMakeViewFilter(CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view) {}

        protected override int QueryCommandStatus(ref Guid guidCmdGroup, uint nCmdId)
        {
            // Show and enable the Insert Snippet command.
            if (guidCmdGroup == VSConstants.VSStd2K &&
                nCmdId == (uint)VSConstants.VSStd2KCmdID.INSERTSNIPPET)
            {
                return (int)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
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
                    TextSpan span = tokenInfo.ToBraceMatchingSpan(line);
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
            }
            return base.HandlePreExec(ref guidCmdGroup, nCmdId, nCmdexecopt, pvaIn,
                pvaOut);
        }
    }
}
