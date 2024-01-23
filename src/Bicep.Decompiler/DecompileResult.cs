// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Decompiler
{
    public record DecompileResult(
        Uri EntrypointUri,
        ImmutableDictionary<Uri, string> FilesToSave);
}
