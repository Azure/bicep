// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Utils;

namespace Bicep.Core.UnitTests.Utils;

public static class ResultHelper
{
    public static Result<TSuccess, TError> Create<TSuccess, TError>(TSuccess? success, TError? error)
        where TSuccess : class
        where TError : class
        => (success, error) switch
        {
            ({ }, null) => new(success),
            (null, { }) => new(error),
            (null, null) => throw new InvalidOperationException($"{nameof(success)} and {nameof(error)} cannot both be null"),
            _ => throw new InvalidOperationException($"{nameof(success)} and {nameof(error)} cannot both be non-null"),
        };

    public static ResultWithDiagnostic<TSuccess> Create<TSuccess>(TSuccess? success, DiagnosticBuilder.ErrorBuilderDelegate? error)
        where TSuccess : class
        => (success, error) switch
        {
            ({ }, null) => new(success),
            (null, { }) => new(error),
            (null, null) => throw new InvalidOperationException($"{nameof(success)} and {nameof(error)} cannot both be null"),
            _ => throw new InvalidOperationException($"{nameof(success)} and {nameof(error)} cannot both be non-null"),
        };
}
