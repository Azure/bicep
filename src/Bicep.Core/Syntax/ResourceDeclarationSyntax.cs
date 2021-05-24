// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class ResourceDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ResourceDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, Token? existingKeyword, SyntaxBase assignment, SyntaxBase value)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ResourceKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertKeyword(existingKeyword, nameof(existingKeyword), LanguageConstants.ExistingKeyword);
            AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);
            AssertSyntaxType(value, nameof(value), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax), typeof(IfConditionSyntax), typeof(ForSyntax));

            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.ExistingKeyword = existingKeyword;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Type { get; }

        public Token? ExistingKeyword { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitResourceDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Value);

        public StringSyntax? TypeString => Type as StringSyntax;

        public bool IsExistingResource() => ExistingKeyword is not null;

        /// <summary>
        /// Returns the declared type of the resource body (based on the type string).
        /// Returns the same value for single resource or resource loops declarations.
        /// </summary>
        /// <param name="resourceTypeProvider">resource type provider</param>
        public TypeSymbol GetDeclaredType(IBinder binder, IResourceTypeProvider resourceTypeProvider)
        {
            var stringSyntax = this.TypeString;

            if (stringSyntax != null && stringSyntax.IsInterpolated())
            {
                // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                // right now, codegen will still generate a format string however, which will cause problems for the type.
                return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).ResourceTypeInterpolationUnsupported());
            }

            var stringContent = stringSyntax?.TryGetLiteralValue();
            if (stringContent == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidResourceType());
            }

            // Before we parse the type name we need to determine if it's a top level resource or not.
            ResourceTypeReference? typeReference;
            var hasParentDeclaration = false;
            var nestedParents = binder.GetAllAncestors<ResourceDeclarationSyntax>(this);
            bool isTopLevelResourceDeclaration = nestedParents.Length == 0;
            if (isTopLevelResourceDeclaration)
            {
                // This is a top level resource - the type is a fully-qualified type.
                typeReference = ResourceTypeReference.TryParse(stringContent);
                if (typeReference == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidResourceType());
                }

                if (binder.GetSymbolInfo(this) is ResourceSymbol resourceSymbol &&
                    binder.TryGetCycle(resourceSymbol) is null &&
                    resourceSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is {} referenceParentSyntax &&
                    binder.GetSymbolInfo(referenceParentSyntax) is ResourceSymbol parentResourceSymbol)
                {
                    hasParentDeclaration = true;

                    var parentType = parentResourceSymbol.DeclaringResource.GetDeclaredType(binder, resourceTypeProvider);
                    if (parentType is not ResourceType parentResourceType)
                    {
                        // TODO should we raise an error, or just rely on the error on the parent?
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(referenceParentSyntax).ParentResourceTypeHasErrors(parentResourceSymbol.DeclaringResource.Name.IdentifierName));
                    }

                    if (!parentResourceType.TypeReference.IsParentOf(typeReference))
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(referenceParentSyntax).ResourceTypeIsNotValidParent(
                            typeReference.FullyQualifiedType,
                            parentResourceType.TypeReference.FullyQualifiedType));
                    }
                }
            }
            else
            {
                // This is a nested resource, the type name is a compound of all of the ancestor
                // type names.
                //
                // Ex: 'My.Rp/someType@2020-01-01' -> 'someChild' -> 'someGrandchild'

                // The top-most resource must have a qualified type name.
                hasParentDeclaration = true;
                var baseTypeStringContent = nestedParents[0].TypeString?.TryGetLiteralValue();
                if (baseTypeStringContent == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidAncestorResourceType(nestedParents[0].Name.IdentifierName));
                }

                var baseTypeReference = ResourceTypeReference.TryParse(baseTypeStringContent);
                if (baseTypeReference == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidAncestorResourceType(nestedParents[0].Name.IdentifierName));
                }

                // Collect each other ancestor resource's type.
                var typeSegments = new List<string>();
                for (var i = 1; i < nestedParents.Length; i++)
                {
                    var typeSegmentStringContent = nestedParents[i].TypeString?.TryGetLiteralValue();
                    if (typeSegmentStringContent == null)
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidAncestorResourceType(nestedParents[i].Name.IdentifierName));
                    }

                    typeSegments.Add(typeSegmentStringContent);
                }

                // Add *this* resource's type
                typeSegments.Add(stringContent);

                // If this fails, let's walk through and find the root cause. This could be confusing
                // for people seeing it the first time.
                typeReference = ResourceTypeReference.TryCombine(baseTypeReference, typeSegments);
                if (typeReference == null)
                {
                    // We'll special case the last one since it refers to *this* resource. We don't
                    // want to cascade a bunch of noisy errors for parents, they get their own errors.
                    for (var j = 0; j < typeSegments.Count - 1; j++)
                    {
                        if (!ResourceTypeReference.TryParseSingleTypeSegment(typeSegments[j], out _, out _))
                        {
                            return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidAncestorResourceType(nestedParents[j+1].Name.IdentifierName));
                        }
                    }

                    if (!ResourceTypeReference.TryParseSingleTypeSegment(stringContent, out _, out _))
                    {
                        // OK this resource is the one that's wrong.
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidResourceTypeSegment(stringContent));
                    }

                    // Something went wrong, this should be unreachable.
                    throw new InvalidOperationException("Failed to find the root cause of an invalid compound resource type.");
                }
            }

            var flags = ResourceTypeGenerationFlags.None;
            if (IsExistingResource())
            {
                flags |= ResourceTypeGenerationFlags.ExistingResource;
            }

            if(!isTopLevelResourceDeclaration)
            {
                flags |= ResourceTypeGenerationFlags.NestedResource;
            }

            if (typeReference.IsRootType || hasParentDeclaration)
            {
                flags |= ResourceTypeGenerationFlags.PermitLiteralNameProperty;
            }

            return resourceTypeProvider.GetType(typeReference, flags);
        }

        public ObjectSyntax? TryGetBody() =>
            this.Value switch
            {
                ObjectSyntax @object => @object,
                IfConditionSyntax ifCondition => ifCondition.Body as ObjectSyntax,
                ForSyntax @for => @for.Body switch
                {
                    ObjectSyntax @object => @object,
                    IfConditionSyntax ifCondition => ifCondition.Body as ObjectSyntax,
                    SkippedTriviaSyntax => null,

                    _=> throw new NotImplementedException($"Unexpected type of for-expression value '{this.Value.GetType().Name}'.")
                },
                SkippedTriviaSyntax => null,

                // blocked by assert in the constructor
                _ => throw new NotImplementedException($"Unexpected type of resource value '{this.Value.GetType().Name}'.")
            };

        public ObjectSyntax GetBody() =>
            this.TryGetBody() ?? throw new InvalidOperationException($"A valid resource body is not available on this module due to errors. Use {nameof(TryGetBody)}() instead.");
    }
}
