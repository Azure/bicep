// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Bicep.Core.UnitTests.Utils;

/// <summary>
/// A byte array for UTF-8 text
/// </summary>
public class TextByteArray
{
    private readonly ImmutableArray<byte> _bytes;

    public TextByteArray(string text) => _bytes = Encoding.UTF8.GetBytes(text).ToImmutableArray();

    public TextByteArray(byte[] bytes) => _bytes = bytes.ToImmutableArray();

    public TextByteArray(ImmutableArray<byte> bytes) => _bytes = bytes;

    public ImmutableArray<byte> Bytes => _bytes;

    public byte[] ToArray() => _bytes.ToArray();

    // Left as property to make it easier to use in the debugger
    public string Text => Encoding.UTF8.GetString(_bytes.ToArray());

    public Stream ToStream()
    {
        return new MemoryStream(Bytes.ToArray());
    }

    public static Stream TextToStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    public static string StreamToText(Stream stream)
    {
        stream.Position = 0;

        int capacity;
        checked
        {
            capacity = ((int)stream.Length);
        }
        var ms = new MemoryStream(capacity);
        stream.CopyTo(ms);
        ms.Position = 0;

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}
