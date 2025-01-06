// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable IDE0051 // Remove unused private members asdfg remove

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
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
        private readonly IRegistryModuleMetadataProvider[] registryModuleMetadataProviders;
        private readonly ISettingsProvider settingsProvider;
        private readonly ITelemetryProvider telemetryProvider;
        //asdfg private readonly IContainerRegistryClientFactory containerRegistryClientFactory;

        private enum ModuleCompletionPriority
        {
            Alias = 0,     // br/[alias], ts/[alias]
            Default = 1,
            FullPath = 2,  // br:, ts:
        }

        // Direct reference to a full registry login server URI via br:<registry>
        private static readonly Regex ModulePrefixWithFullPath = new(@"^br:(?<registry>(.*?))/", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Aliased reference to a registry via br/alias:path
        private static readonly Regex ModulePrefixWithAlias = new(@"^br/(?<alias>.*):(?<path>(.*?))", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex ModuleWithAliasAndVersionSeparator = new(@"^br/(.*):(?<path>(.*?)):", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Direct reference to the MCR (public) registry via br:mcr.microsoft.com/bicep/path
        private static readonly Regex PublicModuleWithFullPathAndVersionSeparator = new($"^br:{PublicMcrRegistry}/bicep/(?<path>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Aliased reference to the MCR (public) registry via br/public:
        private static readonly Regex PublicModuleWithAliasAndVersionSeparator = new(@"^br/public:(?<path>(.*?)):'?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private const string PublicMcrRegistry = LanguageConstants.BicepPublicMcrRegistry; // "mcr.microsoft.com"

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistriesProvider azureContainerRegistriesProvider,
            //asdfg IContainerRegistryClientFactory containerRegistryClientFactory,
            IConfigurationManager configurationManager,
            IRegistryModuleMetadataProvider[] registryModuleMetadataProviders,
            ISettingsProvider settingsProvider,
            ITelemetryProvider telemetryProvider)
        {
            this.azureContainerRegistriesProvider = azureContainerRegistriesProvider;
            //asdfg this.containerRegistryClientFactory = containerRegistryClientFactory;
            this.configurationManager = configurationManager;
            this.registryModuleMetadataProviders = registryModuleMetadataProviders;
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
                    GetOciModuleCompletions(context, trimmedReplacementText, sourceFileUri)
                    .Concat(GetPublicModuleVersionCompletions(context, trimmedReplacementText, sourceFileUri))
                    .Concat(await GetAllRegistryNameAndAliasCompletions(context, trimmedReplacementText, sourceFileUri, cancellationToken));

                completions = [
                    ..completions,
                    ..replacementsRequiringStartingQuote,
                ];
            }

            return completions;
        }

        // Handles bicep registry and template spec top-level schema completions.
        // I.e. typing with an empty path:  module m1 <CURSOR>
        private IEnumerable<CompletionItem> GetTopLevelCompletions(BicepCompletionContext context, string untrimmedReplacementText, Uri sourceFileUri)
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

        // Handles version completions for Microsoft Container Registries (MCR): asdfg
        //
        //   br/module/name:<CURSOR>
        //   br:mcr.microsoft/bicep/module/name:<CURSOR>
        //
        // etc
        private IEnumerable<CompletionItem> GetPublicModuleVersionCompletions(BicepCompletionContext context, string trimmedText, Uri sourceFileUri) //asdfg
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

            var versionsMetadata = IRegistryModuleMetadataProvider.GetModuleVersions(
                registryModuleMetadataProviders,
                LanguageConstants.BicepPublicMcrRegistry,
                $"{LanguageConstants.BicepPublicMcrPathPrefix}{modulePath}");//asdfg

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
                        registry.Equals(PublicMcrRegistry, StringComparison.Ordinal))
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
                                if (modulePath.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix)) //asdfg
                                {
                                    modulePath = modulePath.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length);
                                    return $"{modulePath}/{subpath}";
                                }
                            }
                            else
                            {
                                if (subpath.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix))
                                {
                                    return subpath.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length); //asdfg
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

        // Handles remote (OCI) module path completions, e.g. br: and br/
        private IEnumerable<CompletionItem> GetOciModuleCompletions(BicepCompletionContext context, string trimmedText, Uri sourceFileUri) //asdfg rename?
        {
            if (!IsOciArtifactRegistryReference(trimmedText))
            {
                return [];
            }

            return [
                .. GetModuleCompletions(trimmedText, context, sourceFileUri),
                .. GetPartialPrivatePathCompletionsFromAliases(trimmedText, context, sourceFileUri),
                .. GetPublicPathCompletionFromAliases(trimmedText, context, sourceFileUri),
            ];
        }








