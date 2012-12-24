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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Declarations objects for various types of member selection for CMake.
    /// </summary>
    class CMakeItemDeclarations : Declarations
    {
        /// <summary>
        /// Item types indicating which icon should be displayed next to an item.
        /// </summary>
        public enum ItemType
        {
            Command,
            Function,
            Macro,
            Variable,
            Property,
            Target,
            IncludeFile,
            Module,
            Language,
            Package,
            SourceFile
        }

        private struct Item : IComparable<Item>
        {
            public string Name { get; set; }
            public ItemType Type { get; set; }

            public int CompareTo(Item other)
            {
                // Perform a case-insensitive comparison of the names.
                return string.Compare(Name, other.Name, true);
            }
        }

        private List<Item> _items = new List<Item>();
        private bool _sorted;

        /// <summary>
        /// Add a single item.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <param name="type">The item type</param>
        public void AddItem(string name, ItemType type)
        {
            _items.Add(new Item() { Name = name, Type = type });
        }

        /// <summary>
        /// Add a collection of items.
        /// </summary>
        /// <param name="names">A collection of item names.</param>
        /// <param name="type">An item type.</param>
        public void AddItems(IEnumerable<string> names, ItemType type)
        {
            // Convert a list of names to a list of items of the specified type.
            _items.AddRange(names.Select(
                name => new Item() { Name = name, Type = type }));
            _sorted = false;
        }

        /// <summary>
        /// Remove a collection of items from the list.
        /// </summary>
        /// <param name="names">A collection of item names.</param>
        public void ExcludeItems(IEnumerable<string> names)
        {
            if (names != null)
            {
                List<string> sortedNames = names.ToList();
                sortedNames.Sort();
                _items.RemoveAll(x => sortedNames.BinarySearch(x.Name) >= 0);
            }
        }

        public override int GetCount()
        {
            return _items.Count;
        }

        public override string GetDescription(int index)
        {
            return null;
        }

        public override string GetDisplayText(int index)
        {
            return GetName(index);
        }

        public override int GetGlyph(int index)
        {
            // Return the appropriate icon index, depending on the item type.
            if (index < 0 || index >= _items.Count)
            {
                return -1;
            }
            EnsureSorted();
            switch (_items[index].Type)
            {
            case ItemType.Command:
                // Return the icon index for a keyword.
                return 206;
            case ItemType.Macro:
                // Return the icon index for a macro.
                return 54;
            case ItemType.Function:
                // Return the icon index for a public method.
                return 72;
            case ItemType.Variable:
                // Return the icon index for a public variable.
                return 138;
            case ItemType.Property:
                // Return the icon index for a public property.
                return 102;
            case ItemType.Target:
                // Return the icon index for a VC++ project.
                return 199;
            case ItemType.IncludeFile:
                // Return the icon index for a call.
                return 208;
            case ItemType.Module:
                // Return the icon index for a module
                return 84;
            case ItemType.Language:
                // Return the icon index for a reference.
                return 192;
            case ItemType.Package:
                // Return the icon index for a library.
                return 193;
            case ItemType.SourceFile:
                // Return the icon index for a snippet.  It's the closest thing to a file
                // that's available in the standard icon set.
                return 205;
            default:
                return -1;
            }
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return null;
            }
            EnsureSorted();
            return _items[index].Name;
        }

        private void EnsureSorted()
        {
            // Sort the items if they're not already sorted.
            if (!_sorted)
            {
                _items.Sort();
                _sorted = true;
            }
        }
    }
}
