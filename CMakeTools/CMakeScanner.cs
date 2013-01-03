/* ****************************************************************************
 * 
 * Copyright (C) 2012-2013 by David Golub.  All rights reserved.
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
    /// CMake token types.
    /// </summary>
    public enum CMakeToken
    {
        WhiteSpace,
        String,
        Comment,
        Keyword,
        Identifier,
        NumericIdentifier,
        FileName,
        Variable,
        VariableStart,
        VariableStartEnv,
        VariableStartSetEnv,
        VariableEnd,
        OpenParen,
        CloseParen
    }

    /// <summary>
    /// Scanner for CMake code.
    /// </summary>
    public class CMakeScanner : IScanner
    {
        private string _source;
        private int _offset;
        private bool _textFile;

        // This flag indicates whether the scanner has yet scanned any non-whitespace
        // tokens on the current line.  Since it does not need to persist between lines,
        // it is safe (and indeed necessary) to store this value in a member variable
        // rather than in the scanner state.
        private bool _scannedNonWhitespace;

        // This flag indicates whether the last token scanned on the current line was a
        // whitespace token.
        private bool _lastWhitespace;

        public CMakeScanner(bool textFile = false)
        {
            _textFile = textFile;
        }

        public void SetSource(string source, int offset)
        {
            _source = source;
            _offset = offset;
            _scannedNonWhitespace = false;
            _lastWhitespace = false;
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
                _scannedNonWhitespace = true;
                _lastWhitespace = false;
                return true;
            }

            bool originalScannedNonWhitespace = _scannedNonWhitespace;
            bool originalLastWhitespace = _lastWhitespace;
            bool expectVariable = GetVariableFlag(state);
            SetVariableFlag(ref state, false);
            bool noSeparator = GetNoSeparatorFlag(state);
            SetNoSeparatorFlag(ref state, false);
            tokenInfo.Trigger = TokenTriggers.None;
            _lastWhitespace = false;
            while (_offset < _source.Length)
            {
                if (char.IsWhiteSpace(_source[_offset]))
                {
                    // Scan a whitespace token.
                    tokenInfo.StartIndex = _offset;
                    while (_offset < _source.Length &&
                        char.IsWhiteSpace(_source[_offset]))
                    {
                        _offset++;
                    }
                    tokenInfo.EndIndex = _offset - 1;
                    tokenInfo.Color = TokenColor.Text;
                    tokenInfo.Token = (int)CMakeToken.WhiteSpace;
                    CMakeCommandId id = GetLastCommand(state);
                    if (InsideParens(state))
                    {
                        if (!noSeparator)
                        {
                            if (CMakeSubcommandMethods.HasSubcommands(id))
                            {
                                // The first whitespace token after a subcommand marks
                                // the beginning of the parameters.  The remaining
                                // whitespace parameters separate consecutive parameters.
                                if (GetNeedSubcommandFlag(state))
                                {
                                    SetNeedSubcommandFlag(ref state, false);
                                    tokenInfo.Trigger = TokenTriggers.ParameterStart;
                                }
                                else
                                {
                                    tokenInfo.Trigger = TokenTriggers.ParameterNext;
                                }
                            }
                            else if (id == CMakeCommandId.Unspecified ||
                                DecSeparatorCount(ref state))
                            {
                                tokenInfo.Trigger = TokenTriggers.ParameterNext;
                            }
                        }
                        if (CMakeKeywords.TriggersMemberSelection(id) ||
                            CMakeKeywords.TriggersMemberSelectionOnWhiteSpace(id))
                        {
                            tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                        }
                    }
                    SetNoSeparatorFlag(ref state, true);
                    _lastWhitespace = true;
                    return true;
                }
                else if (_source[_offset] == '#')
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
                    SetNoSeparatorFlag(ref state, noSeparator);
                    return true;
                }

                // If we haven't returned by this point, the token is something other
                // than whitespace.  Therefore, set the flag indicating that a
                // non-whitespace token has been scanned on the current line so that
                // any additional identifier characters at the end won't trigger member
                // selection.  If you add any additional token, ensure that you handle
                // them after this point.
                _scannedNonWhitespace = true;

                if (_source[_offset] == '"')
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
                        CMakeCommandId id = GetLastCommand(state);
                        if (CMakeKeywords.TriggersMemberSelection(id))
                        {
                            tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                            SetNeedSubcommandFlag(ref state, true);
                            SetNoSeparatorFlag(ref state, true);
                        }
                        else
                        {
                            tokenInfo.Trigger |= TokenTriggers.ParameterStart;
                            SetNoSeparatorFlag(ref state, true);
                        }
                    }
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _offset;
                    tokenInfo.Color = TokenColor.Text;
                    tokenInfo.Token = (int)CMakeToken.OpenParen;
                    tokenInfo.Trigger |= TokenTriggers.MatchBraces;
                    _offset++;
                    return true;
                }
                else if (_source[_offset] == ')')
                {
                    // Scan a closing parenthesis.
                    if (!DecParenDepth(ref state))
                    {
                        SetLastCommand(ref state, CMakeCommandId.Unspecified);
                        tokenInfo.Trigger = TokenTriggers.ParameterEnd;
                    }
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _offset;
                    tokenInfo.Color = TokenColor.Text;
                    tokenInfo.Token = (int)CMakeToken.CloseParen;
                    tokenInfo.Trigger |= TokenTriggers.MatchBraces;
                    _offset++;
                    return true;
                }
                else if (char.IsLetter(_source[_offset]) || _source[_offset] == '_')
                {
                    // Scan a keyword, identifier, or file name token.
                    bool isFileName = false;
                    bool isNumeric = false;
                    tokenInfo.StartIndex = _offset;
                    while (_offset < _source.Length - 1)
                    {
                        char ch = _source[_offset + 1];
                        if (ch == '-')
                        {
                            // Variable names may contain hyphens but function names
                            // can't.  There classify an identifier with a hyphen in it
                            // as a numeric identifier.
                            isNumeric = true;
                        }
                        else if (!expectVariable && ScanFileNameChar())
                        {
                            isFileName = true;
                        }
                        else if (!char.IsLetterOrDigit(ch) && ch != '_')
                        {
                            break;
                        }
                        _offset++;
                    }
                    tokenInfo.EndIndex = _offset;
                    _offset++;

                    CMakeCommandId id = GetLastCommand(state);
                    string substr = _source.ExtractToken(tokenInfo);
                    if (expectVariable)
                    {
                        // If we're inside curly braces following a dollar sign, treat
                        // the identifier as a variable.
                        tokenInfo.Color = TokenColor.Identifier;
                        tokenInfo.Token = (int)CMakeToken.Variable;
                    }
                    else if ((id == CMakeCommandId.Set || id == CMakeCommandId.Unset) &&
                        substr.StartsWith("ENV{"))
                    {
                        // Inside a SET or UNSET command, ENV{ indicates an environment
                        // variable.  This token is case-sensitive.
                        SetVariableFlag(ref state, true);
                        tokenInfo.EndIndex = tokenInfo.StartIndex + 3;
                        tokenInfo.Color = TokenColor.Identifier;
                        tokenInfo.Token = (int)CMakeToken.VariableStartSetEnv;
                        _offset = tokenInfo.EndIndex + 1;
                    }
                    else if (isNumeric)
                    {
                        tokenInfo.Color = TokenColor.Identifier;
                        tokenInfo.Token = (int)CMakeToken.NumericIdentifier;
                    }
                    else if (isFileName)
                    {
                        // If we found characters that aren't valid in an identifier,
                        // treat the token as a file name.
                        tokenInfo.Color = TokenColor.Identifier;
                        tokenInfo.Token = (int)CMakeToken.FileName;
                    }
                    else
                    {
                        // Check whether the string is a keyword or not.
                        string tokenText = _source.ExtractToken(tokenInfo);
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
                    if (tokenInfo.StartIndex == tokenInfo.EndIndex)
                    {
                        if (!InsideParens(state) && !originalScannedNonWhitespace)
                        {
                            // Trigger member selection if we're not inside parentheses.
                            tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                        }
                        else if (!originalScannedNonWhitespace || originalLastWhitespace)
                        {
                            // Always trigger member selection in response to certain
                            // commands.
                            if (CMakeKeywords.TriggersMemberSelection(id) ||
                                CMakeKeywords.TriggersMemberSelectionOnWhiteSpace(id))
                            {
                                tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                            }
                        }
                    }
                    return true;
                }
                else if (char.IsDigit(_source[_offset]) || _source[_offset] == '-')
                {
                    // Variable names can start with numbers or hyphens in CMake, but
                    // function names can't.  We'll call these tokens "numeric
                    // identifiers" here and treat them accordingly when parsing.
                    tokenInfo.StartIndex = _offset;
                    bool isFileName = false;
                    while (_offset < _source.Length - 1)
                    {
                        char ch = _source[_offset + 1];
                        if (!expectVariable && ScanFileNameChar())
                        {
                            isFileName = true;
                        }
                        else if (!char.IsLetterOrDigit(ch) && ch != '_')
                        {
                            break;
                        }
                        _offset++;
                    }
                    tokenInfo.EndIndex = _offset;
                    _offset++;
                    tokenInfo.Color = TokenColor.Identifier;
                    if (expectVariable)
                    {
                        tokenInfo.Token = (int)CMakeToken.Variable;
                    }
                    else if (isFileName)
                    {
                        tokenInfo.Token = (int)CMakeToken.FileName;
                    }
                    else
                    {
                        tokenInfo.Token = (int)CMakeToken.NumericIdentifier;
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
                        SetVariableFlag(ref state, true);
                        tokenInfo.Token = (int)CMakeToken.VariableStart;
                        tokenInfo.Trigger = TokenTriggers.MemberSelect;
                        _offset++;
                    }
                    else if (_offset + 3 < _source.Length &&
                        _source.Substring(_offset, 4).Equals("ENV{"))
                    {
                        SetVariableFlag(ref state, true);
                        tokenInfo.Token = (int)CMakeToken.VariableStartEnv;
                        tokenInfo.Trigger = TokenTriggers.MemberSelect;
                        _offset += 4;
                    }
                    tokenInfo.EndIndex = _offset - 1;
                    tokenInfo.Color = TokenColor.Identifier;
                    return true;
                }
                else if (_source[_offset] == '}')
                {
                    // Scan a variable end token.
                    tokenInfo.StartIndex = _offset;
                    tokenInfo.EndIndex = _offset;
                    tokenInfo.Color = TokenColor.Identifier;
                    tokenInfo.Token = (int)CMakeToken.VariableEnd;
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
            tokenInfo.Token = (int)CMakeToken.String;
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

        private bool ScanFileNameChar()
        {
            // Attempt to scan a single character that may be valid in a file name but
            // not in an identifier.  Return true if such a character was found or false
            // otherwise.
            char ch = _source[_offset + 1];
            if (ch == '\\')
            {
                // Skip over escape sequences.
                _offset++;
                return true;
            }
            else if (ch == '~' || ch == '`' || ch == '!' || ch == '%' || ch == '^' ||
                ch == '&' || ch == '*' || ch == '+' || ch == '=' || ch == '[' ||
                ch == ']' || ch == '{' || ch == '}' || ch == ':' || ch == '\'' ||
                ch == ',' || ch == '.' || ch == '?' || ch == '/')
            {
                return true;
            }
            return false;
        }

        // Masks, flags, and shifts to manipulate state values.
        private const uint StringFlag           = 0x80000000;
        private const int VariableFlag          = 0x40000000;
        private const int NoSeparatorFlag       = 0x20000000;
        private const int NeedSubcommandFlag    = 0x10000000;
        private const int ParenDepthMask        = 0x000000FF;
        private const int SeparatorCountMask    = 0x0000FF00;
        private const int SeparatorCountShift   = 8;
        private const int LastCommandMask       = 0x0FFF0000;
        private const int LastCommandShift      = 16;

        public static bool GetStringFlag(int state)
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

        private bool GetNoSeparatorFlag(int state)
        {
            // Get the flag indicating that the next token should not be treated as a
            // parameter separator.
            return (state & NoSeparatorFlag) != 0;
        }

        private void SetNoSeparatorFlag(ref int state, bool noSeparatorFlag)
        {
            // Set the flag indicating that the next token should not be treated as a
            // parameter separator.
            if (noSeparatorFlag)
            {
                state |= NoSeparatorFlag;
            }
            else
            {
                state &= ~NoSeparatorFlag;
            }
        }

        private bool GetNeedSubcommandFlag(int state)
        {
            // Get the flag indicating that the current command takes subcommands.
            return (state & NeedSubcommandFlag) != 0;
        }

        private void SetNeedSubcommandFlag(ref int state, bool needSubcommand)
        {
            // Set the flag indicating that the current command takes subcommands.
            if (needSubcommand)
            {
                state |= NeedSubcommandFlag;
            }
            else
            {
                state &= ~NeedSubcommandFlag;
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
                state |= (depth - 1) & ParenDepthMask;
            }
            return depth > 1;
        }

        private static bool DecSeparatorCount(ref int state)
        {
            // Decrement the number of separators that are expected between the arguments
            // to the command.  Return false when it reaches zero to indicate that any
            // additional whitespace should not be treated as separators.
            int count = (state & SeparatorCountMask) >> SeparatorCountShift;
            if (count == 0)
            {
                return false;
            }
            state &= ~SeparatorCountMask;
            state |= ((count - 1) << SeparatorCountShift) & SeparatorCountMask;
            return true;
        }

        public static bool InsideParens(int state)
        {
            // Check whether we're currently inside parentheses.
            int depth = state & ParenDepthMask;
            return depth > 0;
        }

        public static CMakeCommandId GetLastCommand(int state)
        {
            // Get the identifier of the last command scanned from the state.
            int id = (state & LastCommandMask) >> LastCommandShift;
            if (id == 0x00000FFF)
            {
                return CMakeCommandId.Unspecified;
            }
            return (CMakeCommandId)id;
        }

        private void SetLastCommand(ref int state, CMakeCommandId id)
        {
            // Store the identifier of the last command scanned in the state.
            state &= ~(LastCommandMask | SeparatorCountMask);
            state |= ((int)id << LastCommandShift) & LastCommandMask;
            int separatorCount = CMakeMethods.GetParameterCount(id);
            if (separatorCount > 0)
            {
                separatorCount--;
            }
            state |= (separatorCount << SeparatorCountShift) & SeparatorCountMask;
        }
    }
}
