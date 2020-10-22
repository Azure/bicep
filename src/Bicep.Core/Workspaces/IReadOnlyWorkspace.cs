// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public interface IReadOnlyWorkspace
    {
        // TODO use file URI internally instead of string
        bool TryGetSyntaxTree(string normalizedFileName, [NotNullWhen(true)] out SyntaxTree? syntaxTree);

        IEnumerable<SyntaxTree> GetSyntaxTreesForDirectory(string normalizedFilePath);
    }
}