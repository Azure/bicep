// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;

namespace Bicep.Core.UnitTests.Utils;

/// <summary>
/// A byte array for data that is intended to represent UTF-8 text (but is still stored as binary)
/// </summary>
public class TextByteArray
{
    private readonly ImmutableArray<byte> bytes;

    public TextByteArray(string text) => bytes = [.. Encoding.UTF8.GetBytes(text)];

    public TextByteArray(byte[] bytes) => this.bytes = [.. bytes];

    public TextByteArray(ImmutableArray<byte> bytes) => this.bytes = bytes;

    public ImmutableArray<byte> Bytes => bytes;

    public byte[] ToArray() => [.. bytes];

    // Left as property to make it easier to use in the debugger
    public string Text => Encoding.UTF8.GetString(bytes.AsSpan());

    public Stream ToStream()
    {
        return new MemoryStream([.. Bytes]);
    }

    public static Stream TextToStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    public static string StreamToText(Stream stream)
    {
        stream.Position = 0;
        var sr = new StreamReader(stream, Encoding.UTF8);
        return sr.ReadToEnd();
    }
}
