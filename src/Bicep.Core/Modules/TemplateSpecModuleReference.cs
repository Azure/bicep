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

        private static readonly Regex ResourceNameRegex = new(@"^[-\w\.\(\)]{0,89}[-\w\(\)]$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private TemplateSpecModuleReference(string subscriptionId, string resourceGroupName, string templateSpecName, string version)
            : base(ModuleReferenceSchemes.TemplateSpecs)
        {
            this.SubscriptionId = subscriptionId;
            this.ResourceGroupName = resourceGroupName;
            this.TemplateSpecName = templateSpecName;
            this.Version = version;
        }

        public override string UnqualifiedReference => $"{this.SubscriptionId}/{this.ResourceGroupName}/{this.TemplateSpecName}:{this.Version}";

        public string SubscriptionId { get; }

        public string ResourceGroupName { get; }

        public string TemplateSpecName { get; }

        public string Version { get; }

        public string TemplateSpecResourceId => $"/subscriptions/{this.SubscriptionId}/resourceGroups/{this.ResourceGroupName}/providers/Microsoft.Resources/templateSpecs/{this.TemplateSpecName}/versions/{this.Version}";

        public override bool IsExternal => true;

        public override bool Equals(object? obj) =>
            obj is TemplateSpecModuleReference other &&
            StringComparer.OrdinalIgnoreCase.Equals(this.SubscriptionId, other.SubscriptionId) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.ResourceGroupName, other.ResourceGroupName) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.TemplateSpecName, other.TemplateSpecName) &&
            StringComparer.OrdinalIgnoreCase.Equals(this.Version, other.Version);

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.SubscriptionId, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.ResourceGroupName, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.TemplateSpecName, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.Version, StringComparer.OrdinalIgnoreCase);

            return hash.ToHashCode();
        }

        public static TemplateSpecModuleReference? TryParse(string? aliasName, string referenceValue, RootConfiguration configuration, out DiagnosticBuilder.ErrorBuilderDelegate? errorBuilder)
        {
            if (aliasName is not null)
            {
                if (configuration.ModuleAliases.TryGetTemplateSpecModuleAlias(aliasName, out errorBuilder) is not { } alias)
                {
                    return null;
                }

                referenceValue = $"{alias}/{referenceValue}";
            }

            if (TemplateSpecUriTemplate.GetTemplateMatch(referenceValue) is not { } match)
            {
                errorBuilder = x => x.InvalidTemplateSpecReference(aliasName, FullyQualify(referenceValue));
                return null;
            }

            string subscriptionId = GetBoundVariable(match, nameof(subscriptionId));
            string resourceGroupName = GetBoundVariable(match, nameof(resourceGroupName));
            string templateSpecName = GetBoundVariable(match, nameof(templateSpecName));
            string version = GetBoundVariable(match, nameof(version));

            // Validate subscription ID.
            if (!Guid.TryParse(subscriptionId, out _))
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidSubscirptionId(aliasName, subscriptionId, FullyQualify(referenceValue));
                return null;
            }

            // Validate resource group name.
            if (resourceGroupName.Length > ResourceNameMaximumLength)
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceResourceGroupNameTooLong(aliasName, resourceGroupName, FullyQualify(referenceValue), ResourceNameMaximumLength);
                return null;
            }

            if (resourceGroupName.Length == 0 ||
                resourceGroupName[^1] == '.' ||
                resourceGroupName.Where(c => !char.IsLetterOrDigit(c) && !ResourceGroupNameAllowedCharacterSet.Contains(c)).Any())
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidResourceGroupName(aliasName, resourceGroupName, FullyQualify(referenceValue));
                return null;
            }

            // Validate template spec name.
            if (templateSpecName.Length > ResourceNameMaximumLength)
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceTemplateSpecNameTooLong(aliasName, templateSpecName, FullyQualify(referenceValue), ResourceNameMaximumLength);
                return null;
            }

            if (!ResourceNameRegex.IsMatch(templateSpecName))
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidTemplateSpecName(aliasName, templateSpecName, FullyQualify(referenceValue));
                return null;
            }

            // Validate template spec version.
            if (version.Length > ResourceNameMaximumLength)
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceTemplateSpecVersionTooLong(aliasName, version, FullyQualify(referenceValue), ResourceNameMaximumLength);
                return null;
            }

            if (!ResourceNameRegex.IsMatch(version))
            {
                errorBuilder = x => x.InvalidTemplateSpecReferenceInvalidTemplateSpecVersion(aliasName, version, FullyQualify(referenceValue));
                return null;
            }

            errorBuilder = null;
            return new(subscriptionId, resourceGroupName, templateSpecName, version);
        }
        private static string FullyQualify(string referenceValue) => $"{ModuleReferenceSchemes.TemplateSpecs}:{referenceValue}";

        private static string GetBoundVariable(UriTemplateMatch match, string variableName) =>
            match.BoundVariables[variableName] ?? throw new InvalidOperationException($"Could not get bound variable \"{variableName}\".");
    }
}
