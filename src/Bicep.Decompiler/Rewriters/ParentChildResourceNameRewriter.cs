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

        public SyntaxBase? TryGetReplacementChildName(StringSyntax childName, SyntaxBase parentName, ResourceSymbol parentResourceSymbol)
        {
            switch (parentName)
            {
                case VariableAccessSyntax parentVarAccess:
                {
                    if (childName.Expressions.FirstOrDefault() is not VariableAccessSyntax childVarAccess ||
                        semanticModel.GetSymbolInfo(parentVarAccess) != semanticModel.GetSymbolInfo(childVarAccess))
                    {
                        return null;
                    }

                    if (!childName.SegmentValues[1].StartsWith("/"))
                    {
                        return null;
                    }

                    var newName = SyntaxFactory.CreateString(
                        new [] { childName.SegmentValues[1].Substring(1) }.Concat(childName.SegmentValues.Skip(2)),
                        childName.Expressions.Skip(1));

                    return newName;
                }
                case StringSyntax parentString:
                {
                    if (TryGetReplacementStringSyntax(parentString, childName, parentResourceSymbol) is not {} newName)
                    {
                        return null;
                    }

                    return newName;
                }
            }

            return null;
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

            foreach (var otherResource in semanticModel.AllResources)
            {
                var otherResourceSymbol = otherResource.Symbol;

                if (otherResourceSymbol.Type is not ResourceType otherResourceType ||
                    otherResourceType.TypeReference.Types.Length != resourceType.TypeReference.Types.Length - 1 ||
                    !resourceType.TypeReference.TypesString.StartsWith($"{otherResourceType.TypeReference.TypesString}/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // The other resource is a parent type to this one. check if we can refactor the name.
                if (otherResourceSymbol.DeclaringResource.TryGetBody() is not ObjectSyntax otherResourceBody ||
                    otherResourceBody.SafeGetPropertyByName("name") is not ObjectPropertySyntax otherResourceNameProp)
                {
                    continue;
                }

                if (TryGetReplacementChildName(resourceName, otherResourceNameProp.Value, otherResourceSymbol) is not {} newName)
                {
                    continue;
                }

                var replacementNameProp = new ObjectPropertySyntax(resourceNameProp.Key, resourceNameProp.Colon, newName);
                var parentProp = new ObjectPropertySyntax(
                    SyntaxFactory.CreateIdentifier(LanguageConstants.ResourceParentPropertyName),
                    SyntaxFactory.ColonToken,
                    SyntaxFactory.CreateVariableAccess(otherResourceSymbol.Name));

                var replacementBody = new ObjectSyntax(
                    resourceBody.OpenBrace,
                    // parent prop comes first!
                    parentProp.AsEnumerable().Concat(resourceBody.Children.Replace(resourceNameProp, replacementNameProp)),
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

        public SyntaxBase? TryGetReplacementStringSyntax(StringSyntax parent, StringSyntax child, ResourceSymbol parentResourceSymbol)
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

            var newNameValues = new [] { finalSegmentSuffix.Substring(1) }.Concat(child.SegmentValues.Skip(finalIndex + 1)).ToArray();
            var newExpressions = child.Expressions.Skip(finalIndex).ToArray();

            if (newNameValues.Length == 2 && newNameValues[0] == "" && newNameValues[1] == "")
            {
                // return "expr" rather than "'${expr}'"
                return newExpressions[0];
            }

            return SyntaxFactory.CreateString(newNameValues, newExpressions);
        }
    }
}