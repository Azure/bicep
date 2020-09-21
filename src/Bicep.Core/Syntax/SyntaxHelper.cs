// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;

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

            var allowedProperty = modifierObject.Properties.SingleOrDefault(p => p.GetKeyText() == LanguageConstants.ParameterAllowedPropertyName);
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
    }
}