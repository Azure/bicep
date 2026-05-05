// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Frozen;
using System.Diagnostics;
using Azure.Bicep.Types;
using Bicep.Core.Registry;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.TypeSystem;

public class ArchivedTypeLoader(
    FrozenDictionary<string, BinaryData> typesCache) : TypeLoader
{
    public static ArchivedTypeLoader FromFileHandle(IFileHandle tgzFileHandle) => new(TgzFileExtractor.ExtractFromFileHandle(tgzFileHandle));

    public static ArchivedTypeLoader FromStream(Stream tgzStream) => new(TgzFileExtractor.ExtractFromStream(tgzStream));

    protected override Stream GetContentStreamAtPath(string path)
    {
        if (typesCache.TryGetValue(path, out var data))
        {
            return data.ToStream();
        }
        else
        {
            Trace.WriteLine($"{nameof(GetContentStreamAtPath)} threw an exception. Requested path: '{path}' not found.");
            throw new InvalidArtifactException($"The path: {path} was not found in artifact contents", InvalidArtifactExceptionKind.InvalidArtifactContents);
        }
    }
}
