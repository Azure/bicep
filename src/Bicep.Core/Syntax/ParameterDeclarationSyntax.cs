// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Syntax
{
    public class ParameterDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ParameterDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax name, SyntaxBase type, SyntaxBase? modifier)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ParameterKeyword);
            AssertSyntaxType(name, nameof(name), typeof(IdentifierSyntax));
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

        public TypeSymbol GetAssignedType(ITypeManager typeManager, ArraySyntax? allowedSyntax)
        {
            var assignedType = typeManager.GetDeclaredType(this);
            if (assignedType is null)
            {
                // We don't expect this to happen for a parameter.
                return ErrorType.Empty();
            }

            // TODO: remove SyntaxHelper.TryGetAllowedSyntax when we drop parameter modifiers support.
            if (allowedSyntax is not null && !allowedSyntax.Items.Any())
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(allowedSyntax).AllowedMustContainItems());
            }

            var allowedItemTypes = allowedSyntax?.Items.Select(typeManager.GetTypeInfo).ToArray();

            if (TypeValidator.AreTypesAssignable(assignedType, LanguageConstants.String))
            {
                assignedType = UnionIfLiterals<StringLiteralType>(assignedType, assignedType, allowedItemTypes);
            }
            else if (TypeValidator.AreTypesAssignable(assignedType, LanguageConstants.Int))
            {
                assignedType = UnionIfLiterals<IntegerLiteralType>(assignedType, assignedType, allowedItemTypes);
            }
            else if (TypeValidator.AreTypesAssignable(assignedType, LanguageConstants.Bool))
            {
                assignedType = UnionIfLiterals<BooleanLiteralType>(assignedType, assignedType, allowedItemTypes);
            }
            else if (TypeValidator.AreTypesAssignable(assignedType, LanguageConstants.Array) && allowedItemTypes is not null && allowedItemTypes.All(TypeHelper.IsLiteralType))
            {
                // @allowed has special semantics when applied to an array if none of the allowed values are themselves arrays (ARM will permit any array containing
                // a subset of the allowed values). If any of the allowed item types is a tuple, treat @allowed([...]) as supplying a list of allowed values;
                // otherwise, treat it as supplying a list of allowed *item* values.
                if (allowedItemTypes.Any(t => t is TupleType))
                {
                    assignedType = UnionIfLiterals<TupleType>(assignedType, assignedType, allowedItemTypes);
                }
                else
                {
                    assignedType = new TypedArrayType(TypeHelper.CreateTypeUnion(allowedItemTypes), assignedType.ValidationFlags);
                }
            }

            return assignedType;
        }

        private static TypeSymbol UnionIfLiterals<T>(TypeSymbol assignedType, TypeSymbol looseType, IEnumerable<TypeSymbol>? allowedItemTypes)
        {
            if (allowedItemTypes?.All(itemType => itemType is T) is true)
            {
                return TypeHelper.CreateTypeUnion(allowedItemTypes);
            }

            // In order to support assignment for a generic string/bool/int to enum-typed properties (which generally is forbidden),
            // we need to relax the validation for string/bool/int parameters without 'allowed' values specified.
            return looseType;
        }
    }
}
