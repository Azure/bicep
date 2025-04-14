// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class CompileTimeImportDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
{
    public CompileTimeImportDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase importExpression, SyntaxBase fromClause)
        : base(leadingNodes)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword);
        AssertSyntaxType(importExpression, nameof(importExpression), typeof(ImportedSymbolsListSyntax), typeof(WildcardImportSyntax), typeof(SkippedTriviaSyntax));
        AssertSyntaxType(fromClause, nameof(fromClause), typeof(CompileTimeImportFromClauseSyntax), typeof(SkippedTriviaSyntax));

        this.Keyword = keyword;
        this.ImportExpression = importExpression;
        this.FromClause = fromClause;
    }

    public Token Keyword { get; }

    public SyntaxBase ImportExpression { get; }

    public SyntaxBase FromClause { get; }

    SyntaxBase IArtifactReferenceSyntax.SourceSyntax
        => FromClause is CompileTimeImportFromClauseSyntax fromClauseSyntax ? fromClauseSyntax.Path : FromClause;

    SyntaxBase? IArtifactReferenceSyntax.Path
        => (FromClause as CompileTimeImportFromClauseSyntax)?.Path;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitCompileTimeImportDeclarationSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.FromClause);

    public ArtifactType GetArtifactType() => ArtifactType.Module;
}
