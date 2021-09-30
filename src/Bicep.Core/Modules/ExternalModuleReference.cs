// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Modules
{
    public abstract class ExternalModuleReference : ModuleReference
    {
        protected delegate ModuleAlias? ModuleAliasSelector(string aliasName, out ErrorBuilderDelegate? errorBuilder);

        protected ExternalModuleReference(string scheme)
            : base(scheme)
        {
        }

        protected static string? TryReplaceModuleAliases(string rawReferenceValue, ModuleAliasSelector aliasSelector, out ErrorBuilderDelegate? errorBuilder)
        {
            var lastColonIndex = rawReferenceValue.LastIndexOf(":");

            if (lastColonIndex <= 0 || lastColonIndex == rawReferenceValue.Length - 1)
            {
                // There's no version in the reference or the version is empty. Return the reference as is and let the caller do validation.
                errorBuilder = null;
                return rawReferenceValue;
            }

            var components = rawReferenceValue.Substring(0, lastColonIndex).Split("$");

            if (components.Length < 2)
            {
                // No module aliases in the reference.
                errorBuilder = null;
                return rawReferenceValue;
            }

            if (components.Length > 2)
            {
                // More than one alias found in the reference.
                errorBuilder = x => x.InvalidModuleReferenceTooManyModuleAliasNames(components.Length);
                return null;
            }

            var aliasName = components[0];

            if (aliasSelector(aliasName, out errorBuilder) is not { } alias)
            {
                return null;
            }

            var versionOrTag = rawReferenceValue[(lastColonIndex + 1)..];

            errorBuilder = null;
            return $"{alias}/{components[1]}:{versionOrTag}";
        }
    }
}
