// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System;

namespace CMakeTools
{
    /// <summary>
    /// Numeric identifiers for CMake commands.
    /// </summary>
    public enum CMakeCommandId
    {
        Unspecified = 0,
        AddCustomCommand,
        AddCustomTarget,
        AddDefinitions,
        AddDependencies,
        AddExecutable,
        AddLibrary,
        AddSubdirectory,
        AddTest,
        AuxSourceDirectory,
        Break,
        BuildCommand,
        CMakeMinimumRequired,
        CMakePolicy,
        ConfigureFile,
        CreateTestSourcelist,
        DefineProperty,
        Else,
        ElseIf,
        EnableLanguage,
        EnableTesting,
        EndForEach,
        EndFunction,
        EndIf,
        EndMacro,
        EndWhile,
        ExecuteProcess,
        Export,
        File,
        FindFile,
        FindLibrary,
        FindPackage,
        FindPath,
        FindProgram,
        FLTKWrapUi,
        ForEach,
        Function,
        GetCMakeProperty,
        GetDirectoryProperty,
        GetFileNameComponent,
        GetProperty,
        GetSourceFileProperty,
        GetTargetProperty,
        GetTestProperty,
        If,
        Include,
        IncludeDirectories,
        IncludeExternalMsProject,
        IncludeRegularExpression,
        Install,
        LinkDirectories,
        List,
        LoadCache,
        LoadCommand,
        Macro,
        MarkAsAdvanced,
        Math,
        Message,
        Option,
        Project,
        QtWrapCpp,
        QtWrapUi,
        RemoveDefinitions,
        Return,
        SeparateArguments,
        Set,
        SetDirectoryProperties,
        SetProperty,
        SetSourceFilesProperties,
        SetTargetProperties,
        SetTestsProperties,
        SiteName,
        SourceGroup,
        String,
        TargetLinkLibraries,
        TryCompile,
        TryRun,
        Unset,
        VariableWatch,
        While,
        CommandCount
    }

