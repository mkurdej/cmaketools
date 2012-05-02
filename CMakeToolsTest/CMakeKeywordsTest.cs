// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using CMakeTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMakeTools
{
    /// <summary>
    /// Tests to ensure the valadity of the data structure in CMakeKeywords.
    /// </summary>
    [TestClass]
    public class CMakeKeywordsTest
    {
        /// <summary>
        /// Test that a couple of the keyword identifiers match their corresponding
        /// keywords.
        /// </summary>
        [TestMethod]
        public void TestKeywordIds()
        {
            Assert.AreEqual(CMakeKeywords.GetKeywordId("add_custom_command"),
                CMakeKeywordId.AddCustomCommand);
            Assert.AreEqual(CMakeKeywords.GetKeywordId("while"),
                CMakeKeywordId.While);
        }

        /// <summary>
        /// Test that the commands are in alphabetical order.
        /// </summary>
        [TestMethod]
        public void TestCommandsAlphabetical()
        {
            for (int i = 1; i < (int)CMakeKeywordId.KeywordCount; i++)
            {
                string keyword1 = CMakeKeywords.GetKeywordFromId((CMakeKeywordId)(i - 1));
                string keyword2 = CMakeKeywords.GetKeywordFromId((CMakeKeywordId)i);
                Assert.IsTrue(keyword1.CompareTo(keyword2) < 0);
            }
        }

        /// <summary>
        /// Test that the command-specific keywords are in alphabetical order.
        /// </summary>
        [TestMethod]
        public void TestKeywordsAlphabetical()
        {
            for (int i = 0; i < (int)CMakeKeywordId.KeywordCount; i++)
            {
                string[] keywordArray = CMakeKeywords.GetKeywordsForCommand(
                    (CMakeKeywordId)i);
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
