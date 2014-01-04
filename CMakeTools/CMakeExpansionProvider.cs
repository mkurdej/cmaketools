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
    /// Expansion provider for CMake code snippets.
    /// </summary>
    class CMakeExpansionProvider : ExpansionProvider
    {
        public CMakeExpansionProvider(Source source)
            : base(source) {}

        public override int FormatSpan(IVsTextLines buffer, TextSpan[] ts)
        {
            foreach (TextSpan span in ts)
            {
                // Set the indentation of each line in the snippet to match the first
                // line.
                LanguagePreferences prefs = Source.LanguageService.Preferences;
                char indentChar = prefs.InsertTabs ? '\t' : ' ';
                int level = CMakeParsing.GetIndentationLevel(
                    Source.GetLine(span.iStartLine), indentChar);
                for (int i = span.iStartLine + 1; i <= span.iEndLine; ++i)
                {
                    Source.SetText(i, 0, i, 0, new string(indentChar, level));
                }
            }
            return VSConstants.S_OK;
        }
    }
}
