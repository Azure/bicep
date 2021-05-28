// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyerFixableDiagnostic : IDiagnostic, IFixable
    {
    }
}
