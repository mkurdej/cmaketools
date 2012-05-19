// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Methods object containing parameter information for a CMake command.
    /// </summary>
    class CMakeMethods : Methods
    {
        private static string[] _expression = new string[]
        {
            "expression"
        };

        private static Dictionary<CMakeCommandId, string[]> _parameters =
            new Dictionary<CMakeCommandId, string[]>
        {
            { CMakeCommandId.If,    _expression },
            { CMakeCommandId.While, _expression },
        };

        private CMakeCommandId _id;

        public CMakeMethods(CMakeCommandId id)
        {
            _id = id;
        }

        public override int GetCount()
        {
            return 1;
        }

        public override string GetDescription(int index)
        {
            return "";
        }

        public override string GetName(int index)
        {
            return CMakeKeywords.GetCommandFromId(_id);
        }

        public override int GetParameterCount(int index)
        {
            return _parameters[_id].Length;
        }

        public override void GetParameterInfo(int index, int parameter, out string name,
            out string display, out string description)
        {
            name = _parameters[_id][parameter];
            display = _parameters[_id][parameter];
            description = "";
        }

        public override string GetType(int index)
        {
            return "";
        }

        /// <summary>
        /// Get a methods object containing the subcommands for a given CMake command.
        /// </summary>
        /// <param name="id">The identifier of a CMake command.</param>
        /// <returns>
        /// A methods object or null if there is no parameter information.
        /// </returns>
        public static Methods GetCommandParameters(CMakeCommandId id)
        {
            if (!_parameters.ContainsKey(id))
            {
                return null;
            }
            return new CMakeMethods(id);
        }
    }
}
