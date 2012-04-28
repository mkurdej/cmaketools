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
        EnableLanguage,
        EndForEach,
        EndIf,
        EndWhile,
        ExecuteProcess,
        Export,
        File,
        FindFile,
        FindLibrary,
        FindPackage,
        FindPath,
        FindProgram,
        ForEach,
        GetDirectoryProperty,
        GetFileNameComponent,
        GetProperty,
        If,
        Include,
        IncludeDirectories,
        Install,
        List,
        LoadCache,
        LoadCommand,
        MarkAsAdvanced,
        Message,
        Option,
        SeparateArguments,
        Set,
        SetDirectoryProperties,
        SetProperty,
        SetSourceFilesProperties,
        SetTargetProperties,
        SetTestsProperties,
        SourceGroup,
        String,
        TargetLinkLibraries,
        TryCompile,
        TryRun,
        Unset,
        While
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
            { "enable_language",        CMakeKeywordId.EnableLanguage },
            { "endforeach",             CMakeKeywordId.EndForEach },
            { "endif",                  CMakeKeywordId.EndIf },
            { "endwhile",               CMakeKeywordId.EndWhile },
            { "execute_process",        CMakeKeywordId.ExecuteProcess },
            { "export",                 CMakeKeywordId.Export },
            { "file",                   CMakeKeywordId.File },
            { "find_file",              CMakeKeywordId.FindFile },
            { "find_library",           CMakeKeywordId.FindLibrary },
            { "find_package",           CMakeKeywordId.FindPackage },
            { "foreach",                CMakeKeywordId.ForEach },
            { "get_directory_property", CMakeKeywordId.GetDirectoryProperty },
            { "get_filename_component", CMakeKeywordId.GetFileNameComponent },
            { "get_property",           CMakeKeywordId.GetProperty },
            { "if",                     CMakeKeywordId.If },
            { "include",                CMakeKeywordId.Include },
            { "include_directories",    CMakeKeywordId.IncludeDirectories },
            { "install",                CMakeKeywordId.Install },
            { "list",                   CMakeKeywordId.List },
            { "load_cache",             CMakeKeywordId.LoadCache },
            { "load_command",           CMakeKeywordId.LoadCommand },
            { "mark_as_advanced",       CMakeKeywordId.MarkAsAdvanced },
            { "message",                CMakeKeywordId.Message },
            { "option",                 CMakeKeywordId.Option },
            { "separate_arguments",     CMakeKeywordId.SeparateArguments },
            { "set",                    CMakeKeywordId.Set },
            { "set_directory_properties", CMakeKeywordId.SetDirectoryProperties },
            { "set_property",           CMakeKeywordId.SetProperty },
            { "set_source_files_properties", CMakeKeywordId.SetSourceFilesProperties },
            { "set_target_properties",  CMakeKeywordId.SetTargetProperties },
            { "set_tests_properties",   CMakeKeywordId.SetTestsProperties },
            { "source_group",           CMakeKeywordId.SourceGroup },
            { "string",                 CMakeKeywordId.String },
            { "target_link_libraries",  CMakeKeywordId.TargetLinkLibraries },
            { "try_compile",            CMakeKeywordId.TryCompile },
            { "try_run",                CMakeKeywordId.TryRun },
            { "unset",                  CMakeKeywordId.Unset },
            { "while",                  CMakeKeywordId.While }
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

        // Array of keywords used with the ENABLE_LANGUAGE command.
        private static string[] _enableLanguageKeywords = new string[]
        {
            "optional"
        };

        // Array of keywords used with the EXECUTE_PROCESS command.
        private static string[] _executeProcessKeywords = new string[]
        {
            "command",
            "error_file",
            "error_quiet",
            "error_strip_trailing_whitespace",
            "error_variable",
            "input_file",
            "output_file",
            "output_quiet",
            "output_strip_trailing_whitespace",
            "output_variable",
            "result_variable",
            "timeout",
            "working_directory"
        };

        // Array of keywords used with the EXPORT command.
        private static string[] _exportKeywords = new string[]
        {
            "append",
            "file",
            "namespace",
            "package",
            "targets"
        };

        // Array of keywords used with the FILE command.
        private static string[] _fileKeywords = new string[]
        {
            "append",
            "copy",
            "destination",
            "directory_permissions",
            "download",
            "exclude",
            "expected_md5",
            "file_permissions",
            "files_matching",
            "follow_symlinks",
            "glob",
            "glob_recurse",
            "group_execute",
            "group_read",
            "group_write",
            "hex",
            "inactivity_timeout",
            "install",
            "length_maximum",
            "length_minimum",
            "limit",
            "limit_count",
            "limit_input",
            "limit_output",
            "log",
            "make_directory",
            "md5",
            "newline_consume",
            "no_hex_conversion",
            "no_source_permissions",
            "offset",
            "owner_execute",
            "owner_read",
            "owner_write",
            "pattern",
            "permissions",
            "read",
            "regex",
            "relative",
            "relative_path",
            "remove",
            "remove_recurse",
            "rename",
            "setgid",
            "setuid",
            "sha1",
            "sha224",
            "sha256",
            "sha384",
            "sha512",
            "show_progress",
            "status",
            "strings",
            "timeout",
            "to_cmake_path",
            "to_native_path",
            "upload",
            "use_source_permissions",
            "world_execute",
            "world_read",
            "world_write",
            "write"
        };

        // Array of keywords used with the FIND_FILE command.
        private static string[] _findFileKeywords = new string[]
        {
            "cmake_find_root_path_both",
            "doc",
            "env",
            "hints",
            "names",
            "no_cmake_environment_path",
            "no_cmake_find_root_path",
            "no_cmake_path",
            "no_cmake_system_path",
            "no_default_path",
            "no_system_environment_path",
            "only_cmake_find_root_path",
            "path_suffixes",
            "paths"
        };

        private static string[] _findLibraryKeywords = _findFileKeywords;
        private static string[] _findPathKeywords = _findFileKeywords;
        private static string[] _findProgramKeywords = _findFileKeywords;

        // Array of keywords used with the FIND_PACKAGE command.
        private static string[] _findPackageKeywords = new string[]
        {
            "cmake_find_root_path_both",
            "components",
            "configs",
            "exact",
            "hints",
            "names",
            "no_cmake_builds_path",
            "no_cmake_environment_path",
            "no_cmake_find_root_path",
            "no_cmake_package_registry",
            "no_cmake_path",
            "no_cmake_system_package_registry",
            "no_cmake_system_path",
            "no_default_path",
            "no_module",
            "no_policy_scope",
            "no_system_environment_path",
            "only_cmake_find_root_path",
            "path_suffixes",
            "paths",
            "quiet",
            "required"
        };

        // Array of keywords used with the FOREACH command.
        private static string[] _forEachKeywords = new string[]
        {
            "in",
            "items",
            "lists",
            "range"
        };

        private static string[] _endForEachKeywords = _forEachKeywords;

        // Array of keywords used with the GET_DIRECTORY_PROPERTY command.
        private static string[] _getDirectoryPropertyKeywords = new string[]
        {
            "definition",
            "directory"
        };
        
        // Array of keywords used with the GET_FILENAME_COMPONENT command.
        private static string[] _getFileNameComponentKeywords = new string[]
        {
            "absolute",
            "cache",
            "ext",
            "name",
            "name_we",
            "path",
            "program",
            "program_args",
            "realpath"
        };

        // Array of keywords used with the GET_PROPERTY command.
        private static string[] _getPropertyKeywords = new string[]
        {
            "brief_docs",
            "cache",
            "defined",
            "directory",
            "full_docs",
            "global",
            "property",
            "set",
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
        private static string[] _whileKeywords = _ifKeywords;
        private static string[] _endWhileKeywords = _ifKeywords;

        // Array of keywords used with the INCLUDE command.
        private static string[] _includeKeywords = new string[]
        {
            "no_policy_scope",
            "optional",
            "result_variable"
        };

        // Array of keywords used with the INCLUDE_DIRECTORIES command.
        private static string[] _includeDirectoriesKeywords = new string[]
        {
            "after",
            "before",
            "system"
        };

        // Array of keywords used with the INSTALL command.
        private static string[] _installKeywords = new string[]
        {
            "archive",
            "bundle",
            "code",
            "component",
            "configurations",
            "destination",
            "directory",
            "directory_permissions",
            "exclude",
            "export",
            "file",
            "file_permissions",
            "files",
            "files_matching",
            "framework",
            "group_execute",
            "group_read",
            "group_write",
            "library",
            "namelink_only",
            "namelink_skip",
            "namespace",
            "optional",
            "owner_execute",
            "owner_read",
            "owner_write",
            "pattern",
            "permissions",
            "private_header",
            "programs",
            "public_header",
            "rename",
            "resource",
            "runtime",
            "script",
            "setgid",
            "setuid",
            "targets",
            "use_source_permissions",
            "world_execute",
            "world_read",
            "world_write"
        };

        // Array of keywords used with the LIST command.
        private static string[] _listKeywords = new string[]
        {
            "append",
            "find",
            "get",
            "insert",
            "length",
            "remove_at",
            "remove_duplicates",
            "remove_item",
            "reverse",
            "sort"
        };

        // Array of keywords used with the LOAD_CACHE command.
        private static string[] _loadCacheKeywords = new string[]
        {
            "exclude",
            "include_internals",
            "read_with_prefix"
        };

        // Array of keywords used with the LOAD_COMMAND command.
        private static string[] _loadCommandKeywords = new string[]
        {
            "command_name"
        };

        // Array of keywords used with the MARK_AS_ADVANCED command.
        private static string[] _markAsAdvancedKeywords = new string[]
        {
            "clear",
            "force"
        };

        // Array of keywords used with the MESSAGE command.
        private static string[] _messageKeywords = new string[]
        {
            "author_warning",
            "fatal_error",
            "send_error",
            "status",
            "warning"
        };

        // Array of keywords used with the OPTION command.
        private static string[] _optionKeywords = new string[]
        {
            "off",
            "on"
        };

        // Array of keywords used with the SEPARATE_ARGUMENTS command.
        private static string[] _separateArgumentsKeywords = new string[]
        {
            "unix_command",
            "windows_command"
        };

        // Array of keywords used with the SET command.
        private static string[] _setKeywords = new string[]
        {
            "bool",
            "cache",
            "filepath",
            "force",
            "internal",
            "parent_scope",
            "path",
            "string"
        };

        // Array of keywords used with the SET_DIRECTORY_PROPERTIES command.
        private static string[] _setDirectoryPropertiesKeywords = new string[]
        {
            "properties"
        };

        private static string[] _setSourceFilesPropertiesKeywords =
            _setDirectoryPropertiesKeywords;
        private static string[] _setTargetPropertiesKeywords =
            _setDirectoryPropertiesKeywords;
        private static string[] _setTestsPropertiesKeywords =
            _setDirectoryPropertiesKeywords;

        // Array of keywords used with the SET_PROPERTY command.
        private static string[] _setPropertyKeywords = new string[]
        {
            "append",
            "append_string",
            "cache",
            "directory",
            "global",
            "property",
            "source",
            "target",
            "test"
        };

        // Array of keywords used with the SOURCE_GROUP command.
        private static string[] _sourceGroupKeywords = new string[]
        {
            "files",
            "regular_expression"
        };

        // Array of keywords used with the STRING command.
        private static string[] _stringKeywords = new string[]
        {
            "alphabet",
            "ascii",
            "compare",
            "configure",
            "equal",
            "escape_quotes",
            "find",
            "greater",
            "length",
            "less",
            "match",
            "matchall",
            "md5",
            "notequal",
            "random",
            "random_seed",
            "regex",
            "replace",
            "reverse",
            "sha1",
            "sha224",
            "sha256",
            "sha384",
            "sha512",
            "strip",
            "substring",
            "tolower",
            "toupper"
        };

        // Array of keywords used with the TARGET_LINK_LIBRARIES command.
        private static string[] _targetLinkLibrariesKeywords = new string[]
        {
            "link_interface_libraries",
            "link_private",
            "link_public"
        };

        // Array of keywords used with the TRY_COMPILE command.
        private static string[] _tryCompileKeywords = new string[]
        {
            "cmake_flags",
            "compile_definitions",
            "copy_file",
            "output_variable"
        };

        // Array of keywords used with the TRY_RUN command.
        private static string[] _tryRunKeywords = new string[]
        {
            "args",
            "cmake_flags",
            "compile_definitions",
            "compile_output_variable",
            "output_variable",
            "run_output_variable"
        };

        // Array of keywords used with the UNSET command.
        private static string[] _unsetKeywords = new string[]
        {
            "cache"
        };

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
            _enableLanguageKeywords,
            _endForEachKeywords,
            _endIfKeywords,
            _endWhileKeywords,
            _executeProcessKeywords,
            _exportKeywords,
            _fileKeywords,
            _findFileKeywords,
            _findLibraryKeywords,
            _findPackageKeywords,
            _findPathKeywords,
            _findProgramKeywords,
            _forEachKeywords,
            _getDirectoryPropertyKeywords,
            _getFileNameComponentKeywords,
            _getPropertyKeywords,
            _ifKeywords,
            _includeKeywords,
            _includeDirectoriesKeywords,
            _installKeywords,
            _listKeywords,
            _loadCacheKeywords,
            _loadCommandKeywords,
            _markAsAdvancedKeywords,
            _messageKeywords,
            _optionKeywords,
            _separateArgumentsKeywords,
            _setKeywords,
            _setDirectoryPropertiesKeywords,
            _setPropertyKeywords,
            _setSourceFilesPropertiesKeywords,
            _setTargetPropertiesKeywords,
            _setTestsPropertiesKeywords,
            _sourceGroupKeywords,
            _stringKeywords,
            _targetLinkLibrariesKeywords,
            _tryCompileKeywords,
            _tryRunKeywords,
            _unsetKeywords,
            _whileKeywords
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
