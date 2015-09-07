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

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Methods object containing parameter information for a CMake command.
    /// </summary>
    public class CMakeMethods : Methods
    {
        // Parameters to the ADD_DEPENDENCIES command.
        private static readonly string[] _addDependenciesParams = new string[]
        {
            "target_name",
            "depend_target1 depend_target2 ..."
        };

        // Parameters to the ADD_EXECUTABLE command.
        private static readonly string[] _addExecutableParams = new string[]
        {
            "name",
            "source1 source2 ..."
        };

        // Parameters to the ADD_SUBDIRECTORY command.
        private static readonly string[] _addSubdirectoryParams = new string[]
        {
            "source_dir",
            "[binary_dir]"
        };

        // Parameters to the ADD_TEST command.
        private static readonly string[] _addTestParams = new string[]
        {
            "test_name",
            "exe_name",
            "arg1 arg2 ..."
        };

        // Parameters to the AUX_SOURCE_DIRECTORY command.
        private static readonly string[] _auxSourceDirectoryParams = new string[]
        {
            "dir",
            "variable"
        };

        // Parameters to the CONFIGURE_FILE command.
        private static readonly string[] _configureFileParams = new string[]
        {
            "input",
            "output"
        };

        // Parameters to the ENABLE_LANGUAGE command.
        private static readonly string[] _enableLanguageParams =
            new string[] { "language_name" };

        // Parameters to the ENDFUNCTION command.
        private static readonly string[] _endFunctionParams = new string[] { "[name]" };

        // Parameters to the ENDMACRO command.
        private static readonly string[] _endMacroParams = _endFunctionParams;

        // Parameters to the EXPORT_LIBRARY_DEPENDENCIES command.
        private static readonly string[] _exportLibraryDependenciesParams = new string[]
        {
            "file"
        };

        // Parameters to the FIND_FILE command.
        private static readonly string[] _findFileParams = new string[]
        {
            "variable",
            "name",
            "[path1 path2 ...]"
        };

        private static readonly string[] _findPathParams = _findFileParams;
        private static readonly string[] _findProgramParams = _findFileParams;

        // Parameters to the FLTK_WRAP_UI command.
        private static readonly string[] _fltkWrapUiParams = new string[]
        {
            "resulting_library_name",
            "source1 source2 ..."
        };

        // Parameters to the FOREACH command.
        private static readonly string[] _forEachParams = new string[]
        {
            "loop_variable",
            "arg1 arg2 ..."
        };

        // Parameters to the FUNCTION command.
        private static readonly string[] _functionParams = new string[]
        {
            "name",
            "arg1 arg2 ..."
        };

        // Parameters to the GET_CMAKE_PROPERTY command.
        private static readonly string[] _getCMakePropertyParams = new string[]
        {
            "variable",
            "property"
        };

        // Parameters to the GET_DIRECTORY_PROPERTY command.
        private static readonly string[] _getDirectoryPropertyParams =
            _getCMakePropertyParams;

        // Parameters to the GET_FILENAME_COMPONENT command.
        private static readonly string[] _getFileNameComponentParams = new string[]
        {
            "variable",
            "filename",
            "component"
        };

        // Parameters to the GET_SOURCE_FILE_PROPERTY command.
        private static readonly string[] _getSourceFilePropertyParams = new string[]
        {
            "variable",
            "filename",
            "property"
        };

        // Parameters to the GET_TARGET_PROPERTY command.
        private static readonly string[] _getTargetPropertyParams = new string[]
        {
            "variable",
            "target",
            "property"
        };

        // Parameters to the GET_TEST_PROPERTY command.
        private static readonly string[] _getTestPropertyParams = new string[]
        {
            "test",
            "property",
            "variable"
        };

        // Parameters to the INCLUDE_DIRECTORIES command.
        private static readonly string[] _includeDirectoriesParams = new string[]
        {
            "directory1 directory2 ..."
        };

        // Parameters to the INCLUDE_EXTERNAL_MSPROJECT command.
        private static readonly string[] _includeExternalMsProjectParams = new string[]
        {
            "project_name",
            "location",
            "dependency1 dependency2 ..."
        };

        // Parameters to the INCLUDE_REGULAR_EXPRESSION command.
        private static readonly string[] _includeRegularExpressionParams = new string[]
        {
            "regex_match",
            "[regex_complain]"
        };

        // Parameters to the INSTALL_FILES command.
        private static readonly string[] _installFilesParams = new string[]
        {
            "directory",
            "extension",
            "file1 file2 ..."
        };

        // Parameters to the INSTALL_PROGRAMS command.
        private static readonly string[] _installProgramsParams = new string[]
        {
            "directory",
            "file1 file2 ..."
        };

        // Parameters to the INSTALL_TARGETS command.
        private static readonly string[] _installTargetsParams = new string[]
        {
            "directory",
            "target1 target2 ..."
        };

        // Parameters to the LINK_DIRECTORIES command.
        private static readonly string[] _linkDirectoriesParams = _includeDirectoriesParams;

        // Parameters to the MACRO command.
        private static readonly string[] _macroParams = _functionParams;

        // Parameters to the MAKE_DIRECTORY command.
        private static readonly string[] _makeDirectoryParams = new string[]
        {
            "directory"
        };

        // Parameters to the OPTION command.
        private static readonly string[] _optionParams = new string[]
        {
            "variable",
            "help_string",
            "[initial_value]"
        };

        // Parameters to the OUTPUT_REQUIRED_FILES command.
        private static readonly string[] _outputRequiredFilesParams = new string[]
        {
            "source_file",
            "output_file"
        };

        // Parameters to the PROJECT command.
        private static readonly string[] _projectParams = new string[]
        {
            "project_name",
            "[language1 language2 ...]"
        };

        // Parameters to the REMOVE command.
        private static readonly string[] _removeParams = new string[]
        {
            "variable",
            "value1 value2 ..."
        };

        // Parameters to the SET command.
        private static readonly string[] _setParams = new string[]
        {
            "variable",
            "value"
        };

        // Parameters to the SOURCE_GROUP command.
        private static readonly string[] _sourceGroupParams = new string[]
        {
            "name"
        };

        // Parameters to the SUBDIR_DEPENDS command.
        private static readonly string[] _subdirDependsParams = new string[]
        {
            "subdirectory",
            "depenedency1 dependency2 ..."
        };

        // Parameters to the SUBDIRS command.
        private static readonly string[] _subdirsParams = new string[]
        {
            "directory1 directory2 ..."
        };

        // Parameters to the TARGET_COMPILE_DEFINITIONS command.
        private static readonly string[] _targetCompileDefinitions = new string[]
        {
            "target",
            "item1 item2 ..."
        };

        // Parameters to the TARGET_COMPILE_FEATURES command.
        private static readonly string[] _targetCompileFeatures = new string[]
        {
            "target",
            "feature1 feature2 ..."
        };

        // Parameters to the TARGET_COMPILE_OPTIONS command.
        private static readonly string[] _targetCompileOptions = new string[]
        {
            "target",
            "item1 item2 ..."
        };

        // Parameters to the TARGET_INCLUDE_DIRECTORIES command.
        private static readonly string[] _targetIncludeDirectories = new string[]
        {
            "target",
            "item1 item2 ..."
        };

        // Parameters to the TARGET_LINK_LIBRARIES command.
        private static readonly string[] _targetLinkLibrariesParams = new string[]
        {
            "target",
            "item1 item2 ..."
        };

        // Parameters to the TARGET_SOURCES command.
        private static readonly string[] _targetSources = new string[]
        {
            "target",
            "item1 item2 ..."
        };

        // Parameters to the USE_MANGLED_MESA command.
        private static readonly string[] _useManagledMesaParams = new string[]
        {
            "path_to_mesa",
            "output_directory"
        };

        // Parameters to the UTILITY_SOURCE command.
        private static readonly string[] _utilitySourceParams = new string[]
        {
            "cache_entry",
            "executable_name",
            "path_to_source",
            "[file1 file2 ...]"
        };

        // Parameters to the VARIABLE_REQUIRES command.
        private static readonly string[] _variableRequiresParams = new string[]
        {
            "test_variable",
            "result_variable",
            "required_variable1 required_variable2 ..."
        };

        // Parameters to the VARIABLE_WATCH command.
        private static readonly string[] _variableWatchParams = new string[]
        {
            "variable",
            "[command]"
        };

        // Parameters to the WRITE_FILE command.
        private static readonly string[] _writeFileParams = new string[]
        {
            "filename",
            "message1 message2 ..."
        };

        // Parameters to commands that take a single expression.
        private static readonly string[] _expression = new string[] { "expression" };

        // Parameters to commands that take an optional expression.
        private static readonly string[] _optionalExpression =
            new string[] { "[expression]" };

        // Parameters to commands that take a single variable name.
        private static readonly string[] _variable = new string[] { "variable" };

        // Parameters to commands that take no parameters.
        private static readonly string[] _noParams = new string[] { };

        // Dictionary mapping command identifiers to arrays of parameter names.
        private static readonly Dictionary<CMakeCommandId, string[]> _parameters =
            new Dictionary<CMakeCommandId, string[]>
        {
            { CMakeCommandId.AddDependencies,           _addDependenciesParams },
            { CMakeCommandId.AddExecutable,             _addExecutableParams },
            { CMakeCommandId.AddLibrary,                _addExecutableParams },
            { CMakeCommandId.AddSubdirectory,           _addSubdirectoryParams },
            { CMakeCommandId.AddTest,                   _addTestParams },
            { CMakeCommandId.AuxSourceDirectory,        _auxSourceDirectoryParams },
            { CMakeCommandId.Break,                     _noParams },
            { CMakeCommandId.BuildCommand,              _variable },
            { CMakeCommandId.BuildName,                 _variable },
            { CMakeCommandId.ConfigureFile,             _configureFileParams },
            { CMakeCommandId.Continue,                  _noParams },
            { CMakeCommandId.Else,                      _optionalExpression },
            { CMakeCommandId.ElseIf,                    _expression },
            { CMakeCommandId.EnableLanguage,            _enableLanguageParams },
            { CMakeCommandId.EnableTesting,             _noParams },
            { CMakeCommandId.EndForEach,                _optionalExpression },
            { CMakeCommandId.EndFunction,               _endFunctionParams },
            { CMakeCommandId.EndIf,                     _optionalExpression },
            { CMakeCommandId.EndMacro,                  _endMacroParams },
            { CMakeCommandId.EndWhile,                  _optionalExpression },
            { CMakeCommandId.ExportLibraryDependencies, _exportLibraryDependenciesParams },
            { CMakeCommandId.FindFile,                  _findFileParams },
            { CMakeCommandId.FindPath,                  _findPathParams },
            { CMakeCommandId.FindProgram,               _findProgramParams },
            { CMakeCommandId.FLTKWrapUi,                _fltkWrapUiParams },
            { CMakeCommandId.ForEach,                   _forEachParams },
            { CMakeCommandId.Function,                  _functionParams },
            { CMakeCommandId.GetCMakeProperty,          _getCMakePropertyParams },
            { CMakeCommandId.GetDirectoryProperty,      _getDirectoryPropertyParams },
            { CMakeCommandId.GetFileNameComponent,      _getFileNameComponentParams },
            { CMakeCommandId.GetSourceFileProperty,     _getSourceFilePropertyParams },
            { CMakeCommandId.GetTargetProperty,         _getTargetPropertyParams },
            { CMakeCommandId.GetTestProperty,           _getTestPropertyParams },
            { CMakeCommandId.If,                        _expression },
            { CMakeCommandId.IncludeDirectories,        _includeDirectoriesParams },
            { CMakeCommandId.IncludeExternalMsProject,  _includeExternalMsProjectParams },
            { CMakeCommandId.IncludeRegularExpression,  _includeRegularExpressionParams },
            { CMakeCommandId.InstallFiles,              _installFilesParams },
            { CMakeCommandId.InstallPrograms,           _installProgramsParams },
            { CMakeCommandId.InstallTargets,            _installTargetsParams },
            { CMakeCommandId.LinkDirectories,           _linkDirectoriesParams },
            { CMakeCommandId.Macro,                     _macroParams },
            { CMakeCommandId.MakeDirectory,             _makeDirectoryParams },
            { CMakeCommandId.Option,                    _optionParams },
            { CMakeCommandId.OutputRequiredFiles,       _outputRequiredFilesParams },
            { CMakeCommandId.Project,                   _projectParams },
            { CMakeCommandId.Remove,                    _removeParams },
            { CMakeCommandId.Return,                    _noParams },
            { CMakeCommandId.Set,                       _setParams },
            { CMakeCommandId.SiteName,                  _variable },
            { CMakeCommandId.SourceGroup,               _sourceGroupParams },
            { CMakeCommandId.SubdirDepends,             _subdirDependsParams },
            { CMakeCommandId.Subdirs,                   _subdirsParams },
            { CMakeCommandId.TargetCompileDefinitions,  _targetCompileDefinitions },
            { CMakeCommandId.TargetCompileFeatures,     _targetCompileFeatures },
            { CMakeCommandId.TargetCompileOptions,      _targetCompileOptions },
            { CMakeCommandId.TargetIncludeDirectories,  _targetIncludeDirectories },
            { CMakeCommandId.TargetLinkLibraries,       _targetLinkLibrariesParams },
            { CMakeCommandId.TargetSources,             _targetSources },
            { CMakeCommandId.Unset,                     _variable },
            { CMakeCommandId.UseMangledMesa,            _useManagledMesaParams },
            { CMakeCommandId.UtilitySource,             _utilitySourceParams },
            { CMakeCommandId.VariableRequires,          _variableRequiresParams },
            { CMakeCommandId.VariableWatch,             _variableWatchParams },
            { CMakeCommandId.While,                     _expression },
            { CMakeCommandId.WriteFile,                 _writeFileParams }
        };

        // The identifier of the command for which a given instance will provide
        // parameters.
        private CMakeCommandId _id;

        private CMakeMethods(CMakeCommandId id)
        {
            _id = id;
        }

        public override int GetCount()
        {
            // Only provide a single signature.
            return 1;
        }

        public override string GetDescription(int index)
        {
            return null;
        }

        public override string GetName(int index)
        {
            // Look up the name of the command.
            return CMakeKeywords.GetCommandFromId(_id);
        }

        public override int GetParameterCount(int index)
        {
            // Look up the number of parameters that the command takes.
            return _parameters[_id].Length;
        }

        public override void GetParameterInfo(int index, int parameter, out string name,
            out string display, out string description)
        {
            // Look up the name of the requested parameter.
            name = _parameters[_id][parameter];
            display = _parameters[_id][parameter];
            description = null;
        }

        public override string GetType(int index)
        {
            // CMake commands don't have return types.
            return null;
        }

        public override string Delimiter
        {
            get
            {
                // In CMake, spaces instead of commas are used to separate arguments.
                return " ";
            }
        }

        /// <summary>
        /// Get a methods object containing the parameters for a given CMake command.
        /// </summary>
        /// <param name="id">The identifier of a CMake command.</param>
        /// <param name="subcommand">The name of a subcommand or null if none.</param>
        /// <returns>
        /// A methods object or null if there is no parameter information.
        /// </returns>
        public static Methods GetCommandParameters(CMakeCommandId id, string subcommand)
        {
            if (subcommand != null)
            {
                return CMakeSubcommandMethods.GetSubcommandParameters(id, subcommand);
            }
            if (!_parameters.ContainsKey(id))
            {
                return null;
            }
            return new CMakeMethods(id);
        }

        /// <summary>
        /// Get the number of parameters expected by a given CMake command.
        /// </summary>
        /// <param name="id">The identifier of a CMake command.</param>
        /// <returns>The number of expected parameters.</returns>
        public static int GetParameterCount(CMakeCommandId id)
        {
            if (!_parameters.ContainsKey(id))
            {
                return 0;
            }
            return _parameters[id].Length;
        }

        /// <summary>
        /// Get the Quick Info tip for a given CMake command.
        /// </summary>
        /// <param name="id">The identifier of a CMake command.</param>
        /// <returns>The Quick Info tip.</returns>
        public static string GetCommandQuickInfoTip(CMakeCommandId id)
        {
            if (!_parameters.ContainsKey(id))
            {
                return null;
            }
            return string.Format("{0}({1})", CMakeKeywords.GetCommandFromId(id),
                string.Join(" ", _parameters[id]));
        }
    }
}
