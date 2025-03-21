// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.MicrosoftGraph;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.SourceGraph;
using static Bicep.Core.TypeSystem.Providers.ThirdParty.ThirdPartyResourceTypeLoader;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class MicrosoftGraphNamespaceType
    {
        public const string BuiltInName = "microsoftGraph";
        public const string TemplateExtensionName = "MicrosoftGraph";
        public const string BicepExtensionBetaName = "MicrosoftGraphBeta";
        public const string BicepExtensionV10Name = "MicrosoftGraphV1.0";

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: false,
            BicepExtensionName: BuiltInName,
            ConfigurationType: null,
            TemplateExtensionName: TemplateExtensionName,
            TemplateExtensionVersion: "1.0.0");

        public static NamespaceType Create(string? aliasName, IResourceTypeProvider resourceTypeProvider, ArtifactReference? artifact)
        {
            if (resourceTypeProvider is MicrosoftGraphResourceTypeProvider microsoftGraphProvider &&
            microsoftGraphProvider.GetNamespaceConfiguration() is NamespaceConfiguration namespaceConfig)
            {
                return new NamespaceType(
                    aliasName ?? namespaceConfig.Name,
                    new NamespaceSettings(
                        IsSingleton: namespaceConfig.IsSingleton,
                        BicepExtensionName: namespaceConfig.Name,
                        ConfigurationType: namespaceConfig.ConfigurationObject,
                        TemplateExtensionName: TemplateExtensionName,
                        TemplateExtensionVersion: namespaceConfig.Version),
                    ImmutableArray<NamedTypeProperty>.Empty,
                    ImmutableArray<FunctionOverload>.Empty,
                    ImmutableArray<BannedFunction>.Empty,
                    ImmutableArray<Decorator>.Empty,
                    resourceTypeProvider,
                    artifact);
            }

            throw new ArgumentException("Invalid resource type provider or namespace config for Microsoft Graph resource.");
        }

        public static bool ShouldUseLoader(string? typeSettingName)
        {
            return typeSettingName == TemplateExtensionName || typeSettingName == BicepExtensionBetaName || typeSettingName == BicepExtensionV10Name;
        }
    }
}
