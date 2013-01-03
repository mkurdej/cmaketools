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
    class ProvideFileFilterAttribute : RegistrationAttribute
    {
        private readonly string _guid;
        private readonly string _name;
        private readonly string _filter;
        private readonly int _sortPriority;

        public ProvideFileFilterAttribute(string guid, string name, string filter,
            int sortPriority)
        {
            _guid = guid;
            _name = name;
            _filter = filter;
            _sortPriority = sortPriority;
        }

        private string GetKeyName()
        {
            return string.Format("Projects\\{0}\\Filters\\{1}", _guid, _name);
        }

        public override void Register(RegistrationContext context)
        {
            using (Key key = context.CreateKey(GetKeyName()))
            {
                key.SetValue("", _filter);
                key.SetValue("SortPriority", _sortPriority);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(GetKeyName());
        }
    }
}
