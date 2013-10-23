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
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMakeTools
{
    /// <summary>
    /// Declarations object for CMake include files.
    /// </summary>
    class CMakeIncludeDeclarations : CMakeItemDeclarations
    {
        // Default set of CMake modules to show in list if CMake is not installed.
        private static readonly string[] _defaultModules =
        {
            "AddFileDependencies",
            "BundleUtilities",
            "CheckCCompilerFlag",
            "CheckCSourceCompiles",
            "CheckCSourceRuns",
            "CheckCXXCompilerFlag",
            "CheckCXXSourceCompiles",
            "CheckCXXSourceRuns",
            "CheckCXXSymbolExists",
            "CheckFortranFunctionExists",
            "CheckFunctionExists",
            "CheckIncludeFile",
            "CheckIncludeFileCXX",
            "CheckIncludeFiles",
            "CheckLanguage",
            "CheckLibraryExists",
            "CheckPrototypeDefinition",
            "CheckSizeOf",
            "CheckStructHasMember",
            "CheckSymbolExists",
            "CheckTypeSize",
            "CheckVariableExists",
            "CMake",
            "CMakeAddFortranSubdirectory",
            "CMakeASM_MASMInformation",
            "CMakeASM_NASMInformation",
            "CMakeASMInformation",
            "CMakeBackwardCompatibilityC",
            "CMakeBackwardCompatibilityCXX",
            "CMakeBorlandFindMake",
            "CMakeCInformation",
            "CMakeClDeps",
            "CMakeCommonLanguageInclude",
            "CMakeCXXInformation",
            "CMakeDependentOption",
            "CMakeDetermineASM_MASMCompiler",
            "CMakeDetermineASM_NASMCompiler",
            "CMakeDetermineASMCompiler",
            "CMakeDetermineCCompiler",
            "CMakeDetermineCompiler",
            "CMakeDetermineCXXCompiler",
            "CMakeDetermineFortranCompiler",
            "CMakeDetermineJavaCompiler",
            "CMakeDetermineRCCompiler",
            "CMakeDetermineSystem",
            "CMakeDetermineVSServicePack",
            "CMakeExpandImportedTargets",
            "CMakeExportBuildSettings",
            "CMakeExtraGeneratorDetermineCompilerMacrosAndIncludeDirs",
            "CMakeFindBinUtils",
            "CMakeFindCodeBlocks",
            "CMakeFindEclipseCDT4",
            "CMakeFindFrameworks",
            "CMakeFindKDevelop3",
            "CMakeFindPackageMode",
            "CMakeFindWMake",
            "CMakeFindXCode",
            "CMakeForceCompiler",
            "CMakeFortranInformation",
            "CMakeGenericSystem",
            "CMakeGraphVizOptions",
            "CMakeImportBuildSettings",
            "CMakeJavaInformation",
            "CMakeJOMFindMake",
            "CMakeMinGWFindMake",
            "CMakeMSYSFindMake",
            "CMakeNinjaFindMake",
            "CMakeNMakeFindMake",
            "CMakePackageConfigHelpers",
            "CMakeParseArguments",
            "CMakePrintHelpers",
            "CMakePrintSystemInformation",
            "CMakePushCheckState",
            "CMakeRCInformation",
            "CMakeSystemSpecificInformation",
            "CMakeTestASM_MASMCompiler",
            "CMakeTestASM_NASMCompiler",
            "CMakeTestASMCompiler",
            "CMakeTestCCompiler",
            "CMakeTestCompilerCommon",
            "CMakeTestCXXCompiler",
            "CMakeTestFortranCompiler",
            "CMakeTestJavaCompiler",
            "CMakeTestRCCompiler",
            "CMakeUnixFindMake",
            "CMakeVerifyManifest",
            "CMakeVS6BackwardCompatibility",
            "CMakeVS6FindMake",
            "CMakeVS7BackwardCompatibility",
            "CMakeVS7FindMake",
            "CMakeVS8FindMake",
            "CMakeVS9FindMake",
            "CMakeVS10FindMake",
            "CMakeVS11FindMake",
            "CMakeVS12FindMake",
            "CMakeVS71FindMake",
            "CPack",
            "CPackBundle",
            "CPackComponent",
            "CPackCygwin",
            "CPackDeb",
            "CPackDMG",
            "CPackNSIS",
            "CPackPackageMaker",
            "CPackRPM",
            "CPackWIX",
            "CPackZIP",
            "CTest",
            "CTestScriptMode",
            "CTestTargets",
            "CTestUseLaunchers",
            "Dart",
            "DeployQt4",
            "Documentation",
            "ExternalData",
            "ExternalProject",
            "FeatureSummary",
            "FindALSA",
            "FindArmadillo",
            "FindASPELL",
            "FindAVIFile",
            "FindBISON",
            "FindBLAS",
            "FindBoost",
            "FindBullet",
            "FindBZip2",
            "FindCABLE",
            "FindCoin3D",
            "FindCUDA",
            "FindCups",
            "FindCURL",
            "FindCurses",
            "FindCVS",
            "FindCxxTest",
            "FindCygwin",
            "FindDart",
            "FindDCMTK",
            "FindDevIL",
            "FindDoxygen",
            "FindEXPAT",
            "FindFLEX",
            "FindFLTK",
            "FindFLTK2",
            "FindFreetype",
            "FindGCCXML",
            "FindGDAL",
            "FindGettext",
            "FindGIF",
            "FindGit",
            "FindGLEW",
            "FindGLU",
            "FindGLUT",
            "FindGnuplot",
            "FindGnuTLS",
            "FindGTest",
            "FindGTK",
            "FindGTK2",
            "FindHDF5",
            "FindHg",
            "FindHSPELL",
            "FindHTMLHelp",
            "FindIcotool",
            "FindImageMagick",
            "FindITK",
            "FindJasper",
            "FindJava",
            "FindJNI",
            "FindJPEG",
            "FindKDE3",
            "FindKDE4",
            "FindLAPACK",
            "FindLATEX",
            "FindLibArchive",
            "FindLibLZMA",
            "FindLibXml2",
            "FindLibXslt",
            "FindLua50",
            "FindLua51",
            "FindMatlab",
            "FindMFC",
            "FindMotif",
            "FindMPEG",
            "FindMPEG2",
            "FindMPI",
            "FindOpenAL",
            "FindOpenGL",
            "FindOpenMP",
            "FindOpenSceneGraph",
            "FindOpenSSL",
            "FindOpenThreads",
            "Findosg",
            "Findosg_functions",
            "FindosgAnimation",
            "FindosgDB",
            "FindosgFX",
            "FindosgGA",
            "FindosgIntrospection",
            "FindosgManipulator",
            "FindosgParticle",
            "FindosgPresentation",
            "FindosgProducer",
            "FindosgQt",
            "FindosgShadow",
            "FindosgSim",
            "FindosgTerrain",
            "FindosgText",
            "FindosgUtil",
            "FindosgViewer",
            "FindosgVolume",
            "FindosgWidget",
            "FindPackageHandleStandardArgs",
            "FindPackageMessage",
            "FindPerl",
            "FindPerlLibs",
            "FindPHP4",
            "FindPhysFS",
            "FindPike",
            "FindPkgConfig",
            "FindPNG",
            "FindPostgreSQL",
            "FindProducer",
            "FindProtobuf",
            "FindPythonInterp",
            "FindPythonLibs",
            "FindQt",
            "FindQt3",
            "FindQt4",
            "FindQuickTime",
            "FindRTI",
            "FindRuby",
            "FindSDL",
            "FindSDL_image",
            "FindSDL_mixer",
            "FindSDL_net",
            "FindSDL_sound",
            "FindSDL_ttf",
            "FindSelfPackers",
            "FindSquish",
            "FindSubversion",
            "FindSWIG",
            "FindTCL",
            "FindTclsh",
            "FindTclStub",
            "FindThreads",
            "FindTIFF",
            "FindUnixCommands",
            "FindVTK",
            "FindWget",
            "FindWish",
            "FindwxWidgets",
            "FindwxWindows",
            "FindX11",
            "FindXMLRPC",
            "FindZLIB",
            "FLTKCompatibility",
            "FortranCInterface",
            "GenerateExportHeader",
            "GetPrerequisites",
            "GNUInstallDirs",
            "InstallRequiredSystemLibraries",
            "ITKCompatibility",
            "KDE3Macros",
            "kde3uic",
            "MacroAddFileDependencies",
            "ProcessorCount",
            "Qt4ConfigDependentSettings",
            "Qt4Macros",
            "SelectLibraryConfiguration",
            "SquishTestScript",
            "SystemInformation",
            "TestBigEndian",
            "TestCXXAcceptsFlag",
            "TestForANSIForScope",
            "TestForANSIStreamHeaders",
            "TestForSSTREAM",
            "TestForSTDNamespace",
            "Use_wxWindows",
            "UseEcos",
            "UseJava",
            "UseJavaClassFilelist",
            "UseJavaSymlinks",
            "UsePkgConfig",
            "UseQt4",
            "UseSWIG",
            "UseVTK40",
            "UseVTKBuildSettings40",
            "UseVTKConfig40",
            "UsewxWidgets",
            "VTKCompatibility",
            "WriteBasicConfigVersionFile"
        };

        // Modules used internally by CMake that should be excluded from the
        // member selection list.
        private static readonly string[] _internalModules =
        {
            "CMakeDetermineCompilerABI",
            "CMakeDetermineCompilerId",
            "CMakeParseImplicitLinkInfo"
        };

        // Array of include files to be displayed.
        private string _sourceFilePath;

        public CMakeIncludeDeclarations(string sourceFilePath)
        {
            _sourceFilePath = sourceFilePath;
            FindIncludeFiles();
        }

        private void FindIncludeFiles()
        {
            // Find all *.cmake files in the same directory as the current source file,
            // excluding cmake_install.cmake, which is generated by CMake during
            // configuration.
            string dirPath = Path.GetDirectoryName(_sourceFilePath);
            IEnumerable<string> files = GetFilesFromDir(dirPath);
            AddItems(files, GetIncludeFileItemType());

            // Find all *.cmake files in the Modules directory inside the CMake
            // installation, if there is one.
            string pathToModules = CMakePath.FindCMakeModules();
            bool foundModules = false;
            if (pathToModules != null)
            {
                foundModules = true;
                files = GetFilesFromDir(pathToModules, true);
                AddItems(files, GetModuleItemType());
            }
            if (!foundModules)
            {
                // If we couldn't find modules in a CMake installation, show a default
                // hard-code listing.
                AddItems(GetDefaultModules(), GetModuleItemType());
            }
        }

        /// <summary>
        /// Find all the CMake include files in a given directory.
        /// </summary>
        /// <param name="dirPath">The directory in which to search.</param>
        /// <param name="treatAsModules">
        /// Whether the files should be treated as standard CMake modules rather than
        /// user-defined include files.
        /// </param>
        /// <returns>A collection of file names.</returns>
        protected virtual IEnumerable<string> GetFilesFromDir(string dirPath,
            bool treatAsModules = false)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(dirPath, "*.cmake");
            if (treatAsModules)
            {
                files = files.Select(Path.GetFileNameWithoutExtension);

                // Remove certain modules that are used internally by CMake and
                // shouldn't be included by user code.
                files = files.Where(
                    x => Array.BinarySearch(_internalModules, x) < 0).ToList();
            }
            else
            {
                files = files.Select(Path.GetFileName);
                files = files.Where(x => !x.Equals("cmake_install.cmake"));
            }
            return files;
        }

        /// <summary>
        /// Get a collection of modules that are included with CMake for use when there
        /// is no CMake installation available.
        /// </summary>
        /// <returns>A collection of standard CMake modules.</returns>
        protected virtual IEnumerable<string> GetDefaultModules()
        {
            return _defaultModules;
        }

        /// <summary>
        /// Get the item type to be used for include files.
        /// </summary>
        /// <returns>An item type.</returns>
        protected virtual ItemType GetIncludeFileItemType()
        {
            return ItemType.IncludeFile;
        }

        /// <summary>
        /// Get the item type to be used for modules.
        /// </summary>
        /// <returns>An item type.</returns>
        protected virtual ItemType GetModuleItemType()
        {
            return ItemType.Module;
        }

        public override string OnCommit(IVsTextView textView, string textSoFar,
            char commitCharacter, int index, ref TextSpan initialExtent)
        {
            // Allow the user to type file names involving CMake variables without
            // member selection getting in the way.
            if (commitCharacter == '$')
            {
                return string.IsNullOrEmpty(textSoFar) ? "$" : textSoFar;
            }
            return base.OnCommit(textView, textSoFar, commitCharacter, index,
                ref initialExtent);
        }
    }
}
