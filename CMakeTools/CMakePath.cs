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
using System.IO;
using Microsoft.Win32;

namespace CMakeTools
{
    /// <summary>
    /// Helper class to find the path to CMake.
    /// </summary>
    static class CMakePath
    {
        /// <summary>
        /// Attempt to find a CMake installation in the registry.
        /// </summary>
        /// <returns>
        /// The path to the CMake installation, or null if CMake is not installed.
        /// </returns>
        public static string FindCMake()
        {
            // First try the location manually configured by the user.
            if (CMakePackage.Instance != null)
            {
                string customPath = CMakePackage.Instance.CMakeOptionPage.PathToCMake;
                if (customPath != null &&
                    File.Exists(Path.Combine(customPath, "bin\\cmake.exe")))
                {
                    return customPath;
                }
            }

            // If CMake couldn't be found at a manually configured path, look for it in
            // the registry.
            string location = null;
            RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                RegistryView.Registry32);
            RegistryKey kitwareKey = localMachine.OpenSubKey("Software\\Kitware");
            if (kitwareKey != null)
            {
                foreach (string keyName in kitwareKey.GetSubKeyNames())
                {
                    if (!keyName.StartsWith("CMake"))
                    {
                        continue;
                    }
                    RegistryKey cmakeKey = kitwareKey.OpenSubKey(keyName);
                    if (cmakeKey.GetValueKind(null) == RegistryValueKind.String)
                    {
                        location = cmakeKey.GetValue(null) as string;
                        if (location != null)
                        {
                            break;
                        }
                    }
                }
            }
            return location;
        }

        /// <summary>
        /// Attempt to find the path to the CMake documentation.
        /// </summary>
        /// <returns>
        /// The path to the CMake documentation, or null if CMake is not installed.
        /// </returns>
        public static string FindCMakeHelp()
        {
            string pathToCMake = FindCMake();
            if (pathToCMake != null)
            {
                try
                {
                    string pathToDoc = Path.Combine(pathToCMake, "doc");
                    IEnumerable<string> dirs = Directory.EnumerateDirectories(pathToDoc,
                        "cmake-*.*");
                    foreach (string dir in dirs)
                    {
                        return Path.Combine(pathToDoc, dir);
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    // This exception will occur if the CMake installation is missing
                    // expected subdirectories.  Proceed as if CMake had not been found.
                }
            }
            return null;
        }

        /// <summary>
        /// Attempt to find the path to the CMake modules.
        /// </summary>
        /// <returns>
        /// The path to the CMake modules, or null if CMake is not installed.
        /// </returns>
        public static string FindCMakeModules()
        {
            string pathToCMake = FindCMake();
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
                            return pathToModules;
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    // This exception will occur if the CMake installation is missing
                    // expected subdirectories.  Proceed as if CMake had not been found.
                }
            }
            return null;
        }
    }
}
