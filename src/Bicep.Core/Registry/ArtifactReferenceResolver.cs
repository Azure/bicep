// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

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
    private readonly IFileExplorer fileExplorer;
    private readonly IBicepConfigurationManager configurationManager;
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly IExternalArtifactContextFactory externalArtifactContextFactory;

    public ArtifactReferenceResolver(
        IArtifactRegistryProvider registryProvider,
        IFileExplorer fileExplorer,
        IBicepConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory,
        IExternalArtifactContextFactory externalArtifactContextFactory)
    {
        this.registryProvider = registryProvider;
        this.fileExplorer = fileExplorer;
        this.configurationManager = configurationManager;
        this.featureProviderFactory = featureProviderFactory;
        this.externalArtifactContextFactory = externalArtifactContextFactory;
    }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(IOUri referencingUri, ArtifactType artifactType, string referenceText) =>
        ArtifactAddress.Parse(referenceText) switch
        {
            ArtifactAddress.Path path => this.TryResolvePath(referencingUri, artifactType, path.Text),
            ArtifactAddress.SchemeQualified schemeQualified => this.TryResolveSchemeQualified(referencingUri, artifactType, schemeQualified.Scheme, schemeQualified.Body, aliasContext: null),
            ArtifactAddress.SchemeAndAliasQualified schemeAndAliasQualified => this.TryResolveSchemeAndAliasQualified(referencingUri, artifactType, schemeAndAliasQualified),
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

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolvePath(IOUri referencingUri, ArtifactType artifactType, string path)
    {
        if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Extension)
        {
            return new(x => x.UnsupportedArtifactType(artifactType));
        }

        if (!RelativePath.TryCreate(path).IsSuccess(out var relativePath, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        if (!referencingUri.IsFile)
        {
            // Relative paths can only be resolved from files on disk, not from in-memory files such as snippets or archived sources.
            return new(x => x.ErrorOccurredReadingFile(new FileNotFoundException().Message));
        }

        if (!this.fileExplorer.GetFile(referencingUri).TryGetRelativeFile(relativePath).IsSuccess(out var file, out failureBuilder))
        {
            return new(failureBuilder);
        }

        // A local extension package is unpacked into the artifact cache.
        var cacheRootDirectory = this.featureProviderFactory.GetFeatureProvider(referencingUri).CacheRootDirectory;

        return new(new LocalArtifactReference(cacheRootDirectory, artifactType, file, path));
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveSchemeAndAliasQualified(IOUri referencingUri, ArtifactType artifactType, ArtifactAddress.SchemeAndAliasQualified address)
    {
        if (!this.IsSupportedScheme(address.Scheme) || !SupportsSchemeQualifiedAliases(address.Scheme))
        {
            // Reported the same way as a fully qualified reference with an unsupported scheme.
            return this.TryResolveSchemeQualified(referencingUri, artifactType, address.Scheme, address.Body, aliasContext: null);
        }

        if (!TryExpandSchemeQualifiedAlias(this.GetConfiguration(referencingUri), artifactType, address.Scheme, address.Alias, address.Body).IsSuccess(out var expandedBody, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        return this.TryResolveSchemeQualified(referencingUri, artifactType, address.Scheme, expandedBody, aliasContext: address.Alias);
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveSchemeQualified(IOUri referencingUri, ArtifactType artifactType, string scheme, string body, string? aliasContext)
    {
        if (scheme == OciArtifactReferenceFacts.MockedScheme)
        {
            return new(x => x.ModuleReferenceSchemeBrFsNotSupported());
        }

        if (!this.IsSupportedScheme(scheme))
        {
            return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));
        }

        switch (scheme)
        {
            case ArtifactReferenceSchemes.Oci:
                return OciArtifactReference.TryParse(this.externalArtifactContextFactory.GetOciArtifactContext(referencingUri), artifactType, aliasContext, body).Transform(x => (ArtifactReference)x);
            case ArtifactReferenceSchemes.TemplateSpecs:
                if (artifactType != ArtifactType.Module)
                {
                    return new(x => x.UnsupportedArtifactType(artifactType));
                }

                return TemplateSpecModuleReference.TryParse(this.externalArtifactContextFactory.GetTemplateSpecArtifactContext(referencingUri), aliasContext, body).Transform(x => (ArtifactReference)x);
            default:
                return new(x => x.UnknownModuleReferenceScheme(scheme, this.registryProvider.SupportedSchemes));
        }
    }

    private IBicepConfiguration GetConfiguration(IOUri referencingUri) => this.configurationManager.GetEffectiveConfiguration(referencingUri);

    private bool IsSupportedScheme(string scheme) => this.registryProvider.TryGetRegistry(scheme) is not null;

    private static bool SupportsSchemeQualifiedAliases(string scheme) =>
        scheme is ArtifactReferenceSchemes.Oci or ArtifactReferenceSchemes.TemplateSpecs;
}
