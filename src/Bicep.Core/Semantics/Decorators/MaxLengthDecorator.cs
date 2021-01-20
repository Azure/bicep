// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class MaxLengthDecorator : Decorator
    {
        public MaxLengthDecorator()
            : base(UnionType.Create(LanguageConstants.String, LanguageConstants.Array), new FunctionOverloadBuilder("maxLength")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Defines the maximum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The maximum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType) =>
            targetObject.MergeProperty("maxLength", decoratorSyntax.Arguments.Single().Expression);
    }
}
