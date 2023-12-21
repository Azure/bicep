// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing;

public struct LexerBuffer
{
    private readonly string text;

    private int offset;

    public LexerBuffer(string text)
    {
        this.text = text;
        this.offset = 0;
    }

    public readonly bool IsEmpty => this.offset == this.text.Length;

    public readonly char PeekOne() => this.text[offset];

    public readonly ReadOnlySpan<char> AsSpan() => this.text.AsSpan(this.offset);

    public void Advance(int n = 1) => this.offset += n;
}
