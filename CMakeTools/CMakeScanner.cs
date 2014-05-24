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
using System.Linq;
using Microsoft.VisualStudio.Package;

namespace CMakeTools
{
    /// <summary>
    /// CMake token types.
    /// </summary>
    public enum CMakeToken
    {
        Unspecified,
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
        VariableStartCache,
        VariableStartSetEnv,
        VariableEnd,
        OpenParen,
        CloseParen,
        BracketArgument
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

        // Map from variable start tokens to their string representations.
        private static readonly Dictionary<CMakeToken, string> _varTokenMap =
            new Dictionary<CMakeToken, string>()
        {
            { CMakeToken.VariableStart,         "${" },
            { CMakeToken.VariableStartEnv,      "$ENV{" },
            { CMakeToken.VariableStartCache,    "$CACHE{" }
        };

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
            bool bracketComment = GetBracketCommentFlag(state);
            bool bracketArgument = GetBracketArgumentFlag(state);
            if ((bracketComment || bracketArgument) && _offset < _source.Length)
            {
                // If the line begins inside a bracket comment token, begin by scanning
                // the rest of the bracket comment.
                ScanBracketCommentOrArgument(tokenInfo, bracketComment, ref state);
                _lastWhitespace = true;
                return true;
            }

            bool originalScannedNonWhitespace = _scannedNonWhitespace;
            bool originalLastWhitespace = _lastWhitespace;
            int originalVariableDepth = GetVariableDepth(state);
            SetVariableDepth(ref state, 0);
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
                                if (GetSubcommandParmsFlag(state))
                                {
                                    if (GetNeedSubcommandFlag(state))
                                    {
                                        SetNeedSubcommandFlag(ref state, false);
                                        tokenInfo.Trigger = TokenTriggers.ParameterStart;
                                    }
                                    else if (!originalLastWhitespace)
                                    {
                                        tokenInfo.Trigger = TokenTriggers.ParameterNext;
                                    }
                                }
                            }
                            else if (id == CMakeCommandId.Unspecified ||
                                GetSeparatorCount(state) > 0)
                            {
                                if (!originalLastWhitespace)
                                {
                                    SetSeparatorCount(ref state,
                                        GetSeparatorCount(state) - 1);
                                    tokenInfo.Trigger = TokenTriggers.ParameterNext;
                                }
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
                    // Check if it's a bracket comment.  If so, handle it specially.
                    if (_offset + 1 < _source.Length && _source[_offset + 1] == '[')
                    {
                        int i = _offset + 2;
                        while (i < _source.Length && _source[i] == '=')
                        {
                            i++;
                        }
                        if (i < _source.Length && _source[i] == '[')
                        {
                            SetBracketCommentFlag(ref state, true);
                            ScanBracketCommentOrArgument(tokenInfo, true, ref state);
                            _lastWhitespace = true;
                            return true;
                        }
                    }

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
                    if (!InsideParens(state))
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
                    IncParenDepth(ref state);
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
                    DecParenDepth(ref state);
                    if (GetParenDepth(state) == 0)
                    {
                        SetLastCommand(ref state, CMakeCommandId.Unspecified);
                        SetSeparatorCount(ref state, 0);
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
                else if (char.IsLetter(_source[_offset]) || _source[_offset] == '_' ||
                    (originalVariableDepth == 0 && _source[_offset] == '\\'))
                {
                    // Scan a keyword, identifier, or file name token.
                    bool isFileName = false;
                    bool isNumeric = false;
                    tokenInfo.StartIndex = _offset;
                    if (_source[_offset] == '\\' && _offset != _source.Length - 1)
                    {
                        // If the identifier starts with an escape sequence, skip over it.
                        _offset++;
                        isFileName = true;
                    }
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
                        else if (originalVariableDepth == 0 && ScanFileNameChar())
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
                    if (originalVariableDepth != 0)
                    {
                        // If we're inside curly braces following a dollar sign, treat
                        // the identifier as a variable.
                        tokenInfo.Color = TokenColor.Identifier;
                        tokenInfo.Token = (int)CMakeToken.Variable;
                        SetVariableDepth(ref state, originalVariableDepth);
                    }
                    else if ((id == CMakeCommandId.Set || id == CMakeCommandId.Unset) &&
                        substr.StartsWith("ENV{"))
                    {
                        // Inside a SET or UNSET command, ENV{ indicates an environment
                        // variable.  This token is case-sensitive.
                        SetVariableDepth(ref state, originalVariableDepth + 1);
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
                            id = CMakeKeywords.GetCommandId(tokenText);
                            SetLastCommand(ref state, id);
                            SetSeparatorCount(ref state,
                                CMakeMethods.GetParameterCount(id));
                            int count = GetSeparatorCount(state);
                            if (count > 0)
                            {
                                SetSeparatorCount(ref state, count - 1);
                            }
                        }
                        else
                        {
                            isKeyword = CMakeKeywords.IsKeyword(id, tokenText);
                            if (isKeyword)
                            {
                                SetSubcommandParmsFlag(ref state,
                                    CMakeSubcommandMethods.HasSubcommandParameters(
                                    id, tokenText));
                            }
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
                        if (originalVariableDepth == 0 && ScanFileNameChar())
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
                    if (originalVariableDepth != 0)
                    {
                        tokenInfo.Token = (int)CMakeToken.Variable;
                        SetVariableDepth(ref state, originalVariableDepth);
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
                    CMakeToken varToken = _varTokenMap.FirstOrDefault(
                        x => _source.Substring(_offset).StartsWith(x.Value)).Key;
                    tokenInfo.StartIndex = _offset;
                    _offset++;
                    if (varToken != CMakeToken.Unspecified)
                    {
                        SetVariableDepth(ref state,  originalVariableDepth + 1);
                        tokenInfo.Token = (int)varToken;
                        tokenInfo.Trigger =
                            TokenTriggers.MemberSelect | TokenTriggers.MatchBraces;
                        _offset += _varTokenMap[varToken].Length - 1;
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
                    tokenInfo.Trigger = TokenTriggers.MatchBraces;
                    SetVariableDepth(ref state, originalVariableDepth > 0 ?
                        originalVariableDepth - 1 : 0);
                    _offset++;
                    return true;
                }
                else if (_source[_offset] == '[')
                {
                    // Scan a bracket argument, if it is one.
                    int i = _offset + 1;
                    while (i < _source.Length && _source[i] == '=')
                    {
                        i++;
                    }
                    if (i < _source.Length && _source[i] == '[')
                    {
                        SetBracketArgumentFlag(ref state, true);
                        ScanBracketCommentOrArgument(tokenInfo, false, ref state);
                        _lastWhitespace = false;
                        return true;
                    }
                }
                _offset++;
            }
            return false;
        }

        private void ScanString(TokenInfo tokenInfo, ref int state,
            bool startWithQuote)
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

            // If the user has begun to type a reference to a variable inside the string,
            // trigger member selection to show a list of variables.
            string tokenText = _source.ExtractToken(tokenInfo);
            CMakeToken varToken = _varTokenMap.FirstOrDefault(
                x => tokenText.EndsWith(x.Value)).Key;
            if (varToken != CMakeToken.Unspecified)
            {
                // Don't trigger member selection if the dollar sign is preceded by an
                // escape character, unless the escape character is itself escaped.
                int pos = tokenText.Length - _varTokenMap[varToken].Length - 1;
                int escapeCount = 0;
                while (pos > 0 && tokenText[pos] == '\\')
                {
                    escapeCount++;
                    pos--;
                }
                if (escapeCount % 2 == 0)
                {
                    tokenInfo.Trigger = TokenTriggers.MemberSelect;
                }
            }
        }

        private void ScanBracketCommentOrArgument(TokenInfo tokenInfo, bool isComment,
            ref int state)
        {
            // Scan until reaching the end of the bracket comment, delimited by a
            // closing square bracket, followed by zero or more equals sign, followed by
            // another closing square bracket.
            tokenInfo.StartIndex = _offset;
            tokenInfo.Color = isComment ? TokenColor.Comment : TokenColor.String;
            tokenInfo.Token = isComment ? (int)CMakeToken.Comment :
                (int)CMakeToken.BracketArgument;
            bool hasFirstBracket = false;
            while (_offset < _source.Length)
            {
                switch (_source[_offset])
                {
                case ']':
                    if (!hasFirstBracket)
                    {
                        hasFirstBracket = true;
                    }
                    else
                    {
                        tokenInfo.EndIndex = _offset;
                        _offset++;
                        if (isComment)
                        {
                            SetBracketCommentFlag(ref state, false);
                        }
                        else
                        {
                            SetBracketArgumentFlag(ref state, false);
                        }
                        return;
                    }
                    break;
                case '=':
                    break;
                default:
                    hasFirstBracket = false;
                    break;
                }
                _offset++;
            }
            tokenInfo.EndIndex = _source.Length - 1;
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
                if (_offset + 2 < _source.Length)
                {
                    _offset++;
                }
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

        private const uint StringFlag            = 0x80000000;
        private const int NoSeparatorFlag        = 0x40000000;
        private const int NeedSubcommandFlag     = 0x20000000;
        private const int SubcommandParmsFlag    = 0x10000000;
        private const int BracketCommentFlag     = 0x08000000;
        private const int BracketArgumentFlag    = 0x04000000;
        private const int VariableDepthMask      = 0x00F00000;
        private const int ParenDepthMask         = 0x000F0000;
        private const int SeparatorCountMask     = 0x0000F000;
        private const int LastCommandMask        = 0x00000FFF;
        private const int VariableDepthShift     = 20;
        private const int ParenDepthShift        = 16;
        private const int SeparatorCountShift    = 12;

        public static CMakeCommandId GetLastCommand(int state)
        {
            int id = state & LastCommandMask;
            if (id == LastCommandMask)
            {
                return CMakeCommandId.Unspecified;
            }
            return (CMakeCommandId)id;
        }

        private static void SetLastCommand(ref int state, CMakeCommandId id)
        {
            state &= ~LastCommandMask;
            state |= ((int)id & LastCommandMask);
        }

        private static int GetSeparatorCount(int state)
        {
            return (state & SeparatorCountMask) >> SeparatorCountShift;
        }
        
        private static void SetSeparatorCount(ref int state, int count)
        {
            state &= ~SeparatorCountMask;
            state |= (count << SeparatorCountShift) & SeparatorCountMask;
        }

        private static int GetParenDepth(int state)
        {
            return (state & ParenDepthMask) >> ParenDepthShift;
        }

        private static void IncParenDepth(ref int state)
        {
            int depth = GetParenDepth(state);
            state &= ~ParenDepthMask;
            state |= ((depth + 1) << ParenDepthShift) & ParenDepthMask;
        }

        private static void DecParenDepth(ref int state)
        {
            int depth = GetParenDepth(state);
            if (depth > 0)
            {
                state &= ~ParenDepthMask;
                state |= ((depth - 1) << ParenDepthShift) & ParenDepthMask;
            }
        }

        public static bool InsideParens(int state)
        {
            return GetParenDepth(state) > 0;
        }

        private static int GetVariableDepth(int state)
        {
            return (state & VariableDepthMask) >> VariableDepthShift;
        }

        private static void SetVariableDepth(ref int state, int depth)
        {
            state &= ~VariableDepthMask;
            state |= (depth << VariableDepthShift) & VariableDepthMask;
        }

        public static bool GetBracketCommentFlag(int state)
        {
            return (state & BracketCommentFlag) != 0;
        }

        private static void SetBracketCommentFlag(ref int state, bool flag)
        {
            if (flag)
            {
                state |= BracketCommentFlag;
            }
            else
            {
                state &= ~BracketCommentFlag;
            }
        }

        public static bool GetBracketArgumentFlag(int state)
        {
            return (state & BracketArgumentFlag) != 0;
        }

        private static void SetBracketArgumentFlag(ref int state, bool flag)
        {
            if (flag)
            {
                state |= BracketArgumentFlag;
            }
            else
            {
                state &= ~BracketArgumentFlag;
            }
        }

        private static bool GetSubcommandParmsFlag(int state)
        {
            return (state & SubcommandParmsFlag) != 0;
        }

        private static void SetSubcommandParmsFlag(ref int state, bool flag)
        {
            if (flag)
            {
                state |= SubcommandParmsFlag;
            }
            else
            {
                state &= ~SubcommandParmsFlag;
            }
        }

        private static bool GetNeedSubcommandFlag(int state)
        {
            return (state & NeedSubcommandFlag) != 0;
        }
        
        private static void SetNeedSubcommandFlag(ref int state, bool flag)
        {
            if (flag)
            {
                state |= NeedSubcommandFlag;
            }
            else
            {
                state &= ~NeedSubcommandFlag;
            }
        }

        private static bool GetNoSeparatorFlag(int state)
        {
            return (state & NoSeparatorFlag) != 0;
        }
        
        private static void SetNoSeparatorFlag(ref int state, bool flag)
        {
            if (flag)
            {
                state |= NoSeparatorFlag;
            }
            else
            {
                state &= ~NoSeparatorFlag;
            }
        }

        public static bool GetStringFlag(int state)
        {
            return (state & StringFlag) != 0;
        }

        private static void SetStringFlag(ref int state, bool flag)
        {
            uint unsignedState = (uint)state;
            if (flag)
            {
                unsignedState |= StringFlag;
            }
            else
            {
                unsignedState &= ~StringFlag;
            }
            state = (int)unsignedState;
        }
    }
}
