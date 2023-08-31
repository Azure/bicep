// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;

namespace Bicep.Core.CodeAction
{
    public interface ICodeFixProvider
    {
        IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes);
    }
}
