// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class WithClauseSyntax : SyntaxBase
    {
        public WithClauseSyntax(Token keyword, SyntaxBase value)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.WithKeyword);
            AssertSyntaxType(value, nameof(value), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Value = value;
        }

        public Token Keyword { get; }

        public SyntaxBase Value { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Value);

        public override void Accept(ISyntaxVisitor visitor) => throw new NotImplementedException();
    }
}
