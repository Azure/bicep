// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Configuration.LegacyModuleAliasMocks;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Loads configuration chains and caches them. Loading runs in steps: locate the leaf file, read each file (parsed files are cached),
/// follow "extends" (<see cref="ConfigurationChainResolver"/>), merge (<see cref="ConfigurationMerger"/>), bind, and check cloud trust.
/// No step throws for bad input; each problem becomes a diagnostic on the file that caused it.
/// </summary>
public class BicepConfigurationManager : IBicepConfigurationManager
{
    /// <param name="Chain">The loaded chain.</param>
    /// <param name="Dependencies">Every file the chain tried to read, including missing ones, so that changing, fixing or creating any of them invalidates the chain.</param>
    private sealed record ChainCacheEntry(IBicepConfigurationChain Chain, ImmutableHashSet<IOUri> Dependencies);

    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();

    private readonly ConcurrentDictionary<IDirectoryHandle, ResultWithDiagnostic<IFileHandle?>> configFileLookupCache = new();
    private readonly ConcurrentDictionary<IOUri, ResultWithDiagnostic<ConfigurationFile>> parsedFileCache = new();
    private readonly ConcurrentDictionary<IOUri, ChainCacheEntry> chainCache = new();
    private readonly IFileExplorer fileExplorer;
    private readonly CloudConfigurationTrustPolicy cloudTrustPolicy;
    private readonly ExplicitConfigurationContext explicitConfigurationContext;

    public BicepConfigurationManager(IFileExplorer fileExplorer, CloudConfigurationTrustPolicy cloudTrustPolicy)
        : this(fileExplorer, cloudTrustPolicy, new ExplicitConfigurationContext())
    {
    }

    public BicepConfigurationManager(IFileExplorer fileExplorer, CloudConfigurationTrustPolicy cloudTrustPolicy, ExplicitConfigurationContext explicitConfigurationContext)
    {
        this.fileExplorer = fileExplorer;
        this.cloudTrustPolicy = cloudTrustPolicy;
        this.explicitConfigurationContext = explicitConfigurationContext;
    }

