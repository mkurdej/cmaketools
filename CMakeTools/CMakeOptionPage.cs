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
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace CMakeTools
{
    /// <summary>
    /// Option page to configure CMake Tools for Visual Studio.
    /// </summary>
    public class CMakeOptionPage : DialogPage
    {
        /// <summary>
        /// Values indicating which subdirectories to show in the IntelliSense list box.
        /// </summary>
        [TypeConverter(typeof(SubdirectorySettingConverter))]
        public enum SubdirectorySetting
        {
            /// <summary>
            /// Show all subdirectories.
            /// </summary>
            AllSubdirectories,

            /// <summary>
            /// Show only subdirectories containing a CMakeLists.txt file.
            /// </summary>
            CMakeListsOnly
        }

        // Custom type converter for SubdirectorySetting.
        private class SubdirectorySettingConverter
            : EnumToDisplayNameConverter<SubdirectorySetting>
        {
            private static Dictionary<SubdirectorySetting, string> _map =
                new Dictionary<SubdirectorySetting, string>()
            {
                { SubdirectorySetting.AllSubdirectories, CMakeStrings.AllSubdirectories },
                { SubdirectorySetting.CMakeListsOnly, CMakeStrings.CMakeListsOnly }
            };

            public SubdirectorySettingConverter()
                : base(_map) {}
        }

        /// <summary>
        /// Flag indicating whether to display command names in lowercase.
        /// </summary>
        [DisplayName("Commands In Lowercase")]
        [Description("Display CMake commands with lowercase letters in the " +
            "IntelliSense list box.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool CommandsLower { get; set; }

        /// <summary>
        /// Setting indicating whether to show all subdirectories or just those with
        /// CMakeLists.txt.
        /// </summary>
        [DisplayName("Show Subdirectories")]
        [Description("Configure whether to show all subdirectories in the " +
            "IntelliSense list box or only those containing a CMakeLists.txt file.")]
        [DefaultValue(SubdirectorySetting.AllSubdirectories)]
        public SubdirectorySetting ShowSubdirectories { get; set; }
    }
}
