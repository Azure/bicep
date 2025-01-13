// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Provides completions for OCI (public or private) module references, e.g. br/public:modulePath:version
    /// </summary>
    public class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IAzureContainerRegistriesProvider azureContainerRegistriesProvider;

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

        // Direct reference to a full registry login server URI via br:<registry>
        private static readonly Regex ModulePrefixWithFullPath = new(@"^br:(?<registry>(.*?))/", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Aliased reference to a registry via br/alias:path
        private static readonly Regex ModuleWithAliasAndVersionSeparator = new(@"^br/(.*):(?<path>(.*?)):", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Direct reference to the MCR (public) registry via br:mcr.microsoft.com/bicep/path
        private static readonly Regex PublicModuleWithFullPathAndVersionSeparator = new($"^br:{PublicMCRRegistry}/bicep/(?<path>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Aliased reference to the MCR (public) registry via br/public:
        private static readonly Regex PublicModuleWithAliasAndVersionSeparator = new(@"^br/public:(?<path>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private const string PublicMCRRegistry = LanguageConstants.BicepPublicMcrRegistry; // "mcr.microsoft.com"

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistriesProvider azureContainerRegistriesProvider,
            IConfigurationManager configurationManager,
            IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider,
            ISettingsProvider settingsProvider,
            ITelemetryProvider telemetryProvider)
        {
            this.azureContainerRegistriesProvider = azureContainerRegistriesProvider;
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

            var completions = GetTopLevelCompletions(context, replacementText, sourceFileUri);

            var startsWithSingleQuote = replacementText.StartsWith('\'');
            if (startsWithSingleQuote)
            {
                var trimmedReplacementText = replacementText.Trim('\'');

                var replacementsRequiringStartingQuote =
                    GetOciModulePathCompletions(context, trimmedReplacementText, sourceFileUri)
                    .Concat(GetPublicModuleVersionCompletions(context, trimmedReplacementText, sourceFileUri))
                    .Concat(await GetAllRegistryNameAndAliasCompletions(context, trimmedReplacementText, sourceFileUri, cancellationToken));

                completions = [
                    .. completions,
                    .. replacementsRequiringStartingQuote,
                ];
            }

            return completions;
        }

        // Handles bicep registry and template spec top-level schema completions.
        // I.e. typing with an empty path:  module m1 <CURSOR>
        private IEnumerable<CompletionItem> GetTopLevelCompletions(BicepCompletionContext context, string untrimmedReplacementText, Uri sourceFileUri)
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

            var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
            var templateSpecModuleAliases = rootConfiguration.ModuleAliases.GetTemplateSpecModuleAliases();
            var bicepModuleAliases = GetModuleAliases(sourceFileUri);

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
                    var detail = (string.CompareOrdinal(registry, PublicMCRRegistry) == 0 && string.CompareOrdinal(modulePath, "bicep") == 0)
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

        // Handles version completions for Microsoft Container Registries (MCR):
        //
        //   br/module/name:<CURSOR>
        //   br:mcr.microsoft/bicep/module/name:<CURSOR>
        //
        // etc
        private IEnumerable<CompletionItem> GetPublicModuleVersionCompletions(BicepCompletionContext context, string trimmedText, Uri sourceFileUri)
        {
            if (!IsOciArtifactRegistryReference(trimmedText))
            {
                return [];
            }

            string? modulePath;

            if (PublicModuleWithAliasAndVersionSeparator.IsMatch(trimmedText))
            {
                var matches = PublicModuleWithAliasAndVersionSeparator.Matches(trimmedText);
                modulePath = matches[0].Groups["path"].Value;
            }
            else if (PublicModuleWithFullPathAndVersionSeparator.IsMatch(trimmedText))
            {
                var matches = PublicModuleWithFullPathAndVersionSeparator.Matches(trimmedText);
                modulePath = matches[0].Groups["path"].Value;
            }
            else
            {
                modulePath = GetAliasedMCRModulePath(trimmedText, sourceFileUri);
            }

            if (modulePath is null)
            {
                return [];
            }

            List<CompletionItem> completions = new();

            var versionsMetadata = publicRegistryModuleMetadataProvider.GetModuleVersionsMetadata(modulePath);

            for (int i = versionsMetadata.Length - 1; i >= 0; i--)
            {
                var (version, description, documentationUri) = versionsMetadata[i];

                var insertText = $"'{trimmedText}{version}'$0";

                // Module version is last completion, no follow-up completions triggered
                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, version)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(i))
                    .WithDetail(description)
                    .WithDocumentation(MarkdownHelper.GetDocumentationLink(documentationUri))
                    .Build();

                completions.Add(completionItem);
            }

            return completions;

            // Handles scenario where the user has configured an alias for MCR in bicepconfig.json.
            string? GetAliasedMCRModulePath(string trimmedText, Uri sourceFileUri)
            {
                foreach (var kvp in GetModuleAliases(sourceFileUri))
                {
                    if (kvp.Value.Registry is string registry &&
                        registry.Equals(PublicMCRRegistry, StringComparison.Ordinal))
                    {
                        var aliasFromBicepConfig = $"br/{kvp.Key}:";

                        if (trimmedText.StartsWith(aliasFromBicepConfig, StringComparison.Ordinal))
                        {
                            var matches = ModuleWithAliasAndVersionSeparator.Matches(trimmedText);

                            if (!matches.Any())
                            {
                                continue;
                            }

                            string subpath = matches[0].Groups["path"].Value;

                            if (subpath is null)
                            {
                                continue;
                            }

                            var modulePath = kvp.Value.ModulePath;

                            if (modulePath is not null)
                            {
                                if (modulePath.StartsWith("bicep/"))
                                {
                                    modulePath = modulePath.Substring("bicep/".Length);
                                    return $"{modulePath}/{subpath}";
                                }
                            }
                            else
                            {
                                if (subpath.StartsWith("bicep/"))
                                {
                                    return subpath.Substring("bicep/".Length);
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }

        private ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetModuleAliases(Uri sourceFileUri)
        {
            var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
            return rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();
        }

        // Handles remote (OCI) path completions, e.g. br: and br/
        private IEnumerable<CompletionItem> GetOciModulePathCompletions(BicepCompletionContext context, string trimmedText, Uri sourceFileUri)
        {
            if (!IsOciArtifactRegistryReference(trimmedText))
            {
                return [];
            }

            return [
                .. GetPublicModuleCompletions(trimmedText, context),
                .. GetPartialPrivatePathCompletionsFromAliases(trimmedText, context, sourceFileUri),
                .. GetPublicPathCompletionFromAliases(trimmedText, context, sourceFileUri),
            ];
        }


        // Handles path completions for case where user has specified an alias in bicepconfig.json with registry set to "mcr.microsoft.com".
        private IEnumerable<CompletionItem> GetPublicPathCompletionFromAliases(string trimmedText, BicepCompletionContext context, Uri sourceFileUri)
        {
            List<CompletionItem> completions = new();

            if (IsPrivateRegistryReference(trimmedText, out _))
            {
                return completions;
            }

            foreach (var kvp in GetModuleAliases(sourceFileUri))
            {
                if (kvp.Value.Registry is string registry)
                {
                    // We currently don't support path completion for private modules, but we'll go ahead and log telemetry to track usage.
                    if (!registry.Equals(PublicMCRRegistry, StringComparison.Ordinal) &&
                        trimmedText.Equals($"br/{kvp.Key}:"))
                    {
                        telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));
                        break;
                    }

                    // br/[alias-that-points-to-mcr.microsoft.com]:<cursor>
                    if (registry.Equals(PublicMCRRegistry, StringComparison.Ordinal) &&
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

                            if (trimmedText.Equals($"br/{kvp.Key}:", StringComparison.Ordinal))
                            {
                                var modules = publicRegistryModuleMetadataProvider.GetModulesMetadata();
                                foreach (var (moduleName, description, documentationUri) in modules)
                                {
                                    var label = $"bicep/{moduleName}";
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

                            if (modulePath.Equals("bicep", StringComparison.Ordinal) || !modulePath.StartsWith("bicep/", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            // Completions are e.g. br/[alias]/[module]
                            var modulePathWithoutBicepKeyword = TrimStart(modulePath, "bicep/");
                            var modules = publicRegistryModuleMetadataProvider.GetModulesMetadata();

                            var matchingModules = modules.Where(x => x.Name.StartsWith($"{modulePathWithoutBicepKeyword}/"));

                            foreach (var module in matchingModules)
                            {
                                var label = module.Name.Substring($"{modulePathWithoutBicepKeyword}/".Length);

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
            registry = null;

            var matches = ModulePrefixWithFullPath.Matches(text);
            if (!matches.Any())
            {
                return false;
            }

            registry = matches[0].Groups["registry"].Value;

            return !registry.Equals(PublicMCRRegistry, StringComparison.Ordinal);
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
        private IEnumerable<CompletionItem> GetPartialPrivatePathCompletionsFromAliases(string trimmedText, BicepCompletionContext context, Uri sourceFileUri)
        {
            List<CompletionItem> completions = new();

            if (!IsPrivateRegistryReference(trimmedText, out string? registry) || string.IsNullOrWhiteSpace(registry))
            {
                return completions;
            }

            telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.ACR));
            foreach (var kvp in GetModuleAliases(sourceFileUri))
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

        // Handles module path completions for MCR:
        //   br/public:<CURSOR>
        // or
        //   br:mcr.microsoft.com/bicep/:<CURSOR>
        private IEnumerable<CompletionItem> GetPublicModuleCompletions(string trimmedText, BicepCompletionContext context)
        {
            var (prefix, suffix) = trimmedText switch
            {
                { } x when x.StartsWith("br/public:", StringComparison.Ordinal) => ("br/public:", x["br/public:".Length..]),
                { } x when x.StartsWith($"br:{PublicMCRRegistry}/bicep/", StringComparison.Ordinal) => ($"br:{PublicMCRRegistry}/bicep/", x[$"br:{PublicMCRRegistry}/bicep/".Length..]),
                _ => (null, null),
            };

            if (prefix is null || suffix is null)
            {
                return [];
            }

            List<CompletionItem> completions = new();

            var modules = publicRegistryModuleMetadataProvider.GetModulesMetadata();
            foreach (var (moduleName, description, documentationUri) in modules)
            {
                if (!moduleName.StartsWith(suffix, StringComparison.Ordinal))
                {
                    continue;
                }

                var insertText = $"'{prefix}{moduleName}:$0'";

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, moduleName)
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
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.MCR));
            }

            return completions;
        }

        // Handles top-level completions of registry names/aliases after br: and br/
        private async Task<IEnumerable<CompletionItem>> GetAllRegistryNameAndAliasCompletions(BicepCompletionContext context, string trimmedText, Uri sourceFileUri, CancellationToken cancellationToken)
        {
            var completions = new List<CompletionItem>();

            if (!IsOciArtifactRegistryReference(trimmedText))
            {
                return completions;
            }

            if (trimmedText == "br/")
            {
                foreach (var kvp in GetModuleAliases(sourceFileUri))
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
                var label = $"{PublicMCRRegistry}/bicep";
                var insertText = $"'{trimmedText}{label}/$0'";
                var mcrCompletionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                    .WithFilterText(insertText)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(label))
                    .WithFollowupCompletion("module repository completion")
                    .Build();

                completions.Add(mcrCompletionItem);

                completions.AddRange(await GetPrivateModuleCompletions(trimmedText, context, sourceFileUri, cancellationToken));
            }

            return completions;
        }

        // Handles registry name completions for private modules possibly available in ACR registries
        private async Task<IEnumerable<CompletionItem>> GetPrivateModuleCompletions(string trimmedText, BicepCompletionContext context, Uri sourceFileUri, CancellationToken cancellationToken)
        {
            if (settingsProvider.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting))
            {
                return await GetACRModuleRegistriesCompletionsFromAzure(trimmedText, context, sourceFileUri, cancellationToken);
            }
            else
            {
                // CONSIDER: Somehow indicate in the completion list that users can get more completions by setting GetAllAzureContainerRegistriesForCompletionsSetting
                return GetACRModuleRegistriesCompletionsFromBicepConfig(trimmedText, context, sourceFileUri);
            }
        }

        // Handles private registry name completions for modules available in ACR registries using ResourceGraphClient query.
        // This returns all registries that the user has access to via Azure (whether or not they contain bicep modules, and whether
        //   or not they're registered in the bicepconfig.json file)
        // This is for completions after typing "br:"
        private async Task<IEnumerable<CompletionItem>> GetACRModuleRegistriesCompletionsFromAzure(string trimmedText, BicepCompletionContext context, Uri sourceFileUri, CancellationToken cancellationToken)
        {
            List<CompletionItem> completions = new();

            try
            {
                await foreach (string registryName in azureContainerRegistriesProvider.GetRegistryUrisAccessibleFromAzure(sourceFileUri, cancellationToken)
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

        // Handles private ACR registry name completions only for registries that are configured via aliases in the bicepconfig.json file
        private IEnumerable<CompletionItem> GetACRModuleRegistriesCompletionsFromBicepConfig(string trimmedText, BicepCompletionContext context, Uri sourceFileUri)
        {
            List<CompletionItem> completions = new();
            HashSet<string> aliases = new();

            foreach (var kvp in GetModuleAliases(sourceFileUri))
            {
                var label = kvp.Value.Registry;

                if (label is not null && !label.Equals(PublicMCRRegistry, StringComparison.Ordinal))
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
