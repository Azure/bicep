// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ModuleDeclarationSyntax : SyntaxBase, IDeclarationSyntax
    {
        public ModuleDeclarationSyntax(Token keyword, IdentifierSyntax name, SyntaxBase path, SyntaxBase assignment, SyntaxBase body)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ModuleKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Name = name;
            this.Path = path;
            this.Assignment = assignment;
            this.Body = body;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Path { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Body { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitModuleDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(Keyword, Body);

        public StringSyntax? TryGetPath() => Path as StringSyntax;
    }
}
