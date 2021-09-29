// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDisableLinterRuleCommandHandler : ExecuteTypedCommandHandlerBase<DocumentUri, string, string>
    {
        private readonly string DefaultBicepConfig;

        public BicepDisableLinterRuleCommandHandler(ISerializer serializer)
            : base(LanguageConstants.DisableLinterRuleCommandName, serializer)
        {
            DefaultBicepConfig = DefaultBicepConfigHelper.GetDefaultBicepConfig();
        }

        public override async Task<Unit> Handle(DocumentUri documentUri, string code, string bicepConfigFilePath, CancellationToken cancellationToken)
        {
            (string updatedBicepConfigFilePath, string bicepConfigContents) = GetBicepConfigFilePathAndContents(documentUri, code, bicepConfigFilePath);
            File.WriteAllText(updatedBicepConfigFilePath, bicepConfigContents);

            return await Unit.Task;
        }

        public (string, string) GetBicepConfigFilePathAndContents(DocumentUri documentUri, string code, string bicepConfigFilePath)
        {
            if (File.Exists(bicepConfigFilePath))
            {
                return (bicepConfigFilePath, DisableLinterRule(File.ReadAllText(bicepConfigFilePath), code));
            }
            else
            {
                var directoryContainingSourceFile = Path.GetDirectoryName(documentUri.GetFileSystemPath()) ??
                    throw new ArgumentException("Unable to find directory information");

                bicepConfigFilePath = Path.Combine(directoryContainingSourceFile, LanguageConstants.BicepConfigurationFileName);
                return (bicepConfigFilePath, DisableLinterRule(string.Empty, code));
            }
        }

        public string DisableLinterRule(string bicepConfig, string code)
        {
            try
            {
                if (JsonConvert.DeserializeObject(bicepConfig) is JObject root &&
                    root["analyzers"] is JObject analyzers &&
                    analyzers["core"] is JObject core)
                {
                    if (core["rules"] is JObject rules)
                    {
                        if (rules[code] is JObject ruleName)
                        {
                            if (ruleName.ContainsKey("level"))
                            {
                                ruleName["level"] = "off";
                            }
                            else
                            {
                                ruleName.Add("level", "off");
                            }
                        }
                        else
                        {
                            SetRuleLevelToOff(rules, code);
                        }
                    }
                    else
                    {
                        JObject rule = new JObject();
                        SetRuleLevelToOff(rule, code);

                        core.Add("rules", rule);
                    }

                    return root.ToString(Formatting.Indented);
                }

                if (JsonConvert.DeserializeObject(DefaultBicepConfig) is JObject defaultBicepConfigRoot &&
                    defaultBicepConfigRoot["analyzers"]?["core"]?["rules"] is JObject defaultRules)
                {
                    SetRuleLevelToOff(defaultRules, code);

                    return defaultBicepConfigRoot.ToString();
                }

                return string.Empty;
            }
            catch (Exception)
            {
                throw new Exception("File bicepconfig.json already exists and is invalid. If overwriting the file is intended, delete it manually and retry disable linter rule lightBulb option again");
            }
        }

        private void SetRuleLevelToOff(JObject jObject, string code)
        {
            jObject.Add(code, JToken.Parse(@"{
  ""level"": ""off""
}"));
        }
    }
}
