// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.MicrosoftGraph;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class MicrosoftGraphNamespaceType
    {
        public const string BuiltInName = "microsoftGraph";

        private static readonly Lazy<IResourceTypeProvider> TypeProviderLazy
            = new(() => new MicrosoftGraphResourceTypeProvider(new MicrosoftGraphResourceTypeLoader()));

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
                TypeProviderLazy.Value);
        }
    }
}
