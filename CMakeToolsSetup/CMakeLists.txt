# ****************************************************************************
# 
# Copyright (C) 2012-2014 by David Golub.  All rights reserved.
# 
# This software is subject to the Microsoft Reciprocal License (Ms-RL).
# A copy of the license can be found in the License.txt file included
# in this distribution.
# 
# You must not remove this notice, or any other, from this software.
# 
# ****************************************************************************

ADD_WIX_PROJECT(CMakeToolsSetup)
ADD_DEPENDENCIES(CMakeToolsSetup CMakeTools_VS10 CMakeTools_VS11 CMakeTools_VS12)
