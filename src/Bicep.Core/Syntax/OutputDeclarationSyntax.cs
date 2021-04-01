// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class OutputDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public OutputDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, SyntaxBase assignment, SyntaxBase value)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.OutputKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax), typeof(IdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(TypeSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Type { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitOutputDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, Value);

        public TypeSyntax? OutputType => this.Type as TypeSyntax;

        public TypeSymbol GetDeclaredType()
        {
            // assume "any" type if the output type has parse errors (either missing or skipped)
            var declaredType = this.OutputType == null
                ? LanguageConstants.Any
                : LanguageConstants.TryGetDeclarationType(this.OutputType.TypeName);

            if (declaredType == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidOutputType());
            }

            return declaredType;
        }
    }
}
