// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public abstract class NamedDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
{
    protected NamedDeclarationSyntax(Token keyword, IdentifierSyntax name, IEnumerable<SyntaxBase> leadingNodes)
        : base(leadingNodes)
    {
        this.Keyword = keyword;
        this.Name = name;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Name { get; }
}
