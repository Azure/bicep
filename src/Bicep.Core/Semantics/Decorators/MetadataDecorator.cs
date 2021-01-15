// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class MetadataDecorator : Decorator
    {
        public MetadataDecorator()
            : base(LanguageConstants.Any, new FunctionOverloadBuilder("metadata")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Defines metadata of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Object, "The metadata object.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override KeyValuePair<string, SyntaxBase>? Evaluate(DecoratorSyntax decoratorSyntax, TypeSymbol targetType) =>
            new KeyValuePair<string, SyntaxBase>("metadata", decoratorSyntax.Arguments.Single());
    }
}
