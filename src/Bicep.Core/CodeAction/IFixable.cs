// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Parser;

namespace Bicep.Core.CodeAction
{
    public interface IFixable : IPositionable
    {
        public IEnumerable<CodeFix> Fixes { get; }
    }
}
