// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Utils;

public class Result<TSuccess, TError>
{
    private readonly bool isSuccess;
    private readonly TSuccess? successResult;
    private readonly TError? errorResult;

    public Result(TSuccess success)
    {
        this.isSuccess = true;
        this.successResult = success;
        this.errorResult = default;
    }

    public Result(TError error)
    {
        this.isSuccess = false;
        this.successResult = default;
        this.errorResult = error;
    }

    public bool IsSuccess([NotNullWhen(true)] out TSuccess? success, [NotNullWhen(false)] out TError? error)
    {
        success = successResult;
        error = errorResult;

        return isSuccess;
    }


    public bool IsSuccess() => IsSuccess(out _, out _);

    public bool IsSuccess([NotNullWhen(true)] out TSuccess? success) => IsSuccess(out success, out _);

    /// <summary>
    /// Returns the successful result, assuming success. Throws an exception if not.
    /// This should only be called if you've already verified that the result is successful.
    /// </summary>
    public TSuccess Unwrap()
        => TryUnwrap() ?? throw new InvalidOperationException($"Cannot unwrap a failed result: {errorResult}.");

    public TSuccess? TryUnwrap()
        => successResult;
}
