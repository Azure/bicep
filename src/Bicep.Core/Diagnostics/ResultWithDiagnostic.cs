// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;

namespace Bicep.Core.Diagnostics;

public class ResultWithDiagnostic<TSuccess> : Result<TSuccess, DiagnosticBuilder.DiagnosticBuilderDelegate>
{
    public ResultWithDiagnostic(TSuccess success) : base(success) { }

    public ResultWithDiagnostic(DiagnosticBuilder.DiagnosticBuilderDelegate error) : base(error) { }
}
