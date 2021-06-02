// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class ParameterDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ParameterDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, SyntaxBase? modifier)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ParameterKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
            AssertSyntaxType(type, nameof(type), typeof(TypeSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(modifier, nameof(modifier), typeof(ParameterDefaultValueSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Name = name;
            this.Type = type;
            this.Modifier = modifier;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Type { get; }

        // This is a modifier of the parameter and not a modifier of the type
        public SyntaxBase? Modifier { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitParameterDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, TextSpan.LastNonNull(Type, Modifier));

        /// <summary>
        /// Gets the declared type syntax of this parameter declaration. Certain parse errors will cause it to be null.
        /// </summary>
        public TypeSyntax? ParameterType => this.Type as TypeSyntax;

        public TypeSymbol GetDeclaredType()
        {
            // assume "any" type when the parameter has parse errors (either missing or was skipped)
            var declaredType = this.ParameterType == null
                ? LanguageConstants.Any
                : LanguageConstants.TryGetDeclarationType(this.ParameterType.TypeName);

            if (declaredType == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(this.Type).InvalidParameterType());
            }

            return declaredType;
        }

        public TypeSymbol GetAssignedType(ITypeManager typeManager, ArraySyntax? allowedSyntax)
        {
            var assignedType = this.GetDeclaredType();

            // TODO: remove SyntaxHelper.TryGetAllowedSyntax when we drop parameter modifiers support.
            if (allowedSyntax is not null && !allowedSyntax.Items.Any())
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(allowedSyntax).AllowedMustContainItems());
            }

            var allowedItemTypes = allowedSyntax?.Items.Select(typeManager.GetTypeInfo);

            if (ReferenceEquals(assignedType, LanguageConstants.String))
            {
                if (allowedItemTypes?.All(itemType => itemType is StringLiteralType) == true)
                {
                    assignedType = UnionType.Create(allowedItemTypes);
                }
                else
                {
                    // In order to support assignment for a generic string to enum-typed properties (which generally is forbidden),
                    // we need to relax the validation for string parameters without 'allowed' values specified.
                    assignedType = LanguageConstants.LooseString;
                }
            }

            if (ReferenceEquals(assignedType, LanguageConstants.Array) &&
                allowedItemTypes?.All(itemType => itemType is StringLiteralType) == true)
            {
                assignedType = new TypedArrayType(UnionType.Create(allowedItemTypes), TypeSymbolValidationFlags.Default);
            }

            return assignedType;
        }
    }
}
