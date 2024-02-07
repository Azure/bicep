// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Types
{
    public class LambdaType(ImmutableArray<ITypeReference> argumentTypes, ITypeReference returnType) : TypeSymbol(FormatTypeName(argumentTypes, returnType))
    {
        public override TypeKind TypeKind => TypeKind.Lambda;

        public ImmutableArray<ITypeReference> ArgumentTypes { get; } = argumentTypes;

        public ITypeReference ReturnType { get; } = returnType;

        private static string FormatTypeName(ImmutableArray<ITypeReference> argumentTypes, ITypeReference bodyType)
            => argumentTypes.Length == 1 ?
                $"{argumentTypes.Single().Type.FormatNameForCompoundTypes()} => {bodyType.Type.FormatNameForCompoundTypes()}" :
                $"({string.Join(", ", argumentTypes.Select(x => x.Type.FormatNameForCompoundTypes()))}) => {bodyType.Type.FormatNameForCompoundTypes()}";

        public override string FormatNameForCompoundTypes() => WrapTypeName();
    }
}
