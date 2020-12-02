// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public interface IReadOnlyWorkspace
    {
        bool TryGetSyntaxTree(Uri fileUri, [NotNullWhen(true)] out SyntaxTree? syntaxTree);

        IEnumerable<SyntaxTree> GetSyntaxTreesForDirectory(Uri fileUri);

        ImmutableDictionary<Uri, SyntaxTree> GetActiveSyntaxTrees();
    }
}