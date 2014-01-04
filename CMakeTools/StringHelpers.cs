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
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Extension methods that serve as helpers for manipulating strings.
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Extract a token from a string.
        /// </summary>
        /// <param name="text">The string from which to extract the token.</param>
        /// <param name="tokenInfo">The token to extract.</param>
        /// <returns>The text of the token.</returns>
        public static string ExtractToken(this string text, TokenInfo tokenInfo)
        {
            return text.Substring(tokenInfo.StartIndex,
                tokenInfo.EndIndex - tokenInfo.StartIndex + 1);
        }

        /// <summary>
        /// Generate a text span from token information.
        /// </summary>
        /// <param name="tokenInfo">Information on a token.</param>
        /// <param name="line">The line number on which the token is found.</param>
        /// <returns>A text span for the token.</returns>
        public static TextSpan ToTextSpan(this TokenInfo tokenInfo, int line)
        {
            // The indices used by TokenInfo specify characters, while the indices used
            // by TextSpan specify cursor positions between characters.  Therefore, it
            // is necessary to add one to the end index.
            return new TextSpan()
            {
                iStartLine = line,
                iStartIndex = tokenInfo.StartIndex,
                iEndLine = line,
                iEndIndex = tokenInfo.EndIndex + 1
            };
        }

        /// <summary>
        /// Get a collection containing all the lines in the source file.
        /// </summary>
        /// <param name="source">The source file.</param>
        /// <returns>An IEnumerable providing access to all the lines.</returns>
        public static IEnumerable<string> GetLines(this Source source)
        {
            int lineCount = source.GetLineCount();
            for (int i = 0; i < lineCount; i++)
            {
                yield return source.GetLine(i);
            }
        }
    }
}
