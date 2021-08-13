// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Pipeline;

namespace Azure.Core
{
    /// <summary>
    /// This implements the ARM scenarios for LROs. It is highly recommended to read the ARM spec prior to modifying this code:
    /// https://github.com/Azure/azure-resource-manager-rpc/blob/master/v1.0/Addendum.md#asynchronous-operations
    /// Other reference documents include:
    /// https://github.com/Azure/autorest/blob/master/docs/extensions/readme.md#x-ms-long-running-operation
    /// https://github.com/Azure/adx-documentation-pr/blob/master/sdks/LRO/LRO_AzureSDK.md
    /// </summary>
    /// <typeparam name="T">The final result of the LRO.</typeparam>
#pragma warning disable SA1649 // File name should match first type name
    internal class OperationInternals<T> : OperationInternals
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly IOperationSource<T> _source;
        private T _value = default!;
        private bool _hasValue;

        public OperationInternals(
            IOperationSource<T> source,
            ClientDiagnostics clientDiagnostics,
            HttpPipeline pipeline,
            Request originalRequest,
            Response originalResponse,
            OperationFinalStateVia finalStateVia,
            string scopeName)
            : base(clientDiagnostics, pipeline, originalRequest, originalResponse, finalStateVia, scopeName)
        {
            _source = source;
        }

        public ValueTask<Response<T>> WaitForCompletionAsync(CancellationToken cancellationToken = default)
        {
            return WaitForCompletionAsync(DefaultPollingInterval, cancellationToken);
        }

        public async ValueTask<Response<T>> WaitForCompletionAsync(TimeSpan pollingInterval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UpdateStatusAsync(cancellationToken).ConfigureAwait(false);
                if (HasCompleted)
                {
                    return Response.FromValue(Value, GetRawResponse());
                }

                await Task.Delay(pollingInterval, cancellationToken).ConfigureAwait(false);
            }
        }

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("The operation has not completed yet.");
                }

                return _value;
            }
        }

        public bool HasValue => _hasValue;

        protected override async Task SetValueAsync(bool async, Response finalResponse, CancellationToken cancellationToken = default)
        {
            _value = async
                ? await _source.CreateResultAsync(finalResponse, cancellationToken).ConfigureAwait(false)
                : _source.CreateResult(finalResponse, cancellationToken);
            _hasValue = true;
        }
    }
}
