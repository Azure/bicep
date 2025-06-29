// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.SourceGraph.Artifacts
{
    public interface IExtensionArtifact
    {
        IFileHandle TypesTgzFile { get; }

        IFileHandle BinaryFile { get; }
    }
}
