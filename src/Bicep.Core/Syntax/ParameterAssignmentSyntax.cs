// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ParameterAssignmentSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ParameterAssignmentSyntax(Token keyword, IdentifierSyntax name, SyntaxBase? assignment, SyntaxBase? value, IEnumerable<SyntaxBase> leadingNodes)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ParameterKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));

            this.Keyword = keyword;
            this.Name = name;
            if (assignment is not null && value is not null)
            {
                this.AssignmentClause = new AssignmentClauseSyntax(assignment, value);
            }
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public AssignmentClauseSyntax? AssignmentClause { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitParameterAssignmentSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.AssignmentClause?.Value ?? this.Name);
    }

    public class AssignmentClauseSyntax
    {
        public AssignmentClauseSyntax(SyntaxBase assignment, SyntaxBase value)
        {
            this.Assignment = assignment;
            this.Value = value;
        }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }
    }
}
