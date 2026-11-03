// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry;

/// <summary>
/// Resolves artifact reference text to an <see cref="ArtifactReference"/>.
/// </summary>
/// <remarks>
/// Resolution is done in this order:
/// <list type="number">
///   <item>parse the text into an <see cref="ArtifactAddress"/>;</item>
///   <item>expand scheme-qualified aliases through <c>moduleAliases</c>;</item>
///   <item>parse the fully qualified body with the parser for the scheme.</item>
/// </list>
/// Redirects (including <c>moduleAliasesMock</c>) are not applied here; they only change what the compiler loads.
/// Registries do not parse references; they only operate on resolved references.
/// </remarks>
public sealed class ArtifactReferenceResolver : IArtifactReferenceResolver
{
    private readonly IArtifactRegistryProvider registryProvider;

    public ArtifactReferenceResolver(IArtifactRegistryProvider registryProvider)
    {
        this.registryProvider = registryProvider;
    }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(BicepSourceFile referencingFile, ArtifactType artifactType, string referenceText) =>
        ArtifactAddress.Parse(referenceText) switch
        {
            ArtifactAddress.Path path => this.TryResolvePath(referencingFile, artifactType, path.Text),
            ArtifactAddress.SchemeQualified schemeQualified => this.TryResolveSchemeQualified(referencingFile, artifactType, schemeQualified.Scheme, schemeQualified.Body, aliasContext: null),
            ArtifactAddress.SchemeAndAliasQualified schemeAndAliasQualified => this.TryResolveSchemeAndAliasQualified(referencingFile, artifactType, schemeAndAliasQualified),
            var address => throw new UnreachableException($"Unexpected artifact address type '{address.GetType().Name}'."),
        };

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
}
