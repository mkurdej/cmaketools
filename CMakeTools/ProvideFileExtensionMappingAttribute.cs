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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    class ProvideFileExtensionMappingAttribute : RegistrationAttribute
    {
        public ProvideFileExtensionMappingAttribute(object guid, string name,
            object editorGuid, object package, int sortPriority)
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
            if (editorGuid is string)
            {
                EditorGuid = new Guid((string)editorGuid);
            }
            else if (editorGuid is Type)
            {
                EditorGuid = ((Type)editorGuid).GUID;
            }
            else if (editorGuid is Guid)
            {
                EditorGuid = (Guid)editorGuid;
            }
            else
            {
                throw new ArgumentException();
            }
            if (package is string)
            {
                Package = new Guid((string)package);
            }
            else if (package is Type)
            {
                Package = ((Type)package).GUID;
            }
            else if (package is Guid)
            {
                Package = (Guid)package;
            }
            else
            {
                throw new ArgumentException();
            }
            SortPriority = sortPriority;
        }

        /// <summary>
        /// GUID of this file extension mapping.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Display name of this file extension mapping.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// GUID of the editor factory associated with this file extension mapping.
        /// </summary>
        public Guid EditorGuid { get; private set; }

        /// <summary>
        /// GUID of the package associated with the file extension mapping.
        /// </summary>
        public Guid Package { get; private set; }

        /// <summary>
        /// Sort priority of the file extension mapping.
        /// </summary>
        public int SortPriority { get; private set; }

        private string GetKeyName()
        {
            return string.Format("FileExtensionMapping\\{0}", Guid.ToString("B"));
        }

        public override void Register(RegistrationContext context)
        {
            using (Key mappingKey = context.CreateKey(GetKeyName()))
            {
                mappingKey.SetValue("", Name);
                mappingKey.SetValue("DisplayName", Name);
                mappingKey.SetValue("EditorGuid", EditorGuid.ToString("B"));
                mappingKey.SetValue("Package", Package.ToString("B"));
                mappingKey.SetValue("SortPriority", SortPriority);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(GetKeyName());
        }
    }
}
