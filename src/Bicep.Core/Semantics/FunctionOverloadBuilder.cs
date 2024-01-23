// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FunctionOverloadBuilder
    {
        public delegate TypeSymbol GetFunctionArgumentType(int argIndex);

        public delegate TypeSymbol? FunctionArgumentTypeCalculator(GetFunctionArgumentType getArgumentTypeFunc);

        public FunctionOverloadBuilder(string name)
        {
            Name = name;
            GenericDescription = string.Empty;
            Description = string.Empty;
            ReturnType = LanguageConstants.Any;
            FixedParameters = ImmutableArray.CreateBuilder<FixedFunctionParameter>();
            ResultBuilder = (_, _, _, _) => new(LanguageConstants.Any);
            VariableParameter = null;
        }

        protected string Name { get; }

        protected string GenericDescription { get; private set; }

        protected string Description { get; private set; }

        protected TypeSymbol ReturnType { get; private set; }

        protected ImmutableArray<FixedFunctionParameter>.Builder FixedParameters { get; }

        protected VariableFunctionParameter? VariableParameter { get; private set; }

        protected FunctionOverload.ResultBuilderDelegate ResultBuilder { get; private set; }

        protected FunctionOverload.EvaluatorDelegate? Evaluator { get; private set; }

        protected FunctionFlags Flags { get; private set; }

        public FunctionOverload Build()
        {
            Validate();
            return BuildInternal();
        }

        protected virtual FunctionOverload BuildInternal() =>
            new(
                Name,
                GenericDescription,
                Description,
                ResultBuilder,
                ReturnType,
                FixedParameters.ToImmutable(),
                VariableParameter,
                Evaluator,
                Flags);

        public FunctionOverloadBuilder WithGenericDescription(string genericDescription)
        {
            GenericDescription = genericDescription;
            Description = genericDescription;

            return this;
        }

        public FunctionOverloadBuilder WithDescription(string description)
        {
            Description = description;

            return this;
        }

        public FunctionOverloadBuilder WithReturnType(TypeSymbol returnType)
        {
            ReturnType = returnType;
            ResultBuilder = (_, _, _, _) => new(returnType);

            return this;
        }

        public FunctionOverloadBuilder WithReturnResultBuilder(FunctionOverload.ResultBuilderDelegate resultBuilder, TypeSymbol signatureType)
        {
            ReturnType = signatureType;
            ResultBuilder = resultBuilder;

            return this;
        }

        public FunctionOverloadBuilder WithRequiredParameter(string name, TypeSymbol type, string description, FunctionArgumentTypeCalculator? calculator = null)
        {
            FixedParameters.Add(new FixedFunctionParameter(name, description, type, Required: true, Calculator: calculator));
            return this;
        }

        public FunctionOverloadBuilder WithOptionalParameter(string name, TypeSymbol type, string description, FunctionArgumentTypeCalculator? calculator = null)
        {
            FixedParameters.Add(new FixedFunctionParameter(name, description, type, Required: false, Calculator: calculator));
            return this;
        }

        public FunctionOverloadBuilder WithVariableParameter(string namePrefix, TypeSymbol type, int minimumCount, string description)
        {
            VariableParameter = new(namePrefix, description, type, minimumCount);
            return this;
        }

        public FunctionOverloadBuilder WithEvaluator(FunctionOverload.EvaluatorDelegate evaluator)
        {
            Evaluator = evaluator;
            return this;
        }

        public FunctionOverloadBuilder WithFlags(FunctionFlags flags)
        {
            Flags = flags;

            return this;
        }

        protected virtual void Validate()
        {
            // required params can only be at the beginning
            bool requiredState = true;
            bool optionalParameterPresent = false;

            for (int i = 0; i < FixedParameters.Count; i++)
            {
                var current = FixedParameters[i];

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
                        throw new InvalidOperationException($"Required parameter of function overload '{Name}' at index {i} follows an optional argument, which is not supported.");

                    default:
                        // optional following an optional
                        Debug.Assert(!requiredState && !current.Required, "!requiredState && !current.Required");
                        break;
                }
            }

            if (optionalParameterPresent && VariableParameter != null)
            {
                throw new InvalidOperationException($"The function overload '{Name}' has a variable parameter together with optional parameters, which is not supported.");
            }
        }
    }
}
