// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Handlers;
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
        private readonly IAzureContainerRegistriesProvider azureContainerRegistryNamesProvider;
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

        private static readonly ImmutableDictionary<string, (string, ModuleCompletionPriority)> DefaultSchemaCompletionLabelsWithDetails = new Dictionary<string, (string, ModuleCompletionPriority)>()
        {
            {"br:", (ModuleFullPathCompletionLabel, ModuleCompletionPriority.FullPath) },
            {"br/", (ModuleAliasCompletionLabel, ModuleCompletionPriority.Alias) },
            {"ts:", (TemplateSpecFullPathCompletionLabel, ModuleCompletionPriority.FullPath) }
        }.ToImmutableDictionary();

        // Direct reference to a full registry login server URI via br:<registry>
        private static readonly Regex ModuleRegistryWithoutAlias = new Regex(@"'br:(?<registry>(.*?))/'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        // Aliased reference to a registry via br/alias:path
        private static readonly Regex ModuleRegistryWithAliasAndPath = new Regex(@"'br/(.*):(?<filePath>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        // Direct reference to the MCR registry via br:mcr.microsoft.com/bicep/path
        private static readonly Regex MCRModuleRegistryWithoutAlias = new Regex($"br:{MCRRegistry}/bicep/(?<filePath>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        // Aliased reference to the MCR registry via br/public:
        private static readonly Regex MCRModuleRegistryWithAlias = new Regex(@"br/public:(?<filePath>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private const string MCRRegistry = "mcr.microsoft.com";

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistriesProvider azureContainerRegistryNamesProvider,
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

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri sourceFileUri, BicepCompletionContext context, CancellationToken cancellationToken)
        {
            var replacementText = string.Empty;

            if (context.ReplacementTarget is Token token)
            {
                replacementText = token.Text;
            }

            return GetBicepRegistryAndTemplateSpecShemaCompletions(context, replacementText, sourceFileUri)
                .Concat(await GetOciModulePathCompletions(context, replacementText, sourceFileUri))
                .Concat(await GetMCRModuleRegistryVersionCompletions(context, replacementText, sourceFileUri))
                .Concat(await GetAllRegistryNameAndAliasCompletions(context, replacementText, sourceFileUri, cancellationToken));
        }

        // Handles bicep registry and template spec top-level schema completions.
        // E.g. br:, br/, ts:, ts/[alias]
        private IEnumerable<CompletionItem> GetBicepRegistryAndTemplateSpecShemaCompletions(BicepCompletionContext context, string replacementText, Uri sourceFileUri)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (!string.IsNullOrWhiteSpace(replacementText.Trim('\'')))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
            var templateSpecModuleAliases = rootConfiguration.ModuleAliases.GetTemplateSpecModuleAliases();

            var completionLabelsWithDetails = DefaultSchemaCompletionLabelsWithDetails;
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

        /// <summary>
        /// True if is an OCR module reference (starts with br: or br/)
        /// </summary>
        private bool IsOciModuleRegistryReference(string replacementText)
        {
            if (replacementText.StartsWith("'br/") || replacementText.StartsWith("'br:"))
            {
                return true;
            }

            return false;
        }

        // Handles version completions for Microsoft Container Registries (MCR).
        // I.e. completions starting with "br/" or "br:"
        private async Task<IEnumerable<CompletionItem>> GetMCRModuleRegistryVersionCompletions(BicepCompletionContext context, string replacementText, Uri sourceFileUri)
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
                filePath = GetNonPublicMCRFilePathForVersionCompletion(replacementText, sourceFileUri);
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
        private string? GetNonPublicMCRFilePathForVersionCompletion(string replacementText, Uri sourceFileUri)
        {
            foreach (var kvp in GetOciArtifactModuleAliases(sourceFileUri))
            {
                if (kvp.Value.Registry is string registry &&
                    registry.Equals(MCRRegistry, StringComparison.Ordinal))
                {
                    var aliasFromBicepConfig = $"'br/{kvp.Key}:";
                    var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

                    if (replacementTextWithTrimmedEnd.StartsWith(aliasFromBicepConfig, StringComparison.Ordinal))
                    {
                        var matches = ModuleRegistryWithAliasAndPath.Matches(replacementTextWithTrimmedEnd);

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

        private ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetOciArtifactModuleAliases(Uri sourceFileUri)
        {
            var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
            return rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();
        }

        // Handles remote (OCR) path completions, e.g. br: and br/
        private async Task<IEnumerable<CompletionItem>> GetOciModulePathCompletions(BicepCompletionContext context, string replacementText, Uri sourceFileUri)
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

                completions.AddRange(GetACRPartialPathCompletionsFromBicepConfig(replacementText, context, sourceFileUri));
                completions.AddRange(await GetMCRPathCompletionFromBicepConfig(replacementText, context, sourceFileUri));

                return completions;
            }
        }


        // Handles path completions for case where user has specified an alias in bicepconfig.json with registry set to "mcr.microsoft.com".
        private async Task<IEnumerable<CompletionItem>> GetMCRPathCompletionFromBicepConfig(string replacementText, BicepCompletionContext context, Uri sourceFileUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            if (IsPrivateAcrRegistryReference(replacementTextWithTrimmedEnd, out _))
            {
                return completions;
            }

            foreach (var kvp in GetOciArtifactModuleAliases(sourceFileUri))
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

        /// <summary>
        /// True if a direct reference to a private ACR registry (i.e. not pointing to the Microsoft public bicep registry)
        /// </summary>
        /// <param name="replacementTextWithTrimmedEnd"></param>
        /// <param name="registry"></param>
        /// <returns></returns>
        private bool IsPrivateAcrRegistryReference(string replacementTextWithTrimmedEnd, out string? registry)
        {
            registry = null;

            var matches = ModuleRegistryWithoutAlias.Matches(replacementTextWithTrimmedEnd);
            if (!matches.Any())
            {
                return false;
            }

            registry = matches[0].Groups["registry"].Value;

            return !registry.Equals(MCRRegistry, StringComparison.Ordinal);
        }

        // We only support partial path completions for ACR using module paths listed in bicepconfig.json
        private IEnumerable<CompletionItem> GetACRPartialPathCompletionsFromBicepConfig(string replacementText, BicepCompletionContext context, Uri sourceFileUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');
            if (!IsPrivateAcrRegistryReference(replacementTextWithTrimmedEnd, out string? registry) || string.IsNullOrWhiteSpace(registry))
            {
                return completions;
            }

            telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));

            foreach (var kvp in GetOciArtifactModuleAliases(sourceFileUri))
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
        // I.e., completions after "br/public:" or "br:{MCRRegistry}/bicep/:"
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

        // Handles top-level completions of registry names/aliases after br: and br/
        private async Task<IEnumerable<CompletionItem>> GetAllRegistryNameAndAliasCompletions(BicepCompletionContext context, string replacementText, Uri sourceFileUri, CancellationToken cancellationToken)
        {
            var completions = new List<CompletionItem>();

            if (!IsOciModuleRegistryReference(replacementText))
            {
                return completions;
            }

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            if (replacementTextWithTrimmedEnd == "'br/")
            {
                foreach (var kvp in GetOciArtifactModuleAliases(sourceFileUri))
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

                IEnumerable<CompletionItem> acrCompletions = await GetACRModuleRegistriesCompletions(replacementText, context, sourceFileUri, cancellationToken);
                completions.AddRange(acrCompletions);
            }

            return completions;
        }

        // Handles registry name completions for private modules possibly available in ACR registries
        private async Task<IEnumerable<CompletionItem>> GetACRModuleRegistriesCompletions(string replacementText, BicepCompletionContext context, Uri sourceFileUri, CancellationToken cancellationToken)
        {
            if (settingsProvider.GetSetting(LangServerConstants.IncludeAllAccessibleAzureContainerRegistriesForCompletionsSetting))
            {
                return await GetACRModuleRegistriesCompletionsFromGraphClient(replacementText, context, sourceFileUri, cancellationToken);
            }
            else
            {
                return GetACRModuleRegistriesCompletionsFromBicepConfig(replacementText, context, sourceFileUri);
            }
        }

        // Handles private registry name completions for modules available in ACR registries using ResourceGraphClient query.
        // This returns all registries that the user has access to via Azure (whether or not they contain bicep modules, and whether
        //   or not they're registered in the bicepconfig.json file)
        private async Task<IEnumerable<CompletionItem>> GetACRModuleRegistriesCompletionsFromGraphClient(string replacementText, BicepCompletionContext context, Uri sourceFileUri, CancellationToken cancellationToken)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var registryNames = await azureContainerRegistryNamesProvider.GetRegistryUris(sourceFileUri, cancellationToken);

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

        // Handles private ACR registry name completions only for registries that are configured in the bicepconfig.json file
        private IEnumerable<CompletionItem> GetACRModuleRegistriesCompletionsFromBicepConfig(string replacementText, BicepCompletionContext context, Uri sourceFileUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();
            HashSet<string> aliases = new HashSet<string>();

            foreach (var kvp in GetOciArtifactModuleAliases(sourceFileUri))
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
