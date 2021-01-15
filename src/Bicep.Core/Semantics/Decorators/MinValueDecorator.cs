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

        public override KeyValuePair<string, SyntaxBase>? Evaluate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType) =>
            new KeyValuePair<string, SyntaxBase>("minValue", decoratorSyntax.Arguments.Single());
    }
}
