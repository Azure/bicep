using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class FunctionInfo
    {
        public FunctionInfo(string name, TypeSymbol returnType, int minimumArgumentCount, int? maximumArgumentCount, IEnumerable<TypeSymbol> fixedArgumentTypes, TypeSymbol? variableArgumentType)
        {
            if (maximumArgumentCount.HasValue && maximumArgumentCount.Value < minimumArgumentCount)
            {
                throw new ArgumentException($"{nameof(maximumArgumentCount.Value)} cannot be less than {nameof(minimumArgumentCount)}.");
            }

            var fixedTypes = fixedArgumentTypes.ToImmutableArray();
            if (fixedTypes.Length < minimumArgumentCount && variableArgumentType == null)
            {
                throw new ArgumentException("Not enough argument types are specified.");
            }

            this.Name = name;
            this.ReturnType = returnType;
            this.MinimumArgumentCount = minimumArgumentCount;
            this.MaximumArgumentCount = maximumArgumentCount;
            this.FixedArgumentTypes = fixedTypes;
            this.VariableArgumentType = variableArgumentType;
        }

        public string Name { get; }

        public ImmutableArray<TypeSymbol> FixedArgumentTypes { get; }

        public int MinimumArgumentCount { get; }

        public int? MaximumArgumentCount { get; }

        public TypeSymbol? VariableArgumentType { get; }

        public TypeSymbol ReturnType { get; }

        public FunctionMatchResult Match(IList<TypeSymbol> argumentTypes)
        {
            if (argumentTypes.Count < this.MinimumArgumentCount)
            {
                // too few arguments
                return FunctionMatchResult.Mismatch;
            }

            if (this.MaximumArgumentCount.HasValue && argumentTypes.Count > this.MaximumArgumentCount.Value)
            {
                // too many arguments
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
                var expectedType = i < this.FixedArgumentTypes.Length ? this.FixedArgumentTypes[i] : this.VariableArgumentType;

                if (TypeValidator.AreTypesAssignable(argumentType, expectedType) != true)
                {
                    return FunctionMatchResult.Mismatch;
                }
            }

            return FunctionMatchResult.Match;
        }

        public static FunctionInfo CreateFixed(string name, TypeSymbol returnType, params TypeSymbol[] argumentTypes) => 
            new FunctionInfo(name, returnType, argumentTypes.Length, argumentTypes.Length, argumentTypes, variableArgumentType: null);

        public static FunctionInfo CreatePartialFixed(string name, TypeSymbol returnType, IEnumerable<TypeSymbol> fixedArgumentTypes, TypeSymbol variableArgumentType) => 
            new FunctionInfo(name, returnType, fixedArgumentTypes.Count(), null, fixedArgumentTypes, variableArgumentType);

        public static FunctionInfo CreateWithVarArgs(string name, TypeSymbol returnType, int minimumArgumentCount, TypeSymbol argumentType) =>
            new FunctionInfo(
                name,
                returnType,
                minimumArgumentCount,
                maximumArgumentCount: null,
                Enumerable.Repeat(argumentType, minimumArgumentCount),
                argumentType);
    }
}