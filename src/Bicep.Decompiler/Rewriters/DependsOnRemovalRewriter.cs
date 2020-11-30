// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Decompiler.Visitors;

namespace Bicep.Core.Decompiler.Rewriters
{
    public class DependsOnRemovalRewriter : SyntaxRewriteVisitor
    {
        private readonly SemanticModel semanticModel;

        public DependsOnRemovalRewriter(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        private ObjectSyntax? TryGetReplacementBody(SyntaxBase bodySyntax)
        {
            if (bodySyntax is not ObjectSyntax objectSyntax)
            {
                return null;
            }

            var dependsOnProperty = objectSyntax.SafeGetPropertyByName("dependsOn");
            if (dependsOnProperty is null)
            {
                return null;
            }

            var builtInDependencies = new HashSet<Symbol>();
            foreach (var property in objectSyntax.Properties)
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

            return new ObjectSyntax(
                objectSyntax.OpenBrace,
                newChildren,
                objectSyntax.CloseBrace);
        }

        protected override ResourceDeclarationSyntax ReplaceResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            var replacementBody = TryGetReplacementBody(syntax.Body);
            if (replacementBody is null)
            {
                return base.ReplaceResourceDeclarationSyntax(syntax);
            }

            return new ResourceDeclarationSyntax(
                syntax.Keyword,
                syntax.Name,
                syntax.Type,
                syntax.Assignment,
                replacementBody);
        }

        protected override ModuleDeclarationSyntax ReplaceModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            var replacementBody = TryGetReplacementBody(syntax.Body);
            if (replacementBody is null)
            {
                return base.ReplaceModuleDeclarationSyntax(syntax);
            }

            return new ModuleDeclarationSyntax(
                syntax.Keyword,
                syntax.Name,
                syntax.Path,
                syntax.Assignment,
                replacementBody);
        }
    }
}