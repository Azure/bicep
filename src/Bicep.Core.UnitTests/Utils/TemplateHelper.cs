// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Templates.Engines;
using Bicep.Core.Emit;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TemplateHelper
    {
        public static TemplateDeploymentScope GetDeploymentScope(string templateSchema)
            => templateSchema switch {
                "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#" => TemplateDeploymentScope.Tenant,
                "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#" => TemplateDeploymentScope.ManagementGroup,
                "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#" => TemplateDeploymentScope.Subscription,
                "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#" => TemplateDeploymentScope.ResourceGroup,
                _ => throw new InvalidOperationException($"Unrecognized schema: {templateSchema}"),
            };


        /// <summary>
        /// Validates an ARM template string with the Deployment engine code, and checks the template hash is correctly set.
        /// </summary>
        public static void TemplateShouldBeValid(string templateString)
        {
            Template? template = null;
            FluentActions.Invoking(() => template = TemplateEngine.ParseTemplate(templateString))
                .Should().NotThrow("template can be parsed successfully");

            var templateJtoken = template!.ToJToken();
            var validationApiVersion = TemplateWriter.NestedDeploymentResourceApiVersion;
            var deploymentScope = GetDeploymentScope(template.Schema.Value);

            FluentActions.Invoking(() => TemplateEngine.ValidateTemplate(template, validationApiVersion, deploymentScope))
                .Should().NotThrow("template can be validated successfully");

            var embeddedHash = template.Metadata["_generator"].Value["templateHash"]!.ToString();
            TemplateHelpers.ComputeTemplateHash(templateJtoken).Should().Be(embeddedHash);
        }
    }
}