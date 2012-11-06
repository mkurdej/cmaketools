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

using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Expansion function to convert CMake command names to uppercase or lowercase,
    /// as specified by the user in the configuration.
    /// </summary>
    class ToCommandCaseExpansionFunction : ExpansionFunction
    {
        private bool _commandsLower;

        public ToCommandCaseExpansionFunction(bool commandsLower,
            ExpansionProvider provider) : base(provider)
        {
            _commandsLower = commandsLower;
        }

        public override string GetCurrentValue()
        {
            string commandName = GetArgument(0);
            if (_commandsLower)
            {
                return commandName.ToLower();
            }
            else
            {
                return commandName.ToUpper();
            }
        }
    }
}
