// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class TargetScopeSyntax : SyntaxBase, IDeclarationSyntax
    {
        public TargetScopeSyntax(Token keyword, Token assignment, SyntaxBase value)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.TargetScopeKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public Token Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitTargetScopeSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Value);

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