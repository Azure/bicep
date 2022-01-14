// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.K8s;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class K8sNamespaceType
    {
        public const string BuiltInName = "kubernetes";

        private static readonly IResourceTypeProvider TypeProvider = new K8sResourceTypeProvider(new K8sResourceTypeLoader());

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: false,
            BicepProviderName: BuiltInName,
            ConfigurationType: GetConfigurationType(),
            ArmTemplateProviderName: "Kubernetes",
            ArmTemplateProviderVersion: "1.0");

        private static ObjectType GetConfigurationType()
        {
            return new ObjectType("configuration", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("namespace", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("kubeConfig", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("context", LanguageConstants.String),
            }, null);
        }

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
