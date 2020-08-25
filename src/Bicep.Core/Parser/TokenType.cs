// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Parser
{
    public enum TokenType
    {
        Unrecognized,
        LeftBrace,
        RightBrace,
        LeftParen,
        RightParen,
        LeftSquare,
        RightSquare,
        Comma,
        Dot,
        Question,
        Colon,
        Semicolon,
        Assignment,
        Plus,
        Minus,
        Asterisk,
        Slash,
        Modulo,
        Exclamation,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equals,
        NotEquals,
        EqualsInsensitive,
        NotEqualsInsensitive,
        LogicalAnd,
        LogicalOr,
        Identifier,
        StringLeftPiece,
        StringMiddlePiece,
        StringRightPiece,
        StringComplete,
        Number,
        TrueKeyword,
        FalseKeyword,
        NullKeyword,
        NewLine,
        EndOfFile,
    }
}
