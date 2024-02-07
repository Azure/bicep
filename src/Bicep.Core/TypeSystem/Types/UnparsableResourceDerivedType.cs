// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types;

public class UnparsableResourceDerivedType(string typeReferenceString, TypeSymbol fallbackType) : ITypeReference
{
    private readonly TypeSymbol fallbackType = fallbackType;

    public string TypeReferenceString { get; } = typeReferenceString;

    TypeSymbol ITypeReference.Type => fallbackType;
}
