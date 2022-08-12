// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    /// <summary>
    /// Handles the internal command for code actions to edit a particular linter rule in the bicepconfig.json file
    /// </summary>
    /// <remarks>
    /// Using ExecuteTypedResponseCommandHandlerBase instead of IJsonRpcRequestHandler because IJsonRpcRequestHandler will throw "Content modified" if text changes are detected, and for this command
    /// that is expected.
    /// </remarks>
    public class BicepEditLinterRuleCommandHandler : ExecuteTypedCommandHandlerBase<DocumentUri, string, string>
    {
        private readonly string DefaultBicepConfig;
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly ILanguageServerFacade server;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepEditLinterRuleCommandHandler(ISerializer serializer, ILanguageServerFacade server, IClientCapabilitiesProvider clientCapabilitiesProvider, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.EditLinterRuleCommandName, serializer)
        {
            DefaultBicepConfig = DefaultBicepConfigHelper.GetDefaultBicepConfig();
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.server = server;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<Unit> Handle(DocumentUri documentUri, string ruleCode, string bicepConfigFilePath, CancellationToken cancellationToken)
        {
            string? error = "unknown";
            bool newConfigFile = false;
            bool newRuleAdded = false;
            try
            {
                // bicepConfigFilePath will be empty string if no current configuration file was found
                if (string.IsNullOrEmpty(bicepConfigFilePath))
                {
                    // There is no configuration file currently - create one in the default location
                    var targetFolder = await BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(this.server, this.clientCapabilitiesProvider, documentUri.GetFileSystemPath());
                    bicepConfigFilePath = Path.Combine(targetFolder, LanguageConstants.BicepConfigurationFileName);
                }

                try
                {
                    if (!File.Exists(bicepConfigFilePath))
                    {
                        newConfigFile = true;
                        File.WriteAllText(bicepConfigFilePath, DefaultBicepConfig);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.GetType().Name;
                    server.Window.ShowError($"Unable to create configuration file \"{bicepConfigFilePath}\": {ex.Message}");
                    return Unit.Value;
                }

                newRuleAdded = await AddAndSelectRuleLevel(server, clientCapabilitiesProvider, bicepConfigFilePath, ruleCode);

                error = null;
                return Unit.Value;
            }
            catch (Exception ex)
            {
                error = ex.GetType().Name;
                server.Window.ShowError(ex.Message);
                return Unit.Value;
            }
            finally
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.EditLinterRule(ruleCode, newConfigFile, newRuleAdded, error));
            }
        }

        // Returns true if the rule was added to the config file
        public static async Task<bool> AddAndSelectRuleLevel(ILanguageServerFacade server, IClientCapabilitiesProvider clientCapabilitiesProvider, string bicepConfigFilePath, string ruleCode)
        {
            if (await SelectRuleLevelIfExists(server, clientCapabilitiesProvider, ruleCode, bicepConfigFilePath))
            {
                // The rule already exists and has been shown/selected
                return false;
            }

            string json = File.ReadAllText(bicepConfigFilePath);
            (int line, int column, string text)? insertion = new JsonEditor(json).InsertIfNotExist(
                new string[] { "analyzers", "core", "rules", ruleCode, "level" },
                "warning");

            bool added = false;
            if (insertion.HasValue)
            {
                var (line, column, insertText) = insertion.Value;
                try
                {
                    File.WriteAllText(bicepConfigFilePath, JsonEditor.ApplyInsertion(json, (line, column, insertText)));
                    added = true;
                }
                catch (Exception ex)
                {
                    server.Window.ShowError($"Unable to write to configuration file \"{bicepConfigFilePath}\": {ex.Message}");
                }
            }

            await SelectRuleLevelIfExists(server, clientCapabilitiesProvider, ruleCode, bicepConfigFilePath);
            return added;
        }

        /// <summary>
        /// If the given rule has an entry for its error level in the configuration file, show that file and select the current
        /// level value (so that the end user can immediatey edit it).
        /// </summary>
        /// <returns>True if the rule exists and displaying/highlighting succeeds, otherwise false.</returns>
        private static async Task<bool> SelectRuleLevelIfExists(ILanguageServerFacade server, IClientCapabilitiesProvider clientCapabilitiesProvider, string ruleCode, string configFilePath)
        {
            // Inspect the JSON to figure out the location of the rule's level value
            Range? rangeOfRuleLevelValue = FindRangeOfPropertyValueInJson($"analyzers.core.rules.{ruleCode}.level", configFilePath);
            if (rangeOfRuleLevelValue is not null)
            {
                if (clientCapabilitiesProvider.DoesClientSupportShowDocumentRequest())
                {
                    // Show the document first and allow the dust to settle
                    await server.Window.ShowDocument(new()
                    {
                        Uri = DocumentUri.File(configFilePath),
                    });

                    // Now show the document with our desired selection
                    await server.Window.ShowDocument(new()
                    {
                        Uri = DocumentUri.File(configFilePath),
                        // Put the selection at the beginning of the rule's "level" value ("warning, "off", etc.),
                        //  don't select the entire string, or else editor completion will not show all possible values
                        //  when we trigger it.
                        Selection = new Range(rangeOfRuleLevelValue.Start, rangeOfRuleLevelValue.Start),
                        TakeFocus = true
                    });

                    // If the client supports it, trigger completion of the rule's level value (call is ignored
                    //   if the client doesn't know about it)
                    server.SendNotification("bicep/triggerEditorCompletion");
                }

                return true;
            }

            return false;
        }

        private static Range? FindRangeOfPropertyValueInJson(string propertyPath, string jsonPath)
        {
            using TextReader textReader = File.OpenText(jsonPath);
            using JsonReader jsonReader = new JsonTextReader(textReader);

            var jObject = JObject.Load(jsonReader, new JsonLoadSettings
            {
                LineInfoHandling = LineInfoHandling.Load,
                CommentHandling = CommentHandling.Load
            });

            // Search for the given property path
            JToken? jToken = jObject?.SelectToken(propertyPath);
            if (jObject is not null && jToken is not null)
            {
                var lineInfo = jToken as IJsonLineInfo; // 1-indexed

                int line = lineInfo.LineNumber - 1;
                int column = lineInfo.LinePosition - 1 - jToken.ToString().Length;
                int length = jToken.ToString().Length;
                return new Range(line, column, line, column + length);
            }
            else
            {
                return null;
            }
        }
    }
}
