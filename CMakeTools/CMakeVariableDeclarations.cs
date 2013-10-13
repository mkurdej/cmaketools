/* ****************************************************************************
 * 
 * Copyright (C) 2012-2013 by David Golub.  All rights reserved.
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
using System.Linq;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Type of CMake variables.
    /// </summary>
    enum CMakeVariableType
    {
        Variable,
        EnvVariable,
        CacheVariable
    }

    /// <summary>
    /// Declarations object containing CMake variables
    /// </summary>
    class CMakeVariableDeclarations : CMakeItemDeclarations
    {
        // Array of standard CMake variables.
        private static readonly string[] _standardVariables = new string[]
        {
            "APPLE",
            "BORLAND",
            "BUILD_SHARED_LIBS",
            "CMAKE_ABSOLUTE_DESTINATION_FILES",
            "CMAKE_AR",
            "CMAKE_ARCHIVE_OUTPUT_DIRECTORY",
            "CMAKE_ARGC",
            "CMAKE_ARGV0",
            "CMAKE_AUTOMOC",
            "CMAKE_AUTOMOC_MOC_OPTIONS",
            "CMAKE_AUTOMOC_RELAXED_MODE",
            "CMAKE_BACKWARDS_COMPATIBILITY",
            "CMAKE_BINARY_DIR",
            "CMAKE_BUILD_TOOL",
            "CMAKE_BUILD_TYPE",
            "CMAKE_BUILD_WITH_INSTALL_RPATH",
            "CMAKE_CACHE_MAJOR_VERSION",
            "CMAKE_CACHE_MINOR_VERSION",
            "CMAKE_CACHE_PATCH_VERSION",
            "CMAKE_CACHEFILE_DIR",
            "CMAKE_CFG_INTDIR",
            "CMAKE_CL_64",
            "CMAKE_COLOR_MAKEFILE",
            "CMAKE_COMMAND",
            "CMAKE_COMPILER_2005",
            "CMAKE_COMPILER_IS_GNUCC",
            "CMAKE_COMPILER_IS_GNUCXX",
            "CMAKE_COMPILER_IS_GNUG77",
            "CMAKE_CONFIGURATION_TYPES",
            "CMAKE_CROSSCOMPILING",
            "CMAKE_CTEST_COMMAND",
            "CMAKE_CURRENT_BINARY_DIR",
            "CMAKE_CURRENT_LIST_DIR",
            "CMAKE_CURRENT_LIST_FILE",
            "CMAKE_CURRENT_LIST_LINE",
            "CMAKE_CURRENT_SOURCE_DIR",
            "CMAKE_DL_LIBS",
            "CMAKE_DEBUG_POSTFIX",
            "CMAKE_DEBUG_TARGET_PROPERTIES",
            "CMAKE_EDIT_COMMAND",
            "CMAKE_ERROR_DEPRECATED",
            "CMAKE_ERROR_ON_ABSOLUTE_INSTALL_DESTINATION",
            "CMAKE_EXE_LINKER_FLAGS",
            "CMAKE_EXECUTABLE_SUFFIX",
            "CMAKE_EXTRA_GENERATOR",
            "CMAKE_EXTRA_SHARED_LIBRARY_SUFFIXES",
            "CMAKE_FIND_LIBRARY_PREFIXES",
            "CMAKE_FIND_LIBRARY_SUFFIXES",
            "CMAKE_FIND_PACKAGE_WARN_NO_MODULE",
            "CMAKE_Fortran_FORMAT",
            "CMAKE_Fortran_MODDIR_DEFAULT",
            "CMAKE_Fortran_MODDIR_FLAG",
            "CMAKE_Fortran_MODOUT_FLAG",
            "CMAKE_Fortran_MODULE_DIRECTORY",
            "CMAKE_GENERATOR",
            "CMAKE_GENERATOR_TOOLSET",
            "CMAKE_GNUtoMS",
            "CMAKE_HOME_DIRECTORY",
            "CMAKE_HOST_APPLE",
            "CMAKE_HOST_SYSTEM",
            "CMAKE_HOST_SYSTEM_NAME",
            "CMAKE_HOST_SYSTEM_PROCESSOR",
            "CMAKE_HOST_SYSTEM_VERSION",
            "CMAKE_HOST_UNIX",
            "CMAKE_HOST_WIN32",
            "CMAKE_IGNORE_PATH",
            "CMAKE_IMPORT_LIBRARY_PREFIX",
            "CMAKE_IMPORT_LIBRARY_SUFFIX",
            "CMAKE_INCLUDE_CURRENT_DIR",
            "CMAKE_INCLUDE_CURRENT_DIR_IN_INTERFACE",
            "CMAKE_INCLUDE_PATH",
            "CMAKE_INSTALL_DEFAULT_COMPONENT_NAME",
            "CMAKE_INSTALL_NAME_DIR",
            "CMAKE_INSTALL_PREFIX",
            "CMAKE_INSTALL_RPATH",
            "CMAKE_INSTALL_RPATH_USE_LINK_PATH",
            "CMAKE_INTERNAL_PLATFORM_ABI",
            "CMAKE_LIBRARY_ARCHITECTURE",
            "CMAKE_LIBRARY_ARCHITECTURE_REGEX",
            "CMAKE_LIBRARY_OUTPUT_DIRECTORY",
            "CMAKE_LIBRARY_PATH",
            "CMAKE_LIBRARY_PATH_FLAG",
            "CMAKE_LINK_DEF_FILE_FLAG",
            "CMAKE_LINK_DEPENDS_NO_SHARED",
            "CMAKE_LINK_INTERFACE_LIBRARIES",
            "CMAKE_LINK_LIBRARY_FILE_FLAG",
            "CMAKE_LINK_LIBRARY_FLAG",
            "CMAKE_LINK_LIBRARY_SUFFIX",
            "CMAKE_MACOSX_BUNDLE",
            "CMAKE_MAJOR_VERSION",
            "CMAKE_MAKE_PROGRAM",
            "CMAKE_MFC_FLAG",
            "CMAKE_MINIMUM_REQUIRED_VERSION",
            "CMAKE_MINOR_VERSION",
            "CMAKE_MODULE_LINKER_FLAGS",
            "CMAKE_MODULE_PATH",
            "CMAKE_NO_BUILTIN_CHRPATH",
            "CMAKE_NOT_USING_CONFIG_FLAGS",
            "CMAKE_OBJECT_PATH_MAX",
            "CMAKE_PARENT_LIST_FILE",
            "CMAKE_PATCH_VERSION",
            "CMAKE_PDB_OUTPUT_DIRECTORY",
            "CMAKE_POSITION_INDEPENDENT_CODE",
            "CMAKE_PREFIX_PATH",
            "CMAKE_PROGRAM_PATH",
            "CMAKE_PROJECT_NAME",
            "CMAKE_RANLIB",
            "CMAKE_ROOT",
            "CMAKE_RUNTIME_OUTPUT_DIRECTORY",
            "CMAKE_SCRIPT_MODE_FILE",
            "CMAKE_SHARED_LIBRARY_PREFIX",
            "CMAKE_SHARED_LIBRARY_SUFFIX",
            "CMAKE_SHARED_LINKER_FLAGS",
            "CMAKE_SHARED_MODULE_PREFIX",
            "CMAKE_SHARED_MODULE_SUFFIX",
            "CMAKE_SIZEOF_VOID_P",
            "CMAKE_SKIP_BUILD_RPATH",
            "CMAKE_SKIP_INSTALL_ALL_DEPENDENCY",
            "CMAKE_SKIP_INSTALL_RPATH",
            "CMAKE_SKIP_RPATH",
            "CMAKE_SOURCE_DIR",
            "CMAKE_STANDARD_LIBRARIES",
            "CMAKE_STATIC_LIBRARY_PREFIX",
            "CMAKE_STATIC_LIBRARY_SUFFIX",
            "CMAKE_STATIC_LINKER_FLAGS",
            "CMAKE_SYSTEM",
            "CMAKE_SYSTEM_IGNORE_PATH",
            "CMAKE_SYSTEM_INCLUDE_PATH",
            "CMAKE_SYSTEM_LIBRARY_PATH",
            "CMAKE_SYSTEM_NAME",
            "CMAKE_SYSTEM_PREFIX_PATH",
            "CMAKE_SYSTEM_PROCESSOR",
            "CMAKE_SYSTEM_PROGRAM_PATH",
            "CMAKE_SYSTEM_VERSION",
            "CMAKE_TRY_COMPILE_CONFIGURATION",
            "CMAKE_TWEAK_VERSION",
            "CMAKE_USE_RELATIVE_PATHS",
            "CMAKE_USER_MAKE_RULES_OVERRIDE",
            "CMAKE_USING_VC_FREE_TOOLS",
            "CMAKE_VERBOSE_MAKEFILE",
            "CMAKE_VERSION",
            "CMAKE_VISIBILITY_INLINES_HIDDEN",
            "CMAKE_VS_PLATFORM_TOOLSET",
            "CMAKE_WARN_DEPRECATED",
            "CMAKE_WARN_ON_ABSOLUTE_INSTALL_DESTINATION",
            "CMAKE_WIN32_EXECUTABLE",
            "CMAKE_XCODE_PLATFORM_TOOLSET",
            "CYGWIN",
            "EXECUTABLE_OUTPUT_PATH",
            "LIBRARY_OUTPUT_PATH",
            "MSVC",
            "MSVC10",
            "MSVC11",
            "MSVC12",
            "MSVC60",
            "MSVC70",
            "MSVC71",
            "MSVC80",
            "MSVC90",
            "MSVC_IDE",
            "MSVC_VERSION",
            "PROJECT_BINARY_DIR",
            "PROJECT_NAME",
            "PROJECT_SOURCE_DIR",
            "UNIX",
            "WIN32",
            "XCODE_VERSION"
        };

        // Array of standard CMake variables defined for each language.
        private static readonly string[] _standardLangVariables = new string[]
        {
            "CMAKE_{0}_ARCHIVE_APPEND",
            "CMAKE_{0}_ARCHIVE_CREATE",
            "CMAKE_{0}_ARCHIVE_FINISH",
            "CMAKE_{0}_COMPILER",
            "CMAKE_{0}_COMPILER_ABI",
            "CMAKE_{0}_COMPILER_ID",
            "CMAKE_{0}_COMPILER_LOADED",
            "CMAKE_{0}_COMPILER_VERSION",
            "CMAKE_{0}_COMPILE_OBJECT",
            "CMAKE_{0}_CREATE_SHARED_LIBRARY",
            "CMAKE_{0}_CREATE_SHARED_MODULE",
            "CMAKE_{0}_CREATE_STATIC_LIBRARY",
            "CMAKE_{0}_FLAGS_DEBUG",
            "CMAKE_{0}_FLAGS_MINSIZEREL",
            "CMAKE_{0}_FLAGS_RELEASE",
            "CMAKE_{0}_FLAGS_RELWITHDEBINFO",
            "CMAKE_{0}_IGNORE_EXTENSIONS",
            "CMAKE_{0}_IMPLICIT_INCLUDE_DIRECTORIES",
            "CMAKE_{0}_IMPLICIT_LINK_DIRECTORIES",
            "CMAKE_{0}_IMPLICIT_LINK_FRAMEWORK_DIRECTORIES",
            "CMAKE_{0}_IMPLICIT_LINK_LIBRARIES",
            "CMAKE_{0}_LIBRARY_ARCHITECTURE",
            "CMAKE_{0}_LINKER_PREFERENCE",
            "CMAKE_{0}_LINKER_PREFERENCE_PROPAGATES",
            "CMAKE_{0}_LINK_EXECUTABLE",
            "CMAKE_{0}_OUTPUT_EXTENSION",
            "CMAKE_{0}_PLATFORM_ID",
            "CMAKE_{0}_SIZEOF_DATA_PTR",
            "CMAKE_{0}_SOURCE_FILE_EXTENSIONS",
            "CMAKE_USER_MAKE_RULES_OVERRIDE_{0}"
        };

        // Array of standard cache variables.
        private static readonly string[] _standardCacheVariables = new string[]
        {
            "CMAKE_BACKWARDS_COMPATIBILITY",
            "CMAKE_BUILD_TOOL",
            "CMAKE_CACHE_MAJOR_VERSION",
            "CMAKE_CACHE_MINOR_VERSION",
            "CMAKE_CACHE_PATCH_VERSION",
            "CMAKE_COMMAND",
            "CMAKE_CONFIGURATION_TYPES",
            "CMAKE_EDIT_COMMAND",
            "CMAKE_EXE_LINKER_FLAGS",
            "CMAKE_GENERATOR",
            "CMAKE_HOME_DIRECTORY",
            "CMAKE_INSTALL_PREFIX",
            "CMAKE_MAKE_PROGRAM",
            "CMAKE_PROJECT_NAME",
            "CMAKE_ROOT",
            "CMAKE_SKIP_RPATH",
            "CMAKE_USE_RELATIVE_PATHS",
            "CMAKE_VERBOSE_MAKEFILE",
            "EXECUTABLE_OUTPUT_PATH",
            "LIBRARY_OUTPUT_PATH",
            "PROJECT_BINARY_DIR",
            "PROJECT_SOURCE_DIR"
        };

        // Array of standard cache variables defined for each language.
        private static readonly string[] _standardLangCacheVariables = new string[]
        {
            "CMAKE_{0}_FLAGS_DEBUG",
            "CMAKE_{0}_FLAGS_MINSIZEREL",
            "CMAKE_{0}_FLAGS_RELEASE",
            "CMAKE_{0}_FLAGS_RELWITHDEBINFO"
        };

        // Array of standard environment variables.  This list was taken from
        // http://en.wikipedia.org/wiki/Environment_variable.
        private static readonly string[] _standardEnvVariables = new string[]
        {
            "ALLUSERSPROFILE",
            "APPDATA",
            "COMPUTERNAME",
            "COMMONPROGRAMFILES",
            "COMSPEC",
            "HOMEDRIVE",
            "HOMEPATH",
            "LOCALAPPDATA",
            "LOGONSERVER",
            "PATH",
            "PATHEXT",
            "PROGRAMDATA",
            "PROGRAMFILES",
            "PROMPT",
            "PSMODULEPATH",
            "PUBLIC",
            "SYSTEMDRIVE",
            "SYSTEMROOT",
            "TEMP",
            "TMP",
            "USERDOMAIN",
            "USERDATA",
            "USERNAME",
            "USERPROFILE",
            "WINDIR"
        };

        public CMakeVariableDeclarations(List<string> userVariables,
            CMakeVariableType type)
        {
            AddItems(GetStandardVariables(type), ItemType.Variable);
            if (userVariables != null)
            {
                AddItems(userVariables, ItemType.Variable);
            }
        }

        public override bool IsCommitChar(string textSoFar, int selected,
            char commitCharacter)
        {
            if (commitCharacter == '-')
            {
                // Hyphens are allowed in CMake variable names.
                return false;
            }
            return base.IsCommitChar(textSoFar, selected, commitCharacter);
        }

        /// <summary>
        /// Check whether the given string names a standard variable.
        /// </summary>
        /// <param name="varName">The string to check.</param>
        /// <param name="type">The type of variable to check for.</param>
        /// <returns>
        /// True if the string names a standard variable or false otherwise.
        /// </returns>
        public static bool IsStandardVariable(string varName, CMakeVariableType type)
        {
            return GetStandardVariables(type).IndexOf(varName) >= 0;
        }

        private static List<string> GetStandardVariables(CMakeVariableType type)
        {
            List<string> vars = new List<string>();
            switch (type)
            {
            case CMakeVariableType.Variable:
            case CMakeVariableType.CacheVariable:
                // All cache variables also serve as ordinary CMake variables, but not
                // the other way around.
                if (type == CMakeVariableType.Variable)
                {
                    vars.AddRange(_standardVariables);
                }
                vars.AddRange(_standardCacheVariables);
                string path = CMakePath.FindCMakeModules();
                if (path != null)
                {
                    IEnumerable<string> languages =
                        CMakeLanguageDeclarations.GetLanguagesFromDir(path);
                    foreach (string language in languages)
                    {
                        if (type == CMakeVariableType.Variable)
                        {
                            vars.AddRange(_standardLangVariables.Select(
                                x => string.Format(x, language)));
                        }
                        vars.AddRange(_standardLangCacheVariables.Select(
                            x => string.Format(x, language)));
                    }
                }
                break;
            case CMakeVariableType.EnvVariable:
                vars.AddRange(_standardEnvVariables);
                break;
            }
            return vars;
        }
    }
}
