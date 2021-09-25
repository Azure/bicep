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
    internal class OperationOrResponseInternals<T> : OperationOrResponseInternals
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly OperationInternals<T>? _operation;
        private readonly Response<T>? _valueResponse;

        public OperationOrResponseInternals(
            IOperationSource<T> source,
            ClientDiagnostics clientDiagnostics,
            HttpPipeline pipeline,
            Request originalRequest,
            Response originalResponse,
            OperationFinalStateVia finalStateVia,
            string scopeName)
            : base(new OperationInternals<T>(source, clientDiagnostics, pipeline, originalRequest, originalResponse, finalStateVia, scopeName))
        {
            _operation = Operation as OperationInternals<T>;
        }

        public OperationOrResponseInternals(Response<T> response)
            : base(response.GetRawResponse())
        {
            _valueResponse = response;
        }

        public T Value => DoesWrapOperation ? _operation!.Value : _valueResponse!.Value;

        public bool HasValue => DoesWrapOperation ? _operation!.HasValue : true;

        public async ValueTask<Response<T>> WaitForCompletionAsync(
            CancellationToken cancellationToken = default)
        {
            return await WaitForCompletionAsync(OperationInternals<T>.DefaultPollingInterval, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<Response<T>> WaitForCompletionAsync(
            TimeSpan pollingInterval,
            CancellationToken cancellationToken)
        {
            return DoesWrapOperation
                ? await _operation!.WaitForCompletionAsync(pollingInterval, cancellationToken).ConfigureAwait(false)
                : _valueResponse!;
        }
    }
}
