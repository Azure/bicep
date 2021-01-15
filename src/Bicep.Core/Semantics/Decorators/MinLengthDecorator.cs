// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class MinLengthDecorator : Decorator
    {
        public MinLengthDecorator()
            : base(UnionType.Create(LanguageConstants.String, LanguageConstants.Array), new FunctionOverloadBuilder("minLength")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Defines the minimum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The minimum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override KeyValuePair<string, SyntaxBase>? Evaluate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType) =>
            new KeyValuePair<string, SyntaxBase>("minLength", decoratorSyntax.Arguments.Single());
    }
}
