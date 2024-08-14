// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Diagnostics;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Engines;
using Bicep.Core.Emit;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    public class TemplateEvaluator
    {
        private class TemplateEvaluationContext : IEvaluationContext
        {
            private readonly IEvaluationContext context;
            private readonly OrdinalInsensitiveDictionary<TemplateResource> resourceLookup;
            private readonly EvaluationConfiguration config;

            private TemplateEvaluationContext(IEvaluationContext context, ExpressionScope scope, OrdinalInsensitiveDictionary<TemplateResource> resourceLookup, EvaluationConfiguration config)
            {
                this.context = context;
                this.Scope = scope;
                this.resourceLookup = resourceLookup;
                this.config = config;
            }

            public static TemplateEvaluationContext Create(Template template, OrdinalInsensitiveDictionary<TemplateResource> resourceLookup, EvaluationConfiguration config)
            {
                var context = TemplateEngine.GetExpressionEvaluationContext(config.ManagementGroup, config.SubscriptionId, config.ResourceGroup, template, null);

                return new TemplateEvaluationContext(context, context.Scope, resourceLookup, config);
            }

            public bool IsShortCircuitAllowed => this.context.IsShortCircuitAllowed;

            public ExpressionScope Scope { get; }

            public bool AllowInvalidProperty(Exception exception, FunctionExpression functionExpression, FunctionArgument[] functionParametersValues, JToken[] selectedProperties) =>
                this.context.AllowInvalidProperty(exception, functionExpression, functionParametersValues, selectedProperties);

            public JToken EvaluateFunction(FunctionExpression functionExpression, FunctionArgument[] parameters, IEvaluationContext context, TemplateErrorAdditionalInfo? additionalnfo)
            {
                if (functionExpression.Function.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix) && this.config.OnListFunc is not null)
                {
                    var resourceId = parameters[0].TryGetToken()?.Value<string>() ?? throw new UnreachableException();
                    var apiVersion = parameters[1].TryGetToken()?.Value<string>() ?? throw new UnreachableException();
                    var body = parameters.Length > 2 ? parameters[2].TryGetToken() : null;

                    return this.config.OnListFunc(functionExpression.Function, resourceId, apiVersion, body);
                }

                if (functionExpression.Function.EqualsOrdinalInsensitively("reference"))
                {
                    var resourceId = parameters[0].TryGetToken()?.Value<string>() ?? throw new UnreachableException();
                    var apiVersion = parameters.Length > 1 ? (parameters[1].TryGetToken()?.Value<string>() ?? throw new UnreachableException()) : null;
                    var fullBody = parameters.Length > 2 && parameters[2].TryGetToken()?.Value<string>() is { } fullBodyParam && StringComparer.OrdinalIgnoreCase.Equals(fullBodyParam, "Full");

                    if (apiVersion is not null && this.config.OnReferenceFunc is not null)
                    {
                        return this.config.OnReferenceFunc(resourceId, apiVersion, fullBody);
                    }

                    if (this.resourceLookup.TryGetValue(resourceId, out var foundResource) &&
                        (apiVersion is null || StringComparer.OrdinalIgnoreCase.Equals(apiVersion, foundResource.ApiVersion.Value)))
                    {
                        return fullBody ? foundResource.ToJToken() : foundResource.Properties.ToJToken();
                    }
                }

                return this.context.EvaluateFunction(functionExpression, parameters, context, additionalnfo);
            }

            public bool ShouldIgnoreExceptionDuringEvaluation(Exception exception) =>
                this.context.ShouldIgnoreExceptionDuringEvaluation(exception);

            public IEvaluationContext WithNewScope(ExpressionScope scope) => new TemplateEvaluationContext(this.context, scope, this.resourceLookup, this.config);
        }

        const string TestTenantId = "d4c73686-f7cd-458e-b377-67adcd46b624";
        const string TestManagementGroupName = "3fc9f36e-8699-43af-b038-1c103980942f";
        const string TestSubscriptionId = "f91a30fd-f403-4999-ae9f-ec37a6d81e13";
        const string TestResourceGroupName = "testResourceGroup";
        const string TestLocation = "West US";

        public delegate JToken OnListDelegate(string functionName, string resourceId, string apiVersion, JToken? body);

        public delegate JToken OnReferenceDelegate(string resourceId, string apiVersion, bool fullBody);

        public record EvaluationConfiguration(
            string TenantId,
            string ManagementGroup,
            string SubscriptionId,
            string ResourceGroup,
            string RgLocation,
            Dictionary<string, JToken> Metadata,
            OnListDelegate? OnListFunc,
            OnReferenceDelegate? OnReferenceFunc)
        {
            public static EvaluationConfiguration Default = new(
                TestTenantId,
                TestManagementGroupName,
                TestSubscriptionId,
                TestResourceGroupName,
                TestLocation,
                new(),
                null,
                null
            );
        }

        private static string GetResourceId(string scopeString, TemplateResource resource)
        {
            var typeSegments = resource.Type.Value.Split('/');
            var nameSegments = resource.Name.Value.Split('/');

            var types = new[] { typeSegments.First() }
                .Concat(typeSegments.Skip(1).Zip(nameSegments, (type, name) => $"{type}/{name}"));

            return $"{scopeString}providers/{string.Join('/', types)}";
        }

        private static void ProcessTemplateLanguageExpressions(Template template, EvaluationConfiguration config, TemplateDeploymentScope deploymentScope)
        {
            var scopeString = deploymentScope switch
            {
                TemplateDeploymentScope.Tenant => "/",
                TemplateDeploymentScope.ManagementGroup => $"/providers/Microsoft.Management/managementGroups/{config.ManagementGroup}/",
                TemplateDeploymentScope.Subscription => $"/subscriptions/{config.SubscriptionId}/",
                TemplateDeploymentScope.ResourceGroup => $"/subscriptions/{config.SubscriptionId}/resourceGroups/{config.ResourceGroup}/",
                _ => throw new InvalidOperationException(),
            };

            var resourceLookup = template.Resources.ToOrdinalInsensitiveDictionary(x => GetResourceId(scopeString, x));

            var evaluationContext = TemplateEvaluationContext.Create(template, resourceLookup, config);

            for (int i = 0; i < template.Resources.Length; i++)
            {
                var resource = template.Resources[i];

                if (resource.Properties is not null)
                {
                    var skipEvaluationPaths = new InsensitiveHashSet();
                    if (resource.Type.Value.EqualsOrdinalInsensitively("Microsoft.Resources/deployments"))
                    {
                        skipEvaluationPaths.Add("template");
                    };

                    resource.Properties.Value = ExpressionsEngine.EvaluateLanguageExpressionsRecursive(
                        root: resource.Properties.Value,
                        evaluationContext: evaluationContext,
                        skipEvaluationPaths: skipEvaluationPaths);
                }
            }

            if (template.Outputs is not null && template.Outputs.Count > 0)
            {
                foreach (var outputKey in template.Outputs.Keys.ToList())
                {
                    template.Outputs[outputKey].Value.Value = ExpressionsEngine.EvaluateLanguageExpressionsRecursive(
                        root: template.Outputs[outputKey].Value.Value,
                        evaluationContext: evaluationContext);
                }
            }
        }

        public static JToken Evaluate(JToken? templateJtoken, JToken? parametersJToken = null, Func<EvaluationConfiguration, EvaluationConfiguration>? configBuilder = null)
        {
            var configuration = EvaluationConfiguration.Default;

            if (configBuilder is not null)
            {
                configuration = configBuilder(configuration);
            }

            return EvaluateTemplate(templateJtoken, parametersJToken, configuration);
        }

        private static JToken EvaluateTemplate(JToken? templateJtoken, JToken? parametersJToken, EvaluationConfiguration config)
        {
            templateJtoken = templateJtoken ?? throw new ArgumentNullException(nameof(templateJtoken));

            var deploymentScope = TemplateHelper.GetDeploymentScope(templateJtoken["$schema"]!.ToString());

            var metadata = new InsensitiveDictionary<JToken>(config.Metadata);
            if (deploymentScope == TemplateDeploymentScope.Subscription || deploymentScope == TemplateDeploymentScope.ResourceGroup)
            {
                metadata["subscription"] = new JObject
                {
                    ["id"] = $"/subscriptions/{config.SubscriptionId}",
                    ["subscriptionId"] = config.SubscriptionId,
                    ["tenantId"] = config.TenantId,
                };
            }
            if (deploymentScope == TemplateDeploymentScope.ResourceGroup)
            {
                metadata["resourceGroup"] = new JObject
                {
                    ["id"] = $"/subscriptions/{config.SubscriptionId}/resourceGroups/{config.ResourceGroup}",
                    ["location"] = config.RgLocation,
                };
            };
            if (deploymentScope == TemplateDeploymentScope.ManagementGroup)
            {
                metadata["managementGroup"] = new JObject
                {
                    ["id"] = $"/providers/Microsoft.Management/managementGroups/{config.ManagementGroup}",
                    ["name"] = config.ManagementGroup,
                    ["type"] = "Microsoft.Management/managementGroups",
                };
            };
            // tenant() function is available at all scopes
            metadata["tenant"] = new JObject
            {
                ["tenantId"] = config.TenantId,
            };

            try
            {
                var template = TemplateEngine.ParseTemplate(templateJtoken.ToString());
                var parameters = ParseParametersFile(parametersJToken);

                TemplateEngine.ValidateTemplate(template, TemplateWriter.NestedDeploymentResourceApiVersion, deploymentScope);

                TemplateEngine.ProcessTemplateLanguageExpressions(
                    managementGroupName: config.ManagementGroup,
                    subscriptionId: config.SubscriptionId,
                    resourceGroupName: config.ResourceGroup,
                    template: template,
                    apiVersion: TemplateWriter.NestedDeploymentResourceApiVersion,
                    inputParameters: new(parameters),
                    metadata: metadata,
                    metricsRecorder: new TemplateMetricsRecorder());

                ProcessTemplateLanguageExpressions(template, config, deploymentScope);

                TemplateEngine.ValidateProcessedTemplate(template, TemplateWriter.NestedDeploymentResourceApiVersion, deploymentScope);

                return template.ToJToken();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Evaluating template failed: {exception.Message}." +
                    $"\nTemplate file: {templateJtoken}" +
                    (parametersJToken is null ? "" : $"\nParameters file: {parametersJToken}"),
                    exception);
            }
        }

        public static ImmutableDictionary<string, JToken> ParseParametersFile(JToken? parametersJToken)
        {
            if (parametersJToken is null)
            {
                return ImmutableDictionary<string, JToken>.Empty;
            }

            parametersJToken.Should().HaveValueAtPath("$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");
            parametersJToken.Should().HaveValueAtPath("contentVersion", "1.0.0.0");
            var parametersObject = parametersJToken["parameters"] as JObject;
            parametersObject.Should().NotBeNull();

            return parametersObject!.Properties().ToImmutableDictionary(x => x.Name, x => x.Value["value"]!);
        }
    }
}
