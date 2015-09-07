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
            "CheckFortranCompilerFlag",
            "CheckFortranFunctionExists",
            "CheckFortranSourceCompiles",
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
            "CMakeCXXInformation",
            "CMakeClDeps",
            "CMakeCommonLanguageInclude",
            "CMakeCompilerIdDetection",
            "CMakeDependentOption",
            "CMakeDetermineASM_MASMCompiler",
            "CMakeDetermineASM_NASMCompiler",
            "CMakeDetermineASMCompiler",
            "CMakeDetermineCCompiler",
            "CMakeDetermineCompileFeatures",
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
            "CMakeFindDependencyMacro",
            "CMakeFindEclipseCDT4",
            "CMakeFindFrameworks",
            "CMakeFindKDevelop3",
            "CMakeFindKate",
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
            "CMakeSystemSpecificInitialize",
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
            "CPackDMG",
            "CPackDeb",
            "CPackIFW",
            "CPackNSIS",
            "CPackPackageMaker",
            "CPackRPM",
            "CPackWIX",
            "CPackZIP",
            "CTest",
            "CTestCoverageCollectGCOV",
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
            "FindASPELL",
            "FindAVIFile",
            "FindArmadillo",
            "FindBISON",
            "FindBLAS",
            "FindBZip2",
            "FindBacktrace",
            "FindBoost",
            "FindBullet",
            "FindCABLE",
            "FindCUDA",
            "FindCURL",
            "FindCVS",
            "FindCoin3D",
            "FindCups",
            "FindCurses",
            "FindCxxTest",
            "FindCygwin",
            "FindDCMTK",
            "FindDart",
            "FindDevIL",
            "FindDoxygen",
            "FindEXPAT",
            "FindFLEX",
            "FindFLTK",
            "FindFLTK2",
            "FindFreetype",
            "FindGCCXML",
            "FindGDAL",
            "FindGIF",
            "FindGLEW",
            "FindGLU",
            "FindGLUT",
            "FindGTK",
            "FindGTK2",
            "FindGTest",
            "FindGettext",
            "FindGit",
            "FindGnuplot",
            "FindGnuTLS",
            "FindHDF5",
            "FindHSPELL",
            "FindHTMLHelp",
            "FindHg",
            "FindIce",
            "FindIcotool",
            "FindImageMagick",
            "FindIntl",
            "FindITK",
            "FindJNI",
            "FindJPEG",
            "FindJasper",
            "FindJava",
            "FindKDE3",
            "FindKDE4",
            "FindLAPACK",
            "FindLATEX",
            "FindLibArchive",
            "FindLibLZMA",
            "FindLibXml2",
            "FindLibXslt",
            "FindLua",
            "FindLua50",
            "FindLua51",
            "FindMFC",
            "FindMPEG",
            "FindMPEG2",
            "FindMPI",
            "FindMatlab",
            "FindMotif",
            "FindOpenAL",
            "FindOpenCL",
            "FindOpenGL",
            "FindOpenMP",
            "FindOpenSSL",
            "FindOpenSceneGraph",
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
            "FindPHP4",
            "FindPNG",
            "FindPackageHandleStandardArgs",
            "FindPackageMessage",
            "FindPerl",
            "FindPerlLibs",
            "FindPhysFS",
            "FindPike",
            "FindPkgConfig",
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
            "FindSWIG",
            "FindSelfPackers",
            "FindSquish",
            "FindSubversion",
            "FindTCL",
            "FindTIFF",
            "FindTclStub",
            "FindTclsh",
            "FindThreads",
            "FindUnixCommands",
            "FindVTK",
            "FindWget",
            "FindWish",
            "FindX11",
            "FindXCTest",
            "FindXMLRPC",
            "FindXercesC",
            "FindZLIB",
            "FindwxWidgets",
            "FindwxWindows",
            "FLTKCompatibility",
            "FortranCInterface",
            "GNUInstallDirs",
            "GenerateExportHeader",
            "GetPrerequisites",
            "ITKCompatibility",
            "InstallRequiredSystemLibraries",
            "KDE3Macros",
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
            "Use_wxWindows",
            "UsewxWidgets",
            "VTKCompatibility",
            "WriteBasicConfigVersionFile",
            "WriteCompilerDetectionHeader",
            "ecos_clean",
            "kde3uic"
        };

        // Modules used internally by CMake that should be excluded from the
        // member selection list.
        private static readonly string[] _internalModules =
        {
            "CMakeCheckCompilerFlagCommonPatterns",
            "CMakeDetermineCompilerABI",
            "CMakeDetermineCompilerId",
            "CMakeFindJavaCommon",
            "CMakeParseImplicitLinkInfo"
        };

        // Array of include files to be displayed.
        private string _sourceFilePath;

        public CMakeIncludeDeclarations(string sourceFilePath)
        {
            _sourceFilePath = sourceFilePath;
        }

        /// <summary>
        /// Find all include files and add them to this object.
        /// </summary>
        public void FindIncludeFiles()
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
