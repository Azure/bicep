// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types;

public class NullType : TypeSymbol
{
    internal NullType() : base(LanguageConstants.NullKeyword)
    {
    }

    public override TypeKind TypeKind => TypeKind.Primitive;
}
