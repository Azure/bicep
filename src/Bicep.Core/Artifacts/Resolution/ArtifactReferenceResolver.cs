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
///   <item>expand scheme-qualified aliases through <c>moduleAliases</c>, and artifact aliases through <c>artifacts.aliases</c>
///   and the predefined aliases. The two never fall back to each other;</item>
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
            ArtifactAddress.AliasQualified aliasQualified when this.IsArtifactAliasesAndRedirectsEnabled(referencingUri) => this.TryResolveAliasQualified(referencingUri, artifactType, aliasQualified),
            // Without the experimental feature, "<alias>::<body>" is resolved as the scheme "<alias>" followed by the body
            // ":<body>", which is never valid.
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
        if (!referencingUri.IsFile)
        {
            if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Extension)
            {
                return new(x => x.UnsupportedArtifactType(artifactType));
            }

            if (!RelativePath.TryCreate(path).IsSuccess(out _, out var pathFailureBuilder))
            {
                return new(pathFailureBuilder);
            }

            // Relative paths can only be resolved from files on disk, not from in-memory files such as snippets or archived sources.
            return new(x => x.ErrorOccurredReadingFile(new FileNotFoundException().Message));
        }

        return this.TryResolveLocalFile(() => this.fileExplorer.GetFile(referencingUri).GetParent(), artifactType, path, sourceText, requireKnownModuleExtension: false);
    }

    /// <summary>
    /// Resolves <paramref name="path"/> relative to a directory.
    /// </summary>
    /// <param name="getDirectory">Gets the directory. Only called once the path is known to be valid, and can throw <see cref="IOException"/>.</param>
    /// <param name="requireKnownModuleExtension">
    /// True if a module must name a file whose extension says what kind of file it is: a Bicep file or an ARM template.
    /// Bare paths don't require it, so that existing references keep working.
    /// </param>
    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveLocalFile(Func<IDirectoryHandle> getDirectory, ArtifactType artifactType, string path, string sourceText, bool requireKnownModuleExtension)
    {
        if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Extension)
        {
            return new(x => x.UnsupportedArtifactType(artifactType));
        }

        if (!RelativePath.TryCreate(path).IsSuccess(out var relativePath, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        IDirectoryHandle directory;
        try
        {
            directory = getDirectory();
        }
        catch (IOException exception)
        {
            return new(x => x.ErrorOccurredReadingFile(exception.Message));
        }

        if (!directory.TryGetRelativeFile(relativePath).IsSuccess(out var file, out failureBuilder))
        {
            return new(failureBuilder);
        }

        if (requireKnownModuleExtension &&
            artifactType == ArtifactType.Module &&
            !(file.Uri.HasBicepExtension() || file.Uri.HasBicepParamExtension() || file.Uri.HasArmTemplateLikeExtension()))
        {
            return new(x => x.LocalModuleFileExtensionNotSupported(sourceText));
        }

        return new(new LocalArtifactReference(artifactType, file, sourceText));
    }

    /// <summary>
    /// Resolves a <c>file:</c> reference relative to the referencing file.
    /// </summary>
    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveFile(IOUri referencingUri, ArtifactType artifactType, string path)
    {
        var sourceText = $"{ArtifactSchemes.File}:{path}";

        if (!referencingUri.IsFile)
        {
            return this.TryResolvePath(referencingUri, artifactType, path, sourceText);
        }

        return this.TryResolveLocalFile(() => this.fileExplorer.GetFile(referencingUri).GetParent(), artifactType, path, sourceText, requireKnownModuleExtension: true);
    }

    /// <summary>
    /// Expands an artifact alias (<c>&lt;alias&gt;::</c>) through <c>artifacts.aliases</c> or the predefined aliases, using the
    /// configuration of the referencing file.
    /// </summary>
    private ResultWithDiagnosticBuilder<ArtifactReference> TryResolveAliasQualified(IOUri referencingUri, ArtifactType artifactType, ArtifactAddress.AliasQualified address)
    {
        if (!this.GetConfiguration(referencingUri).Artifacts.TryGetAlias(address.Alias).IsSuccess(out var alias, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        var body = address.Body;

        switch (alias)
        {
            case OciArtifactAlias ociAlias:
                string unqualifiedReference;
                if (body.Length == 0)
                {
                    return new(x => x.ArtifactAliasPathMissing(address.Text, alias.Name, versionOnly: false));
                }
                else if (body[0] is ':' or '@')
                {
                    // The "storage:::1.0.0" form: the alias names the complete repository and the body only the tag or digest.
                    if (ociAlias.RepositoryPrefix is null)
                    {
                        return new(x => x.ArtifactAliasPathMissing(address.Text, alias.Name, versionOnly: true));
                    }

                    unqualifiedReference = $"{ociAlias.Registry}/{ociAlias.RepositoryPrefix}{body}";
                }
                else
                {
                    unqualifiedReference = ociAlias.RepositoryPrefix is null
                        ? $"{ociAlias.Registry}/{body}"
                        : $"{ociAlias.Registry}/{ociAlias.RepositoryPrefix}/{body}";
                }

                return this.TryResolveSchemeQualified(referencingUri, artifactType, ArtifactSchemes.Oci, unqualifiedReference, aliasContext: alias.Name);

            case TemplateSpecArtifactAlias templateSpecAlias:
                if (artifactType != ArtifactType.Module)
                {
                    return new(x => x.UnsupportedArtifactType(artifactType));
                }

                if (body.Length == 0 || body[0] == ':')
                {
                    return new(x => x.ArtifactAliasPathMissing(address.Text, alias.Name, versionOnly: false));
                }

                return this.TryResolveSchemeQualified(referencingUri, artifactType, ArtifactSchemes.TemplateSpecs, $"{templateSpecAlias.SubscriptionId}/{templateSpecAlias.ResourceGroup}/{body}", aliasContext: alias.Name);

            case FileArtifactAlias fileAlias:
                if (body.Length == 0)
                {
                    return new(x => x.ArtifactAliasPathMissing(address.Text, alias.Name, versionOnly: false));
                }

                // The alias root is relative to the configuration file that declared it, not to the referencing file.
                return this.TryResolveLocalFile(
                    () => this.fileExplorer.GetFile(fileAlias.DeclaringConfigUri).GetParent().GetDirectory(fileAlias.Path),
                    artifactType,
                    body,
                    address.Text,
                    requireKnownModuleExtension: true);

            default:
                throw new UnreachableException($"Unexpected artifact alias type '{alias.GetType().Name}'.");
        }
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
                return OciArtifactReference.TryParse(this.remoteArtifactContextFactory.GetOciArtifactContext(referencingUri), artifactType, aliasContext, scheme, body).Transform(x => (ArtifactReference)x);
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
        this.IsArtifactAliasesAndRedirectsEnabled(referencingUri) ? this.allSchemes : this.stableSchemes;

    private bool IsArtifactAliasesAndRedirectsEnabled(IOUri referencingUri) =>
        this.featureProviderFactory.GetFeatureProvider(referencingUri).ArtifactAliasesAndRedirectsEnabled;

    private static bool SupportsSchemeQualifiedAliases(string scheme) =>
        scheme is ArtifactSchemes.Br or ArtifactSchemes.TemplateSpecs;
}
