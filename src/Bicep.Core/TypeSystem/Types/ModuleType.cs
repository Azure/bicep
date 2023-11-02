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
        {
            if (Body is ObjectType objectType &&
                objectType.Properties.TryGetValue(LanguageConstants.ModuleParamsPropertyName, out var paramsProperty) &&
                paramsProperty.TypeReference.Type is ObjectType paramsType &&
                paramsType.Properties.TryGetValue(propertyName, out var property))
            {
                return property.TypeReference.Type;
            }

            return null;
        }
    }
}
