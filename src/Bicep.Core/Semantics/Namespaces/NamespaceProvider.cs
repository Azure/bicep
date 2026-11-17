// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.Core.TypeSystem.Types;

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
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope)
    {
        var extensions = SyntaxAggregator.AggregateByType<ExtensionDeclarationSyntax>(sourceFile.ProgramSyntax).ToImmutableArray();
        var implicitExtensions = artifactFileLookup.ImplicitExtensions[sourceFile].ToDictionary(x => x.Name, LanguageConstants.IdentifierComparer);

        if (implicitExtensions.TryGetValue(SystemNamespaceType.BuiltInName, out var sysProvider))
        {
            var nsType = ErrorType.Create(DiagnosticBuilder.ForDocumentStart().InvalidReservedImplicitExtensionNamespace(sysProvider.Name));
            yield return new(sysProvider.Name, nsType, null);
        }

        var assignedProviders = new HashSet<string>(LanguageConstants.IdentifierComparer);
        foreach (var extension in extensions)
        {
            var type = GetNamespaceType(artifactFileLookup, sourceFile, targetScope, extension);
            if (type is NamespaceType validType)
            {
                assignedProviders.Add(validType.ExtensionName);
            }

            yield return new(extension.TryGetSymbolName() ?? type.Name, type, extension);
        }

        // sys isn't included in the implicit extensions config, because we don't want users to customize it.
        // for the purposes of this logic, it's simpler to treat it as if it is.
        implicitExtensions[SystemNamespaceType.BuiltInName] = new ImplicitExtension(SystemNamespaceType.BuiltInName, null);

        foreach (var (extensionName, implicitExtension) in implicitExtensions)
        {
            if (assignedProviders.Contains(extensionName))
            {
                // if an implicit extension has been explicitly registered in a file, it shouldn't also be registered as implicit
                continue;
            }

            var nsType = GetNamespaceTypeForConfigManagedExtension(sourceFile, targetScope, implicitExtension.Dependency, null, extensionName);
            yield return new(extensionName, nsType, null);
        }
    }

    private TypeSymbol GetNamespaceType(
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ExtensionDeclarationSyntax syntax)
    {
        if (syntax.SpecificationString.IsSkipped)
        {
            // this will have raised a parsing diagnostic
            return ErrorType.Empty();
        }

        if (artifactFileLookup.ArtifactLookup.TryGetValue(syntax, out var dependency))
        {
            if (GetNamespaceTypeForArtifact(dependency, sourceFile, targetScope, syntax.TryGetSymbolName()).IsSuccess(out var namespaceType, out var errorBuilder))
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

        return GetNamespaceTypeForConfigManagedExtension(sourceFile, targetScope, null, syntax, identifier.IdentifierName);
    }

    protected virtual TypeSymbol GetNamespaceTypeForConfigManagedExtension(
        BicepSourceFile sourceFile,
        ResourceScope targetScope,
        ArtifactDependency? dependency,
        ExtensionDeclarationSyntax? syntax,
        string extensionName)
    {
        var aliasName = syntax?.TryGetSymbolName() ?? extensionName;
        var diagBuilder = syntax is { } ? DiagnosticBuilder.ForPosition(syntax) : DiagnosticBuilder.ForDocumentStart();

        if (dependency is { })
        {
            // not a built-in extension
            if (GetNamespaceTypeForArtifact(dependency, sourceFile, targetScope, aliasName).IsSuccess(out var namespaceType, out var errorBuilder))
            {
                return namespaceType;
            }

            return ErrorType.Create(errorBuilder(diagBuilder));
        }

        // built-in extension
        if (LanguageConstants.IdentifierComparer.Equals(extensionName, SystemNamespaceType.BuiltInName))
        {
            return SystemNamespaceType.Create(aliasName, sourceFile.LoadFeatures(), sourceFile.FileKind);
        }

        if (LanguageConstants.IdentifierComparer.Equals(extensionName, AzNamespaceType.BuiltInName))
        {
            var typeProvider = targetScope switch
            {
                ResourceScope.Local => new EmptyResourceTypeProvider(),
                _ => resourceTypeProviderFactory.GetBuiltInAzResourceTypesProvider(),
            };

            var configurationType = sourceFile.LoadFeatures().AzExtensionConfigEnabled && typeProvider is AzResourceTypeProvider azProvider
                ? azProvider.GetConfigurationType()
                : null;

            return AzNamespaceType.Create(aliasName, targetScope, typeProvider, sourceFile.FileKind, configurationType);
        }

        if (LanguageConstants.IdentifierComparer.Equals(extensionName, K8sNamespaceType.BuiltInName))
        {
            return K8sNamespaceType.Create(aliasName, sourceFile.LoadFeatures());
        }

        // microsoftGraph built-in extension is no longer supported.
        if (LanguageConstants.IdentifierComparer.Equals(extensionName, MicrosoftGraphExtensionFacts.builtInExtensionName))
        {
            return ErrorType.Create(diagBuilder.MicrosoftGraphBuiltinRetired(syntax));
        }

        return ErrorType.Create(diagBuilder.InvalidExtension_NotABuiltInExtension(sourceFile.LoadConfiguration().ConfigFileUri, extensionName));
    }

    private ResultWithDiagnosticBuilder<NamespaceType> GetNamespaceTypeForArtifact(ArtifactDependency dependency, BicepSourceFile sourceFile, ResourceScope targetScope, string? aliasName)
    {
        if (!dependency.TryGetArtifact().IsSuccess(out var artifact, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        if (artifact is not ExtensionArtifact extensionArtifact)
        {
            throw new InvalidOperationException($"Expected an extension, but the artifact for '{artifact.Reference.FullyQualifiedReference}' is a {artifact.Reference.Type}.");
        }

        if (!resourceTypeProviderFactory.GetResourceTypeProvider(extensionArtifact.Types).IsSuccess(out var typeProvider, out errorBuilder))
        {
            return new(errorBuilder);
        }

        return typeProvider switch
        {
            AzResourceTypeProvider azResourceTypeProvider => new(AzNamespaceType.Create(
                aliasName,
                targetScope,
                azResourceTypeProvider,
                sourceFile.FileKind,
                sourceFile.LoadFeatures().AzExtensionConfigEnabled ? azResourceTypeProvider.GetConfigurationType() : null)),
            ExtensionResourceTypeProvider extensionResourceTypeProvider => new(ExtensionNamespaceType.Create(aliasName, extensionResourceTypeProvider, extensionArtifact, sourceFile.LoadFeatures(), sourceFile.FileKind)),
            _ => throw new InvalidOperationException($"Unexpected resource type provider type: {typeProvider.GetType().Name}."),
        };
    }
}
