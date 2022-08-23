// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ParameterAssignmentSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ParameterAssignmentSyntax(Token keyword, IdentifierSyntax name, SyntaxBase assignment, SyntaxBase value)
            : base(Enumerable.Empty<SyntaxBase>())
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ParameterKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));

            this.Keyword = keyword;
            this.Name = name;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitParameterAssignmentSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Value);
    }
}
