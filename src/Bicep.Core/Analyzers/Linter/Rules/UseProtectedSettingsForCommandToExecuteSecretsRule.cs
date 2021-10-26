// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseProtectedSettingsForCommandToExecuteSecretsRule : LinterRuleBase
    {
        public new const string Code = "use-protectedsettings-for-commandtoexecute-secrets";

        public UseProtectedSettingsForCommandToExecuteSecretsRule() : base(
            code: Code,
            description: CoreResources.UseProtectedSettingsForCommandToExecuteSecretsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}")
        )
        { }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.UseProtectedSettingsForCommandToExecuteSecretsRuleMessage, (string)values[0]);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel semanticModel)
        {
            List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            foreach (ResourceMetadata resource in semanticModel.AllResources)
            {
                // We're looking for this pattern:
                //
                // resource xxx '{Microsoft.Compute/virtualMachines/extensions,Microsoft.HybridCompute/machines/extensions}@xxx' = { // right now, these properties apply to Microsoft.{Hybrid,}Compute/virtualMachines/extensions
                //   name: 'xxx'
                //   properties: { <<== required
                //     publisher: 'Microsoft.Compute'
                //     type: 'CustomScriptExtension' <<==  contains "CustomScript"
                //     autoUpgradeMinorVersion: true
                //     settings: {  <<== required
                //       fileUris: split(fileUris, ' ')
                //       commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}'  <<== this property under settings
                //     }
                //   }
                // }
                //
                // This will be a test failure if commandToExecute contains possible secrets

                var typeName = resource.TypeReference.FormatType();
                if (LanguageConstants.ResourceTypeComparer.Equals(typeName, "Microsoft.Compute/virtualMachines/extensions")
                    || LanguageConstants.ResourceTypeComparer.Equals(typeName, "Microsoft.HybridCompute/machines/extensions"))
                {
                    ResourceDeclarationSyntax resourceSyntax = resource.Symbol.DeclaringResource;
                    if (resourceSyntax.TryGetBody()?.SafeGetPropertyByName("properties") is ObjectPropertySyntax propertiesSyntax
                        && propertiesSyntax.Value is ObjectSyntax properties)
                    {
                        if (properties.SafeGetPropertyByName("type")?.Value is StringSyntax typeSyntax
                            && typeSyntax.TryGetLiteralValue() is string typePropertyValue)
                        {
                            if (typePropertyValue.Contains("CustomScript", StringComparison.OrdinalIgnoreCase))
                            {
                                if (properties.SafeGetPropertyByNameRecursive("settings", "commandToExecute") is ObjectPropertySyntax commandToExecuteSyntax)
                                {
                                    // We found a commandToExecute property under "settings" instead of "protectedSettings"

                                    // Does it contain any secrets?
                                    var secrets = FindPossibleSecretsVisitor.FindPossibleSecrets(semanticModel, commandToExecuteSyntax.Value);
                                    if (secrets.Any())
                                    {
                                        diagnostics.Add(CreateDiagnosticForSpan(commandToExecuteSyntax.Key.Span, secrets[0].FoundMessage));
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

