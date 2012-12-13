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

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Simple declarations object to display a set of items for member selection all
    /// with the sample icon.
    /// </summary>
    class SimpleDeclarations : Declarations
    {
        private List<string> _items;
        private int _glyphIndex;

        public SimpleDeclarations(List<string> items, int glyphIndex)
        {
            _items = items;
            _glyphIndex = glyphIndex;
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
            return _glyphIndex;
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return null;
            }
            return _items[index];
        }
    }
}
