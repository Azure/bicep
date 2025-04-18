// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
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
            BicepExtensionName: BuiltInName,
            ConfigurationType: GetConfigurationType(),
            TemplateExtensionName: "Kubernetes",
            TemplateExtensionVersion: BuiltInVersion);

        private static ObjectType GetConfigurationType()
        {
            return new ObjectType("configuration", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("namespace", LanguageConstants.String, TypePropertyFlags.Required, "The default Kubernetes namespace to deploy resources to."),
                new NamedTypeProperty("kubeConfig", LanguageConstants.SecureString, TypePropertyFlags.Required, "The Kubernetes configuration file, base-64 encoded."),
                new NamedTypeProperty("context", LanguageConstants.String),
            }, null);
        }

        public static NamespaceType Create(string aliasName, IFeatureProvider features)
        {
            return new NamespaceType(
                aliasName,
                Settings,
                ExtensionNamespaceTypeHelper.GetExtensionNamespaceObjectProperties(Settings, features),
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                TypeProviderLazy.Value);
        }
    }
}
