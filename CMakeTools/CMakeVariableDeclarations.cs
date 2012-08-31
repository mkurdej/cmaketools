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

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object containing CMake variables
    /// </summary>
    class CMakeVariableDeclarations : Declarations
    {
        // Array of standard CMake variables.
        private static string[] _standardVariables = new string[]
        {
            "APPLE",
            "BORLAND",
            "BUILD_SHARED_LIBS",
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
            "CMAKE_EDIT_COMMAND",
            "CMAKE_EXE_LINKER_FLAGS",
            "CMAKE_EXECUTABLE_SUFFIX",
            "CMAKE_EXTRA_GENERATOR",
            "CMAKE_EXTRA_SHARED_LIBRARY_SUFFIXES",
            "CMAKE_FIND_LIBRARY_PREFIXES",
            "CMAKE_FIND_LIBRARY_SUFFIXES",
            "CMAKE_GENERATOR",
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
            "CMAKE_INCLUDE_PATH",
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
            "CMAKE_LINK_INTERFACE_LIBRARIES",
            "CMAKE_LINK_LIBRARY_FILE_FLAG",
            "CMAKE_LINK_LIBRARY_FLAG",
            "CMAKE_LINK_LIBRARY_SUFFIX",
            "CMAKE_MAJOR_VERSION",
            "CMAKE_MAKE_PROGRAM",
            "CMAKE_MFC_FLAG",
            "CMAKE_MINOR_VERSION",
            "CMAKE_MODULE_PATH",
            "CMAKE_NO_BUILTIN_CHRPATH",
            "CMAKE_NOT_USING_CONFIG_FLAGS",
            "CMAKE_OBJECT_PATH_MAX",
            "CMAKE_PARENT_LIST_FILE",
            "CMAKE_PATCH_VERSION",
            "CMAKE_PREFIX_PATH",
            "CMAKE_PROGRAM_PATH",
            "CMAKE_PROJECT_NAME",
            "CMAKE_RANLIB",
            "CMAKE_ROOT",
            "CMAKE_RUNTIME_OUTPUT_DIRECTORY",
            "CMAKE_SCRIPT_MODE_FILE",
            "CMAKE_SHARED_LIBRARY_PREFIX",
            "CMAKE_SHARED_LIBRARY_SUFFIX",
            "CMAKE_SHARED_MODULE_PREFIX",
            "CMAKE_SHARED_MODULE_SUFFIX",
            "CMAKE_SIZEOF_VOID_P",
            "CMAKE_SKIP_BUILD_RPATH",
            "CMAKE_SKIP_INSTALL_ALL_DEPENDENCY",
            "CMAKE_SKIP_RPATH",
            "CMAKE_SOURCE_DIR",
            "CMAKE_STANDARD_LIBRARIES",
            "CMAKE_STATIC_LIBRARY_PREFIX",
            "CMAKE_STATIC_LIBRARY_SUFFIX",
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
            "CYGWIN",
            "EXECUTABLE_OUTPUT_PATH",
            "LIBRARY_OUTPUT_PATH",
            "MSVC",
            "MSVC80",
            "MSVC_IDE",
            "MSVC_VERSION",
            "PROJECT_BINARY_DIR",
            "PROJECT_NAME",
            "PROJECT_SOURCE_DIR",
            "UNIX",
            "WIN32",
            "XCODE_VERSION"
        };

        // Array of standard environment variables.  This list was taken from
        // http://en.wikipedia.org/wiki/Environment_variable.
        private static string[] _standardEnvVariables = new string[]
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

        // Array of variables to be displayed.
        private List<string> _variables;

        public CMakeVariableDeclarations(List<string> userVariables, bool useEnv = false)
        {
            _variables = new List<string>(
                useEnv ? _standardEnvVariables : _standardVariables);
            if (userVariables != null)
            {
                _variables.AddRange(userVariables);
            }
            _variables.Sort();
        }

        public override int GetCount()
        {
            return _variables.Count;
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
            // Always return the icon index for a public variable.
            return 138;
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _variables.Count)
            {
                return null;
            }
            return _variables[index];
        }

        /// <summary>
        /// Check whether the given string names a standard variable.
        /// </summary>
        /// <param name="varName">The string to check.</param>
        /// <param name="useEnv">
        /// Boolean value indicating whether to check for standard environment variables
        /// instead of standard CMake variables.
        /// </param>
        /// <returns>
        /// True if the string names a standard variable or false otherwise.
        /// </returns>
        public static bool IsStandardVariable(string varName, bool useEnv = false)
        {
            string[] variables = useEnv ? _standardEnvVariables : _standardVariables;
            return Array.BinarySearch(variables, varName.ToUpper()) >= 0;
        }
    }
}
