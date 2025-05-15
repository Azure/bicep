// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using System.Text;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using MediatR;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/getConfigInfo", Direction.ClientToServer)]
    public record BicepGetConfigInfoParams(string BicepOrConfigPath) : IRequest<BicepGetConfigInfoResult>;

    public record BicepGetConfigInfoResult(string? ConfigPath, string EffectiveConfig, string LinterState);


    public class BicepGetConfigInfoHandler(
        IBicepAnalyzer bicepAnalyzer,
        IConfigurationManager configurationManager
    ) : IJsonRpcRequestHandler<BicepGetConfigInfoParams, BicepGetConfigInfoResult>
    {
        public Task<BicepGetConfigInfoResult> Handle(BicepGetConfigInfoParams request, CancellationToken cancellationToken)
        {
            var documentUri = new Uri(request.BicepOrConfigPath);
            configurationManager.PurgeCache();
            RootConfiguration configuration = configurationManager.GetConfiguration(documentUri);
            IOUri? configFilePath = configuration.ConfigFileUri;
            var configJson = configuration.ToUtf8Json();

            var linterState = GetAnalyzerRulesStates(bicepAnalyzer, configuration.Analyzers);

            return Task.FromResult(new BicepGetConfigInfoResult(configFilePath?.GetLocalFilePath(), configJson, linterState));
        }

        private string GetAnalyzerRulesStates(IBicepAnalyzer analyzer, AnalyzersConfiguration configuration)
        {
            var sb = new StringBuilder();
            var analyzerEnabled = analyzer.IsEnabled(configuration);
            sb.AppendLine($"Analyzer enabled: {analyzerEnabled}");
            var sbEnabledRules = new StringBuilder();
            var sbDisabledRules = new StringBuilder();

            var rules = analyzer.GetRuleSet();
            foreach (var rule in rules)
            {
                var level = rule.GetDiagnosticLevel(configuration);
                var isDefaultString = rule.DefaultDiagnosticLevel == level ? " (default)" : string.Empty;
                var isEnabled = level != DiagnosticLevel.Off;

                if (!analyzerEnabled)
                {
                    sbDisabledRules.AppendLine($"{rule.Code}: Off (analyzer disabled)");
                }
                else if (isEnabled)
                {
                    sbEnabledRules.AppendLine($"{rule.Code}: {level}{isDefaultString}");
                }
                else
                {
                    sbDisabledRules.AppendLine($"{rule.Code}: {level}{isDefaultString}");
                }
            }

            return $"""
                {sb.ToString()}
                Enabled rules:
                {sbEnabledRules.ToString().IndentLines(2)}
                Disabled rules:
                {sbDisabledRules.ToString().IndentLines(2)}
                """;
        }
    }
}
