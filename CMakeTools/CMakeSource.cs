// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

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
    }
}
