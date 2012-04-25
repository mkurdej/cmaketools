// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using System;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Scanner for CMake code.
    /// </summary>
    class CMakeScanner : IScanner
    {
        // Array of CMake commands.
        private static string[] _keywords = new string[]
        {
            "add_custom_command",
            "add_custom_target",
            "add_definitions",
            "add_dependencies",
            "add_executable",
            "add_library",
            "add_subdirectory",
            "add_test",
            "aux_source_directory",
            "break",
            "build_command",
            "cmake_minimum_required",
            "cmake_policy",
            "configure_file",
            "create_test_sourcelist",
            "define_property",
            "else",
            "elseif",
            "enable_language",
            "enable_testing",
            "endforeach",
            "endfunction",
            "endif",
            "endmacro",
            "endwhile",
            "execute_process",
            "export",
            "file",
            "find_file",
            "find_library",
            "find_package",
            "find_path",
            "find_program",
            "fltk_wrap_ui",
            "foreach",
            "function",
            "get_cmake_property",
            "get_directory_property",
            "get_filename_component",
            "get_property",
            "get_source_file_property",
            "get_target_property",
            "get_test_property",
            "if",
            "include",
            "include_directories",
            "include_external_msproject",
            "include_regular_expression",
            "install",
            "link_directories",
            "list",
            "load_cache",
            "load_command",
            "macro",
            "mark_as_advanced",
            "math",
            "message",
            "option",
            "project",
            "qt_wrap_cpp",
            "qt_wrap_ui",
            "remove_definitions",
            "return",
            "separate_arguments",
            "set",
            "set_directory_properties",
            "set_property",
            "set_source_files_properties",
            "set_target_properties",
            "set_tests_properties",
            "site_name",
            "source_group",
            "string",
            "target_link_libraries",
            "try_compile",
            "try_run",
            "unset",
            "variable_watch",
            "while"
        };

        private string _source;
        private int _offset;

        public void SetSource(string source, int offset)
        {
            _source = source;
            _offset = offset;
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if (state == 1 && _offset < _source.Length)
            {
                // If the line begins inside a string token, begin by scanning the rest
                // of the string.
                ScanString(tokenInfo, ref state, false);
                return true;
            }
            while (_offset < _source.Length)
            {
                if (_source[_offset] == '#')
                {
                    // Scan a comment token.
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _source.Length - 1;
                    tokenInfo.Color = TokenColor.Comment;
                    _offset = _source.Length;
                    return true;
                }
                else if (_source[_offset] == '"')
                {
                    // Scan a string token.
                    ScanString(tokenInfo, ref state, true);
                    return true;
                }
                else if (char.IsLetter(_source[_offset]) || _source[_offset] == '_')
                {
                    // Scan a keyword or identifier token.
                    tokenInfo.StartIndex = _offset;
                    while (_offset < _source.Length - 1)
                    {
                        char ch = _source[_offset + 1];
                        if (!char.IsLetterOrDigit(ch) && ch != '_')
                        {
                            break;
                        }
                        _offset++;
                    }
                    tokenInfo.EndIndex = _offset;
                    _offset++;

                    // Check whether the string is a keyword or not.
                    int length = tokenInfo.EndIndex - tokenInfo.StartIndex + 1;
                    string tokenText = _source.Substring(tokenInfo.StartIndex, length);
                    int index = Array.BinarySearch(_keywords, tokenText.ToLower());
                    tokenInfo.Color = index >= 0 ? TokenColor.Keyword :
                        TokenColor.Identifier;
                    return true;
                }
                _offset++;
            }
            return false;
        }

        private void ScanString(TokenInfo tokenInfo, ref int state, bool startWithQuote)
        {
            tokenInfo.StartIndex = _offset;
            tokenInfo.Color = TokenColor.String;
            if (startWithQuote)
            {
                // If the string token began with a quotation mark (as opposed to
                // carrying over from a string on the previous line), advance past it.
                _offset++;
            }
            while (_offset < _source.Length)
            {
                if (_source[_offset] == '\\' && _offset != _source.Length - 1)
                {
                    // Skip over escape sequences
                    _offset++;
                }
                else if (_source[_offset] == '"')
                {
                    // An unescaped quotation mark signals the end of the string.
                    tokenInfo.EndIndex = _offset;
                    _offset++;
                    state = 0;
                    return;
                }
                _offset++;
            }

            // If we made it to the end of the string without hitting an unescaped
            // quotation mark, return a string token consisting of the rest of the line
            // and set the state to carry over onto the next line.
            _offset = _source.Length;
            tokenInfo.EndIndex = _source.Length - 1;
            state = 1;
        }
    }
}
