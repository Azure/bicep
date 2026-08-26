// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

public class BicepConfigurationManager : IBicepConfigurationManager, IConfigurationManager
{
    private const int MaxChainDepth = 64;

    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();

    private readonly ConcurrentDictionary<IOUri, IDirectoryHandle> directoryHandleCache = new();
    private readonly ConcurrentDictionary<IDirectoryHandle, ResultWithDiagnostic<IFileHandle?>> configFileLookupCache = new();
    private readonly ConcurrentDictionary<IFileHandle, ResultWithDiagnostic<IBicepConfigurationChain>> chainCache = new();
    private readonly ConcurrentDictionary<IFileHandle, ImmutableHashSet<IOUri>> chainDependencies = new();
    private readonly IFileExplorer fileExplorer;

    public BicepConfigurationManager(IFileExplorer fileExplorer)
    {
        this.fileExplorer = fileExplorer;
    }

    public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri)
    {
        if (!sourceFileUri.IsFile)
        {
            return GetBuiltInChain();
        }

        var sourceDirectory = this.directoryHandleCache.GetOrAdd(
            sourceFileUri,
            uri => this.fileExplorer.GetFile(uri).GetParent());

        if (!this.configFileLookupCache.GetOrAdd(sourceDirectory, LookupConfigurationFile).IsSuccess(out var configFileHandle, out var lookupDiagnostic))
        {
            return GetBuiltInChain(diagnostics: [lookupDiagnostic]);
        }

        if (configFileHandle is null)
        {
            return GetBuiltInChain();
        }

        if (!this.chainCache.GetOrAdd(configFileHandle, LoadChain).IsSuccess(out var chain, out var loadDiagnostic))
        {
            return GetBuiltInChain(diagnostics: [loadDiagnostic]);
        }

        return chain;
    }

    public void PurgeAllCaches()
    {
        this.directoryHandleCache.Clear();
        this.configFileLookupCache.Clear();
        this.chainCache.Clear();
        this.chainDependencies.Clear();
    }

    public void PurgeCache()
    {
        PurgeLookupCache();
        this.chainCache.Clear();
        this.chainDependencies.Clear();
    }

    public void PurgeLookupCache() => this.configFileLookupCache.Clear();

    public void PurgeChainCache()
    {
        this.chainCache.Clear();
        this.chainDependencies.Clear();
    }

    public void PurgeCacheForAffectedChains(IOUri changedFileUri)
    {
        foreach (var (leafHandle, deps) in this.chainDependencies)
        {
            if (deps.Contains(changedFileUri))
            {
                this.chainCache.TryRemove(leafHandle, out _);
                this.chainDependencies.TryRemove(leafHandle, out _);
                // Lookup cache must also be cleared so the next GetConfigurationChain
                // re-resolves the leaf file from its source directory.
                this.configFileLookupCache.Clear();
            }
        }
    }

    /// <summary>
    /// Returns the set of config file URIs that the chain rooted at <paramref name="leafUri"/> depends on.
    /// Exposed internally for testing.
    /// </summary>
    internal ImmutableHashSet<IOUri> GetDependenciesForLeaf(IOUri leafUri)
    {
        var leafHandle = this.fileExplorer.GetFile(leafUri);
        return this.chainDependencies.TryGetValue(leafHandle, out var deps) ? deps : [];
    }

    public IBicepConfiguration GetMergedConfiguration(IOUri sourceFileUri)
    {
        var chain = GetConfigurationChain(sourceFileUri);

        return chain.GetEffectiveConfiguration();
    }

    /// <summary>
    /// Satisfies <see cref="IConfigurationManager"/>. Returns the fully merged effective
    /// configuration for the given source file (walking the "extends" chain).
    /// </summary>
    public IBicepConfiguration GetConfiguration(IOUri sourceFileUri) => GetMergedConfiguration(sourceFileUri);

    public void RemoveChainCacheEntry(IOUri configFileUri)
    {
        var configFileHandle = this.fileExplorer.GetFile(configFileUri);

        if (this.chainCache.TryRemove(configFileHandle, out _))
        {
            PurgeLookupCache();
        }
    }

    private static IBicepConfigurationChain GetBuiltInChain(IEnumerable<IDiagnostic>? diagnostics = null)
    {
        var builtInConfig = GetBuiltInConfiguration(diagnostics);

        return new BicepConfigurationChain(builtInConfig, [builtInConfig]);
    }

    private static IBicepConfiguration GetBuiltInConfiguration(IEnumerable<IDiagnostic>? diagnostics = null) =>
        diagnostics is null
            ? IConfigurationManager.GetBuiltInConfiguration()
            : IConfigurationManager.GetBuiltInConfiguration().With(diagnostics: diagnostics);

    private ResultWithDiagnostic<IBicepConfigurationChain> LoadChain(IFileHandle leafFileHandle)
    {
        var leafUri = leafFileHandle.Uri;
        var rawLayers = new List<(IFileHandle FileHandle, JsonElement Element)>();
        var visited = new HashSet<IOUri>();
        var currentFileHandle = leafFileHandle;

        while (true)
        {
            JsonElement currentElement;
            try
            {
                using var stream = currentFileHandle.OpenRead();
                currentElement = JsonElementFactory.CreateElementFromStream(stream);
            }
            catch (JsonException exception)
            {
                return new(ConfigDiagnosticBuilder.UnparsableBicepConfigFile(currentFileHandle.Uri, exception.Message));
            }
            catch (Exception exception)
            {
                return new(ConfigDiagnosticBuilder.UnloadableBicepConfigFile(currentFileHandle.Uri, exception.Message));
            }

            rawLayers.Add((currentFileHandle, currentElement));
            visited.Add(currentFileHandle.Uri);

            if (!currentElement.TryGetProperty("extends", out var extendsElement) ||
                extendsElement.ValueKind == JsonValueKind.Null)
            {
                break;
            }

            var extendsPath = extendsElement.GetString();

            if (string.IsNullOrWhiteSpace(extendsPath))
            {
                break;
            }

            if (IOUri.IsAbsoluteFilePath(extendsPath))
            {
                return new(ConfigDiagnosticBuilder.BicepConfigExtendsAbsolutePath(currentFileHandle.Uri));
            }

            var nextUri = currentFileHandle.Uri.Resolve(extendsPath);

            if (visited.Contains(nextUri))
            {
                var cycleDisplay = string.Join(" -> ", visited.Select(u => u.ToString())) + " -> " + nextUri;

                return new(ConfigDiagnosticBuilder.BicepConfigExtendsCycle(leafUri, cycleDisplay));
            }

            if (rawLayers.Count >= MaxChainDepth)
            {
                return new(ConfigDiagnosticBuilder.BicepConfigExtendsChainTooDeep(leafUri));
            }

            var nextFileHandle = this.fileExplorer.GetFile(nextUri);

            if (!nextFileHandle.Exists())
            {
                return new(ConfigDiagnosticBuilder.UnloadableBicepConfigFile(nextUri, "File not found."));
            }

            currentFileHandle = nextFileHandle;
        }

        // Record all config files this chain depends on for targeted cache invalidation.
        this.chainDependencies[leafFileHandle] = rawLayers
            .Select(layer => layer.FileHandle.Uri)
            .ToImmutableHashSet();

        return new(BuildChain(leafUri, rawLayers));
    }

    private static IBicepConfigurationChain BuildChain(IOUri leafUri, List<(IFileHandle FileHandle, JsonElement Element)> rawLayers)
    {
        // Merge: built-in first, then base configs in reverse order, leaf last (leaf wins).
        var accumulated = IConfigurationManager.BuiltInConfigurationElement;

        foreach (var (_, element) in Enumerable.Reverse(rawLayers))
        {
            accumulated = accumulated.Merge(StripExtendsProperty(element));
        }

        IBicepConfiguration effectiveConfig;
        try
        {
            effectiveConfig = BicepConfiguration.Bind(accumulated, leafUri);
        }
        catch (ConfigurationException exception)
        {
            return GetBuiltInChain(diagnostics: [DiagnosticBuilder.ForDocumentStart().InvalidBicepConfigFile(leafUri, exception.Message)]);
        }

        // Build per-layer configs so diagnostics can be attributed to the exact file that caused them.
        var layers = rawLayers
            .Select(layer =>
            {
                try
                {
                    var merged = IConfigurationManager.BuiltInConfigurationElement.Merge(StripExtendsProperty(layer.Element));

                    return (IBicepConfiguration)BicepConfiguration.Bind(merged, layer.FileHandle.Uri);
                }
                catch (ConfigurationException)
                {
                    return IConfigurationManager.GetBuiltInConfiguration().With(configFileIdentifier: layer.FileHandle.Uri);
                }
            })
            .ToImmutableArray();

        return new BicepConfigurationChain(effectiveConfig, layers);
    }

    private static JsonElement StripExtendsProperty(JsonElement element)
    {
        if (!element.TryGetProperty("extends", out _))
        {
            return element;
        }

        var bufferWriter = new System.Buffers.ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(bufferWriter);

        writer.WriteStartObject();
        foreach (var property in element.EnumerateObject())
        {
            if (!string.Equals(property.Name, "extends", StringComparison.Ordinal))
            {
                property.WriteTo(writer);
            }
        }
        writer.WriteEndObject();
        writer.Flush();

        return JsonElementFactory.CreateElement(bufferWriter.WrittenMemory);
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
