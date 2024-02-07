// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public class TestType(string name, ITypeReference body) : TypeSymbol(name)
    {
        public override TypeKind TypeKind => TypeKind.Test;

        public override TypeSymbolValidationFlags ValidationFlags { get; } = TypeSymbolValidationFlags.PreventAssignment;

        public ITypeReference Body { get; } = body;

        public static TestType? TryUnwrap(TypeSymbol typeSymbol)
            => typeSymbol switch
            {
                TestType testType => testType,
                _ => null
            };

        public TypeSymbol? TryGetParameterType(string propertyName)
        {
            if (Body is ObjectType objectType &&
                objectType.Properties.TryGetValue(LanguageConstants.TestParamsPropertyName, out var paramsProperty) &&
                paramsProperty.TypeReference.Type is ObjectType paramsType &&
                paramsType.Properties.TryGetValue(propertyName, out var property))
            {
                return property.TypeReference.Type;
            }

            return null;
        }
    }
}
