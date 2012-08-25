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

using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object for CMake functions, macros, and commands.
    /// </summary>
    class CMakeFunctionDeclarations : Declarations
    {
        private string[] _functions;

        public CMakeFunctionDeclarations()
        {
            _functions = CMakeKeywords.GetAllCommands();
        }

        public override int GetCount()
        {
            return _functions.Length;
        }

        public override string GetDescription(int index)
        {
            return null;
        }

        public override string GetDisplayText(int index)
        {
            return GetName(index);
        }

        public override int GetGlyph(int index)
        {
            // Always return the icon index for a keyword.
            return 206;
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _functions.Length)
            {
                return null;
            }
            return _functions[index];
        }
    }
}