#if false
            if (replacementText == "'br/public:'" ||
                replacementText == $"'br:{PublicMCRRegistry}/bicep/'" ||
                replacementText == "'br/public:" ||
                replacementText == $"'br:{PublicMCRRegistry}/bicep/")
            {
                return await GetPublicMCRPathCompletions(replacementText, context, sourceFileUri);
            }
            else
            {
                List<CompletionItem> completions = new();

                completions.AddRange(GetACRPartialPathCompletionsFromBicepConfig(replacementText, context, sourceFileUri));
                completions.AddRange(await GetMCRPathCompletionFromBicepConfig(replacementText, context, sourceFileUri));

                var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');
                foreach (var kvp in GetOciArtifactModuleAliases(sourceFileUri)/*asdfg non-aliased*/)
                {
                    var registry = kvp.Value.Registry;

                    if (registry is not null && !registry.Equals(PublicMCRRegistry, StringComparison.Ordinal)/*asdfg?*/)
                    {
                    //asdfg2
                        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
                        var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
                        var catalog = await acrManager.GetCatalogAsync(rootConfiguration, registry); //asdfg cache

                        //asdfg?if (!aliases.TryGetValue(label, out _))
                        //{
                        //asdfg
                            var replacementTextWithoutQuotes = replacementText.Trim('\''); // e.g. replacementText = "'br/demo/spaces:"
                        foreach (var module in catalog)
                        {
                            var label = module;
                            var insertText = $"'{replacementTextWithoutQuotes}{module}:$0'";
                            var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                            .WithFilterText(insertText)
                            .WithSnippetEdit(context.ReplacementRange, insertText)
                            //asdfg .WithSortText(GetSortText(registry))
                            .WithFollowupCompletion("private module path completion asdfg")
                            .Build();
                            completions.Add(completionItem);
                        }

                        //asdfg aliases.Add(label );
                        //}
                    }
                }

                return completions;
            }
        }


