// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public abstract class Decorator
    {
        protected Decorator(TypeSymbol attachableType, FunctionOverload overload)
        {
            this.AttachableType = attachableType;
            this.Overload = overload;
        }

        public TypeSymbol AttachableType { get; }

        public FunctionOverload Overload { get; }

        public abstract ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType);

        public virtual void ValidateTarget(ITypeManager typeManager, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, IDiagnosticWriter diagnostics)
        {
            if (targetType is ErrorType)
            {
                return;
            }

            if (!TypeValidator.AreTypesAssignable(targetType, this.AttachableType))
            {
                diagnostics.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).CannotAttacheDecoratorToTarget(this.Overload.Name, this.AttachableType, targetType));
            }
        }
    }
}
