// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class ResourceDeclarationSyntax : SyntaxBase, INamedDeclarationSyntax
    {
        public ResourceDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, SyntaxBase assignment, SyntaxBase? ifCondition, SyntaxBase body)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ResourceKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);
            AssertSyntaxType(ifCondition, nameof(ifCondition), typeof(SkippedTriviaSyntax), typeof(IfConditionSyntax));
            AssertSyntaxType(body, nameof(body), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax));

            this.LeadingNodes = leadingNodes.ToImmutableArray();
            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.IfCondition = ifCondition;
            this.Body = body;
        }

        public ImmutableArray<SyntaxBase> LeadingNodes { get; }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Type { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase? IfCondition { get; }

        public SyntaxBase Body { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitResourceDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, Body);

        public StringSyntax? TypeString => Type as StringSyntax;

        public IEnumerable<DecoratorSyntax> Decorators => this.LeadingNodes.OfType<DecoratorSyntax>();

        public TypeSymbol GetDeclaredType(ResourceScopeType targetScope, IResourceTypeProvider resourceTypeProvider)
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

            var typeReference = ResourceTypeReference.TryParse(stringContent);
            if (typeReference == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidResourceType());
            }

            return resourceTypeProvider.GetType(targetScope, typeReference);
        }
    }
}
