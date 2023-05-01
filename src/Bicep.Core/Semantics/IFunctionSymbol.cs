// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public interface IFunctionSymbol 
{
    string Name { get; }

    ImmutableArray<FunctionOverload> Overloads { get; }

    FunctionFlags FunctionFlags { get; }
}