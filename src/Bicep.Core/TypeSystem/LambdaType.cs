// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class LambdaType : TypeSymbol
    {
        public LambdaType(ImmutableArray<ITypeReference> argumentTypes, ITypeReference returnType)
            : base(FormatTypeName(argumentTypes, returnType))
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        public override TypeKind TypeKind => TypeKind.Lambda;

        public ImmutableArray<ITypeReference> ArgumentTypes { get; }

        public ITypeReference ReturnType { get; }

        private static string FormatTypeName(ImmutableArray<ITypeReference> argumentTypes, ITypeReference bodyType)
            => argumentTypes.Length == 1 ?
                $"{argumentTypes.Single().Type.FormatNameForCompoundTypes()} => {bodyType.Type.FormatNameForCompoundTypes()}" :
                $"({string.Join(", ", argumentTypes.Select(x => x.Type.FormatNameForCompoundTypes()))}) => {bodyType.Type.FormatNameForCompoundTypes()}";

        public override string FormatNameForCompoundTypes() => this.WrapTypeName();
    }
}
