// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata
{
    public record ParameterMetadata(string Name, ITypeReference TypeReference, bool IsRequired, string? Description)
    {
    }
}
