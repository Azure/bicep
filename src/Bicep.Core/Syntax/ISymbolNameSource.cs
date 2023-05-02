// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public interface ISymbolNameSource : IPositionable
    {
        public bool IsValid { get; }
    }
}
