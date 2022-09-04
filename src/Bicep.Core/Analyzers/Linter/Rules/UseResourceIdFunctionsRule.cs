// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseResourceIdFunctionsRule : LinterRuleBase
    {
        public new const string Code = "use-resource-id-functions";

        public UseResourceIdFunctionsRule() : base(
            code: Code,
            description: CoreResources.UseResourceIdFunctionsRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        private static readonly HashSet<string> allowedFunctions = new() {
                "extensionResourceId",
                "resourceId",
                "subscriptionResourceId",
                "tenantResourceId",
                "if",
                "reference",
                "subscription",
                "guid"
                };

        private static readonly HashSet<string> excludedProperties = new(
            new[] {
                // These properties are ignored, since they are not actually resourceIds
                // (the names are unique enough that we won't worry about matching against
                //   an exact resource type, plus some of them occur in multiple resource types)
                //
                // (case insensitive)
                "appId",                       // Microsoft.Insights
                "clientId",                    // Microsoft.BotService - common var name
                "contentId",                   // Microsoft.Sentinel/Solutions/Metadata
                "connectorId",                 // Microsoft.Sentinel/Solutions/Analytical Rule/Metadata
                "DataTypeId",                  // Microsoft.OperationalInsights/workspaces/dataSources
                "defaultMenuItemId",           // Microsoft.Portal/dashboards - it's a messy resource
                "deploymentSpecId",            // Microsoft.NetApp/netAppAccounts/volumeGroups
                "detector",                    // microsoft.alertsmanagement/smartdetectoralertrules (detector.id is the one we want to skip)
                "groupId",                     // Microsoft.DataFactory/factories/managedVirtualNetworks/managedPrivateEndpoints
                "IllusiveIncidentId",          // Microsoft.Sentinel/Solutions/Analytical Rule/Metadata
                "keyId",                       // Microsoft.Cdn/profiles urlSigningKeys
                "keyVaultId",                  // KeyVaultIDs
                "keyVaultSecretId",            // Microsoft.Network/applicationGateways sslCertificates - this is actually a uri created with reference() and concat /secrets/secretname
                "locations",                   // Microsoft.Insights/webtests
                "menuId",                      // Microsoft.Portal/dashboards
                "metadata",                    // Multiple resources
                "metricId",                    // Microsoft.ServiceBus/namespaces
                "nodeAgentSkuId",              // Microsoft.Batch/batchAccounts/pools
                "objectId",                    // Common Property name
                "parentId",                    // Microsoft.Sentinel/Solutions/Metadata
                "policyDefinitionReferenceId", // Microsft.Authorization/policySetDefinition unique Id used when setting up a PolicyDefinitionReference
                "requestedServiceObjectiveId", // Microsoft.Sql/servers/databases
                "resource",                    // Microsoft.DocumentDB/databaseAccounts/sqlDatabase and child resources
                "ruleId",                      // Microsoft.Network/applicationGatewayWebApplicationFirewallPolicies
                "schemaId",                    // Microsoft.ApiManagement/service/apis/operations
                "servicePrincipalClientId",    // common var name
                "sid",                         // Microsoft.Sql/servers/administrators/activeDirectory
                "StartingDeviceID",            // SQLIaasVMExtension > settings/ServerConfigurationsManagementSettings/SQLStorageUpdateSettings
                "subscriptionId",              // Microsoft.Cdn/profiles urlSigningKeys
                "SyntheticMonitorId",          // Microsoft.Insights/webtests
                "tags",                        // Multiple resources
                "targetProtectionContainerId", // Microsoft.RecoveryServices/vaults/replicationFabrics/replicationProtectionContainers/replicationProtectionContainerMappings (yes really)
                "targetWorkerSizeId",          // Microsoft.Web/serverFarms (later apiVersions)
                "tenantId",                    // Common Property name
                "timezoneId",                  // Microsoft.SQL/managedInstances
                "vlanId",                      // Unique Id to establish peering when setting up an ExpressRoute circuit
                "workerSizeId",                // Microsoft.Web/serverFarms (older apiVersions)
                "UniqueFindingId",             // Microsoft.Sentinel/Solutions/Metadata
            },
            StringComparer.OrdinalIgnoreCase);

        private static readonly Regex[] excludedResourceTypesRegex =
            (new string[]
            {
                // These are regex's (case-insensitive)
                "^microsoft.portal/dashboards$",
                "^microsoft.logic/workflows$",
                "^microsoft.ApiManagement/service/backends$",
                "^Microsoft.Web/sites/config",
                // Skip for Microsoft.DocumentDb/databaseAccounts/mongodbDatabases/collections
                // and for "other collections" on docDB
                @"^Microsoft\.DocumentDb/databaseAccounts/\w{0,}/collections$",
            }).Select(s => new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase)).ToArray();

        internal record Failure(
            ObjectPropertySyntax Property,
            IEnumerable<DeclaredSymbol> PathToExpression
        );

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            foreach (var resource in model.DeclaredResources)
            {
                if (resource.IsAzResource
                    && resource.Symbol.TryGetBodyPropertyValue(LanguageConstants.ResourcePropertiesPropertyName) is ObjectSyntax properties)
                {
                    // On excluded resources list?
                    if (resource.TypeReference.FormatType() is not String resourceType
                    || excludedResourceTypesRegex.Any(regex => regex.IsMatch(resourceType)))
                    {
                        continue;
                    }

                    var visitor = new IdPropertyVisitor(model);
                    properties.Accept(visitor);

                    foreach (Failure failure in visitor.Failures)
                    {
                        var propertyName = failure.Property.Key.ToText();
                        var paths = failure.PathToExpression.Any() ?
                            (new string[] { propertyName }).Concat(failure.PathToExpression.Select(s => s.Name)) :
                            Enumerable.Empty<string>();
                        var path = string.Join(" -> ", paths);
                        yield return CreateDiagnosticForSpan(
                            failure.Property.Key.Span,
                            failure.Property.Key.ToText(),
                            path);
                    }
                }
            }
        }

        public override string FormatMessage(params object[] values)
        {
            var propertyName = (string)values[0];
            var path = (string)values[1];
            var allowedList = string.Join(", ", allowedFunctions.OrderByAscendingInsensitively(k => k).ToArray());
            var mainMessage = string.Format(CoreResources.UseResourceIdFunctionsRule_MessageFormat, propertyName, allowedList);
            var pathMessage = string.IsNullOrWhiteSpace(path) ? null : string.Format(CoreResources.UseResourceIdFunctionsRule_NonConformingExprPath, path);
            return pathMessage is null ? mainMessage : $"{mainMessage} {pathMessage}";
        }

        private class IdPropertyVisitor : SyntaxVisitor
        {
            private readonly List<Failure> failures = new();
            public IEnumerable<Failure> Failures => failures;

            private readonly SemanticModel model;

            internal IdPropertyVisitor(SemanticModel model)
            {
                this.model = model;
            }

            private enum ResourceIdStatus
            {
                NotResourceId,
                IsResourceId,
                SkipChildren,
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax propertySyntax)
            {
                var status = IsResourceIdProperty(propertySyntax);
                switch (status)
                {
                    case ResourceIdStatus.NotResourceId:
                        break;
                    case ResourceIdStatus.IsResourceId:
                        var failure = AnalyzeIdProperty(model, propertySyntax);
                        if (failure is not null)
                        {
                            failures.Add(failure);
                        }
                        return;
                    case ResourceIdStatus.SkipChildren:
                        return;
                    default:
                        throw new InvalidOperationException($"Unexpected {nameof(ResourceIdStatus)} {status}");
                }

                base.VisitObjectPropertySyntax(propertySyntax);
            }

            private static ResourceIdStatus IsResourceIdProperty(ObjectPropertySyntax PropertySyntax)
            {
                if (PropertySyntax.TryGetKeyText() is not string propertyName)
                {
                    return ResourceIdStatus.NotResourceId;
                }

                // In our exception list?
                if (excludedProperties.Contains(propertyName))
                {
                    return ResourceIdStatus.SkipChildren;
                }

                if (propertyName is null || !propertyName.EndsWithOrdinalInsensitively("id"))
                {
                    return ResourceIdStatus.NotResourceId;
                }

                // Ignore properties like "UID" and "guid" (but not 'guId')
                if (propertyName.EndsWith("uid", StringComparison.Ordinal) || propertyName.EndsWith("UID", StringComparison.Ordinal))
                {
                    return ResourceIdStatus.NotResourceId;
                }

                return ResourceIdStatus.IsResourceId;
            }

            // Returns true to indicate we should stop searching deeper
            private static Failure? AnalyzeIdProperty(SemanticModel model, ObjectPropertySyntax propertySyntax)
            {
                var type = model.GetTypeInfo(propertySyntax.Value);
                if (type.IsStrictlyAssignableToString())
                {
                    return AnalyzeIdPropertyValue(model, propertySyntax, propertySyntax.Value, Array.Empty<DeclaredSymbol>());
                }

                return null;
            }

            private static Failure? AnalyzeIdPropertyValue(SemanticModel model, ObjectPropertySyntax propertySyntax, SyntaxBase expression, DeclaredSymbol[] currentPaths)
            {
                // Does it contain any reference to a resource or module symbolic name anywhere in the expression?  If so, we assume it's being done correctly
                if (expression.Any(syntax =>
                    syntax is VariableAccessSyntax variableAccessSyntax && model.GetSymbolInfo(variableAccessSyntax) is DeclaredSymbol symbol
                    && (symbol is ResourceSymbol || symbol is ModuleSymbol)))
                {
                    return null;
                }

                // Otherwise, check if it's using a recommended function call
                switch (expression)
                {
                    case FunctionCallSyntax functionCallSyntax:
                        if (allowedFunctions.Contains(functionCallSyntax.Name.IdentifierName))
                        {
                            // Matches one of our allowed function calls, so we're good.
                            return null;
                        }
                        break;
                    case VariableAccessSyntax: // Variable and parameter access
                        if (model.GetSymbolInfo(expression) is DeclaredSymbol symbol)
                        {
                            // Create nested path for recursive call
                            var nestedPath = new DeclaredSymbol[currentPaths.Length + 1];
                            Array.Copy(currentPaths, nestedPath, currentPaths.Length);
                            nestedPath[^1] = symbol;

                            if (symbol is VariableSymbol variable)
                            {
                                // Analyze the variable's definition
                                var variableValue = (variable.DeclaringSyntax as VariableDeclarationSyntax)?.Value;
                                return variableValue is null ? null : AnalyzeIdPropertyValue(model, propertySyntax, variableValue, nestedPath);
                            }
                            else if (symbol is ParameterSymbol parameter)
                            {
                                // Parameters are always okay
                                return null;
                            }
                        }
                        break;
                    case TernaryOperationSyntax:
                        // "if"/ternary is acceptable
                        return null;
                    case StringSyntax stringSyntax:
                        if (stringSyntax.TryGetLiteralValue() is string stringLiteral)
                        {
                            // If the string literal has forward slashes in it, we'll assume it's meant to be a resource ID and thus fails the test.
                            // If it has no forward slashes, we'll assume it's not meant to be a resource ID and let it pass
                            if (!stringLiteral.Contains('/'))
                            {
                                return null;
                            }
                        }
                        break;
                }

                return new Failure(propertySyntax, currentPaths);
            }
        }
    }
}
