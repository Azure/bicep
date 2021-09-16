// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDisableLinterRuleHandler : ExecuteTypedCommandHandlerBase<DocumentUri, string, string>
    {
        private const string bicepConfigResourceName = "Bicep.LanguageServer.bicepconfig.json";

        private readonly string DefaultBicepConfig;

        public BicepDisableLinterRuleHandler(ISerializer serializer)
            : base(LanguageConstants.DisableLinterRuleCommandName, serializer)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(bicepConfigResourceName);
            var streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            DefaultBicepConfig = streamReader.ReadToEnd();
        }

        public override async Task<Unit> Handle(DocumentUri documentUri, string code, string bicepConfigSettingsFilePath, CancellationToken cancellationToken)
        {
            (string updatedBicepConfigFilePath, string bicepConfigContents) = GetBicepConfigSettingsFilePathAndContents(documentUri, code, bicepConfigSettingsFilePath);

            File.WriteAllText(updatedBicepConfigFilePath, bicepConfigContents);

            return await Unit.Task;
        }

        public (string, string) GetBicepConfigSettingsFilePathAndContents(DocumentUri documentUri, string code, string bicepConfigSettingsFilePath)
        {
            if (File.Exists(bicepConfigSettingsFilePath))
            {
                return (bicepConfigSettingsFilePath, DisableLinterRule(File.ReadAllText(bicepConfigSettingsFilePath), code));
            }
            else
            {
                var directoryContainingSourceFile = Path.GetDirectoryName(documentUri.GetFileSystemPath());

                if (string.IsNullOrWhiteSpace(directoryContainingSourceFile))
                {
                    throw new ArgumentException("Unable to find directory information");
                }

                string updatedBicepConfigSettingsFilePath = Path.Combine(directoryContainingSourceFile, LanguageConstants.BicepConfigSettingsFileName);

                return (updatedBicepConfigSettingsFilePath, DisableLinterRule(DefaultBicepConfig, code));
            }
        }

        public string DisableLinterRule(string bicepConfig, string code)
        {
            try
            {
                if (JsonConvert.DeserializeObject(bicepConfig) is JObject root &&
                    root["analyzers"] is JObject analyzers &&
                    analyzers["core"] is JObject core &&
                    core["rules"] is JObject rules)
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
                        rules.Add(code, JToken.Parse(@"{
  ""level"": ""off""
}"));
                    }

                    return root.ToString(Formatting.Indented);
                }
                else
                {
                    return JObject.Parse(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        " + code + @": {
          ""level"": ""off""
        }
      }
    }
  }
}").ToString(Formatting.Indented);
                }
            }
            catch (Exception)
            {
                throw new Exception("File bicepconfig.json already exists and is invalid. If overwriting the file is intended, delete it manually and retry disable linter rule lightBulb option again");
            }
        }
    }
}
