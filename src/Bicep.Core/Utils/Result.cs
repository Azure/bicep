// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.Diagnostics;
using JetBrains.Annotations;

namespace Bicep.Core.Utils;

public class Result<TSuccess, TError>
    where TSuccess : class
    where TError : class
{
    TSuccess? Success { get; }

    TError? Error { get; }

    public Result(TSuccess success)
    {
        Success = success;
        Error = null;
    }

    public Result(TError error)
    {
        Success = null;
        Error = error;
    }

    public bool IsSuccess()
        => Success is not null;

    public bool IsSuccess([NotNullWhen(true)] out TSuccess? success, [NotNullWhen(false)] out TError? error)
    {
        success = Success;
        error = Error;

        return IsSuccess();
    }

    /// <summary>
    /// Returns the succcessful result, assuming success. Throws an exception if not.
    /// </summary>
    public TSuccess Unwrap()
        => TryUnwrap() ?? throw new InvalidOperationException($"{nameof(Success)} was not set!");

    public TSuccess? TryUnwrap()
        => Success;
}
