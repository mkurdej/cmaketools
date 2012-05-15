// CMake Tools for Visual Studio
// Copyright (C) 2012 by David Golub.
// All rights reserved.

using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// CMake token types.
    /// </summary>
    enum CMakeToken
    {
        String,
        Comment,
        Keyword,
        Identifier,
        Variable,
        VariableStart,
        VariableEnd,
        OpenParen,
        CloseParen
    }

    /// <summary>
    /// Scanner for CMake code.
    /// </summary>
    class CMakeScanner : IScanner
    {
        private string _source;
        private int _offset;
        private bool _textFile;

        public CMakeScanner(bool textFile = false)
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
            bool expectVariable = GetVariableFlag(state);
            SetVariableFlag(ref state, false);
            while (_offset < _source.Length)
            {
                if (_source[_offset] == '#')
                {
                    // Scan a comment token.
                    tokenInfo.StartIndex = _offset;
                    int endPos = _source.IndexOf('\n', _offset);
                    if (endPos > _offset)
                    {
                        while (endPos < _source.Length - 1 &&
                            (_source[endPos + 1] == '\r' || _source[endPos + 1] == '\n'))
                        {
                            endPos++;
                        }
                    }
                    else
                    {
                        endPos = _source.Length - 1;
                    }
                    tokenInfo.EndIndex = endPos;
                    tokenInfo.Color = TokenColor.Comment;
                    tokenInfo.Token = (int)CMakeToken.Comment;
                    _offset = endPos + 1;
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
                    // Scan an opening parenthesis.
                    if (!IncParenDepth(ref state))
                    {
                        if (CMakeKeywords.TriggersMemberSelection(GetLastCommand(state)))
                        {
                            tokenInfo.Trigger = TokenTriggers.MemberSelect;
                        }
                    }
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _offset;
                    tokenInfo.Color = TokenColor.Text;
                    tokenInfo.Token = (int)CMakeToken.OpenParen;
                    _offset++;
                    return true;
                }
                else if (_source[_offset] == ')')
                {
                    // Scan a closing parenthesis.
                    if (DecParenDepth(ref state))
                    {
                        SetLastCommand(ref state, CMakeCommandId.Unspecified);
                    }
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _offset;
                    tokenInfo.Color = TokenColor.Text;
                    tokenInfo.Token = (int)CMakeToken.CloseParen;
                    _offset++;
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

                    if (expectVariable)
                    {
                        // If we're inside curly braces following a dollar sign, treat
                        // the identifier as a variable.
                        tokenInfo.Color = TokenColor.Identifier;
                        tokenInfo.Token = (int)CMakeToken.Variable;
                    }
                    else
                    {
                        // Check whether the string is a keyword or not.
                        int length = tokenInfo.EndIndex - tokenInfo.StartIndex + 1;
                        string tokenText = _source.Substring(tokenInfo.StartIndex,
                            length);
                        bool isKeyword = false;
                        if (!InsideParens(state))
                        {
                            isKeyword = CMakeKeywords.IsCommand(tokenText);
                            SetLastCommand(ref state, CMakeKeywords.GetCommandId(
                                tokenText));
                        }
                        else
                        {
                            isKeyword = CMakeKeywords.IsKeyword(GetLastCommand(state),
                                tokenText);
                        }
                        tokenInfo.Color = isKeyword ? TokenColor.Keyword :
                            TokenColor.Identifier;
                        tokenInfo.Token = isKeyword ? (int)CMakeToken.Keyword :
                            (int)CMakeToken.Identifier;
                    }
                    return true;
                }
                else if (_source[_offset] == '$')
                {
                    // Scan a variable start token.
                    tokenInfo.StartIndex = _offset;
                    _offset++;
                    if (_offset < _source.Length && _source[_offset] == '{')
                    {
                        tokenInfo.Trigger = TokenTriggers.MemberSelect;
                        _offset++;
                    }
                    tokenInfo.EndIndex = _offset - 1;
                    tokenInfo.Color = TokenColor.Identifier;
                    tokenInfo.Token = (int)CMakeToken.VariableStart;
                    return true;
                }
                else if (_source[_offset] == '}')
                {
                    // Scan a variable end token.
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _offset;
                    tokenInfo.Color = TokenColor.Identifier;
                    _offset++;
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
        private const int VariableFlag      = 0x40000000;
        private const int ParenDepthMask    = 0x0000FFFF;
        private const int LastCommandMask   = 0x0FFF0000;
        private const int LastCommandShift  = 16;

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

        private bool GetVariableFlag(int state)
        {
            // Get the flag indicating whether we're expecting a variable.
            return (state & VariableFlag) != 0;
        }

        private void SetVariableFlag(ref int state, bool variableFlag)
        {
            // Set the flag indicating whether we're inside a variable.
            if (variableFlag)
            {
                state |= VariableFlag;
            }
            else
            {
                state &= ~VariableFlag;
            }
        }

        private bool IncParenDepth(ref int state)
        {
            // Increment the number of parentheses in which we're nested.
            int depth = state & ParenDepthMask;
            state &= ~ParenDepthMask;
            state |= depth + 1;
            return depth > 0;
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

        public static CMakeCommandId GetLastCommand(int state)
        {
            // Get the identifier of the last command scanned from the state.
            int id = (state & LastCommandMask) >> LastCommandShift;
            return (CMakeCommandId)id;
        }

        private void SetLastCommand(ref int state, CMakeCommandId id)
        {
            // Store the identifier of the last command scanned in the state.
            state &= ~LastCommandMask;
            state |= ((int)id << LastCommandShift) & LastCommandMask;
        }
    }
}
