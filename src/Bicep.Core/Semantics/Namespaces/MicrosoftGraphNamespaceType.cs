// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.MicrosoftGraph;
using System.Collections.Immutable;

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
                ImmutableArray<TypeTypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                TypeProvider);
        }
    }
}
