// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Scanner for CMake code.
    /// </summary>
    class CMakeScanner : IScanner
    {
        public void SetSource(string source, int offset)
        {
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            return false;
        }
    }
}
