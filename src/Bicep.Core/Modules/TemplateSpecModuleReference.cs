// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Azure.Deployments.Core.Uri;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Modules
{
    public class TemplateSpecModuleReference : ModuleReference
    {
        private const int ResourceNameMaximumLength = 90;

        private static readonly UriTemplate TemplateSpecUriTemplate = new("{subscriptionId}/{resourceGroupName}/{templateSpecName}:{version}");

        private static readonly HashSet<char> ResourceGroupNameAllowedCharacterSet = new(new[] { '-', '_', '.', '(', ')' });

        private static readonly Regex ResourceNameRegex = new(@"^[a-zA-Z0-9-_\.\(\)]{0,89}[a-zA-Z0-9-_\(\)]$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private TemplateSpecModuleReference(string endpoint, string subscriptionId, string resourceGroupName, string templateSpecName, string version)
            : base(ModuleReferenceSchemes.TemplateSpecs)
        {
            this.EndpointUri = new Uri(endpoint);
            this.SubscriptionId = subscriptionId;
            this.ResourceGroupName = resourceGroupName;
            this.TemplateSpecName = templateSpecName;
            this.Version = version;
        }

        public override string UnqualifiedReference => $"{this.SubscriptionId}/{this.ResourceGroupName}/{this.TemplateSpecName}:{this.Version}";

        public Uri EndpointUri { get; }

        public string SubscriptionId { get; }

        public string ResourceGroupName { get; }

        public string TemplateSpecName { get; }

        public string Version { get; }

        public string TemplateSpecResourceId => $"/subscriptions/{this.SubscriptionId}/resourceGroups/{this.ResourceGroupName}/providers/Microsoft.Resources/templateSpecs/{this.TemplateSpecName}/versions/{this.Version}";

        public override bool IsExternal => true;

        public override bool Equals(object? obj) =>
            obj is TemplateSpecModuleReference other &&
            StringComparer.OrdinalIgnoreCase.Equals(this.EndpointUri.AbsoluteUri, other.EndpointUri.AbsoluteUri) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.SubscriptionId, other.SubscriptionId) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.ResourceGroupName, other.ResourceGroupName) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.TemplateSpecName, other.TemplateSpecName) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.Version, other.Version);

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.EndpointUri.AbsoluteUri, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.SubscriptionId, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.ResourceGroupName, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.TemplateSpecName, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.Version, StringComparer.OrdinalIgnoreCase);

            return hash.ToHashCode();
        }

        public static TemplateSpecModuleReference? TryParse(string referenceValue, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? errorBuilder)
        {
            if (configuration.Cloud.TryGetCurrentResourceManagerEndpoint(out errorBuilder) is not { } endpoint)
            {
                return null;
            }

            if (TemplateSpecUriTemplate.GetTemplateMatch(referenceValue) is not { } match)
            {
                errorBuilder = x => x.InvalidTemplateSpecReference(FullyQualify(referenceValue));
                return null;
            }

            string subscriptionId = match.BoundVariables["subscriptionId"];
            string resourceGroupName = match.BoundVariables["resourceGroupName"];
            string templateSpecName = match.BoundVariables["templateSpecName"];
            string templateSpecVersion = match.BoundVariables["version"];

            // Validate subscription ID.
            if (!Guid.TryParse(subscriptionId, out _))
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidSubscirptionId(subscriptionId, FullyQualify(referenceValue));
                return null;
            }

            // Validate resource group name.
            if (resourceGroupName.Length > ResourceNameMaximumLength)
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceResourceGroupNameTooLong(resourceGroupName, FullyQualify(referenceValue), ResourceNameMaximumLength);
                return null;
            }

            if (resourceGroupName.Length == 0 ||
                resourceGroupName[^1] == '.' ||
                resourceGroupName.Where(c => !char.IsLetterOrDigit(c) && !ResourceGroupNameAllowedCharacterSet.Contains(c)).Any())
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidResourceGroupName(resourceGroupName, FullyQualify(referenceValue));
                return null;
            }

            // Validate template spec name.
            if (templateSpecName.Length > ResourceNameMaximumLength)
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceTemplateSpecNameTooLong(templateSpecName, FullyQualify(referenceValue), ResourceNameMaximumLength);
                return null;
            }

            if (!ResourceNameRegex.IsMatch(templateSpecName))
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidTemplateSpecName(templateSpecName, FullyQualify(referenceValue));
                return null;
            }

            // Validate template spec version.
            if (templateSpecVersion.Length > ResourceNameMaximumLength)
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceTemplateSpecVersionTooLong(templateSpecVersion, FullyQualify(referenceValue), ResourceNameMaximumLength);
                return null;
            }

            if (!ResourceNameRegex.IsMatch(templateSpecVersion))
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidTemplateSpecVersion(templateSpecVersion, FullyQualify(referenceValue));
                return null;
            }

            errorBuilder = null;
            return new(endpoint, subscriptionId, resourceGroupName, templateSpecName, templateSpecVersion);
        }
        private static string FullyQualify(string referenceValue) => $"{ModuleReferenceSchemes.TemplateSpecs}:{referenceValue}";
    }
}
