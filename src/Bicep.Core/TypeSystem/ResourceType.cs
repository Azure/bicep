using System.Collections.Generic;

namespace Bicep.Core.TypeSystem
{
    public class ResourceType : NamedObjectType
    {
        public ResourceType(string name, IEnumerable<TypeProperty> properties, TypeSymbol? additionalPropertiesType)
            : base(name, properties, additionalPropertiesType)
        {
        }

        public override TypeKind TypeKind => TypeKind.Resource;
    }
}