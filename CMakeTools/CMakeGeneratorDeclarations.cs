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
    /// Declarations object containing CMake generator expressions.
    /// </summary>
    class CMakeGeneratorDeclarations : CMakeItemDeclarations
    {
        // Array of CMake generator expressions.
        private static readonly string[] _generatorExpressions = new string[]
        {
            "AND",
            "ANGLE-R",
            "BUILD_INTERFACE",
            "COMMA",
            "BOOL",
            "C_COMPILER_ID",
            "C_COMPILER_VERSION",
            "CONFIG",
            "CONFIGURATION",
            "CXX_COMPILER_ID",
            "CXX_COMPILER_VERSION",
            "EQUAL",
            "INSTALL_INTERFACE",
            "INSTALL_PREFIX",
            "JOIN",
            "LOWER_CASE",
            "MAKE_C_IDENTIFIER",
            "OR",
            "NOT",
            "PLATFORM_ID",
            "SEMICOLON",
            "STREQUAL",
            "TARGET_FILE",
            "TARGET_FILE_DIR",
            "TARGET_FILE_NAME",
            "TARGET_LINKER_FILE_DIR",
            "TARGET_LINKER_FILE_NAME",
            "TARGET_NAME",
            "TARGET_POLICY",
            "TARGET_PROPERTY",
            "TARGET_SONAME_FILE",
            "TARGET_SONAME_FILE_DIR",
            "TARGET_SONAME_FILE_NAME",
            "UPPER_CASE",
            "VERSION_EQUAL",
            "VERSION_GREATER",
            "VERSION_LESS"
        };

        public CMakeGeneratorDeclarations()
        {
            AddItems(_generatorExpressions, ItemType.GeneratorExpression);
        }
    }
}
