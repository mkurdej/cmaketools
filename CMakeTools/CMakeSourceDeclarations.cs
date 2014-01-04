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
using System.IO;
using System.Linq;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object for C/C++ source files.
    /// </summary>
    class CMakeSourceDeclarations : CMakeIncludeDeclarations
    {
        private static readonly string[] _fileExtensions = new string[]
        {
            ".c",
            ".cc",
            ".cpp",
            ".cxx"
        };

        public CMakeSourceDeclarations(string sourceFilePath)
            : base(sourceFilePath)
        {}

        protected override IEnumerable<string> GetFilesFromDir(string dirPath,
            bool treatAsModules = false)
        {
            if (treatAsModules)
            {
                return new string[] {};
            }

            // Find all C/C++ source files in the specified directory.
            List<string> allFiles = new List<string>();
            foreach (string extension in _fileExtensions)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(dirPath,
                    "*" + extension);
                files = files.Select(Path.GetFileName);
                allFiles.AddRange(files);
            }

            return allFiles;
        }

        protected override IEnumerable<string> GetDefaultModules()
        {
            // Don't show list of CMake modules when there is no CMake installation.
            return new string[] {};
        }

        protected override ItemType GetIncludeFileItemType()
        {
            return ItemType.SourceFile;
        }
    }
}