    /// <summary>
    /// Utility class to identify CMake keywords.
    /// </summary>
    public static class CMakeKeywords
    {
        // Array of CMake commands.  These should be in alphabetical order.
        private static string[] _commands = new string[]
        {
            "<unspecified>",            // Dummy keyword for CMakeKeywordId.Unspecified.
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

        // Dummy arrays for commands that have no associated keywords.
        private static string[] _addDefinitionsKeywords = null;
        private static string[] _addDependenciesKeywords = null;
        private static string[] _auxSourceDirectoriesKeywords = null;
        private static string[] _breakKeywords = null;
        private static string[] _enableTestingKeywords = null;
        private static string[] _endFunctionKeywords = null;
        private static string[] _endMacroKeywords = null;
        private static string[] _fltkWrapUiKeywords = null;
        private static string[] _functionKeywords = null;
        private static string[] _getCMakePropertyKeywords = null;
        private static string[] _getSourceFilePropertyKeywords = null;
        private static string[] _getTargetPropertyKeywords = null;
        private static string[] _getTestPropertyKeywords = null;
        private static string[] _includeExternalMsProjectKeywords = null;
        private static string[] _includeRegularExpressionKeywords = null;
        private static string[] _linkDirectoryiesKeywords = null;
        private static string[] _macroKeywords = null;
        private static string[] _mathKeywords = null;
        private static string[] _projectKeywords = null;
        private static string[] _qtWrapCppKeywords = null;
        private static string[] _qtWrapUiKeywords = null;
        private static string[] _removeDefinitionsKeywords = null;
        private static string[] _returnKeywords = null;
        private static string[] _siteNameKeywords = null;
        private static string[] _variableWatchKeywords = null;

        // Arrays of keywords that appear in parentheses after other keywords.
        // The items in this list must be in the same order as the their corresponding
        // keyword identifier in the CMakeKeywordId enumeration.
        private static string[][] _keywordArrays = new string[][]
        {
            null,
            _addCustomCommandKeywords,
            _addCustomTargetKeywords,
            _addDefinitionsKeywords,
            _addDependenciesKeywords,
            _addExecutableKeywords,
            _addLibraryKeywords,
            _addSubdirectoryKeywords,
            _addTestKeywords,
            _auxSourceDirectoriesKeywords,
            _breakKeywords,
            _buildCommandKeywords,
            _cmakeMinimumRequiredKeywords,
            _cmakePolicyKeywords,
            _configureFileKeywords,
            _createTestSourcelistKeywords,
            _definePropertyKeywords,
            _elseKeywords,
            _elseIfKeywords,
            _enableLanguageKeywords,
            _enableTestingKeywords,
            _endForEachKeywords,
            _endFunctionKeywords,
            _endIfKeywords,
            _endMacroKeywords,
            _endWhileKeywords,
            _executeProcessKeywords,
            _exportKeywords,
            _fileKeywords,
            _findFileKeywords,
            _findLibraryKeywords,
            _findPackageKeywords,
            _findPathKeywords,
            _findProgramKeywords,
            _fltkWrapUiKeywords,
            _forEachKeywords,
            _functionKeywords,
            _getCMakePropertyKeywords,
            _getDirectoryPropertyKeywords,
            _getFileNameComponentKeywords,
            _getPropertyKeywords,
            _getSourceFilePropertyKeywords,
            _getTargetPropertyKeywords,
            _getTestPropertyKeywords,
            _ifKeywords,
            _includeKeywords,
            _includeDirectoriesKeywords,
            _includeExternalMsProjectKeywords,
            _includeRegularExpressionKeywords,
            _installKeywords,
            _linkDirectoryiesKeywords,
            _listKeywords,
            _loadCacheKeywords,
            _loadCommandKeywords,
            _macroKeywords,
            _markAsAdvancedKeywords,
            _mathKeywords,
            _messageKeywords,
            _optionKeywords,
            _projectKeywords,
            _qtWrapCppKeywords,
            _qtWrapUiKeywords,
            _removeDefinitionsKeywords,
            _returnKeywords,
            _separateArgumentsKeywords,
            _setKeywords,
            _setDirectoryPropertiesKeywords,
            _setPropertyKeywords,
            _setSourceFilesPropertiesKeywords,
            _setTargetPropertiesKeywords,
            _setTestsPropertiesKeywords,
            _siteNameKeywords,
            _sourceGroupKeywords,
            _stringKeywords,
            _targetLinkLibrariesKeywords,
            _tryCompileKeywords,
            _tryRunKeywords,
            _unsetKeywords,
            _variableWatchKeywords,
            _whileKeywords
        };

        // Array of Booleans indicating whether command should trigger member selection.
        static bool[] _memberSelectionCommands;

        static CMakeKeywords()
        {
            // Initialize the array of Booleans to indicate which commands should trigger
            // member selection.
            _memberSelectionCommands = new bool[_commands.Length];
            foreach (CMakeCommandId id in
                CMakeSubcommandDeclarations.GetMemberSelectionTriggers())
            {
                _memberSelectionCommands[(int)id] = true;
            }
        }

        /// <summary>
        /// Check whether the specified token is a CMake command.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>True if the token is command or false otherwise.</returns>
        public static bool IsCommand(string token)
        {
            int index = Array.BinarySearch(_commands, token.ToLower());
            return index > 0;
        }

        /// <summary>
        /// Check whether the specified command should trigger member selection.
        /// </summary>
        /// <param name="id">Identifier of the command to check.</param>
        /// <returns>
        /// True if member selection should be triggered or false otherwise.
        /// </returns>
        public static bool TriggersMemberSelection(CMakeCommandId id)
        {
            return _memberSelectionCommands[(int)id];
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
        public static bool IsKeyword(CMakeCommandId containingKeyword, string token)
        {
            if (containingKeyword == CMakeCommandId.Unspecified)
            {
                return false;
            }
            string[] keywordArray = _keywordArrays[(int)containingKeyword];
            if (keywordArray == null)
            {
                return false;
            }
            int index = Array.BinarySearch(keywordArray, token.ToLower());
            return index >= 0;
        }

        /// <summary>
        /// Get the command identifier corresponding to a given token string.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>The keyword identifier.</returns>
        public static CMakeCommandId GetCommandId(string token)
        {
            int index = Array.BinarySearch(_commands, token.ToLower());
            if (index < 0)
            {
                return CMakeCommandId.Unspecified;
            }
            return (CMakeCommandId)index;
        }

        /// <summary>
        /// Get the text of a command from its identifier.
        /// </summary>
        /// <param name="id">A command identifier.</param>
        /// <returns>The corresponding keyword text.</returns>
        public static string GetCommandFromId(CMakeCommandId id)
        {
            return _commands[(int)id];
        }

        /// <summary>
        /// Get the array of command-specific keywords for a given command.
        /// </summary>
        /// <param name="id">A command identifier.</param>
        /// <returns>The corresponding array of command-specific keywords.</returns>
        public static string[] GetKeywordsForCommand(CMakeCommandId id)
        {
            string[] keywordArray = _keywordArrays[(int)id];
            if (keywordArray == null)
            {
                return null;
            }
            return (string[])keywordArray.Clone();
        }
    }
}
