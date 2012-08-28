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
    /// Declarations object for CMake functions, macros, and commands.
    /// </summary>
    class CMakeFunctionDeclarations : Declarations
    {
        private enum ItemType
        {
            Command,
            Function,
            Macro
        }

        private struct Item : IComparable<Item>
        {
            public string Name { get; set; }
            public ItemType Type { get; set; }

            public int CompareTo(Item other)
            {
                // Perform a case-insensitive comparision of the names.
                return string.Compare(Name, other.Name, true);
            }
        }

        private List<Item> _items;

        public CMakeFunctionDeclarations(IEnumerable<string> functions,
            IEnumerable<string> macros)
        {
            _items = ConvertToItems(CMakeKeywords.GetAllCommands(),
                ItemType.Command).ToList();
            _items.AddRange(ConvertToItems(functions, ItemType.Function));
            _items.AddRange(ConvertToItems(macros, ItemType.Macro));
            _items.Sort();
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
            // Return the icon index for a keyword, function, or macro.
            if (index < 0 || index >= _items.Count)
            {
                return -1;
            }
            switch (_items[index].Type)
            {
            case ItemType.Command:
                return 206;
            case ItemType.Macro:
                return 54;
            case ItemType.Function:
            default:
                return 72;
            }
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return null;
            }
            return _items[index].Name;
        }

        private static IEnumerable<Item> ConvertToItems(IEnumerable<string> names,
            ItemType type)
        {
            // Convert a list of names to a list of items of the specified type.
            return names.Select(name => new Item() { Name = name, Type = type });
        }
    }
}
