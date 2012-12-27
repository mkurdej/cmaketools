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
        private static readonly string[] _targetProperties = new string[]
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
        private static readonly string[] _sourceFileProperties = new string[]
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

        // Array of CMake test properties.
        private static readonly string[] _testProperties = new string[]
        {
            "ATTACHED_FILES",
            "ATTACHED_FILES_ON_FAIL",
            "COST",
            "DEPENDS",
            "ENVIRONMENT",
            "FAIL_REGULAR_EXPRESSION",
            "LABELS",
            "MEASUREMENT",
            "PASS_REGULAR_EXPRESSION",
            "PROCESSORS",
            "REQUIRED_FILES",
            "RESOURCE_LOCK",
            "RUN_SERIAL",
            "TIMEOUT",
            "WILL_FAIL",
            "WORKING_DIRECTORY"
        };

        // Array of CMake directory properties.
        private static readonly string[] _directoryProperties = new string[]
        {
            "ADDITIONAL_MAKE_CLEAN_FILES",
            "CACHE_VARIABLES",
            "CLEAN_NO_CUSTOM",
            "COMPILE_DEFINITIONS",
            "DEFINITIONS",
            "EXCLUDE_FROM_ALL",
            "IMPLICIT_DEPENDS_INCLUDE_TRANSFORM",
            "INCLUDE_DIRECTORIES",
            "INCLUDE_REGULAR_EXPRESSION",
            "INTERPROCEDURAL_OPTIMIZATION",
            "LINK_DIRECTORIES",
            "LISTFILE_STACK",
            "MACROS",
            "PARENT_DIRECTORY",
            "RULE_LAUNCH_COMPILE",
            "RULE_LAUNCH_CUSTOM",
            "RULE_LAUNCH_LINK",
            "TEST_INCLUDE_FILE",
            "VARIABLES"
        };

        // Array of CMake instance properties that are not also properties of
        // global scope.
        private static readonly string[] _instanceOnlyProperties = new string[]
        {
            "CACHE_VARIABLES",
            "COMMANDS",
            "COMPONENTS",
            "MACROS",
            "VARIABLES"
        };

        // List of all CMake instances properties.  This must be filled in by
        // the static constructor.
        private static readonly List<string> _instanceProperties = new List<string>();
        
        // Array of CMake properties of global scope.
        private static readonly string[] _globalProperties = new string[]
        {
            "ALLOW_DUPLICATE_CUSTOM_TARGETS",
            "DEBUG_CONFIGURATIONS",
            "DISABLED_FEATURES",
            "ENABLED_FEATURES",
            "ENABLED_LANGUAGES",
            "FIND_LIBRARY_USE_LIB64_PATHS",
            "FIND_LIBRARY_USE_OPENBSD_VERSIONING",
            "GLOBAL_DEPENDS_DEBUG_MODE",
            "GLOBAL_DEPENDS_NO_CYCLES",
            "IN_TRY_COMPILE",
            "PACKAGES_FOUND",
            "PACKAGES_NOT_FOUND",
            "PREDEFINED_TARGETS_FOLDER",
            "REPORT_UNDEFINED_PROPERTIES",
            "RULE_LAUNCH_COMPILE",
            "RULE_LAUNCH_CUSTOM",
            "RULE_LAUNCH_LINK",
            "RULE_MESSAGES",
            "TARGET_ARCHIVES_MAY_BE_SHARED_LIBS",
            "TARGET_SUPPORTES_SHARED_LIBS",
            "USE_FOLDERS"
        };

        // Map from CMake commands to standard properties.
        private static readonly Dictionary<CMakeCommandId, IEnumerable<string>>
            _commandProperties = new Dictionary<CMakeCommandId, IEnumerable<string>>()
        {
            { CMakeCommandId.GetTargetProperty,         _targetProperties },
            { CMakeCommandId.SetTargetProperties,       _targetProperties },
            { CMakeCommandId.GetSourceFileProperty,     _sourceFileProperties },
            { CMakeCommandId.SetSourceFilesProperties,  _sourceFileProperties },
            { CMakeCommandId.GetTestProperty,           _testProperties },
            { CMakeCommandId.SetTestsProperties,        _testProperties },
            { CMakeCommandId.GetDirectoryProperty,      _directoryProperties },
            { CMakeCommandId.SetDirectoryProperties,    _directoryProperties },
            { CMakeCommandId.GetCMakeProperty,          _instanceProperties }
        };

        static CMakeProperties()
        {
            // All properties of global scope are also properties of the CMake instance.
            _instanceProperties.AddRange(_instanceOnlyProperties);
            _instanceProperties.AddRange(_globalProperties);
        }

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
            case CMakeCommandId.GetTestProperty:
            case CMakeCommandId.GetDirectoryProperty:
            case CMakeCommandId.GetCMakeProperty:
                return 1;
            case CMakeCommandId.GetTargetProperty:
            case CMakeCommandId.GetSourceFileProperty:
                return 2;
            default:
                return -1;
            }
        }

        public static int GetObjectParameterIndex(CMakeCommandId id)
        {
            switch (id)
            {
            case CMakeCommandId.GetTestProperty:
                return 0;
            case CMakeCommandId.GetTargetProperty:
            case CMakeCommandId.GetSourceFileProperty:
                return 1;
            default:
                return -1;
            }
        }
    }
}
