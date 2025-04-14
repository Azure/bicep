// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Diagnostics;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Emit;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Utils
{
    public partial class TemplateEvaluator
    {
        private class NoOpTemplateMetricRecorder : ITemplateMetricsRecorder
        {
            public static readonly NoOpTemplateMetricRecorder Instance = new();

            public void Record(MetricDatum metricDatum)
            {
            }
        }

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
                var context = TemplateEngine.GetExpressionEvaluationContext(config.ManagementGroup, config.SubscriptionId, config.ResourceGroup, template, NoOpTemplateMetricRecorder.Instance);

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

        private static readonly string DummyTenantId = Guid.Empty.ToString();
        private static readonly string DummyManagementGroupName = Guid.Empty.ToString();
        private static readonly string DummySubscriptionId = Guid.Empty.ToString();
        private const string DummyResourceGroupName = "DummyResourceGroup";
        private const string DummyLocation = "Dummy Location";

        [GeneratedRegex(@"https?://schema\.management\.azure\.com/schemas/[0-9a-zA-Z-]+/(?<templateType>[a-zA-Z]+)Template\.json#?", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex templateSchemaPattern();
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
                DummyTenantId,
                DummyManagementGroupName,
                DummySubscriptionId,
                DummyResourceGroupName,
                DummyLocation,
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
                    template.Outputs[outputKey].Value.Value = ExpressionsEngine.EvaluateLanguageExpressionsOptimistically(
                        root: template.Outputs[outputKey].Value.Value,
                        evaluationContext: evaluationContext);
                }
            }
        }

        public static Template Evaluate(JToken? templateJtoken, JToken? parametersJToken = null, Func<EvaluationConfiguration, EvaluationConfiguration>? configBuilder = null)
        {
            var configuration = EvaluationConfiguration.Default;

            if (configBuilder is not null)
            {
                configuration = configBuilder(configuration);
            }

            return EvaluateTemplate(templateJtoken, parametersJToken, configuration);
        }

        private static Template EvaluateTemplate(JToken? templateJtoken, JToken? parametersJToken, EvaluationConfiguration config)
        {
            templateJtoken = templateJtoken ?? throw new ArgumentNullException(nameof(templateJtoken));

            var deploymentScope = GetDeploymentScope(templateJtoken["$schema"]!.ToString());

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
                var parameters = ConvertParameters(parametersJToken);

                TemplateEngine.ValidateTemplate(template, EmitConstants.NestedDeploymentResourceApiVersion, deploymentScope);

                TemplateEngine.ProcessTemplateLanguageExpressions(
                    managementGroupName: config.ManagementGroup,
                    subscriptionId: config.SubscriptionId,
                    resourceGroupName: config.ResourceGroup,
                    template: template,
                    apiVersion: EmitConstants.NestedDeploymentResourceApiVersion,
                    inputParameters: new(parameters),
                    metadata: metadata,
                    extensionConfigs: [], // TODO: check this with Kyle
                    metricsRecorder: new TemplateMetricsRecorder());

                ProcessTemplateLanguageExpressions(template, config, deploymentScope);

                TemplateEngine.ValidateProcessedTemplate(template, EmitConstants.NestedDeploymentResourceApiVersion, deploymentScope);

                return template;
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

        private static ImmutableDictionary<string, JToken> ConvertParameters(JToken? parametersJToken)
        {
            if (parametersJToken is null)
            {
                return ImmutableDictionary<string, JToken>.Empty;
            }

            var parametersObject = parametersJToken["parameters"] as JObject;

            var externalInputsObject = parametersJToken["externalInputs"] as JObject;
            var externalInputs = externalInputsObject?.Properties().ToImmutableDictionary(
                x => x.Name, 
                x => new DeploymentExternalInput { Value = x.Value["value"] }) ?? ImmutableDictionary<string, DeploymentExternalInput>.Empty;

            var helper = new ParameterExpressionEvaluationHelper(
                metricsRecorder: new TemplateMetricsRecorder(),
                externalInputs: externalInputs);

            
            return parametersObject!.Properties().ToImmutableDictionary(x => x.Name, x => {
                if (x.Value["expression"] is { } expression)
                {
                    return ExpressionsEngine.ParseLanguageExpression(expression.ToString()).EvaluateExpression(helper.EvaluationContext);
                }

                return x.Value["value"]!;
            });
        }

        private static TemplateDeploymentScope GetDeploymentScope(string templateSchema)
        {
            var templateSchemaMatch = templateSchemaPattern().Match(templateSchema);
            var templateType = templateSchemaMatch.Groups["templateType"].Value.ToLowerInvariant();

            return templateType switch
            {
                "deployment" => TemplateDeploymentScope.ResourceGroup,
                "subscriptiondeployment" => TemplateDeploymentScope.Subscription,
                "managementgroupdeployment" => TemplateDeploymentScope.ManagementGroup,
                "tenantdeployment" => TemplateDeploymentScope.Tenant,
                _ => throw new InvalidOperationException($"Unrecognized schema: {templateSchema}"),
            };
        }
    }
}
