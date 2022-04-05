// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyerFixableDiagnostic : IDiagnostic, IFixable
    {
    }
}
