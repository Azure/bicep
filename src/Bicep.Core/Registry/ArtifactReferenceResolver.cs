// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;

namespace Bicep.Core.Registry;

/// <summary>
/// Resolves artifact reference text and syntax to an <see cref="ArtifactReference"/>.
/// </summary>
public sealed class ArtifactReferenceResolver : IArtifactReferenceFactory
{
    private readonly IArtifactRegistryProvider registryProvider;

    public ArtifactReferenceResolver(IArtifactRegistryProvider registryProvider)
    {
        this.registryProvider = registryProvider;
    }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string reference) =>
        ArtifactAddress.Parse(reference) switch
        {
            ArtifactAddress.Relative relative => this.TryParseWithRegistry(ArtifactReferenceSchemes.Local, referencingFile, artifactType, null, relative.Text),
            ArtifactAddress.Qualified qualified => this.TryParseWithSchemeRegistry(qualified.Scheme, referencingFile, artifactType, null, qualified.Body),
            ArtifactAddress.SchemeQualifiedAlias alias => this.TryParseWithSchemeRegistry(alias.Scheme, referencingFile, artifactType, alias.Alias, alias.Body),
            var address => throw new UnreachableException($"Unexpected artifact address type '{address.GetType().Name}'."),
        };

    public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, IArtifactReferenceSyntax artifactReferenceSyntax)
    {
        if (artifactReferenceSyntax is ExtensionDeclarationSyntax extensionSyntax)
        {
            if (!TryGetExtensionReferenceText(referencingFile, extensionSyntax).IsSuccess(out var extensionReference, out var extensionFailureBuilder))
            {
                return new(extensionFailureBuilder);
            }

            return this.TryGetArtifactReference(referencingFile, artifactReferenceSyntax.GetArtifactType(), extensionReference);
        }

        if (!SyntaxHelper.TryGetForeignTemplatePath(artifactReferenceSyntax, GetPathNotSpecifiedErrorBuilder(artifactReferenceSyntax)).IsSuccess(out var artifactPath, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        return this.TryGetArtifactReference(referencingFile, artifactReferenceSyntax.GetArtifactType(), artifactPath);
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryParseWithSchemeRegistry(string scheme, BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string body)
    {
        // An empty scheme must not fall back to the local registry, which is registered under the empty scheme.
        if (string.IsNullOrEmpty(scheme))
        {
            return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));
        }

        return this.TryParseWithRegistry(scheme, referencingFile, artifactType, aliasName, body);
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryParseWithRegistry(string scheme, BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string body)
    {
        if (this.registryProvider.TryGetRegistry(scheme) is not { } registry)
        {
            return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));
        }

        return registry.TryParseArtifactReference(referencingFile, artifactType, aliasName, body);
    }

    private static DiagnosticBuilder.DiagnosticBuilderDelegate GetPathNotSpecifiedErrorBuilder(IArtifactReferenceSyntax artifactReferenceSyntax) => artifactReferenceSyntax switch
    {
        UsingDeclarationSyntax => x => x.UsingPathHasNotBeenSpecified(),
        ExtendsDeclarationSyntax => x => x.ExtendsPathHasNotBeenSpecified(),
        CompileTimeImportDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
        ModuleDeclarationSyntax => x => x.ModulePathHasNotBeenSpecified(),
        TestDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
        _ => throw new NotImplementedException($"Unexpected artifact reference syntax type '{artifactReferenceSyntax.GetType().Name}'."),
    };

    private static ResultWithDiagnosticBuilder<string> TryGetExtensionReferenceText(BicepSourceFile referencingFile, ExtensionDeclarationSyntax extensionSyntax)
    {
        switch (extensionSyntax.SpecificationString)
        {
            case StringSyntax inlinedSpec:
                if (inlinedSpec.TryGetLiteralValue() is not string pathValue)
                {
                    return new(x => x.ExtensionSpecificationInterpolationUnsupported());
                }

                return new(pathValue);
            case IdentifierSyntax configSpec:
                var config = referencingFile.LoadConfiguration();
                if (!config.Extensions.TryGetExtensionSource(configSpec.IdentifierName).IsSuccess(out var extensionEntry, out var error))
                {
                    return new(error);
                }

                var extensionPath = extensionEntry.Value;
                if (extensionPath.Contains(':'))
                {
                    // The extension path contains a scheme.
                    return new(extensionPath);
                }

                if (config.ConfigFileUri is null)
                {
                    throw new InvalidOperationException("The configuration file URI must be set when trying to resolve an extension reference.");
                }

                // A bare path is relative to the configuration file; rewrite it relative to the referencing file.
                var extensionUri = config.ConfigFileUri.Resolve(extensionPath);

                return new(extensionUri.GetPathRelativeTo(referencingFile.FileHandle.Uri));
            default:
                return new(x => x.ExpectedExtensionSpecification());
        }
    }
}
