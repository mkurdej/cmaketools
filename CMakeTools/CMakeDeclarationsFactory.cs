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
using System.Linq;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Factory to create declarations objects for CMake commands.
    /// </summary>
    static class CMakeDeclarationsFactory
    {
        private delegate CMakeItemDeclarations FactoryMethod(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters);

        // Map from CMake commands to factory methods to create their corresponding
        // declarations objects for use after an opening parenthesis.
        private readonly static Dictionary<CMakeCommandId, FactoryMethod> _methods =
            new Dictionary<CMakeCommandId, FactoryMethod>()
        {
            { CMakeCommandId.Include,                   CreateIncludeDeclarations },
            { CMakeCommandId.FindPackage,               CreatePackageDeclarations },
            { CMakeCommandId.AddSubdirectory,           CreateSubdirectoryDeclarations },
            { CMakeCommandId.EnableLanguage,            CreateLanguageDeclarations },
            { CMakeCommandId.AddDependencies,           CreateTargetDeclarations },
            { CMakeCommandId.TargetCompileDefinitions,  CreateTargetDeclarations },
            { CMakeCommandId.TargetCompileFeatures,     CreateTargetDeclarations },
            { CMakeCommandId.TargetCompileOptions,      CreateTargetDeclarations },
            { CMakeCommandId.TargetIncludeDirectories,  CreateTargetDeclarations },
            { CMakeCommandId.TargetLinkLibraries,       CreateTargetDeclarations },
            { CMakeCommandId.TargetSources,             CreateTargetDeclarations },
            { CMakeCommandId.SetTargetProperties,       CreateSetXPropertyDeclarations },
            { CMakeCommandId.SetSourceFilesProperties,  CreateSetXPropertyDeclarations },
            { CMakeCommandId.SetTestsProperties,        CreateSetXPropertyDeclarations },
            { CMakeCommandId.SetDirectoryProperties,    CreateSetXPropertyDeclarations },
            { CMakeCommandId.GetTestProperty,           CreateGetXPropertyDeclarations }
        };

        // Map from CMake commands to factory methods to create their corresponding
        // declarations object for use after whitespace.
        private readonly static Dictionary<CMakeCommandId, FactoryMethod> _wsMethods =
            new Dictionary<CMakeCommandId, FactoryMethod>()
        {
            { CMakeCommandId.AddExecutable,             CreateSourceDeclarations },
            { CMakeCommandId.AddLibrary,                CreateSourceDeclarations },
            { CMakeCommandId.AddDependencies,           CreateTargetDeclarations },
            { CMakeCommandId.GetTargetProperty,         CreateGetXPropertyDeclarations },
            { CMakeCommandId.GetSourceFileProperty,     CreateGetXPropertyDeclarations },
            { CMakeCommandId.GetTestProperty,           CreateGetXPropertyDeclarations },
            { CMakeCommandId.GetDirectoryProperty,      CreateGetXPropertyDeclarations },
            { CMakeCommandId.GetCMakeProperty,          CreateGetXPropertyDeclarations },
            { CMakeCommandId.SetTargetProperties,       CreateSetXPropertyDeclarations },
            { CMakeCommandId.SetSourceFilesProperties,  CreateSetXPropertyDeclarations },
            { CMakeCommandId.SetTestsProperties,        CreateSetXPropertyDeclarations },
            { CMakeCommandId.SetDirectoryProperties,    CreateSetXPropertyDeclarations },
            { CMakeCommandId.GetProperty,               CreateGetPropertyDeclarations },
            { CMakeCommandId.SetProperty,               CreateSetPropertyDeclarations },
            { CMakeCommandId.GetFileNameComponent,      CreateGetFileNameComponentDeclarations },
            { CMakeCommandId.CMakeHostSystemInformation,
                CreateCMakeHostSystemInformationDeclarations }
        };

        // Map from SET_*_PROPERTIES commands to factory methods to create declarations
        // objects for the objects on which properties can be set.
        private readonly static Dictionary<CMakePropertyType, FactoryMethod>
            _propObjMethods = new Dictionary<CMakePropertyType, FactoryMethod>()
        {
            { CMakePropertyType.Cache,      CreateCacheVariableDeclarations },
            { CMakePropertyType.Directory,  CreateSubdirectoryDeclarations },
            { CMakePropertyType.Source,     CreateSourceDeclarations },
            { CMakePropertyType.Target,     CreateTargetDeclarations },
            { CMakePropertyType.Test,       CreateTestDeclarations }
        };

        // Keywords that can be used with the ADD_EXECUTABLE command.
        private static readonly string[] _addExecutableKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "MACOSX_BUNDLE",
            "WIN32"
        };

        // Keywords that can be used with the ADD_LIBRARY command.
        private static readonly string[] _addLibraryKeywords = new string[]
        {
            "EXCLUDE_FROM_ALL",
            "MODULE",
            "SHARED",
            "STATIC"
        };

        // Keys for use with the CMAKE_HOST_SYSTEM_INFORMATION command.
        private static readonly string[] _cmakeHostSystemInformationKeys = new string[]
        {
            "AVAILABLE_PHYSICAL_MEMORY",
            "AVAILABLE_VIRTUAL_MEMORY",
            "FQDN",
            "HOSTNAME",
            "NUMBER_OF_LOGICAL_CORES",
            "NUMBER_OF_PHYSICAL_CORES",
            "TOTAL_PHYSICAL_MEMORY",
            "TOTAL_VIRTUAL_MEMORY"
        };

        // File name components for use with the GET_FILENAME_COMPONENT command.
        private static readonly string[] _getFileNameComponentComponents = new string[]
        {
            "ABSOLUTE",
            "DIRECTORY",
            "EXT",
            "NAME",
            "NAME_WE",
            "PATH",
            "REALPATH"
        };

        // Keywords that can be used with the SET_PROPERTY command.
        private static readonly string[] _setPropertyKeywords = new string[]
        {
            "APPEND",
            "APPEND_STRING",
            "PROPERTY"
        };

        private static readonly Dictionary<CMakeCommandId, string[]> _commandKeywords =
            new Dictionary<CMakeCommandId, string[]>()
        {
            { CMakeCommandId.AddExecutable, _addExecutableKeywords },
            { CMakeCommandId.AddLibrary,    _addLibraryKeywords }
        };

        static CMakeDeclarationsFactory()
        {
            // Display subcommands for all commands that have them.
            IEnumerable<CMakeCommandId> triggers =
                CMakeSubcommandMethods.GetMemberSelectionTriggers();
            foreach (CMakeCommandId id in triggers)
            {
                _methods[id] = CreateSubcommandDeclarations;
            }
        }

        private static CMakeItemDeclarations CreateIncludeDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters)
        {
            CMakeIncludeDeclarations decls = new CMakeIncludeDeclarations(req.FileName);
            decls.FindIncludeFiles();
            return decls;
        }

        private static CMakeItemDeclarations CreatePackageDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters)
        {
            CMakePackageDeclarations decls = new CMakePackageDeclarations(req.FileName);
            decls.FindIncludeFiles();
            return decls;
        }

        private static CMakeItemDeclarations CreateSubdirectoryDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            bool requireCMakeLists =
                (CMakePackage.Instance.CMakeOptionPage.ShowSubdirectories ==
                CMakeOptionPage.SubdirectorySetting.CMakeListsOnly);
            CMakeSubdirectoryDeclarations decls =
                new CMakeSubdirectoryDeclarations(req.FileName, requireCMakeLists);
            decls.FindIncludeFiles();
            return decls;
        }

        private static CMakeItemDeclarations CreateLanguageDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            CMakeLanguageDeclarations decls =
                new CMakeLanguageDeclarations(req.FileName);
            decls.FindIncludeFiles();
            return decls;
        }

        private static CMakeItemDeclarations CreateTargetDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters)
        {
            List<string> targets = CMakeParsing.ParseForTargetNames(source.GetLines());
            CMakeItemDeclarations decls = new CMakeItemDeclarations();
            decls.AddItems(targets, CMakeItemDeclarations.ItemType.Target);
            decls.ExcludeItems(priorParameters);
            return decls;
        }

        private static CMakeItemDeclarations CreateTestDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters)
        {
            List<string> tests = CMakeParsing.ParseForTargetNames(source.GetLines(),
                true);
            CMakeItemDeclarations decls = new CMakeItemDeclarations();
            decls.AddItems(tests, CMakeItemDeclarations.ItemType.Target);
            decls.ExcludeItems(priorParameters);
            return decls;
        }

        private static CMakeItemDeclarations CreateSubcommandDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            IEnumerable<string> subcommands = CMakeSubcommandMethods.GetSubcommands(id);
            if (subcommands == null)
            {
                return null;
            }

            CMakeItemDeclarations decls = new CMakeItemDeclarations();
            decls.AddItems(subcommands, CMakeItemDeclarations.ItemType.Command);
            return decls;
        }

        private static CMakeItemDeclarations CreateSourceDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters)
        {
            CMakeSourceDeclarations decls = new CMakeSourceDeclarations(req.FileName);
            decls.FindIncludeFiles();
            if (_commandKeywords.ContainsKey(id))
            {
                decls.AddItems(_commandKeywords[id],
                    CMakeItemDeclarations.ItemType.Command);
            }
            if (id == CMakeCommandId.AddExecutable || id == CMakeCommandId.AddLibrary)
            {
                // Exclude all files that already appear in the parameter list, except
                // for the first token, which is the name of the executable to be
                // generated.
                decls.ExcludeItems(priorParameters.Skip(1));
            }
            return decls;
        }

        private static CMakeItemDeclarations CreateCacheVariableDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            List<string> vars = CMakeParsing.ParseForCacheVariables(source.GetLines());
            return new CMakeVariableDeclarations(vars, CMakeVariableType.CacheVariable);
        }

        private static CMakeItemDeclarations CreateGetXPropertyDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            int priorParameterCount =
                priorParameters != null ? priorParameters.Count : 0;
            if (priorParameterCount == CMakeProperties.GetPropertyParameterIndex(id))
            {
                IEnumerable<string> properties = CMakeProperties.GetPropertiesForCommand(id);
                if (properties != null)
                {
                    CMakeItemDeclarations decls = new CMakeItemDeclarations();
                    decls.AddItems(properties, CMakeItemDeclarations.ItemType.Property);
                    if (id == CMakeCommandId.GetDirectoryProperty)
                    {
                        // The DIRECTORY keyword can be specified before the property
                        // to set a property of a different directory.
                        decls.AddItem("DIRECTORY",
                            CMakeItemDeclarations.ItemType.Command);
                    }
                    return decls;
                }
            }
            else if (priorParameterCount == CMakeProperties.GetObjectParameterIndex(id))
            {
                CMakePropertyType type = CMakeProperties.GetPropertyTypeFromCommand(id);
                if (_propObjMethods.ContainsKey(type))
                {
                    CMakeItemDeclarations decls = _propObjMethods[type](id, req, source,
                        priorParameters);
                    return decls;
                }
            }
            else if (id == CMakeCommandId.GetDirectoryProperty &&
                priorParameterCount == 2 && priorParameters[1] == "DIRECTORY")
            {
                return CreateSubdirectoryDeclarations(id, req, source, priorParameters);
            }
            else if (id == CMakeCommandId.GetDirectoryProperty &&
                priorParameterCount == 3 && priorParameters[1] == "DIRECTORY")
            {
                IEnumerable<string> properties = CMakeProperties.GetPropertiesForCommand(
                    CMakeCommandId.GetDirectoryProperty);
                if (properties != null)
                {
                    CMakeItemDeclarations decls = new CMakeItemDeclarations();
                    decls.AddItems(properties, CMakeItemDeclarations.ItemType.Property);
                    return decls;
                }
            }
            return null;
        }

        private static CMakeItemDeclarations CreateSetXPropertyDeclarations(
            CMakeCommandId id, ParseRequest req, Source source, 
            List<string> priorParameters)
        {
            bool afterPropsKeyword = false;
            if (priorParameters != null)
            {
                int index = priorParameters.FindIndex(x => x.Equals("PROPERTIES"));
                if (index >= 0)
                {
                    afterPropsKeyword = true;
                    if ((priorParameters.Count - index) % 2 == 1)
                    {
                        IEnumerable<string> properties =
                            CMakeProperties.GetPropertiesForCommand(id);
                        if (properties != null)
                        {
                            CMakeItemDeclarations decls = new CMakeItemDeclarations();
                            decls.AddItems(properties,
                                CMakeItemDeclarations.ItemType.Property);
                            return decls;
                        }
                    }
                }
            }
            if (!afterPropsKeyword)
            {
                CMakeItemDeclarations decls;
                CMakePropertyType type = CMakeProperties.GetPropertyTypeFromCommand(id);
                if (_propObjMethods.ContainsKey(type))
                {
                    decls = _propObjMethods[type](id, req, source, priorParameters);
                }
                else
                {
                    decls = new CMakeItemDeclarations();
                }
                if ((priorParameters != null && priorParameters.Count > 0) ||
                    id == CMakeCommandId.SetSourceFilesProperties ||
                    id == CMakeCommandId.SetDirectoryProperties)
                {
                    // The PROPERTIES can appear in the SET_SOURCE_FILES_PROPERTIES and
                    // SET_DIRECTORY_PROPERTIES command without another parameter before
                    // it.
                    decls.AddItem("PROPERTIES", CMakeItemDeclarations.ItemType.Command);
                }
                return decls;
            }
            return null;
        }

        private static CMakeItemDeclarations CreateGetPropertyDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            CMakeItemDeclarations decls = null;
            if (priorParameters != null)
            {
                if (priorParameters.Count == 1)
                {
                    decls = new CMakeItemDeclarations();
                    decls.AddItems(CMakeProperties.GetPropertyTypeKeywords(),
                        CMakeItemDeclarations.ItemType.Command);
                }
                else if (priorParameters.Count > 2 &&
                    priorParameters[priorParameters.Count - 1] == "PROPERTY")
                {
                    IEnumerable<string> properties = CMakeProperties.GetPropertiesOfType(
                        CMakeProperties.GetPropertyTypeFromKeyword(
                        priorParameters[1]));
                    decls = new CMakeItemDeclarations();
                    decls.AddItems(properties, CMakeItemDeclarations.ItemType.Property);
                }
                else if (priorParameters.Count == 2)
                {
                    CMakePropertyType type = CMakeProperties.GetPropertyTypeFromKeyword(
                        priorParameters[1]);
                    if (_propObjMethods.ContainsKey(type))
                    {
                        decls = _propObjMethods[type](id, req, source, priorParameters);
                    }
                    else
                    {
                        decls = new CMakeItemDeclarations();
                    }
                    if (!CMakeProperties.IsObjectRequired(type))
                    {
                        decls.AddItem("PROPERTY",
                            CMakeItemDeclarations.ItemType.Command);
                    }
                }
                else if (priorParameters.Count == 3)
                {
                    decls = new CMakeItemDeclarations();
                    decls.AddItem("PROPERTY", CMakeItemDeclarations.ItemType.Command);
                }
            }
            return decls;
        }

        private static CMakeItemDeclarations CreateSetPropertyDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            CMakeItemDeclarations decls = null;
            int propertyIndex = priorParameters.IndexOf("PROPERTY");
            if (propertyIndex < 0)
            {
                CMakePropertyType type = CMakeProperties.GetPropertyTypeFromKeyword(
                    priorParameters[0]);
                if (_propObjMethods.ContainsKey(type) &&
                    !priorParameters.Any(x => _setPropertyKeywords.Contains(x)))
                {
                    decls = _propObjMethods[type](id, req, source, priorParameters);
                }
                else
                {
                    decls = new CMakeItemDeclarations();
                }
                if (priorParameters.Count > 1 || type == CMakePropertyType.Global)
                {
                    decls.AddItems(_setPropertyKeywords,
                        CMakeItemDeclarations.ItemType.Command);
                }
                decls.ExcludeItems(priorParameters.Skip(1));
            }
            else if (propertyIndex == priorParameters.Count - 1)
            {
                CMakePropertyType type = CMakeProperties.GetPropertyTypeFromKeyword(
                    priorParameters[0]);
                IEnumerable<string> properties = CMakeProperties.GetPropertiesOfType(
                    type);
                decls = new CMakeItemDeclarations();
                decls.AddItems(properties, CMakeItemDeclarations.ItemType.Property);
            }
            return decls;
        }

        private static CMakeItemDeclarations CreateGetFileNameComponentDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            if (priorParameters.Count != 2)
            {
                return null;
            }
            CMakeItemDeclarations decls = new CMakeItemDeclarations();
            decls.AddItems(_getFileNameComponentComponents,
                CMakeItemDeclarations.ItemType.Command);
            return decls;
        }

        private static CMakeItemDeclarations CreateCMakeHostSystemInformationDeclarations(
            CMakeCommandId id, ParseRequest req, Source source,
            List<string> priorParameters)
        {
            if (priorParameters.Count != 3 || priorParameters[2] != "QUERY")
            {
                return null;
            }
            CMakeItemDeclarations decls = new CMakeItemDeclarations();
            decls.AddItems(_cmakeHostSystemInformationKeys,
                CMakeItemDeclarations.ItemType.Command);
            return decls;
        }

        /// <summary>
        /// Create a declarations object.
        /// </summary>
        /// <param name="id">The CMake command for which to create the object.</param>
        /// <param name="req">The parse request for which to create the object.</param>
        /// <param name="source">The CMake source file.</param>
        /// <param name="priorParameters">
        /// List of parameters appearing prior to the parameters that triggered the
        /// parse request, if it was triggered by whitespace, or null otherwise.
        /// </param>
        /// <returns>The newly created declarations object.</returns>
        public static CMakeItemDeclarations CreateDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, List<string> priorParameters = null)
        {
            Dictionary<CMakeCommandId, FactoryMethod> map =
                priorParameters == null ? _methods : _wsMethods;
            if (!map.ContainsKey(id))
            {
                return null;
            }
            return map[id](id, req, source, priorParameters);
        }

        /// <summary>
        /// Get all the commands that should trigger member selection.
        /// </summary>
        /// <returns>A collection of commands.</returns>
        public static IEnumerable<CMakeCommandId> GetMemberSelectionTriggers()
        {
            return _methods.Keys;
        }

        /// <summary>
        /// Get all the commands that should trigger member selection on whitespace.
        /// </summary>
        /// <returns>A collection of commands.</returns>
        public static IEnumerable<CMakeCommandId> GetWSMemberSelectionTriggers()
        {
            return _wsMethods.Keys;
        }
    }
}
