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

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Source object for CMake code.
    /// </summary>
    class CMakeSource : Source
    {
        public CMakeSource(LanguageService service, IVsTextLines textLines,
            Colorizer colorizer) : base(service, textLines, colorizer)
        {
            // Just call the base class.
        }

        public override CommentInfo GetCommentFormat()
        {
            // Provide information on how comments are specified in CMake code.
            CommentInfo info = new CommentInfo();
            info.LineStart = "#";
            info.UseLineComments = true;
            return info;
        }

        public override void Completion(IVsTextView textView, TokenInfo info,
            ParseReason reason)
        {
            bool oldValue = LanguageService.Preferences.EnableAsyncCompletion;
            if (reason == ParseReason.MemberSelect ||
                reason == ParseReason.MemberSelectAndHighlightBraces)
            {
                if (info.Token == (int)CMakeToken.VariableStart ||
                    info.Token == (int)CMakeToken.VariableStartEnv)
                {
                    // Disable asynchronous parsing for member selection requests
                    // involving variables.  It doesn't work properly when a parameter
                    // information tool tip is visible.
                    LanguageService.Preferences.EnableAsyncCompletion = false;
                }
            }
            base.Completion(textView, info, reason);
            LanguageService.Preferences.EnableAsyncCompletion = oldValue;
        }
    }
}
