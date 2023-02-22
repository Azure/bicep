// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Azure;
using Bicep.Core.Configuration;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Azure.Management.ResourceGraph;
using System.Threading.Tasks;
using Bicep.LanguageServer.Settings;
using System.Reflection.Emit;

namespace Bicep.LanguageServer.Completions
{
    public class ModuleReferenceCompletionProvider : IModuleReferenceCompletionProvider
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IModulesMetadataProvider modulesMetadataProvider;
        private readonly IServiceClientCredentialsProvider serviceClientCredentialsProvider;
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
            IConfigurationManager configurationManager,
            IModulesMetadataProvider modulesMetadataProvider,
            IServiceClientCredentialsProvider serviceClientCredentialsProvider,
            ISettingsProvider settingsProvider)
        {
            this.configurationManager = configurationManager;
            this.modulesMetadataProvider = modulesMetadataProvider;
            this.serviceClientCredentialsProvider = serviceClientCredentialsProvider;
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
                .Concat(GetPublicMcrModuleRegistryVersionCompletions(context, replacementText))
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

        private IEnumerable<CompletionItem> GetPublicMcrModuleRegistryVersionCompletions(BicepCompletionContext context, string replacementText)
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

            List<CompletionItem> completions = new List<CompletionItem>();

            if (replacementText == "'br/public:'" ||
                replacementText == "'br:mcr.microsoft.com/bicep/'" ||
                replacementText == "'br/public:" ||
                replacementText == "'br:mcr.microsoft.com/bicep/")
            {
                completions.AddRange(GetMcrPathCompletions(replacementText, context));
            }
            else
            {
                completions.AddRange(GetAcrPathCompletions(replacementText, context, templateUri));
            }

            return completions;
        }

        private IEnumerable<CompletionItem> GetAcrPathCompletions(string replacementText, BicepCompletionContext context, Uri templateUri)
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
                    sb.Append("/$0'");

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

        private async Task<IEnumerable<CompletionItem>> GetAcrModuleRegistriesCompletions(string replacementText,BicepCompletionContext context,  Uri templateUri)
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

            ClientCredentials clientCredentials = await serviceClientCredentialsProvider.GetServiceClientCredentials(templateUri); ;

            ResourceGraphClient resourceGraphClient = new ResourceGraphClient(clientCredentials);
            QueryRequest queryRequest = new QueryRequest(@"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]
");
            QueryResponse queryResponse = resourceGraphClient.Resources(queryRequest);
            JArray jArray = JArray.FromObject(queryResponse.Data);

            foreach (JObject item in jArray)
            {
                if (item is not null &&
                    item.GetValue("properties_loginServer") is JToken jToken &&
                    jToken is not null &&
                    jToken.Value<string>() is string loginServer)
                {
                    var replacementTextWithTrimmedEnd = replacementText.Trim('\'');
                    var insertText = $"{replacementTextWithTrimmedEnd}{loginServer}/$0'";

                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Snippet, loginServer)
                        .WithFilterText(loginServer)
                        .WithSnippetEdit(context.ReplacementRange, insertText)
                        .WithSortText(GetSortText(loginServer, CompletionPriority.High))
                        .Build();
                    completions.Add(completionItem);
                }
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
