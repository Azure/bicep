// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class ProtectCommandToExecuteSecretsRule : LinterRuleBase
    {
        public new const string Code = "protect-commandtoexecute-secrets";

        private static readonly ImmutableArray<(string publisher, string type)> _publisherAndNameList = ImmutableArray.Create<(string publisher, string type)>(
            // NOTE: This list was obtained by running "az vm extension image list"
            ("Microsoft.Azure.Extensions", "CustomScript"),
            ("Microsoft.Compute", "CustomScriptExtension"),
            ("Microsoft.OSTCExtensions", "CustomScriptForLinux")
        );

        public static ImmutableArray<(string publisher, string type)> PublisherAndNameList => _publisherAndNameList;

        public ProtectCommandToExecuteSecretsRule() : base(
            code: Code,
            description: CoreResources.ProtectCommandToExecuteSecretsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}")
        )
        { }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.ProtectCommandToExecuteSecretsRuleMessage, (string)values[0]);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel semanticModel)
        {
            var diagnosticLevel = GetDiagnosticLevel(semanticModel);
            List<IDiagnostic> diagnostics = new();

            foreach (var resource in semanticModel.DeclaredResources.Where(r => r.IsAzResource))
            {
                // We're looking for this pattern:
                //
                // resource xxx 'xxx/extensions@yyy' = { // right now, these properties apply to Microsoft.{Hybrid,}Compute/virtualMachines/extensions, but it could be extended
                //   name: 'xxx'
                //   properties: {
                //     publisher: 'Microsoft.Compute'  << === matches entry in PublisherAndNameList
                //     type: 'CustomScriptExtension' <<=== matches entry in PublisherAndNameList
                //     autoUpgradeMinorVersion: true
                //     settings: {  <<== commandToExecute under "settings" instead of "protectedSettings"
                //       fileUris: split(fileUris, ' ')
                //       commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}'  <<== required
                //     }
                //   }
                // }
                //
                // This will be a test failure if commandToExecute contains possible secrets

                var resourceTypeName = resource.TypeReference.FormatType();
                if (resourceTypeName.EndsWith("/extensions", LanguageConstants.ResourceTypeComparison))
                {
                    ResourceDeclarationSyntax resourceSyntax = resource.Symbol.DeclaringResource;
                    if (resourceSyntax.TryGetBody()?.TryGetPropertyByName("properties") is ObjectPropertySyntax propertiesSyntax
                        && propertiesSyntax.Value is ObjectSyntax properties)
                    {
                        // Only of interest to us if it contains a "settings" object with "commandToExecute" property
                        if (properties.TryGetPropertyByNameRecursive("settings", "commandToExecute") is ObjectPropertySyntax commandToExecuteSyntax)
                        {
                            // Check if the publisher/type match an entry in PublisherAndNameList
                            if (properties.TryGetPropertyByName("type")?.Value is StringSyntax extensionTypeSyntax
                                && extensionTypeSyntax.TryGetLiteralValue() is string extensionTypeValue
                                && properties.TryGetPropertyByName("publisher")?.Value is StringSyntax publisherSyntax
                                && publisherSyntax.TryGetLiteralValue() is string publisherValue
                                )
                            {
                                bool matches = PublisherAndNameList.Any(e =>
                                StringComparer.OrdinalIgnoreCase.Equals(e.publisher, publisherValue)
                                && StringComparer.OrdinalIgnoreCase.Equals(e.type, extensionTypeValue));
                                if (matches)
                                {
                                    // Does it contain any possible secrets?
                                    var secrets = FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(semanticModel, commandToExecuteSyntax.Value);
                                    if (secrets.Any())
                                    {
                                        diagnostics.Add(CreateDiagnosticForSpan(diagnosticLevel, commandToExecuteSyntax.Key.Span, secrets[0].FoundMessage));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return diagnostics;
        }
    }
}
