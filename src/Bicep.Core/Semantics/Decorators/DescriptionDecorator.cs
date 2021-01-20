// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class DescriptionDecorator : Decorator
    {
        public DescriptionDecorator()
            : base(LanguageConstants.Any, new FunctionOverloadBuilder("description")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Describes the parameter.")
                .WithRequiredParameter("text", LanguageConstants.String, "The description.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType) =>
            targetObject.MergeProperty("metadata", SyntaxFactory.CreateObject(
                SyntaxFactory.CreateObjectProperty("description", decoratorSyntax.Arguments.Single().Expression).AsEnumerable()));
    }
}
