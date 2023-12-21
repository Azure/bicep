// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.Compiler.Lexing;
using Bicep.Compiler.Lexing.LexerModes;

namespace Bicep.Compiler;

public class Lexer
{
    private readonly LexerBuffer buffer;

    private readonly Stack<LexerMode> modes;

    public Lexer(string text)
    {
        this.buffer = new(text);
        this.modes = new(new[]
        {
            new MainLexerMode(this.buffer),
        });
    }

    public Token Next() => this.modes.Peek().Next(this.modes);
}
