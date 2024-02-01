// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Utils;

public class ResultWithException<TSuccess> : Result<TSuccess, Exception>
    where TSuccess : class
{
    public ResultWithException(TSuccess success) : base(success) { }

    public ResultWithException(Exception exception) : base(exception) { }

    public TSuccess UnwrapOrThrow()
    => IsSuccess(out var success, out var exception) ? success : throw exception;
}
