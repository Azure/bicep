// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class MinValueDecorator : Decorator
    {
        public MinValueDecorator()
            : base(LanguageConstants.Int, new FunctionOverloadBuilder("minValue")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Defines the minimum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The minimum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType) =>
            targetObject.MergeProperty("maxValue", decoratorSyntax.Arguments.Single().Expression);
    }
}
