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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace CMakeTools
{
    /// <summary>
    /// Type converter to handle conversions between enumeration values and a custom
    /// set of strings.
    /// </summary>
    /// <typeparam name="T">The numeration to be mapped to strings.</typeparam>
    class EnumToDisplayNameConverter<T> : TypeConverter
    {
        private Dictionary<T, string> _map;

        public EnumToDisplayNameConverter(Dictionary<T, string> map)
        {
            _map = map;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
            Type destinationType)
        {
            // Allow conversions to strings.
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            // Look in the map for a string and return it if found.
            if (destinationType == typeof(string))
            {
                T enumValue = (T)value;
                if (_map.ContainsKey(enumValue))
                {
                    return _map[enumValue];
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {
            // Allow conversions from string.
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            // Look in the map to see if there is an enumeration value corresponding
            // to the specified string.
            string valueStr = value as string;
            if (valueStr != null)
            {
                foreach (KeyValuePair<T, string> pair in _map)
                {
                    if (valueStr.Equals(pair.Value,
                        StringComparison.CurrentCultureIgnoreCase) ||
                        valueStr.Equals(pair.Key.ToString(),
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        return pair.Key;
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(
            ITypeDescriptorContext context)
        {
            // Display the strings in the list box.
            return new TypeConverter.StandardValuesCollection(_map.Keys);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            // Always return true to indicate that there is a set of strings to be
            // displayed in the list box.
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // Force the user to select a value from the list box, rather than typing in
            // a custom value.
            return true;
        }
    }
}
