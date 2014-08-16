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

CMAKE_MINIMUM_REQUIRED(VERSION 2.8.12)

# Set the solution name.
PROJECT(CMakeTools)

# Detect the version of Visual Studio being used to build CMake Tools for Visual Studio.
IF(MSVC12)
    SET(TOOLS_VERSION 12.0)
ELSEIF(MSVC11)
    SET(TOOLS_VERSION 11.0)
ELSEIF(MSVC10)
    SET(TOOLS_VERSION 4.0)
ELSE()
    MESSAGE(
        FATAL_ERROR "This project must be generated using VS 2010, VS 2012, or VS 2013")
ENDIF()

# Function to configure a C# project for a single version of Visual Studio.
FUNCTION(ADD_CS_PROJECT PROJECT_NAME PROJECT_GUID VS_VERSION DOTNET_VERSION)
    FILE(TO_NATIVE_PATH ${CMAKE_SOURCE_DIR} NATIVE_SOURCE_DIR)
    CONFIGURE_FILE(
        ${CMAKE_CURRENT_SOURCE_DIR}/${PROJECT_NAME}.csproj.in
        ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}_VS${VS_VERSION}.csproj)
    INCLUDE_EXTERNAL_MSPROJECT(
        ${PROJECT_NAME}_VS${VS_VERSION}
        ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}_VS${VS_VERSION}.csproj
        TYPE {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC})
ENDFUNCTION(ADD_CS_PROJECT)

# Function to configure a C# project for all supported versions of Visual Studio.
FUNCTION(ADD_CS_PROJECT_ALL PROJECT_NAME)
    IF(MSVC10 OR MSVC11 OR MSVC12)
        LIST(GET ARGN 0 PROJECT_GUID)
        ADD_CS_PROJECT(${PROJECT_NAME} ${PROJECT_GUID} 10 4.0)
    ENDIF(MSVC10 OR MSVC11 OR MSVC12)
    IF(MSVC11 OR MSVC12)
        LIST(GET ARGN 1 PROJECT_GUID)
        ADD_CS_PROJECT(${PROJECT_NAME} ${PROJECT_GUID} 11 4.5)
    ENDIF(MSVC11 OR MSVC12)
    IF(MSVC12)
        LIST(GET ARGN 2 PROJECT_GUID)
        ADD_CS_PROJECT(${PROJECT_NAME} ${PROJECT_GUID} 12 4.5)
    ENDIF(MSVC12)
ENDFUNCTION(ADD_CS_PROJECT_ALL)

# Function to configure a Windows Installer XML project.
FUNCTION(ADD_WIX_PROJECT PROJECT_NAME)
    INCLUDE_EXTERNAL_MSPROJECT(
        ${PROJECT_NAME}
        ${CMAKE_CURRENT_SOURCE_DIR}/${PROJECT_NAME}.wixproj
        TYPE {930C7802-8A8C-48F9-8165-68863BCCD9DD})
ENDFUNCTION(ADD_WIX_PROJECT)

ADD_SUBDIRECTORY(CMakeTools)
ADD_SUBDIRECTORY(CMakeToolsTest)
ADD_SUBDIRECTORY(CMakeToolsSetup)
