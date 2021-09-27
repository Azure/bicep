// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;
using Azure.Deployments.Core.Uri;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Modules
{
    public class TemplateSpecModuleReference : ModuleReference
    {
        private static readonly UriTemplate FullyQualifiedTemplateSpecUriTemplate = new("/{endpoint}/{subscriptionId}/{resourceGroupName}/{templateSpecName}:{version}");

        private static readonly UriTemplate DefaultTemplateSpecUriTemplate = new("/{subscriptionId}/{resourceGroupName}/{templateSpecName}:{version}");

        private static readonly Regex ResourceNameRegex = new(@"^[-\w\._\(\)]{0,89}[-\w_\(\)]$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private TemplateSpecModuleReference(string? endpoint, string subscriptionId, string resourceGroupName, string templateSpecName, string version)
            : base(ModuleReferenceSchemes.TemplateSpecs)
        {
            this.Endpoint = endpoint;
            this.SubscriptionId = subscriptionId;
            this.ResourceGroupName = resourceGroupName;
            this.TemplateSpecName = templateSpecName;
            this.Version = version;
        }

        public override string UnqualifiedReference => this.Endpoint is not null
            ? $"{this.Endpoint}/{this.SubscriptionId}/{this.ResourceGroupName}/{this.TemplateSpecName}:{this.Version}"
            : $"{this.SubscriptionId}/{this.ResourceGroupName}/{this.TemplateSpecName}:{this.Version}";

        public string? Endpoint { get; }

        public string SubscriptionId { get; }

        public string ResourceGroupName { get; }

        public string TemplateSpecName { get; }

        public string Version { get; }

        public Uri? EndpointUri => this.Endpoint is not null ? new Uri($"https://{this.Endpoint}") : null;

        public string TemplateSpecResourceId => $"/subscriptions/{this.SubscriptionId}/resourceGroups/{this.ResourceGroupName}/providers/Microsoft.Resources/templateSpecs/{this.TemplateSpecName}/versions/{this.Version}";

        public override bool IsExternal => true;

        public override bool Equals(object? obj) =>
            obj is TemplateSpecModuleReference other &&
            StringComparer.OrdinalIgnoreCase.Equals(this.Endpoint, other.Endpoint) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.SubscriptionId, other.SubscriptionId) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.ResourceGroupName, other.ResourceGroupName) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.TemplateSpecName, other.TemplateSpecName) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.Version, other.Version);

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.Endpoint, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.SubscriptionId, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.ResourceGroupName, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.TemplateSpecName, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.Version, StringComparer.OrdinalIgnoreCase);

            return hash.ToHashCode();
        }

        public static TemplateSpecModuleReference? TryParse(string value, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            static DiagnosticBuilder.ErrorBuilderDelegate CreateErrorFunc(string rawValue) => x => x.InvalidTemplateSpecReference($"ts:{rawValue}");

            string? endpoint = null;
            string subscriptionId = "";
            string resourceGroupName = "";
            string templateSpecName = "";
            string templateSpecVersion = "";

            // Parse the template spec URI and validate each part.
            if (DefaultTemplateSpecUriTemplate.GetTemplateMatch(value) is { } defaultMatch)
            {
                subscriptionId = defaultMatch.BoundVariables["subscriptionId"];
                resourceGroupName = defaultMatch.BoundVariables["resourceGroupName"];
                templateSpecName = defaultMatch.BoundVariables["templateSpecName"];
                templateSpecVersion = defaultMatch.BoundVariables["version"];
            }
            else if (FullyQualifiedTemplateSpecUriTemplate.GetTemplateMatch(value) is { } fullyQualifiedMatch)
            {
                endpoint = fullyQualifiedMatch.BoundVariables["endpoint"];
                subscriptionId = fullyQualifiedMatch.BoundVariables["subscriptionId"];
                resourceGroupName = fullyQualifiedMatch.BoundVariables["resourceGroupName"];
                templateSpecName = fullyQualifiedMatch.BoundVariables["templateSpecName"];
                templateSpecVersion = fullyQualifiedMatch.BoundVariables["version"];
            }
            else
            {
                failureBuilder = CreateErrorFunc(value);
                return null;
            }

            if ((endpoint is not null && !Uri.TryCreate($"https://{endpoint}", UriKind.Absolute, out _)) ||
                !Guid.TryParse(subscriptionId, out _) ||
                !ResourceNameRegex.IsMatch(resourceGroupName) ||
                !ResourceNameRegex.IsMatch(templateSpecName) ||
                !ResourceNameRegex.IsMatch(templateSpecVersion))
            {
                failureBuilder = CreateErrorFunc(value);
                return null;
            }

            failureBuilder = null;
            return new(endpoint, subscriptionId, resourceGroupName, templateSpecName, templateSpecVersion);
        }
    }
}
