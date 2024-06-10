// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Headers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public record NamespaceResult(
    string Name,
    TypeSymbol Type,
    ProviderDeclarationSyntax? Origin);

public class NamespaceProvider : INamespaceProvider
{
    private readonly IResourceTypeProviderFactory resourceTypeProviderFactory;

    public NamespaceProvider(IResourceTypeProviderFactory resourceTypeProviderFactory)
    {
        this.resourceTypeProviderFactory = resourceTypeProviderFactory;
    }

    public IEnumerable<NamespaceResult> GetNamespaces(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope)
    {
        var providerDeclarations = SyntaxAggregator.AggregateByType<ProviderDeclarationSyntax>(sourceFile.ProgramSyntax).ToImmutableArray();
        var implicitProviders = artifactFileLookup.ImplicitProviders[sourceFile].ToDictionary(x => x.Name, LanguageConstants.IdentifierComparer);

        if (implicitProviders.TryGetValue(SystemNamespaceType.BuiltInName, out var sysProvider))
        {
            // TODO proper diag here
            var nsType = ErrorType.Create(DiagnosticBuilder.ForDocumentStart().ProvidersAreDisabled());
            yield return new(sysProvider.Name, nsType, null);
        }

        var assignedProviders = new HashSet<string>(LanguageConstants.IdentifierComparer);
        foreach (var provider in providerDeclarations)
        {
            var type = GetNamespaceType(rootConfig, features, artifactFileLookup, sourceFile, targetScope, provider);
            if (type is NamespaceType validType)
            {
                assignedProviders.Add(validType.ProviderName);
            }

            var name = provider.Alias?.IdentifierName ?? type.Name;
            yield return new(name, type, provider);
        }

        // sys isn't included in the implicit providers config, because we don't want users to customize it.
        // for the purposes of this logic, it's simpler to treat it as if it is.
        implicitProviders[SystemNamespaceType.BuiltInName] = new ImplicitProvider(SystemNamespaceType.BuiltInName, new("builtin:"), null);

        foreach (var (providerName, implicitProvider) in implicitProviders)
        {
            if (assignedProviders.Contains(providerName))
            {
                // if an implicit provider has been explicitly registered in a file, it shouldn't also be registered as implicit
                continue;
            }

            var nsType = GetNamespaceTypeForImplicitProvider(rootConfig, features, sourceFile, targetScope, implicitProvider, null);
            yield return new(providerName, nsType, null);
        }
    }

    private TypeSymbol GetNamespaceTypeForImplicitProvider(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ImplicitProvider implicitProvider,
        ProviderDeclarationSyntax? syntax)
    {
        if (implicitProvider.Config is null)
        {
            return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().InvalidProvider_ImplicitProviderMissingConfig(rootConfig.ConfigFileUri, implicitProvider.Name));
        }

        return GetNamespaceTypeForConfigManagedProvider(rootConfig, features, sourceFile, targetScope, implicitProvider.Artifact, syntax, implicitProvider.Name);
    }

    private TypeSymbol GetNamespaceType(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ProviderDeclarationSyntax syntax)
    {
        if (!features.ExtensibilityEnabled)
        {
            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ProvidersAreDisabled());
        }

        if (syntax.SpecificationString.IsSkipped)
        {
            // this will have raised a parsing diagnostic
            return ErrorType.Empty();
        }

        if (artifactFileLookup.ArtifactLookup.TryGetValue(syntax, out var artifact))
        {
            var aliasName = syntax.Alias?.IdentifierName;
            if (GetNamespaceTypeForArtifact(features, artifact, sourceFile, targetScope, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
            {
                return namespaceType;
            }

            return ErrorType.Create(errorBuilder(DiagnosticBuilder.ForPosition(syntax.SpecificationString)));
        }

        if (syntax.SpecificationString is not IdentifierSyntax identifier)
        {
            // this will have raised a parsing diagnostic
            return ErrorType.Empty();
        }

        return GetNamespaceTypeForConfigManagedProvider(rootConfig, features, sourceFile, targetScope, null, syntax, identifier.IdentifierName);
    }

    protected virtual TypeSymbol GetNamespaceTypeForConfigManagedProvider(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ArtifactResolutionInfo? artifact,
        ProviderDeclarationSyntax? syntax,
        string providerName)
    {
        var aliasName = syntax?.Alias?.IdentifierName ?? providerName;
        var diagBuilder = syntax is { } ? DiagnosticBuilder.ForPosition(syntax) : DiagnosticBuilder.ForDocumentStart();

        if (artifact is { })
        {
            // not a built-in provider
            if (GetNamespaceTypeForArtifact(features, artifact, sourceFile, targetScope, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
            {
                return namespaceType;
            }

            return ErrorType.Create(errorBuilder(diagBuilder));
        }

        // built-in provider
        if (LanguageConstants.IdentifierComparer.Equals(providerName, SystemNamespaceType.BuiltInName))
        {
            return SystemNamespaceType.Create(aliasName, features, sourceFile.FileKind);
        }

        if (LanguageConstants.IdentifierComparer.Equals(providerName, AzNamespaceType.BuiltInName))
        {
            return AzNamespaceType.Create(aliasName, targetScope, resourceTypeProviderFactory.GetBuiltInAzResourceTypesProvider(), sourceFile.FileKind);
        }

        if (LanguageConstants.IdentifierComparer.Equals(providerName, MicrosoftGraphNamespaceType.BuiltInName))
        {
            return MicrosoftGraphNamespaceType.Create(aliasName);
        }

        if (LanguageConstants.IdentifierComparer.Equals(providerName, K8sNamespaceType.BuiltInName))
        {
            return K8sNamespaceType.Create(aliasName);
        }

        return ErrorType.Create(diagBuilder.InvalidProvider_NotABuiltInProvider(rootConfig.ConfigFileUri, providerName));
    }

    private ResultWithDiagnostic<NamespaceType> GetNamespaceTypeForArtifact(IFeatureProvider features, ArtifactResolutionInfo artifact, BicepSourceFile sourceFile, ResourceScope targetScope, string? aliasName)
    {
        if (!artifact.Result.IsSuccess(out var typesTgzUri, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        var useAzLoader = artifact.Reference is OciArtifactReference ociArtifact && ociArtifact.Repository.EndsWith("/az");
        if (useAzLoader && !features.DynamicTypeLoadingEnabled)
        {
            return new(x => x.FetchingAzTypesRequiresExperimentalFeature());
        }

        if (!useAzLoader && !features.ProviderRegistryEnabled)
        {
            return new(x => x.FetchingTypesRequiresExperimentalFeature());
        }

        if (!resourceTypeProviderFactory.GetResourceTypeProvider(artifact.Reference, typesTgzUri, useAzLoader: useAzLoader).IsSuccess(out var typeProvider, out errorBuilder))
        {
            return new(errorBuilder);
        }

        if (useAzLoader)
        {
            return new(AzNamespaceType.Create(aliasName, targetScope, typeProvider, sourceFile.FileKind));
        }

        return new(ThirdPartyNamespaceType.Create(aliasName, typeProvider, artifact.Reference));
    }
}