private async Task<ImmutableArray<string>?> TryGetCatalog(string loginServer)
        {
            Trace.WriteLine($"Retrieving list of public registry modules...");

            try
            {
                var catalogEndpoint = $"https://{loginServer}/v2/_catalog";
                var metadata = await this.httpClient.GetFromJsonAsync<string[]>(catalogEndpoint, JsonSerializerOptions);

                if (metadata is not null)
                {
                    return metadata.ToImmutableArray();
                }
                else
                {
                    throw new Exception($"asdfgList of MCR modules at {LiveDataEndpoint} was empty");
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(string.Format("asdfgError retrieving MCR modules metadata: {0}", e.Message));
                return null;
            }
        }
#endif


        // private async Task<ImmutableArray<string>?> TryGetCatalog(string loginServer)
        // {
        //     Trace.WriteLine($"Retrieving list of public registry modules...");

        //     try
        //     {
        //         var catalogEndpoint = $"https://{loginServer}/v2/_catalog";
        //         var metadata = await this.httpClient.GetFromJsonAsync<string[]>(catalogEndpoint, JsonSerializerOptions);

        //         if (metadata is not null)
        //         {
        //             return metadata.ToImmutableArray();
        //         }
        //         else
        //         {
        //             throw new Exception($"asdfgList of MCR modules at {LiveDataEndpoint} was empty");
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Trace.TraceError(string.Format("asdfgError retrieving MCR modules metadata: {0}", e.Message));
        //         return null;
        //     }
        // }



        // Handles path completions for case where user has specified an alias in bicepconfig.json with registry set to "mcr.microsoft.com".
        private IEnumerable<CompletionItem> GetPublicPathCompletionFromAliases(string trimmedText, BicepCompletionContext context, Uri sourceFileUri) //asdfg rewrite or remove
        {
            List<CompletionItem> completions = new();

            if (IsPrivateRegistryReference(trimmedText, out _))
            {
                return completions;
            }

            foreach (var kvp in GetModuleAliases(sourceFileUri))
            {
                if (kvp.Value.Registry is string inputRegistry)
                {
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
                                var modules = IRegistryModuleMetadataProvider.GetModules(registryModuleMetadataProviders, PublicMcrRegistry);//asdfg testpoint
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
                            var modules = IRegistryModuleMetadataProvider.GetModules(registryModuleMetadataProviders, PublicMcrRegistry); //asdfg testpoint

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

        private (string?, string?) GetFirstMatch(Regex regex, string text, string group1, string group2, bool allowEmpty)
        {
            var matches = regex.Matches(text);
            if (!matches.Any())
            {
                return (null, null);
            }

            var value1 = matches[0].Groups[group1].Value;
            var value2 = matches[0].Groups[group2].Value;

            if (!allowEmpty && (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))) //asdfg?
            {
                return (null, null);
            }

            return (value1, value2);
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

        private bool IsFullPathModuleReference(string text, [NotNullWhen(true)] out string? registry, out string? path)
        {
            registry = null;
            path = null;

            var matches = ModulePrefixWithFullPath.Matches(text);
            if (!matches.Any())
            {
                return false;
            }

            registry = matches[0].Groups["registry"].Value;
            path = matches[0].Groups["module"].Value;

            return true;
        }

        private bool IsAliasedModuleReference(string text, [NotNullWhen(true)] out string? alias, out string? path)
        {
            alias = null;
            path = null;

            var matches = ModulePrefixWithAlias.Matches(text);
            if (!matches.Any())
            {
                return false;
            }

            alias = matches[0].Groups["alias"].Value;
            alias = string.IsNullOrWhiteSpace(alias) ? null : alias;

            path = matches[0].Groups["path"].Value;
            path = string.IsNullOrWhiteSpace(path) ? null : path;

            return alias is not null;
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

        //asdfg make sure sorted by version

        // Handles module path completions for MCR:
        //   br/public:<CURSOR>
        // or
        //   br:mcr.microsoft.com/bicep/:<CURSOR>
        private IEnumerable<CompletionItem> GetModuleCompletions(string trimmedText, BicepCompletionContext context, Uri sourceFileUri)//asdfgasdfgasdfg
        {
            string? alias = null;
            string? inputPath = null;
            string? basePath = null;

            if (!IsFullPathModuleReference(trimmedText, out string? inputRegistry, out inputPath)
                || string.IsNullOrWhiteSpace(inputRegistry))
            {
                if (IsAliasedModuleReference(trimmedText, out alias, out inputPath))
                {
                    var aliases = GetModuleAliases(sourceFileUri);
                    if (aliases.TryGetValue(alias, out var aliasValue))
                    {
                        basePath = aliasValue.ModulePath;
                        inputPath = string.IsNullOrEmpty(aliasValue.ModulePath) ? inputPath : $"{aliasValue.ModulePath}/{inputPath}";
                        inputRegistry = aliasValue.Registry; //adsfg testpoint
                    }
                }

                if (inputPath is null || string.IsNullOrWhiteSpace(inputRegistry))
                {
                    return [];
                }
            }

            //var (prefix, suffix, registry, path, alias) = trimmedText switch //asdfg shouldn't have to have specific code for "br" or PublicMCRRegistry
            //{//asdfg
            //    { } x when x.StartsWith("br/public:", StringComparison.Ordinal) => ("br/public:", x["br/public:".Length..], PublicMCRRegistry, "bicep/" + x["br/public:".Length..], "public"),
            //    { } x when x.StartsWith($"br:{PublicMCRRegistry}/bicep/", StringComparison.Ordinal) => ($"br:{PublicMCRRegistry}/bicep/", x[$"br:{PublicMCRRegistry}/".Length..], PublicMCRRegistry, x[$"br:{PublicMCRRegistry}/bicep/".Length..], null),
            //    _ => (null, null, null, null, null),
            //};

            //if (prefix is null || suffix is null/*asdfg*/)
            //{
            //    if (IsPrivateRegistryReference/*asdfg?*/(trimmedText, out registry))
            //    {
            //        prefix = $"'br:{registry}/";
            //        suffix = trimmedText[prefix.Length..].TrimEnd('\'');
            //    }
            //    else
            //    {
            //        return [];
            //    }
            //}

            List<CompletionItem> completions = new();

            var modules = IRegistryModuleMetadataProvider.GetModules(registryModuleMetadataProviders, inputRegistry); //asdfg2
            foreach (var (registry, moduleName, description, documentationUri) in modules)
            {
                //asdfg remove?
                //if (!moduleName.StartsWith(suffix, StringComparison.Ordinal)) //asdfg case-insensitive
                //{
                //    continue;
                //}

                //if (!moduleName.StartsWith(suffix, StringComparison.Ordinal)) //asdfg case-insensitive
                //{
                //    continue;
                //}

                var isPublicModule = registry.EqualsOrdinally(LanguageConstants.BicepPublicMcrRegistry)
                    && moduleName.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix, StringComparison.InvariantCulture)
                    && (
                        trimmedText.StartsWith($"br:{LanguageConstants.BicepPublicMcrRegistry}/bicep/", StringComparison.Ordinal) //asdfg test when trimmedText.StartsWith fails
                        || trimmedText.StartsWith($"br/public:", StringComparison.Ordinal) //asdfg test when trimmedText.StartsWith fails
                    );

                string insertText;
                if (alias is string)
                {
                    var module = isPublicModule ? moduleName.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length)/*asdfg extract*/ : moduleName;
                    insertText = $"'br/{alias}:{module}:$0'";
                }
                else
                {
                    //asdfg?
                    //if (registry.EqualsOrdinally(LanguageConstants.BicepPublicMcrRegistry)
                    //    && moduleName.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix, StringComparison.InvariantCulture))
                    //{
                    //    insertText = $"'br:{registry}/{moduleName.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length)/*asdfg extract*/}:$0'"; //asdfg
                    //}
                    //else
                    //{
                    insertText = $"'br:{registry}/{moduleName}:$0'"; //asdfg
                    //}
                }

                var label = isPublicModule
                    ? moduleName.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length)/*asdfg extract?*///asdfg?
                    : moduleName;                                                                                                                       //}

                var completionItem = CompletionItemBuilder.Create(
                    CompletionItemKind.Snippet/*asdfg?*/, label)
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
                telemetryProvider.PostEvent(BicepTelemetryEvent.ModuleRegistryPathCompletion(ModuleRegistryType.MCR)); //asdfg useful??  especially if returning all and not filtering
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
                var label = $"{PublicMcrRegistry}/bicep";
                var insertText = $"'{trimmedText}{label}/$0'";
                var mcrCompletionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                    .WithFilterText(insertText)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(label))
                    .WithFollowupCompletion("module repository completion")
                    .Build();

                completions.Add(mcrCompletionItem);

                completions.AddRange(await GetPrivateModuleCompletionsAsdfg(trimmedText, context, sourceFileUri, cancellationToken));
            }

            return completions;
        }

        // Handles registry name completions for private modules possibly available in ACR registries
        private async Task<IEnumerable<CompletionItem>> GetPrivateModuleCompletionsAsdfg(string trimmedText, BicepCompletionContext context, Uri sourceFileUri, CancellationToken cancellationToken)
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
