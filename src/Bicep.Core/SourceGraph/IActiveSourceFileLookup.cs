// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface IActiveSourceFileLookup : IEnumerable<ISourceFile>
    {
        ISourceFile? TryGetSourceFile(IOUri fileUri);

        bool HasSourceFile(IOUri fileUri);
    }
}
