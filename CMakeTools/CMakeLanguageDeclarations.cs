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
    /// Declarations object for languages supported by the ENABLE_LANGUAGE
    /// CMake object.
    /// </summary>
    class CMakeLanguageDeclarations : CMakeIncludeDeclarations
    {
        public CMakeLanguageDeclarations(string sourceFilePath)
            : base(sourceFilePath) {}

        protected override IEnumerable<string> GetFilesFromDir(string dirPath,
            bool treatAsModules = false)
        {
            if (!treatAsModules)
            {
                return new string[] {};
            }

            return GetLanguagesFromDir(dirPath);
        }

        protected override IEnumerable<string> GetDefaultModules()
        {
            // Find the standard CMake modules that take the form CMakeXInformation and
            // extract the language name X.
            IEnumerable<string> files = base.GetDefaultModules();
            files = files.Where(x => x.StartsWith("CMake"));
            files = files.Where(x => x.EndsWith("Information"));
            files = files.Select(x => x.Substring(5, x.Length - 16));
            return files;
        }

        protected override ItemType GetModuleItemType()
        {
            return ItemType.Language;
        }

        public static IEnumerable<string> GetLanguagesFromDir(string dirPath)
        {
            // Each language X supported by CMake has a file called
            // CMakeXInformation.cmake.
            IEnumerable<string> files = Directory.EnumerateFiles(dirPath,
                "CMake*.cmake");
            files = files.Select(Path.GetFileNameWithoutExtension);
            files = files.Where(x => x.EndsWith("Information"));
            files = files.Select(x => x.Substring(5, x.Length - 16));
            return files;
        }
    }
}
