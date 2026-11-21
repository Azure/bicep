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
/// Loads configuration chains and caches them. Loading runs in steps: locate the leaf file, read each file,
/// follow "extends" (<see cref="ConfigurationChainResolver"/>), merge (<see cref="ConfigurationMerger"/>), bind, and check cloud trust.
/// No step throws for bad input; each problem becomes a diagnostic on the file that caused it.
/// </summary>
/// <remarks>
/// Each cache entry records the files it was built from (<see cref="FileDependencies"/>) and is checked against them the first time it's read
/// in each <see cref="FileChangeEpoch"/>; a stale entry is reloaded. Without an epoch, entries are never checked again, as in a one-shot command.
/// The manager returns the same configuration instance until it's reloaded, so callers can compare instances to find out whether a configuration changed.
/// </remarks>
public class BicepConfigurationManager : IBicepConfigurationManager
{
    /// <param name="Value">For the lookup cache, the nearest configuration file, or <see langword="null"/> if there's none. For the chain cache, the loaded chain.</param>
    /// <param name="Dependencies">For the lookup cache, the configuration file path checked in each folder searched.
    /// For the chain cache, every file the chain tried to read, including missing ones.</param>
    /// <param name="CheckedInEpoch">The <see cref="FileChangeEpoch"/> in which the entry was last known to be current.</param>
    private sealed record CacheEntry<TValue>(TValue Value, FileDependencies Dependencies, long CheckedInEpoch);

    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();

    // Keyed by the folder of the source file.
    private readonly ConcurrentDictionary<IOUri, CacheEntry<ResultWithDiagnostic<IFileHandle?>>> configFileLookupCache = new();
    private readonly ConcurrentDictionary<IOUri, CacheEntry<IBicepConfigurationChain>> chainCache = new();
    private readonly IFileExplorer fileExplorer;
    private readonly CloudConfigurationTrustPolicy cloudTrustPolicy;
    private readonly ExplicitConfigurationContext explicitConfigurationContext;
    private readonly FileChangeEpoch? fileChangeEpoch;

    public BicepConfigurationManager(
        IFileExplorer fileExplorer,
        CloudConfigurationTrustPolicy cloudTrustPolicy,
        ExplicitConfigurationContext? explicitConfigurationContext = null,
        FileChangeEpoch? fileChangeEpoch = null)
    {
        this.fileExplorer = fileExplorer;
        this.cloudTrustPolicy = cloudTrustPolicy;
        this.explicitConfigurationContext = explicitConfigurationContext ?? new();
        this.fileChangeEpoch = fileChangeEpoch;
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
        var lookup = GetOrLoad(this.configFileLookupCache, sourceDirectory.Uri, () => LookupConfigurationFile(sourceDirectory));

        if (!lookup.IsSuccess(out var configFileHandle, out var lookupDiagnostic))
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
        GetOrLoad(this.chainCache, leafConfigFileHandle.Uri, () => LoadChain(leafConfigFileHandle));

    private TValue GetOrLoad<TValue>(ConcurrentDictionary<IOUri, CacheEntry<TValue>> cache, IOUri key, Func<(TValue Value, FileDependencies Dependencies)> load)
    {
        // Read before loading, so an entry loaded while a new epoch starts is checked again in that epoch.
        var epoch = this.fileChangeEpoch?.Current ?? 0;

        if (cache.TryGetValue(key, out var cachedEntry))
        {
            if (cachedEntry.CheckedInEpoch == epoch)
            {
                return cachedEntry.Value;
            }

            if (cachedEntry.Dependencies.IsCurrent())
            {
                // Losing a race with a concurrent update only means the entry is checked again.
                cache.TryUpdate(key, cachedEntry with { CheckedInEpoch = epoch }, cachedEntry);

                return cachedEntry.Value;
            }
        }

        var (value, dependencies) = load();
        var loadedEntry = new CacheEntry<TValue>(value, dependencies, epoch);

        // Callers compare instances to find out whether a configuration changed, so concurrent loads must agree:
        // only one entry replaces the one that was read (or fills the empty slot), and every caller returns that one.
        var publishedEntry = cachedEntry is not null && cache.TryUpdate(key, loadedEntry, cachedEntry)
            ? loadedEntry
            : cache.GetOrAdd(key, loadedEntry);

        return publishedEntry.Value;
    }

    /// <param name="configFileUris">The files that were read before loading stopped, so their diagnostics can be published (or cleared).</param>
    private static IBicepConfigurationChain GetBuiltInChain(ImmutableArray<IOUri> configFileUris, ImmutableArray<ConfigurationDiagnostic> diagnostics) =>
        new BicepConfigurationChain(BicepConfiguration.BuiltIn, configFileUris, diagnostics);

    private (IBicepConfigurationChain, FileDependencies) LoadChain(IFileHandle leafFileHandle)
    {
        var dependencies = new FileDependencies.Builder();
        IBicepConfigurationChain chain;

        try
        {
            chain = LoadChain(leafFileHandle.Uri, dependencies);
        }
        catch (Exception exception)
        {
            // Bad user input is reported by the steps that load the chain; anything that escapes is a Bicep bug, but it must not crash the caller.
            chain = GetBuiltInChain([leafFileHandle.Uri], [new(leafFileHandle.Uri, ConfigDiagnosticBuilder.UnloadableBicepConfigFile(leafFileHandle.Uri, exception.Message))]);
            dependencies.AddFailedRead();
        }

        return (chain, dependencies.Build());
    }

    private IBicepConfigurationChain LoadChain(IOUri leafUri, FileDependencies.Builder dependencies)
    {
        var resolution = ConfigurationChainResolver.Resolve(leafUri, uri => TryReadFile(uri, dependencies));

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

    private ResultWithDiagnostic<ConfigurationFile>? TryReadFile(IOUri fileUri, FileDependencies.Builder dependencies)
    {
        var fileHandle = this.fileExplorer.GetFile(fileUri);

        // The version is recorded before the file is read, so a change made while reading makes the chain stale.
        if (dependencies.AddFile(fileHandle) is null)
        {
            return null;
        }

        var file = ConfigurationFileReader.Read(fileHandle);

        // BCP272 here can only mean the file couldn't be read (for example, while it's locked). That may be temporary and
        // doesn't change the file's version, so the chain is retried in the next epoch. A file that was read but is invalid
        // isn't retried: fixing it changes its version.
        if (!file.IsSuccess(out _, out var diagnostic) && diagnostic.Code == "BCP272")
        {
            dependencies.AddFailedRead();
        }

        return file;
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

        return configuration;
    }

    private static (ResultWithDiagnostic<IFileHandle?>, FileDependencies) LookupConfigurationFile(IDirectoryHandle? directoryToLookup)
    {
        var dependencies = new FileDependencies.Builder();

        try
        {
            while (directoryToLookup is not null)
            {
                var configFileHandle = directoryToLookup.GetFile(LanguageConstants.BicepConfigurationFileName);

                // Only whether the file exists matters here; the chain records its content.
                if (dependencies.AddExistence(configFileHandle))
                {
                    return (new(configFileHandle), dependencies.Build());
                }

                directoryToLookup = directoryToLookup.GetParent();
            }
        }
        catch (IOException exception)
        {
            dependencies.AddFailedRead();

            return (new(ConfigDiagnosticBuilder.PotentialConfigDirectoryCouldNotBeScanned(directoryToLookup?.Uri, exception.Message)), dependencies.Build());
        }

        return (new((IFileHandle?)null), dependencies.Build());
    }
}
