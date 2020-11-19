// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Decompiler.Rewriters
{
    public class TypeCasingFixerRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel.SemanticModel semanticModel;

        public TypeCasingFixerRewriter(SemanticModel.SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        protected override ObjectSyntax ReplaceObjectSyntax(ObjectSyntax syntax)
        {
            var declaredType = semanticModel.GetDeclaredType(syntax);
            if (declaredType is not ObjectType objectType)
            {
                return syntax;
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
                            newKeySyntax = new IdentifierSyntax(new Token(TokenType.Identifier, new TextSpan(0, 0), insensitivePropertyKey, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>()));
                        }
                        else
                        {
                            var stringToken = new Token(TokenType.StringComplete, new TextSpan(0, 0), StringUtils.EscapeBicepString(insensitivePropertyKey), Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());
                            newKeySyntax = new StringSyntax(stringToken.AsEnumerable(), Enumerable.Empty<SyntaxBase>(), insensitivePropertyKey.AsEnumerable());
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
                return syntax;
            }

            return new ObjectSyntax(
                syntax.OpenBrace,
                newChildren,
                syntax.CloseBrace);
        }

        protected override StringSyntax ReplaceStringSyntax(StringSyntax syntax)
        {
            var declaredType = semanticModel.GetDeclaredType(syntax);

            if (semanticModel.GetTypeInfo(syntax) is not StringLiteralType actualType)
            {
                return syntax;
            }

            if (declaredType is null || TypeValidator.AreTypesAssignable(actualType, declaredType))
            {
                return syntax;
            }

            var stringLiteralCandidates = Enumerable.Empty<StringLiteralType>();
            if (declaredType is StringLiteralType stringLiteralType)
            {
                stringLiteralCandidates = stringLiteralType.AsEnumerable();
            }
            else if (declaredType is UnionType unionType && unionType.Members.All(x => x is StringLiteralType))
            {
                stringLiteralCandidates = unionType.Members.OfType<StringLiteralType>();
            }

            var insensitiveMatch = stringLiteralCandidates.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, actualType.Name));
            if (insensitiveMatch == null)
            {
                return syntax;
            }

            var stringToken = new Token(TokenType.StringComplete, new TextSpan(0, 0), insensitiveMatch.Name, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

            return new StringSyntax(stringToken.AsEnumerable(), Enumerable.Empty<SyntaxBase>(), insensitiveMatch.RawStringValue.AsEnumerable());
        }
    }
}