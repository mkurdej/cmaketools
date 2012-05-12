// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object containing subcommands for a given CMake command.
    /// </summary>
    class CMakeSubcommandDeclarations : Declarations
    {
        // Array of subcommands for the FILE command.
        private static string[] _fileSubcommands = new string[]
        {
            "APPEND",
            "DOWNLOAD",
            "GLOB",
            "GLOB_RECURSE",
            "MAKE_DIRECTORY",
            "MD5",
            "READ",
            "RELATIVE_PATH",
            "REMOVE",
            "REMOVE_RECURSE",
            "RENAME",
            "SHA1",
            "SHA224",
            "SHA256",
            "SHA384",
            "SHA512",
            "STRINGS",
            "TO_CMAKE_PATH",
            "TO_NATIVE_PATH",
            "UPLOAD",
            "WRITE"
        };

        // Map from command identifiers to arrays of subcommands.
        private static Dictionary<CMakeKeywordId, string[]> _subcommandArrays =
            new Dictionary<CMakeKeywordId, string[]>
        {
            { CMakeKeywordId.File,  _fileSubcommands }
        };

        // Array of subcommands to be displayed.
        private string[] _subcommands;

        private CMakeSubcommandDeclarations(string[] subcommands)
        {
            _subcommands = subcommands;
        }

        public override int GetCount()
        {
            return _subcommands.Length;
        }

        public override string GetDescription(int index)
        {
            return "";
        }

        public override string GetDisplayText(int index)
        {
            return GetName(index);
        }

        public override int GetGlyph(int index)
        {
            // Always return the index for a keyword.
            return 207;
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _subcommands.Length)
            {
                return null;
            }
            return _subcommands[index];
        }

        /// <summary>
        /// Get a declarations object containing the subcommands for a given CMake
        /// command.
        /// </summary>
        /// <param name="id">The identifier of a CMake command.</param>
        /// <returns>A declarations object or null if there are no subcommands.</returns>
        public static CMakeSubcommandDeclarations GetSubcommandDeclarations(
            CMakeKeywordId id)
        {
            if (!_subcommandArrays.ContainsKey(id))
            {
                return null;
            }
            return new CMakeSubcommandDeclarations(_subcommandArrays[id]);
        }
    }
}
