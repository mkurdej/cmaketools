// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System;
using System.Collections.Generic;

namespace CMakeTools
{
    /// <summary>
    /// Numeric identifiers for CMake keywords.
    /// </summary>
    enum CMakeKeywordId
    {
        Unspecified = 0,
        AddCustomCommand,
        AddCustomTarget,
        AddExecutable,
        AddLibrary,
        AddSubdirectory,
        AddTest,
        BuildCommand,
        CMakeMinimumRequired,
        CMakePolicy,
        ConfigureFile,
        CreateTestSourcelist,
        DefineProperty,
        Else,
        ElseIf,
        EndIf,
        If
    }

    /// <summary>
    /// Utility class to identify CMake keywords.
    /// </summary>
    static class CMakeKeywords
    {
        // Array of CMake commands.
        private static string[] _keywords = new string[]
        {
            "add_custom_command",
            "add_custom_target",
            "add_definitions",
            "add_dependencies",
            "add_executable",
            "add_library",
            "add_subdirectory",
            "add_test",
            "aux_source_directory",
            "break",
            "build_command",
            "cmake_minimum_required",
            "cmake_policy",
            "configure_file",
            "create_test_sourcelist",
            "define_property",
            "else",
            "elseif",
            "enable_language",
            "enable_testing",
            "endforeach",
            "endfunction",
            "endif",
            "endmacro",
            "endwhile",
            "execute_process",
            "export",
            "file",
            "find_file",
            "find_library",
            "find_package",
            "find_path",
            "find_program",
            "fltk_wrap_ui",
            "foreach",
            "function",
            "get_cmake_property",
            "get_directory_property",
            "get_filename_component",
            "get_property",
            "get_source_file_property",
            "get_target_property",
            "get_test_property",
            "if",
            "include",
            "include_directories",
            "include_external_msproject",
            "include_regular_expression",
            "install",
            "link_directories",
            "list",
            "load_cache",
            "load_command",
            "macro",
            "mark_as_advanced",
            "math",
            "message",
            "option",
            "project",
            "qt_wrap_cpp",
            "qt_wrap_ui",
            "remove_definitions",
            "return",
            "separate_arguments",
            "set",
            "set_directory_properties",
            "set_property",
            "set_source_files_properties",
            "set_target_properties",
            "set_tests_properties",
            "site_name",
            "source_group",
            "string",
            "target_link_libraries",
            "try_compile",
            "try_run",
            "unset",
            "variable_watch",
            "while"
        };

        // Map from keyword strings to numeric keyword identifier.
        private static Dictionary<string, CMakeKeywordId> _keywordIdMap =
            new Dictionary<string, CMakeKeywordId>
        {
            { "add_custom_command",     CMakeKeywordId.AddCustomCommand },
            { "add_custom_target",      CMakeKeywordId.AddCustomTarget },
            { "add_executable",         CMakeKeywordId.AddExecutable },
            { "add_library",            CMakeKeywordId.AddLibrary },
            { "add_subdirectory",       CMakeKeywordId.AddSubdirectory },
            { "add_test",               CMakeKeywordId.AddTest },
            { "build_command",          CMakeKeywordId.BuildCommand },
            { "cmake_minimum_required", CMakeKeywordId.CMakeMinimumRequired },
            { "cmake_policy",           CMakeKeywordId.CMakePolicy },
            { "configure_file",         CMakeKeywordId.ConfigureFile },
            { "create_test_sourcelist", CMakeKeywordId.CreateTestSourcelist },
            { "define_property",        CMakeKeywordId.DefineProperty },
            { "else",                   CMakeKeywordId.Else },
            { "elseif",                 CMakeKeywordId.ElseIf },
            { "endif",                  CMakeKeywordId.EndIf },
            { "if",                     CMakeKeywordId.If }
        };

        // Array of keywords used with the ADD_CUSTOM_COMMAND command.
        private static string[] _addCustomCommandKeywords = new string[]
        {
            "append",
            "args",
            "command",
            "comment",
            "depends",
            "implicit_depends",
            "main_dependency",
            "output",
            "post_build",
            "pre_build",
            "pre_link",
            "target",
            "verbatim",
            "working_directory"
        };

        // Array of keywords used with the ADD_CUSTOM_TARGET command.
        private static string[] _addCustomTargetKeywords = new string[]
        {
            "all",
            "command",
            "comment",
            "depends",
            "sources",
            "verbatim",
            "working_directory"
        };

        // Array of keywords used with the ADD_EXECUTABLE command.
        private static string[] _addExecutableKeywords = new string[]
        {
            "exclude_from_all",
            "macosx_bundle",
            "win32"
        };

