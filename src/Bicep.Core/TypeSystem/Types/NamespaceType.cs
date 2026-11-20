// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Semantics;
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
            ExtensionArtifact? extensionArtifact = null)
            : base(aliasName, TypeSymbolValidationFlags.PreventAssignment, properties, null, obj => new FunctionResolver(obj, functionOverloads, bannedFunctions))
        {
            Settings = settings;
            ResourceTypeProvider = resourceTypeProvider;
            ExtensionArtifact = extensionArtifact;
            DecoratorResolver = new DecoratorResolver(this, decorators);
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public DecoratorResolver DecoratorResolver { get; }

        public NamespaceSettings Settings { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        /// <summary>
        /// Gets the extension artifact the namespace was loaded from, if any.
        /// </summary>
        public ExtensionArtifact? ExtensionArtifact { get; }

        public string ExtensionName => Settings.BicepExtensionName;

        public string ExtensionVersion => Settings.TemplateExtensionVersion;

        public ObjectLikeType? ConfigurationType => Settings.ConfigurationType;
    }
}
