// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class MaxValueDecorator : Decorator
    {
        public MaxValueDecorator()
            : base(LanguageConstants.Int, new FunctionOverloadBuilder("maxValue")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Defines the maximum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The maximum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType) =>
            targetObject.MergeProperty("maxValue", decoratorSyntax.Arguments.Single().Expression);
    }
}
