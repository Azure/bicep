// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.TypeSystem.Types
{
    public record NamespaceSettings(
        bool IsSingleton,
        string BicepProviderName,
        ObjectType? ConfigurationType,
        string ArmTemplateProviderName,
        string ArmTemplateProviderVersion);

    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(
            string aliasName,
            NamespaceSettings settings,
            IEnumerable<TypeProperty> properties,
            IEnumerable<FunctionOverload> functionOverloads,
            IEnumerable<BannedFunction> bannedFunctions,
            IEnumerable<Decorator> decorators,
            IResourceTypeProvider resourceTypeProvider,
            ArtifactReference? artifact = null)
            : base(aliasName, TypeSymbolValidationFlags.PreventAssignment, properties, null, TypePropertyFlags.None, obj => new FunctionResolver(obj, functionOverloads, bannedFunctions))
        {
            Settings = settings;
            ResourceTypeProvider = resourceTypeProvider;
            Artifact = artifact;
            DecoratorResolver = new DecoratorResolver(this, decorators);
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public DecoratorResolver DecoratorResolver { get; }

        public NamespaceSettings Settings { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public ArtifactReference? Artifact { get; }

        public string ProviderName => Settings.BicepProviderName;

        public ObjectType? ConfigurationType => Settings.ConfigurationType;
    }
}
