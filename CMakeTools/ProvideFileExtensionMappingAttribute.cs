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
using Microsoft.VisualStudio.Shell;

namespace CMakeTools
{
    /// <summary>
    /// Attribute to allow to user to map new file extensions to the specified editor
    /// from the Options dialog box in Visual Studio.
    /// </summary>
    /// <remarks>
    /// This class is loosely based on the class with the same name in Python Tools for
    /// Visual Studio.
    /// </remarks>
    class ProvideFileExtensionMappingAttribute : RegistrationAttribute
    {
        private readonly Guid _guid;
        private readonly string _name;
        private readonly Guid _editorGuid;
        private readonly Guid _package;
        private readonly int _sortPriority;

        public ProvideFileExtensionMappingAttribute(object guid, string name,
            object editorGuid, object package, int sortPriority)
        {
            _guid = guid is Type ? ((Type)guid).GUID : new Guid(guid.ToString());
            _name = name;
            _editorGuid = editorGuid is Type ? ((Type)editorGuid).GUID :
                new Guid(editorGuid.ToString());
            _package = package is Type ? ((Type)package).GUID :
                new Guid(package.ToString());
            _sortPriority = sortPriority;
        }

        private string GetKeyName()
        {
            return string.Format("FileExtensionMapping\\{0}", _guid.ToString("B"));
        }

        public override void Register(RegistrationContext context)
        {
            using (Key mappingKey = context.CreateKey(GetKeyName()))
            {
                mappingKey.SetValue("", _name);
                mappingKey.SetValue("DisplayName", _name);
                mappingKey.SetValue("EditorGuid", _editorGuid.ToString("B"));
                mappingKey.SetValue("Package", _package.ToString("B"));
                mappingKey.SetValue("SortPriority", _sortPriority);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(GetKeyName());
        }
    }
}
