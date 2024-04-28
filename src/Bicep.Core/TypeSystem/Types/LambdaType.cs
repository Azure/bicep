// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Bicep.Core.TypeSystem.Types
{
    public class LambdaType : TypeSymbol
    {
        public LambdaType(ImmutableArray<ITypeReference> argumentTypes, ImmutableArray<ITypeReference> optionalArgumentTypes, ITypeReference returnType)
            : base(FormatTypeName(argumentTypes, optionalArgumentTypes, returnType))
        {
            ArgumentTypes = argumentTypes;
            OptionalArgumentTypes = optionalArgumentTypes;
            ReturnType = returnType;
        }

        public override TypeKind TypeKind => TypeKind.Lambda;

        public ImmutableArray<ITypeReference> ArgumentTypes { get; }

        public ImmutableArray<ITypeReference> OptionalArgumentTypes { get; }

        public ITypeReference ReturnType { get; }

        private static string FormatTypeName(ImmutableArray<ITypeReference> argumentTypes, ImmutableArray<ITypeReference> optionalArgumentTypes, ITypeReference bodyType)
        {
            IEnumerable<(string name, bool isOptional)> GetArgumentTypes()
            {
                foreach (var argType in argumentTypes)
                {
                    yield return (argType.Type.FormatNameForCompoundTypes(), false);
                }
                foreach (var argType in optionalArgumentTypes)
                {
                    yield return (argType.Type.FormatNameForCompoundTypes(), true);
                }
            }

            var allArgs = GetArgumentTypes().ToImmutableArray();
            var returnTypeName = bodyType.Type.FormatNameForCompoundTypes();

            if (allArgs.Length == 0)
            {
                // () => bar
                return $"() => {returnTypeName}";
            }

            if (allArgs.Length == 1)
            {
                if (allArgs[0].isOptional)
                {
                    // ([foo]) => bar
                    return $"([{allArgs[0].name}]) => {returnTypeName}";
                }

                // foo => bar
                return $"{allArgs[0].name} => {returnTypeName}";
            }

            var fixedArgPortion = string.Join(", ", allArgs.Where(x => !x.isOptional).Select(x => x.name));
            var optionalArgPortion = string.Join(", ", allArgs.Where(x => x.isOptional).Select(x => x.name));

            if (fixedArgPortion.Length == 0)
            {
                // ([foo, bar]) => baz
                return $"([{optionalArgPortion}]) => {returnTypeName}";
            }

            if (optionalArgPortion.Length == 0)
            {
                // (foo, bar) => baz
                return $"({fixedArgPortion}) => {returnTypeName}";
            }

            // (foo, bar[, baz]) => qux
            return $"({fixedArgPortion}[, {optionalArgPortion}]) => {returnTypeName}";
        }

        /// <summary>
        /// Gets the argument type at a particular position, including optional args.
        /// <remarks>
        /// The index must be valid, or this method will throw.
        /// </remarks>
        /// </summary>
        public ITypeReference GetArgumentType(int position)
        {
            if (position < ArgumentTypes.Length)
            {
                return ArgumentTypes[position];
            }

            return OptionalArgumentTypes[position - ArgumentTypes.Length];
        }

        public int MaximumArgCount => ArgumentTypes.Length + OptionalArgumentTypes.Length;

        public int MinimumArgCount => ArgumentTypes.Length;

        public override string FormatNameForCompoundTypes() => WrapTypeName();
    }
}
