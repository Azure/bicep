// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ExtensionWithClauseSyntax : SyntaxBase
    {
        public ExtensionWithClauseSyntax(Token keyword, SyntaxBase config)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.WithKeyword);
            AssertSyntaxType(config, nameof(config), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Config = config;
        }

        public Token Keyword { get; }

        public SyntaxBase Config { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Config);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitExtensionWithClauseSyntax(this);
    }
}
