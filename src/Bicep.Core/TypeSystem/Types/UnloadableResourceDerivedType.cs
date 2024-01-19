// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

public class UnloadableResourceDerivedType : ITypeReference
{
    private readonly TypeSymbol fallbackType;

    public UnloadableResourceDerivedType(string typeReferenceString, TypeSymbol fallbackType)
    {
        TypeReferenceString = typeReferenceString;
        this.fallbackType = fallbackType;
    }

    public string TypeReferenceString { get; }

    TypeSymbol ITypeReference.Type => fallbackType;
}
