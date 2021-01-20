// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class AllowedValuesDecorator : Decorator
    {
        public AllowedValuesDecorator()
            : base(LanguageConstants.Any, new FunctionOverloadBuilder("allowed")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Defines the allowed values of the parameter.")
                .WithRequiredParameter("values", LanguageConstants.Array, "The allowed values.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType) =>
            targetObject.MergeProperty("allowedValues", decoratorSyntax.Arguments.Single().Expression);

        public override void ValidateTarget(ITypeManager typeManager, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, IDiagnosticWriter diagnostics)
        {
            if (targetType is ErrorType)
            {
                return;
            }

            TypeValidator.NarrowTypeAndCollectDiagnostics(
                typeManager,
                decoratorSyntax.Arguments.Single().Expression,
                new TypedArrayType(targetType, TypeSymbolValidationFlags.Default),
                diagnostics);
        }
    }
}
