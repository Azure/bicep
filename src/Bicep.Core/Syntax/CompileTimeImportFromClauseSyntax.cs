// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class CompileTimeImportFromClauseSyntax : SyntaxBase
{
    public CompileTimeImportFromClauseSyntax(Token keyword, SyntaxBase path)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.FromKeyword);
        AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

        this.Keyword = keyword;
        this.Path = path;
    }

    public Token Keyword { get; }

    public SyntaxBase Path { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitCompileTimeImportFromClauseSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Keyword, this.Path);
}
