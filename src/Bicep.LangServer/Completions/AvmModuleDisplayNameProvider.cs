// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using Bicep.Core.Features;
using Microsoft.VisualBasic.FileIO;

namespace Bicep.LanguageServer.Completions
{
    public interface IAvmModuleDisplayNameProvider
    {
        void StartCache();

        bool TryGetModuleDisplayName(string modulePath, [NotNullWhen(true)] out string? displayName);

        bool TryGetModuleStatus(string modulePath, [NotNullWhen(true)] out string? moduleStatus);
    }

    public class AvmModuleDisplayNameProvider : IAvmModuleDisplayNameProvider
    {
        private static readonly Uri UtilityModulesCsvUri = new("https://azure.github.io/Azure-Verified-Modules/module-indexes/BicepUtilityModules.csv");
        private static readonly Uri PatternModulesCsvUri = new("https://azure.github.io/Azure-Verified-Modules/module-indexes/BicepPatternModules.csv");
        private static readonly Uri ResourceModulesCsvUri = new("https://azure.github.io/Azure-Verified-Modules/module-indexes/BicepResourceModules.csv");

        private static readonly AvmModuleInfoLookup EmptyLookup = new([], [], []);

        private readonly IAvmModuleCsvIndexHttpClient client;
        private readonly object startLock = new();

        private volatile AvmModuleInfoLookup lookup = EmptyLookup;
        private Task? loadTask;

        public AvmModuleDisplayNameProvider(IAvmModuleCsvIndexHttpClient client)
        {
            this.client = client;
        }

        public void StartCache()
        {
            lock (startLock)
            {
                loadTask ??= Task.Run(LoadAsync);
            }
        }

        public bool TryGetModuleDisplayName(string modulePath, [NotNullWhen(true)] out string? displayName)
        {
            displayName = null;

            if (TryGetModuleInfo(modulePath) is { } info)
            {
                displayName = info.DisplayName;
                return true;
            }

            return false;
        }

        public bool TryGetModuleStatus(string modulePath, [NotNullWhen(true)] out string? moduleStatus)
        {
            moduleStatus = null;

            if (TryGetModuleInfo(modulePath) is { } info && info.ModuleStatus is not null)
            {
                moduleStatus = info.ModuleStatus;
                return true;
            }

            return false;
        }

        private AvmModuleInfo? TryGetModuleInfo(string modulePath)
        {
            if (TryGetNormalizedModuleName(modulePath) is not { } normalizedModuleName)
            {
                return null;
            }

            var currentLookup = lookup;
            if (normalizedModuleName.StartsWith("avm/utl/", StringComparison.Ordinal))
            {
                return currentLookup.Utility.TryGetValue(normalizedModuleName, out var info) ? info : null;
            }

            if (normalizedModuleName.StartsWith("avm/ptn/", StringComparison.Ordinal))
            {
                return currentLookup.Pattern.TryGetValue(normalizedModuleName, out var info) ? info : null;
            }

            if (normalizedModuleName.StartsWith("avm/res/", StringComparison.Ordinal))
            {
                return currentLookup.Resource.TryGetValue(normalizedModuleName, out var info) ? info : null;
            }

            return null;
        }

        private async Task LoadAsync()
        {
            try
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                var utilityTask = LoadCsvAsync(UtilityModulesCsvUri, timeoutCts.Token);
                var patternTask = LoadCsvAsync(PatternModulesCsvUri, timeoutCts.Token);
                var resourceTask = LoadCsvAsync(ResourceModulesCsvUri, timeoutCts.Token);

                await Task.WhenAll(utilityTask, patternTask, resourceTask);

                var utility = await utilityTask;
                var pattern = await patternTask;
                var resource = await resourceTask;

                lookup = new AvmModuleInfoLookup(
                    Utility: utility,
                    Pattern: pattern,
                    Resource: resource);

                Trace.WriteLineIf(
                    FeatureProvider.TracingEnabled,
                    string.Format(
                        "{0}: AVM display-name cache loaded (utility={1}, pattern={2}, resource={3}).",
                        nameof(AvmModuleDisplayNameProvider),
                        utility.Count,
                        pattern.Count,
                        resource.Count));
            }
            catch (Exception exception)
            {
                lookup = EmptyLookup;

                Trace.WriteLineIf(
                    FeatureProvider.TracingEnabled,
                    string.Format(
                        "{0}: Failed to preload AVM display-name cache. Falling back to empty cache. Error: {1}",
                        nameof(AvmModuleDisplayNameProvider),
                        exception.Message));
            }
        }

