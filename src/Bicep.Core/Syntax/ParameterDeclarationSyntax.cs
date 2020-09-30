// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParameterDeclarationSyntax : SyntaxBase, IDeclarationSyntax
    {
        public ParameterDeclarationSyntax(Token keyword, IdentifierSyntaxBase name, SyntaxBase type, SyntaxBase? modifier)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ParameterKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax), typeof(MalformedIdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(TypeSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(modifier, nameof(modifier), typeof(ParameterDefaultValueSyntax), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));
            
            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.Modifier = modifier;
        }

        public Token Keyword { get; }
        
        public IdentifierSyntaxBase Name { get; }

        public SyntaxBase Type { get; }

        // This is a modifier of the parameter and not a modifier of the type
        public SyntaxBase? Modifier { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitParameterDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(Type, Modifier));

        /// <summary>
        /// Gets the declared type syntax of this parameter declaration. Certain parse errors will cause it to be null.
        /// </summary>
        public TypeSyntax? ParameterType => this.Type as TypeSyntax;
    }
}
