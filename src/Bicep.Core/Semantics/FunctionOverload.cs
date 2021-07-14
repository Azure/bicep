// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FunctionOverload
    {
        public delegate TypeSymbol ReturnTypeBuilderDelegate(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, ImmutableArray<FunctionArgumentSyntax> arguments, ImmutableArray<TypeSymbol> argumentTypes);
        public delegate SyntaxBase EvaluatorDelegate(FunctionCallSyntaxBase functionCall, Symbol symbol, TypeSymbol typeSymbol);

        public FunctionOverload(string name, string description, ReturnTypeBuilderDelegate returnTypeBuilder, TypeSymbol signatureType, IEnumerable<FixedFunctionParameter> fixedParameters, VariableFunctionParameter? variableParameter, EvaluatorDelegate? evaluator, FunctionFlags flags = FunctionFlags.Default)
        {
            Name = name;
            Description = description;
            ReturnTypeBuilder = returnTypeBuilder;
            Evaluator = evaluator;
            FixedParameters = fixedParameters.ToImmutableArray();
            VariableParameter = variableParameter;
            Flags = flags;

            MinimumArgumentCount = FixedParameters.Count(fp => fp.Required) + (VariableParameter?.MinimumCount ?? 0);
            MaximumArgumentCount = VariableParameter == null ? FixedParameters.Length : (int?)null;

            TypeSignature = $"({string.Join(", ", ParameterTypeSignatures)}): {signatureType}";
            TypeSignatureSymbol = signatureType;
        }

        public string Name { get; }

        public string Description { get; }

        public ImmutableArray<FixedFunctionParameter> FixedParameters { get; }

        public int MinimumArgumentCount { get; }

        public int? MaximumArgumentCount { get; }

        public VariableFunctionParameter? VariableParameter { get; }

        public ReturnTypeBuilderDelegate ReturnTypeBuilder { get; }
        public TypeSymbol TypeSignatureSymbol { get; }
        public EvaluatorDelegate? Evaluator { get; }

        public FunctionFlags Flags { get; }

        public string TypeSignature { get; }

        public IEnumerable<string> ParameterTypeSignatures => this.FixedParameters
            .Select(fp => fp.Signature)
            .Concat(this.VariableParameter?.GenericSignature.AsEnumerable() ?? Enumerable.Empty<string>());

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
                TypeSymbol expectedType;

                if (i < this.FixedParameters.Length)
                {
                    expectedType = this.FixedParameters[i].Type;
                }
                else
                {
                    if (this.VariableParameter == null)
                    {
                        // Theoretically this shouldn't happen, becase it already passed argument count checking, either:
                        // - The function takes 0 argument - argumentTypes must be empty, so it won't enter the loop
                        // - The function take at least one argument - when i >= FixedParameterTypes.Length, VariableParameterType
                        //   must not be null, otherwise, the function overload has invalid parameter count definition.
                        throw new ArgumentException($"Got unexpected null value for {nameof(this.VariableParameter)}. Ensure the function overload definition is correct: '{this.TypeSignature}'.");
                    }

                    expectedType = this.VariableParameter.Type;
                }

                if (TypeValidator.AreTypesAssignable(argumentType, expectedType) != true)
                {
                    argumentTypeMismatch = new ArgumentTypeMismatch(this, i, argumentType, expectedType);

                    return FunctionMatchResult.Mismatch;
                }
            }

            return FunctionMatchResult.Match;
        }

    }
}
