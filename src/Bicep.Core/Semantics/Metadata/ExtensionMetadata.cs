// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Metadata
{
    public record ExtensionMetadata
    {
        public string Alias { get; init; }

        public string Name { get; init; }

        public string Version { get; init; }

        public NamespaceType? NamespaceType { get; init; }

        public ObjectType? UserAssignedDefaultConfigType { get; init; }

        public ObjectLikeType? ConfigAssignmentDeclaredType { get; init; }

        public ExtensionMetadata(string Alias, string Name, string Version, NamespaceType? NamespaceType, ObjectType? UserAssignedDefaultConfigType)
        {
            this.Alias = Alias;
            this.Name = Name;
            this.Version = Version;
            this.NamespaceType = NamespaceType;
            this.UserAssignedDefaultConfigType = UserAssignedDefaultConfigType;

            this.ConfigAssignmentDeclaredType = ConfigType is not null
                ? TypeHelper.CreateExtensionConfigAssignmentType(ConfigType, UserAssignedDefaultConfigType)
                : null;
        }

        public ObjectLikeType? ConfigType => NamespaceType?.ConfigurationType;

        public bool RequiresConfigAssignment => ConfigAssignmentDeclaredType switch
        {
            null => false,
            ObjectType assignmentObjType => assignmentObjType.Properties.Any(p => p.Value.Flags.HasFlag(TypePropertyFlags.Required)),
            _ => true
        };
    }
}
