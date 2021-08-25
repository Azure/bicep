// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Configuration;

namespace Bicep.Core.Workspaces
{
    public interface IWorkspace : IReadOnlyWorkspace
    {
        (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> sourceFiles);

        void RemoveSourceFiles(IEnumerable<ISourceFile> sourceFiles);

        void UpsertActiveBicepConfig(BicepConfig? bicepConfig);

        BicepConfig? GetActiveBicepConfig();
    }
}
