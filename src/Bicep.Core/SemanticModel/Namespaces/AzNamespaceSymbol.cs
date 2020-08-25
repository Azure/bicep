// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel.Namespaces
{
    public class AzNamespaceSymbol : NamespaceSymbol
    {
        private static readonly ImmutableArray<FunctionOverload> AzOverloads = new[]
        {
            // TODO: Only valid at subscription scope
            // TODO: Add schema for return type
            FunctionOverload.CreateFixed("subscription", LanguageConstants.Object),

            // TODO: Only valid at RG scope
            // TODO: Add schema for return type
            FunctionOverload.CreateFixed("resourceGroup", LanguageConstants.Object),

            // TODO: Add schema for return type
            FunctionOverload.CreateFixed("deployment", LanguageConstants.Object),

            // TODO: Add schema for return type
            FunctionOverload.CreateFixed("environment", LanguageConstants.Object),

            // TODO: This is based on docs. Verify
            FunctionOverload.CreateWithVarArgs("resourceId", LanguageConstants.String, 2, LanguageConstants.String),
            FunctionOverload.CreateWithVarArgs("subscriptionResourceId", LanguageConstants.String, 2, LanguageConstants.String),
            FunctionOverload.CreateWithVarArgs("tenantResourceId", LanguageConstants.String, 2, LanguageConstants.String),
            FunctionOverload.CreateWithVarArgs("extensionResourceId", LanguageConstants.String, 3, LanguageConstants.String),

            // TODO: Not sure about return type
            new FunctionOverload("providers", LanguageConstants.Array, 1, 2, Enumerable.Repeat(LanguageConstants.String, 2), null),

            // TODO: return type is string[]
            new FunctionOverload("pickZones", LanguageConstants.Array, 3, 5, new[] {LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int}, null),

            // the use of FunctionPlacementConstraints.Resources prevents use of these functions anywhere where they can't be directly inlined into a resource body
            new FunctionOverload("reference", LanguageConstants.Object, 1, 3, Enumerable.Repeat(LanguageConstants.String, 3), null, FunctionFlags.RequiresInlining),
            new FunctionWildcardOverload("list*", LanguageConstants.Any, 2, 3, new[] { LanguageConstants.String, LanguageConstants.String, LanguageConstants.Object }, null, new Regex("^list[a-zA-Z]+"), FunctionFlags.RequiresInlining),
        }.ToImmutableArray();

        public AzNamespaceSymbol() : base("az", AzOverloads)
        {
        }
    }
}

