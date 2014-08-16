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

using System;
using System.Collections.Generic;

namespace CMakeTools
{
    /// <summary>
    /// CMake property types.
    /// </summary>
    enum CMakePropertyType
    {
        Unspecified = -1,
        Cache = 0,
        Directory,
        Global,
        Source,
        Target,
        Test,
        Variable
    }

    /// <summary>
    /// Utility class to identify CMake properties.
    /// </summary>
    static class CMakeProperties
    {
        // Array of CMake property type keywords.  These must be in alphabetical order.
        private static readonly string[] _propertyTypeKeywords = new string[]
        {
            "CACHE",
            "DIRECTORY",
            "GLOBAL",
            "SOURCE",
            "TARGET",
            "TEST",
            "VARIABLE"
        };

        // Array of CMake target properties.
        private static readonly string[] _targetProperties = new string[]
        {
            "ALIASED_TARGET",
            "ARCHIVE_OUTPUT_DIRECTORY",
            "ARCHIVE_OUTPUT_NAME",
            "AUTOGEN_TARGET_DEPENDS",
            "AUTOMOC",
            "AUTOMOC_MOC_OPTIONS",
            "AUTORCC",
            "AUTORCC_OPTIONS",
            "AUTOUIC",
            "AUTOUIC_OPTIONS",
            "BUILD_WITH_INSTALL_RPATH",
            "BUNDLE",
            "BUNDLE_EXTENSION",
            "COMPATIBLE_INTERFACE_BOOL",
            "COMPATIBLE_INTERFACE_NUMBER_MAX",
            "COMPATIBLE_INTERFACE_NUMBER_MIN",
            "COMPATIBLE_INTERFACE_STRING",
            "COMPILE_DEFINITIONS",
            "COMPILE_FLAGS",
            "COMPILE_OPTIONS",
            "DEBUG_POSTFIX",
            "DEFINE_SYMBOL",
            "ENABLE_EXPORTS",
            "EXCLUDE_FROM_ALL",
            "EXCLUDE_FROM_DEFAULT_BUILD",
            "EXPORT_NAME",
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
            "INTERFACE_AUTOUIC_OPTIONS",
            "INTERFACE_COMPILE_DEFINITIONS",
            "INTERFACE_COMPILE_OPTIONS",
            "INTERFACE_INCLUDE_DEFINITIONS",
            "INTERFACE_LINK_LIBRARIES",
            "INTERFACE_POSITION_INDEPENDENT_CODE",
            "INTERFACE_SYSTEM_INCLUDE_DIRECTORIES",
            "INTERPROCEDURAL_OPTIMIZATION",
            "JOB_POOL_COMPILE",
            "JOB_POOL_LINK",
            "LABELS",
            "LIBRARY_OUTPUT_DIRECTORY",
            "LIBRARY_OUTPUT_NAME",
            "LINKER_LANGUAGE",
            "LINK_DEPENDS",
            "LINK_DEPENDS_NO_SHARED",
            "LINK_FLAGS",
            "LINK_INTERFACE_LIBRARIES",
            "LINK_INTERFACE_MULTIPLICITY",
            "LINK_LIBRARIES",
            "LINK_SEARCH_END_STATIC",
            "LINK_SEARCH_START_STATIC",
            "LOCATION",
            "MACOSX_BUNDLE",
            "MACOSX_BUNDLE_INFO_PLIST",
            "MACOSX_FRAMEWORK_INFO_PLIST",
            "MACOSX_RPATH",
            "NAME",
            "NO_SONAME",
            "NO_SYSTEM_FROM_IMPORTED",
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
            "VISIBILITY_INLINES_HIDDEN",
            "VS_DOTNET_REFERENCES",
            "VS_DOTNET_TARGET_FRAMEWORK_VERSION",
            "VS_GLOBAL_KEYWORD",
            "VS_GLOBAL_PROJECT_TYPES",
            "VS_GLOBAL_ROOTNAMESPACE",
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
            "AUTORCC_OPTIONS",
            "AUTOUIC_OPTIONS",
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
            "SKIP_RETURN_CODE",
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
            "CMAKE_CONFIGURE_DEPENDS",
            "COMPILE_DEFINITIONS",
            "COMPILE_OPTIONS",
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

        // Array of CMake cache entry properties.
        private static readonly string[] _cacheProperties = new string[]
        {
            "ADVANCED",
            "HELPSTRING",
            "MODIFIED",
            "STRINGS",
            "TYPE",
            "VALUE"
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
            "AUTOGEN_TARGETS_FOLDER",
            "AUTOMOC_TARGETS_FOLDER",
            "DEBUG_CONFIGURATIONS",
            "DISABLED_FEATURES",
            "ECLIPSE_EXTRA_NATURES",
            "ENABLED_FEATURES",
            "ENABLED_LANGUAGES",
            "FIND_LIBRARY_USE_LIB64_PATHS",
            "FIND_LIBRARY_USE_OPENBSD_VERSIONING",
            "GLOBAL_DEPENDS_DEBUG_MODE",
            "GLOBAL_DEPENDS_NO_CYCLES",
            "IN_TRY_COMPILE",
            "JOB_POOLS",
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

        // Map from CMake commands to property types.
        private static readonly Dictionary<CMakeCommandId, CMakePropertyType>
            _commandPropertyTypes = new Dictionary<CMakeCommandId, CMakePropertyType>()
        {
            { CMakeCommandId.GetTargetProperty,         CMakePropertyType.Target },
            { CMakeCommandId.SetTargetProperties,       CMakePropertyType.Target },
            { CMakeCommandId.GetSourceFileProperty,     CMakePropertyType.Source },
            { CMakeCommandId.SetSourceFilesProperties,  CMakePropertyType.Source },
            { CMakeCommandId.GetTestProperty,           CMakePropertyType.Test },
            { CMakeCommandId.SetTestsProperties,        CMakePropertyType.Test },
            { CMakeCommandId.GetDirectoryProperty,      CMakePropertyType.Directory },
            { CMakeCommandId.SetDirectoryProperties,    CMakePropertyType.Directory }
        };

        // Map from CMake property types to standard properties.
        private static readonly Dictionary<CMakePropertyType, IEnumerable<string>>
            _allProperties = new Dictionary<CMakePropertyType, IEnumerable<string>>()
        {
            { CMakePropertyType.Cache,      _cacheProperties },
            { CMakePropertyType.Directory,  _directoryProperties },
            { CMakePropertyType.Global,     _globalProperties },
            { CMakePropertyType.Source,     _sourceFileProperties },
            { CMakePropertyType.Target,     _targetProperties },
            { CMakePropertyType.Test,       _testProperties }
        };

        static CMakeProperties()
        {
            // All properties of global scope are also properties of the CMake instance.
            _instanceProperties.AddRange(_instanceOnlyProperties);
            _instanceProperties.AddRange(_globalProperties);
        }

        /// <summary>
        /// Get the keywords used to identify CMake property types.
        /// </summary>
        /// <returns>A collection of keyword.</returns>
        public static IEnumerable<string> GetPropertyTypeKeywords()
        {
            return _propertyTypeKeywords;
        }

        /// <summary>
        /// Get the property type represented by the specified keyword.
        /// </summary>
        /// <param name="keyword">A property type keyword.</param>
        /// <returns>A property type identifier.</returns>
        public static CMakePropertyType GetPropertyTypeFromKeyword(string keyword)
        {
            int index = Array.BinarySearch(_propertyTypeKeywords, keyword);
            if (index < 0)
            {
                return CMakePropertyType.Unspecified;
            }
            return (CMakePropertyType)index;
        }

        /// <summary>
        /// Get the CMake properties of the specific type.
        /// </summary>
        /// <param name="type">A property type.</param>
        /// <returns>A collection of property names.</returns>
        public static IEnumerable<string> GetPropertiesOfType(CMakePropertyType type)
        {
            if (_allProperties.ContainsKey(type))
            {
                return _allProperties[type];
            }
            return null;
        }

        /// <summary>
        /// Get the CMake properties to be displayed for use with the specified command.
        /// </summary>
        /// <param name="id">A command identifier.</param>
        /// <returns>A collection of property names.</returns>
        public static IEnumerable<string> GetPropertiesForCommand(CMakeCommandId id)
        {
            if (id == CMakeCommandId.GetCMakeProperty)
            {
                // The properties used with this command do not correspond to any of the
                // property types used with GET_PROPERTY and SET_PROPERTY.  Therefore, it
                // is treated as a special case here.
                return _instanceProperties;
            }
            if (_commandPropertyTypes.ContainsKey(id))
            {
                return GetPropertiesOfType(_commandPropertyTypes[id]);
            }
            return null;
        }

        /// <summary>
        /// Get the property type that the specified command gets or sets.
        /// </summary>
        /// <param name="id">A command identifier.</param>
        /// <returns>A property type.</returns>
        public static CMakePropertyType GetPropertyTypeFromCommand(CMakeCommandId id)
        {
            if (_commandPropertyTypes.ContainsKey(id))
            {
                return _commandPropertyTypes[id];
            }
            return CMakePropertyType.Unspecified;
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

        /// <summary>
        /// Get the zero-based index of the parameter to the specified command that
        /// should specify the object from which a CMake property is retrieved.
        /// </summary>
        /// <param name="id">A command identifier</param>
        /// <returns>The index of the parameter specifying the object.</returns>
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

        /// <summary>
        /// Check if an object must be specified to retrieve a property of the specified
        /// type.
        /// </summary>
        /// <param name="type">A property type.</param>
        /// <returns>True if an object is require or false otherwise.</returns>
        public static bool IsObjectRequired(CMakePropertyType type)
        {
            return type == CMakePropertyType.Target ||
                type == CMakePropertyType.Source ||
                type == CMakePropertyType.Test ||
                type == CMakePropertyType.Cache;
        }
    }
}
