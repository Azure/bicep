// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types;

public class UnparsableResourceDerivedType : ITypeReference
{
    private readonly TypeSymbol fallbackType;

    public UnparsableResourceDerivedType(string typeReferenceString, TypeSymbol fallbackType)
    {
        TypeReferenceString = typeReferenceString;
        this.fallbackType = fallbackType;
    }

    public string TypeReferenceString { get; }

    TypeSymbol ITypeReference.Type => fallbackType;
}
