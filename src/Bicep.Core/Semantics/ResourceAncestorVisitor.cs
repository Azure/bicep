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
        private readonly ImmutableDictionary<ResourceMetadata, ResourceAncestor>.Builder ancestry;

        public ResourceAncestorVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.ancestry = ImmutableDictionary.CreateBuilder<ResourceMetadata, ResourceAncestor>();
        }

        public ImmutableDictionary<ResourceMetadata, ResourceAncestor> Ancestry 
            => this.ancestry.ToImmutableDictionary();

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // Skip analysis for ErrorSymbol and similar cases, these are invalid cases, and won't be emitted.
            if (semanticModel.ResourceMetadata.TryLookup(syntax) is not {} resource)
            {
                base.VisitResourceDeclarationSyntax(syntax);
                return;
            }
            
            if (semanticModel.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax) is {} nestedParentSyntax)
            {
                // nested resource parent syntax
                if (semanticModel.ResourceMetadata.TryLookup(nestedParentSyntax) is {} parentResource)
                {
                    this.ancestry.Add(resource, new ResourceAncestor(ResourceAncestorType.Nested, parentResource, null));
                }
            }
            else if (resource.Symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is {} referenceParentSyntax)
            {
                SyntaxBase? indexExpression = null;
                if (referenceParentSyntax is ArrayAccessSyntax arrayAccess)
                {
                    referenceParentSyntax = arrayAccess.BaseExpression;
                    indexExpression = arrayAccess.IndexExpression;
                }

                // parent property reference syntax
                if (semanticModel.ResourceMetadata.TryLookup(referenceParentSyntax) is {} parentResource)
                {
                    this.ancestry.Add(resource, new ResourceAncestor(ResourceAncestorType.ParentProperty, parentResource, indexExpression));
                }
            }

            base.VisitResourceDeclarationSyntax(syntax);
            return;
        }
    }
}