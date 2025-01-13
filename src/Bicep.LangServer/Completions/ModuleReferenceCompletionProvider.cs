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
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Indexing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Microsoft.Win32;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Provides completions for OCI (public or private) module references, e.g. br/public:modulePath:version
    /// </summary>
    public partial class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IAzureContainerRegistriesProvider azureContainerRegistriesProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly IRegistryIndexer registryModuleIndexer;
        private readonly ISettingsProvider settingsProvider;
        private readonly ITelemetryProvider telemetryProvider;

        private enum ModuleCompletionPriority
        {
            Alias = 0,     // br/[alias], ts/[alias]
            Default = 1,
            FullPath = 2,  // br:, ts:
        }

        // Direct reference to a full registry login server URI via br:<registry>
        private static readonly Regex ModulePrefixWithFullPath = new(@"^br:(?<registry>(.*?))/", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Examples: asdfg move?
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

            public Parts WithModulePath(string newResolvedModulePath) //asdfg test?
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
        }

        //asdfg test?
        [GeneratedRegex(
            """
                (?x) # Extended mode (allow comments and whitespace)
                ^
                ( # Prefix and registry or alias

                    br/(?<alias>[a-zA-Z0-9-_]*):   # see src\Bicep.Core\Configuration\ModuleAliasesConfiguration.cs::ModuleAliasNameRegex
                    |
                    br:(?<registry>[^/:']*)\/   #asdfg valid?
                )

                # Path
                (
                    (?<path>[^:']+) #asdfg valid?
                )?

                # Version
                (
                    (?<versionSeparator>:)
                    (?<version>[^:']+)?   #asdfg valid?
                )?

                '?
                """,
            RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant,
            matchTimeoutMilliseconds: 10
        )]
        private static partial Regex PartialModuleReferenceRegex();

        private const string PublicMcrRegistry = LanguageConstants.BicepPublicMcrRegistry; // "mcr.microsoft.com"
        private const string ModuleVersionsResolutionKey = "moduleVer";

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistriesProvider azureContainerRegistriesProvider,
            IConfigurationManager configurationManager,
            IRegistryIndexer registryModuleIndexer,
            ISettingsProvider settingsProvider,
            ITelemetryProvider telemetryProvider)
        {
            this.azureContainerRegistriesProvider = azureContainerRegistriesProvider;
            this.configurationManager = configurationManager;
            this.registryModuleIndexer = registryModuleIndexer;
            this.settingsProvider = settingsProvider;
            this.telemetryProvider = telemetryProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri sourceFileUri, BicepCompletionContext context, CancellationToken cancellationToken)
        {
            var replacementText = string.Empty;

            if (context.ReplacementTarget is Token token)
            {
                replacementText = token.Text;
            }

            var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
            var completions = GetTopLevelCompletions(context, replacementText, rootConfiguration);

            var startsWithSingleQuote = replacementText.StartsWith('\'');
            if (startsWithSingleQuote)
            {
                var trimmedReplacementText = replacementText.Trim('\'');

                var replacementsRequiringStartingQuote =
                    (await GetOciModuleCompletions(context, trimmedReplacementText, rootConfiguration))
                    .Concat(await GetVersionCompletions(context, trimmedReplacementText, rootConfiguration))
                    .Concat(await GetAllRegistryNameAndAliasCompletions(context, trimmedReplacementText, rootConfiguration, cancellationToken));

                completions = [
                    ..completions,
                    ..replacementsRequiringStartingQuote,
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
                var aliases = GetModuleAliases(rootConfiguration);
                if (aliases.TryGetValue(alias, out OciArtifactModuleAlias? aliasConfig) && !string.IsNullOrWhiteSpace(aliasConfig.Registry)) //asdfg extract
                {
                    return new Parts(
                        SpecifiedRegistry: null,
                        ResolvedRegistry: aliasConfig.Registry,
                        SpecifiedAlias: alias,
                        ModulePathPrefix: aliasConfig.ModulePath,
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
                !context.Kind.HasFlag(BicepCompletionContextKind.UsingFilePath)) //asdfg?
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

            var versionsMetadata =
                await registryModuleIndexer.GetRegistry(parts.ResolvedRegistry, rootConfiguration.Cloud)
                .TryGetModuleVersionsAsync($"{parts.ResolvedModulePath}");

            for (int i = versionsMetadata.Length - 1; i >= 0; i--)
            {
                var (version, description, documentationUri) = versionsMetadata[i];

                var insertText = $"'{trimmedText}{version}'$0";

                // Module version is last completion, no follow-up completions triggered
                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, version)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(i))
                    .WithDetail(description) //asdfg
                    .WithDocumentation(MarkdownHelper.GetDocumentationLink(documentationUri))//asdfg
                    .WithResolve(ModuleVersionsResolutionKey, new { Registry = parts.ResolvedRegistry, Module = parts.ResolvedModulePath, Version = version })
                    .Build();

                completions.Add(completionItem);
            }

            return completions;

            //asdfg test
            //// Handles scenario where the user has configured an alias for MCR in bicepconfig.json.
            //string? GetAliasedMCRModulePath(string trimmedText, Uri sourceFileUri)
            //{
            //    foreach (var kvp in GetModuleAliases(sourceFileUri))
            //    {
            //        if (kvp.Value.Registry is string registry &&
            //            registry.Equals(PublicMcrRegistry, StringComparison.Ordinal))
            //        {
            //            var aliasFromBicepConfig = $"br/{kvp.Key}:";

            //            if (trimmedText.StartsWith(aliasFromBicepConfig, StringComparison.Ordinal))
            //            {
            //                var matches = ModuleWithAliasAndVersionSeparator.Matches(trimmedText);
            //                if (!matches.Any())
            //                {
            //                    continue;
            //                }

            //                string subpath = matches[0].Groups["path"].Value;
            //                if (subpath is null)
            //                {
            //                    continue;
            //                }

            //                var modulePath = kvp.Value.ModulePath;

            //                if (modulePath is not null)
            //                {
            //                    if (modulePath.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix)) //asdfg
            //                    {
            //                        modulePath = modulePath.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length);
            //                        return $"{modulePath}/{subpath}";
            //                    }
            //                }
            //                else
            //                {
            //                    if (subpath.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix))
            //                    {
            //                        return subpath.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length); //asdfg
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    return null;
            //}
        }

        private async Task<CompletionItem> ResolveVersion(CompletionItem completionItem, string registry, string modulePath, string version, CancellationToken _)
        {
            if (registryModuleIndexer.TryGetCachedRegistry(registry) is IRegistryModuleMetadataProvider cachedRegistry
                && await cachedRegistry.TryGetModuleVersionsAsync(modulePath) is { } versionsMetadata
                && versionsMetadata.FirstOrDefault(x => x.Version.EqualsOrdinally(version)) is { } versionMetadata)
            {
                return completionItem with
                {
                    Detail = versionMetadata.Description,
                    Documentation = MarkdownHelper.GetDocumentationLink(versionsMetadata.Length > 0 ? versionsMetadata[0].DocumentationUri : null),
                };
            }

            return completionItem;
        }

        private ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetModuleAliases(RootConfiguration configuration)
        {
            return configuration.ModuleAliases.GetOciArtifactModuleAliases();
        }

        // Handles remote (OCI) module path completions, e.g. br: and br/
        private async Task<IEnumerable<CompletionItem>> GetOciModuleCompletions(BicepCompletionContext context, string trimmedText, RootConfiguration rootConfiguration) //asdfg rename?
        {
            if (!IsOciArtifactRegistryReference(trimmedText))
            {
                return [];
            }

            return [
                .. await GetModuleCompletions(trimmedText, context, rootConfiguration),
                .. GetPartialPrivatePathCompletionsFromAliases(trimmedText, context, rootConfiguration),
                .. await GetPublicPathCompletionFromAliases(trimmedText, context, rootConfiguration),
            ];
        }

        // Handles path completions for case where user has specified an alias in bicepconfig.json with registry set to "mcr.microsoft.com".
        private async Task<IEnumerable<CompletionItem>> GetPublicPathCompletionFromAliases(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration) //asdfg rewrite or remove
        {
            List<CompletionItem> completions = new();

            if (IsPrivateRegistryReference(trimmedText, out _))
            {
                return completions;
            }

            foreach (var kvp in GetModuleAliases(rootConfiguration)) //asdfg test
            {
                if (kvp.Value.Registry is string inputRegistry)
                {
                    //asdfg ntoe: breaks VerifyTelemetryEventIsPostedOnModuleRegistryPathCompletion
                    // We currently don't support path completion for private modules, but we'll go ahead and log telemetry to track usage. asdfg
                    if (!inputRegistry.Equals(PublicMcrRegistry, StringComparison.Ordinal) && //asdfg?
                        trimmedText.Equals($"br/{kvp.Key}:"))
                    {
                        telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));
                        break;
                    }

                    // br/[alias-that-points-to-mcr.microsoft.com]:<cursor>
                    if (inputRegistry.Equals(PublicMcrRegistry, StringComparison.Ordinal) &&
                        trimmedText.Equals($"br/{kvp.Key}:"))
                    {
                        var modulePath = kvp.Value.ModulePath;

                        if (modulePath is null)
                        {
                            // E.g bicepconfig.json
                            // {
                            //   "moduleAliases": {
                            //     "br": {
                            //       "test": {
                            //         "registry": "mcr.microsoft.com"
                            //       }
                            //     }
                            //   }
                            // }

                            if (trimmedText.Equals($"br/{kvp.Key}:", StringComparison.Ordinal)) //asdfg?
                            {
                                var modules = await registryModuleIndexer.GetRegistry(PublicMcrRegistry, rootConfiguration.Cloud).TryGetModulesAsync();//asdfg testpoint
                                foreach (var (registry, moduleName, description, documentationUri) in modules)
                                {
                                    //asdfg make sure registry is inputRegistry?

                                    var label = $"bicep/{moduleName}";//asdfg??
                                    var insertText = $"'{trimmedText}bicep/{moduleName}:$0'";
                                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                                        .WithSnippetEdit(context.ReplacementRange, insertText)
                                        .WithFilterText(insertText)
                                        .WithSortText(GetSortText(label, ModuleCompletionPriority.Alias))
                                        .WithDetail(description)
                                        .WithDocumentation(MarkdownHelper.GetDocumentationLink(documentationUri))
                                        .WithFollowupCompletion("module version completion")
                                        .Build();

                                    completions.Add(completionItem);
                                }
                            }
                        }
                        else
                        {
                            // E.g bicepconfig.json
                            // {
                            //   "moduleAliases": {
                            //     "br": {
                            //       "test": {
                            //         "registry": "mcr.microsoft.com",
                            //         "modulePath": "bicep/app"
                            //       }
                            //     }
                            //   }
                            // }

                            if (modulePath.Equals(LanguageConstants.BicepPublicMcrPathPrefix, StringComparison.Ordinal) || !modulePath.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix, StringComparison.Ordinal))
                            {
                                continue;
                            }

                            // Completions are e.g. br/[alias]/[module]
                            var modulePathWithoutBicepKeyword = TrimStart(modulePath, LanguageConstants.BicepPublicMcrPathPrefix);
                            var modules = await registryModuleIndexer.GetRegistry(PublicMcrRegistry, rootConfiguration.Cloud).TryGetModulesAsync(); //asdfg testpoint

                            var matchingModules = modules.Where(x => x.ModuleName.StartsWith($"{modulePathWithoutBicepKeyword}/"));

                            foreach (var module in matchingModules)
                            {
                                var label = module.ModuleName.Substring($"{modulePathWithoutBicepKeyword}/".Length);

                                StringBuilder sb = new($"'{trimmedText}");
                                if (!trimmedText.EndsWith(':'))
                                {
                                    sb.Append(":");
                                }
                                sb.Append($"{label}:$0'");
                                var insertText = sb.ToString();

                                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                                    .WithSnippetEdit(context.ReplacementRange, insertText)
                                    .WithFilterText(insertText)
                                    .WithSortText(GetSortText(label, ModuleCompletionPriority.Alias))
                                    .WithDetail(module.Description)
                                    .WithDocumentation(MarkdownHelper.GetDocumentationLink(module.DocumentationUri))
                                    .WithFollowupCompletion("module version completion")
                                    .Build();
                                completions.Add(completionItem);
                            }
                        }
                    }
                }
            }

            if (completions.Any())
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.MCR));
            }

            return completions;
        }

        private string TrimStart(string text, string prefixToTrim) => text.StartsWith(prefixToTrim) ? text.Substring(prefixToTrim.Length) : text;

        private string? GetFirstMatch(Regex regex, string text, string group, bool allowEmpty)
        {
            var matches = regex.Matches(text);
            if (!matches.Any())
            {
                return null;
            }

            string? value = matches[0].Groups[group].Value;
            if (!allowEmpty && string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// True if a direct reference to a private ACR registry (i.e. not pointing to the Microsoft public bicep registry)
        /// Example:
        ///   "br:privateacr.azurecr.io/" => true
        /// </summary>
        /// <param name="text"></param>
        /// <param name="registry">Won't be null with true return value, but could be empty</param>
        /// <returns></returns>
        private bool IsPrivateRegistryReference(string text, [NotNullWhen(true)] out string? registry)
        {
            registry = GetFirstMatch(ModulePrefixWithFullPath, text, "registry", allowEmpty: false);
            return registry is not null && !registry.Equals(PublicMcrRegistry, StringComparison.Ordinal);
        }

        // We only support partial path completions for ACR using module paths listed in bicepconfig.json

        // Handles ACR path completions for full paths, but only for the case where the user has configured an alias in bicepconfig.json.
        // Example:
        //   bicepconfig.json:
        //   {
        //     "moduleAliases": {
        //       "br": {
        //         "whatever": {
        //           "registry": "privateacr.azurecr.io",
        //           "modulePath": "bicep/app"
        //    ...
        //
        //   br:privateacr.azurecr.io/<CURSOR>
        //      =>
        //   br:privateacr.azurecr.io/bicep/app:<CURSOR>
        //asdfg make sure tested, then remove
        private IEnumerable<CompletionItem> GetPartialPrivatePathCompletionsFromAliases(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration) //asdfg test
        {
            if (ParseParts(trimmedText, rootConfiguration) is not Parts parts
                || parts.HasVersionSeparator)
            {
                return [];
            }

            List<CompletionItem> completions = new();

            if (!IsPrivateRegistryReference(trimmedText, out string? registry) || string.IsNullOrWhiteSpace(registry))
            {
                return completions;
            }

            telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));
            foreach (var kvp in GetModuleAliases(rootConfiguration))
            {
                if (registry.Equals(kvp.Value.Registry, StringComparison.Ordinal))
                {
                    var modulePath = kvp.Value.ModulePath;

                    if (modulePath is null)
                    {
                        continue;
                    }

                    var insertText = $"'{trimmedText}{modulePath}:$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, modulePath)
                       .WithSnippetEdit(context.ReplacementRange, insertText)
                       .WithFilterText(insertText)
                       .WithSortText(GetSortText(modulePath))
                       .WithFollowupCompletion("module path completion")
                       .Build();
                    completions.Add(completionItem);
                }
            }

            return completions;
        }

        //asdfg make sure sorted by version

        // Handles module path completions for MCR:
        //   br/public:<CURSOR>
        // or
        //   br:mcr.microsoft.com/bicep/:<CURSOR>
        private async Task<IEnumerable<CompletionItem>> GetModuleCompletions(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration)//asdfgasdfgasdfg
        {
            if (ParseParts(trimmedText, rootConfiguration) is not Parts parts
                || parts.ResolvedModulePath is null
                || parts.HasVersionSeparator)
            {
                return [];
            }

            List<CompletionItem> completions = new();

            var modules = await registryModuleIndexer.GetRegistry(parts.ResolvedRegistry, rootConfiguration.Cloud).TryGetModulesAsync(); //asdfg2
            foreach (var (registry, moduleName, description, documentationUri) in modules)
            {
                if (!moduleName.StartsWith(parts.ResolvedModulePath, StringComparison.Ordinal)) //asdfg case-insensitive
                {
                    continue;
                }

                string insertText = $"'{parts.WithModulePath(moduleName).ToNotation()}:$0'";

                // Remove the base path prefix from the label if we're dealing with a module alias, because the user doesn't need to see it
                var label = !string.IsNullOrWhiteSpace(parts.SpecifiedAlias) && !string.IsNullOrWhiteSpace(parts.ModulePathPrefix)
                    ? moduleName.Substring(parts.ModulePathPrefixWithSeparator.Length)
                    : moduleName;

                var completionItem = CompletionItemBuilder.Create(
                    CompletionItemKind.Snippet, label)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithFilterText(insertText)
                        .WithSortText(GetSortText(moduleName))
                        .WithDetail(description)
                        .WithDocumentation(MarkdownHelper.GetDocumentationLink(documentationUri))
                        .WithFollowupCompletion("module version completion")
                        .Build();

                completions.Add(completionItem);
            }

            if (completions.Any())
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.MCR/*asdfg wrong*/));
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

                completions.AddRange(await GetPrivateModuleCompletionsAsdfg(trimmedText, context, rootConfiguration, cancellationToken));
            }

            return completions;
        }

        // Handles registry name completions for private modules possibly available in ACR registries
        private async Task<IEnumerable<CompletionItem>> GetPrivateModuleCompletionsAsdfg(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration, CancellationToken cancellationToken)
        {
            if (settingsProvider.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting))
            {
                return await GetACRModuleRegistriesCompletionsFromAzure(trimmedText, context, rootConfiguration, cancellationToken);
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
        private async Task<IEnumerable<CompletionItem>> GetACRModuleRegistriesCompletionsFromAzure(string trimmedText, BicepCompletionContext context, RootConfiguration rootConfiguration, CancellationToken cancellationToken)
        {
            List<CompletionItem> completions = new();

            try
            {
                await foreach (string registryName in azureContainerRegistriesProvider.GetRegistryUrisAccessibleFromAzure(rootConfiguration.Cloud, cancellationToken)
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

        public async Task<CompletionItem> ResolveCompletionItem(CompletionItem completionItem, CancellationToken cancellationToken)
        {
            if (completionItem.Data != null && completionItem.Data[ModuleVersionsResolutionKey] is JObject data)
            {
                var registry = data["Registry"]?.ToString();
                var modulePath = data["Module"]?.ToString();
                var version = data["Version"]?.ToString();

                if (registry != null && modulePath != null && version != null)
                {
                    return await ResolveVersion(completionItem, registry, modulePath, version, cancellationToken);
                }
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
