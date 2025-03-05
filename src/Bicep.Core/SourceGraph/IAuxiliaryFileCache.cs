// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Core.Caching;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface IAuxiliaryFileCache : IConcurrentCache<IOUri, ResultWithDiagnosticBuilder<AuxiliaryFile>>
    {
    }
}
