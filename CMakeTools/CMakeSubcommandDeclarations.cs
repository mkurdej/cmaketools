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
using System.Linq;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object containing subcommands for a given CMake command.
    /// </summary>
    class CMakeSubcommandDeclarations : Declarations
    {
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
            IEnumerable<string> subcommands = CMakeSubcommandMethods.GetSubcommands(id);
            if (subcommands == null)
            {
                return null;
            }
            return new CMakeSubcommandDeclarations(subcommands.ToArray());
        }
    }
}
