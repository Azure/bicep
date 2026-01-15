// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Extensibility;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Engine.Definitions;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.ParsedEntities;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Cli.Helpers.Snapshot;

public static class SnapshotHelper
{
    public record ExternalInputValue(
        string Kind,
        JToken? Config,
        JToken Value);

    public static async Task<Snapshot> GetSnapshot(
        ResourceScope targetScope,
        string templateContent,
        string parametersContent,
        string? tenantId,
        string? subscriptionId,
        string? resourceGroup,
        string? deploymentName,
        string? location,
        ImmutableArray<ExternalInputValue> externalInputs,
        CancellationToken cancellationToken)
    {
        var parameters = parametersContent.FromJson<DeploymentParametersDefinition>();
        var template = TemplateEngine.ParseTemplate(templateContent);

        var scope = EnumConverter.ToTemplateDeploymentScope(targetScope)
            ?? throw new CommandLineException($"Cannot create snapshot of template with a target scope of {targetScope}");

        var expansionResult = await TemplateEngine.ExpandNestedDeployments(
            EmitConstants.NestedDeploymentResourceApiVersion,
            scope,
            template,
            parameters: ResolveParameters(parameters, externalInputs),
            rootDeploymentMetadata: GetDeploymentMetadata(tenantId, subscriptionId, resourceGroup, deploymentName, location, scope, template),
            referenceFunctionPreflightEnabled: true,
            cancellationToken: cancellationToken);

        return new(
            [
                .. expansionResult.preflightResources.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(DeploymentPreflightResourceWithParsedExpressions.From(x)))),
                .. expansionResult.extensibleResources.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(new DeploymentWhatIfExtensibleResource
                    {
                        Type = x.Type,
                        ApiVersion = x.ApiVersion,
                        Identifiers = x.Identifiers?.ToObject<JObject>(),
                        Properties = x.Properties?.ToObject<JObject>(),
                    }))),
            ],
            [
                .. expansionResult.diagnostics.Select(d => $"{d.Target} {d.Level} {d.Code}: {d.Message}")
            ]);
    }

    private static JToken? TryGetExternalInputValue(DeploymentExternalInputDefinition externalInputDefinition, ImmutableArray<ExternalInputValue> externalInputs)
    {
        if (externalInputs.FirstOrDefault(
            x => x.Kind == externalInputDefinition.Kind &&
            JToken.DeepEquals(x.Config, externalInputDefinition.Config)) is not { } externalInputValue)
        {
            return null;
        }

        // return an explicit JValue if null, to differentiate from a missing value
        return externalInputValue.Value ?? JValue.CreateNull();
    }

    private static Dictionary<string, ITemplateLanguageExpression> ResolveParameters(DeploymentParametersDefinition parameters, ImmutableArray<ExternalInputValue> externalInputs)
    {
        ParametersRewriteVisitor rewriteVisitor = new(parameters, input => TryGetExternalInputValue(input, externalInputs));

        return parameters.Parameters.ToDictionary(
            kvp => kvp.Key,
            kvp => ResolveParameter(kvp.Key, kvp.Value, rewriteVisitor));
    }

    private static ITemplateLanguageExpression ResolveParameter(
        string parameterName,
        DeploymentParameterDefinition parameter,
        ParametersRewriteVisitor rewriteVisitor)
    {
        if (parameter.Value is not null)
        {
            return JTokenConverter.ConvertToLanguageExpression(parameter.Value);
        }

        if (parameter.Reference is not null)
        {
            return new FunctionExpression("parameters", [parameterName.AsExpression()], null, irreducible: true);
        }

        if (parameter.Expression is not null)
        {
            return rewriteVisitor.Rewrite(ExpressionParser.ParseLanguageExpression(parameter.Expression));
        }

        return new NullExpression(position: null);
    }

    private class ParametersRewriteVisitor : ExpressionRewriteVisitor
    {
        private readonly DeploymentParametersDefinition parameters;
        private readonly Func<DeploymentExternalInputDefinition, JToken?> tryResolveExternalInput;

        public ParametersRewriteVisitor(DeploymentParametersDefinition parameters, Func<DeploymentExternalInputDefinition, JToken?> tryResolveExternalInput)
        {
            this.parameters = parameters;
            this.tryResolveExternalInput = tryResolveExternalInput;
        }

        protected override ITemplateLanguageExpression RewriteFunction(FunctionExpression expression, Func<ITemplateLanguageExpression, ITemplateLanguageExpression> rewriteChildExpression)
        {
            if (expression.Name.Equals(LanguageConstants.ExternalInputsArmFunctionName, StringComparison.OrdinalIgnoreCase))
            {
                if (expression.Arguments.Count() != 1 ||
                    expression.Arguments[0] is not StringExpression stringLiteral)
                {
                    // we will never codegen this function
                    throw new InvalidOperationException();
                }

                var externalInput = parameters.ExternalInputDefinitions[stringLiteral.Value];
                if (tryResolveExternalInput(externalInput) is { } value)
                {
                    return ExpressionParser.ParseLanguageExpression(value);
                }

                return expression with
                {
                    Arguments = [.. expression.Arguments.Select(Rewrite)],
                    Properties = [.. expression.Properties.Select(Rewrite)],
                    // we won't know the value of external inputs until the real deployment
                    Irreducible = true,
                };
            }

            return base.RewriteFunction(expression, rewriteChildExpression);
        }
    }

    private static Dictionary<string, ITemplateLanguageExpression> GetDeploymentMetadata(
        string? tenantId,
        string? subscriptionId,
        string? resourceGroup,
        string? deploymentName,
        string? location,
        TemplateDeploymentScope scope,
        Template template)
    {
        Dictionary<string, ITemplateLanguageExpression> metadata = new(StringComparer.OrdinalIgnoreCase);

        if (tenantId is not null)
        {
            metadata[DeploymentMetadata.TenantKey] = new ObjectExpression(
                [
                    new("countryCode".AsExpression(), MetadataPlaceholder("tenant", "countryCode")),
                    new("displayName".AsExpression(), MetadataPlaceholder("tenant", "displayName")),
                    new("id".AsExpression(), $"/tenants/{tenantId}".AsExpression()),
                    new("tenantId".AsExpression(), tenantId.AsExpression()),
                ],
                position: null);
        }

        if (subscriptionId is not null)
        {
            if (scope is not TemplateDeploymentScope.Subscription and not TemplateDeploymentScope.ResourceGroup)
            {
                throw new CommandLineException($"Subscription ID cannot be specified for a template of scope {scope}");
            }

            metadata[DeploymentMetadata.SubscriptionKey] = new ObjectExpression(
                [
                    new("id".AsExpression(), $"/subscriptions/{subscriptionId}".AsExpression()),
                    new("subscriptionId".AsExpression(), subscriptionId.AsExpression()),
                    new(
                        "tenantId".AsExpression(),
                        tenantId?.AsExpression() ?? MetadataPlaceholder("tenant", "tenantId")),
                    new("displayName".AsExpression(), MetadataPlaceholder("subscription", "displayName")),
                ],
                position: null);
        }

        if (resourceGroup is not null)
        {
            if (scope is not TemplateDeploymentScope.ResourceGroup)
            {
                throw new CommandLineException($"Resource group name cannot be specified for a template of scope {scope}");
            }

            metadata[DeploymentMetadata.ResourceGroupKey] = new ObjectExpression(
                [
                    new("id".AsExpression(), subscriptionId is not null
                        ? $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}".AsExpression()
                        : MetadataPlaceholder("resourceGroup", "id")),
                    new("name".AsExpression(), resourceGroup.AsExpression()),
                    new("type".AsExpression(), "Microsoft.Resources/resourceGroups".AsExpression()),
                    new("location".AsExpression(), location is not null
                        ? location.AsExpression()
                        : MetadataPlaceholder("resourceGroup", "location")),
                    new("tags".AsExpression(), MetadataPlaceholder("resourceGroup", "tags")),
                    new("managedBy".AsExpression(), MetadataPlaceholder("resourceGroup", "managedBy")),
                    new("properties".AsExpression(), MetadataPlaceholder("resourceGroup", "properties")),
                ],
                position: null);
        }

        if (deploymentName is not null ||
            (location is not null && scope is not TemplateDeploymentScope.ResourceGroup))
        {
            Dictionary<ITemplateLanguageExpression, ITemplateLanguageExpression> properties = new()
            {
                {
                    "name".AsExpression(),
                    deploymentName?.AsExpression() ?? MetadataPlaceholder("deployment", "name")
                },
                {
                    "properties".AsExpression(),
                    new ObjectExpression(
                        [
                            new("template".AsExpression(), new ObjectExpression(
                                [new("contentVersion".AsExpression(), template.ContentVersion.Value.AsExpression())],
                                position: null))
                        ],
                        position: null)
                },
            };

            if (scope is not TemplateDeploymentScope.ResourceGroup)
            {
                properties["location".AsExpression()] = location?.AsExpression()
                    ?? MetadataPlaceholder("deployment", "location");
            }

            metadata[DeploymentMetadata.DeploymentKey] = new ObjectExpression(properties, position: null);
        }

        return metadata;
    }

    private static ITemplateLanguageExpression MetadataPlaceholder(string name, params string[] properties)
        => new FunctionExpression(name, [], [.. properties.Select(p => p.AsExpression())], null, irreducible: true);

    public static Snapshot Deserialize(string contents)
        => JsonSerializer.Deserialize<Snapshot>(contents, SnapshotSerializationContext.FileSerializer.Snapshot) ?? throw new InvalidOperationException("Failed to deserialize snapshot");

    public static string Serialize(Snapshot newSnapshot)
        => JsonSerializer.Serialize(newSnapshot, SnapshotSerializationContext.FileSerializer.Snapshot);
}
