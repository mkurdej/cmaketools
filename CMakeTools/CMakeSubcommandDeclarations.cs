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
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object containing subcommands for a given CMake command.
    /// </summary>
    class CMakeSubcommandDeclarations : Declarations
    {
        // Array of subcommands for the CMAKE_POLICY command.
        private static string[] _cmakePolicySubcommands = new string[]
        {
            "GET",
            "POP",
            "PUSH",
            "SET",
            "VERSION"
        };

        // Array of subcommands for the DEFINE_PROPERTY command.
        private static string[] _definePropertySubcommands = new string[]
        {
            "CACHED_VARIABLE",
            "DIRECTORY",
            "GLOBAL",
            "SOURCE",
            "TARGET",
            "TEST",
            "VARIABLE",
        };

        // Array of subcommands for the EXPORT command.
        private static string[] _exportSubcommands = new string[]
        {
            "PACKAGE",
            "TARGETS"
        };

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

        // Array of subcommands for the INSTALL command.
        private static string[] _installSubcommands = new string[]
        {
            "CODE",
            "DIRECTORY",
            "EXPORT",
            "FILES",
            "PROGRAMS",
            "SCRIPT",
            "TARGETS"
        };

        // Array of subcommands for the LIST command.
        private static string[] _listSubcommands = new string[]
        {
            "APPEND",
            "FIND",
            "GET",
            "INSERT",
            "LENGTH",
            "REMOVE_AT",
            "REMOVE_DUPLICATES",
            "REMOVE_ITEM",
            "REVERSE",
            "SORT"
        };

        // Array of subcommands for the SET_PROPERTY command.
        private static string[] _setPropertySubcommands = new string[]
        {
            "CACHE",
            "DIRECTORY",
            "GLOBAL",
            "SOURCE",
            "TARGET",
            "TEST"
        };

        // Array of subcommands for the STRING command.
        private static string[] _stringSubcommands = new string[]
        {
            "ASCII",
            "COMPARE",
            "CONFIGURE",
            "FIND",
            "LENGTH",
            "MD5",
            "RANDOM",
            "REGEX",
            "REPLACE",
            "SHA1",
            "SHA224",
            "SHA256",
            "SHA384",
            "SHA512",
            "STRIP",
            "SUBSTRING",
            "TOLOWER",
            "TOUPPER"
        };

        // Map from command identifiers to arrays of subcommands.
        private static Dictionary<CMakeCommandId, string[]> _subcommandArrays =
            new Dictionary<CMakeCommandId, string[]>
        {
            { CMakeCommandId.CMakePolicy,       _cmakePolicySubcommands },
            { CMakeCommandId.DefineProperty,    _definePropertySubcommands },
            { CMakeCommandId.Export,            _exportSubcommands },
            { CMakeCommandId.File,              _fileSubcommands },
            { CMakeCommandId.Install,           _installSubcommands },
            { CMakeCommandId.List,              _listSubcommands },
            { CMakeCommandId.SetProperty,       _setPropertySubcommands },
            { CMakeCommandId.String,            _stringSubcommands },
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
            CMakeCommandId id)
        {
            if (!_subcommandArrays.ContainsKey(id))
            {
                return null;
            }
            return new CMakeSubcommandDeclarations(_subcommandArrays[id]);
        }

        /// <summary>
        /// Get a collection of commands that should trigger member selection because
        /// they have subcommands.
        /// </summary>
        /// <returns>A collection of command identifiers.</returns>
        public static IEnumerable<CMakeCommandId> GetMemberSelectionTriggers()
        {
            return _subcommandArrays.Keys;
        }
    }
}
