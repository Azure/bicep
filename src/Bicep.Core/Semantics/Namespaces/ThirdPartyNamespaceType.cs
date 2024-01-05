// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ThirdPartyNamespaceType
    {
        // harsh - we may not need a built in name or may have to get this from Create function 
        public const string BuiltInName = "thirdparty";

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: null,
            ArmTemplateProviderName: "",
            ArmTemplateProviderVersion: "");

        public static NamespaceType Create(string name, string aliasName, IResourceTypeProvider resourceTypeProvider)
        {
            return new NamespaceType(
                aliasName,
                new NamespaceSettings(
                    IsSingleton: true,
                    BicepProviderName: BuiltInName,
                    ConfigurationType: null,
                    ArmTemplateProviderName: name,
                    ArmTemplateProviderVersion: resourceTypeProvider.Version),
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider);
        }
    }
}
