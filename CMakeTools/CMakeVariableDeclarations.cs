// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object containing CMake variables
    /// </summary>
    class CMakeVariableDeclarations : Declarations
    {
        // Array of standard CMake variables.
        private static string[] _standardVariables = new string[]
        {
            "CMAKE_BINARY_DIR"
        };

        public override int GetCount()
        {
            return _standardVariables.Length;
        }

        public override string GetDescription(int index)
        {
            return "";
        }

        public override string GetDisplayText(int index)
        {
            if (index < 0 || index >= _standardVariables.Length)
            {
                return null;
            }
            return _standardVariables[index];
        }

        public override int GetGlyph(int index)
        {
            // Always return the icon index for a public variable.
            return 138;
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _standardVariables.Length)
            {
                return null;
            }
            return "${" + _standardVariables[index];
        }
    }
}