        // Array of keywords used with the ADD_LIBRARY command.
        private static string[] _addLibraryKeywords = new string[]
        {
            "exclude_from_all",
            "imported",
            "module",
            "shared",
            "static",
            "unknown"
        };

        // Array of keywords used with the ADD_SUBDIRECTORY command.
        private static string[] _addSubdirectoryKeywords = new string[]
        {
            "exclude_from_all"
        };

        // Array of keywords used with the ADD_TEST command.
        private static string[] _addTestKeywords = new string[]
        {
            "command",
            "configurations",
            "name",
            "working_directory",
        };

        // Array of keywords used with the BUILD_COMMAND command.
        private static string[] _buildCommandKeywords = new string[]
        {
            "configuration",
            "project_name",
            "target"
        };

        // Array of keywords used with the CMAKE_MINIMUM_REQUIRED command.
        private static string[] _cmakeMinimumRequiredKeywords = new string[]
        {
            "fatal_error",
            "version"
        };

        // Array of keywords used with the CMAKE_POLICY command.
        private static string[] _cmakePolicyKeywords = new string[]
        {
            "get",
            "new",
            "old",
            "pop",
            "push",
            "set",
            "version"
        };

        // Array of keywords used with the CONFIGURE_FILE command.
        private static string[] _configureFileKeywords = new string[]
        {
            "copyonly",
            "crlf",
            "dos",
            "escape_quotes",
            "lf",
            "newline_style",
            "unix",
            "win32"
        };

        // Array of keywords used with the CREATE_TEST_SOURCELIST command.
        private static string[] _createTestSourcelistKeywords = new string[]
        {
            "extra_include",
            "function"
        };

        // Array of keywords used with the DEFINE_PROPERTY command.
        private static string[] _definePropertyKeywords = new string[]
        {
            "brief_docs",
            "cached_variable",
            "directory",
            "full_docs",
            "global",
            "inherited",
            "property",
            "source",
            "target",
            "test",
            "variable"
        };

        // Array of keywords used with the IF command.
        private static string[] _ifKeywords = new string[]
        {
            "and",
            "command",
            "defined",
            "equal",
            "exists",
            "greater",
            "is_absolute",
            "is_directory",
            "is_newer_then",
            "is_symlink",
            "less",
            "matches",
            "not",
            "or",
            "policy",
            "strequal",
            "strgreater",
            "strless",
            "target",
            "version_equal",
            "version_greater",
            "version_less"
        };

        private static string[] _endIfKeywords = _ifKeywords;
        private static string[] _elseKeywords = _ifKeywords;
        private static string[] _elseIfKeywords = _ifKeywords;

        // Arrays of keywords that appear in parentheses after other keywords.
        // The items in this list must be in the same order as the their corresponding
        // keyword identifier in the CMakeKeywordId enumeration.
        private static string[][] _keywordArrays = new string[][]
        {
            null,
            _addCustomCommandKeywords,
            _addCustomTargetKeywords,
            _addExecutableKeywords,
            _addLibraryKeywords,
            _addSubdirectoryKeywords,
            _addTestKeywords,
            _buildCommandKeywords,
            _cmakeMinimumRequiredKeywords,
            _cmakePolicyKeywords,
            _configureFileKeywords,
            _createTestSourcelistKeywords,
            _definePropertyKeywords,
            _elseKeywords,
            _elseIfKeywords,
            _endIfKeywords,
            _ifKeywords
        };

        /// <summary>
        /// Check whether the specified token is a CMake command.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>True if the token is command or false otherwise.</returns>
        public static bool IsCommand(string token)
        {
            int index = Array.BinarySearch(_keywords, token.ToLower());
            return index >= 0;
        }

        /// <summary>
        /// Check whether the specified token appearing in parentheses should be
        /// considered a keyword.
        /// </summary>
        /// <param name="containingKeyword">
        /// Identifier of the command preceding the parentheses.
        /// </param>
        /// <param name="token">Token to check.</param>
        /// <returns>True if the token is keyword or false otherwise.</returns>
        public static bool IsKeyword(CMakeKeywordId containingKeyword, string token)
        {
            if (containingKeyword == CMakeKeywordId.Unspecified)
            {
                return false;
            }
            string[] keywordArray = _keywordArrays[(int)containingKeyword];
            int index = Array.BinarySearch(keywordArray, token.ToLower());
            return index >= 0;
        }

        /// <summary>
        /// Get the keyword identifier corresponding to a given token string.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>The keyword identifier.</returns>
        public static CMakeKeywordId GetKeywordId(string token)
        {
            if (!_keywordIdMap.ContainsKey(token.ToLower()))
            {
                return CMakeKeywordId.Unspecified;
            }
            return _keywordIdMap[token.ToLower()];
        }
    }
}
