// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        // stores results of type checks
        private readonly TypeAssignmentVisitor typeAssignmentVisitor;
        private readonly DeclaredTypeManager declaredTypeManager;

        public TypeManager(SemanticModel model, IBinder binder)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.typeAssignmentVisitor = new(this, model);
            this.declaredTypeManager = new(this, binder, model.Features);
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
        {
            if (syntax is ImplicitBaseIdentifierSyntax synthetic)
            {
                return GetSyntheticBaseParametersType(synthetic);
            }

            return typeAssignmentVisitor.GetTypeInfo(syntax);
        }

        private readonly ConcurrentDictionary<ImplicitBaseIdentifierSyntax, TypeSymbol> syntheticBaseTypes = new();

        private TypeSymbol GetSyntheticBaseParametersType(ImplicitBaseIdentifierSyntax syntax)
        {
            return syntheticBaseTypes.GetOrAdd(syntax, key =>
            {
                if (this.declaredTypeManager is null)
                {
                    return LanguageConstants.Any;
                }

                if (((SemanticModel)this.typeAssignmentVisitor.GetType().GetField("model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this.typeAssignmentVisitor)!)
                    .Binder.FileSymbol.Declarations.OfType<BaseParametersSymbol>()
                    .FirstOrDefault(s => ReferenceEquals(s.DeclaringSyntax, key)) is not { } baseSymbol)
                {
                    return LanguageConstants.Any;
                }

                var namedProperties = baseSymbol.ParentAssignments
                    .GroupBy(pa => pa.Name, LanguageConstants.IdentifierComparer)
                    .Select(group => group.First())
                    .Select(pa => new NamedTypeProperty(pa.Name, GetTypeInfo(pa.DeclaringParameterAssignment.Value), TypePropertyFlags.ReadOnly));

                return new ObjectType(
                    name: "baseParameters",
                    validationFlags: TypeSymbolValidationFlags.Default,
                    properties: namedProperties,
                    additionalProperties: null);
            });
        }

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax)
            => declaredTypeManager.GetDeclaredType(syntax);

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax)
            => declaredTypeManager.GetDeclaredTypeAssignment(syntax);

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
            => typeAssignmentVisitor.GetAllDiagnostics();

        public FunctionOverload? GetMatchedFunctionOverload(FunctionCallSyntaxBase syntax)
            => typeAssignmentVisitor.GetMatchedFunctionOverload(syntax);

        public Expression? GetMatchedFunctionResultValue(FunctionCallSyntaxBase syntax)
            => typeAssignmentVisitor.GetMatchedFunctionResultValue(syntax);

        public TypeExpression? TryGetReifiedType(ParameterizedTypeInstantiationSyntaxBase syntax)
            => declaredTypeManager.TryGetReifiedType(syntax);
    }
}
