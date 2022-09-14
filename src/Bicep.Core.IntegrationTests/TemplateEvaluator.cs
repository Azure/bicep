// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Expression.Engines;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Core.ErrorResponses;
using Bicep.Core.UnitTests.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using System.Collections.Immutable;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.Core.IntegrationTests
{
    public class TemplateEvaluator
    {
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

            var evaluationContext = TemplateEngine.GetExpressionEvaluationContext(config.ManagementGroup, config.SubscriptionId, config.ResourceGroup, template, null);
            var defaultEvaluateFunction = evaluationContext.EvaluateFunction;
            evaluationContext.EvaluateFunction = (FunctionExpression functionExpression, FunctionArgument[] parameters, TemplateErrorAdditionalInfo additionalInfo) =>
            {
                if (functionExpression.Function.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix) && config.OnListFunc is not null)
                {
                    return config.OnListFunc(
                        functionExpression.Function,
                        parameters[0].Token.ToString(),
                        parameters[1].Token.ToString(),
                        parameters.Length > 2 ? parameters[2].Token : null);
                }

                if (functionExpression.Function.EqualsOrdinalInsensitively("reference"))
                {
                    var resourceId = parameters[0].Token.ToString();
                    var apiVersion = parameters.Length > 1 ? parameters[1].Token.ToString() : null;
                    var fullBody = parameters.Length > 2 ? parameters[2].Token.ToString().EqualsOrdinalInsensitively("Full") : false;

                    if (resourceLookup.TryGetValue(resourceId, out var foundResource) &&
                        (apiVersion is null || StringComparer.OrdinalIgnoreCase.Equals(apiVersion, foundResource.ApiVersion.Value)))
                    {
                        return fullBody ? foundResource.ToJToken() : foundResource.Properties.ToJToken();
                    }

                    if (apiVersion is not null && config.OnReferenceFunc is not null)
                    {
                        return config.OnReferenceFunc(resourceId, apiVersion, fullBody);
                    }
                }

                var value = defaultEvaluateFunction(functionExpression, parameters, additionalInfo);
                return value;
            };

            for (int i = 0; i < template.Resources.Length; i++)
            {
                var resource = template.Resources[i];

                if (resource.Properties is not null)
                {
                    resource.Properties.Value = ExpressionsEngine.EvaluateLanguageExpressionsRecursive(
                        root: resource.Properties.Value,
                        evaluationContext: evaluationContext);
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

                TemplateEngine.ValidateTemplate(template, "2020-10-01", deploymentScope);
                TemplateEngine.ParameterizeTemplate(template, new InsensitiveDictionary<JToken>(parameters), metadata, null, new InsensitiveDictionary<JToken>());

                TemplateEngine.ProcessTemplateLanguageExpressions(config.ManagementGroup, config.SubscriptionId, config.ResourceGroup, template, "2020-10-01", null);

                ProcessTemplateLanguageExpressions(template, config, deploymentScope);

                TemplateEngine.ValidateProcessedTemplate(template, "2020-10-01", deploymentScope);

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

        private static ImmutableDictionary<string, JToken> ParseParametersFile(JToken? parametersJToken)
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
