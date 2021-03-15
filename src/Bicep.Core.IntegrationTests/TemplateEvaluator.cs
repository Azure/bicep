// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Configuration;
using Azure.Deployments.Core.Collections;
using Azure.Deployments.Core.Instrumentation;
using Azure.Deployments.Templates.Schema;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Expression.Engines;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Core.ErrorResponses;
using System.Collections.Immutable;

namespace Bicep.Core.IntegrationTests
{
    public class TemplateEvaluator
    {
        public delegate JToken OnListDelegate(string functionName, string resourceId, string apiVersion, JToken? body);

        public delegate JToken OnReferenceDelegate(string resourceId, string apiVersion, bool fullBody);

        public record EvaluationConfiguration(
            ImmutableDictionary<string, JToken> Parameters,
            OnListDelegate? OnListFunc,
            OnReferenceDelegate? OnReferenceFunc);

        private static string GetResourceId(TemplateResource resource)
        {
            var typeSegments = resource.Type.Value.Split('/');
            var nameSegments = resource.Name.Value.Split('/');

            var types = new [] { typeSegments.First() }
                .Concat(typeSegments.Skip(1).Zip(nameSegments, (type, name) => $"{type}/{name}"));

            return string.Join('/', types);
        }

        private static void ProcessTemplateLanguageExpressions(Template template, EvaluationConfiguration configuration)
        {
            var resourceLookup = template.Resources.ToOrdinalInsensitiveDictionary(GetResourceId);

            var evaluationContext = TemplateEngine.GetExpressionEvaluationContext(template);
            var oldEvaluateFunction = evaluationContext.EvaluateFunction;
            evaluationContext.EvaluateFunction = (FunctionExpression functionExpression, JToken[] parameters, TemplateErrorAdditionalInfo additionalInfo) =>
            {
                if (functionExpression.Function.StartsWithOrdinalInsensitively("list") &&
                    configuration.OnListFunc is not null)
                {
                    return configuration.OnListFunc(
                        functionExpression.Function,
                        parameters[0].ToString(),
                        parameters[1].ToString(),
                        parameters.Length > 2 ? parameters[2] : null);
                }

                if (functionExpression.Function.EqualsOrdinalInsensitively("reference"))
                {
                    var resourceId = parameters[0].ToString();
                    var apiVersion = parameters.Length > 1 ? parameters[1].ToString() : null;
                    var fullBody = parameters.Length > 2 ? parameters[2].ToString().EqualsOrdinalInsensitively("Full") : false;

                    // TODO use exact matching with the scope of the template
                    var foundResource = resourceLookup.FirstOrDefault(x => resourceId.EndsWithOrdinalInsensitively(x.Key)).Value;
                    if (apiVersion is not null || foundResource is not null)
                    {
                        if (apiVersion is null || foundResource is not null && apiVersion.EqualsOrdinalInsensitively(foundResource.ApiVersion.Value))
                        {
                            return fullBody ? foundResource.ToJToken() : foundResource.Properties.ToJToken();
                        }

                        if (configuration.OnReferenceFunc is not null)
                        {
                            return configuration.OnReferenceFunc(resourceId, apiVersion, fullBody);
                        }
                    }
                }

                var value = oldEvaluateFunction(functionExpression, parameters, additionalInfo);
                return value;
            };

            for (int i = 0; i < template.Resources.Length; i++)
            {
                var resource = template.Resources[i];

                resource.Properties.Value = ExpressionsEngine.EvaluateLanguageExpressionsOptimistically(
                    root: resource.Properties.Value,
                    evaluationContext: evaluationContext);
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

        public static JToken Evaluate(JToken? templateJtoken, Func<EvaluationConfiguration, EvaluationConfiguration>? configurationBuilder = null)
        {
            var configuration = new EvaluationConfiguration(
                ImmutableDictionary<string, JToken>.Empty,
                null,
                null);

            if (configurationBuilder is not null)
            {
                configuration = configurationBuilder(configuration);
            }

            return EvaluateTemplate(templateJtoken, configuration);
        }

        private static JToken EvaluateTemplate(JToken? templateJtoken, EvaluationConfiguration configuration)
        {
            const string testSubscriptionId = "f91a30fd-f403-4999-ae9f-ec37a6d81e13";
            const string testResourceGroupName = "testResourceGroup";
            const string testLocation = "West US";

            DeploymentsInterop.Initialize();

            templateJtoken = templateJtoken ?? throw new ArgumentNullException(nameof(templateJtoken));

            var deploymentScope = templateJtoken["$schema"]?.ToString() switch {
                "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#" => TemplateDeploymentScope.Tenant,
                "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#" => TemplateDeploymentScope.ManagementGroup,
                "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#" => TemplateDeploymentScope.Subscription,
                "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#" => TemplateDeploymentScope.ResourceGroup,
                _ => throw new InvalidOperationException(),
            };

            var metadata = new InsensitiveDictionary<JToken>();
            if (deploymentScope == TemplateDeploymentScope.Subscription || deploymentScope == TemplateDeploymentScope.ResourceGroup)
            {
                metadata["subscription"] = new JObject {
                    ["id"] = $"/subscriptions/{testSubscriptionId}",
                };
            }
            if (deploymentScope == TemplateDeploymentScope.ResourceGroup)
            {
                metadata["resourceGroup"] = new JObject {
                    ["id"] = $"/subscriptions/{testSubscriptionId}/resourceGroups/{testResourceGroupName}",
                    ["location"] = testLocation,
                };
            };

            var template = TemplateEngine.ParseTemplate(templateJtoken.ToString());

            TemplateEngine.ValidateTemplate(template, "2020-06-01", deploymentScope);
            TemplateEngine.ParameterizeTemplate(template, new InsensitiveDictionary<JToken>(configuration.Parameters), metadata, new InsensitiveDictionary<JToken>());

            TemplateEngine.ProcessTemplateLanguageExpressions(template, "2020-06-01");

            ProcessTemplateLanguageExpressions(template, configuration);

            TemplateEngine.ValidateProcessedTemplate(template, "2020-06-01", deploymentScope);

            return template.ToJToken();
        }
    }
}