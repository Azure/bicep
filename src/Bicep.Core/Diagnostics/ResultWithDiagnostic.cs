// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;

namespace Bicep.Core.Diagnostics;

public class ResultWithDiagnostic<TSuccess> : Result<TSuccess, DiagnosticBuilder.ErrorBuilderDelegate>
    where TSuccess : class
{
    public ResultWithDiagnostic(TSuccess success) : base(success) { }

    public ResultWithDiagnostic(DiagnosticBuilder.ErrorBuilderDelegate error) : base(error) { }
}