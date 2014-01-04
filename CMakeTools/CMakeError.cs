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

using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Errors that can be detected in CMake code.
    /// </summary>
    public enum CMakeError
    {
        InvalidVariableRef,
        UnmatchedParen,
        ExpectedCommand,
        ExpectedEOL,
        ExpectedOpenParen,
        DeprecatedCommand,
        InvalidEscapeSequence
    }

    /// <summary>
    /// Information about an error detected in CMake code.
    /// </summary>
    public struct CMakeErrorInfo
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public CMakeError ErrorCode { get; set; }

        /// <summary>
        /// A text span specifying where the error was detected.
        /// </summary>
        public TextSpan Span { get; set; }

        /// <summary>
        /// Boolean value indicating is the error is a warning.
        /// </summary>
        public bool Warning { get; set; }
    }
}
