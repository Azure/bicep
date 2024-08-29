// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;

namespace Bicep.Core.Diagnostics;

public class ResultWithDiagnosticBuilder<TSuccess> : Result<TSuccess, DiagnosticBuilder.DiagnosticBuilderDelegate>
{
    public ResultWithDiagnosticBuilder(TSuccess success) : base(success) { }

    public ResultWithDiagnosticBuilder(DiagnosticBuilder.DiagnosticBuilderDelegate error) : base(error) { }
}
