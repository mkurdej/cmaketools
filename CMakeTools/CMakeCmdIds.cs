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

namespace CMakeTools
{
    /// <summary>
    /// Command identifiers for commands defined by CMake Tools for Visual Studio.
    /// </summary>
    static class CMakeCmdIds
    {
        public const uint cmdidCMake = 0x0100;
        public const uint cmdidCMakeHelp = 0x0101;
        public const uint cmdidCMakeHelpCommands = 0x0102;
        public const uint cmdidCMakeHelpModules = 0x0103;
        public const uint cmdidCMakeHelpProperties = 0x0104;
        public const uint cmdidCMakeHelpVariables = 0x0105;
        public const uint cmdidCMakeHelpCPack = 0x0106;
        public const uint cmdidCMakeHelpCTest = 0x0107;
        public const uint cmdidCMakeHelpWebSite = 0x0108;
    }
}
