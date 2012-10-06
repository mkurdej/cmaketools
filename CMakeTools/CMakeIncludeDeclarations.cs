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
    class CMakeIncludeDeclarations : Declarations
    {
        // Default set of CMake modules to show in list if CMake is not installed.
        private static string[] _defaultModules =
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
            "CheckLibraryExists",
            "CheckPrototypeDefinition",
            "CheckSizeOf",
            "CheckStructHasMember",
            "CheckSymbolExists",
            "CheckTypeSize",
            "CheckVariableExists",
            "CMake",
            "CMakeASM_MASMInformation",
            "CMakeASM_NASMInformation",
            "CMakeASMInformation",
            "CMakeBackwardCompatibilityC",
            "CMakeBackwardCompatibilityCXX",
            "CMakeBorlandFindMake",
            "CMakeCInformation",
            "CMakeCommonLanguageInclude",
            "CMakeCXXInformation",
            "CMakeDependentOption",
            "CMakeDetermineASM_MASMCompiler",
            "CMakeDetermineASM_NASMCompiler",
            "CMakeDetermineASMCompiler",
            "CMakeDetermineCCompiler",
            "CMakeDetermineCXXCompiler",
            "CMakeDetermineFortranCompiler",
            "CMakeDetermineJavaCompiler",
            "CMakeDetermineRCCompiler",
            "CMakeDetermineSystem",
            "CMakeDetermineVSServicePack",
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
            "CMakeImportBuildSettings",
            "CMakeJavaInformation",
            "CMakeJOMFindMake",
            "CMakeMinGWFindMake",
            "CMakeMSYSFindMake",
            "CMakeNMakeFindMake",
            "CMakeParseArguments",
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
            "CMakeVS71FindMake",
            "CPack",
            "CPackBundle",
            "CPackComponent",
            "CPackDeb",
            "CPackNSIS",
            "CPackRPM",
            "CPackZIP",
            "CTest",
            "CTestScriptMode",
            "CTestTargets",
            "Dart",
            "DeployQt4",
            "Documentation",
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
            "FindGLU",
            "FindGLUT",
            "FindGnuplot",
            "FindGnuTLS",
            "FindGTest",
            "FindGTK",
            "FindGTK2",
            "FindHDF5",
            "FindHSPELL",
            "FindHTMLHelp",
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
            "FindosgProducer",
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
        private static string[] _internalModules =
        {
            "CMakeDetermineCompilerABI",
            "CMakeDetermineCompilerId",
            "CMakeParseImplicitLinkInfo"
        };

        // Array of include files to be displayed.
        private List<string> _includeFiles;
        private string _sourceFilePath;

        public CMakeIncludeDeclarations(string sourceFilePath)
        {
            _sourceFilePath = sourceFilePath;
        }

        private void FindIncludeFiles()
        {
            // Find all *.cmake files in the same directory as the current source file,
            // excluding cmake_install.cmake, which is generated by CMake during
            // configuration.
            string dirPath = Path.GetDirectoryName(_sourceFilePath);
            IEnumerable<string> files = GetFilesFromDir(dirPath);
            _includeFiles = files.ToList();

            // Find all *.cmake files in the Modules directory inside the CMake
            // installation, if there is one.
            string pathToCMake = CMakePath.FindCMake();
            bool foundModules = false;
            if (pathToCMake != null)
            {
                try
                {
                    string pathToShare = Path.Combine(pathToCMake, "share");
                    IEnumerable<string> dirs = Directory.EnumerateDirectories(
                        pathToShare, "cmake-*.*");
                    foreach (string dir in dirs)
                    {
                        string pathToModules = Path.Combine(pathToShare, dir, "Modules");
                        if (Directory.Exists(pathToModules))
                        {
                            foundModules = true;
                            files = GetFilesFromDir(pathToModules, true);
                            _includeFiles.AddRange(files);
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    // This exception will occur if the CMake installation is missing
                    // expected subdirectories.  Proceed as if CMake had not been found.
                }
            }
            if (!foundModules)
            {
                // If we couldn't find modules in a CMake installation, show a default
                // hard-code listing.
                _includeFiles.AddRange(GetDefaultModules());
            }
            _includeFiles.Sort();
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
                _includeFiles = _includeFiles.Where(
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

        public override int GetCount()
        {
            if (_includeFiles == null)
            {
                FindIncludeFiles();
            }
            return _includeFiles.Count;
        }

        public override string GetDescription(int index)
        {
            return null;
        }

        public override string GetDisplayText(int index)
        {
            return GetFileName(index);
        }

        public override int GetGlyph(int index)
        {
            if (_includeFiles == null)
            {
                FindIncludeFiles();
            }

            // If the item is for a module, return the icon index for a module.
            // Otherwise, return the index for a call.
            if (index >= 0 && index < _includeFiles.Count &&
                _includeFiles[index].EndsWith(".cmake"))
            {
                return 208;
            }
            return 84;
        }

        public override string GetName(int index)
        {
            return GetFileName(index);
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

        private string GetFileName(int index)
        {
            if (_includeFiles == null)
            {
                FindIncludeFiles();
            }

            // The purpose of this function is to ensure that either GetName() or
            // GetDisplayText() can be overriden without affecting the behavior of the
            // other.
            if (index < 0 || index >= _includeFiles.Count)
            {
                return null;
            }
            return _includeFiles[index];
        }
    }
}
