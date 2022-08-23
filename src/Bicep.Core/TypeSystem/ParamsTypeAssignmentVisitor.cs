// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Linq;
using Bicep.Core.Extensions;
using System.Collections.Concurrent;

namespace Bicep.Core.TypeSystem
{
    public sealed class ParamsTypeAssignmentVisitor : SyntaxVisitor
    {
        private readonly IParamsTypeManager paramsTypeManager;
        private readonly ParamBinder paramBinder;
        private readonly ConcurrentDictionary<SyntaxBase, TypeAssignment> assignedTypes;

        public ParamsTypeAssignmentVisitor(IParamsTypeManager paramsTypeManager, ParamBinder paramBinder)
        {
            this.paramsTypeManager = paramsTypeManager;
            this.paramBinder = paramBinder;
            assignedTypes = new();
        }

        private TypeAssignment GetTypeAssignment(SyntaxBase syntax)
        {
            Visit(syntax);

            if (!assignedTypes.TryGetValue(syntax, out var typeAssignment))
            {
                return new TypeAssignment(ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).InvalidExpression()));
            }
            return typeAssignment;
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => GetTypeAssignment(syntax).Reference.Type;

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var valueType = paramsTypeManager.GetTypeInfo(syntax.Value);
                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, valueType))
                {
                    return ErrorType.Create(errors);
                }

                return valueType;
            });

        private void AssignType(SyntaxBase syntax, Func<ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () => new TypeAssignment(assignFunc()));

        private void AssignTypeWithDiagnostics(SyntaxBase syntax, Func<IDiagnosticWriter, ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () =>
            {
                var diagnosticWriter = ToListDiagnosticWriter.Create();
                var reference = assignFunc(diagnosticWriter);

                return new TypeAssignment(reference, diagnosticWriter.GetDiagnostics());
            });

        private void AssignTypeWithCaching(SyntaxBase syntax, Func<TypeAssignment> assignFunc) =>
            assignedTypes.GetOrAdd(syntax, key =>
                CheckForCyclicError(key) is { } cyclicErrorType
                    ? new TypeAssignment(cyclicErrorType)
                    : assignFunc());

        private TypeSymbol? CheckForCyclicError(SyntaxBase syntax)
        {
            if (this.paramBinder.GetSymbolInfo(syntax) is not DeclaredSymbol declaredSymbol)
            {
                return null;
            }

            if (declaredSymbol.DeclaringSyntax == syntax)
            {
                // Report cycle errors on accesses to cyclic symbols, not on the declaration itself
                return null;
            }

            if (this.paramBinder.TryGetCycle(declaredSymbol) is { } cycle)
            {
                // there's a cycle. stop visiting now or we never will!
                if (cycle.Length == 1)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).CyclicExpressionSelfReference());
                }

                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).CyclicExpression(cycle.Select(x => x.Name)));
            }

            return null;
        }

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Bool);

        public override void VisitStringSyntax(StringSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (syntax.TryGetLiteralValue() is string literalValue)
                {
                    // uninterpolated strings have a known type
                    return new StringLiteralType(literalValue);
                }

                var errors = new List<ErrorDiagnostic>();
                var expressionTypes = new List<TypeSymbol>();

                foreach (var interpolatedExpression in syntax.Expressions)
                {
                    var expressionType = paramsTypeManager.GetTypeInfo(interpolatedExpression);
                    CollectErrors(errors, expressionType);
                    expressionTypes.Add(expressionType);
                }

                if (PropagateErrorType(errors, expressionTypes))
                {
                    return ErrorType.Create(errors);
                }

                // normally we would also do an assignability check, but we allow "any" type in string interpolation expressions
                // so the assignability check cannot possibly fail (we already collected type errors from the inner expressions at this point)
                return LanguageConstants.String;
            });

        public override void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Int);

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Null);

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
            => AssignType(syntax, () =>
            {
                // error should have already been raised by the ParseDiagnosticsVisitor - no need to add another
                return ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>());
            });

        public override void VisitObjectSyntax(ObjectSyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();

                var propertyTypes = new List<TypeSymbol>();
                foreach (var objectProperty in syntax.Properties)
                {
                    var propertyType = paramsTypeManager.GetTypeInfo(objectProperty);
                    CollectErrors(errors, propertyType);
                    propertyTypes.Add(propertyType);
                }

                if (PropagateErrorType(errors, propertyTypes))
                {
                    return ErrorType.Create(errors);
                }

                // type results are cached
                var namedProperties = syntax.Properties
                    .GroupByExcludingNull(p => p.TryGetKeyText(), LanguageConstants.IdentifierComparer)
                    .Select(group =>
                    {
                        var resolvedType = TypeHelper.CreateTypeUnion(group.Select(p => paramsTypeManager.GetTypeInfo(p)));
                        return new TypeProperty(group.Key, resolvedType);
                    });

                // TODO: Add structural naming?
                return new ObjectType(LanguageConstants.Object.Name, TypeSymbolValidationFlags.Default, namedProperties, null);
            });

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();
                var types = new List<TypeSymbol>();

                if (syntax.Key is StringSyntax stringSyntax && stringSyntax.IsInterpolated())
                {
                    // if the key is an interpolated string, we need to check the expressions referenced by it
                    var keyType = paramsTypeManager.GetTypeInfo(syntax.Key);
                    CollectErrors(errors, keyType);
                    types.Add(keyType);
                }

                var valueType = paramsTypeManager.GetTypeInfo(syntax.Value);

                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, types.Concat(valueType)))
                {
                    valueType = ErrorType.Create(errors);
                }
                return valueType;
            });

        public override void VisitArrayItemSyntax(ArrayItemSyntax syntax)
            => AssignType(syntax, () => paramsTypeManager.GetTypeInfo(syntax.Value));

        public override void VisitArraySyntax(ArraySyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();

                var itemTypes = new List<TypeSymbol>(syntax.Children.Length);
                foreach (var arrayItem in syntax.Items)
                {
                    var itemType = paramsTypeManager.GetTypeInfo(arrayItem);
                    itemTypes.Add(itemType);
                    CollectErrors(errors, itemType);
                }

                if (PropagateErrorType(errors, itemTypes))
                {
                    return ErrorType.Create(errors);
                }

                if (TypeHelper.TryCollapseTypes(itemTypes) is not { } collapsedItemType)
                {
                    return LanguageConstants.Array;
                }

                return new TypedArrayType(collapsedItemType, TypeSymbolValidationFlags.Default);
            });

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
            => AssignType(syntax, () => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParameterTernaryOperationNotSupported()));

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
            => AssignType(syntax, () => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParameterBinaryOperationNotSupported()));

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
            => AssignType(syntax, () => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParameterUnaryOperationNotSupported()));

        public override void VisitLambdaSyntax(LambdaSyntax syntax)
            => AssignType(syntax, () => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParameterLambdaFunctionNotSupported()));

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            => AssignType(syntax, () => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParameterFunctionCallNotSupported()));

        private static bool PropagateErrorType(IEnumerable<ErrorDiagnostic> errors, params TypeSymbol[] types)
            => PropagateErrorType(errors, types as IEnumerable<TypeSymbol>);

        private static bool PropagateErrorType(IEnumerable<ErrorDiagnostic> errors, IEnumerable<TypeSymbol> types)
        {
            if (errors.Any())
            {
                return true;
            }

            return types.Any(x => x.TypeKind == TypeKind.Error);
        }

        private static void CollectErrors(List<ErrorDiagnostic> errors, ITypeReference reference)
        {
            errors.AddRange(reference.Type.GetDiagnostics());
        }

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
        {
            // ensure we've visited all of the syntax nodes
            Visit(this.paramBinder.ParamFileSymbol.Syntax);

            return assignedTypes.Values.SelectMany(x => x.Diagnostics);
        }
    }
}
