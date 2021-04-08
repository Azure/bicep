// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Decompiler.Rewriters
{
    // Looks for cases where the child and parent share a common syntax structure for naming, and replaces with a direct reference to the parent instead.
    //
    // As an example, because 'resB' below has its name formatted as '${parentName}/resB', we can replace this with '${resA.name}/resB':
    //   resource resA 'My.Rp/resA@2020-01-01' = {
    //     name: parentName
    //   }
    //   
    //   resource resB 'My.Rp/resA/childB@2020-01-01' = {
    //     name: '${parentName}/resB'
    //     dependsOn: [
    //       resA
    //     ]
    //   }
    public class ParentChildResourceNameRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel semanticModel;

        public ParentChildResourceNameRewriter(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        protected override SyntaxBase ReplaceResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (syntax.TryGetBody() is not ObjectSyntax resourceBody ||
                resourceBody.SafeGetPropertyByName("name") is not ObjectPropertySyntax resourceNameProp ||
                resourceNameProp.Value is not StringSyntax resourceName)
            {
                return syntax;
            }

            if (semanticModel.GetSymbolInfo(syntax) is not ResourceSymbol resourceSymbol ||
                resourceSymbol.Type is not ResourceType resourceType)
            {
                return syntax;
            }

            if (resourceType.TypeReference.Types.Length < 2)
            {
                // we're only looking for child resources here
                return syntax;
            }

            foreach (var otherResourceSymbol in semanticModel.Root.GetAllResourceDeclarations())
            {
                if (otherResourceSymbol.Type is not ResourceType otherResourceType ||
                    otherResourceType.TypeReference.Types.Length != resourceType.TypeReference.Types.Length - 1 ||
                    !resourceType.TypeReference.TypesString.StartsWith(otherResourceType.TypeReference.TypesString, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // The other resource is a parent type to this one. check if we can refactor the name.
                if (otherResourceSymbol.DeclaringResource.TryGetBody() is not ObjectSyntax otherResourceBody ||
                    otherResourceBody.SafeGetPropertyByName("name") is not ObjectPropertySyntax otherResourceNameProp)
                {
                    continue;
                }

                StringSyntax replacementStringSyntax;
                if (otherResourceNameProp.Value is StringSyntax otherResourceName)
                {
                    var newStringSyntax = TryGetReplacementStringSyntax(otherResourceName, resourceName, otherResourceSymbol);
                    if (newStringSyntax == null)
                    {
                        continue;
                    }

                    replacementStringSyntax = newStringSyntax;
                }
                else if (otherResourceNameProp.Value is VariableAccessSyntax parentVarAccess &&
                    resourceName.Expressions.FirstOrDefault() is VariableAccessSyntax childVarAccess)
                {
                    if (semanticModel.GetSymbolInfo(parentVarAccess) != semanticModel.GetSymbolInfo(childVarAccess))
                    {
                        continue;
                    }

                    var otherResourceIdentifier = new Token(TokenType.Identifier, new TextSpan(0, 0), otherResourceSymbol.Name, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());
                    var nameProperty = new Token(TokenType.Identifier, new TextSpan(0, 0), "name", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

                    var replacementExpression = new PropertyAccessSyntax(
                        new VariableAccessSyntax(new IdentifierSyntax(otherResourceIdentifier)),
                        new Token(TokenType.Dot, new TextSpan(0, 0), ".", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>()),
                        new IdentifierSyntax(nameProperty));

                    replacementStringSyntax = new StringSyntax(
                        resourceName.StringTokens,
                        replacementExpression.AsEnumerable().Concat(resourceName.Expressions.Skip(1)),
                        resourceName.SegmentValues);
                }
                else
                {
                    continue;
                }

                var replacementNameProp = new ObjectPropertySyntax(
                    resourceNameProp.Key,
                    resourceNameProp.Colon,
                    replacementStringSyntax);

                var replacementBody = new ObjectSyntax(
                    resourceBody.OpenBrace,
                    resourceBody.Children.Replace(resourceNameProp, replacementNameProp),
                    resourceBody.CloseBrace);

                // at the top we just checked if there is a legitimate body
                // but to do the replacement correctly we may need to wrap it inside an IfConditionSyntax
                SyntaxBase replacementValue = syntax.Value switch
                {
                    ObjectSyntax => replacementBody,
                    IfConditionSyntax ifCondition => new IfConditionSyntax(ifCondition.Keyword, ifCondition.ConditionExpression, replacementBody),

                    // should not be possible
                    _ => throw new NotImplementedException($"Unexpected resource value type '{syntax.Value.GetType().Name}'.")
                };

                return new ResourceDeclarationSyntax(
                    syntax.LeadingNodes,
                    syntax.Keyword,
                    syntax.Name,
                    syntax.Type,
                    syntax.ExistingKeyword,
                    syntax.Assignment,
                    replacementValue);
            }

            return syntax;
        }

        public StringSyntax? TryGetReplacementStringSyntax(StringSyntax parent, StringSyntax child, ResourceSymbol parentResourceSymbol)
        {
            if (parent.SegmentValues.Length > child.SegmentValues.Length ||
                parent.Expressions.Length > child.Expressions.Length)
            {
                return null;
            }

            for (var i = 0; i < parent.Expressions.Length; i++)
            {
                var childSymbol = semanticModel.GetSymbolInfo(child.Expressions[i]);
                var parentSymbol = semanticModel.GetSymbolInfo(parent.Expressions[i]);

                if (childSymbol == null || childSymbol != parentSymbol)
                {
                    return null;
                }
            }

            for (var i = 0; i < parent.SegmentValues.Length - 1; i++)
            {
                if (child.SegmentValues[i] != parent.SegmentValues[i])
                {
                    return null;
                }
            }

            var finalIndex = parent.SegmentValues.Length - 1;
            if (!child.SegmentValues[finalIndex].StartsWith(parent.SegmentValues[finalIndex], StringComparison.Ordinal))
            {
                return null;
            }

            var finalSegmentSuffix = child.SegmentValues[finalIndex].Substring(parent.SegmentValues[finalIndex].Length);
            if (finalSegmentSuffix.Length == 0 || finalSegmentSuffix[0] != '/')
            {
                return null;
            }

            var parentResourceIdentifier = new Token(TokenType.Identifier, new TextSpan(0, 0), parentResourceSymbol.Name, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());
            var nameProperty = new Token(TokenType.Identifier, new TextSpan(0, 0), "name", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());
            var replacementExpression = new PropertyAccessSyntax(
                new VariableAccessSyntax(new IdentifierSyntax(parentResourceIdentifier)),
                new Token(TokenType.Dot, new TextSpan(0, 0), ".", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>()),
                new IdentifierSyntax(nameProperty));

            var leftStringToken = new Token(TokenType.StringLeftPiece, new TextSpan(0, 0), "'${", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());
            var nextStringToken = parent.SegmentValues.Length == child.SegmentValues.Length ? 
                new Token(TokenType.StringRightPiece, new TextSpan(0, 0), StringUtils.EscapeBicepString(finalSegmentSuffix, "}", "'"), Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>()) :
                new Token(TokenType.StringMiddlePiece, new TextSpan(0, 0), StringUtils.EscapeBicepString(finalSegmentSuffix, "}", "${"), Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());
            return new StringSyntax(
                new [] { leftStringToken, nextStringToken }.Concat(child.StringTokens.Skip(parent.StringTokens.Length)),
                replacementExpression.AsEnumerable().Concat(child.Expressions.Skip(parent.Expressions.Length)),
                new [] { "", finalSegmentSuffix }.Concat(child.SegmentValues.Skip(parent.SegmentValues.Length)));
        }
    }
}