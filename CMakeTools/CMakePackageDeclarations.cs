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
    /// Declarations object for CMake packages.
    /// </summary>
    class CMakePackageDeclarations : CMakeIncludeDeclarations
    {
        public CMakePackageDeclarations(string sourceFilePath)
            : base(sourceFilePath) {}

        protected override IEnumerable<string> GetFilesFromDir(string dirPath,
            bool treatAsModules = false)
        {
            // Find all the find scripts in a directory.
            IEnumerable<string> files = Directory.EnumerateFiles(dirPath, "Find*.cmake");
            files = files.Select(Path.GetFileNameWithoutExtension);
            files = files.Select(x => x.Substring(4));
            return files;
        }

        protected override IEnumerable<string> GetDefaultModules()
        {
            // Find the standard CMake modules that are find scripts and extract the
            // package names.
            IEnumerable<string> files = base.GetDefaultModules();
            files = files.Where(x => x.StartsWith("Find"));
            return files.Select(x => x.Substring(4));
        }

        protected override ItemType GetModuleItemType()
        {
            return ItemType.Package;
        }
    }
}
