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
using System.ComponentModel;
using System.Globalization;
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

        private class SubdirectorySettingConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context,
                Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    switch ((SubdirectorySetting)value)
                    {
                    case SubdirectorySetting.AllSubdirectories:
                        return CMakeStrings.AllSubdirectories;
                    case SubdirectorySetting.CMakeListsOnly:
                        return CMakeStrings.CMakeListsOnly;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context,
                Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                CultureInfo culture, object value)
            {
                string valueStr = (string)value;
                if (valueStr.Equals(CMakeStrings.AllSubdirectories,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    return SubdirectorySetting.AllSubdirectories;
                }
                else if (valueStr.Equals(CMakeStrings.CMakeListsOnly,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    return SubdirectorySetting.CMakeListsOnly;
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(
                ITypeDescriptorContext context)
            {
                return new TypeConverter.StandardValuesCollection(new SubdirectorySetting[]
                {
                    SubdirectorySetting.AllSubdirectories,
                    SubdirectorySetting.CMakeListsOnly
                });
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        /// <summary>
        /// Flag indicating whether to display command names in lowercase.
        /// </summary>
        [DisplayName("Commands In Lowercase")]
        [Description("Display CMake commands with lowercase letters in the " +
            "IntelliSense list box.")]
        public bool CommandsLower { get; set; }

        /// <summary>
        /// Setting indicating whether to show all subdirectories or just those with
        /// CMakeLists.txt.
        /// </summary>
        [DisplayName("Show Subdirectories")]
        [Description("Configure whether to show all subdirectories in the " +
            "IntelliSense list box or only those containing a CMakeLists.txt file.")]
        public SubdirectorySetting ShowSubdirectories { get; set; }
    }
}
