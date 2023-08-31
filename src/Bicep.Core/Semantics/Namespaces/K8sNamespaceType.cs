// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.K8s;
using System.Collections.Immutable;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class K8sNamespaceType
    {
        public const string BuiltInName = "kubernetes";

        private static readonly IResourceTypeProvider TypeProvider = new K8sResourceTypeProvider(new K8sResourceTypeLoader());

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: GetConfigurationType(),
            ArmTemplateProviderName: "Kubernetes",
            ArmTemplateProviderVersion: "1.0.0");

        private static ObjectType GetConfigurationType()
        {
            return new ObjectType("configuration", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("namespace", LanguageConstants.String, TypePropertyFlags.Required, "The default Kubernetes namespace to deploy resources to."),
                new TypeProperty("kubeConfig", LanguageConstants.String, TypePropertyFlags.Required, "The Kubernetes configuration file, base-64 encoded."),
                new TypeProperty("context", LanguageConstants.String),
            }, null);
        }

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
