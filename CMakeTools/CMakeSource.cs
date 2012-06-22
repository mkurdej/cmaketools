// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

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

        /// <summary>
        /// Get a collection containing all the lines in the source file.
        /// </summary>
        /// <returns>An IEnumerable providing access to all the lines.</returns>
        public IEnumerable<string> GetLines()
        {
            int lineCount = GetLineCount();
            for (int i = 0; i < lineCount; i++)
            {
                yield return GetLine(i);
            }
        }
    }
}
