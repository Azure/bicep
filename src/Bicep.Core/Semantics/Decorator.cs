// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public delegate void DecoratorValidator(
        string decoratorName,
        DecoratorSyntax decoratorSyntax,
        TypeSymbol targetType,
        ITypeManager typeManager,
        IBinder binder,
        IDiagnosticWriter diagnosticWriter);

    public delegate ObjectExpression? DecoratorEvaluator(
        FunctionCallExpression functionCall,
        TypeSymbol targetType,
        ObjectExpression? targetObject);

    public class Decorator
    {
        private readonly TypeSymbol attachableType;

        private readonly DecoratorValidator? validator;

        private readonly DecoratorEvaluator? evaluator;

        public Decorator(FunctionOverload overload, TypeSymbol attachableType, DecoratorValidator? validator, DecoratorEvaluator? evaluator)
        {
            this.Overload = overload;
            this.attachableType = attachableType;
            this.validator = validator;
            this.evaluator = evaluator;
        }

        public FunctionOverload Overload { get; }

        public bool CanAttachTo(TypeSymbol targetType) => TypeValidator.AreTypesAssignable(targetType, attachableType);

        public void Validate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IBinder binder, IDiagnosticWriter diagnosticWriter)
        {
            // The following line makes the simplifying assumption that nullability does not impact decorator validity. This assumption is true at the moment
            // because aside from @metadata and @description (which are attachable to targets of any type), all decorators represent validation constraints
            // (which are no-ops on null values within the ARM runtime). This assumption may or may not hold when 3P extensibility providers define their own
            // decorators, at which point we'll probably want a .AllowsNullableTargets property on decorators or the like.
            targetType = RemoveImplicitNull(targetType);

            if (targetType is ErrorType)
            {
                return;
            }

            if (!this.CanAttachTo(targetType))
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).CannotAttachDecoratorToTarget(this.Overload.Name, attachableType, targetType));
            }

            // Invoke custom validator if provided.
            this.validator?.Invoke(this.Overload.Name, decoratorSyntax, targetType, typeManager, binder, diagnosticWriter);
        }

        public ObjectExpression? Evaluate(FunctionCallExpression functionCall, TypeSymbol targetType, ObjectExpression? targetObject)
        {
            if (this.evaluator is null)
            {
                return targetObject;
            }

            return this.evaluator(functionCall, RemoveImplicitNull(targetType), targetObject);
        }

        private static TypeSymbol RemoveImplicitNull(TypeSymbol type) => TypeHelper.TryRemoveNullability(type) ?? type;
    }
}
