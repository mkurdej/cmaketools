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

namespace CMakeTools
{
    /// <summary>
    /// Numeric identifiers for CMake commands.
    /// </summary>
    public enum CMakeCommandId
    {
        Unspecified = -1,
        AddCustomCommand = 0,
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
            "APPEND",
            "ARGS",
            "COMMAND",
            "COMMENT",
            "DEPENDS",
            "IMPLICIT_DEPENDS",
            "MAIN_DEPENDENCY",
            "OUTPUT",
            "POST_BUILD",
            "PRE_BUILD",
            "PRE_LINK",
            "TARGET",
            "VERBATIM",
            "WORKING_DIRECTORY"
        };

        // Array of keywords used with the ADD_CUSTOM_TARGET command.
        private static string[] _addCustomTargetKeywords = new string[]
        {
            "ALL",
            "COMMAND",
            "COMMENT",
            "DEPENDS",
            "SOURCES",
            "VERBATIM",
            "WORKING_DIRECTORY"
        };

        // Array of keywords used with the ADD_EXECUTABLE command.
        private static string[] _addExecutableKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "MACOSX_BUNDLE",
            "WIN32"
        };

        // Array of keywords used with the ADD_LIBRARY command.
        private static string[] _addLibraryKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "IMPORTED",
            "MODULE",
            "SHARED",
            "STATIC",
            "UNKNOWN"
        };

        // Array of keywords used with the ADD_SUBDIRECTORY command.
        private static string[] _addSubdirectoryKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL"
        };

        // Array of keywords used with the ADD_TEST command.
        private static string[] _addTestKeywords = new string[]
        {
            "COMMAND",
            "CONFIGURATIONS",
            "NAME",
            "WORKING_DIRECTORY"
        };

        // Array of keywords used with the BUILD_COMMAND command.
        private static string[] _buildCommandKeywords = new string[]
        {
            "CONFIGURATION",
            "PROJECT_NAME",
            "TARGET"
        };

        // Array of keywords used with the CMAKE_MINIMUM_REQUIRED command.
        private static string[] _cmakeMinimumRequiredKeywords = new string[]
        {
            "FATAL_ERROR",
            "VERSION"
        };

        // Array of keywords used with the CMAKE_POLICY command.
        private static string[] _cmakePolicyKeywords = new string[]
        {
            "GET",
            "NEW",
            "OLD",
            "POP",
            "PUSH",
            "SET",
            "VERSION"
        };

        // Array of keywords used with the CONFIGURE_FILE command.
        private static string[] _configureFileKeywords = new string[]
        {
            "COPYONLY",
            "CRLF",
            "DOS",
            "ESCAPE_QUOTES",
            "LF",
            "NEWLINE_STYLE",
            "UNIX",
            "WIN32"
        };

        // Array of keywords used with the CREATE_TEST_SOURCELIST command.
        private static string[] _createTestSourcelistKeywords = new string[]
        {
            "EXTRA_INCLUDE",
            "FUNCTION"
        };

        // Array of keywords used with the DEFINE_PROPERTY command.
        private static string[] _definePropertyKeywords = new string[]
        {
            "BRIEF_DOCS",
            "CACHED_VARIABLE",
            "DIRECTORY",
            "FULL_DOCS",
            "GLOBAL",
            "INHERITED",
            "PROPERTY",
            "SOURCE",
            "TARGET",
            "TEST",
            "VARIABLE"
        };

        // Array of keywords used with the ENABLE_LANGUAGE command.
        private static string[] _enableLanguageKeywords = new string[]
        {
            "OPTIONAL"
        };

        // Array of keywords used with the EXECUTE_PROCESS command.
        private static string[] _executeProcessKeywords = new string[]
        {
            "COMMAND",
            "ERROR_FILE",
            "ERROR_QUIET",
            "ERROR_STRIP_TRAILING_WHITESPACE",
            "ERROR_VARIABLE",
            "INPUT_FILE",
            "OUTPUT_FILE",
            "OUTPUT_QUIET",
            "OUTPUT_STRIP_TRAILING_WHITESPACE",
            "OUTPUT_VARIABLE",
            "RESULT_VARIABLE",
            "TIMEOUT",
            "WORKING_DIRECTORY"
        };

        // Array of keywords used with the EXPORT command.
        private static string[] _exportKeywords = new string[]
        {
            "APPEND",
            "FILE",
            "NAMESPACE",
            "PACKAGE",
            "TARGETS"
        };

        // Array of keywords used with the FILE command.
        private static string[] _fileKeywords = new string[]
        {
            "APPEND",
            "COPY",
            "DESTINATION",
            "DIRECTORY_PERMISSIONS",
            "DOWNLOAD",
            "EXCLUDE",
            "EXPECTED_MD5",
            "FILE_PERMISSIONS",
            "FILES_MATCHING",
            "FOLLOW_SYMLINKS",
            "GLOB",
            "GLOB_RECURSE",
            "GROUP_EXECUTE",
            "GROUP_READ",
            "GROUP_WRITE",
            "HEX",
            "INACTIVITY_TIMEOUT",
            "INSTALL",
            "LENGTH_MAXIMUM",
            "LENGTH_MINIMUM",
            "LIMIT",
            "LIMIT_COUNT",
            "LIMIT_INPUT",
            "LIMIT_OUTPUT",
            "LOG",
            "MAKE_DIRECTORY",
            "MD5",
            "NEWLINE_CONSUME",
            "NO_HEX_CONVERSION",
            "NO_SOURCE_PERMISSIONS",
            "OFFSET",
            "OWNER_EXECUTE",
            "OWNER_READ",
            "OWNER_WRITE",
            "PATTERN",
            "PERMISSIONS",
            "READ",
            "REGEX",
            "RELATIVE",
            "RELATIVE_PATH",
            "REMOVE",
            "REMOVE_RECURSE",
            "RENAME",
            "SETGID",
            "SETUID",
            "SHA1",
            "SHA224",
            "SHA256",
            "SHA384",
            "SHA512",
            "SHOW_PROGRESS",
            "STATUS",
            "STRINGS",
            "TIMEOUT",
            "TO_CMAKE_PATH",
            "TO_NATIVE_PATH",
            "UPLOAD",
            "USE_SOURCE_PERMISSIONS",
            "WORLD_EXECUTE",
            "WORLD_READ",
            "WORLD_WRITE",
            "WRITE"
        };

        // Array of keywords used with the FIND_FILE command.
        private static string[] _findFileKeywords = new string[]
        {
            "CMAKE_FIND_ROOT_PATH_BOTH",
            "DOC",
            "ENV",
            "HINTS",
            "NAMES",
            "NO_CMAKE_ENVIRONMENT_PATH",
            "NO_CMAKE_FIND_ROOT_PATH",
            "NO_CMAKE_PATH",
            "NO_CMAKE_SYSTEM_PATH",
            "NO_DEFAULT_PATH",
            "NO_SYSTEM_ENVIRONMENT_PATH",
            "ONLY_CMAKE_FIND_ROOT_PATH",
            "PATH_SUFFIXES",
            "PATHS"
        };

        private static string[] _findLibraryKeywords = _findFileKeywords;
        private static string[] _findPathKeywords = _findFileKeywords;
        private static string[] _findProgramKeywords = _findFileKeywords;

        // Array of keywords used with the FIND_PACKAGE command.
        private static string[] _findPackageKeywords = new string[]
        {
            "CMAKE_FIND_ROOT_PATH_BOTH",
            "COMPONENTS",
            "CONFIGS",
            "EXACT",
            "HINTS",
            "NAMES",
            "NO_CMAKE_BUILDS_PATH",
            "NO_CMAKE_ENVIRONMENT_PATH",
            "NO_CMAKE_FIND_ROOT_PATH",
            "NO_CMAKE_PACKAGE_REGISTRY",
            "NO_CMAKE_PATH",
            "NO_CMAKE_SYSTEM_PACKAGE_REGISTRY",
            "NO_CMAKE_SYSTEM_PATH",
            "NO_DEFAULT_PATH",
            "NO_MODULE",
            "NO_POLICY_SCOPE",
            "NO_SYSTEM_ENVIRONMENT_PATH",
            "ONLY_CMAKE_FIND_ROOT_PATH",
            "PATH_SUFFIXES",
            "PATHS",
            "QUIET",
            "REQUIRED"
        };

        // Array of keywords used with the FOREACH command.
        private static string[] _forEachKeywords = new string[]
        {
            "IN",
            "ITEMS",
            "LISTS",
            "RANGE"
        };

        private static string[] _endForEachKeywords = _forEachKeywords;

        // Array of keywords used with the GET_DIRECTORY_PROPERTY command.
        private static string[] _getDirectoryPropertyKeywords = new string[]
        {
            "DEFINITION",
            "DIRECTORY"
        };
        
        // Array of keywords used with the GET_FILENAME_COMPONENT command.
        private static string[] _getFileNameComponentKeywords = new string[]
        {
            "ABSOLUTE",
            "CACHE",
            "EXT",
            "NAME",
            "NAME_WE",
            "PATH",
            "PROGRAM",
            "PROGRAM_ARGS",
            "REALPATH"
        };

        // Array of keywords used with the GET_PROPERTY command.
        private static string[] _getPropertyKeywords = new string[]
        {
            "BRIEF_DOCS",
            "CACHE",
            "DEFINED",
            "DIRECTORY",
            "FULL_DOCS",
            "GLOBAL",
            "PROPERTY",
            "SET",
            "SOURCE",
            "TARGET",
            "TEST",
            "VARIABLE"
        };

        // Array of keywords used with the IF command.
        private static string[] _ifKeywords = new string[]
        {
            "AND",
            "COMMAND",
            "DEFINED",
            "EQUAL",
            "EXISTS",
            "GREATER",
            "IS_ABSOLUTE",
            "IS_DIRECTORY",
            "IS_NEWER_THAN",
            "IS_SYMLINK",
            "LESS",
            "MATCHES",
            "NOT",
            "OR",
            "POLICY",
            "STREQUAL",
            "STRGREATER",
            "STRLESS",
            "TARGET",
            "VERSION_EQUAL",
            "VERSION_GREATER",
            "VERSION_LESS"
        };

        private static string[] _endIfKeywords = _ifKeywords;
        private static string[] _elseKeywords = _ifKeywords;
        private static string[] _elseIfKeywords = _ifKeywords;
        private static string[] _whileKeywords = _ifKeywords;
        private static string[] _endWhileKeywords = _ifKeywords;

        // Array of keywords used with the INCLUDE command.
        private static string[] _includeKeywords = new string[]
        {
            "NO_POLICY_SCOPE",
            "OPTIONAL",
            "RESULT_VARIABLE"
        };

        // Array of keywords used with the INCLUDE_DIRECTORIES command.
        private static string[] _includeDirectoriesKeywords = new string[]
        {
            "AFTER",
            "BEFORE",
            "SYSTEM"
        };

        // Array of keywords used with the INSTALL command.
        private static string[] _installKeywords = new string[]
        {
            "ARCHIVE",
            "BUNDLE",
            "CODE",
            "COMPONENT",
            "CONFIGURATIONS",
            "DESTINATION",
            "DIRECTORY",
            "DIRECTORY_PERMISSIONS",
            "EXCLUDE",
            "EXPORT",
            "FILE",
            "FILE_PERMISSIONS",
            "FILES",
            "FILES_MATCHING",
            "FRAMEWORK",
            "GROUP_EXECUTE",
            "GROUP_READ",
            "GROUP_WRITE",
            "LIBRARY",
            "NAMELINK_ONLY",
            "NAMELINK_SKIP",
            "NAMESPACE",
            "OPTIONAL",
            "OWNER_EXECUTE",
            "OWNER_READ",
            "OWNER_WRITE",
            "PATTERN",
            "PERMISSIONS",
            "PRIVATE_HEADER",
            "PROGRAMS",
            "PUBLIC_HEADER",
            "RENAME",
            "RESOURCE",
            "RUNTIME",
            "SCRIPT",
            "SETGID",
            "SETUID",
            "TARGETS",
            "USE_SOURCE_PERMISSIONS",
            "WORLD_EXECUTE",
            "WORLD_READ",
            "WORLD_WRITE"
        };

        // Array of keywords used with the LIST command.
        private static string[] _listKeywords = new string[]
        {
            "APPEND",
            "FIND",
            "GET",
            "INSERT",
            "LENGTH",
            "REMOVE_AT",
            "REMOVE_DUPLICATES",
            "REMOVE_ITEM",
            "REVERSE",
            "SORT"
        };

        // Array of keywords used with the LOAD_CACHE command.
        private static string[] _loadCacheKeywords = new string[]
        {
            "EXCLUDE",
            "INCLUDE_INTERNALS",
            "READ_WITH_PREFIX"
        };

        // Array of keywords used with the LOAD_COMMAND command.
        private static string[] _loadCommandKeywords = new string[]
        {
            "COMMAND_NAME"
        };

        // Array of keywords used with the MARK_AS_ADVANCED command.
        private static string[] _markAsAdvancedKeywords = new string[]
        {
            "CLEAR",
            "FORCE"
        };

        // Array of keywords used with the MESSAGE command.
        private static string[] _messageKeywords = new string[]
        {
            "AUTHOR_WARNING",
            "FATAL_ERROR",
            "SEND_ERROR",
            "STATUS",
            "WARNING"
        };

        // Array of keywords used with the OPTION command.
        private static string[] _optionKeywords = new string[]
        {
            "OFF",
            "ON"
        };

        // Array of keywords used with the SEPARATE_ARGUMENTS command.
        private static string[] _separateArgumentsKeywords = new string[]
        {
            "UNIX_COMMAND",
            "WINDOWS_COMMAND"
        };

        // Array of keywords used with the SET command.
        private static string[] _setKeywords = new string[]
        {
            "BOOL",
            "CACHE",
            "FILEPATH",
            "FORCE",
            "INTERNAL",
            "PARENT_SCOPE",
            "PATH",
            "STRING"
        };

        // Array of keywords used with the SET_DIRECTORY_PROPERTIES command.
        private static string[] _setDirectoryPropertiesKeywords = new string[]
        {
            "PROPERTIES"
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
            "APPEND",
            "APPEND_STRING",
            "CACHE",
            "DIRECTORY",
            "GLOBAL",
            "PROPERTY",
            "SOURCE",
            "TARGET",
            "TEST"
        };

        // Array of keywords used with the SOURCE_GROUP command.
        private static string[] _sourceGroupKeywords = new string[]
        {
            "FILES",
            "REGULAR_EXPRESSION"
        };

        // Array of keywords used with the STRING command.
        private static string[] _stringKeywords = new string[]
        {
            "ALPHABET",
            "ASCII",
            "COMPARE",
            "CONFIGURE",
            "EQUAL",
            "ESCAPE_QUOTES",
            "FIND",
            "GREATER",
            "LENGTH",
            "LESS",
            "MATCH",
            "MATCHALL",
            "MD5",
            "NOTEQUAL",
            "RANDOM",
            "RANDOM_SEED",
            "REGEX",
            "REPLACE",
            "REVERSE",
            "SHA1",
            "SHA224",
            "SHA256",
            "SHA384",
            "SHA512",
            "STRIP",
            "SUBSTRING",
            "TOLOWER",
            "TOUPPER"
        };

        // Array of keywords used with the TARGET_LINK_LIBRARIES command.
        private static string[] _targetLinkLibrariesKeywords = new string[]
        {
            "LINK_INTERFACE_LIBRARIES",
            "LINK_PRIVATE",
            "LINK_PUBLIC"
        };

        // Array of keywords used with the TRY_COMPILE command.
        private static string[] _tryCompileKeywords = new string[]
        {
            "CMAKE_FLAGS",
            "COMPILE_DEFINITIONS",
            "COPY_FILE",
            "OUTPUT_VARIABLE"
        };

        // Array of keywords used with the TRY_RUN command.
        private static string[] _tryRunKeywords = new string[]
        {
            "ARGS",
            "CMAKE_FLAGS",
            "COMPILE_DEFINITIONS",
            "COMPILE_OUTPUT_VARIABLE",
            "OUTPUT_VARIABLE",
            "RUN_OUTPUT_VARIABLE"
        };

        // Array of keywords used with the UNSET command.
        private static string[] _unsetKeywords = new string[]
        {
            "CACHE"
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

        // Array of Booleans indicating whether a command should trigger member
        // selection after the opening parenthesis.
        private static bool[] _memberSelectionCommands;

        // Array of Booleans indicating whether a command should trigger member
        // selection after whitespace.
        private static bool[] _memberSelectionWSCommands;

        static CMakeKeywords()
        {
            // Initialize the arrays of Booleans to indicate which commands should
            // trigger member selection.
            _memberSelectionCommands = new bool[_commands.Length];
            foreach (CMakeCommandId id in
                CMakeSubcommandMethods.GetMemberSelectionTriggers())
            {
                _memberSelectionCommands[(int)id] = true;
            }

            // The INCLUDE, FIND_PACKAGE, ADD_SUBDIRECTORY, and ENABLE_LANGUAGE commands
            // don't have subcommands but should still trigger member selection.
            _memberSelectionCommands[(int)CMakeCommandId.Include] = true;
            _memberSelectionCommands[(int)CMakeCommandId.FindPackage] = true;
            _memberSelectionCommands[(int)CMakeCommandId.AddSubdirectory] = true;
            _memberSelectionCommands[(int)CMakeCommandId.EnableLanguage] = true;

            // These commands should trigger member selection on whitespace.
            _memberSelectionWSCommands = new bool[_commands.Length];
            _memberSelectionWSCommands[(int)CMakeCommandId.AddExecutable] = true;
            _memberSelectionWSCommands[(int)CMakeCommandId.AddLibrary] = true;
        }

        /// <summary>
        /// Check whether the specified token is a CMake command.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>True if the token is command or false otherwise.</returns>
        public static bool IsCommand(string token)
        {
            int index = Array.BinarySearch(_commands, token.ToLower());
            return index >= 0;
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
            if (id == CMakeCommandId.Unspecified)
            {
                return false;
            }
            return _memberSelectionCommands[(int)id];
        }

        /// <summary>
        /// Check whether the specified command should trigger member selection in
        /// response to whitespace.
        /// </summary>
        /// <param name="id">Identifier of the command to check.</param>
        /// <returns>
        /// True if member selection should be triggered of false otherwise.
        /// </returns>
        public static bool TriggersMemberSelectionOnWhiteSpace(CMakeCommandId id)
        {
            if (id == CMakeCommandId.Unspecified)
            {
                return false;
            }
            return _memberSelectionWSCommands[(int)id];
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
            int index = Array.BinarySearch(keywordArray, token);
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
            if (id == CMakeCommandId.Unspecified)
            {
                return null;
            }
            string[] keywordArray = _keywordArrays[(int)id];
            if (keywordArray == null)
            {
                return null;
            }
            return (string[])keywordArray.Clone();
        }

        /// <summary>
        /// Get the names of all CMake commands.
        /// </summary>
        /// <returns>An array of all CMake commands.</returns>
        public static string[] GetAllCommands()
        {
            return _commands;
        }
    }
}
