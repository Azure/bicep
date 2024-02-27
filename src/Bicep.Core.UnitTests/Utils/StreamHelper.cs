// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.UnitTests.Utils;

public static class StreamHelper
{
    public static BinaryData GetBinaryData(Action<Stream> streamWriteFunc)
    {
        using var memoryStream = new MemoryStream();
        streamWriteFunc(memoryStream);

        return new BinaryData(memoryStream.ToArray());
    }

    public static string GetString(Action<Stream> streamWriteFunc)
        => GetBinaryData(streamWriteFunc).ToString();
}
