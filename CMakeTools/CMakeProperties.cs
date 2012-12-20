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

namespace CMakeTools
{
    /// <summary>
    /// Utility class to identify CMake properties.
    /// </summary>
    static class CMakeProperties
    {
        // Array of CMake target properties
        private static string[] _targetProperties = new string[]
        {
            "SOURCES"
        };

        // Map from CMake commands to standard properties.
        private static Dictionary<CMakeCommandId, string[]> _commandProperties =
            new Dictionary<CMakeCommandId, string[]>()
        {
            { CMakeCommandId.GetTargetProperty, _targetProperties }
        };

        /// <summary>
        /// Get the CMake properties to be displayed for use with the specified command.
        /// </summary>
        /// <param name="id">A command identifier.</param>
        /// <returns>A collection of property names.</returns>
        public static IEnumerable<string> GetPropertiesForCommand(CMakeCommandId id)
        {
            if (_commandProperties.ContainsKey(id))
            {
                return _commandProperties[id];
            }
            return null;
        }

        /// <summary>
        /// Get the zero-based index of the parameter to the specified command that
        /// should be a CMake property.
        /// </summary>
        /// <param name="id">A command identifier.</param>
        /// <returns>The index of the parameter specifying a CMake property.</returns>
        public static int GetPropertyParameterIndex(CMakeCommandId id)
        {
            switch (id)
            {
            case CMakeCommandId.GetTargetProperty:
                return 2;
            default:
                return -1;
            }
        }
    }
}
