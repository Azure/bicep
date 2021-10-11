// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
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

    public delegate ObjectSyntax? DecoratorEvaluator(
        DecoratorSyntax decoratorSyntax,
        TypeSymbol targetType,
        ObjectSyntax? targetObject);

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

        public ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ObjectSyntax? targetObject)
        {
            if (this.evaluator is null)
            {
                return targetObject;
            }

            return this.evaluator(decoratorSyntax, targetType, targetObject);
        }
    }
}
