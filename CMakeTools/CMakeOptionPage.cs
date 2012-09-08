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

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace CMakeTools
{
    /// <summary>
    /// Option page to configure CMake Tools for Visual Studio.
    /// </summary>
    public class CMakeOptionPage : DialogPage
    {
        /// <summary>
        /// Flags indicating whether to display command names in lowercase.
        /// </summary>
        [DisplayName("Commands In Lowercase")]
        [Description("Display CMake commands with lowercase letters in the IntelliSense list box.")]
        public bool CommandsLower { get; set; }
    }
}
