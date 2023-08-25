// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

public class VoidType : TypeSymbol
{
    public static readonly VoidType Instance = new();

    private VoidType() : base(LanguageConstants.VoidKeyword) {}

    public override TypeKind TypeKind => TypeKind.Void;
}
