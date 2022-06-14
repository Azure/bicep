// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using static Bicep.Core.Semantics.ResourceAncestorGraph;

namespace Bicep.Core.Semantics
{
    public sealed class ResourceAncestorVisitor : SyntaxVisitor
    {
        private readonly SemanticModel semanticModel;
        private readonly ImmutableDictionary<DeclaredResourceMetadata, ResourceAncestor>.Builder ancestry;

        public ResourceAncestorVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.ancestry = ImmutableDictionary.CreateBuilder<DeclaredResourceMetadata, ResourceAncestor>();
        }

        public ImmutableDictionary<DeclaredResourceMetadata, ResourceAncestor> Ancestry
            => this.ancestry.ToImmutableDictionary();

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // Skip analysis for ErrorSymbol and similar cases, these are invalid cases, and won't be emitted.
            if (semanticModel.ResourceMetadata.TryLookup(syntax) is not DeclaredResourceMetadata resource)
            {
                base.VisitResourceDeclarationSyntax(syntax);
                return;
            }

            if (semanticModel.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax) is {} nestedParentSyntax)
            {
                // nested resource parent syntax
                if (semanticModel.ResourceMetadata.TryLookup(nestedParentSyntax) is DeclaredResourceMetadata parentResource)
                {
                    this.ancestry.Add(resource, new ResourceAncestor(ResourceAncestorType.Nested, parentResource, null));
                }
            }
            else if (resource.Symbol.TryGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is { } referenceParentSyntax)
            {
                var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(referenceParentSyntax);

                // parent property reference syntax
                //
                // This check is safe because we don't allow resources declared as parameters to be used with the
                // parent property. Resources provided as parameters have an unknown scope.
                if (semanticModel.ResourceMetadata.TryLookup(baseSyntax) is DeclaredResourceMetadata parentResource)
                {
                    this.ancestry.Add(resource, new ResourceAncestor(ResourceAncestorType.ParentProperty, parentResource, indexExpression));
                }
            }

            base.VisitResourceDeclarationSyntax(syntax);
            return;
        }
    }
}
