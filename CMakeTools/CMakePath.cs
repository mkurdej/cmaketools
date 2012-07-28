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
    }
}
