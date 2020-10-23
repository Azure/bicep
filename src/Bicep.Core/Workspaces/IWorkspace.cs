// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public interface IWorkspace : IReadOnlyWorkspace
    {
        (ImmutableArray<SyntaxTree> added, ImmutableArray<SyntaxTree> removed) UpsertSyntaxTrees(IEnumerable<SyntaxTree> syntaxTrees);

        void RemoveSyntaxTrees(IEnumerable<SyntaxTree> syntaxTrees);
    }
}