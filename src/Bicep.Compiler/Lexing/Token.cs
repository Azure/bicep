// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing;

public class Token
{
    public Token(TokenKind kind)
    {
        Kind = kind;
    }

    public TokenKind Kind { get; }
}
