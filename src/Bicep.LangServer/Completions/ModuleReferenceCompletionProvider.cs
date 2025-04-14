// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Provides completions for OCI (public or private) module references, e.g. br/public:modulePath:version
    /// </summary>
    public partial class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IAzureContainerRegistriesProvider azureContainerRegistriesProvider;
        private readonly IRegistryModuleCatalog registryModuleCatalog;
        private readonly ISettingsProvider settingsProvider;
        private readonly ITelemetryProvider telemetryProvider;

        private enum ModuleCompletionPriority
        {
            Alias = 0,     // br/[alias], ts/[alias]
            Default = 1,
            FullPath = 2,  // br:, ts:
        }

        [GeneratedRegex(
            """
                (?x) # Extended mode (allow comments and whitespace)
                ^
                ( # Prefix and registry or alias

                    br/(?<alias>[a-zA-Z0-9-_]*):   # see src\Bicep.Core\Configuration\ModuleAliasesConfiguration.cs::ModuleAliasNameRegex
                    |
                    br:(?<registry>[-0-9A-Za-z|.^]*)\/
                )

                # Path
                (
                    (?<path>[a-z0-9._\-/]*) # see src\Bicep.Core\Configuration\ModuleAliasesConfiguration.cs::OciNamespaceSegmentRegex
                )?

                # Version
                (
                    (?<versionSeparator>:)
                    (?<version>[a-zA-Z0-9_][a-zA-Z0-9._-]{0,127})?   # see src\Bicep.Core\Registry\Oci\OciArtifactReferenceFacts.cs::TagNameRegex
                )?

                '?
                $
                """,
            RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant,
            matchTimeoutMilliseconds: 10
        )]
        private static partial Regex PartialModuleReferenceRegex();

        // Examples:
        //   br:contoso.io/path1/path2/module:2.0.1 =>
        //     SpecifiedRegistry: "contoso.io"
        //     ResolvedRegistry: "contoso.io"
        //     SpecifiedModulePath: "path1/path2/module"
        //     ModulePathPrefix: "contosoBasePath/"   (from bicepconfig.json, may be empty string)
        //     ResolvedModulePath: "contosoBasePath/path1/path2/module"
        //     Version: "2.0.1"
        //     ToNotation: "br:contoso.io/path1/path2/module:2.0.1"
        //   br/public:path1/path2/module:2.0.1 =>
        //     SpecifiedRegistry: null
        //     ResolvedRegistry: "mcr.microsoft.com"
        //     ModulePathPrefix: "bicep/"    (from bicepconfig.json default values)
        //     SpecifiedModulePath: "path1/path2/module"
        //     ResolvedModulePath: "bicep/path1/path2/module"
        //     Version: "2.0.1"
        //     ToNotation: "br/public:path1/path2/module:2.0.1"
        private record Parts(
            string? SpecifiedRegistry,
            string ResolvedRegistry,
            string? SpecifiedAlias,
            string? ModulePathPrefix,
            string? SpecifiedModulePath,
            string? Version,
            bool HasVersionSeparator
        )
        {
            public string ToNotation() =>
                SpecifiedAlias is string
                    ? $"br/{SpecifiedAlias}:{SpecifiedModulePath}{VersionTextWithSeparator}"
                    : $"br:{SpecifiedRegistry}/{SpecifiedModulePath}{VersionTextWithSeparator}";

            private string VersionTextWithSeparator => HasVersionSeparator ? $":{Version}" : string.Empty;

            public string ModulePathPrefixWithSeparator => string.IsNullOrWhiteSpace(ModulePathPrefix) ? "" : $"{ModulePathPrefix}/";

            public string ResolvedModulePath => $"{ModulePathPrefixWithSeparator}{SpecifiedModulePath}";

            public Parts WithModulePath(string newResolvedModulePath)
            {
                if (!string.IsNullOrWhiteSpace(ModulePathPrefix) && newResolvedModulePath.StartsWith(ModulePathPrefixWithSeparator, StringComparison.Ordinal))
                {
                    return this with
                    {
                        SpecifiedModulePath = newResolvedModulePath.Substring(ModulePathPrefixWithSeparator.Length)
                    };
                }
                else
                {
                    return this with
                    {
                        SpecifiedModulePath = newResolvedModulePath
                    };
                }
            }

            public bool IsPublicRegistry => string.CompareOrdinal(ResolvedRegistry, PublicMcrRegistry) == 0;
        }

        private const string PublicMcrRegistry = LanguageConstants.BicepPublicMcrRegistry; // "mcr.microsoft.com"

        private const string ModuleVersionResolutionKey = "ociVersion";
        private const string ModuleResolutionKey = "oci";

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistriesProvider azureContainerRegistriesProvider,
            IRegistryModuleCatalog registryModuleCatalog,
            ISettingsProvider settingsProvider,
            ITelemetryProvider telemetryProvider)
        {
            this.azureContainerRegistriesProvider = azureContainerRegistriesProvider;
            this.registryModuleCatalog = registryModuleCatalog;
            this.settingsProvider = settingsProvider;
            this.telemetryProvider = telemetryProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(BicepSourceFile sourceFile, BicepCompletionContext context, CancellationToken cancellationToken)
        {
            var replacementText = string.Empty;

            if (context.ReplacementTarget is Token token)
            {
                replacementText = token.Text;
            }

            var rootConfiguration = sourceFile.Configuration;
            var completions = GetTopLevelCompletions(context, replacementText, rootConfiguration);

            var startsWithSingleQuote = replacementText.StartsWith('\'');
            if (startsWithSingleQuote)
            {
                var trimmedReplacementText = replacementText.Trim('\'');

                var replacementsRequiringStartingQuote =
                    (await GetModuleCompletions(trimmedReplacementText, context, rootConfiguration))
                    .Concat(GetPartialPrivatePathCompletionsFromAliases(trimmedReplacementText, context, rootConfiguration))
                    .Concat(await GetVersionCompletions(context, trimmedReplacementText, rootConfiguration))
                    .Concat(await GetAllRegistryNameAndAliasCompletions(context, trimmedReplacementText, rootConfiguration, cancellationToken));

                completions = [
                    .. completions,
                    .. replacementsRequiringStartingQuote,
                ];
            }

            return completions;
        }

        private Parts? ParseParts(string text, RootConfiguration rootConfiguration)
        {
            var match = PartialModuleReferenceRegex().Match(text);
            if (!match.Success)
            {
                return null;
            }

            var path = NullIfEmpty(match.Groups["path"].Value);
            var version = NullIfEmpty(match.Groups["version"].Value);
            var hasVersionSeparator = match.Groups["versionSeparator"].Success;

            if (NullIfEmpty(match.Groups["registry"].Value) is string registry)
            {
                // Reference with fully-specified registry
                return new Parts(
                    SpecifiedRegistry: registry,
                    ResolvedRegistry: registry,
                    SpecifiedAlias: null,
                    SpecifiedModulePath: path,
                    ModulePathPrefix: null,
                    Version: version,
                    HasVersionSeparator: hasVersionSeparator
                );
            }
            else if (NullIfEmpty(match.Groups["alias"].Value) is string alias)
            {
                // Reference with alias
                if (TryGetValidModuleAlias(rootConfiguration, alias, out var aliasRegistry, out var aliasModulePath))
                {
                    return new Parts(
                        SpecifiedRegistry: null,
                        ResolvedRegistry: aliasRegistry,
                        SpecifiedAlias: alias,
                        ModulePathPrefix: aliasModulePath,
                        SpecifiedModulePath: path,
                        Version: version,
                        HasVersionSeparator: hasVersionSeparator
                    );
                }
            }

            return null;

            string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
        }

        // Handles bicep registry and template spec top-level schema completions.
        // I.e. typing with an empty path:  module m1 <CURSOR>
        private IEnumerable<CompletionItem> GetTopLevelCompletions(BicepCompletionContext context, string untrimmedReplacementText, RootConfiguration rootConfiguration)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath) &&
                !context.Kind.HasFlag(BicepCompletionContextKind.UsingFilePath))
            {
                return [];
            }

            if (!string.IsNullOrWhiteSpace(untrimmedReplacementText.Trim('\'')))
            {
                return [];
            }

            List<CompletionItem> completionItems = new();

            var templateSpecModuleAliases = rootConfiguration.ModuleAliases.GetTemplateSpecModuleAliases();
            var bicepModuleAliases = GetModuleAliases(rootConfiguration);

            // Top-level TemplateSpec completions
            AddCompletionItem("ts:", null, "Template spec", ModuleCompletionPriority.FullPath, "template spec completion");
            if (templateSpecModuleAliases.Any())
            {
                // ts/<alias>
                foreach (var kvp in templateSpecModuleAliases)
                {
                    AddCompletionItem("ts/", kvp.Key, "Template spec", ModuleCompletionPriority.Alias, "template spec alias completion");
                }
            }
            else
            {
                // "ts/"
                AddCompletionItem("ts/", null, "Template spec (alias)", ModuleCompletionPriority.Alias, "template spec alias completion");
            }

            // Top-level Bicep registry completions
            AddCompletionItem(OciArtifactReferenceFacts.SchemeWithColon, null, "Bicep registry", ModuleCompletionPriority.FullPath, "module registry completion");
            if (!bicepModuleAliases.IsEmpty)
            {
                // br/<alias>
                foreach (var kvp in bicepModuleAliases)
                {
                    var alias = kvp.Key;
                    var registry = kvp.Value.Registry as string;
                    var modulePath = kvp.Value.ModulePath as string;
                    var detail = (string.CompareOrdinal(registry, PublicMcrRegistry) == 0 && string.CompareOrdinal(modulePath, "bicep") == 0)
                        ? "Public Bicep registry"
                        : $"Alias for br:{registry}/{(modulePath == null ? "" : (modulePath + "/"))}";

                    AddCompletionItem("br/", alias, detail, ModuleCompletionPriority.Alias, "module registry completion");
                }
            }
            else
            {
                // "br/"
                AddCompletionItem("br/", null, "Bicep registry (alias)", ModuleCompletionPriority.Alias, "module registry alias completion");
            }

            return completionItems;

            void AddCompletionItem(string schemePrefix, string? alias, string detailLabel, ModuleCompletionPriority priority, string triggerNextCompletionTitle)
            {
                var label = alias is null
                    ? $"{schemePrefix}"          // Full path (ts:, br:)
                    : $"{schemePrefix}{alias}:"; // Alias (ts/, br/)
                var insertText = $"'{label}$0'";
                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, label)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(label, priority))
                    .WithFilterText(insertText)
                    .WithDetail(detailLabel)
                    .WithFollowupCompletion(triggerNextCompletionTitle)
                    .Build();

                completionItems.Add(completionItem);
            }
        }

        /// <summary>
        /// True if is an OCI module reference (i.e., it starts with br: or br/)
        /// </summary>
        private bool IsOciArtifactRegistryReference(string trimmedText)
        {
            if (trimmedText.StartsWith("br/") || trimmedText.StartsWith("br:"))
            {
                return true;
            }

            return false;
        }

        private async Task<IEnumerable<CompletionItem>> GetVersionCompletions(BicepCompletionContext context, string trimmedText, RootConfiguration rootConfiguration)
        {
            if (ParseParts(trimmedText, rootConfiguration) is not Parts parts
                || parts.ResolvedModulePath is null
                || !parts.HasVersionSeparator
                || !string.IsNullOrWhiteSpace(parts.Version)
                )
            {
                return [];
            }

            List<CompletionItem> completions = new();

            if (await registryModuleCatalog.GetProviderForRegistry(rootConfiguration.Cloud, parts.ResolvedRegistry)
                .TryGetModuleAsync($"{parts.ResolvedModulePath}") is { } module)
            {
                var versions = (await module.TryGetVersionsAsync())
                    .Where(v => v.IsBicepModule != false)
                    .ToArray();

                for (int i = 0; i < versions.Length; ++i)
                {
                    var version = versions[versions.Length - 1 - i].Version; // Show versions from most recent to oldest
                    var insertText = $"'{trimmedText}{version}'$0";

                    // Module version is last completion, no follow-up completions triggered
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, version)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithFilterText(insertText)
                        .WithSortText(GetSortText(i))
                        // Description and documentation will be resolved later as needed
                        .WithResolveData(ModuleVersionResolutionKey, new { Registry = parts.ResolvedRegistry, Module = parts.ResolvedModulePath, Version = version })
                        .Build();

                    completions.Add(completionItem);
                }
            }

            return completions;
        }

        private static ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetModuleAliases(RootConfiguration configuration)
        {
            return configuration.ModuleAliases.GetOciArtifactModuleAliases();
        }

        private static bool TryGetValidModuleAlias(
            RootConfiguration configuration,
            string aliasName,
            [NotNullWhen(true)] out string? registry,
            out string? modulePath)
        {
            registry = null;
            modulePath = null;

            if (configuration.ModuleAliases.GetOciArtifactModuleAliases().TryGetValue(aliasName, out var aliasConfig)
                && !string.IsNullOrWhiteSpace(aliasConfig.Registry))
            {
                registry = aliasConfig.Registry;
                modulePath = aliasConfig.ModulePath;
                return true;
            }

            return false;
        }

        // Automatically adds completions for the base path portion of a private registry from an alias in bicepconfig.json.
        // Example:
        //   bicepconfig.json:
        //   {
        //     "moduleAliases": {
        //       "br": {
        //         "whatever": {
        //           "registry": "privateacr.azurecr.io",
        //           "modulePath": "bicep/app"    => we'll automatically complete this part
        //    ...
        //
        //   br:privateacr.azurecr.io/<CURSOR>
        //      =>
        //   br:privateacr.azurecr.io/bicep/app:<CURSOR>
        private IEnumerable<CompletionItem> GetPartialPrivatePathCompletionsFromAliases(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration)
        {
            if (ParseParts(trimmedText, rootConfiguration) is not Parts parts
                || !string.IsNullOrWhiteSpace(parts.ResolvedModulePath)
                || parts.HasVersionSeparator
                || !string.IsNullOrEmpty(parts.SpecifiedAlias)
                || parts.IsPublicRegistry
                || string.IsNullOrWhiteSpace(parts.ResolvedRegistry))
            {
                return [];
            }

            List<CompletionItem> completions = new();

            var sentTelemetry = false;
            foreach (var kvp in GetModuleAliases(rootConfiguration))
            {
                if (parts.ResolvedRegistry.Equals(kvp.Value.Registry, StringComparison.Ordinal))
                {
                    var modulePath = kvp.Value.ModulePath;

                    if (modulePath is null) // No module path to complete
                    {
                        continue;
                    }

                    var insertText = $"'{trimmedText}{modulePath}:$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, modulePath)
                       .WithSnippetEdit(context.ReplacementRange, insertText)
                       .WithFilterText(insertText)
                       .WithSortText(GetSortText(modulePath))
                       .WithResolveData(ModuleResolutionKey, new { Registry = parts.ResolvedRegistry, Module = modulePath })
                       .WithFollowupCompletion("module path completion")
                       .Build();
                    completions.Add(completionItem);

                    if (!sentTelemetry)
                    {
                        telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.AcrBasePathFromAlias, true, null));
                        sentTelemetry = true;
                    }
                }
            }

            return completions;
        }

        // Handles module path completions for public or private OCI modules
        //   br/alias:<CURSOR>
        // or
        //   br:registry.contoso.io/bicep/:<CURSOR>
        private async Task<IEnumerable<CompletionItem>> GetModuleCompletions(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration)
        {
            if (ParseParts(trimmedText, rootConfiguration) is not Parts parts
                || parts.ResolvedModulePath is null
                || parts.HasVersionSeparator)
            {
                return [];
            }

            List<CompletionItem> completions = new();
            var acr = false;
            var mcr = false;
            var isAliased = !string.IsNullOrWhiteSpace(parts.SpecifiedAlias);

            var modules = await registryModuleCatalog
                .GetProviderForRegistry(rootConfiguration.Cloud, parts.ResolvedRegistry)
                .TryGetModulesAsync();
            foreach (var module in modules)
            {
                var moduleName = module.ModuleName;

                if (!moduleName.StartsWith(parts.ResolvedModulePath, StringComparison.Ordinal))
                {
                    continue;
                }

                string insertText = $"'{parts.WithModulePath(moduleName).ToNotation()}:$0'";

                // Remove the base path prefix from the label if we're dealing with a module alias
                var label = !string.IsNullOrWhiteSpace(parts.SpecifiedAlias) && !string.IsNullOrWhiteSpace(parts.ModulePathPrefix)
                    ? moduleName.Substring(parts.ModulePathPrefixWithSeparator.Length)
                    : moduleName;

                var completionItem = CompletionItemBuilder.Create(
                    CompletionItemKind.Snippet, label)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithFilterText(insertText)
                        .WithSortText(GetSortText(moduleName))
                        .WithResolveData(
                            ModuleResolutionKey,
                            new { Registry = module.Registry, Module = moduleName })
                        .WithFollowupCompletion("module version completion")
                        .Build();

                completions.Add(completionItem);

                if (parts.ResolvedRegistry.Equals(PublicMcrRegistry, StringComparison.Ordinal))
                {
                    mcr = true;
                }
                else
                {
                    acr = true;
                }
            }

            if (acr)
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR, isAliased, modules.Length));
            }
            if (mcr)
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.MCR, isAliased, modules.Length));
            }

            return completions;
        }

        // Handles top-level completions of registry names/aliases after br: and br/
        private async Task<IEnumerable<CompletionItem>> GetAllRegistryNameAndAliasCompletions(BicepCompletionContext context, string trimmedText, RootConfiguration rootConfiguration, CancellationToken cancellationToken)
        {
            var completions = new List<CompletionItem>();

            if (!IsOciArtifactRegistryReference(trimmedText))
            {
                return completions;
            }

            if (trimmedText == "br/")
            {
                foreach (var kvp in GetModuleAliases(rootConfiguration))
                {
                    var alias = kvp.Key;
                    var insertText = $"'{trimmedText}{alias}:$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, alias)
                        .WithFilterText(insertText)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithSortText(GetSortText(alias))
                        .WithFollowupCompletion("module repository completion")
                        .Build();
                    completions.Add(completionItem);
                }
            }
            else if (trimmedText == "br:")
            {
                var label = $"{PublicMcrRegistry}/bicep";
                var insertText = $"'{trimmedText}{label}/$0'";
                var mcrCompletionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                    .WithFilterText(insertText)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(label))
                    .WithFollowupCompletion("module repository completion")
                    .Build();

                completions.Add(mcrCompletionItem);

                completions.AddRange(await GetPrivateRegistryNameCompletions(trimmedText, context, rootConfiguration, cancellationToken));
            }

            return completions;
        }

        // Handles registry name completions for private modules possibly available in ACR registries
        private async Task<IEnumerable<CompletionItem>> GetPrivateRegistryNameCompletions(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration, CancellationToken cancellationToken)
        {
            if (settingsProvider.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting))
            {
                return await GetACRRegistryNameCompletionsFromAzure(trimmedText, context, rootConfiguration, cancellationToken);
            }
            else
            {
                // CONSIDER: Somehow indicate in the completion list that users can get more completions by setting GetAllAzureContainerRegistriesForCompletionsSetting
                return GetPartialACRModuleRegistriesCompletionsFromBicepConfig(trimmedText, context, rootConfiguration);
            }
        }

        // Handles private registry name completions for modules available in ACR registries using ResourceGraphClient query.
        // This returns all registries that the user has access to via Azure (whether or not they contain bicep modules, and whether
        //   or not they're registered in the bicepconfig.json file)
        // This is for completions after typing "br:"
        private async Task<IEnumerable<CompletionItem>> GetACRRegistryNameCompletionsFromAzure(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration, CancellationToken cancellationToken)
        {
            List<CompletionItem> completions = new();

            try
            {
                await foreach (string registryName in azureContainerRegistriesProvider.GetContainerRegistriesAccessibleFromAzure(rootConfiguration.Cloud, cancellationToken)
                    .WithCancellation(cancellationToken))
                {
                    var insertText = $"'{trimmedText}{registryName}/$0'";

                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, registryName)
                        .WithFilterText(insertText)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithSortText(GetSortText(registryName, ModuleCompletionPriority.FullPath))
                        .WithFollowupCompletion("module path completion")
                        .Build();
                    completions.Add(completionItem);
                }

                return completions;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task<CompletionItem> ResolveCompletionItem(CompletionItem completionItem, CancellationToken _)
        {
            if (completionItem.Data != null && completionItem.Data[ModuleVersionResolutionKey] is JObject versionData)
            {
                var registry = versionData["Registry"]?.ToString();
                var modulePath = versionData["Module"]?.ToString();
                var version = versionData["Version"]?.ToString();

                if (registry != null && modulePath != null && version != null)
                {
                    return await ResolveVersionCompletionItem(completionItem, registry, modulePath, version);
                }
            }
            else if (completionItem.Data != null && completionItem.Data[ModuleResolutionKey] is JObject moduleVersion)
            {
                var registry = moduleVersion["Registry"]?.ToString();
                var modulePath = moduleVersion["Module"]?.ToString();

                if (registry != null && modulePath != null)
                {
                    return await ResolveModuleCompletionItem(completionItem, registry, modulePath);
                }
            }

            return completionItem;
        }

        private async Task<CompletionItem> ResolveVersionCompletionItem(CompletionItem completionItem, string registry, string modulePath, string version)
        {
            if (registryModuleCatalog.TryGetCachedRegistry(registry) is IRegistryModuleMetadataProvider cachedRegistry
                && await cachedRegistry.TryGetModuleAsync(modulePath) is { } module
                && await module.TryGetVersionsAsync() is { } versions
                && versions.FirstOrDefault(v => v.Version.Equals(version, StringComparison.Ordinal)) is RegistryModuleVersionMetadata metadata)
            {
                // Resolutions for MCR aren't very interesting because all the data is downloaded at once
                if (!registry.Equals(LanguageConstants.BicepPublicMcrRegistry, StringComparison.Ordinal))
                {
                    telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryResolution(ModuleRegistryResolutionType.AcrVersion));
                }

                return (completionItem with
                {
                    Detail = metadata.Details.Description,
                })
                .WithDocumentation(MarkdownHelper.GetDocumentationLink(metadata.Details.DocumentationUri));
            }

            return completionItem;
        }

        private async Task<CompletionItem> ResolveModuleCompletionItem(CompletionItem completionItem, string registry, string modulePath)
        {
            if (registryModuleCatalog.TryGetCachedRegistry(registry) is IRegistryModuleMetadataProvider cachedRegistry
                && await cachedRegistry.TryGetModuleAsync(modulePath) is { } module
                && await module.TryGetDetailsAsync() is { } details
                )
            {
                // Resolutions for MCR aren't very interesting because all the data is downloaded at once
                if (!registry.Equals(LanguageConstants.BicepPublicMcrRegistry, StringComparison.Ordinal))
                {
                    telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryResolution(ModuleRegistryResolutionType.AcrModulePath));
                }

                return (completionItem with
                {
                    Detail = details.Description,
                }).WithDocumentation(MarkdownHelper.GetDocumentationLink(details.DocumentationUri));
            }

            return completionItem;
        }

        // Handles private ACR registry name completions only for registries that are configured via aliases in the bicepconfig.json file
        private IEnumerable<CompletionItem> GetPartialACRModuleRegistriesCompletionsFromBicepConfig(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration)
        {
            List<CompletionItem> completions = new();
            HashSet<string> aliases = new();

            foreach (var kvp in GetModuleAliases(rootConfiguration))
            {
                var label = kvp.Value.Registry;

                if (label is not null && !label.Equals(PublicMcrRegistry, StringComparison.Ordinal))
                {
                    if (!aliases.TryGetValue(label, out _))
                    {
                        var insertText = $"'{trimmedText}{label}/$0'";
                        var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                            .WithFilterText(insertText)
                            .WithSnippetEdit(context.ReplacementRange, insertText)
                            .WithSortText(GetSortText(label))
                            .WithFollowupCompletion("module path completion")
                            .Build();
                        completions.Add(completionItem);

                        aliases.Add(label);
                    }
                }
            }

            return completions;
        }

        private static string GetSortText(int priority) => $"{priority}".PadLeft(4, '0');

        private static string GetSortText(string label, ModuleCompletionPriority priority = ModuleCompletionPriority.Default)
        {
            // We want all module completion priorities to come after other completions (e.g. local module paths), so we start with "9"
            return $"9{(int)priority}_{label}";
        }
    }
}
