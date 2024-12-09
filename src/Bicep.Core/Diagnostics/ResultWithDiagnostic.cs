// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;

namespace Bicep.Core.Diagnostics;

public class ResultWithDiagnostic<TSuccess> : Result<TSuccess, IDiagnostic>
{
    public ResultWithDiagnostic(TSuccess success) : base(success) { }

    public ResultWithDiagnostic(IDiagnostic diagnostic) : base(diagnostic) { }

    public static implicit operator ResultWithDiagnostic<TSuccess>(TSuccess success) => new(success);
}
