// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Workspaces;
using Bicep.LocalDeploy.Namespaces;

namespace Bicep.Core.Semantics.Namespaces;

public class ExtensibilityNamespaceProvider : DefaultNamespaceProvider
{
    public ExtensibilityNamespaceProvider(IResourceTypeProviderFactory resourceTypeLoaderFactory)
        : base(resourceTypeLoaderFactory)
    {
    }

    public override ResultWithDiagnostic<NamespaceType> TryGetNamespace(
        ResourceTypesProviderDescriptor descriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind)
    {
        if (features.LocalDeployEnabled)
        {
            var namespaceType = descriptor.Name switch {
                SystemNamespaceType.BuiltInName => SystemNamespaceType.Create(descriptor.Alias, features, sourceFileKind),
                K8sNamespaceType.BuiltInName => K8sNamespaceType.Create(descriptor.Alias),
                UtilsNamespaceType.BuiltInName => UtilsNamespaceType.Create(descriptor.Alias),
                GithubNamespaceType.BuiltInName => GithubNamespaceType.Create(descriptor.Alias),
                _ => null,
            };

            return namespaceType is {} ? 
                new(namespaceType) :
                new(x => x.UnrecognizedProvider(descriptor.Name));
        }

        return base.TryGetNamespace(descriptor, resourceScope, features, sourceFileKind);
    }
}
