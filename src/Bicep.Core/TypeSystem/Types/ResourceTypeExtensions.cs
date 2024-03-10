// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.TypeSystem.Types;

public static class ResourceTypeExtensions
{
    public static bool IsAzResource(this ResourceType resourceType)
        => resourceType.DeclaringNamespace.ProviderNameEquals(AzNamespaceType.BuiltInName);
}
