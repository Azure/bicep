// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public static class SyntaxHelper
    {
        public static IEnumerable<SyntaxBase>? TryGetAllowedItems(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            if (!(parameterDeclarationSyntax.Modifier is ObjectSyntax modifierObject))
            {
                return null;
            }

            var allowedProperty = modifierObject.Properties.SingleOrDefault(p => p.TryGetKeyText() == LanguageConstants.ParameterAllowedPropertyName);
            if (allowedProperty == null)
            {
                return null;
            }

            if (!(allowedProperty.Value is ArraySyntax allowedArraySyntax))
            {
                return null;
            }

            return allowedArraySyntax.Items.Select(i => i.Value);
        }

        public static TypeSymbol? TryGetPrimitiveType(ParameterDeclarationSyntax parameterDeclarationSyntax)
            => LanguageConstants.TryGetDeclarationType(parameterDeclarationSyntax.ParameterType?.TypeName);
    }
}