// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class TestDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax, IArtifactReferenceSyntax
    {
        public TestDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase path, SyntaxBase assignment, SyntaxBase value)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.TestKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertTokenType(keyword, nameof(keyword), TokenType.Identifier);
            AssertSyntaxType(assignment, nameof(assignment), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(assignment as Token, nameof(assignment), TokenType.Assignment);
            AssertSyntaxType(value, nameof(value), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax));

            this.Keyword = keyword;
            this.Name = name;
            this.Path = path;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Path { get; }

        public SyntaxBase Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTestDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Value);

        SyntaxBase IArtifactReferenceSyntax.SourceSyntax => Path;

        public ObjectSyntax? TryGetBody() =>
            this.Value switch
            {
                ObjectSyntax @object => @object,
                SkippedTriviaSyntax => null,

                // blocked by assert in the constructor
                _ => throw new NotImplementedException($"Unexpected type of test value '{this.Value.GetType().Name}'.")
            };

        public ObjectSyntax GetBody() =>
            this.TryGetBody() ?? throw new InvalidOperationException($"A valid test body is not available on this test due to errors. Use {nameof(TryGetBody)}() instead.");

        public ArtifactType GetArtifactType() => ArtifactType.Module;
    }
}
