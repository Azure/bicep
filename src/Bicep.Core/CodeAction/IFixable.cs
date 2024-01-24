// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.CodeAction
{
    public interface IFixable : IPositionable
    {
        public IEnumerable<CodeFix> Fixes { get; }
    }
}
