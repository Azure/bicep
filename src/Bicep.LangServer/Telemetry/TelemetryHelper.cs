// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryHelper
    {
        public const string LinterEnabledSetting = "core.enabled";

        public static void SendTelemetryOnBicepConfigChange(RootConfiguration prevConfiguration, RootConfiguration curConfiguration, ILinterRulesProvider linterRulesProvider, ITelemetryProvider telemetryProvider)
        {
            foreach (var telemetryEvent in GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, linterRulesProvider))
            {
                telemetryProvider.PostEvent(telemetryEvent);
            }
        }

        public static IEnumerable<BicepTelemetryEvent> GetTelemetryEventsForBicepConfigChange(RootConfiguration prevConfiguration, RootConfiguration curConfiguration, ILinterRulesProvider linterRulesProvider)
        {
            bool prevLinterEnabledSettingValue = prevConfiguration.Analyzers.GetValue(LinterEnabledSetting, true);
            bool curLinterEnabledSettingValue = curConfiguration.Analyzers.GetValue(LinterEnabledSetting, true);

            if (!prevLinterEnabledSettingValue && !curLinterEnabledSettingValue)
            {
                return Enumerable.Empty<BicepTelemetryEvent>();
            }

            List<BicepTelemetryEvent> telemetryEvents = new();

            if (prevLinterEnabledSettingValue != curLinterEnabledSettingValue)
            {
                var telemetryEvent = BicepTelemetryEvent.CreateOverallLinterStateChangeInBicepConfig(prevLinterEnabledSettingValue.ToString().ToLowerInvariant(), curLinterEnabledSettingValue.ToString().ToLowerInvariant());
                telemetryEvents.Add(telemetryEvent);
            }
            else
            {
                foreach (var kvp in linterRulesProvider.GetLinterRules())
                {
                    string prevLinterRuleDiagnosticLevelValue = prevConfiguration.Analyzers.GetValue(kvp.Value, "warning");
                    string curLinterRuleDiagnosticLevelValue = curConfiguration.Analyzers.GetValue(kvp.Value, "warning");

                    if (prevLinterRuleDiagnosticLevelValue != curLinterRuleDiagnosticLevelValue)
                    {
                        var telemetryEvent = BicepTelemetryEvent.CreateLinterRuleStateChangeInBicepConfig(kvp.Key, prevLinterRuleDiagnosticLevelValue, curLinterRuleDiagnosticLevelValue);
                        telemetryEvents.Add(telemetryEvent);
                    }
                }
            }

            return telemetryEvents;
        }

        // Per LSP spec - https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#command
        // title and name are required
        public static Command CreateCommand(string title, string name, JArray? args)
        {
            return new Command()
            {
                Title = title,
                Name = name,
                Arguments = args
            };
        }
    }
}
