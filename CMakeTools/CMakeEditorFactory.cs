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

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Editor factory for CMake.
    /// </summary>
    [Guid(CMakeGuids.guidCMakeEditorFactory)]
    public class CMakeEditorFactory : EditorFactory
    {
        public CMakeEditorFactory(CMakePackage package)
            : base(package) {}

        public override Guid GetLanguageServiceGuid()
        {
            return typeof(CMakeLanguageService).GUID;
        }
    }
}
