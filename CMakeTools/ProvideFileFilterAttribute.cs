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
using Microsoft.VisualStudio.Shell;

namespace CMakeTools
{
    /// <summary>
    /// Attribute to add a file filter to the File Open dialog box in Visual Studio.
    /// </summary>
    /// <remarks>
    /// This class is loosely based on the class with the same name in Python Tools for
    /// Visual Studio.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    class ProvideFileFilterAttribute : RegistrationAttribute
    {
        public ProvideFileFilterAttribute(object guid, string name, string filter,
            int sortPriority)
        {
            if (guid is string)
            {
                Guid = new Guid((string)guid);
            }
            else if (guid is Type)
            {
                Guid = ((Type)guid).GUID;
            }
            else if (guid is Guid)
            {
                Guid = (Guid)guid;
            }
            else
            {
                throw new ArgumentException();
            }
            Name = name;
            Filter = filter;
            SortPriority = sortPriority;
        }

        /// <summary>
        /// GUID of the project type with which this file filter is associated.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Name of this file filter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The filter string.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Sort priority of this file filter.
        /// </summary>
        public int SortPriority { get; private set; }

        private string GetKeyName()
        {
            return string.Format("Projects\\{0}\\Filters\\{1}", Guid.ToString("B"), Name);
        }

        public override void Register(RegistrationContext context)
        {
            using (Key key = context.CreateKey(GetKeyName()))
            {
                key.SetValue("", Filter);
                key.SetValue("SortPriority", SortPriority);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(GetKeyName());
        }
    }
}
