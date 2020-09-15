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

            var allowedProperty = modifierObject.Properties.FirstOrDefault(p => p.GetKeyText() == "allowed");

            if (allowedProperty == null)
            {
                return null;
            }

            return (allowedProperty.Value as ArraySyntax)?.Items.Select(i => i.Value);
        }
    }
}