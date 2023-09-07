// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

    public bool IsSuccess([NotNullWhen(true)] out TSuccess? success, [NotNullWhen(false)] out TError? error)
    {
        success = Success;
        error = Error;

        return success is not null;
    }
}