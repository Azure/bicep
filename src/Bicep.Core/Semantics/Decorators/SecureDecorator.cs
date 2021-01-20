// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Decorators
{
    public sealed class SecureDecorator : Decorator
    {
        public SecureDecorator()
            : base(UnionType.Create(LanguageConstants.String, LanguageConstants.Object), new FunctionOverloadBuilder("secure")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Makes the parameter a secure parameter.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .Build())
        {
        }

        public override ObjectSyntax? Evaluate(DecoratorSyntax decoratorSyntax, ObjectSyntax? targetObject, TypeSymbol targetType)
        {
            if (ReferenceEquals(targetType, LanguageConstants.String))
            {
                return targetObject.MergeProperty("type", SyntaxFactory.CreateStringLiteral("secureString"));
            }

            if (ReferenceEquals(targetType, LanguageConstants.Object))
            {
                return targetObject.MergeProperty("type", SyntaxFactory.CreateStringLiteral("secureObject"));
            }

            return targetObject;
        }
    }
}
