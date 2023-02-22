// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IAzureContainerRegistryNamesProvider azureContainerRegistryNamesProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly IModulesMetadataProvider modulesMetadataProvider;
        private readonly ISettingsProvider settingsProvider;

        private static readonly Dictionary<string, string> BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails = new Dictionary<string, string>()
        {
            {"br:", "Bicep registry schema name" },
            {"br/", "Bicep registry schema name" },
            {"ts:", "Template spec schema name" },
            {"ts/", "Template spec schema name" },
        };

        private static readonly Regex AcrPublicModuleRegistryAlias = new Regex(@"br:(?<registry>(.*).azurecr.io)/", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex McrPublicModuleRegistryAliasWithPath = new Regex(@"br/public:(?<filePath>(.*?)):", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex McrPublicModuleRegistryWithoutAliasWithPath = new Regex(@"br:mcr.microsoft.com/bicep/(?<filePath>(.*?)):", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        public ModuleReferenceCompletionProvider(
            IAzureContainerRegistryNamesProvider azureContainerRegistryNamesProvider,
            IConfigurationManager configurationManager,
            IModulesMetadataProvider modulesMetadataProvider,
            ISettingsProvider settingsProvider)
        {
            this.azureContainerRegistryNamesProvider = azureContainerRegistryNamesProvider;
            this.configurationManager = configurationManager;
            this.modulesMetadataProvider = modulesMetadataProvider;
            this.settingsProvider = settingsProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Uri templateUri, BicepCompletionContext context)
        {
            var replacementText = string.Empty;

            if (context.ReplacementTarget is Token token)
            {
                replacementText = token.Text;
            }

            return GetPathCompletions(context, replacementText, templateUri)
                .Concat(GetMcrModuleRegistryVersionCompletions(context, replacementText))
                .Concat(await GetRegistryCompletions(context, replacementText, templateUri))
                .Concat(GetBicepRegistryAndTemplateSpecShemaCompletions(context, replacementText));
        }

        private IEnumerable<CompletionItem> GetBicepRegistryAndTemplateSpecShemaCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (!string.IsNullOrWhiteSpace(replacementText.Trim('\'')))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completionItems = new List<CompletionItem>();
            foreach (var kvp in BicepRegistryAndTemplateSpecShemaCompletionLabelsWithDetails)
            {
                var text = kvp.Key;
                var sb = new StringBuilder();
                sb.Append("'");
                sb.Append(text);
                sb.Append("$0");
                sb.Append("'");

                var completionText = sb.ToString();

                BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateBicepRegistryOrTemplateSpecShemaCompletion(text);
                var command = TelemetryHelper.CreateCommand
                (
                    title: "Bicep registry or template spec shema completion",
                    name: TelemetryConstants.CommandName,
                    args: JArray.FromObject(new List<object> { telemetryEvent })
                );

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, text)
                    .WithFilterText(completionText)
                    .WithSortText(GetSortText(text, CompletionPriority.Medium))
                    .WithSnippetEdit(context.ReplacementRange, completionText)
                    .WithDetail(kvp.Value)
                    .WithCommand(command)
                    .Build();
                completionItems.Add(completionItem);
            }

            return completionItems;
        }

        private IEnumerable<CompletionItem> GetMcrModuleRegistryVersionCompletions(BicepCompletionContext context, string replacementText)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            string? filePath = null;

            if (McrPublicModuleRegistryAliasWithPath.IsMatch(replacementText))
            {
                var matches = McrPublicModuleRegistryAliasWithPath.Matches(replacementText);
                filePath = matches[0].Groups["filePath"].Value;
            }
            if (McrPublicModuleRegistryWithoutAliasWithPath.IsMatch(replacementText))
            {
                var matches = McrPublicModuleRegistryWithoutAliasWithPath.Matches(replacementText);
                filePath = matches[0].Groups["filePath"].Value;
            }

            if (filePath is null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            List<CompletionItem> completions = new List<CompletionItem>();
            replacementText = replacementText.TrimEnd('\'');
            foreach (var version in modulesMetadataProvider.GetVersions(filePath))
            {
                StringBuilder sb = new StringBuilder(replacementText);
                sb.Append(version);
                sb.Append("'$0");

                var insertText = sb.ToString();

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, version)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(version, CompletionPriority.High))
                    .Build();

                completions.Add(completionItem);
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetPathCompletions(BicepCompletionContext context, string replacementText, Uri templateUri)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (replacementText == "'br/public:'" ||
                replacementText == "'br:mcr.microsoft.com/bicep/'" ||
                replacementText == "'br/public:" ||
                replacementText == "'br:mcr.microsoft.com/bicep/")
            {
                return GetMcrPathCompletions(replacementText, context);
            }
            else
            {
                List<CompletionItem> completions = new List<CompletionItem>();

                completions.AddRange(GetAcrPartialPathCompletionsFromBicepConfig(replacementText, context, templateUri));
                completions.AddRange(GetMCRPathCompletionFromBicepConfig(replacementText, context, templateUri));

                return completions;
            }
        }

        // Case where user has specified an alias in bicepconfig.json with registry set to "mcr.microsoft.com".
        private IEnumerable<CompletionItem> GetMCRPathCompletionFromBicepConfig(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            if (AcrPublicModuleRegistryAlias.IsMatch(replacementTextWithTrimmedEnd))
            {
                return completions;
            }

            var rootConfiguration = configurationManager.GetConfiguration(templateUri);
            var ociArtifactModuleAliases = rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();

            foreach (var kvp in ociArtifactModuleAliases)
            {
                if (kvp.Value.Registry is string registry &&
                    registry.Equals("mcr.microsoft.com", StringComparison.Ordinal) &&
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
                        if (replacementTextWithTrimmedEnd.Equals($"'br/{kvp.Key}/", StringComparison.Ordinal))
                        {
                            return GetMcrPathCompletions(replacementText, context);
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

                        var modulePathWithoutBicepKeyword = modulePath.Substring("bicep/".Length);
                        var matchingModuleNames = modulesMetadataProvider.GetModuleNames().Where(x => x.StartsWith($"{modulePathWithoutBicepKeyword}/"));

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
                                .WithSortText(GetSortText(label, CompletionPriority.High))
                                .Build();
                            completions.Add(completionItem);
                        }
                    }
                }
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetAcrPartialPathCompletionsFromBicepConfig(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');
            if (!AcrPublicModuleRegistryAlias.IsMatch(replacementTextWithTrimmedEnd))
            {
                return completions;
            }

            var matches = AcrPublicModuleRegistryAlias.Matches(replacementTextWithTrimmedEnd);

            if (!matches.Any())
            {
                return completions;
            }

            var registry = matches[0].Groups["registry"].Value;

            if (registry is null)
            {
                return completions;
            }
            var rootConfiguration = configurationManager.GetConfiguration(templateUri);
            var ociArtifactModuleAliases = rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();

            foreach (var kvp in ociArtifactModuleAliases)
            {
                if (registry.Equals(kvp.Value.Registry, StringComparison.Ordinal))
                {
                    var modulePath = kvp.Value.ModulePath;

                    if (modulePath is null)
                    {
                        continue;
                    }

                    StringBuilder sb = new StringBuilder(replacementTextWithTrimmedEnd);
                    sb.Append(modulePath);
                    sb.Append(":$0'");

                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, modulePath)
                        .WithSnippetEdit(context.ReplacementRange, sb.ToString())
                        .WithFilterText(modulePath)
                        .WithSortText(GetSortText(modulePath, CompletionPriority.High))
                        .Build();
                    completions.Add(completionItem);
                }
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetMcrPathCompletions(string replacementText, BicepCompletionContext context)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            foreach (var moduleName in modulesMetadataProvider.GetModuleNames())
            {
                StringBuilder sb = new StringBuilder(replacementText.TrimEnd('\''));
                sb.Append(moduleName);
                sb.Append(":$0'");

                var insertText = sb.ToString();

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, moduleName)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithFilterText(insertText)
                    .WithSortText(GetSortText(moduleName, CompletionPriority.High))
                    .Build();

                completions.Add(completionItem);
            }

            return completions;
        }

        private async Task<IEnumerable<CompletionItem>> GetRegistryCompletions(BicepCompletionContext context, string replacementText, Uri templateUri)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OciModuleRegistryReference))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var replacementTextWithTrimmedEnd = replacementText.TrimEnd('\'');

            var completions = new List<CompletionItem>();
            if (replacementTextWithTrimmedEnd == "'br/")
            {
                var rootConfiguration = configurationManager.GetConfiguration(templateUri);
                var ociArtifactModuleAliases = rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();

                foreach (var kvp in ociArtifactModuleAliases)
                {
                    var alias = kvp.Key;
                    var insertText = $"{replacementTextWithTrimmedEnd}{alias}:$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, alias)
                        .WithFilterText(insertText)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithSortText(GetSortText(alias, CompletionPriority.High))
                        .Build();
                    completions.Add(completionItem);
                }
            }
            else if (replacementTextWithTrimmedEnd == "'br:")
            {
                var label = "mcr.microsoft.com/bicep/";
                var insertText = $"{replacementTextWithTrimmedEnd}{label}$0'";
                var mcrCompletionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                    .WithFilterText(insertText)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(label, CompletionPriority.High))
                    .Build();

                completions.Add(mcrCompletionItem);

                IEnumerable<CompletionItem> acrCompletions = await GetAcrModuleRegistriesCompletions(replacementText, context, templateUri);
                completions.AddRange(acrCompletions);
            }

            return completions;
        }

        private async Task<IEnumerable<CompletionItem>> GetAcrModuleRegistriesCompletions(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            if (settingsProvider.GetSetting(LangServerConstants.IncludeAllAccessibleAzureContainerRegistriesForCompletionsSetting))
            {
                return await GetAcrModuleRegistriesCompletionsFromGraphClient(replacementText, context, templateUri);
            }
            else
            {
                return GetAcrModuleRegistriesCompletionsFromBicepConfig(replacementText, context, templateUri);
            }
        }

        private async Task<IEnumerable<CompletionItem>> GetAcrModuleRegistriesCompletionsFromGraphClient(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var registryNames = await azureContainerRegistryNamesProvider.GetRegistryNames(templateUri);

            foreach (string registryName in registryNames)
            {
                var replacementTextWithTrimmedEnd = replacementText.Trim('\'');
                var insertText = $"{replacementTextWithTrimmedEnd}{registryName}/$0'";

                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, registryName)
                    .WithFilterText(registryName)
                    .WithSnippetEdit(context.ReplacementRange, insertText)
                    .WithSortText(GetSortText(registryName, CompletionPriority.High))
                    .Build();
                completions.Add(completionItem);
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetAcrModuleRegistriesCompletionsFromBicepConfig(string replacementText, BicepCompletionContext context, Uri templateUri)
        {
            List<CompletionItem> completions = new List<CompletionItem>();

            var rootConfiguration = configurationManager.GetConfiguration(templateUri);
            var ociArtifactModuleAliases = rootConfiguration.ModuleAliases.GetOciArtifactModuleAliases();

            foreach (var kvp in ociArtifactModuleAliases)
            {
                var label = kvp.Value.Registry;

                if (label is not null && !label.Equals("mcr.microsoft.com", StringComparison.Ordinal))
                {
                    var replacementTextWithTrimmedEnd = replacementText.Trim('\'');
                    var insertText = $"{replacementTextWithTrimmedEnd}{label}/$0'";
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                        .WithFilterText(insertText)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithSortText(GetSortText(label, CompletionPriority.High))
                        .Build();
                    completions.Add(completionItem);
                }
            }

            return completions;
        }

        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";
    }
}
