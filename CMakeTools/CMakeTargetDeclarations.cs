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
    class CMakeTargetDeclarations : Declarations
    {
        private List<string> _targets;

        public CMakeTargetDeclarations(List<string> targets)
        {
            _targets = targets;
        }

        public override int GetCount()
        {
            return _targets.Count;
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
            // Always return the icon for a VC++ project.
            return 199;
        }

        public override string GetName(int index)
        {
            if (index < 0 || index >= _targets.Count)
            {
                return null;
            }
            return _targets[index];
        }
    }
}
