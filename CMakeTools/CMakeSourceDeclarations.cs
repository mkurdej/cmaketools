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

        private List<string> _filesToExclude;

        public CMakeSourceDeclarations(string sourceFilePath,
            IEnumerable<string> priorParameters) : base(sourceFilePath)
        {
            // Exclude all files that already appear in the parameter list, except for
            // the first token, which is the name of the executable to be generated.
            // Sort the files so that we can quick check if a file is in the list using
            // a binary search.
            _filesToExclude = new List<string>(priorParameters);
            if (_filesToExclude.Count > 0)
            {
                _filesToExclude.RemoveAt(0);
            }
            _filesToExclude.Sort();
        }

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
                files = files.Where(x => _filesToExclude.BinarySearch(x) < 0);
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
