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

using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Methods object containing parameter information for a user-defined function or
    /// macro.
    /// </summary>
    class CMakeUserMethods : Methods
    {
        private string _functionName;
        private List<string> _parameters;

        public CMakeUserMethods(string functionName, List<string> parameters)
        {
            _functionName = functionName;
            _parameters = parameters;
        }

        public override int GetCount()
        {
            // Only provide a single signature.
            return 1;
        }

        public override string GetDescription(int index)
        {
            return null;
        }

        public override string GetName(int index)
        {
            // Return the function name as typed by the user.
            return _functionName;
        }

        public override int GetParameterCount(int index)
        {
            // Return the number of parameters found from parsing.
            return _parameters.Count;
        }

        public override void GetParameterInfo(int index, int parameter, out string name,
            out string display, out string description)
        {
            // Return the name of the specified parameter found from parsing.
            name = _parameters[parameter];
            display = _parameters[parameter];
            description = null;
        }

        public override string GetType(int index)
        {
            // CMake commands don't have return types.
            return null;
        }

        public override string Delimiter
        {
	        get
	        {
                // In CMake, spaces instead of commas are used to separate arguments.
		        return " ";
	        }
        }
    }
}
