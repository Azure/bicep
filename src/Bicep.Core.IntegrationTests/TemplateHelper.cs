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
    public class TemplateHelper
    {
        public record ParameterProperties(JToken? Value, JToken? Expression);
        public static ImmutableDictionary<string, ParameterProperties> ConvertAndAssertParameters(JToken? parametersJToken)
        {
            if (parametersJToken is null)
            {
                return [];
            }

            parametersJToken.Should().HaveValueAtPath("$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");
            parametersJToken.Should().HaveValueAtPath("contentVersion", "1.0.0.0");
            var parametersObject = parametersJToken["parameters"] as JObject;
            parametersObject.Should().NotBeNull();

            return parametersObject!.Properties().ToImmutableDictionary(x => x.Name, x => new ParameterProperties(x.Value["value"], x.Value["expression"]));
        }

        public static JObject ConvertAndAssertExternalInputs(JToken? parametersJToken)
        {
            if (parametersJToken is null)
            {
                return [];
            }

            var externalInputsObject = parametersJToken["externalInputDefinitions"] as JObject;
            externalInputsObject.Should().NotBeNull();

            return externalInputsObject!;
        }
    }
}
