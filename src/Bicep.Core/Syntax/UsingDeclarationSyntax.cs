// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class UsingDeclarationSyntax : StatementSyntax
    {
        public UsingDeclarationSyntax(Token keyword, SyntaxBase path)
            : base(Enumerable.Empty<SyntaxBase>())
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.UsingKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Path = path;

        }

        public Token Keyword { get; }

        public SyntaxBase Path { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitUsingDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Path);

        public StringSyntax? TryGetPath() => Path as StringSyntax;
    }
}
