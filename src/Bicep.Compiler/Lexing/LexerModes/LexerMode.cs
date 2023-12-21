// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing.LexerModes;

public abstract class LexerMode
{
    protected LexerMode(LexerBuffer buffer)
    {
        this.Buffer = buffer;
    }

    protected LexerBuffer Buffer { get; }

    public abstract Token Next(Stack<LexerMode> modes);
}
