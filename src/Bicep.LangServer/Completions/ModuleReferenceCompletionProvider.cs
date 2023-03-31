// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Provides completions for remote (public or private) module references, e.g. br/public:modulePath:version
    /// </summary>
    public class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IAzureContainerRegistryNamesProvider azureContainerRegistryNamesProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider;
        private readonly ISettingsProvider settingsProvider;
        private readonly ITelemetryProvider telemetryProvider;

        private enum ModuleCompletionPriority
        {
            Alias = 0,     // br/[alias], ts/[alias]
            Default = 1,
            FullPath = 2,  // br:, ts:
        }

        private const string ModuleFullPathCompletionLabel = "Bicep registry";
        private const string ModuleAliasCompletionLabel = "Bicep registry (alias)";
        private const string TemplateSpecFullPathCompletionLabel = "Template spec";
        private const string TemplateSpecAliasCompletionLabel = "Template spec (alias)";

        private static readonly ImmutableDictionary<string, (string, ModuleCompletionPriority)> BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails = new Dictionary<string, (string, ModuleCompletionPriority)>()
        {
            {"br:", (ModuleFullPathCompletionLabel, ModuleCompletionPriority.FullPath) },
            {"br/", (ModuleAliasCompletionLabel, ModuleCompletionPriority.Alias) },
            {"ts:", (TemplateSpecFullPathCompletionLabel, ModuleCompletionPriority.FullPath) }
        }.ToImmutableDictionary();

        private static readonly Regex ModuleRegistryWithoutAlias = new Regex(@"'br:(?<regxistry>(.*?))/'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex MCRModuleRegistryWithAlias = new Regex(@"br/public:(?<filePath>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex MCRModuleRegistryWithoutAlias = new Regex($"br:{MCRRegistry}/bicep/(?<filePath>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex ModuleRegistryAliasWithPath = new Regex(@"'br/(.*):(?<filePath>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private const string MCRRegistry = "mcr.microsoft.com";

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistryNamesProvider azureContainerRegistryNamesProvider,
            IConfigurationManager configurationManager,
            IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider,
            ISettingsProvider settingsProvider,
            ITelemetryProvider telemetryProvider)
        {
            this.azureContainerRegistryNamesProvider = azureContainerRegistryNamesProvider;
            this.configurationManager = configurationManager;
            this.publicRegistryModuleMetadataProvider = publicRegistryModuleMetadataProvider;
            this.settingsProvider = settingsProvider;
            this.telemetryProvider = telemetryProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri templateUri, BicepCompletionContext context)
        {
            if (!settingsProvider.GetSetting(LangServerConstants.EnableModuleRegistryReferenceCompletionsSetting))
            {
                return Enumerable.Empty<CompletionItem>();
            }
            var replacementText = string.Empty;

            if (context.ReplacementTarget is Token token)
            {
                replacementText = token.Text;
            }

            return GetBicepRegistryAndTemplateSpecShemaCompletions(context, replacementText, templateUri)
                .Concat(await GetPathCompletions(context, replacementText, templateUri))
                .Concat(await GetMCRModuleRegistryVersionCompletions(context, replacementText, templateUri))
                .Concat(await GetRegistryCompletions(context, replacementText, templateUri));
        }

        // Handles bicep registry and template spec schema completions.
        private IEnumerable<CompletionItem> GetBicepRegistryAndTemplateSpecShemaCompletions(BicepCompletionContext context, string replacementText, Uri templateUri)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (!string.IsNullOrWhiteSpace(replacementText.Trim('\'')))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var rootConfiguration = configurationManager.GetConfiguration(templateUri);
            var templateSpecModuleAliases = rootConfiguration.ModuleAliases.GetTemplateSpecModuleAliases();

            var completionLabelsWithDetails = BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails;
            if (templateSpecModuleAliases.Any())
            {
                completionLabelsWithDetails = completionLabelsWithDetails.Add("ts/", (TemplateSpecAliasCompletionLabel, ModuleCompletionPriority.Alias));
            }

            List<CompletionItem> completionItems = new List<CompletionItem>();
            foreach (var kvp in completionLabelsWithDetails)
            {
                var text = kvp.Key;
                var insertionText = $"'{text}$0'";
                (var details, ModuleCompletionPriority completionPriority) = kvp.Value;

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, text)
                    .WithFilterText(insertionText)
                    .WithSortText(GetSortText(text, completionPriority))
                    .WithSnippetEdit(context.ReplacementRange, insertionText)
                    .WithDetail(details)
                    .Build();
                completionItems.Add(completionItem);
            }

            return completionItems;
        }

        private bool IsOciModuleRegistryReference(string replacementText)
        {
            if (replacementText.StartsWith("'br/") || replacementText.StartsWith("'br:"))
            {
                return true;
            }

            return false;
        }

        // Handles version completions for Microsoft Container Registries(MCR).
        private async Task<IEnumerable<CompletionItem>> GetMCRModuleRegistryVersionCompletions(BicepCompletionContext context, string replacementText, Uri templateUri)
        {
            if (!IsOciModuleRegistryReference(replacementText))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            string? filePath;

            if (MCRModuleRegistryWithAlias.IsMatch(replacementText))
            {
                var matches = MCRModuleRegistryWithAlias.Matches(replacementText);
                filePath = matches[0].Groups["filePath"].Value;
            }
            else if (MCRModuleRegistryWithoutAlias.IsMatch(replacementText))
            {
                var matches = MCRModuleRegistryWithoutAlias.Matches(replacementText);
                filePath = matches[0].Groups["filePath"].Value;
            }
            else
            {
                filePath = GetNonPublicMCRFilePathForVersionCompletion(replacementText, templateUri);
            }

            if (filePath is null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completions = new List<CompletionItem>();
            replacementText = replacementText.TrimEnd('\'');

            var versions = await publicRegistryModuleMetadataProvider.GetVersions(filePath);
            for (int i = versions.Count() - 1; i >= 0; i--)
            {
                var version = versions.ElementAt(i);

                var insertText = $"{replacementText}{version}'$0";

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, version)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(version, i))
                    .Build();

                completions.Add(completionItem);
            }

            return completions;
        }

        // Handles scenario where the user has configured an alias for MCR in bicepconfig.json.
        private string? GetNonPublicMCRFilePathForVersionCompletion(string replacementText, Uri templateUri)
        {
            foreach (var kvp in GetOciArtifactModuleAliases(templateUri))
            {
                if (kvp.Value.Registry is string registry &&
                    registry.Equals(MCRRegistry, StringComparison.Ordinal))
                {
                    var aliasFromBicepConfig = $"'br/{kvp.Key}:";
                    var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

                    if (replacementTextWithTrimmedEnd.StartsWith(aliasFromBicepConfig, StringComparison.Ordinal))
                    {
                        var matches = ModuleRegistryAliasWithPath.Matches(replacementTextWithTrimmedEnd);

                        if (!matches.Any())
                        {
                            continue;
                        }

                        string filePath = matches[0].Groups["filePath"].Value;

                        if (filePath is null)
                        {
                            continue;
                        }

                        var modulePath = kvp.Value.ModulePath;

                        if (modulePath is not null)
                        {
                            if (modulePath.StartsWith("bicep/"))
                            {
                                modulePath = modulePath.Substring("bicep/".Length);
                                return $"{modulePath}/{filePath}";
                            }
                        }
                        else
                        {
                            if (filePath.StartsWith("bicep/"))
                            {
                                return filePath.Substring("bicep/".Length);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetOciArtifactModuleAliases(Uri templateUri)
        {
            var rootConfiguration = configurationManager.GetConfiguration(templateUri);
            return rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();
        }

        // Handles path completions.
        private async Task<IEnumerable<CompletionItem>> GetPathCompletions(BicepCompletionContext context, string replacementText, Uri templateUri)
        {
            if (!IsOciModuleRegistryReference(replacementText))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (replacementText == "'br/public:'" ||
                replacementText == $"'br:{MCRRegistry}/bicep/'" ||
                replacementText == "'br/public:" ||
                replacementText == $"'br:{MCRRegistry}/bicep/")
            {
                return await GetPublicMCRPathCompletions(replacementText, context);
            }
            else
            {
                List<CompletionItem> completions = new List<CompletionItem>();

                completions.AddRange(GetACRPartialPathCompletionsFromBicepConfig(replacementText, context, templateUri));
                completions.AddRange(await GetMCRPathCompletionFromBicepConfig(replacementText, context, templateUri));

                return completions;
            }
        }


        // Handles path completions for case where user has specified an alias in bicepconfig.json with registry set to "mcr.microsoft.com".
        private async Task<IEnumerable<CompletionItem>> GetMCRPathCompletionFromBicepConfig(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            if (IsAcrRegistryReference(replacementTextWithTrimmedEnd, out _))
            {
                return completions;
            }

            foreach (var kvp in GetOciArtifactModuleAliases(templateUri))
            {
                if (kvp.Value.Registry is string registry)
                {
                    // We currently don't support path completion for ACR, but we'll go ahead and log telemetry to track usage.
                    if (!registry.Equals(MCRRegistry, StringComparison.Ordinal) &&
                        replacementTextWithTrimmedEnd.Equals($"'br/{kvp.Key}:"))
                    {
                        telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));
                        break;
                    }

                    // br/[alias]:<cursor>
                    if (registry.Equals(MCRRegistry, StringComparison.Ordinal) &&
                        replacementTextWithTrimmedEnd.Equals($"'br/{kvp.Key}:"))
                    {
                        var modulePath = kvp.Value.ModulePath;

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
                        if (modulePath is null)
                        {
                            // Completions are e.g. br/[alias]/bicep/[module]
                            if (replacementTextWithTrimmedEnd.Equals($"'br/{kvp.Key}:", StringComparison.Ordinal))
                            {
                                var moduleNames = await publicRegistryModuleMetadataProvider.GetModuleNames();
                                foreach (var moduleName in moduleNames)
                                {
                                    var label = $"bicep/{moduleName}";
                                    var insertText = $"{replacementTextWithTrimmedEnd}bicep/{moduleName}:$0'";

                                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                                        .WithSnippetEdit(context.ReplacementRange, insertText)
                                        .WithFilterText(insertText)
                                        .WithSortText(GetSortText(label, ModuleCompletionPriority.Alias))
                                        .Build();

                                    completions.Add(completionItem);
                                }
                            }
                        }
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
                        else
                        {
                            if (modulePath.Equals("bicep", StringComparison.Ordinal) || !modulePath.StartsWith("bicep/", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            // Completions are e.g. br/[alias]/[module]
                            var modulePathWithoutBicepKeyword = modulePath.Substring("bicep/".Length);
                            var moduleNames = await publicRegistryModuleMetadataProvider.GetModuleNames();

                            var matchingModuleNames = moduleNames.Where(x => x.StartsWith($"{modulePathWithoutBicepKeyword}/"));

                            foreach (var moduleName in matchingModuleNames)
                            {
                                var label = moduleName.Substring($"{modulePathWithoutBicepKeyword}/".Length);
                                StringBuilder sb = new StringBuilder(replacementTextWithTrimmedEnd);

                                if (!replacementTextWithTrimmedEnd.EndsWith(":"))
                                {
                                    sb.Append(":");
                                }
                                sb.Append($"{label}:$0'");
                                var insertText = sb.ToString();
                                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                                    .WithSnippetEdit(context.ReplacementRange, insertText)
                                    .WithFilterText(insertText)
                                    .WithSortText(GetSortText(label, ModuleCompletionPriority.Alias))
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

        private bool IsAcrRegistryReference(string replacementTextWithTrimmedEnd, out string? registry)
        {
            registry = null;

            if (ModuleRegistryWithoutAlias.IsMatch(replacementTextWithTrimmedEnd))
            {
                var matches = ModuleRegistryWithoutAlias.Matches(replacementTextWithTrimmedEnd);
                if (!matches.Any())
                {
                    return false;
                }
                registry = matches[0].Groups["registry"].Value;

                return !registry.Equals(MCRRegistry, StringComparison.Ordinal);
            }

            return false;
        }

        // We only support partial path completions for ACR using module paths listed in bicepconfig.json
        private IEnumerable<CompletionItem> GetACRPartialPathCompletionsFromBicepConfig(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');
            if (!IsAcrRegistryReference(replacementTextWithTrimmedEnd, out string? registry) || string.IsNullOrWhiteSpace(registry))
            {
                return completions;
            }

            telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));

            foreach (var kvp in GetOciArtifactModuleAliases(templateUri))
            {
                if (registry.Equals(kvp.Value.Registry, StringComparison.Ordinal))
                {
                    var modulePath = kvp.Value.ModulePath;

                    if (modulePath is null)
                    {
                        continue;
                    }

                    var insertText = $"{replacementTextWithTrimmedEnd}{modulePath}:$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, modulePath)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithFilterText(insertText)
                        .WithSortText(GetSortText(modulePath))
                        .Build();
                    completions.Add(completionItem);
                }
            }

            return completions;
        }

        // Handles path completions for MCR.
        private async Task<IEnumerable<CompletionItem>> GetPublicMCRPathCompletions(string replacementText, BicepCompletionContext context)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            var moduleNames = await publicRegistryModuleMetadataProvider.GetModuleNames();
            foreach (var moduleName in moduleNames)
            {
                var insertText = $"{replacementTextWithTrimmedEnd}{moduleName}:$0'";

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, moduleName)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(moduleName))
                    .Build();

                completions.Add(completionItem);
            }

            if (completions.Any())
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.MCR));
            }

            return completions;
        }

        // Handles registry completions.
        private async Task<IEnumerable<CompletionItem>> GetRegistryCompletions(BicepCompletionContext context, string replacementText, Uri templateUri)
        {
            var completions = new List<CompletionItem>();

            if (!IsOciModuleRegistryReference(replacementText))
            {
                return completions;
            }

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            if (replacementTextWithTrimmedEnd == "'br/")
            {
                foreach (var kvp in GetOciArtifactModuleAliases(templateUri))
                {
                    var alias = kvp.Key;
                    var insertText = $"{replacementTextWithTrimmedEnd}{alias}:$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, alias)
                        .WithFilterText(insertText)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithSortText(GetSortText(alias))
                        .Build();
                    completions.Add(completionItem);
                }
            }
            else if (replacementTextWithTrimmedEnd == "'br:")
            {
                var label = $"{MCRRegistry}/bicep";
                var insertText = $"{replacementTextWithTrimmedEnd}{label}/$0'";
                var mcrCompletionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                    .WithFilterText(insertText)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(label))
                    .Build();

                completions.Add(mcrCompletionItem);

                IEnumerable<CompletionItem> acrCompletions = await GetACRModuleRegistriesCompletions(replacementText, context, templateUri);
                completions.AddRange(acrCompletions);
            }

            return completions;
        }

        // Handles registry completions for modules configured in ACR.
        private async Task<IEnumerable<CompletionItem>> GetACRModuleRegistriesCompletions(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            if (settingsProvider.GetSetting(LangServerConstants.IncludeAllAccessibleAzureContainerRegistriesForCompletionsSetting))
            {
                return await GetACRModuleRegistriesCompletionsFromGraphClient(replacementText, context, templateUri);
            }
            else
            {
                return GetACRModuleRegistriesCompletionsFromBicepConfig(replacementText, context, templateUri);
            }
        }

        // Handles registry completions for modules configured in ACR using ResourceGraphClient query.
        private async Task<IEnumerable<CompletionItem>> GetACRModuleRegistriesCompletionsFromGraphClient(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var registryNames = await azureContainerRegistryNamesProvider.GetRegistryNames(templateUri);

            foreach (string registryName in registryNames)
            {
                var replacementTextWithTrimmedEnd = replacementText.Trim('\'');
                var insertText = $"'{replacementTextWithTrimmedEnd}{registryName}/$0'";

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, registryName)
                    .WithFilterText(insertText)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(registryName, ModuleCompletionPriority.FullPath))
                    .Build();
                completions.Add(completionItem);
            }

            return completions;
        }

        // Handles ACR registry completions for entries in bicepconfig.json.
        private IEnumerable<CompletionItem> GetACRModuleRegistriesCompletionsFromBicepConfig(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();
            HashSet<string> aliases = new HashSet<string>();

            foreach (var kvp in GetOciArtifactModuleAliases(templateUri))
            {
                var label = kvp.Value.Registry;

                if (label is not null && !label.Equals(MCRRegistry, StringComparison.Ordinal))
                {
                    if (!aliases.TryGetValue(label, out _))
                    {
                        var replacementTextWithTrimmedEnd = replacementText.Trim('\'');
                        var insertText = $"'{replacementTextWithTrimmedEnd}{label}/$0'";
                        var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                            .WithFilterText(insertText)
                            .WithSnippetEdit(context.ReplacementRange, insertText)
                            .WithSortText(GetSortText(label))
                            .Build();
                        completions.Add(completionItem);

                        aliases.Add(label);
                    }
                }
            }

            return completions;
        }

        private static string GetSortText(string label, int priority) => $"{priority}_{label}";

        private static string GetSortText(string label, ModuleCompletionPriority priority = ModuleCompletionPriority.Default)
        {
            // We want all module completion priorities to come after other completions (e.g. local module paths), so we start with "9"
            return $"9{(int)priority}_{label}";
        }
    }
}
