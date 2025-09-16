// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.IO.Abstraction;

namespace Bicep.Decompiler
{
    public record DecompileResult(
        IOUri EntrypointUri,
        ImmutableDictionary<IOUri, string> FilesToSave);
}
