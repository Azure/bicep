// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyzerDiagnostic : IDiagnostic
    {
        public string AnalyzerName { get; }
    }
}
