// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing.LexerModes;

public class StringLexerMode : LexerMode
{
    public StringLexerMode(LexerBuffer buffer)
        : base(buffer)
    {
    }

    public override Token Next(Stack<LexerMode> modes)
    {
        throw new NotImplementedException();
    }
}
