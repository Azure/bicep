// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDisableLinterRuleHandler : ExecuteTypedCommandHandlerBase<string, string>
    {
        private readonly string DefaultBicepConfig;

        public BicepDisableLinterRuleHandler(ISerializer serializer)
            : base(LanguageConstants.DisableLinterRuleCommandName, serializer)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string manifestResourceName = assembly.GetManifestResourceNames().Where(p => p.EndsWith(ConfigHelper.SettingsFileName, StringComparison.Ordinal)).First();
            Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
            var streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            DefaultBicepConfig = streamReader.ReadToEnd();
        }

        public override async Task<Unit> Handle(string code, string directory, CancellationToken cancellationToken)
        {
            string configFilePath = Path.Combine(directory, ConfigHelper.SettingsFileName);
            string updatedBicepConfig;

            if (File.Exists(configFilePath))
            {
                updatedBicepConfig = DisableLinterRule(File.ReadAllText(configFilePath), code);
            }
            else
            {
                updatedBicepConfig = DisableLinterRule(DefaultBicepConfig, code);
            }

            File.WriteAllText(configFilePath, updatedBicepConfig);

            return await Unit.Task;
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

                    return root.ToString();
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
}").ToString();
                }
            }
            catch (Exception)
            {
                throw new Exception("File bicepconfig.json already exists and is invalid. If overwriting the file is intended, delete it manually and retry disable linter rule lightBulb option again");
            }
        }
    }
}
