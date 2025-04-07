// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ExtensionNamespaceTypeHelper
    {
        public static ImmutableArray<NamedTypeProperty> GetExtensionNamespaceObjectProperties(NamespaceSettings namespaceSettings, IFeatureProvider features)
        {
            if (features is not { ExtensibilityEnabled: true, ModuleExtensionConfigsEnabled: true } || namespaceSettings.ConfigurationType is null)
            {
                return [];
            }

            var namespaceProperties = ImmutableArray.CreateBuilder<NamedTypeProperty>();

            var namespaceConfigType = TypeHelper.DeepCloneAndModifyPropertyFlagsRecursively(namespaceSettings.ConfigurationType, f => f | TypePropertyFlags.ReadOnly);
            namespaceProperties.Add(new NamedTypeProperty(LanguageConstants.ExtensionConfigPropertyName, namespaceConfigType, TypePropertyFlags.ReadOnly, "The extension configuration."));

            return namespaceProperties.ToImmutable();
        }
    }
}
