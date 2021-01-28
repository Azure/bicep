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
        IDiagnosticWriter diagnosticWriter);

    public delegate ObjectSyntax? DecoratorEvaluator(
        DecoratorSyntax decoratorSyntax,
        TypeSymbol targetType,
        ObjectSyntax? targetObject);

    public class Decorator
    {
        private readonly DecoratorValidator? validator;

        private readonly DecoratorEvaluator? evaluator;

        public Decorator(FunctionOverload overload, DecoratorValidator? validator, DecoratorEvaluator? evaluator)
        {
            this.Overload = overload;
            this.validator = validator;
            this.evaluator = evaluator;
        }

        public FunctionOverload Overload { get; }

        public void Validate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IDiagnosticWriter diagnosticWriter)
        {
            if (targetType is ErrorType)
            {
                return;
            }

            this.validator?.Invoke(this.Overload.Name, decoratorSyntax, targetType, typeManager, diagnosticWriter);
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
