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

public class BicepConfigurationManager : IBicepConfigurationManager
{
    private const int MaxChainDepth = 64;

    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();

    private readonly ConcurrentDictionary<IOUri, IDirectoryHandle> directoryHandleCache = new();
    private readonly ConcurrentDictionary<IDirectoryHandle, ResultWithDiagnostic<IFileHandle?>> configFileLookupCache = new();
    private readonly ConcurrentDictionary<IFileHandle, ResultWithDiagnostic<IBicepConfigurationChain>> chainCache = new();
    private readonly ConcurrentDictionary<IFileHandle, ImmutableHashSet<IOUri>> chainDependencies = new();
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

        return GetConfigurationChainForConfigFile(configFileHandle);
    }

    private IBicepConfigurationChain GetConfigurationChainForConfigFile(IFileHandle leafConfigFileHandle)
    {
        if (!this.chainCache.GetOrAdd(leafConfigFileHandle, LoadChain).IsSuccess(out var chain, out var loadDiagnostic))
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

    private IBicepConfigurationChain GetBuiltInChain(IEnumerable<IDiagnostic>? diagnostics = null)
    {
        var builtInConfig = GetBuiltInConfiguration(diagnostics);

        return new BicepConfigurationChain(builtInConfig, [builtInConfig]);
    }

    private static IBicepConfiguration GetBuiltInConfiguration(IEnumerable<IDiagnostic>? diagnostics = null) =>
        diagnostics is null
            ? BicepConfiguration.BuiltIn
            : BicepConfiguration.BuiltIn.With(diagnostics: diagnostics);

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

    private IBicepConfigurationChain BuildChain(IOUri leafUri, List<(IFileHandle FileHandle, JsonElement Element)> rawLayers)
    {
        // Merge: built-in first, then base configs in reverse order, leaf last (leaf wins).
        var accumulated = BicepConfiguration.BuiltInConfigurationElement;

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

        if (!cloudTrustPolicy.TryGetIsTrusted(effectiveConfig.Cloud).IsSuccess(out var isTrusted, out var diagnostic))
        {
            return GetBuiltInChain(diagnostics: [diagnostic]);
        }

        if (!isTrusted)
        {
            return GetBuiltInChain(diagnostics: [ConfigDiagnosticBuilder.UntrustedCloudProfile(leafUri, effectiveConfig.Cloud.CurrentProfileName)]);
        }

        // Annotate moduleAliasesMock aliases with the URI of the config file that declared each one.
        var mockDeclaringUris = FindDeclaringUrisByKey(rawLayers, BicepConfiguration.ModuleAliasesMockKey, "br");
        if (mockDeclaringUris.Count > 0)
        {
            var annotatedMock = ((ModuleAliasesMockConfiguration)effectiveConfig.ModuleAliasesMock)
                .WithDeclaringUris(mockDeclaringUris);
            effectiveConfig = effectiveConfig.With(moduleAliasesMock: annotatedMock);
        }

        // Track the config file that declared "bicep.version", so version-constraint diagnostics can point users at the exact file to edit.
        if (FindCompilerVersionDeclaringUri(rawLayers) is { } compilerVersionDeclaringUri)
        {
            var annotatedCompiler = ((CompilerConfiguration)effectiveConfig.Compiler)
                .WithDeclaringUri(compilerVersionDeclaringUri);
            effectiveConfig = effectiveConfig.With(compiler: annotatedCompiler);
        }

        // Build per-layer configs so diagnostics can be attributed to the exact file that caused them.
        var layers = rawLayers
            .Select(layer =>
            {
                try
                {
                    var merged = BicepConfiguration.BuiltInConfigurationElement.Merge(StripExtendsProperty(layer.Element));

                    return BicepConfiguration.Bind(merged, layer.FileHandle.Uri);
                }
                catch (ConfigurationException)
                {
                    return BicepConfiguration.BuiltIn.With(configFileIdentifier: layer.FileHandle.Uri);
                }
            })
            .ToImmutableArray();

        return new BicepConfigurationChain(effectiveConfig, layers);
    }

    /// <summary>
    /// For the object at <paramref name="sectionPath"/>, maps each property name to the URI of the config file that declared it.
    /// Layers are visited leaf-first so the most-derived (leaf) declaration wins.
    /// </summary>
    private static ImmutableDictionary<string, IOUri> FindDeclaringUrisByKey(
        List<(IFileHandle FileHandle, JsonElement Element)> rawLayers,
        params string[] sectionPath)
    {
        var map = ImmutableDictionary.CreateBuilder<string, IOUri>(StringComparer.Ordinal);

        foreach (var (fileHandle, element) in rawLayers) // leaf first
        {
            if (!TryGetSection(element, sectionPath, out var sectionElement) || sectionElement.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            foreach (var property in sectionElement.EnumerateObject())
            {
                map.TryAdd(property.Name, fileHandle.Uri);
            }
        }

        return map.ToImmutable();
    }

    private static bool TryGetSection(JsonElement element, string[] sectionPath, out JsonElement section)
    {
        section = element;
        foreach (var key in sectionPath)
        {
            if (section.ValueKind != JsonValueKind.Object || !section.TryGetProperty(key, out section))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Finds the URI of the config file layer that declares "bicep.version", walking leaf-first so the
    /// most-derived declaration wins. Returns null if no layer declares it.
    /// </summary>
    private static IOUri? FindCompilerVersionDeclaringUri(
        List<(IFileHandle FileHandle, JsonElement Element)> rawLayers)
    {
        foreach (var (fileHandle, element) in rawLayers) // leaf first
        {
            if (element.TryGetProperty(BicepConfiguration.CompilerKey, out var compilerElement) &&
                compilerElement.TryGetProperty("version", out var versionElement) &&
                versionElement.ValueKind != JsonValueKind.Null)
            {
                return fileHandle.Uri;
            }
        }

        return null;
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
