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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object for subdirectories.
    /// </summary>
    class CMakeSubdirectoryDeclarations : CMakeIncludeDeclarations
    {
        // Directories created by CMake that should not be included in the build.
        private static string[] _internalDirectories = new string[]
        {
            "CMakeFiles"
        };

        private static char[] _quoteChars = new char[] { '#', '(', ')' };

        private bool _requireCMakeLists;

        public CMakeSubdirectoryDeclarations(string sourceFilePath,
            bool requireCMakeLists) : base(sourceFilePath)
        {
            _requireCMakeLists = requireCMakeLists;
        }

        protected override IEnumerable<string> GetFilesFromDir(string dirPath,
            bool treatAsModules = false)
        {
            if (treatAsModules)
            {
                return new string[] {};
            }

            // Find all subdirectories of the given directory.
            IEnumerable<string> subdirs = Directory.EnumerateDirectories(dirPath);
            subdirs = subdirs.Select(Path.GetFileName);
            subdirs = subdirs.Where(x => !_internalDirectories.Contains(x));

            // If the setting is configured to require CMakeLists.txt for a subdirectory
            // to be shown in the list, filter out all subdirectories that don't have it.
            if (_requireCMakeLists)
            {
                subdirs = subdirs.Where(x => File.Exists(Path.Combine(dirPath, x,
                    "CMakeLists.txt")));
            }

            return subdirs;
        }

        protected override IEnumerable<string> GetDefaultModules()
        {
            return new string[] {};
        }

        public override int GetGlyph(int index)
        {
            // Always return the icon for an open folder.
            return 201;
        }

        public override string GetName(int index)
        {
            // Names that contain whitespace or certain special characters need
            // to be quoted.
            string name = base.GetName(index);
            if (name != null &&
                (name.Any(char.IsWhiteSpace) || name.Any(x => _quoteChars.Contains(x))))
            {
                name = "\"" + name + "\"";
            }
            return name;
        }

        public override bool IsMatch(string textSoFar, int index)
        {
            if (base.IsMatch(textSoFar, index))
            {
                return true;
            }

            // Match a name that needs to be quoted, even if the user leaves out the
            // quotation marks.
            string name = GetName(index);
            if (name != null && name.Length > 1 && name[0] == '"')
            {
                name = name.Substring(1);
                if (name.StartsWith(textSoFar, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
