// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public class ModuleType : TypeSymbol, IScopeReference
    {
        public ModuleType(string name, ResourceScope validParentScopes, ITypeReference body)
            : base(name)
        {
            ValidParentScopes = validParentScopes;
            Body = body;
        }

        public override TypeKind TypeKind => TypeKind.Module;

        /// <summary>
        /// Represents the possible scopes that this module type can be deployed at.
        /// Does not account for cross-scope deployment limitations.
        /// </summary>
        public ResourceScope ValidParentScopes { get; }

        public ITypeReference Body { get; }

        public ResourceScope Scope => ResourceScope.Module;

        public static ModuleType? TryUnwrap(TypeSymbol typeSymbol)
            => typeSymbol switch
            {
                ModuleType moduleType => moduleType,
                ArrayType { Item: ModuleType moduleType } => moduleType,
                _ => null
            };

        public TypeSymbol? TryGetParameterType(string propertyName)
            => TryGetNestedBodyPropertyType(LanguageConstants.ModuleParamsPropertyName, propertyName);

        private TypeSymbol? TryGetNestedBodyPropertyType(string bodyPropertyName, string nestedPropertyName)
        {
            if (Body is ObjectType objectType &&
                objectType.Properties.TryGetValue(bodyPropertyName, out var bodyProperty) &&
                bodyProperty.TypeReference.Type is ObjectType bodyPropertyObject &&
                bodyPropertyObject.Properties.TryGetValue(nestedPropertyName, out var property))
            {
                return property.TypeReference.Type;
            }

            return null;
        }

        public TypeSymbol? TryGetOutputType(string outputName)
            => TryGetNestedBodyPropertyType(LanguageConstants.ModuleOutputsPropertyName, outputName);
    }
}
