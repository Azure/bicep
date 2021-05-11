// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Rewriters
{
    // Looks for resources where a dependency can already be inferred by the structure of the resource declaration.
    // 
    // As an example, because the below resource already has a reference to 'otherRes' in the name property, the dependsOn is not adding anything:
    //   resource myRes 'My.Rp/myResource@2020-01-01' = {
    //     name: otherRes.name
    //     dependsOn: [
    //       otherRes
    //     ]
    //   }
    public class DependsOnRemovalRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel semanticModel;

        public DependsOnRemovalRewriter(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        private SyntaxBase? TryGetReplacementValue(SyntaxBase value) =>
            value switch
            {
                ObjectSyntax @object => TryGetReplacementBody(@object),
                IfConditionSyntax ifCondition => TryGetReplacementIfCondition(ifCondition),
                _ => null
            };

        private IfConditionSyntax? TryGetReplacementIfCondition(IfConditionSyntax ifCondition)
        {
            if (ifCondition.Body is not ObjectSyntax @object)
            {
                return null;
            }

            var replacementBody = TryGetReplacementBody(@object);
            if (replacementBody == null)
            {
                return null;
            }

            return new IfConditionSyntax(ifCondition.Keyword, ifCondition.ConditionExpression, replacementBody);
        }

        private ObjectSyntax? TryGetReplacementBody(ObjectSyntax @object)
        {
            var dependsOnProperty = @object.SafeGetPropertyByName("dependsOn");
            if (dependsOnProperty is null)
            {
                return null;
            }

            var builtInDependencies = new HashSet<Symbol>();
            foreach (var property in @object.Properties)
            {
                if (property == dependsOnProperty)
                {
                    continue;
                }

                var dependencies = ResourceDependencyFinderVisitor.GetResourceDependencies(semanticModel, property);
                builtInDependencies.UnionWith(dependencies);
            }

            if (dependsOnProperty.Value is not ArraySyntax dependsOnArray)
            {
                return null;
            }

            var newDependsOnArrayChildren = new List<SyntaxBase>();
            foreach (var child in dependsOnArray.Children)
            {
                if (child is not ArrayItemSyntax childArrayItem)
                {
                    newDependsOnArrayChildren.Add(child);
                    continue;
                }

                if (semanticModel.GetSymbolInfo(childArrayItem.Value) is not Symbol childSymbol)
                {
                    newDependsOnArrayChildren.Add(child);
                    continue;
                }

                if (!builtInDependencies.Contains(childSymbol))
                {
                    newDependsOnArrayChildren.Add(child);
                    continue;
                }
            }

            if (newDependsOnArrayChildren.Count == dependsOnArray.Children.Length)
            {
                return null;
            }

            var newChildren = new List<SyntaxBase>();
            foreach (var child in @object.Children)
            {
                if (child == dependsOnProperty)
                {
                    if (newDependsOnArrayChildren.Any(x => x is ArrayItemSyntax))
                    {
                        newChildren.Add(new ObjectPropertySyntax(
                            dependsOnProperty.Key,
                            dependsOnProperty.Colon,
                            new ArraySyntax(
                                dependsOnArray.OpenBracket,
                                newDependsOnArrayChildren,
                                dependsOnArray.CloseBracket)));
                    }
                    continue;
                }

                newChildren.Add(child);
            }

            return new ObjectSyntax(
                @object.OpenBrace,
                newChildren,
                @object.CloseBrace);
        }

        protected override SyntaxBase ReplaceResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            var replacementValue = TryGetReplacementValue(syntax.Value);
            if (replacementValue is null)
            {
                return base.ReplaceResourceDeclarationSyntax(syntax);
            }

            return new ResourceDeclarationSyntax(
                syntax.LeadingNodes,
                syntax.Keyword,
                syntax.Name,
                syntax.Type,
                syntax.ExistingKeyword,
                syntax.Assignment,
                replacementValue);
        }

        protected override SyntaxBase ReplaceModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            var replacementValue = TryGetReplacementValue(syntax.Value);
            if (replacementValue is null)
            {
                return base.ReplaceModuleDeclarationSyntax(syntax);
            }

            return new ModuleDeclarationSyntax(
                syntax.LeadingNodes,
                syntax.Keyword,
                syntax.Name,
                syntax.Path,
                syntax.Assignment,
                replacementValue);
        }
    }
}
