// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Resolution;

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
    // Schemes that are only recognized when the "artifactAliasesAndRedirects" experimental feature is enabled.
    private static readonly ImmutableHashSet<string> ExperimentalSchemes = [ArtifactSchemes.Oci, ArtifactSchemes.File];

    private readonly ImmutableArray<string> allSchemes;
    private readonly ImmutableArray<string> stableSchemes;
    private readonly IFileExplorer fileExplorer;
    private readonly IBicepConfigurationManager configurationManager;
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly IRemoteArtifactContextFactory remoteArtifactContextFactory;

    public ArtifactReferenceResolver(
        IEnumerable<IRemoteArtifactRegistry> registries,
        IFileExplorer fileExplorer,
        IBicepConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory,
        IRemoteArtifactContextFactory remoteArtifactContextFactory)
    {
        this.allSchemes = [.. registries.SelectMany(registry => registry.Schemes).Append(ArtifactSchemes.File).Distinct().OrderBy(scheme => scheme, StringComparer.Ordinal)];
        this.stableSchemes = [.. this.allSchemes.Where(scheme => !ExperimentalSchemes.Contains(scheme))];
        this.fileExplorer = fileExplorer;
        this.configurationManager = configurationManager;
        this.featureProviderFactory = featureProviderFactory;
        this.remoteArtifactContextFactory = remoteArtifactContextFactory;
    }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(IOUri referencingUri, ArtifactType artifactType, string referenceText) =>
        ArtifactAddress.Parse(referenceText) switch
        {
            ArtifactAddress.Path path => this.TryResolvePath(referencingUri, artifactType, path.Text, path.Text),
            ArtifactAddress.SchemeQualified schemeQualified => this.TryResolveSchemeQualified(referencingUri, artifactType, schemeQualified.Scheme, schemeQualified.Body, aliasContext: null),
            ArtifactAddress.SchemeAndAliasQualified schemeAndAliasQualified => this.TryResolveSchemeAndAliasQualified(referencingUri, artifactType, schemeAndAliasQualified),
            // Artifact aliases are not supported yet. Until they are, "<alias>::<body>" is resolved as the scheme
            // "<alias>" followed by the body ":<body>", which is never valid.
            ArtifactAddress.AliasQualified aliasQualified => this.TryResolveSchemeQualified(referencingUri, artifactType, aliasQualified.Alias, $":{aliasQualified.Body}", aliasContext: null),
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
            case ArtifactSchemes.Br:
                if (!configuration.ModuleAliases.TryGetOciArtifactModuleAlias(aliasName).IsSuccess(out var ociAlias, out var ociFailureBuilder))
                {
                    return new(ociFailureBuilder);
                }

                return new($"{ociAlias}/{body}");
            case ArtifactSchemes.TemplateSpecs:
                if (!configuration.ModuleAliases.TryGetTemplateSpecModuleAlias(aliasName).IsSuccess(out var templateSpecAlias, out var templateSpecFailureBuilder))
                {
                    return new(templateSpecFailureBuilder);
                }

                return new($"{templateSpecAlias}/{body}");
            default:
                throw new ArgumentException($"Scheme '{scheme}' does not support scheme-qualified aliases.", nameof(scheme));
        }
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolvePath(IOUri referencingUri, ArtifactType artifactType, string path, string sourceText)
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

        return new(new LocalArtifactReference(artifactType, file, sourceText));
    }

    /// <summary>
    /// Resolves a <c>file:</c> reference. Unlike a bare path, a module must name a file whose extension says what kind of
    /// file it is: a Bicep file or an ARM template.
    /// </summary>
    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveFile(IOUri referencingUri, ArtifactType artifactType, string path)
    {
        var sourceText = $"{ArtifactSchemes.File}:{path}";

        if (!this.TryResolvePath(referencingUri, artifactType, path, sourceText).IsSuccess(out var reference, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        if (artifactType == ArtifactType.Module &&
            reference is LocalArtifactReference { File.Uri: var uri } &&
            !(uri.HasBicepExtension() || uri.HasBicepParamExtension() || uri.HasArmTemplateLikeExtension()))
        {
            return new(x => x.LocalModuleFileExtensionNotSupported(sourceText));
        }

        return new(reference);
    }

    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveSchemeAndAliasQualified(IOUri referencingUri, ArtifactType artifactType, ArtifactAddress.SchemeAndAliasQualified address)
    {
        if (!this.GetSupportedSchemes(referencingUri).Contains(address.Scheme))
        {
            // Reported the same way as a fully qualified reference with an unsupported scheme.
            return this.TryResolveSchemeQualified(referencingUri, artifactType, address.Scheme, address.Body, aliasContext: null);
        }

        if (!SupportsSchemeQualifiedAliases(address.Scheme))
        {
            return new(x => x.SchemeQualifiedAliasNotSupported(address.Scheme));
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

        var supportedSchemes = this.GetSupportedSchemes(referencingUri);
        if (!supportedSchemes.Contains(scheme))
        {
            return new(x => x.UnknownModuleReferenceScheme(scheme, supportedSchemes));
        }

        switch (scheme)
        {
            case ArtifactSchemes.Br:
            case ArtifactSchemes.Oci:
                return OciArtifactReference.TryParse(this.remoteArtifactContextFactory.GetOciArtifactContext(referencingUri), artifactType, aliasContext, body, scheme).Transform(x => (ArtifactReference)x);
            case ArtifactSchemes.TemplateSpecs:
                if (artifactType != ArtifactType.Module)
                {
                    return new(x => x.UnsupportedArtifactType(artifactType));
                }

                return TemplateSpecArtifactReference.TryParse(this.remoteArtifactContextFactory.GetTemplateSpecArtifactContext(referencingUri), aliasContext, body).Transform(x => (ArtifactReference)x);
            case ArtifactSchemes.File:
                return this.TryResolveFile(referencingUri, artifactType, body);
            default:
                return new(x => x.UnknownModuleReferenceScheme(scheme, supportedSchemes));
        }
    }

    private IBicepConfiguration GetConfiguration(IOUri referencingUri) => this.configurationManager.GetEffectiveConfiguration(referencingUri);

    private ImmutableArray<string> GetSupportedSchemes(IOUri referencingUri) =>
        this.featureProviderFactory.GetFeatureProvider(referencingUri).ArtifactAliasesAndRedirectsEnabled ? this.allSchemes : this.stableSchemes;

    private static bool SupportsSchemeQualifiedAliases(string scheme) =>
        scheme is ArtifactSchemes.Br or ArtifactSchemes.TemplateSpecs;
}
