// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Parser;

namespace Bicep.Core.Linter
{
    public interface IFixable : IPositionable
    {
        public IReadOnlyList<Fix> Fixes { get; }
    }
}
