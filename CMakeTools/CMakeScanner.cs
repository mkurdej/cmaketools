// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// Scanner for CMake code.
    /// </summary>
    class CMakeScanner : IScanner
    {
        private string _source;
        private int _offset;

        public void SetSource(string source, int offset)
        {
            _source = source;
            _offset = offset;
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if (GetStringFlag(state) && _offset < _source.Length)
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
                else if (_source[_offset] == '(')
                {
                    IncParenDepth(ref state);
                }
                else if (_source[_offset] == ')')
                {
                    DecParenDepth(ref state);
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
                    bool isKeyword = false;
                    if (!InsideParens(state))
                    {
                        isKeyword = CMakeKeywords.IsCommand(tokenText);
                    }
                    tokenInfo.Color = isKeyword ? TokenColor.Keyword :
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
                    SetStringFlag(ref state, false);
                    return;
                }
                _offset++;
            }

            // If we made it to the end of the string without hitting an unescaped
            // quotation mark, return a string token consisting of the rest of the line
            // and set the state to carry over onto the next line.
            _offset = _source.Length;
            tokenInfo.EndIndex = _source.Length - 1;
            SetStringFlag(ref state, true);
        }

        private bool GetStringFlag(int state)
        {
            // Get the flag indicating whether we're inside a string.
            return ((uint)state & 0x80000000) != 0;
        }

        private void SetStringFlag(ref int state, bool stringFlag)
        {
            // Set the flag indicating whether we're inside a string.
            uint unsignedState = (uint)state;
            if (stringFlag)
            {
                unsignedState |= 0x80000000;
            }
            else
            {
                unsignedState &= ~0x80000000;
            }
            state = (int)unsignedState;
        }

        private void IncParenDepth(ref int state)
        {
            // Increment the number of parentheses in which we're nested.
            int depth = state & 0x0FFFFFFF;
            state &= ~0x0FFFFFFF;
            state |= depth + 1;
        }

        private void DecParenDepth(ref int state)
        {
            // Decrement the number of parentheses in wihch we're nested.
            int depth = state & 0x0FFFFFFF;
            if (depth > 0)
            {
                state &= ~0x0FFFFFFF;
                state |= depth - 1;
            }
        }

        private bool InsideParens(int state)
        {
            // Check whether we're currently inside parentheses.
            int depth = state & 0x0FFFFFFF;
            return depth > 0;
        }
    }
}
