// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Emit;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.Decompiler.Rewriters
{
    public class DependsOnRemovalRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel.SemanticModel semanticModel;

        public DependsOnRemovalRewriter(SemanticModel.SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        protected override ResourceDeclarationSyntax ReplaceResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (syntax.Body is not ObjectSyntax objectSyntax)
            {
                return syntax;
            }

            var dependsOnProperty = objectSyntax.SafeGetPropertyByName("dependsOn");
            if (dependsOnProperty is null)
            {
                return syntax;
            }

            var builtInDependencies = new HashSet<Symbol>();
            foreach (var property in objectSyntax.Properties)
            {
                if (property == dependsOnProperty)
                {
                    continue;
                }

                var dependencies = ResourceDependencyTestVisitor.GetResourceDependencies(semanticModel, property);
                builtInDependencies.UnionWith(dependencies);
            }

            if (dependsOnProperty.Value is not ArraySyntax dependsOnArray)
            {
                return syntax;
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
                return syntax;
            }

            var newChildren = new List<SyntaxBase>();
            foreach (var child in objectSyntax.Children)
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

            return new ResourceDeclarationSyntax(
                syntax.Keyword,
                syntax.Name,
                syntax.Type,
                syntax.Assignment,
                new ObjectSyntax(
                    objectSyntax.OpenBrace,
                    newChildren,
                    objectSyntax.CloseBrace));
        }
    }
}