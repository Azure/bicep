// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FunctionOverloadBuilder
    {
        public FunctionOverloadBuilder(string name)
        {
            this.Name = name;
            this.Description = string.Empty;
            this.ReturnType = LanguageConstants.Any;
            this.FixedParameters = ImmutableArray.CreateBuilder<FixedFunctionParameter>();
            this.ReturnTypeBuilder = (_, _, _, _, _) => LanguageConstants.Any;
            this.VariableParameter = null;
        }

        protected string Name { get; }

        protected string Description { get; private set; }

        protected TypeSymbol ReturnType { get; private set; }

        protected ImmutableArray<FixedFunctionParameter>.Builder FixedParameters { get; }

        protected VariableFunctionParameter? VariableParameter { get; private set; }

        protected FunctionOverload.ReturnTypeBuilderDelegate ReturnTypeBuilder { get; private set; }

        protected FunctionOverload.EvaluatorDelegate? Evaluator { get; private set; }

        protected FunctionFlags Flags { get; private set; }

        public FunctionOverload Build()
        {
            this.Validate();
            return this.BuildInternal();
        }

        public virtual FunctionOverload BuildInternal() =>
            new FunctionOverload(
                this.Name,
                this.Description,
                this.ReturnTypeBuilder,
                this.ReturnType,
                this.FixedParameters.ToImmutable(),
                this.VariableParameter,
                this.Evaluator,
                this.Flags);

        public FunctionOverloadBuilder WithDescription(string description)
        {
            this.Description = description;

            return this;
        }

        public FunctionOverloadBuilder WithReturnType(TypeSymbol returnType)
        {
            this.ReturnType = returnType;
            this.ReturnTypeBuilder = (_, _, _, _, _) => returnType;

            return this;
        }

        public FunctionOverloadBuilder WithDynamicReturnType(FunctionOverload.ReturnTypeBuilderDelegate returnTypeBuilder, TypeSymbol signatureType)
        {
            this.ReturnType = signatureType;
            this.ReturnTypeBuilder = returnTypeBuilder;

            return this;
        }

        public FunctionOverloadBuilder WithRequiredParameter(string name, TypeSymbol type, string description)
        {
            this.FixedParameters.Add(new FixedFunctionParameter(name, description, type, required: true));
            return this;
        }

        public FunctionOverloadBuilder WithOptionalParameter(string name, TypeSymbol type, string description)
        {
            this.FixedParameters.Add(new FixedFunctionParameter(name, description, type, required: false));
            return this;
        }

        public FunctionOverloadBuilder WithVariableParameter(string namePrefix, TypeSymbol type, int minimumCount, string description)
        {
            this.VariableParameter = new VariableFunctionParameter(namePrefix, description, type, minimumCount);
            return this;
        }

        public FunctionOverloadBuilder WithEvaluator(FunctionOverload.EvaluatorDelegate evaluator)
        {
            Evaluator = evaluator;
            return this;
        }

        public FunctionOverloadBuilder WithFlags(FunctionFlags flags)
        {
            this.Flags = flags;

            return this;
        }

        protected virtual void Validate()
        {
            // required params can only be at the beginning
            bool requiredState = true;
            bool optionalParameterPresent = false;

            for (int i = 0; i < this.FixedParameters.Count; i++)
            {
                var current = this.FixedParameters[i];

                if (!current.Required)
                {
                    optionalParameterPresent = true;
                }

                switch (requiredState)
                {
                    case true when current.Required:
                        // required param following a required param (or we're at the beginning)
                        // no state transition
                        break;
                    case true:
                        // encountered an optional parameter
                        Debug.Assert(!current.Required, "!current.Required");

                        // should expect only optional params from now on
                        requiredState = false;
                        break;

                    case false when current.Required:
                        // required param after we've seen optionals
                        throw new InvalidOperationException($"Required parameter of function overload '{this.Name}' at index {i} follows an optional argument, which is not supported.");

                    default:
                        // optional following an optional
                        Debug.Assert(!requiredState && !current.Required, "!requiredState && !current.Required");
                        break;
                }
            }

            if (optionalParameterPresent && this.VariableParameter != null)
            {
                throw new InvalidOperationException($"The function overload '{this.Name}' has a variable parameter together with optional parameters, which is not supported.");
            }
        }
    }
}
