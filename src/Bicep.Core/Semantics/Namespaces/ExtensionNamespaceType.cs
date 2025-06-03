// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.Core.TypeSystem.Types;
using static Bicep.Core.TypeSystem.Providers.Extensibility.ExtensionResourceTypeLoader;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ExtensionNamespaceType
    {
        public static NamespaceType Create(string? aliasName, ExtensionResourceTypeProvider resourceTypeProvider, ArtifactReference? artifact, IFeatureProvider features)
        {
            var namespaceSettings = resourceTypeProvider.GetNamespaceSettings();

            return new NamespaceType(
                // TODO: Using BicepExtensionName is not ideal. We need to add default aliasing support for extensions.
                aliasName ?? namespaceSettings.BicepExtensionName,
                namespaceSettings,
                ExtensionNamespaceTypeHelper.GetExtensionNamespaceObjectProperties(namespaceSettings, features),
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider,
                artifact);
        }
    }
}
