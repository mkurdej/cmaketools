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

        // Map from variable start tokens to their string representations.
        private static readonly Dictionary<CMakeToken, string> _varTokenMap =
            new Dictionary<CMakeToken, string>()
        {
            { CMakeToken.VariableStart,         "${" },
            { CMakeToken.VariableStartEnv,      "$ENV{" },
            { CMakeToken.VariableStartCache,    "$CACHE{" }
        };

        // Scanner state data
        private class ScanInfo
        {
            public bool stringFlag;
            public bool noSeparatorFlag;
            public bool needSubcommandFlag;
            public bool subcommandParmsFlag;
            public bool variableFlag;
            public int parenDepth;
            public int separatorCount;
            public CMakeCommandId lastCommand;
        }

        // List of scanner state data, one item for time the scanner state is
        // set back to zero.
        private List<ScanInfo> _scanInfoList;

        public CMakeScanner(bool textFile = false)
        {
            _textFile = textFile;
            _scanInfoList = new List<ScanInfo>();
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

            ScanInfo scanInfo = GetScanInfo(ref state);
            if (scanInfo.stringFlag && _offset < _source.Length)
            {
                // If the line begins inside a string token, begin by scanning the rest
                // of the string.
                ScanString(tokenInfo, scanInfo, false);
                _scannedNonWhitespace = true;
                _lastWhitespace = false;
                return true;
            }

            bool originalScannedNonWhitespace = _scannedNonWhitespace;
            bool originalLastWhitespace = _lastWhitespace;
            bool expectVariable = scanInfo.variableFlag;
            scanInfo.variableFlag = false;
            bool noSeparator = scanInfo.noSeparatorFlag;
            scanInfo.noSeparatorFlag = false;
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
                    CMakeCommandId id = scanInfo.lastCommand;
                    if (scanInfo.parenDepth > 0)
                    {
                        if (!noSeparator)
                        {
                            if (CMakeSubcommandMethods.HasSubcommands(id))
                            {
                                // The first whitespace token after a subcommand marks
                                // the beginning of the parameters.  The remaining
                                // whitespace parameters separate consecutive parameters.
                                if (scanInfo.subcommandParmsFlag)
                                {
                                    if (scanInfo.needSubcommandFlag)
                                    {
                                        scanInfo.needSubcommandFlag = false;
                                        tokenInfo.Trigger = TokenTriggers.ParameterStart;
                                    }
                                    else
                                    {
                                        tokenInfo.Trigger = TokenTriggers.ParameterNext;
                                    }
                                }
                            }
                            else if (id == CMakeCommandId.Unspecified ||
                                scanInfo.separatorCount > 0)
                            {
                                scanInfo.separatorCount--;
                                tokenInfo.Trigger = TokenTriggers.ParameterNext;
                            }
                        }
                        if (CMakeKeywords.TriggersMemberSelection(id) ||
                            CMakeKeywords.TriggersMemberSelectionOnWhiteSpace(id))
                        {
                            tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                        }
                    }
                    scanInfo.noSeparatorFlag = true;
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
                    scanInfo.noSeparatorFlag = noSeparator;
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
                    ScanString(tokenInfo, scanInfo, true);
                    return true;
                }
                else if (_source[_offset] == '(')
                {
                    // Scan an opening parenthesis.
                    if (scanInfo.parenDepth == 0)
                    {
                        CMakeCommandId id = scanInfo.lastCommand;
                        if (CMakeKeywords.TriggersMemberSelection(id))
                        {
                            tokenInfo.Trigger |= TokenTriggers.MemberSelect;
                            scanInfo.needSubcommandFlag = true;
                            scanInfo.noSeparatorFlag = true;
                        }
                        else
                        {
                            tokenInfo.Trigger |= TokenTriggers.ParameterStart;
                            scanInfo.noSeparatorFlag = true;
                        }
                    }
                    scanInfo.parenDepth++;
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
                    if (scanInfo.parenDepth > 0)
                    {
                        scanInfo.parenDepth--;
                    }
                    if (scanInfo.parenDepth == 0)
                    {
                        scanInfo.lastCommand = CMakeCommandId.Unspecified;
                        scanInfo.separatorCount = 0;
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

                    CMakeCommandId id = scanInfo.lastCommand;
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
                        scanInfo.variableFlag = true;
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
                        if (scanInfo.parenDepth == 0)
                        {
                            isKeyword = CMakeKeywords.IsCommand(tokenText);
                            scanInfo.lastCommand = CMakeKeywords.GetCommandId(
                                tokenText);
                            scanInfo.separatorCount = CMakeMethods.GetParameterCount(
                                scanInfo.lastCommand);
                            if (scanInfo.separatorCount > 0)
                            {
                                scanInfo.separatorCount--;
                            }
                        }
                        else
                        {
                            isKeyword = CMakeKeywords.IsKeyword(scanInfo.lastCommand,
                                tokenText);
                            if (isKeyword)
                            {
                                scanInfo.subcommandParmsFlag =
                                    CMakeSubcommandMethods.HasSubcommandParameters(
                                    scanInfo.lastCommand, tokenText);
                            }
                        }
                        tokenInfo.Color = isKeyword ? TokenColor.Keyword :
                            TokenColor.Identifier;
                        tokenInfo.Token = isKeyword ? (int)CMakeToken.Keyword :
                            (int)CMakeToken.Identifier;
                    }
                    if (tokenInfo.StartIndex == tokenInfo.EndIndex)
                    {
                        if (scanInfo.parenDepth == 0 && !originalScannedNonWhitespace)
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
                    CMakeToken varToken = _varTokenMap.FirstOrDefault(
                        x => _source.Substring(_offset).StartsWith(x.Value)).Key;
                    tokenInfo.StartIndex = _offset;
                    _offset++;
                    if (varToken != CMakeToken.Unspecified)
                    {
                        scanInfo.variableFlag = true;
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
                    _offset++;
                    return true;
                }
                _offset++;
            }
            return false;
        }

        private void ScanString(TokenInfo tokenInfo, ScanInfo scanInfo,
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
                    scanInfo.stringFlag = false;
                    return;
                }
                _offset++;
            }

            // If we made it to the end of the string without hitting an unescaped
            // quotation mark, return a string token consisting of the rest of the line
            // and set the state to carry over onto the next line.
            _offset = _source.Length;
            tokenInfo.EndIndex = _source.Length - 1;
            scanInfo.stringFlag = true;

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

        private ScanInfo GetScanInfo(ref int state)
        {
            if (state == 0)
            {
                _scanInfoList.Add(new ScanInfo());
                state = _scanInfoList.Count;
            }
            return _scanInfoList[state - 1];
        }

        public CMakeCommandId GetLastCommand(int state)
        {
            return GetScanInfo(ref state).lastCommand;
        }

        public bool InsideParens(int state)
        {
            return GetScanInfo(ref state).parenDepth > 0;
        }

        public bool GetStringFlag(int state)
        {
            return GetScanInfo(ref state).stringFlag;
        }
    }
}
