// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// IUnresolvedResourceDerivedType represents a type expressed via a reference to a resource type or partial body thereof
/// (e.g., Microsoft.KeyVault/vaults@2022-07-01#/properties/accessPolicies/items). This type is "unresolved" because it
/// was used in the type of a parameter, output, or exported type definition in an ARM JSON template and must be matched
/// by name to a concrete resource definition based on the configured extensions of the consuming Bicep module.
/// </summary>
public interface IUnresolvedResourceDerivedType
{
    // TODO This type needs to capture an extension identifier (built-in name or OCI reference) in order to support extensions other than `az`

    public ResourceTypeReference TypeReference { get; }

    public ImmutableArray<string> PointerSegments { get; }

    public TypeSymbol FallbackType { get; }

    public ResourceDerivedTypeVariant Variant { get; }
}
