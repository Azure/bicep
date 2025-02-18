// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Metadata
{
    public record ExtensionMetadata(
        string Alias,
        string Name,
        string Version,
        ObjectType? ConfigType,
        bool IsConfigRequired);
}
