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
using System.IO;
using System.Linq;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object for C/C++ source files.
    /// </summary>
    class CMakeSourceDeclarations : CMakeIncludeDeclarations
    {
        private static string[] _fileFilters = new string[]
        {
            "*.c",
            "*.cc",
            "*.cpp",
            "*.cxx"
        };

        public CMakeSourceDeclarations(string sourceFilePath)
            : base(sourceFilePath) {}

        protected override IEnumerable<string> GetFilesFromDir(string dirPath,
            bool treatAsModules = false)
        {
            if (treatAsModules)
            {
                return new string[] {};
            }

            // Find all C/C++ source files in the specified directory.
            List<string> allFiles = new List<string>();
            foreach (string filter in _fileFilters)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(dirPath, filter);
                files = files.Select(Path.GetFileName);
                allFiles.AddRange(files);
            }
            allFiles.Sort();
            return allFiles;
        }

        public override int GetGlyph(int index)
        {
            // Always return the icon for a snippet.  It's the closest thing to a file
            // that's available in the standard icon set.
            return 205;
        }
    }
}
