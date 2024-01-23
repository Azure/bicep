// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;

namespace Bicep.Core.Diagnostics;

public class ResultWithException<TSuccess> : Result<TSuccess, Exception>
    where TSuccess : class
{
    public ResultWithException(TSuccess success) : base(success) { }

    public ResultWithException(Exception exception) : base(exception) { }
}
