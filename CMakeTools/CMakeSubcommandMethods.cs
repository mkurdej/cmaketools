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
    class CMakeSubcommandMethods : Methods
    {
        // Parameters to the FILE(APPEND) command.
        private static string[] _fileAppendParams = new string[]
        {
            "filename",
            "message"
        };

        // Map from subcommands of FILE command to parameters.
        private static Dictionary<string, string[]> _fileSubcommands =
            new Dictionary<string, string[]>
        {
            { "APPEND",     _fileAppendParams }
        };

        // Map from commands to subcommands.
        private static Dictionary<CMakeCommandId, Dictionary<string, string[]>> _allSubcommands =
            new Dictionary<CMakeCommandId, Dictionary<string, string[]>>
        {
            { CMakeCommandId.File,  _fileSubcommands }
        };

        // The command and subcommand for which the given instance will provie
        // parameters.
        private CMakeCommandId _id;
        private string _subcommand;

        private CMakeSubcommandMethods(CMakeCommandId id, string subcommand)
        {
            _id = id;
            _subcommand = subcommand;
        }

        public override int GetCount()
        {
            // Only provide a single signature.
            return 1;
        }

        public override string GetDescription(int index)
        {
            return null;
        }

        public override string GetName(int index)
        {
            // Look up the name of the command and combine it with the subcommand.
            return CMakeKeywords.GetCommandFromId(_id) + "(" + _subcommand;
        }

        public override int GetParameterCount(int index)
        {
            // Look up the number of parameters that the command takes.
            if (!_allSubcommands.ContainsKey(_id) ||
                !_allSubcommands[_id].ContainsKey(_subcommand) ||
                _allSubcommands[_id][_subcommand] == null)
            {
                return 0;
            }
            return _allSubcommands[_id][_subcommand].Length;
        }

        public override void GetParameterInfo(int index, int parameter, out string name,
            out string display, out string description)
        {
            // Look up the name of the requested parameter.
            if (!_allSubcommands.ContainsKey(_id) ||
                !_allSubcommands[_id].ContainsKey(_subcommand) ||
                _allSubcommands[_id][_subcommand] == null)
            {
                name = null;
                display = null;
                description = null;
                return;
            }
            name = _allSubcommands[_id][_subcommand][parameter];
            display = _allSubcommands[_id][_subcommand][parameter];
            description = null;
        }

        public override string GetType(int index)
        {
            // CMake commands don't have return types.
            return null;
        }

        public override string Delimiter
        {
            get
            {
                // In CMake, spaces instead of commas are used to separate arguments.
                return " ";
            }
        }

        public override string OpenBracket
        {
            get
            {
                // An opening parenthesis separates the command from the subcommand, and
                // whitespace separates the subcommand from its first parameter.
                return " ";
            }
        }

        /// <summary>
        /// Check whether the specified command takes subcommands.
        /// </summary>
        /// <param name="id">Identifier of the command to check.</param>
        /// <returns>True if the command takes subcommands or false otherwise.</returns>
        public static bool HasSubcommands(CMakeCommandId id)
        {
            return _allSubcommands.ContainsKey(id);
        }

        /// <summary>
        /// Get a methods object containing the parameters for a given CMake subcommand.
        /// </summary>
        /// <param name="id">The identifier of a CMake command.</param>
        /// <param name="subcommand">The name of a subcommand.</param>
        /// <returns>
        /// A methods object or null if there is no parameter information.
        /// </returns>
        public static Methods GetSubcommandParameters(CMakeCommandId id,
            string subcommand)
        {
            if (!_allSubcommands.ContainsKey(id) ||
                !_allSubcommands[id].ContainsKey(subcommand) ||
                _allSubcommands[id][subcommand] == null)
            {
                return null;
            }
            return new CMakeSubcommandMethods(id, subcommand);
        }
    }
}
