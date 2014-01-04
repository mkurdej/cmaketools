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
    /// Tests to ensure the validity of the data structure in CMakeKeywords.
    /// </summary>
    [TestClass]
    public class CMakeKeywordsTest
    {
        /// <summary>
        /// Test that a couple of the command identifiers match their corresponding
        /// keywords.
        /// </summary>
        [TestMethod]
        public void TestKeywordIds()
        {
            Assert.AreEqual(CMakeCommandId.AddCustomCommand,
                CMakeKeywords.GetCommandId("add_custom_command"));
            Assert.AreEqual(CMakeCommandId.While,
                CMakeKeywords.GetCommandId("while"));
        }

        /// <summary>
        /// Test that the commands are in alphabetical order.
        /// </summary>
        [TestMethod]
        public void TestCommandsAlphabetical()
        {
            for (int i = 1; i < (int)CMakeCommandId.CommandCount; i++)
            {
                string keyword1 = CMakeKeywords.GetCommandFromId((CMakeCommandId)(i - 1));
                string keyword2 = CMakeKeywords.GetCommandFromId((CMakeCommandId)i);
                Assert.IsTrue(keyword1.CompareTo(keyword2) < 0);
            }
        }

        /// <summary>
        /// Test that the command-specific keywords are in alphabetical order.
        /// </summary>
        [TestMethod]
        public void TestKeywordsAlphabetical()
        {
            for (int i = 0; i < (int)CMakeCommandId.CommandCount; i++)
            {
                string[] keywordArray = CMakeKeywords.GetKeywordsForCommand(
                    (CMakeCommandId)i);
                if (keywordArray != null)
                {
                    for (int j = 1; j < keywordArray.Length; j++)
                    {
                        Assert.IsTrue(keywordArray[j - 1].CompareTo(keywordArray[j]) < 0);
                    }
                }
            }
        }
    }
}
