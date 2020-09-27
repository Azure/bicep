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

        public static IEnumerable<Diagnostic> GetTypeValidationDiagnostics(IReadOnlyDictionary<SyntaxBase, Symbol> bindings, ITypeManager typeManager, ProgramSyntax program)
        {
            var diagnostics = new List<Diagnostic>();

            var visitor = new TypeValidationVisitor(bindings, typeManager, diagnostics);
            visitor.Visit(program);

            return diagnostics;
        }

        private TypeValidationVisitor(IReadOnlyDictionary<SyntaxBase, Symbol> bindings, ITypeManager typeManager, IList<Diagnostic> diagnostics)
        {
            this.bindings = bindings;
            this.typeManager = typeManager;
            this.diagnostics = diagnostics;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            diagnostics.AddRange(this.ValidateIdentifierAccess(syntax));

            var assignedType = typeManager.GetTypeInfo(syntax);

            switch (syntax.Modifier)
            {
                case ParameterDefaultValueSyntax defaultValueSyntax:
                    diagnostics.AddRange(ValidateDefaultValue(defaultValueSyntax, assignedType));
                    break;

                case ObjectSyntax modifierSyntax:
                    if (assignedType.TypeKind != TypeKind.Error && SyntaxHelper.TryGetPrimitiveType(syntax) is PrimitiveType primitiveType)
                    {
                        var modifierType = LanguageConstants.CreateParameterModifierType(primitiveType, assignedType);
                        diagnostics.AddRange(TypeValidator.GetExpressionAssignmentDiagnostics(typeManager, modifierSyntax, modifierType));
                    }
                    break;
            }
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {

        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            var assignedType = typeManager.GetTypeInfo(syntax);
            
            var currentDiagnostics = TypeValidator.GetExpressionAssignmentDiagnostics(typeManager, syntax.Body, assignedType);

            diagnostics.AddRange(currentDiagnostics);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            var currentDiagnostics = GetOutputDeclarationDiagnostics(syntax);

            diagnostics.AddRange(currentDiagnostics);
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
                        
                        // Error: already has error info attached, no need to add more
                        // Parameter: references are permitted in other parameters' default values as long as there is not a cycle (BCP080)
                        // Function: we already validate that a function cannot be used as a variable (BCP063)
                        // Output: we already validate that outputs cannot be referenced in expressions (BCP058)
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