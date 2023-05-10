// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public class ApiVersionProviderFactory : IApiVersionProviderFactory
{
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly IAzResourceTypeLoaderFactory azResourceTypeLoaderFactory;

    public ApiVersionProviderFactory(IFeatureProviderFactory featureProviderFactory, IAzResourceTypeLoaderFactory azResourceTypeLoaderFactory)
    {
        this.azResourceTypeLoaderFactory = azResourceTypeLoaderFactory;
        this.featureProviderFactory = featureProviderFactory;
    }

    public IApiVersionProvider GetApiVersionProvider(BicepSourceFile bicepFile)
    {
        var azProviderDeclaration = bicepFile.ProgramSyntax.Children
            .OfType<ImportDeclarationSyntax>()
            .SingleOrDefault(x => x.Specification.Name.Equals(AzNamespaceType.BuiltInName, LanguageConstants.IdentifierComparison));
        var features = featureProviderFactory.GetFeatureProvider(bicepFile.FileUri);
        return new ApiVersionProvider(features, azResourceTypeLoaderFactory.GetResourceTypeLoader(azProviderDeclaration, features));
    }
}
