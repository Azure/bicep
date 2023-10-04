// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.MicrosoftGraph;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class MicrosoftGraphNamespaceType
    {
        public const string BuiltInName = "microsoftGraph";

        private static readonly IResourceTypeProvider TypeProvider = new MicrosoftGraphResourceTypeProvider(new MicrosoftGraphResourceTypeLoader());

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: null,
            ArmTemplateProviderName: "MicrosoftGraph",
            ArmTemplateProviderVersion: "1.0.0");

        public static NamespaceType Create(string aliasName)
        {
            return new NamespaceType(
                aliasName,
                Settings,
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                TypeProvider);
        }
    }
}
