// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class DecoratorBuilder
    {
        private readonly FunctionOverloadBuilder functionOverloadBuilder;

        private DecoratorValidator? validator;

        private DecoratorEvaluator? evaluator;

        public DecoratorBuilder(string name)
        {
            this.functionOverloadBuilder = new FunctionOverloadBuilder(name);
        }

        public DecoratorBuilder WithDescription(string description)
        {
            this.functionOverloadBuilder.WithDescription(description);

            return this;
        }

        public DecoratorBuilder WithRequiredParameter(string name, TypeSymbol type, string description)
        {
            this.functionOverloadBuilder.WithRequiredParameter(name, type, description);

            return this;
        }

        public DecoratorBuilder WithOptionalParameter(string name, TypeSymbol type, string description)
        {
            this.functionOverloadBuilder.WithOptionalParameter(name, type, description);

            return this;
        }

        public DecoratorBuilder WithVariableParameter(string namePrefix, TypeSymbol type, int minimumCount, string description)
        {
            this.functionOverloadBuilder.WithVariableParameter(namePrefix, type, minimumCount, description);

            return this;
        }

        public DecoratorBuilder WithFlags(FunctionFlags flags)
        {
            this.functionOverloadBuilder.WithFlags(flags);

            return this;
        }

        public DecoratorBuilder WithValidator(DecoratorValidator validator)
        {
            this.validator = validator;

            return this;
        }

        public DecoratorBuilder WithEvaluator(DecoratorEvaluator evaluator)
        {
            this.evaluator = evaluator;

            return this;
        }

        public Decorator Build() => new Decorator(this.functionOverloadBuilder.Build(), this.validator, this.evaluator);
    }
}
