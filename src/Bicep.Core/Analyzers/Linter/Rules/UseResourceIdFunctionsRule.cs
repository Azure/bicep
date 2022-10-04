// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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

        private class Exclusion
        {
            public Regex? ResourceType { get; init; }
            public string? propertyName { get; init; }

            public Exclusion(string? regexResourceType, string? propertyName)
            {
                this.ResourceType = regexResourceType is null ? null : new Regex(regexResourceType, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
                this.propertyName = propertyName;
            }
        }

        // These properties are ignored, since they are not actually resourceIds.
        // Some of the names are unique enough (and for backwards compat with the ARM TTK) that we won't worry about matching against
        //   an exact resource type, plus some of them occur in multiple resource types.

        // Both resource type and property name are case-insensitive
        private static readonly Exclusion[] allowedResourcesAndProperties = new[] {
            new Exclusion(null, "appId"),                       // Example: Microsoft.Insights
                new Exclusion(null, "appId"),                       // Example: Microsoft.Insights
                new Exclusion(null, "clientId"),                    // Example: Microsoft.BotService - common var name
                new Exclusion(null, "contentId"),                   // Example: Microsoft.Sentinel/Solutions/Metadata
                new Exclusion(null, "connectorId"),                 // Example: Microsoft.Sentinel/Solutions/Analytical Rule/Metadata
                new Exclusion(null, "DataTypeId"),                  // Example: Microsoft.OperationalInsights/workspaces/dataSources
                new Exclusion(null, "defaultMenuItemId"),           // Example: Microsoft.Portal/dashboards - it's a messy resource
                new Exclusion(null, "deploymentSpecId"),            // Example: Microsoft.NetApp/netAppAccounts/volumeGroups
                new Exclusion(null, "detector"),                    // Example: microsoft.alertsmanagement/smartdetectoralertrules (detector.id is the one we want to skip)
                new Exclusion(null, "groupId"),                     // Example: Microsoft.DataFactory/factories/managedVirtualNetworks/managedPrivateEndpoints
                new Exclusion(null, "IllusiveIncidentId"),          // Example: Microsoft.Sentinel/Solutions/Analytical Rule/Metadata
                new Exclusion(null, "keyId"),                       // Example: Microsoft.Cdn/profiles urlSigningKeys
                new Exclusion(null, "keyVaultId"),                  // Example: KeyVaultIDs
                new Exclusion(null, "keyVaultSecretId"),            // Example: Microsoft.Network/applicationGateways sslCertificates - this is actually a uri created with reference() and concat /secrets/secretname
                new Exclusion(null, "locations"),                   // Example: Microsoft.Insights/webtests
                new Exclusion(null, "menuId"),                      // Example: Microsoft.Portal/dashboards
                new Exclusion(null, "metadata"),                    // Multiple resources
                new Exclusion(null, "metricId"),                    // Example: Microsoft.ServiceBus/namespaces
                new Exclusion(null, "nodeAgentSkuId"),              // Example: Microsoft.Batch/batchAccounts/pools
                new Exclusion(null, "objectId"),                    // Common Property name
                new Exclusion(null, "parentId"),                    // Example: Microsoft.Sentinel/Solutions/Metadata
                new Exclusion(null, "policyDefinitionReferenceId"), // Example: Microsft.Authorization/policySetDefinition unique Id used when setting up a PolicyDefinitionReference
                new Exclusion(null, "requestedServiceObjectiveId"), // Example: Microsoft.Sql/servers/databases
                new Exclusion(null, "resource"),                    // Example: Microsoft.DocumentDB/databaseAccounts/sqlDatabase and child resources
                new Exclusion(null, "ruleId"),                      // Example: Microsoft.Network/applicationGatewayWebApplicationFirewallPolicies
                new Exclusion(null, "schemaId"),                    // Example: Microsoft.ApiManagement/service/apis/operations
                new Exclusion(null, "servicePrincipalClientId"),    // Common var name
                new Exclusion(null, "sid"),                         // Example: Microsoft.Sql/servers/administrators/activeDirectory
                new Exclusion(null, "StartingDeviceID"),            // Example: SQLIaasVMExtension > settings/ServerConfigurationsManagementSettings/SQLStorageUpdateSettings
                new Exclusion(null, "subscriptionId"),              // Example: Microsoft.Cdn/profiles urlSigningKeys
                new Exclusion(null, "SyntheticMonitorId"),          // Example: Microsoft.Insights/webtests
                new Exclusion(null, "tags"),                        // Multiple resources
                new Exclusion(null, "targetProtectionContainerId"), // Example: Microsoft.RecoveryServices/vaults/replicationFabrics/replicationProtectionContainers/replicationProtectionContainerMappings (yes really)
                new Exclusion(null, "targetWorkerSizeId"),          // Example: Microsoft.Web/serverFarms (later apiVersions)
                new Exclusion(null, "tenantId"),                    // Common Property name
                new Exclusion(null, "timezoneId"),                  // Example: Microsoft.SQL/managedInstances
                new Exclusion(null, "vlanId"),                      // Example: Unique Id to establish peering when setting up an ExpressRoute circuit
                new Exclusion(null, "workerSizeId"),                // Example: Microsoft.Web/serverFarms (older apiVersions)
                new Exclusion(null, "UniqueFindingId"),             // Example: Microsoft.Sentinel/Solutions/Metadata
                new Exclusion(null, "principalId"),                 // Example: Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments

                // These resource types are completely excluded from the rule
                new Exclusion("^microsoft.portal/dashboards$", null),
                new Exclusion("^microsoft.logic/workflows$", null),
                new Exclusion("^microsoft.ApiManagement/service/backends$", null),
                new Exclusion("^Microsoft.Web/sites/config", null),
                // Skip for Microsoft.DocumentDb/databaseAccounts/mongodbDatabases/collections
                // and for "other collections" on docDB
                new Exclusion(@"^Microsoft\.DocumentDb/databaseAccounts/\w{0,}/collections$", null),

                // Specific properties of specific resource types
                new Exclusion("^Microsoft.ApiManagement/service/subscriptions$", "ownerId"), // #8382
            };

        internal record Failure(
            ObjectPropertySyntax Property,
            IEnumerable<DeclaredSymbol> PathToExpression
        );

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var diagnosticLevel = GetDiagnosticLevel(model);
            foreach (var resource in model.DeclaredResources)
            {
                if (resource.IsAzResource
                    && resource.Symbol.TryGetBodyPropertyValue(LanguageConstants.ResourcePropertiesPropertyName) is ObjectSyntax properties)
                {
                    if (resource.TypeReference.FormatType() is not String resourceType)
                    {
                        continue;
                    }

                    Exclusion[] exclusionsMatchingResourceType = allowedResourcesAndProperties.Where(allowed => allowed.ResourceType is null || allowed.ResourceType.IsMatch(resourceType)).ToArray();
                    if (exclusionsMatchingResourceType.Any(excl => excl.propertyName is null)) {
                        // All properties on this resource type are excluded
                        continue;
                    }

                    string[] excludedPropertiesForThisResource = exclusionsMatchingResourceType.Select(excl => excl.propertyName!).ToArray(); // propertyName can't be null in this list
                    var visitor = new IdPropertyVisitor(model, excludedPropertiesForThisResource.ToArray());
                    properties.Accept(visitor);

                    foreach (Failure failure in visitor.Failures)
                    {
                        var propertyName = failure.Property.Key.ToText();
                        var paths = failure.PathToExpression.Any() ?
                            (new string[] { propertyName }).Concat(failure.PathToExpression.Select(s => s.Name)) :
                            Enumerable.Empty<string>();
                        var path = string.Join(" -> ", paths);
                        yield return CreateDiagnosticForSpan(
                            diagnosticLevel,
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
            private readonly string[] allowedPropertyNames;

            internal IdPropertyVisitor(SemanticModel model, string[] allowedPropertyNames)
            {
                this.model = model;
                this.allowedPropertyNames = allowedPropertyNames;
            }

            private enum ResourceIdStatus
            {
                NotResourceId,
                IsResourceId,
                SkipChildren,
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax propertySyntax)
            {
                var status = IsResourceIdProperty(propertySyntax, allowedPropertyNames);
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

            private static ResourceIdStatus IsResourceIdProperty(ObjectPropertySyntax PropertySyntax, string[] allowedPropertyNames)
            {
                if (PropertySyntax.TryGetKeyText() is not string propertyName)
                {
                    return ResourceIdStatus.NotResourceId;
                }

                // In our always-allowed list?
                if (allowedPropertyNames.Any(prop => propertyName.EqualsOrdinalInsensitively(prop)))
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
                if (type.IsString())
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
                    case ParenthesizedExpressionSyntax parenthesizedExpressionSyntax:
                        // Analyze inside parentheses
                        return AnalyzeIdPropertyValue(model, propertySyntax, parenthesizedExpressionSyntax.Expression, currentPaths);
                }

                return new Failure(propertySyntax, currentPaths);
            }
        }
    }
}
