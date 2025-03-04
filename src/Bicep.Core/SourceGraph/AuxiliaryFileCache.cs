// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Caching;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public class AuxiliaryFileCache : ConcurrentCache<IOUri, ResultWithDiagnosticBuilder<AuxiliaryFile>>, IAuxiliaryFileCache
    {
    }
}
