// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.LegacyModuleAliasMocks;

/// <summary>
/// Resolves <c>br/&lt;alias&gt;:</c> module references whose alias is configured in <c>moduleAliasesMock</c> to local files.
/// </summary>
/// <remarks>
/// All <c>moduleAliasesMock</c> behavior is isolated here so the feature can be deleted without touching shared code
/// (an architecture test enforces this). To delete it, remove:
/// <list type="bullet">
///   <item>the <c>LegacyModuleAliasMocks</c> folders in product code and tests;</item>
///   <item>the call to this resolver in <see cref="ArtifactReferenceResolver"/>;</item>
///   <item>the <c>ModuleAliasesMock</c> configuration property, its bind/<c>With</c>/<c>WriteTo</c> code and its declaring-URI registration;</item>
///   <item>the <c>moduleAliasesMock</c> placeholder in the built-in <c>bicepconfig.json</c> and in the schema;</item>
///   <item>diagnostic methods that become unused (their BCP codes stay reserved).</item>
/// </list>
/// </remarks>
internal sealed class ModuleAliasMockResolver
{
    private readonly IFileExplorer fileExplorer;

    public ModuleAliasMockResolver(IFileExplorer fileExplorer)
    {
        this.fileExplorer = fileExplorer;
    }

    /// <summary>
    /// Resolves a <c>br/&lt;alias&gt;:</c> reference through <c>moduleAliasesMock</c>.
    /// Returns <c>null</c> if no mock is configured for the alias, in which case the reference is resolved normally.
    /// </summary>
    public ResultWithDiagnosticBuilder<ArtifactReference>? TryResolve(BicepSourceFile referencingFile, ArtifactType artifactType, string aliasName, string unqualifiedReference)
    {
        var configuration = referencingFile.LoadConfiguration();
        if (!configuration.ModuleAliasesMock.TryGetOciArtifactModuleAliasMock(aliasName).IsSuccess(out var mockAlias, out _))
        {
            return null;
        }

        if (artifactType != ArtifactType.Module)
        {
            return new(x => x.OciArtifactModuleAliasMapToFilePathOnlySupportsModules(aliasName));
        }

        if (configuration.ConfigFileUri is not { } configFileUri)
        {
            return new(x => x.ConfigurationFileNotFound("OciModuleAliasesMock"));
        }

        if (mockAlias.MapToFilePath is null)
        {
            return new(x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, configFileUri));
        }

        // Use the declaring config's URI so that a relative mapToFilePath inherited from a base
        // config is resolved from that base config's directory, not the leaf's.
        var resolveBaseUri = mockAlias.DeclaringConfigUri ?? configFileUri;

        if (!TryCreateReference(referencingFile, mockAlias.MapToFilePath, resolveBaseUri, unqualifiedReference, this.fileExplorer, aliasName).IsSuccess(out var mockedReference, out var failureBuilder))
        {
            return new(failureBuilder);
        }

        return new(mockedReference);
    }

    /// <summary>
    /// Extracts the module path from an unqualified reference by removing any tag or digest suffix.
    /// </summary>
    public static string ExtractModulePath(string unqualifiedReference)
    {
        var digestIndex = unqualifiedReference.IndexOf('@');
        if (digestIndex >= 0)
        {
            return unqualifiedReference[..digestIndex];
        }

        var tagIndex = unqualifiedReference.LastIndexOf(':');
        if (tagIndex >= 0)
        {
            return unqualifiedReference[..tagIndex];
        }

        return unqualifiedReference;
    }

    /// <summary>
    /// Creates a reference to <c>&lt;mapToFilePath&gt;/&lt;module path&gt;.bicep</c>, where a relative
    /// <paramref name="mapToFilePath"/> is resolved against <paramref name="configFileUri"/>.
    /// </summary>
    public static ResultWithDiagnosticBuilder<OciArtifactMockedReference> TryCreateReference(
        BicepSourceFile referencingFile,
        string mapToFilePath,
        IOUri configFileUri,
        string unqualifiedReference,
        IFileExplorer fileExplorer,
        string? aliasName = null)
    {
        var modulePath = ExtractModulePath(unqualifiedReference);

        if (string.IsNullOrEmpty(modulePath))
        {
            return new(x => x.ModulePathHasNotBeenSpecified());
        }

        foreach (var segment in modulePath.Split('/'))
        {
            if (!OciArtifactReferenceFacts.IsOciNamespaceSegment(segment))
            {
                return new(x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, unqualifiedReference, segment));
            }
        }

        // Ensure the mapToFilePath path ends with '/' so it's treated as a directory.
        var directoryPath = mapToFilePath.EndsWith('/') || mapToFilePath.EndsWith('\\')
             ? mapToFilePath
             : mapToFilePath + "/";

        IOUri baseUri;
        if (IOUri.IsAbsoluteFilePath(mapToFilePath))
        {
            try
            {
                baseUri = IOUri.FromFilePath(directoryPath);
            }
            catch (IOException ex)
            {
                return new(x => x.InvalidOciArtifactModuleAliasMapToFilePath(aliasName, mapToFilePath, ex.Message));
            }
        }
        else
        {
            baseUri = configFileUri.Resolve(directoryPath);
        }

        var moduleFileUri = baseUri.Resolve(modulePath + ".bicep");

        return new(new OciArtifactMockedReference(referencingFile, modulePath, fileExplorer.GetFile(moduleFileUri)));
    }
}
