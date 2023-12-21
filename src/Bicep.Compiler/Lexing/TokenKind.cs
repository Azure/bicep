// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing;

public enum TokenKind : ushort
{
    // Punctuators
    OpenParen,
    CloseParen,
    OpenBrace,
    CloseBrace,
    OpenBracket,
    CloseBracket,
    Assignment,
    Comma,
    Plus,
    Minus,
    Asterisk,
    Slash,
    Colon,
    Exclamation,
    Question,
    LessThan,
    GreaterThan,
    And,
    Pipe,

    // Composite operators
    Equals = 100,
    NotEquals,
    LessThanOrEquals,
    GreaterThanOrEquals,
    EqualsInsensitive,
    NotEqualsInsensitive,
    LogicalAnd,
    LogicalOr,

    // String tokens
    StringStart = 200,
    StringEnd,
    StringInterpolationStart,
    StringInterpolationEnd,
    StringLiteralPart,
    MultilineString,

    // Reserved keywords
    NullKeyword = 300,
    TrueKeyword,
    FalseKeyword,

    Identifier = 400,

    EndOfFile = ushort.MaxValue,
}

