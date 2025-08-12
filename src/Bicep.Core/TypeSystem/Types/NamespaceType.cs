// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.TypeSystem.Types
{
    public record NamespaceSettings(
        bool IsSingleton,
        string BicepExtensionName,
        ObjectLikeType? ConfigurationType,
        string TemplateExtensionName,
        string TemplateExtensionVersion);

    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(
            string aliasName,
            NamespaceSettings settings,
            IEnumerable<NamedTypeProperty> properties,
            IEnumerable<FunctionOverload> functionOverloads,
            IEnumerable<BannedFunction> bannedFunctions,
            IEnumerable<Decorator> decorators,
            IResourceTypeProvider resourceTypeProvider,
            IExtensionArtifactReference? extensionArtifactReference = null)
            : base(aliasName, TypeSymbolValidationFlags.PreventAssignment, properties, null, obj => new FunctionResolver(obj, functionOverloads, bannedFunctions))
        {
            Settings = settings;
            ResourceTypeProvider = resourceTypeProvider;
            ExtensionArtifactReference = extensionArtifactReference;
            DecoratorResolver = new DecoratorResolver(this, decorators);
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public DecoratorResolver DecoratorResolver { get; }

        public NamespaceSettings Settings { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public IExtensionArtifactReference? ExtensionArtifactReference { get; }

        public string ExtensionName => Settings.BicepExtensionName;

        public string ExtensionVersion => Settings.TemplateExtensionVersion;

        public ObjectLikeType? ConfigurationType => Settings.ConfigurationType;
    }
}