    public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri) =>
        this.explicitConfigurationContext.ConfigFileUri is { } explicitConfigFileUri
            ? GetConfigurationChainForConfigFile(this.fileExplorer.GetFile(explicitConfigFileUri))
            : DiscoverConfigurationChain(sourceFileUri);

    private IBicepConfigurationChain DiscoverConfigurationChain(IOUri sourceFileUri)
    {
        if (!sourceFileUri.IsFile)
        {
            return GetBuiltInChain([], []);
        }

        var sourceDirectory = this.fileExplorer.GetFile(sourceFileUri).GetParent();

        if (!this.configFileLookupCache.GetOrAdd(sourceDirectory, LookupConfigurationFile).IsSuccess(out var configFileHandle, out var lookupDiagnostic))
        {
            return GetBuiltInChain([], [new(null, lookupDiagnostic)]);
        }

        if (configFileHandle is null)
        {
            return GetBuiltInChain([], []);
        }

        return GetConfigurationChainForConfigFile(configFileHandle);
    }

    private IBicepConfigurationChain GetConfigurationChainForConfigFile(IFileHandle leafConfigFileHandle) =>
        this.chainCache.GetOrAdd(leafConfigFileHandle.Uri, _ => LoadChain(leafConfigFileHandle)).Chain;

    public bool InvalidateFile(IOUri fileUri)
    {
        var invalidated = false;

        // Clients may report only the folder when a folder is deleted, so the URI also covers everything under it.
        // IsBaseOf compares whole path segments, so "base" covers "base/x.json" but not "base.json".
        foreach (var entry in this.chainCache)
        {
            // Only removes the entry that was checked, not one a concurrent load has just added.
            if (entry.Value.Dependencies.Any(fileUri.IsBaseOf) && this.chainCache.TryRemove(entry))
            {
                invalidated = true;
            }
        }

        // Parsed files are only read through chains, so dropping one doesn't make any returned configuration stale by itself.
        foreach (var parsedFileUri in this.parsedFileCache.Keys)
        {
            if (fileUri.IsBaseOf(parsedFileUri))
            {
                this.parsedFileCache.TryRemove(parsedFileUri, out _);
            }
        }

        // Clients don't reliably report whether a file was created or changed, so any event on a bicepconfig.json resets discovery.
        if (string.Equals(fileUri.GetFileName(), LanguageConstants.BicepConfigurationFileName, StringComparison.OrdinalIgnoreCase))
        {
            this.configFileLookupCache.Clear();
            invalidated = true;
        }
        else
        {
            // A deleted folder can be recreated without its configuration file, so forget what was found under it.
            // This doesn't count as invalidation: if the folder still exists, discovery finds the same files, and
            // chains that read files under the folder were already invalidated above.
            foreach (var directory in this.configFileLookupCache.Keys)
            {
                if (fileUri.IsBaseOf(directory.Uri))
                {
                    this.configFileLookupCache.TryRemove(directory, out _);
                }
            }
        }

        return invalidated;
    }

    public void InvalidateAll()
    {
        this.configFileLookupCache.Clear();
        this.parsedFileCache.Clear();
        this.chainCache.Clear();
    }

    /// <param name="configFileUris">The files that were read before loading stopped, so their diagnostics can be published (or cleared).</param>
    private static IBicepConfigurationChain GetBuiltInChain(ImmutableArray<IOUri> configFileUris, ImmutableArray<ConfigurationDiagnostic> diagnostics) =>
        new BicepConfigurationChain(BicepConfiguration.BuiltIn, configFileUris, diagnostics);

    private ChainCacheEntry LoadChain(IFileHandle leafFileHandle)
    {
        var dependencies = new List<IOUri>();
        IBicepConfigurationChain chain;

        try
        {
            chain = LoadChain(leafFileHandle.Uri, dependencies);
        }
        catch (Exception exception)
        {
            // Bad user input is reported by the steps that load the chain; anything that escapes is a Bicep bug, but it must not crash the caller.
            chain = GetBuiltInChain([leafFileHandle.Uri], [new(leafFileHandle.Uri, ConfigDiagnosticBuilder.UnloadableBicepConfigFile(leafFileHandle.Uri, exception.Message))]);
        }

        return new(chain, [.. dependencies]);
    }

    private IBicepConfigurationChain LoadChain(IOUri leafUri, List<IOUri> dependencies)
    {
        var resolution = ConfigurationChainResolver.Resolve(leafUri, uri =>
        {
            dependencies.Add(uri);
            return TryReadFile(uri);
        });

        var fileUris = resolution.Files.Select(file => file.Uri).ToImmutableArray();

        if (resolution.Error is { } chainError)
        {
            return GetBuiltInChain(fileUris, [chainError]);
        }

        var merged = ConfigurationMerger.Merge(BicepConfiguration.BuiltInConfigurationElement, resolution.Files);

        IBicepConfiguration configuration;
        try
        {
            configuration = BicepConfiguration.Bind(merged.Root, leafUri);
        }
        catch (ConfigurationException exception)
        {
            var declaringFileUri = merged.Provenance.GetDeclaringFile(exception.PropertyPath) ?? leafUri;

            return GetBuiltInChain(fileUris, [new(declaringFileUri, ConfigDiagnosticBuilder.InvalidBicepConfigFile(declaringFileUri, exception.Message))]);
        }
        catch (Exception exception) when (exception is JsonException or InvalidOperationException)
        {
            return GetBuiltInChain(fileUris, [new(leafUri, ConfigDiagnosticBuilder.InvalidBicepConfigFile(leafUri, exception.Message))]);
        }

        if (!cloudTrustPolicy.TryGetIsTrusted(configuration.Cloud).IsSuccess(out var isTrusted, out var trustPolicyDiagnostic))
        {
            return GetBuiltInChain(fileUris, [new(null, trustPolicyDiagnostic)]);
        }

        if (!isTrusted)
        {
            var profileName = configuration.Cloud.CurrentProfileName;
            var selectingFileUri =
                merged.Provenance.GetDeclaringFile(ConfigurationPropertyPath.Create(BicepConfiguration.CloudKey, "currentProfile")) ??
                merged.Provenance.GetDeclaringFile(ConfigurationPropertyPath.Create(BicepConfiguration.CloudKey, "profiles", profileName)) ??
                leafUri;

            return GetBuiltInChain(fileUris, [new(selectingFileUri, ConfigDiagnosticBuilder.UntrustedCloudProfile(selectingFileUri, profileName))]);
        }

        return new BicepConfigurationChain(AnnotateDeclaringFiles(configuration, merged.Provenance), fileUris, []);
    }

    private ResultWithDiagnostic<ConfigurationFile>? TryReadFile(IOUri fileUri)
    {
        var fileHandle = this.fileExplorer.GetFile(fileUri);

        if (!fileHandle.Exists())
        {
            return null;
        }

        // A base file shared by several chains is read once.
        return this.parsedFileCache.GetOrAdd(fileUri, _ => ConfigurationFileReader.Read(fileHandle));
    }

    /// <summary>
    /// Records which file declared values whose relative paths, or error messages, depend on it.
    /// </summary>
    private static IBicepConfiguration AnnotateDeclaringFiles(IBicepConfiguration configuration, ConfigurationProvenance provenance)
    {
        var mockDeclaringUris = ImmutableDictionary.CreateBuilder<string, IOUri>(StringComparer.Ordinal);
        foreach (var aliasName in configuration.ModuleAliasesMock.GetOciArtifactModuleAliasesMock().Keys)
        {
            if (provenance.GetDeclaringFile(ConfigurationPropertyPath.Create(BicepConfiguration.ModuleAliasesMockKey, "br", aliasName, "mapToFilePath")) is { } declaringUri)
            {
                mockDeclaringUris[aliasName] = declaringUri;
            }
        }

        if (mockDeclaringUris.Count > 0)
        {
            var annotatedMock = ((ModuleAliasesMockConfiguration)configuration.ModuleAliasesMock).WithDeclaringUris(mockDeclaringUris.ToImmutable());
            configuration = configuration.With(moduleAliasesMock: annotatedMock);
        }

        // Version-constraint diagnostics point users at the file that declared "bicep.version".
        if (configuration.Compiler.Version is not null &&
            provenance.GetDeclaringFile(ConfigurationPropertyPath.Create(BicepConfiguration.CompilerKey, "version")) is { } compilerVersionDeclaringUri)
        {
            var annotatedCompiler = ((CompilerConfiguration)configuration.Compiler).WithDeclaringUri(compilerVersionDeclaringUri);
            configuration = configuration.With(compiler: annotatedCompiler);
        }

        // File alias paths are relative to the file that declared them, and alias errors name it.
        if (configuration.Artifacts is ArtifactsConfiguration { IsDeclared: true } artifacts)
        {
            configuration = configuration.With(artifacts: artifacts.WithProvenance(provenance));
        }

        return configuration;
    }

    private ResultWithDiagnostic<IFileHandle?> LookupConfigurationFile(IDirectoryHandle? directoryToLookup)
    {
        try
        {
            while (directoryToLookup is not null)
            {
                var configFileHandle = directoryToLookup.GetFile(LanguageConstants.BicepConfigurationFileName);

                if (configFileHandle.Exists())
                {
                    return new(configFileHandle);
                }

                directoryToLookup = directoryToLookup.GetParent();
            }
        }
        catch (IOException exception)
        {
            return new(ConfigDiagnosticBuilder.PotentialConfigDirectoryCouldNotBeScanned(directoryToLookup?.Uri, exception.Message));
        }

        return new((IFileHandle?)null);
    }
}
