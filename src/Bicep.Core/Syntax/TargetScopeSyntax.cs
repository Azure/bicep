// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class TargetScopeSyntax : StatementSyntax, ITopLevelDeclarationSyntax
    {
        public TargetScopeSyntax(Token keyword, SyntaxBase assignment, SyntaxBase value)
            : base(ImmutableArray<SyntaxBase>.Empty)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.TargetScopeKeyword);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Assignment = assignment;
            this.Value = value;
        }

        public TargetScopeSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase assignment, SyntaxBase value)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.TargetScopeKeyword);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTargetScopeSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Value);

        public TypeSymbol GetDeclaredType()
        {
            // TODO: Implement the ability to declare a file targeting multiple scopes
            return UnionType.Create(
                new StringLiteralType(LanguageConstants.TargetScopeTypeTenant),
                new StringLiteralType(LanguageConstants.TargetScopeTypeManagementGroup),
                new StringLiteralType(LanguageConstants.TargetScopeTypeSubscription),
                new StringLiteralType(LanguageConstants.TargetScopeTypeResourceGroup));
        }
    }
}