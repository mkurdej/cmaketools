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
        private bool _textFile;

        public CMakeScanner(bool textFile)
        {
            _textFile = textFile;
        }

        public void SetSource(string source, int offset)
        {
            _source = source;
            _offset = offset;
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if (_textFile)
            {
                // Don't perform syntax highlighting if the file is an ordinary text
                // file.
                return false;
            }

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
                    if (DecParenDepth(ref state))
                    {
                        SetLastKeyword(ref state, CMakeKeywordId.Unspecified);
                    }
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
                        SetLastKeyword(ref state, CMakeKeywords.GetKeywordId(tokenText));
                    }
                    else
                    {
                        isKeyword = CMakeKeywords.IsKeyword(GetLastKeyword(state),
                            tokenText);
                    }
                    tokenInfo.Color = isKeyword ? TokenColor.Keyword :
                        TokenColor.Identifier;
                    return true;
                }
                else if (_source[_offset] == '$')
                {
                    // Scan a variable token.
                    tokenInfo.StartIndex = _offset;
                    _offset++;
                    if (_source[_offset] == '{')
                    {
                        _offset++;
                        if (char.IsLetter(_source[_offset]) || _source[_offset] == '_')
                        {
                            _offset++;
                            while (char.IsLetterOrDigit(_source[_offset]) ||
                                _source[_offset] == '_')
                            {
                                _offset++;
                            }
                            if (_source[_offset] == '}')
                            {
                                _offset++;
                            }
                        }
                    }
                    tokenInfo.EndIndex = _offset - 1;
                    tokenInfo.Color = TokenColor.Identifier;
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

        // Masks, flags, and shifts to manipulate state values.
        private const uint StringFlag       = 0x80000000;
        private const int ParenDepthMask    = 0x0000FFFF;
        private const int LastKeywordMask   = 0x0FFF0000;
        private const int LastKeywordShift  = 16;

        private bool GetStringFlag(int state)
        {
            // Get the flag indicating whether we're inside a string.
            return ((uint)state & StringFlag) != 0;
        }

        private void SetStringFlag(ref int state, bool stringFlag)
        {
            // Set the flag indicating whether we're inside a string.
            uint unsignedState = (uint)state;
            if (stringFlag)
            {
                unsignedState |= StringFlag;
            }
            else
            {
                unsignedState &= ~StringFlag;
            }
            state = (int)unsignedState;
        }

        private void IncParenDepth(ref int state)
        {
            // Increment the number of parentheses in which we're nested.
            int depth = state & ParenDepthMask;
            state &= ~ParenDepthMask;
            state |= depth + 1;
        }

        private bool DecParenDepth(ref int state)
        {
            // Decrement the number of parentheses in which we're nested.
            int depth = state & ParenDepthMask;
            if (depth > 0)
            {
                state &= ~ParenDepthMask;
                state |= depth - 1;
            }
            return depth > 1;
        }

        private bool InsideParens(int state)
        {
            // Check whether we're currently inside parentheses.
            int depth = state & ParenDepthMask;
            return depth > 0;
        }

        private CMakeKeywordId GetLastKeyword(int state)
        {
            // Get the identifier of the last keyword from the state.
            int id = (state & LastKeywordMask) >> LastKeywordShift;
            return (CMakeKeywordId)id;
        }

        private void SetLastKeyword(ref int state, CMakeKeywordId id)
        {
            // Store the identifier of the last keyword scanned in the state.
            state &= ~LastKeywordMask;
            state |= ((int)id << LastKeywordShift) & LastKeywordMask;
        }
    }
}
