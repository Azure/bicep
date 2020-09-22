// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.TypeSystem
{
    public sealed class TypeValidationVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly ITypeManager typeManager;
        private readonly IList<Diagnostic> diagnostics;
        private readonly IDictionary<SyntaxBase, ImmutableArray<Diagnostic>> syntaxDiagnostics;

        public TypeValidationVisitor(IReadOnlyDictionary<SyntaxBase, Symbol> bindings, ITypeManager typeManager, IList<Diagnostic> diagnostics)
        {
            this.bindings = bindings;
            this.typeManager = typeManager;
            this.diagnostics = diagnostics;
            this.syntaxDiagnostics = new Dictionary<SyntaxBase, ImmutableArray<Diagnostic>>();
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);
            diagnostics.AddRange(syntaxDiagnostics.Values.SelectMany(x => x));
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            var diagnostics = this.ValidateIdentifierAccess(syntax);

            var assignedType = typeManager.GetTypeInfo(syntax);

            switch (syntax.Modifier)
            {
                case ParameterDefaultValueSyntax defaultValueSyntax:
                    diagnostics = diagnostics.Concat(ValidateDefaultValue(defaultValueSyntax, assignedType));
                    break;

                case ObjectSyntax modifierSyntax:
                    if (assignedType.TypeKind != TypeKind.Error && SyntaxHelper.TryGetPrimitiveType(syntax) is PrimitiveType primitiveType)
                    {
                        var modifierType = LanguageConstants.CreateParameterModifierType(primitiveType, assignedType);
                        diagnostics = diagnostics.Concat(TypeValidator.GetExpressionAssignmentDiagnostics(typeManager, modifierSyntax, modifierType));
                    }
                    break;
            }

            syntaxDiagnostics[syntax] = diagnostics.ToImmutableArray();
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {

        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            var assignedType = typeManager.GetTypeInfo(syntax);
            
            var diagnostics = TypeValidator.GetExpressionAssignmentDiagnostics(typeManager, syntax.Body, assignedType);

            syntaxDiagnostics[syntax] = diagnostics.ToImmutableArray();
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            var diagnostics = GetOutputDeclarationDiagnostics(syntax);

            syntaxDiagnostics[syntax] = diagnostics.ToImmutableArray();
        }

        private IEnumerable<Diagnostic> GetOutputDeclarationDiagnostics(OutputDeclarationSyntax syntax)
        {
            var assignedType = typeManager.GetTypeInfo(syntax);
            var valueType = typeManager.GetTypeInfo(syntax.Value);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (valueType is ErrorTypeSymbol)
            {
                return valueType.GetDiagnostics();
            }

            if (TypeValidator.AreTypesAssignable(valueType, assignedType) == false)
            {
                return DiagnosticBuilder.ForPosition(syntax.Value).OutputTypeMismatch(assignedType.Name, valueType.Name).AsEnumerable();
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ValidateDefaultValue(ParameterDefaultValueSyntax defaultValueSyntax, TypeSymbol assignedType)
        {
            // figure out type of the default value
            var defaultValueType = typeManager.GetTypeInfo(defaultValueSyntax.DefaultValue);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (defaultValueType is ErrorTypeSymbol)
            {
                return defaultValueType.GetDiagnostics();
            }

            if (TypeValidator.AreTypesAssignable(defaultValueType, assignedType) == false)
            {
                return DiagnosticBuilder.ForPosition(defaultValueSyntax.DefaultValue).ParameterTypeMismatch(assignedType.Name, defaultValueType.Name).AsEnumerable();
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ValidateIdentifierAccess(ParameterDeclarationSyntax syntax)
        {
            return SyntaxAggregator.Aggregate(syntax, new List<Diagnostic>(), (accumulated, current) =>
                {
                    if (current is VariableAccessSyntax)
                    {
                        var symbol = bindings[current];
                        
                        // excluded symbol kinds already generate errors - no need to duplicate
                        if (symbol.Kind != SymbolKind.Error &&
                            symbol.Kind != SymbolKind.Parameter &&
                            symbol.Kind != SymbolKind.Function &&
                            symbol.Kind != SymbolKind.Output)
                        {
                            accumulated.Add(DiagnosticBuilder.ForPosition(current).CannotReferenceSymbolInParamDefaultValue());
                        }
                    }

                    return accumulated;
                },
                accumulated => accumulated);
        }
    }
}