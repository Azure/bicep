using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class FunctionOverload
    {
        public FunctionOverload(string name, TypeSymbol returnType, int minimumArgumentCount, int? maximumArgumentCount, IEnumerable<TypeSymbol> fixedParameterTypes, TypeSymbol? variableParameterType, FunctionFlags flags = FunctionFlags.Default)
        {
            if (maximumArgumentCount.HasValue && maximumArgumentCount.Value < minimumArgumentCount)
            {
                throw new ArgumentException($"{nameof(maximumArgumentCount.Value)} cannot be less than {nameof(minimumArgumentCount)}.");
            }

            var fixedTypes = fixedParameterTypes.ToImmutableArray();
            if (fixedTypes.Length < minimumArgumentCount && variableParameterType == null)
            {
                throw new ArgumentException("Not enough argument types are specified.");
            }

            this.Name = name;
            this.ReturnType = returnType;
            this.MinimumArgumentCount = minimumArgumentCount;
            this.MaximumArgumentCount = maximumArgumentCount;
            this.FixedParameterTypes = fixedTypes;
            this.VariableParameterType = variableParameterType;
            this.Flags = flags;

            this.ParameterTypeSignatures = fixedTypes
                .Select((parameterType, i) => $"param{i}: {parameterType}")
                .ToImmutableArray();

            if (variableParameterType != null)
            {
                var restParameterType = variableParameterType.TypeKind == TypeKind.Union ? $"({variableParameterType})[]": $"{variableParameterType}[]";
                this.ParameterTypeSignatures.Add($"...rest: {restParameterType}");
            }

            this.TypeSignature = $"({string.Join(", ", this.ParameterTypeSignatures)}): {returnType}";
        }

        public string Name { get; }

        public ImmutableArray<TypeSymbol> FixedParameterTypes { get; }

        public int MinimumArgumentCount { get; }

        public int? MaximumArgumentCount { get; }

        public TypeSymbol? VariableParameterType { get; }

        public TypeSymbol ReturnType { get; }

        public FunctionFlags Flags { get; }

        public string TypeSignature { get; }

        public ImmutableArray<string> ParameterTypeSignatures { get; }

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
                var expectedType = i < this.FixedParameterTypes.Length ? this.FixedParameterTypes[i] : this.VariableParameterType!;

                if (TypeValidator.AreTypesAssignable(argumentType, expectedType) != true)
                {
                    argumentTypeMismatch = new ArgumentTypeMismatch(this, i, argumentType, expectedType);

                    return FunctionMatchResult.Mismatch;
                }
            }

            return FunctionMatchResult.Match;
        }

        public static FunctionOverload CreateFixed(string name, TypeSymbol returnType, params TypeSymbol[] argumentTypes) => 
            new FunctionOverload(name, returnType, argumentTypes.Length, argumentTypes.Length, argumentTypes, variableParameterType: null);

        public static FunctionOverload CreatePartialFixed(string name, TypeSymbol returnType, IEnumerable<TypeSymbol> fixedArgumentTypes, TypeSymbol variableArgumentType) => 
            new FunctionOverload(name, returnType, fixedArgumentTypes.Count(), null, fixedArgumentTypes, variableArgumentType);

        public static FunctionOverload CreateWithVarArgs(string name, TypeSymbol returnType, int minimumArgumentCount, TypeSymbol argumentType) =>
            new FunctionOverload(
                name,
                returnType,
                minimumArgumentCount,
                maximumArgumentCount: null,
                Enumerable.Repeat(argumentType, minimumArgumentCount),
                argumentType);
    }
}