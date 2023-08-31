// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using System.Collections.Immutable;

namespace Bicep.Core.Semantics;

public interface IFunctionSymbol
{
    string Name { get; }

    ImmutableArray<FunctionOverload> Overloads { get; }

    FunctionFlags FunctionFlags { get; }
}