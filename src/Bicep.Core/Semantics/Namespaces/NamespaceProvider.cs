// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Headers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public record NamespaceResult(
    string Name,
    string ProviderName,
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
            yield return new(sysProvider.Name, SystemNamespaceType.BuiltInName, nsType, null);
        }

        var assignedProviders = new HashSet<string>(LanguageConstants.IdentifierComparer);
        foreach (var provider in providerDeclarations)
        {
            var providerName = provider.Specification.NamespaceIdentifier;
            var aliasName = provider.Alias?.IdentifierName ?? providerName;
            if (provider.Specification.IsValid)
            {
                assignedProviders.Add(providerName);
            }

            var nsType = GetNamespaceType(rootConfig, features, artifactFileLookup, sourceFile, targetScope, provider);
            yield return new(aliasName, providerName, nsType, provider);
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

            var nsType = GetNamespaceTypeForImplicitProvider(rootConfig, features, sourceFile, targetScope, implicitProvider, implicitProvider.Name, implicitProvider.Name, null);
            yield return new(implicitProvider.Name, providerName, nsType, null);
        }
    }

    private TypeSymbol GetNamespaceTypeForImplicitProvider(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ImplicitProvider implicitProvider,
        string providerName,
        string aliasName,
        ProviderDeclarationSyntax? syntax)
    {
        if (implicitProvider.Config is null)
        {
            return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().InvalidProvider_ImplicitProviderMissingConfig(rootConfig.ConfigFileUri, providerName));
        }

        return GetNamespaceTypeForConfigManagedProvider(rootConfig, features, sourceFile, targetScope, implicitProvider.Artifact, providerName, aliasName, syntax);
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

        // Check for interpolated specification strings
        if (syntax.SpecificationString is StringSyntax specificationString && specificationString.IsInterpolated())
        {
            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.SpecificationString).ProviderSpecificationInterpolationUnsupported());
        }

        if (!syntax.Specification.IsValid)
        {
            return (syntax.SpecificationString is StringSyntax)
                ? ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.SpecificationString).InvalidProviderSpecification())
                : ErrorType.Empty();
        }

        var providerName = syntax.Specification.NamespaceIdentifier;
        var aliasName = syntax.Alias?.IdentifierName ?? providerName;
        if (syntax.Specification is ConfigurationManagedProviderSpecification or LegacyProviderSpecification)
        {
            if (!TryGetProviderConfig(rootConfig, providerName).IsSuccess(out var providerConfig, out var errorBuilder))
            {
                return ErrorType.Create(errorBuilder(DiagnosticBuilder.ForPosition(syntax)));
            }

            var artifact = providerConfig.BuiltIn ? null : artifactFileLookup.ArtifactLookup[syntax];
            return GetNamespaceTypeForConfigManagedProvider(rootConfig, features, sourceFile, targetScope, artifact, providerName, aliasName, syntax);
        }

        if (syntax.Specification is InlinedProviderSpecification)
        {
            var artifact = artifactFileLookup.ArtifactLookup[syntax];

            if (GetNamespaceTypeForArtifact(features, artifact, sourceFile, targetScope, providerName, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
            {
                return namespaceType;
            }

            return ErrorType.Create(errorBuilder(DiagnosticBuilder.ForPosition(syntax.SpecificationString)));
        }

        // we've exhaustively checked all the different implementations of IProviderSpecification
        throw new UnreachableException();
    }

    private TypeSymbol GetNamespaceTypeForConfigManagedProvider(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ArtifactResolutionInfo? artifact,
        string providerName,
        string aliasName,
        ProviderDeclarationSyntax? syntax)
    {
        var diagBuilder = syntax is { } ? DiagnosticBuilder.ForPosition(syntax) : DiagnosticBuilder.ForDocumentStart();

        if (artifact is { })
        {
            // not a built-in provider
            if (GetNamespaceTypeForArtifact(features, artifact, sourceFile, targetScope, providerName, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
            {
                return namespaceType;
            }

            return ErrorType.Create(errorBuilder(diagBuilder));
        }
        else
        {
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
                if (!features.MicrosoftGraphPreviewEnabled)
                {
                    return ErrorType.Create(diagBuilder.UnrecognizedProvider(providerName));
                }

                return MicrosoftGraphNamespaceType.Create(aliasName);
            }

            if (LanguageConstants.IdentifierComparer.Equals(providerName, K8sNamespaceType.BuiltInName))
            {
                return K8sNamespaceType.Create(aliasName);
            }

            return ErrorType.Create(diagBuilder.InvalidProvider_NotABuiltInProvider(rootConfig.ConfigFileUri, providerName));
        }
    }

    private ResultWithDiagnostic<NamespaceType> GetNamespaceTypeForArtifact(IFeatureProvider features, ArtifactResolutionInfo artifact, BicepSourceFile sourceFile, ResourceScope targetScope, string providerName, string aliasName)
    {
        var useAzLoader = LanguageConstants.IdentifierComparer.Equals(providerName, AzNamespaceType.BuiltInName);
        if (useAzLoader && !features.DynamicTypeLoadingEnabled)
        {
            return new(x => x.FetchingAzTypesRequiresExperimentalFeature());
        }

        if (!useAzLoader && !features.ProviderRegistryEnabled)
        {
            return new(x => x.FetchingTypesRequiresExperimentalFeature());
        }

        if (!artifact.Result.IsSuccess(out var typesTgzUri, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        if (!resourceTypeProviderFactory.GetResourceTypeProvider(typesTgzUri, useAzLoader: useAzLoader).IsSuccess(out var typeProvider, out errorBuilder))
        {
            return new(errorBuilder);
        }

        if (useAzLoader)
        {
            return new(AzNamespaceType.Create(aliasName, targetScope, typeProvider, sourceFile.FileKind));
        }

        return new(ThirdPartyNamespaceType.Create(providerName, aliasName, typeProvider));
    }

    private ResultWithDiagnostic<ProviderConfigEntry> TryGetProviderConfig(RootConfiguration rootConfig, string providerName)
    {
        if (LanguageConstants.IdentifierComparer.Equals(providerName, SystemNamespaceType.BuiltInName))
        {
            // synthesize an entry for 'sys'
            return new(new ProviderConfigEntry("builtin:"));
        }

        return rootConfig.ProvidersConfig.TryGetProviderSource(providerName);
    }
}
