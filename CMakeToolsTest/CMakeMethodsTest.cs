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

using CMakeTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMakeTools
{
    /// <summary>
    /// Tests of CMake methods objects.
    /// </summary>
    [TestClass]
    public class CMakeMethodsTest
    {
        /// <summary>
        /// Test the correctness of Quick Info tips.
        /// </summary>
        [TestMethod]
        public void TestQuickInfoTips()
        {
            // Test ordinary commands.
            Assert.AreEqual("if(expression)",
                CMakeMethods.GetCommandQuickInfoTip(CMakeCommandId.If));
            Assert.AreEqual("set(variable value)",
                CMakeMethods.GetCommandQuickInfoTip(CMakeCommandId.Set));
            Assert.IsNull(CMakeMethods.GetCommandQuickInfoTip(
                CMakeCommandId.AddCustomCommand));

            // Test subcommands.
            Assert.AreEqual("cmake_policy(PUSH)",
                CMakeSubcommandMethods.GetSubcommandQuickInfoTip(
                    CMakeCommandId.CMakePolicy, "PUSH"));
            Assert.AreEqual("export(PACKAGE name)",
                CMakeSubcommandMethods.GetSubcommandQuickInfoTip(
                    CMakeCommandId.Export, "PACKAGE"));
            Assert.AreEqual("file(READ filename variable)",
                CMakeSubcommandMethods.GetSubcommandQuickInfoTip(
                    CMakeCommandId.File, "READ"));
            Assert.IsNull(CMakeSubcommandMethods.GetSubcommandQuickInfoTip(
                CMakeCommandId.Install, "CODE"));
        }
    }
}
