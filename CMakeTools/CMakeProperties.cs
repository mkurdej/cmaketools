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
        // Array of CMake target properties.
        private static string[] _targetProperties = new string[]
        {
            "ARCHIVE_OUTPUT_DIRECTORY",
            "ARCHIVE_OUTPUT_NAME",
            "AUTOMOC",
            "AUTOMOC_MOC_OPTIONS",
            "BUILD_WITH_INSTALL_RPATH",
            "BUNDLE",
            "BUNDLE_EXTENSION",
            "COMPILE_DEFINITIONS",
            "COMPILE_FLAGS",
            "DEBUG_POSTFIX",
            "DEFINE_SYMBOL",
            "ENABLE_EXPORTS",
            "EXCLUDE_FROM_ALL",
            "EchoString",
            "FOLDER",
            "FRAMEWORK",
            "Fortran_FORMAT",
            "Fortran_MODULE_DIRECTORY",
            "GENERATOR_FILE_NAME",
            "GNUtoMS",
            "HAS_CXX",
            "IMPLICIT_DEPENDS_INCLUDE_TRANSFORM",
            "IMPORTED",
            "IMPORTED_CONFIGURATIONS",
            "IMPORTED_IMPLIB",
            "IMPORTED_LINK_DEPENDENT_LIBRARIES",
            "IMPORTED_LINK_INTERFACE_LANGUAGES",
            "IMPORTED_LINK_INTERFACE_LIBRARIES",
            "IMPORTED_LINK_INTERFACE_MULTIPLICITY",
            "IMPORTED_LOCATION",
            "IMPORTED_NO_SONAME",
            "IMPORTED_SONAME",
            "IMPORT_PREFIX",
            "IMPORT_SUFFIX",
            "INCLUDE_DIRECTORIES",
            "INSTALL_NAME_DIR",
            "INSTALL_RPATH",
            "INSTALL_RPATH_USE_LINK_PATH",
            "INTERPROCEDURAL_OPTIMIZATION",
            "LABELS",
            "LIBRARY_OUTPUT_DIRECTORY",
            "LIBRARY_OUTPUT_NAME",
            "LINKER_LANGUAGE",
            "LINK_DEPENDS",
            "LINK_FLAGS",
            "LINK_INTERFACE_LIBRARIES",
            "LINK_INTERFACE_MULTIPLICITY",
            "LINK_SEARCH_END_STATIC",
            "LINK_SEARCH_START_STATIC",
            "LOCATION",
            "MACOSX_BUNDLE",
            "MACOSX_BUNDLE_INFO_PLIST",
            "MACOSX_FRAMEWORK_INFO_PLIST",
            "NO_SONAME",
            "OSX_ARCHITECTURES",
            "OUTPUT_NAME",
            "PDB_NAME",
            "PDB_OUTPUT_DIRECTORY",
            "POSITION_INDEPENDENT_CODE",
            "POST_INSTALL_SCRIPT",
            "PREFIX",
            "PRE_INSTALL_SCRIPT",
            "PRIVATE_HEADER",
            "PROJECT_LABEL",
            "PUBLIC_HEADER",
            "RESOURCE",
            "RULE_LAUNCH_COMPILE",
            "RULE_LAUNCH_CUSTOM",
            "RULE_LAUNCH_LINK",
            "RUNTIME_OUTPUT_DIRECTORY",
            "RUNTIME_OUTPUT_NAME",
            "SKIP_BUILD_RPATH",
            "SOURCES",
            "SOVERSION",
            "STATIC_LIBRARY_FLAGS",
            "SUFFIX",
            "TYPE",
            "VERSION",
            "VS_DOTNET_REFERENCES",
            "VS_GLOBAL_KEYWORD",
            "VS_GLOBAL_PROJECT_TYPES",
            "VS_KEYWORD",
            "VS_SCC_AUXPATH",
            "VS_SCC_LOCALPATH",
            "VS_SCC_PROJECTNAME",
            "VS_SCC_PROVIDER",
            "VS_WINRT_EXTENSIONS",
            "VS_WINRT_REFERENCES",
            "WIN32_EXECUTABLE"
        };

        // Array of CMake source file properties.
        private static string[] _sourceFileProperties = new string[]
        {
            "ABSTRACT",
            "COMPILE_DEFINITIONS",
            "COMPILE_FLAGS",
            "EXTERNAL_OBJECT",
            "Fortran_FORMAT",
            "GENERATED",
            "HEADER_FILE_ONLY",
            "KEEP_EXTENSION",
            "LABELS",
            "LANGUAGE",
            "LOCATION",
            "MACOSX_PACKAGE_LOCATION",
            "OBJECT_DEPENDS",
            "OBJECT_OUTPUTS",
            "SYMBOLIC",
            "WRAP_EXCLUDE"
        };

        // Map from CMake commands to standard properties.
        private static Dictionary<CMakeCommandId, string[]> _commandProperties =
            new Dictionary<CMakeCommandId, string[]>()
        {
            { CMakeCommandId.GetTargetProperty,     _targetProperties },
            { CMakeCommandId.GetSourceFileProperty, _sourceFileProperties }
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
            case CMakeCommandId.GetSourceFileProperty:
                return 2;
            default:
                return -1;
            }
        }
    }
}
