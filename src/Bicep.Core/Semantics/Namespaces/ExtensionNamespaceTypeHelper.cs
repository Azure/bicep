// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ExtensionNamespaceTypeHelper
    {
        public static ImmutableArray<NamedTypeProperty> GetExtensionNamespaceObjectProperties(NamespaceSettings namespaceSettings, IFeatureProvider features)
        {
            if (!features.ModuleExtensionConfigsEnabled || namespaceSettings.ConfigurationType is null)
            {
                return [];
            }

            return
            [
                new NamedTypeProperty(LanguageConstants.ExtensionConfigPropertyName, namespaceSettings.ConfigurationType, TypePropertyFlags.ReadOnly, "The extension configuration."),
            ];
        }
    }
}
