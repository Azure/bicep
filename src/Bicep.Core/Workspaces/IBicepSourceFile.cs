// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public interface IBicepSourceFile : ISourceFile
    {
        ImmutableArray<int> LineStarts { get; }

        ProgramSyntax ProgramSyntax { get; }
    }
}
