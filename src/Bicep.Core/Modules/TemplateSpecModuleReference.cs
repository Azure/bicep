// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Azure.Deployments.Core.Uri;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Modules
{
    public class TemplateSpecModuleReference : ArtifactReference
    {
        private readonly Lazy<TemplateSpecModuleArtifact> templateSpecModuleArtifact;

        private const int ResourceNameMaximumLength = 90;

        private static readonly UriTemplate TemplateSpecUriTemplate = new("{subscriptionId}/{resourceGroupName}/{templateSpecName}:{version}");

        private static readonly HashSet<char> ResourceGroupNameAllowedCharacterSet = new(new[] { '-', '_', '.', '(', ')' });

        private static readonly Regex ResourceNameRegex = new(@"^[-\w\.\(\)]{0,89}[-\w\(\)]$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private TemplateSpecModuleReference(BicepSourceFile referencingFile, string subscriptionId, string resourceGroupName, string templateSpecName, string version)
            : base(referencingFile, ArtifactReferenceSchemes.TemplateSpecs)
        {
            this.SubscriptionId = subscriptionId;
            this.ResourceGroupName = resourceGroupName;
            this.TemplateSpecName = templateSpecName;
            this.Version = version;
            this.templateSpecModuleArtifact = new(() => new(subscriptionId, resourceGroupName, templateSpecName, version, referencingFile.Features.CacheRootDirectory));
        }

        public override string UnqualifiedReference => $"{this.SubscriptionId}/{this.ResourceGroupName}/{this.TemplateSpecName}:{this.Version}";

        public string SubscriptionId { get; }

        public string ResourceGroupName { get; }

        public string TemplateSpecName { get; }

        public string Version { get; }

        public string TemplateSpecResourceId => $"/subscriptions/{this.SubscriptionId}/resourceGroups/{this.ResourceGroupName}/providers/Microsoft.Resources/templateSpecs/{this.TemplateSpecName}/versions/{this.Version}";

        public override bool IsExternal => true;

        public IFileHandle MainTemplateSpecFile => this.templateSpecModuleArtifact.Value.MainTemplateSpecFile;

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

        public static ResultWithDiagnosticBuilder<TemplateSpecModuleReference> TryParse(BicepSourceFile referencingFile, string? aliasName, string referenceValue)
        {
            if (aliasName is not null)
            {
                if (!referencingFile.Configuration.ModuleAliases.TryGetTemplateSpecModuleAlias(aliasName).IsSuccess(out var alias, out var errorBuilder))
                {
                    return new(errorBuilder);
                }

                referenceValue = $"{alias}/{referenceValue}";
            }

            if (TemplateSpecUriTemplate.GetTemplateMatch(referenceValue) is not { } match)
            {
                return new(x => x.InvalidTemplateSpecReference(aliasName, FullyQualify(referenceValue)));
            }

            string subscriptionId = GetBoundVariable(match, nameof(subscriptionId));
            string resourceGroupName = GetBoundVariable(match, nameof(resourceGroupName));
            string templateSpecName = GetBoundVariable(match, nameof(templateSpecName));
            string version = GetBoundVariable(match, nameof(version));

            // Validate subscription ID.
            if (!Guid.TryParse(subscriptionId, out _))
            {
                return new(x => x.InvalidTemplateSpecReferenceInvalidSubscriptionId(aliasName, subscriptionId, FullyQualify(referenceValue)));
            }

            // Validate resource group name.
            if (resourceGroupName.Length > ResourceNameMaximumLength)
            {
                return new(x => x.InvalidTemplateSpecReferenceResourceGroupNameTooLong(aliasName, resourceGroupName, FullyQualify(referenceValue), ResourceNameMaximumLength));
            }

            if (resourceGroupName.Length == 0 ||
                resourceGroupName[^1] == '.' ||
                resourceGroupName.Where(c => !char.IsLetterOrDigit(c) && !ResourceGroupNameAllowedCharacterSet.Contains(c)).Any())
            {
                return new(x => x.InvalidTemplateSpecReferenceInvalidResourceGroupName(aliasName, resourceGroupName, FullyQualify(referenceValue)));
            }

            // Validate template spec name.
            if (templateSpecName.Length > ResourceNameMaximumLength)
            {
                return new(x => x.InvalidTemplateSpecReferenceTemplateSpecNameTooLong(aliasName, templateSpecName, FullyQualify(referenceValue), ResourceNameMaximumLength));
            }

            if (!ResourceNameRegex.IsMatch(templateSpecName))
            {
                return new(x => x.InvalidTemplateSpecReferenceInvalidTemplateSpecName(aliasName, templateSpecName, FullyQualify(referenceValue)));
            }

            // Validate template spec version.
            if (version.Length > ResourceNameMaximumLength)
            {
                return new(x => x.InvalidTemplateSpecReferenceTemplateSpecVersionTooLong(aliasName, version, FullyQualify(referenceValue), ResourceNameMaximumLength));
            }

            if (!ResourceNameRegex.IsMatch(version))
            {
                return new(x => x.InvalidTemplateSpecReferenceInvalidTemplateSpecVersion(aliasName, version, FullyQualify(referenceValue)));
            }

            return new(new TemplateSpecModuleReference(referencingFile, subscriptionId, resourceGroupName, templateSpecName, version));
        }

        public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle() => new(this.MainTemplateSpecFile);

        private static string FullyQualify(string referenceValue) => $"{ArtifactReferenceSchemes.TemplateSpecs}:{referenceValue}";

        private static string GetBoundVariable(UriTemplateMatch match, string variableName) =>
            match.BoundVariables[variableName] ?? throw new InvalidOperationException($"Could not get bound variable \"{variableName}\".");
    }
}
