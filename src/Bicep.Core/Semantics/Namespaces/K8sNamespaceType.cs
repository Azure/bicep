// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.K8s;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class K8sNamespaceType
    {
        public const string BuiltInName = "kubernetes";
        public const string BuiltInVersion = "1.0.0";

        private static readonly Lazy<IResourceTypeProvider> TypeProviderLazy
            = new(() => new K8sResourceTypeProvider(new K8sResourceTypeLoader()));

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: GetConfigurationType(),
            ArmTemplateProviderName: "Kubernetes",
            ArmTemplateProviderVersion: BuiltInVersion);

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
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                TypeProviderLazy.Value);
        }
    }
}
