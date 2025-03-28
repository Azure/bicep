// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ExtensionNamespaceTypeHelper
    {
        public static ImmutableArray<NamedTypeProperty> GetExtensionNamespaceObjectProperties(NamespaceSettings namespaceSettings)
        {
            var namespaceProperties = ImmutableArray.CreateBuilder<NamedTypeProperty>();

            if (namespaceSettings.ConfigurationType is not null)
            {
                var namespaceConfigType = TypeHelper.DeepCloneAndModifyPropertyFlagsRecursively(namespaceSettings.ConfigurationType, f => f | TypePropertyFlags.ReadOnly);

                namespaceProperties.Add(new NamedTypeProperty(LanguageConstants.ExtensionConfigPropertyName, namespaceConfigType, TypePropertyFlags.ReadOnly, "The extension configuration."));
            }

            return namespaceProperties.ToImmutable();
        }
    }
}
