// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Configuration.LegacyModuleAliasMocks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

public class BicepConfigurationManager : IBicepConfigurationManager
{
    private const int MaxChainDepth = 64;

    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();

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

        var sourceDirectory = this.fileExplorer.GetFile(sourceFileUri).GetParent();

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

    public bool InvalidateFile(IOUri fileUri)
    {
        var invalidated = false;

        // Clients may report only the folder when a folder is deleted, so the URI also covers everything under it.
        // IsBaseOf compares whole path segments, so "base" covers "base/x.json" but not "base.json".
        foreach (var (leafHandle, dependencies) in this.chainDependencies)
        {
            if (dependencies.Any(fileUri.IsBaseOf))
            {
                this.chainCache.TryRemove(leafHandle, out _);
                this.chainDependencies.TryRemove(leafHandle, out _);
                invalidated = true;
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
        this.chainCache.Clear();
        this.chainDependencies.Clear();
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
        var chainFileUris = new List<IOUri>();

        try
        {
            return LoadChain(leafFileHandle, chainFileUris);
        }
        catch (Exception exception)
        {
            // Bad user input is reported by LoadChain itself; anything that escapes is a Bicep bug, but it must not crash the caller.
            return new(ConfigDiagnosticBuilder.UnloadableBicepConfigFile(leafFileHandle.Uri, exception.Message));
        }
        finally
        {
            // Record the files read so far even when loading fails, so that fixing any of them invalidates the cached failure.
            this.chainDependencies[leafFileHandle] = [.. chainFileUris];
        }
    }

    private ResultWithDiagnostic<IBicepConfigurationChain> LoadChain(IFileHandle leafFileHandle, List<IOUri> chainFileUris)
    {
        var leafUri = leafFileHandle.Uri;
        var rawLayers = new List<(IFileHandle FileHandle, JsonElement Element)>();
        var currentFileHandle = leafFileHandle;

        while (true)
        {
            chainFileUris.Add(currentFileHandle.Uri);

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

            if (currentElement.ValueKind != JsonValueKind.Object)
            {
                return new(ConfigDiagnosticBuilder.InvalidBicepConfigFile(currentFileHandle.Uri, "The configuration must be a JSON object."));
            }

            rawLayers.Add((currentFileHandle, currentElement));

            if (!currentElement.TryGetProperty("extends", out var extendsElement) ||
                extendsElement.ValueKind == JsonValueKind.Null)
            {
                break;
            }

            if (extendsElement.ValueKind != JsonValueKind.String)
            {
                return new(ConfigDiagnosticBuilder.BicepConfigExtendsMustBeRelativePath(currentFileHandle.Uri));
            }

            var extendsPath = extendsElement.GetString();

            if (string.IsNullOrWhiteSpace(extendsPath))
            {
                break;
            }

            if (IOUri.IsAbsoluteFilePath(extendsPath))
            {
                return new(ConfigDiagnosticBuilder.BicepConfigExtendsMustBeRelativePath(currentFileHandle.Uri));
            }

            var nextUri = currentFileHandle.Uri.Resolve(extendsPath);

            if (chainFileUris.Contains(nextUri))
            {
                var cycleDisplay = string.Join(" -> ", chainFileUris.Append(nextUri));

                return new(ConfigDiagnosticBuilder.BicepConfigExtendsCycle(leafUri, cycleDisplay));
            }

            if (rawLayers.Count >= MaxChainDepth)
            {
                return new(ConfigDiagnosticBuilder.BicepConfigExtendsChainTooDeep(leafUri));
            }

            var nextFileHandle = this.fileExplorer.GetFile(nextUri);

            if (!nextFileHandle.Exists())
            {
                // Track the missing file too, so that creating it invalidates the cached failure.
                chainFileUris.Add(nextUri);

                return new(ConfigDiagnosticBuilder.UnloadableBicepConfigFile(nextUri, "File not found."));
            }

            currentFileHandle = nextFileHandle;
        }

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
        catch (Exception exception) when (exception is ConfigurationException or JsonException or InvalidOperationException)
        {
            return GetBuiltInChain(diagnostics: [ConfigDiagnosticBuilder.InvalidBicepConfigFile(leafUri, exception.Message)]);
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

        // One entry per file in the chain, so the language server can publish (and clear) diagnostics for each file.
        // Each entry only identifies its file; the file's own settings are part of the effective configuration.
        var layers = rawLayers
            .Select(layer => BicepConfiguration.BuiltIn.With(configFileIdentifier: layer.FileHandle.Uri))
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
            if (TryGetSection(element, [BicepConfiguration.CompilerKey, "version"], out var versionElement) &&
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
