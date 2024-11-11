// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.MicrosoftGraph;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public record NamespaceResult(
    string Name,
    TypeSymbol Type,
    ExtensionDeclarationSyntax? Origin);

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
        var extensions = SyntaxAggregator.AggregateByType<ExtensionDeclarationSyntax>(sourceFile.ProgramSyntax).ToImmutableArray();
        var implicitExtensions = artifactFileLookup.ImplicitExtensions[sourceFile].ToDictionary(x => x.Name, LanguageConstants.IdentifierComparer);

        if (implicitExtensions.TryGetValue(SystemNamespaceType.BuiltInName, out var sysProvider))
        {
            // TODO proper diag here
            var nsType = ErrorType.Create(DiagnosticBuilder.ForDocumentStart().ExtensionsAreDisabled());
            yield return new(sysProvider.Name, nsType, null);
        }

        var assignedProviders = new HashSet<string>(LanguageConstants.IdentifierComparer);
        foreach (var extension in extensions)
        {
            var type = GetNamespaceType(rootConfig, features, artifactFileLookup, sourceFile, targetScope, extension);
            if (type is NamespaceType validType)
            {
                assignedProviders.Add(validType.ExtensionName);
            }

            var name = extension.Alias?.IdentifierName ?? type.Name;
            yield return new(name, type, extension);
        }

        // sys isn't included in the implicit extensions config, because we don't want users to customize it.
        // for the purposes of this logic, it's simpler to treat it as if it is.
        implicitExtensions[SystemNamespaceType.BuiltInName] = new ImplicitExtension(SystemNamespaceType.BuiltInName, new("builtin:"), null);

        foreach (var (extensionName, implicitExtension) in implicitExtensions)
        {
            if (assignedProviders.Contains(extensionName))
            {
                // if an implicit extension has been explicitly registered in a file, it shouldn't also be registered as implicit
                continue;
            }

            var nsType = GetNamespaceTypeForImplicitExtension(rootConfig, features, sourceFile, targetScope, implicitExtension, null);
            yield return new(extensionName, nsType, null);
        }
    }

    private TypeSymbol GetNamespaceTypeForImplicitExtension(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ImplicitExtension extension,
        ExtensionDeclarationSyntax? syntax)
    {
        if (extension.Config is null)
        {
            return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().InvalidExtension_ImplicitExtensionMissingConfig(rootConfig.ConfigFileIdentifier, extension.Name));
        }

        return GetNamespaceTypeForConfigManagedExtension(rootConfig, features, sourceFile, targetScope, extension.Artifact, syntax, extension.Name);
    }

    private TypeSymbol GetNamespaceType(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ExtensionDeclarationSyntax syntax)
    {
        if (!features.ExtensibilityEnabled)
        {
            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ExtensionsAreDisabled());
        }

        if (syntax.SpecificationString.IsSkipped)
        {
            // this will have raised a parsing diagnostic
            return ErrorType.Empty();
        }

        if (artifactFileLookup.ArtifactLookup.TryGetValue(syntax, out var artifact))
        {
            var aliasName = syntax.Alias?.IdentifierName;
            if (GetNamespaceTypeForArtifact(artifact, sourceFile, targetScope, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
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

        return GetNamespaceTypeForConfigManagedExtension(rootConfig, features, sourceFile, targetScope, null, syntax, identifier.IdentifierName);
    }

    protected virtual TypeSymbol GetNamespaceTypeForConfigManagedExtension(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ArtifactResolutionInfo? artifact,
        ExtensionDeclarationSyntax? syntax,
        string extensionName)
    {
        var aliasName = syntax?.Alias?.IdentifierName ?? extensionName;
        var diagBuilder = syntax is { } ? DiagnosticBuilder.ForPosition(syntax) : DiagnosticBuilder.ForDocumentStart();

        if (artifact is { })
        {
            // not a built-in extension
            if (GetNamespaceTypeForArtifact(artifact, sourceFile, targetScope, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
            {
                return namespaceType;
            }

            return ErrorType.Create(errorBuilder(diagBuilder));
        }

        // built-in extension
        if (LanguageConstants.IdentifierComparer.Equals(extensionName, SystemNamespaceType.BuiltInName))
        {
            return SystemNamespaceType.Create(aliasName, features, sourceFile.FileKind);
        }

        if (LanguageConstants.IdentifierComparer.Equals(extensionName, AzNamespaceType.BuiltInName))
        {
            return AzNamespaceType.Create(aliasName, targetScope, resourceTypeProviderFactory.GetBuiltInAzResourceTypesProvider(), sourceFile.FileKind);
        }

        if (LanguageConstants.IdentifierComparer.Equals(extensionName, MicrosoftGraphNamespaceType.BuiltInName))
        {
            return MicrosoftGraphNamespaceType.Create(aliasName);
        }

        if (LanguageConstants.IdentifierComparer.Equals(extensionName, K8sNamespaceType.BuiltInName))
        {
            return K8sNamespaceType.Create(aliasName);
        }

        return ErrorType.Create(diagBuilder.InvalidExtension_NotABuiltInExtension(rootConfig.ConfigFileIdentifier, extensionName));
    }

    private ResultWithDiagnosticBuilder<NamespaceType> GetNamespaceTypeForArtifact(ArtifactResolutionInfo artifact, BicepSourceFile sourceFile, ResourceScope targetScope, string? aliasName)
    {
        if (!artifact.Result.IsSuccess(out var typesTgzUri, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        if (!resourceTypeProviderFactory.GetResourceTypeProvider(artifact.Reference, typesTgzUri).IsSuccess(out var typeProvider, out errorBuilder))
        {
            return new(errorBuilder);
        }

        if (typeProvider is AzResourceTypeProvider)
        {
            return new(AzNamespaceType.Create(aliasName, targetScope, typeProvider, sourceFile.FileKind));
        }

        if (typeProvider is MicrosoftGraphResourceTypeProvider)
        {
            return new(MicrosoftGraphNamespaceType.Create(aliasName, typeProvider, artifact.Reference));
        }

        return new(ThirdPartyNamespaceType.Create(aliasName, typeProvider, artifact.Reference));
    }
}
