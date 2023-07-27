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
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using System.Collections.Immutable;
using Bicep.Core;
using System.Text.RegularExpressions;

namespace Bicep.Cli.Services
{
    public class TemplateEvaluation
    {
        private const string DummyTenantId = "";
        private const string DummyManagementGroupName = "";
        private const string DummySubscriptionId = "";
        private const string DummyResourceGroupName = "";
        private const string DummyLocation = "";
        private static readonly Regex templateSchemaPattern = new Regex(@"https?://schema\.management\.azure\.com/schemas/[0-9a-zA-Z-]+/(?<templateType>[a-zA-Z]+)Template\.json#?", RegexOptions.Compiled);
        
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
                    var fullBody = parameters.Length > 2 && parameters[2].Token.ToString().EqualsOrdinalInsensitively("Full");

                    if (apiVersion is not null && config.OnReferenceFunc is not null)
                    {
                        return config.OnReferenceFunc(resourceId, apiVersion, fullBody);
                    }

                    if (resourceLookup.TryGetValue(resourceId, out var foundResource) &&
                        (apiVersion is null || StringComparer.OrdinalIgnoreCase.Equals(apiVersion, foundResource.ApiVersion.Value)))
                    {
                        return fullBody ? foundResource.ToJToken() : foundResource.Properties.ToJToken();
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

        public static Evaluation Evaluate(JToken? templateJtoken, JToken? parametersJToken = null, Func<EvaluationConfiguration, EvaluationConfiguration>? configBuilder = null)
        {
            var configuration = EvaluationConfiguration.Default;

            if (configBuilder is not null)
            {
                configuration = configBuilder(configuration);
            }

            return EvaluateTemplate(templateJtoken, parametersJToken, configuration);
        }

        private static Evaluation EvaluateTemplate(JToken? templateJtoken, JToken? parametersJToken, EvaluationConfiguration config)
        {
            templateJtoken = templateJtoken ?? throw new ArgumentNullException(nameof(templateJtoken));

            var deploymentScope = GetDeploymentScope(templateJtoken["$schema"]!.ToString());

            var metadata = new InsensitiveDictionary<JToken>();

            try
            {
                var template = TemplateEngine.ParseTemplate(templateJtoken.ToString());
                var parameters = ParseParametersFile(parametersJToken);
                
                TemplateEngine.ValidateTemplate(template, "2020-10-01", deploymentScope);
                TemplateEngine.ParameterizeTemplate(template, new InsensitiveDictionary<JToken>(parameters), metadata, null, new InsensitiveDictionary<JToken>());

                TemplateEngine.ProcessTemplateLanguageExpressions(config.ManagementGroup, config.SubscriptionId, config.ResourceGroup, template, "2020-10-01", null);

                ProcessTemplateLanguageExpressions(template, config, deploymentScope);

                TemplateEngine.ValidateProcessedTemplate(template, "2020-10-01", deploymentScope);

                var assertions = template.Asserts?.ToDictionary(p => p.Key, p => (bool)p.Value.Value) ?? new Dictionary<string, bool>();

                return new Evaluation(template, assertions, null);
            }
            catch (Exception exception)
            {
                var error = new InvalidOperationException(
                    $"Evaluating template failed: {exception.Message}." +
                    $"\nTemplate file: {templateJtoken}" +
                    (parametersJToken is null ? "" : $"\nParameters file: {parametersJToken}"),
                    exception);

                return new Evaluation(null, null, error);
            }
        }

        private static ImmutableDictionary<string, JToken> ParseParametersFile(JToken? parametersJToken)
        {
            if (parametersJToken is null)
            {
                return ImmutableDictionary<string, JToken>.Empty;
            }

            return parametersJToken.Cast<JProperty>().ToImmutableDictionary(x => x.Name, x => x.Value!);
        }
        
        private static TemplateDeploymentScope GetDeploymentScope(string templateSchema)
        {
            var templateSchemaMatch = templateSchemaPattern.Match(templateSchema);
            var templateType = templateSchemaMatch.Groups["templateType"].Value.ToLowerInvariant();

            return templateType switch 
            {
                "deployment" => TemplateDeploymentScope.ResourceGroup,
                "subscriptiondeployment" => TemplateDeploymentScope.Subscription,
                "managementgroupdeployment" =>TemplateDeploymentScope.ManagementGroup,
                "tenantdeployment" => TemplateDeploymentScope.Tenant,
                _ => throw new InvalidOperationException($"Unrecognized schema: {templateSchema}"),
            };
        }
    }
}

