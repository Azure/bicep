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
    [Method("bicep/getConfiguration", Direction.ClientToServer)]
    public record BicepGetConfigParams(string BicepOrConfigPath) : IRequest<BicepGetConfigResult>;

    public record BicepGetConfigResult(string? configPath, string effectiveConfig, string linterState);


    public class BicepGetConfigHandler(
        IBicepAnalyzer bicepAnalyzer,
        IConfigurationManager configurationManager
    ) : IJsonRpcRequestHandler<BicepGetConfigParams, BicepGetConfigResult>
    {
        public Task<BicepGetConfigResult> Handle(BicepGetConfigParams request, CancellationToken cancellationToken)
        {
            var documentUri = new Uri(request.BicepOrConfigPath);
            RootConfiguration configuration = configurationManager.GetConfiguration(documentUri);
            IOUri? configFilePath = configuration.ConfigFileUri;
            var configJson = configuration.ToUtf8Json();

            var linterState = GetEffectiveAnalyzerConfig(bicepAnalyzer, configuration.Analyzers);

            return Task.FromResult(new BicepGetConfigResult(configFilePath?.GetLocalFilePath(), configJson, linterState));
        }

        private string GetEffectiveAnalyzerConfig(IBicepAnalyzer analyzer, AnalyzersConfiguration configuration)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"  Analyzer enabled: {analyzer.IsEnabled(configuration)}");
            var sbEnabledRules = new StringBuilder();
            var sbDisabledRules = new StringBuilder();

            var rules = analyzer.GetRuleSet();
            foreach (var rule in rules)
            {
                var level = rule.GetDiagnosticLevel(configuration);
                var isDefaultString = rule.DefaultDiagnosticLevel == level ? " (default)" : string.Empty;
                var isEnabled = level != DiagnosticLevel.Off;

                if (isEnabled)
                {
                    sbEnabledRules.AppendLine($"    {rule.Code}: {level}{isDefaultString}");
                }
                else
                {
                    sbDisabledRules.AppendLine($"    {rule.Code}: {level}{isDefaultString}");
                }
            }

            return $"""
                    {sb.ToString()}
                      Enabled rules:
                    {sbEnabledRules.ToString()}
                      Disabled rules:
                    {sbDisabledRules.ToString()}
                    """;
        }
    }
}
