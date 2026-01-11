// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public class FunctionOverload
    {
        public delegate FunctionResult ResultBuilderDelegate(
            SemanticModel model,
            IDiagnosticWriter diagnostics,
            FunctionCallSyntaxBase functionCall,
            ImmutableArray<TypeSymbol> argumentTypes);

        public delegate Expression EvaluatorDelegate(
            FunctionCallExpression expression);

        public delegate LanguageExpression LanguageExpressionTransformerDelegate(
            FunctionExpression expression);

        public FunctionOverload(string name, string genericDescription, string description, ResultBuilderDelegate resultBuilder, TypeSymbol signatureType, IEnumerable<FixedFunctionParameter> fixedParameters, VariableFunctionParameter? variableParameter, EvaluatorDelegate? evaluator, LanguageExpressionTransformerDelegate? expressionConverter, FunctionFlags flags = FunctionFlags.Default)
        {
            Name = name;
            GenericDescription = genericDescription;
            Description = description;
            ResultBuilder = resultBuilder;
            Evaluator = evaluator;
            LanguageExpressionTransformer  = expressionConverter;
            FixedParameters = [.. fixedParameters];
            VariableParameter = variableParameter;
            Flags = flags;

            MinimumArgumentCount = FixedParameters.Count(fp => fp.Required) + (VariableParameter?.MinimumCount ?? 0);
            MaximumArgumentCount = VariableParameter == null ? FixedParameters.Length : (int?)null;

            TypeSignature = $"({string.Join(", ", ParameterTypeSignatures)}): {signatureType}";
            TypeSignatureSymbol = signatureType;
        }

        public string Name { get; }

        public string GenericDescription { get; }

        public string Description { get; }

        public ImmutableArray<FixedFunctionParameter> FixedParameters { get; }

        public int MinimumArgumentCount { get; }

        public int? MaximumArgumentCount { get; }

        public VariableFunctionParameter? VariableParameter { get; }

        public ResultBuilderDelegate ResultBuilder { get; }

        public TypeSymbol TypeSignatureSymbol { get; }

        public EvaluatorDelegate? Evaluator { get; }

        public LanguageExpressionTransformerDelegate? LanguageExpressionTransformer { get; }

        public FunctionFlags Flags { get; }

        public string TypeSignature { get; }

        public IEnumerable<string> ParameterTypeSignatures => this.FixedParameters
            .Select(fp => fp.Signature)
            .Concat(this.VariableParameter?.GenericSignature.AsEnumerable() ?? []);

        public bool HasParameters => this.MinimumArgumentCount > 0 || this.MaximumArgumentCount > 0;

        public FunctionMatchResult Match(IList<TypeSymbol> argumentTypes, out ArgumentCountMismatch? argumentCountMismatch, out ArgumentTypeMismatch? argumentTypeMismatch)
        {
            argumentCountMismatch = null;
            argumentTypeMismatch = null;

            if (argumentTypes.Count < this.MinimumArgumentCount ||
                (this.MaximumArgumentCount.HasValue && argumentTypes.Count > this.MaximumArgumentCount.Value))
            {
                // Too few or too many arguments.
                argumentCountMismatch = new ArgumentCountMismatch(argumentTypes.Count, this.MinimumArgumentCount, this.MaximumArgumentCount);

                return FunctionMatchResult.Mismatch;
            }

            if (argumentTypes.All(a => a.TypeKind == TypeKind.Any))
            {
                // all argument types are "any"
                // it's a potential match at best
                return FunctionMatchResult.PotentialMatch;
            }

            for (int i = 0; i < argumentTypes.Count; i++)
            {
                var argumentType = argumentTypes[i];
                var expectedType = GetArgumentType(i);

                if (TypeValidator.AreTypesAssignable(argumentType, expectedType) != true)
                {
                    argumentTypeMismatch = new ArgumentTypeMismatch(this, i, argumentType, expectedType);

                    return FunctionMatchResult.Mismatch;
                }
            }

            return argumentTypes.OfType<AnyType>().Any()
                ? FunctionMatchResult.PotentialMatch
                : FunctionMatchResult.Match;
        }

        public TypeSymbol GetArgumentType(
            int index,
            FunctionOverloadBuilder.GetFunctionArgumentType? getFunctionArgumentType = null,
            FunctionOverloadBuilder.GetAttachedType? getAttachedType = null)
        {
            if (index < this.FixedParameters.Length)
            {
                if (FixedParameters[index].Calculator is { } calculator &&
                    getFunctionArgumentType is not null &&
                    getAttachedType is not null &&
                    calculator(getFunctionArgumentType, getAttachedType) is { } calculatedType)
                {
                    return calculatedType;
                }

                return this.FixedParameters[index].Type;
            }
            else
            {
                if (this.VariableParameter == null)
                {
                    // Theoretically this shouldn't happen, because it already passed argument count checking, either:
                    // - The function takes 0 argument - argumentTypes must be empty, so it won't enter the loop
                    // - The function take at least one argument - when i >= FixedParameterTypes.Length, VariableParameterType
                    //   must not be null, otherwise, the function overload has invalid parameter count definition.
                    throw new ArgumentException($"Got unexpected null value for {nameof(this.VariableParameter)}. Ensure the function overload definition is correct: '{this.TypeSignature}'.");
                }

                return this.VariableParameter.Type;
            }
        }
    }
}
