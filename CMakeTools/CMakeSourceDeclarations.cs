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
        private static readonly string[] _fileExtensions = new string[]
        {
            ".c",
            ".cc",
            ".cpp",
            ".cxx"
        };

        private static readonly string[] _addExecutableKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "MACOSX_BUNDLE",
            "WIN32"
        };

        private static readonly string[] _addLibraryKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "MODULE",
            "SHARED",
            "STATIC"
        };

        private static readonly Dictionary<CMakeCommandId, string[]> _commandKeywords =
            new Dictionary<CMakeCommandId, string[]>()
        {
            { CMakeCommandId.AddExecutable, _addExecutableKeywords },
            { CMakeCommandId.AddLibrary,    _addLibraryKeywords }
        };

        private List<string> _filesToExclude;
        private CMakeCommandId _id;

        public CMakeSourceDeclarations(string sourceFilePath,
            IEnumerable<string> priorParameters, CMakeCommandId id)
            : base(sourceFilePath)
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
            _id = id;
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
            foreach (string extension in _fileExtensions)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(dirPath,
                    "*" + extension);
                files = files.Select(Path.GetFileName);
                files = files.Where(x => _filesToExclude.BinarySearch(x) < 0);
                allFiles.AddRange(files);
            }

            // Add in any keywords that are allowed with the specified command.
            if (_commandKeywords.ContainsKey(_id))
            {
                allFiles.AddRange(_commandKeywords[_id]);
            }
            allFiles.Sort();
            return allFiles;
        }

        protected override IEnumerable<string> GetDefaultModules()
        {
            // Don't show list of CMake modules when there is no CMake installation.
            return new string[] {};
        }

        public override int GetGlyph(int index)
        {
            // If the index specifies a keyword, return the index for a keyword.
            string name = GetName(index);
            if (name != null && !_fileExtensions.Any(x => name.ToLower().EndsWith(x)))
            {
                return 206;
            }

            // Return the icon for a snippet.  It's the closest thing to a file
            // that's available in the standard icon set.
            return 205;
        }
    }
}
