// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Rewriters
{
    // Looks for object properties where type information is available, and the key matches a known property, but the casing is different.
    // This occurs commonly when decompiling from JSON where properties are case insensitive, and avoids generating a .bicep file with lots of warnings that need to be fixed.
    public class TypeCasingFixerRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel semanticModel;

        public TypeCasingFixerRewriter(SemanticModel semanticModel)
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
                    !objectType.Properties.ContainsKey(propertyKey))
                {
                    var insensitivePropertyKey = objectType.Properties.Keys.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x, propertyKey));
                    if (insensitivePropertyKey != null)
                    {
                        SyntaxBase newKeySyntax;
                        if (Regex.IsMatch(insensitivePropertyKey, "^[a-zA-Z][a-zA-Z0-9_]*$"))
                        {
                            newKeySyntax = new IdentifierSyntax(new Token(TokenType.Identifier, TextSpan.TextDocumentStart, insensitivePropertyKey, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>()));
                        }
                        else
                        {
                            newKeySyntax = SyntaxFactory.CreateStringLiteral(insensitivePropertyKey);
                        }

                        newChildren.Add(new ObjectPropertySyntax(
                            newKeySyntax,
                            objectProperty.Colon,
                            Rewrite(objectProperty.Value)));
                        continue;
                    }
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

        protected override SyntaxBase ReplacePropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            var baseType = semanticModel.GetDeclaredType(syntax.BaseExpression);
            if (baseType is not ObjectType objectType)
            {
                return base.ReplacePropertyAccessSyntax(syntax);
            }

            var propertyName = syntax.PropertyName.IdentifierName;
            if (objectType.Properties.ContainsKey(propertyName))
            {
                return base.ReplacePropertyAccessSyntax(syntax);
            }

            var insensitivePropertyName = objectType.Properties.Keys.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x, propertyName));
            if (insensitivePropertyName is null)
            {
                return base.ReplacePropertyAccessSyntax(syntax);
            }

            var propertySyntax = new IdentifierSyntax(new Token(TokenType.Identifier, TextSpan.TextDocumentStart, insensitivePropertyName, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>()));
            return new PropertyAccessSyntax(
                syntax.BaseExpression,
                syntax.Dot,
                propertySyntax);
        }

        protected override SyntaxBase ReplaceStringSyntax(StringSyntax syntax)
        {
            var declaredType = semanticModel.GetDeclaredType(syntax);

            if (semanticModel.GetTypeInfo(syntax) is not StringLiteralType actualType)
            {
                return base.ReplaceStringSyntax(syntax);
            }

            if (declaredType is null || TypeValidator.AreTypesAssignable(actualType, declaredType))
            {
                return base.ReplaceStringSyntax(syntax);
            }

            var stringLiteralCandidates = Enumerable.Empty<StringLiteralType>();
            if (declaredType is StringLiteralType stringLiteralType)
            {
                stringLiteralCandidates = stringLiteralType.AsEnumerable();
            }
            else if (declaredType is UnionType unionType && unionType.Members.All(x => x.Type is StringLiteralType))
            {
                stringLiteralCandidates = unionType.Members.Select(x => (StringLiteralType)x.Type);
            }

            var insensitiveMatch = stringLiteralCandidates.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, actualType.Name));
            if (insensitiveMatch == null)
            {
                return base.ReplaceStringSyntax(syntax);
            }

            return SyntaxFactory.CreateStringLiteral(insensitiveMatch.RawStringValue);
        }
    }
}
