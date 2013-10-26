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

namespace CMakeTools
{
    /// <summary>
    /// Numeric identifiers for CMake commands.
    /// </summary>
    public enum CMakeCommandId
    {
        Unspecified = -1,
        AddCompileOptions = 0,
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
        BuildName,
        CMakeHostSystemInformation,
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
        ExecProgram,
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
        MakeDirectory,
        MarkAsAdvanced,
        Math,
        Message,
        Option,
        Project,
        QtWrapCpp,
        QtWrapUi,
        Remove,
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
        TargetCompileDefinitions,
        TargetCompileOptions,
        TargetIncludeDirectories,
        TargetLinkLibraries,
        TryCompile,
        TryRun,
        Unset,
        UseMangledMesa,
        VariableWatch,
        While,
        WriteFile,
        CommandCount
    }

    /// <summary>
    /// Utility class to identify CMake keywords.
    /// </summary>
    public static class CMakeKeywords
    {
        // Array of CMake command identifiers for commands that are deprecated.  These
        // must be in alphabetical order.
        private static readonly CMakeCommandId[] _deprecatedIds = new CMakeCommandId[]
        {
            CMakeCommandId.BuildName,
            CMakeCommandId.ExecProgram,
            CMakeCommandId.MakeDirectory,
            CMakeCommandId.Remove,
            CMakeCommandId.WriteFile
        };

        // Array of CMake commands.  These should be in alphabetical order.
        private static readonly string[] _commands = new string[]
        {
            "add_compile_options",
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
            "build_name",
            "cmake_host_system_information",
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
            "exec_program",
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
            "make_directory",
            "mark_as_advanced",
            "math",
            "message",
            "option",
            "project",
            "qt_wrap_cpp",
            "qt_wrap_ui",
            "remove",
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
            "target_compile_definitions",
            "target_compile_options",
            "target_include_directories",
            "target_link_libraries",
            "try_compile",
            "try_run",
            "unset",
            "use_mangled_mesa",
            "variable_watch",
            "while",
            "write_file"
        };

        // Array of keywords used with the ADD_CUSTOM_COMMAND command.
        private static readonly string[] _addCustomCommandKeywords = new string[]
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
        private static readonly string[] _addCustomTargetKeywords = new string[]
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
        private static readonly string[] _addExecutableKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "MACOSX_BUNDLE",
            "WIN32"
        };

