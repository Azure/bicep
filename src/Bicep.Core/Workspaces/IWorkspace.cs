// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Configuration;

namespace Bicep.Core.Workspaces
{
    public interface IWorkspace : IReadOnlyWorkspace
    {
        (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpsertSourceFiles(IEnumerable<ISourceFile> sourceFiles);

        void RemoveSourceFiles(IEnumerable<ISourceFile> sourceFiles);

        void UpsertBicepConfig(Uri uri, BicepConfig bicepConfig);

        BicepConfig? GetBicepConfig(Uri uri);

        void RemoveBicepConfig(Uri uri);
    }
}
