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

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Factory to create declarations objects for CMake commands.
    /// </summary>
    static class CMakeDeclarationsFactory
    {
        private delegate Declarations FactoryMethod(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package);

        // Map from CMake commands to factory methods to create their corresponding
        // declarations objects.
        private static Dictionary<CMakeCommandId, FactoryMethod> _methods =
            new Dictionary<CMakeCommandId, FactoryMethod>()
        {
            { CMakeCommandId.Include,               CreateIncludeDeclarations },
            { CMakeCommandId.FindPackage,           CreatePackageDeclarations },
            { CMakeCommandId.AddSubdirectory,       CreateSubdirectoryDeclarations },
            { CMakeCommandId.EnableLanguage,        CreateLanguageDeclarations },
            { CMakeCommandId.AddDependencies,       CreateTargetDeclarations },
            { CMakeCommandId.TargetLinkLibraries,   CreateTargetDeclarations }
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

        private static Declarations CreateIncludeDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            return new CMakeIncludeDeclarations(req.FileName);
        }

        private static Declarations CreatePackageDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            return new CMakePackageDeclarations(req.FileName);
        }

        private static Declarations CreateSubdirectoryDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            bool requireCMakeLists =
                (package.CMakeOptionPage.ShowSubdirectories ==
                CMakeOptionPage.SubdirectorySetting.CMakeListsOnly);
            return new CMakeSubdirectoryDeclarations(req.FileName, requireCMakeLists);
        }

        private static Declarations CreateLanguageDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            return new CMakeLanguageDeclarations(req.FileName);
        }

        private static Declarations CreateTargetDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            List<string> targets = CMakeParsing.ParseForTargetNames(source.GetLines());
            return new CMakeTargetDeclarations(targets);
        }

        private static Declarations CreateSubcommandDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            return CMakeSubcommandDeclarations.GetSubcommandDeclarations(id);
        }

        /// <summary>
        /// Create a declarations object.
        /// </summary>
        /// <param name="id">The CMake command for which to create the object.</param>
        /// <param name="req">The parse request for which to create the object.</param>
        /// <param name="source">The CMake source file.</param>
        /// <param name="package">The CMake Tools for Visual Studio package.</param>
        /// <returns>The newly created declarations object.</returns>
        public static Declarations CreateDeclarations(CMakeCommandId id,
            ParseRequest req, Source source, CMakePackage package)
        {
            if (!_methods.ContainsKey(id))
            {
                return null;
            }
            return _methods[id](id, req, source, package);
        }

        /// <summary>
        /// Get all the commands that should trigger member selection.
        /// </summary>
        /// <returns>A collection of commands.</returns>
        public static IEnumerable<CMakeCommandId> GetMemberSelectionTriggers()
        {
            return _methods.Keys;
        }
    }
}