        // Array of keywords used with the ADD_LIBRARY command.
        private static readonly string[] _addLibraryKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "IMPORTED",
            "MODULE",
            "SHARED",
            "STATIC",
            "UNKNOWN"
        };

        // Array of keywords used with the ADD_SUBDIRECTORY command.
        private static readonly string[] _addSubdirectoryKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL"
        };

        // Array of keywords used with the ADD_TEST command.
        private static readonly string[] _addTestKeywords = new string[]
        {
            "COMMAND",
            "CONFIGURATIONS",
            "NAME",
            "WORKING_DIRECTORY"
        };

        // Array of keywords used with the BUILD_COMMAND command.
        private static readonly string[] _buildCommandKeywords = new string[]
        {
            "CONFIGURATION",
            "PROJECT_NAME",
            "TARGET"
        };

        // Array of keywords used with the CMAKE_HOST_SYSTEM_INFORMATION command.
        private static readonly string[] _cmakeHostSystemInformationKeywords =
            new string[]
        {
            "QUERY",
            "RESULT"
        };

        // Array of keywords used with the CMAKE_MINIMUM_REQUIRED command.
        private static readonly string[] _cmakeMinimumRequiredKeywords = new string[]
        {
            "FATAL_ERROR",
            "VERSION"
        };

        // Array of keywords used with the CMAKE_POLICY command.
        private static readonly string[] _cmakePolicyKeywords = new string[]
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
        private static readonly string[] _configureFileKeywords = new string[]
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
        private static readonly string[] _createTestSourcelistKeywords = new string[]
        {
            "EXTRA_INCLUDE",
            "FUNCTION"
        };

        // Array of keywords used with the DEFINE_PROPERTY command.
        private static readonly string[] _definePropertyKeywords = new string[]
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
        private static readonly string[] _enableLanguageKeywords = new string[]
        {
            "OPTIONAL"
        };

        // Array of keywords used with the EXEC_PROGRAM command.
        private static readonly string[] _execProgramKeywords = new string[]
        {
            "ARGS",
            "OUTPUT_VARIABLE",
            "RETURN_VALUE"
        };

        // Array of keywords used with the EXECUTE_PROCESS command.
        private static readonly string[] _executeProcessKeywords = new string[]
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
        private static readonly string[] _exportKeywords = new string[]
        {
            "APPEND",
            "FILE",
            "NAMESPACE",
            "PACKAGE",
            "TARGETS"
        };

        // Array of keywords used with the FILE command.
        private static readonly string[] _fileKeywords = new string[]
        {
            "APPEND",
            "CONDITION",
            "CONTENT",
            "COPY",
            "DESTINATION",
            "DIRECTORY_PERMISSIONS",
            "DOWNLOAD",
            "EXCLUDE",
            "EXPECTED_MD5",
            "FILE_PERMISSIONS",
            "FILES_MATCHING",
            "FOLLOW_SYMLINKS",
            "GENERATE",
            "GLOB",
            "GLOB_RECURSE",
            "GROUP_EXECUTE",
            "GROUP_READ",
            "GROUP_WRITE",
            "HEX",
            "INACTIVITY_TIMEOUT",
            "INPUT",
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
            "OUTPUT",
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
            "TIMESTAMP",
            "TO_CMAKE_PATH",
            "TO_NATIVE_PATH",
            "UPLOAD",
            "USE_SOURCE_PERMISSIONS",
            "UTC",
            "WORLD_EXECUTE",
            "WORLD_READ",
            "WORLD_WRITE",
            "WRITE"
        };

        // Array of keywords used with the FIND_FILE command.
        private static readonly string[] _findFileKeywords = new string[]
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

        private static readonly string[] _findLibraryKeywords = _findFileKeywords;
        private static readonly string[] _findPathKeywords = _findFileKeywords;
        private static readonly string[] _findProgramKeywords = _findFileKeywords;

        // Array of keywords used with the FIND_PACKAGE command.
        private static readonly string[] _findPackageKeywords = new string[]
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
        private static readonly string[] _forEachKeywords = new string[]
        {
            "IN",
            "ITEMS",
            "LISTS",
            "RANGE"
        };

        private static readonly string[] _endForEachKeywords = _forEachKeywords;

        // Array of keywords used with the GET_DIRECTORY_PROPERTY command.
        private static readonly string[] _getDirectoryPropertyKeywords = new string[]
        {
            "DEFINITION",
            "DIRECTORY"
        };
        
        // Array of keywords used with the GET_FILENAME_COMPONENT command.
        private static readonly string[] _getFileNameComponentKeywords = new string[]
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
        private static readonly string[] _getPropertyKeywords = new string[]
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
        private static readonly string[] _ifKeywords = new string[]
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

        private static readonly string[] _endIfKeywords = _ifKeywords;
        private static readonly string[] _elseKeywords = _ifKeywords;
        private static readonly string[] _elseIfKeywords = _ifKeywords;
        private static readonly string[] _whileKeywords = _ifKeywords;
        private static readonly string[] _endWhileKeywords = _ifKeywords;

        // Array of keywords used with the INCLUDE command.
        private static readonly string[] _includeKeywords = new string[]
        {
            "NO_POLICY_SCOPE",
            "OPTIONAL",
            "RESULT_VARIABLE"
        };

        // Array of keywords used with the INCLUDE_DIRECTORIES command.
        private static readonly string[] _includeDirectoriesKeywords = new string[]
        {
            "AFTER",
            "BEFORE",
            "SYSTEM"
        };

        // Array of keywords used with the INCLUDE_EXTERNAL_MSPROJECT command.
        private static readonly string[] _includeExternalMsProjectKeywords = new string[]
        {
            "GUID",
            "PLATFORM",
            "TYPE"
        };

        // Array of keywords used with the INSTALL command.
        private static readonly string[] _installKeywords = new string[]
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
        private static readonly string[] _listKeywords = new string[]
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
        private static readonly string[] _loadCacheKeywords = new string[]
        {
            "EXCLUDE",
            "INCLUDE_INTERNALS",
            "READ_WITH_PREFIX"
        };

        // Array of keywords used with the LOAD_COMMAND command.
        private static readonly string[] _loadCommandKeywords = new string[]
        {
            "COMMAND_NAME"
        };

        // Array of keywords used with the MARK_AS_ADVANCED command.
        private static readonly string[] _markAsAdvancedKeywords = new string[]
        {
            "CLEAR",
            "FORCE"
        };

        // Array of keywords used with the MESSAGE command.
        private static readonly string[] _messageKeywords = new string[]
        {
            "AUTHOR_WARNING",
            "FATAL_ERROR",
            "SEND_ERROR",
            "STATUS",
            "WARNING"
        };

        // Array of keywords used with the OPTION command.
        private static readonly string[] _optionKeywords = new string[]
        {
            "OFF",
            "ON"
        };

        // Array of keywords used with the SEPARATE_ARGUMENTS command.
        private static readonly string[] _separateArgumentsKeywords = new string[]
        {
            "UNIX_COMMAND",
            "WINDOWS_COMMAND"
        };

        // Array of keywords used with the SET command.
        private static readonly string[] _setKeywords = new string[]
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
        private static readonly string[] _setDirectoryPropertiesKeywords = new string[]
        {
            "PROPERTIES"
        };

        private static readonly string[] _setSourceFilesPropertiesKeywords =
            _setDirectoryPropertiesKeywords;
        private static readonly string[] _setTargetPropertiesKeywords =
            _setDirectoryPropertiesKeywords;
        private static readonly string[] _setTestsPropertiesKeywords =
            _setDirectoryPropertiesKeywords;

        // Array of keywords used with the SET_PROPERTY command.
        private static readonly string[] _setPropertyKeywords = new string[]
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
        private static readonly string[] _sourceGroupKeywords = new string[]
        {
            "FILES",
            "REGULAR_EXPRESSION"
        };

        // Array of keywords used with the STRING command.
        private static readonly string[] _stringKeywords = new string[]
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
            "MAKE_C_IDENTIFIER",
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
            "TIMESTAMP",
            "TOLOWER",
            "TOUPPER",
            "UTC"
        };

        // Array of keywords used with the TARGET_COMPILE_DEFINITIONS command.
        private static readonly string[] _targetCompileDefinitionsKeywords = new string[]
        {
            "INTERFACE",
            "PRIVATE",
            "PUBLIC"
        };

        // Array of keywords used with the TARGET_COMPILE_OPTIONS command.
        private static readonly string[] _targetCompileOptionsKeywords = new string[]
        {
            "BEFORE",
            "INTERFACE",
            "PRIVATE",
            "PUBLIC"
        };

        private static readonly string[] _targetIncludeDirectoriesKeywords =
            _targetCompileOptionsKeywords;

        // Array of keywords used with the TARGET_LINK_LIBRARIES command.
        private static readonly string[] _targetLinkLibrariesKeywords = new string[]
        {
            "LINK_INTERFACE_LIBRARIES",
            "LINK_PRIVATE",
            "LINK_PUBLIC"
        };

        // Array of keywords used with the TRY_COMPILE command.
        private static readonly string[] _tryCompileKeywords = new string[]
        {
            "CMAKE_FLAGS",
            "COMPILE_DEFINITIONS",
            "COPY_FILE",
            "COPY_FILE_ERROR",
            "LINK_LIBRARIES",
            "OUTPUT_VARIABLE",
            "SOURCES"
        };

        // Array of keywords used with the TRY_RUN command.
        private static readonly string[] _tryRunKeywords = new string[]
        {
            "ARGS",
            "CMAKE_FLAGS",
            "COMPILE_DEFINITIONS",
            "COMPILE_OUTPUT_VARIABLE",
            "OUTPUT_VARIABLE",
            "RUN_OUTPUT_VARIABLE"
        };

        // Array of keywords used with the UNSET command.
        private static readonly string[] _unsetKeywords = new string[]
        {
            "CACHE"
        };

        // Array of keywords used with the WRITE_FILE command.
        private static readonly string[] _writeFileKeywords = new string[]
        {
            "APPEND"
        };

        // Dummy arrays for commands that have no associated keywords.
        private static readonly string[] _addCompileOptionsKeywords = null;
        private static readonly string[] _addDefinitionsKeywords = null;
        private static readonly string[] _addDependenciesKeywords = null;
        private static readonly string[] _auxSourceDirectoriesKeywords = null;
        private static readonly string[] _breakKeywords = null;
        private static readonly string[] _buildNameKeywords = null;
        private static readonly string[] _enableTestingKeywords = null;
        private static readonly string[] _endFunctionKeywords = null;
        private static readonly string[] _endMacroKeywords = null;
        private static readonly string[] _fltkWrapUiKeywords = null;
        private static readonly string[] _functionKeywords = null;
        private static readonly string[] _getCMakePropertyKeywords = null;
        private static readonly string[] _getSourceFilePropertyKeywords = null;
        private static readonly string[] _getTargetPropertyKeywords = null;
        private static readonly string[] _getTestPropertyKeywords = null;
        private static readonly string[] _includeRegularExpressionKeywords = null;
        private static readonly string[] _linkDirectoryiesKeywords = null;
        private static readonly string[] _macroKeywords = null;
        private static readonly string[] _makeDirectoryKeywords = null;
        private static readonly string[] _mathKeywords = null;
        private static readonly string[] _projectKeywords = null;
        private static readonly string[] _qtWrapCppKeywords = null;
        private static readonly string[] _qtWrapUiKeywords = null;
        private static readonly string[] _removeKeywords = null;
        private static readonly string[] _removeDefinitionsKeywords = null;
        private static readonly string[] _returnKeywords = null;
        private static readonly string[] _siteNameKeywords = null;
        private static readonly string[] _useMangledMesaKeywords = null;
        private static readonly string[] _variableWatchKeywords = null;

        // Arrays of keywords that appear in parentheses after other keywords.
        // The items in this list must be in the same order as the their corresponding
        // keyword identifier in the CMakeKeywordId enumeration.
        private static readonly string[][] _keywordArrays = new string[][]
        {
            _addCompileOptionsKeywords,
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
            _buildNameKeywords,
            _cmakeHostSystemInformationKeywords,
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
            _execProgramKeywords,
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
            _makeDirectoryKeywords,
            _markAsAdvancedKeywords,
            _mathKeywords,
            _messageKeywords,
            _optionKeywords,
            _projectKeywords,
            _qtWrapCppKeywords,
            _qtWrapUiKeywords,
            _removeKeywords,
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
            _targetCompileDefinitionsKeywords,
            _targetCompileOptionsKeywords,
            _targetIncludeDirectoriesKeywords,
            _targetLinkLibrariesKeywords,
            _tryCompileKeywords,
            _tryRunKeywords,
            _unsetKeywords,
            _useMangledMesaKeywords,
            _variableWatchKeywords,
            _whileKeywords,
            _writeFileKeywords
        };

        // Array of Booleans indicating whether a command should trigger member
        // selection after the opening parenthesis.
        private static readonly bool[] _memberSelectionCommands;

        // Array of Booleans indicating whether a command should trigger member
        // selection after whitespace.
        private static readonly bool[] _memberSelectionWSCommands;

        static CMakeKeywords()
        {
            // Initialize the arrays of Booleans to indicate which commands should
            // trigger member selection.
            _memberSelectionCommands = new bool[_commands.Length];
            foreach (CMakeCommandId id in
                CMakeDeclarationsFactory.GetMemberSelectionTriggers())
            {
                _memberSelectionCommands[(int)id] = true;
            }
            _memberSelectionWSCommands = new bool[_commands.Length];
            foreach (CMakeCommandId id in
                CMakeDeclarationsFactory.GetWSMemberSelectionTriggers())
            {
                _memberSelectionWSCommands[(int)id] = true;
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
        /// <param name="includeDeprecated">
        /// Boolean value indicating whether deprecated commands should be included.
        /// </param>
        /// <returns>A collection of all CMake commands.</returns>
        public static IEnumerable<string> GetAllCommands(bool includeDeprecated)
        {
            if (!includeDeprecated)
            {
                List<string> commands = new List<string>(_commands);
                foreach(CMakeCommandId id in _deprecatedIds.Reverse())
                {
                    commands.RemoveAt((int)id);
                }
                return commands;
            }
            return _commands;
        }

        /// <summary>
        /// Check if the specified command is deprecated.
        /// </summary>
        /// <param name="id">Identifier of the command to check.</param>
        /// <returns>True if the command is deprecated or false otherwise.</returns>
        public static bool IsDeprecated(CMakeCommandId id)
        {
            return _deprecatedIds.Contains(id);
        }
    }
}
