// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Rewriters
{
    // Removes any object properties which have been explicitly marked as "ReadOnly"
    public class ReadOnlyPropertyRemovalRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel semanticModel;

        public ReadOnlyPropertyRemovalRewriter(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        protected override SyntaxBase ReplaceObjectSyntax(ObjectSyntax syntax)
        {
            var declaredType = semanticModel.GetDeclaredType(syntax);
            if (declaredType is not ObjectType objectType)
            {
                return base.ReplaceObjectSyntax(syntax);
            }

            var newChildren = new List<SyntaxBase>();
            foreach (var child in syntax.Children)
            {
                if (child is ObjectPropertySyntax objectProperty &&
                    objectProperty.TryGetKeyText() is string propertyKey &&
                    objectType.Properties.TryGetValue(propertyKey) is { } propertyValue &&
                    propertyValue.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                {
                    continue;
                }

                if (child is ObjectPropertySyntax nullProperty &&
                    nullProperty.Value is NullLiteralSyntax)
                {
                    // skip null values
                    continue;
                }

                if (child is Token { Type: TokenType.NewLine } &&
                    newChildren.LastOrDefault() is Token { Type: TokenType.NewLine })
                {
                    // collapse blank lines
                    continue;
                }

                newChildren.Add(Rewrite(child));
            }

            if (Enumerable.SequenceEqual(newChildren, syntax.Children))
            {
                return base.ReplaceObjectSyntax(syntax);
            }

            return new ObjectSyntax(
                syntax.OpenBrace,
                newChildren,
                syntax.CloseBrace);
        }
    }
}
