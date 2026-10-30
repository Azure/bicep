// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Artifacts.LegacyModuleAliasMocks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// Resolves artifact reference text and syntax to an <see cref="ArtifactReference"/>.
/// </summary>
/// <remarks>
/// Resolution is done in this order:
/// <list type="number">
///   <item>parse the text into an <see cref="ArtifactAddress"/>;</item>
///   <item>for a <c>br/&lt;alias&gt;:</c> reference, resolve <c>moduleAliasesMock</c> (a hit is terminal);</item>
///   <item>expand scheme-qualified aliases through <c>moduleAliases</c>;</item>
///   <item>parse the fully qualified body with the parser for the scheme.</item>
/// </list>
/// Registries do not parse references; they only operate on resolved references.
/// </remarks>
public sealed class ArtifactReferenceResolver : IArtifactReferenceFactory
{
    private readonly IArtifactRegistryProvider registryProvider;
    private readonly ModuleAliasMockResolver moduleAliasMockResolver;

    public ArtifactReferenceResolver(IArtifactRegistryProvider registryProvider, IFileExplorer fileExplorer)
    {
        this.registryProvider = registryProvider;
        this.moduleAliasMockResolver = new(fileExplorer);
    }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string referenceText) =>
        ArtifactAddress.Parse(referenceText) switch
        {
            ArtifactAddress.Path path => this.TryResolvePath(referencingFile, artifactType, path.Text),
            ArtifactAddress.SchemeQualified schemeQualified => this.TryResolveSchemeQualified(referencingFile, artifactType, schemeQualified.Scheme, schemeQualified.Body, aliasContext: null),
            ArtifactAddress.SchemeAndAliasQualified schemeAndAliasQualified => this.TryResolveSchemeAndAliasQualified(referencingFile, artifactType, schemeAndAliasQualified),
            var address => throw new UnreachableException($"Unexpected artifact address type '{address.GetType().Name}'."),
        };

    public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, IArtifactReferenceSyntax artifactReferenceSyntax)
    {
        if (artifactReferenceSyntax is ExtensionDeclarationSyntax extensionSyntax)
        {
            if (!TryGetExtensionReferenceText(referencingFile, extensionSyntax).IsSuccess(out var extensionReferenceText, out var extensionFailureBuilder))
            {
                return new(extensionFailureBuilder);
            }

            return this.TryGetArtifactReference(referencingFile, artifactReferenceSyntax.GetArtifactType(), extensionReferenceText);
        }

        if (!SyntaxHelper.TryGetForeignTemplatePath(artifactReferenceSyntax, GetPathNotSpecifiedErrorBuilder(artifactReferenceSyntax)).IsSuccess(out var referenceText, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        return this.TryGetArtifactReference(referencingFile, artifactReferenceSyntax.GetArtifactType(), referenceText);
    }

    /// <summary>
    /// Expands a scheme-qualified alias (<c>br/&lt;alias&gt;:</c> or <c>ts/&lt;alias&gt;:</c>) through <c>moduleAliases</c>
    /// into a fully qualified reference body for <paramref name="scheme"/>.
    /// </summary>
    public static ResultWithDiagnosticBuilder<string> TryExpandSchemeQualifiedAlias(IBicepConfiguration configuration, ArtifactType artifactType, string scheme, string aliasName, string body)
    {
        // Scheme-qualified aliases only support modules.
        if (artifactType != ArtifactType.Module)
        {
            return new(x => x.UnsupportedArtifactType(artifactType));
        }

        switch (scheme)
        {
            case ArtifactReferenceSchemes.Oci:
                if (!configuration.ModuleAliases.TryGetOciArtifactModuleAlias(aliasName).IsSuccess(out var ociAlias, out var ociFailureBuilder))
                {
                    return new(ociFailureBuilder);
                }

                return new($"{ociAlias}/{body}");
            case ArtifactReferenceSchemes.TemplateSpecs:
                if (!configuration.ModuleAliases.TryGetTemplateSpecModuleAlias(aliasName).IsSuccess(out var templateSpecAlias, out var templateSpecFailureBuilder))
                {
                    return new(templateSpecFailureBuilder);
                }

                return new($"{templateSpecAlias}/{body}");
            default:
                throw new ArgumentException($"Scheme '{scheme}' does not support scheme-qualified aliases.", nameof(scheme));
        }
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolvePath(BicepSourceFile referencingFile, ArtifactType artifactType, string path)
    {
        if (!this.IsSupportedScheme(ArtifactReferenceSchemes.Local))
        {
            return new(x => x.UnknownModuleReferenceScheme(ArtifactReferenceSchemes.Local, this.registryProvider.SupportedSchemes));
        }

        if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Extension)
        {
            return new(x => x.UnsupportedArtifactType(artifactType));
        }

        return LocalModuleReference.TryParse(referencingFile, artifactType, path).Transform(x => (ArtifactReference)x);
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveSchemeAndAliasQualified(BicepSourceFile referencingFile, ArtifactType artifactType, ArtifactAddress.SchemeAndAliasQualified address)
    {
        if (!this.IsSupportedScheme(address.Scheme) || !SupportsSchemeQualifiedAliases(address.Scheme))
        {
            // Reported the same way as a fully qualified reference with an unsupported scheme.
            return this.TryResolveSchemeQualified(referencingFile, artifactType, address.Scheme, address.Body, aliasContext: null);
        }

        if (address.Scheme == ArtifactReferenceSchemes.Oci &&
            this.moduleAliasMockResolver.TryResolve(referencingFile, artifactType, address.Alias, address.Body) is { } mockResult)
        {
            return mockResult;
        }

        if (!TryExpandSchemeQualifiedAlias(referencingFile.LoadConfiguration(), artifactType, address.Scheme, address.Alias, address.Body).IsSuccess(out var expandedBody, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        return this.TryResolveSchemeQualified(referencingFile, artifactType, address.Scheme, expandedBody, aliasContext: address.Alias);
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveSchemeQualified(BicepSourceFile referencingFile, ArtifactType artifactType, string scheme, string body, string? aliasContext)
    {
        // An empty scheme must not fall back to the local registry, which is registered under the empty scheme.
        if (string.IsNullOrEmpty(scheme) || !this.IsSupportedScheme(scheme))
        {
            return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));
        }

        switch (scheme)
        {
            case ArtifactReferenceSchemes.Oci:
                return OciArtifactReference.TryParse(referencingFile.LoadFeatures(), referencingFile.LoadConfiguration(), artifactType, aliasContext, body).Transform(x => (ArtifactReference)x);
            case ArtifactReferenceSchemes.TemplateSpecs:
                if (artifactType != ArtifactType.Module)
                {
                    return new(x => x.UnsupportedArtifactType(artifactType));
                }

                return TemplateSpecModuleReference.TryParse(referencingFile.LoadFeatures(), referencingFile.LoadConfiguration(), aliasContext, body).Transform(x => (ArtifactReference)x);
            case ArtifactReferenceSchemes.OciMocked:
                return new(x => x.ModuleReferenceSchemeBrFsNotSupported());
            default:
                return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));
        }
    }

    private bool IsSupportedScheme(string scheme) => this.registryProvider.TryGetRegistry(scheme) is not null;

    private static bool SupportsSchemeQualifiedAliases(string scheme) =>
        scheme is ArtifactReferenceSchemes.Oci or ArtifactReferenceSchemes.TemplateSpecs;

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

                var referenceText = extensionEntry.Value;
                if (referenceText.Contains(':'))
                {
                    // The extension path contains a scheme.
                    return new(referenceText);
                }

                if (config.ConfigFileUri is null)
                {
                    throw new InvalidOperationException("The configuration file URI must be set when trying to resolve an extension reference.");
                }

                // A bare path is relative to the configuration file; rewrite it relative to the referencing file.
                var extensionUri = config.ConfigFileUri.Resolve(referenceText);

                return new(extensionUri.GetPathRelativeTo(referencingFile.FileHandle.Uri));
            default:
                return new(x => x.ExpectedExtensionSpecification());
        }
    }
}