        private async Task<ImmutableDictionary<string, AvmModuleInfo>> LoadCsvAsync(Uri csvUri, CancellationToken cancellationToken)
        {
            var csvContent = await client.GetCsvAsync(csvUri, cancellationToken);
            using var reader = new StringReader(csvContent);
            using var parser = new TextFieldParser(reader)
            {
                TextFieldType = FieldType.Delimited,
                HasFieldsEnclosedInQuotes = true,
                TrimWhiteSpace = false,
            };

            parser.SetDelimiters(",");

            if (parser.EndOfData)
            {
                return [];
            }

            var headers = parser.ReadFields();
            if (headers is null)
            {
                return [];
            }

            var moduleNameIndex = Array.FindIndex(headers, x => string.Equals(x, "ModuleName", StringComparison.Ordinal));
            var moduleDisplayNameIndex = Array.FindIndex(headers, x => string.Equals(x, "ModuleDisplayName", StringComparison.Ordinal));
            var moduleStatusIndex = Array.FindIndex(headers, x => string.Equals(x, "ModuleStatus", StringComparison.Ordinal));
            if (moduleNameIndex == -1 || moduleDisplayNameIndex == -1)
            {
                return [];
            }

            var entries = new Dictionary<string, AvmModuleInfo>(StringComparer.Ordinal);
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields is null ||
                    moduleNameIndex >= fields.Length ||
                    moduleDisplayNameIndex >= fields.Length)
                {
                    continue;
                }

                if (TryGetNormalizedModuleName(fields[moduleNameIndex]) is not { } normalizedModuleName)
                {
                    continue;
                }

                var moduleDisplayName = fields[moduleDisplayNameIndex]?.Trim();
                if (string.IsNullOrWhiteSpace(moduleDisplayName))
                {
                    continue;
                }

                string? moduleStatus = null;
                if (moduleStatusIndex != -1 && moduleStatusIndex < fields.Length)
                {
                    var statusValue = fields[moduleStatusIndex]?.Trim();
                    if (!string.IsNullOrWhiteSpace(statusValue))
                    {
                        moduleStatus = statusValue;
                    }
                }

                entries.TryAdd(normalizedModuleName, new AvmModuleInfo(moduleDisplayName, moduleStatus));
            }

            return entries.ToImmutableDictionary(StringComparer.Ordinal);
        }

        private static string? TryGetNormalizedModuleName(string? modulePath)
        {
            if (string.IsNullOrWhiteSpace(modulePath))
            {
                return null;
            }

            var normalized = modulePath.Trim().Replace('\\', '/');
            if (normalized.StartsWith("bicep/", StringComparison.Ordinal))
            {
                normalized = normalized["bicep/".Length..];
            }

            return normalized;
        }

        private sealed record AvmModuleInfo(string DisplayName, string? ModuleStatus);

        private sealed record AvmModuleInfoLookup(
            ImmutableDictionary<string, AvmModuleInfo> Utility,
            ImmutableDictionary<string, AvmModuleInfo> Pattern,
            ImmutableDictionary<string, AvmModuleInfo> Resource);
    }

    public interface IAvmModuleCsvIndexHttpClient
    {
        Task<string> GetCsvAsync(Uri csvUri, CancellationToken cancellationToken);
    }

    public class AvmModuleCsvIndexHttpClient : IAvmModuleCsvIndexHttpClient
    {
        private readonly HttpClient client;

        public AvmModuleCsvIndexHttpClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<string> GetCsvAsync(Uri csvUri, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, csvUri);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }

    public class NullAvmModuleDisplayNameProvider : IAvmModuleDisplayNameProvider
    {
        public static readonly NullAvmModuleDisplayNameProvider Instance = new();

        public void StartCache()
        {
        }

        public bool TryGetModuleDisplayName(string modulePath, [NotNullWhen(true)] out string? displayName)
        {
            displayName = null;
            return false;
        }

        public bool TryGetModuleStatus(string modulePath, [NotNullWhen(true)] out string? moduleStatus)
        {
            moduleStatus = null;
            return false;
        }
    }
}
