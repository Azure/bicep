// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
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

        public static TypeSymbol GetDeclaredType(IFeatureProvider features)
        {
            List<string> scopes = [
                LanguageConstants.TargetScopeTypeTenant,
                LanguageConstants.TargetScopeTypeManagementGroup,
                LanguageConstants.TargetScopeTypeSubscription,
                LanguageConstants.TargetScopeTypeResourceGroup,
            ];

            // We add the DSC constant here so we can have it as a completion when the feature is enabled.
            if (features.DesiredStateConfigurationEnabled)
            {
                scopes.Add(LanguageConstants.TargetScopeTypeDesiredStateConfiguration);
            }

            if (features.LocalDeployEnabled)
            {
                scopes.Add(LanguageConstants.TargetScopeTypeLocal);
            }

            if (features.OrchestrationEnabled)
            {
                scopes.Add(LanguageConstants.TargetScopeTypeOrchestrator);
            }

            return TypeHelper.CreateTypeUnion(
                scopes.Select(x => TypeFactory.CreateStringLiteralType(x)).ToImmutableArray());
        }
    }
}
