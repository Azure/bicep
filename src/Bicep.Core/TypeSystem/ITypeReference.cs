// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public interface ITypeReference
    {
        TypeSymbol Type { get; }
    }
}