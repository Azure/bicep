﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
    internal class OperationInternals
    {
        public static TimeSpan DefaultPollingInterval { get; } = TimeSpan.FromSeconds(1);

        private static readonly string[] s_failureStates = { "failed", "canceled" };
        private static readonly string[] s_terminalStates = { "succeeded", "failed", "canceled" };

        private readonly HttpPipeline _pipeline;
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly string _scopeName;
        private readonly RequestMethod _requestMethod;
        private readonly Uri _originalUri;
        private readonly OperationFinalStateVia _finalStateVia;
        private HeaderFrom _headerFrom;
        private string _pollUri = default!;
        private bool _originalHasLocation;
        private string? _lastKnownLocation;

        private Response _rawResponse;
        private bool _hasCompleted;
        private bool _shouldPoll;

        public OperationInternals(
            ClientDiagnostics clientDiagnostics,
            HttpPipeline pipeline,
            Request originalRequest,
            Response originalResponse,
            OperationFinalStateVia finalStateVia,
            string scopeName)
        {
            _rawResponse = originalResponse;
            _requestMethod = originalRequest.Method;
            _originalUri = originalRequest.Uri.ToUri();
            _finalStateVia = finalStateVia;
            InitializeScenarioInfo();

            _pipeline = pipeline;
            _clientDiagnostics = clientDiagnostics;
            _scopeName = scopeName;
            // When the original response has no headers, we do not start polling immediately.
            _shouldPoll = _headerFrom != HeaderFrom.None;
        }

        public Response GetRawResponse() => _rawResponse;

        public ValueTask<Response> WaitForCompletionResponseAsync(CancellationToken cancellationToken = default)
        {
            return WaitForCompletionResponseAsync(DefaultPollingInterval, cancellationToken);
        }

        public async ValueTask<Response> WaitForCompletionResponseAsync(TimeSpan pollingInterval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UpdateStatusAsync(cancellationToken).ConfigureAwait(false);
                if (HasCompleted)
                {
                    return GetRawResponse();
                }

                await Task.Delay(pollingInterval, cancellationToken).ConfigureAwait(false);
            }
        }

        private async ValueTask<Response> UpdateStatusAsync(bool async, CancellationToken cancellationToken)
        {
            if (_hasCompleted)
            {
                return GetRawResponse();
            }

            if (_shouldPoll)
            {
                UpdatePollUri();
                _rawResponse = async
                    ? await GetResponseAsync(_pollUri, cancellationToken).ConfigureAwait(false)
                    : GetResponse(_pollUri, cancellationToken);
            }

            _shouldPoll = true;
            _hasCompleted = IsTerminalState(out string state);
            if (_hasCompleted)
            {
                Response finalResponse = GetRawResponse();
                if (s_failureStates.Contains(state))
                {
                    throw _clientDiagnostics.CreateRequestFailedException(finalResponse);
                }

                string? finalUri = GetFinalUri();
                if (finalUri != null)
                {
                    finalResponse = async
                        ? await GetResponseAsync(finalUri, cancellationToken).ConfigureAwait(false)
                        : GetResponse(finalUri, cancellationToken);
                }

                switch (finalResponse.Status)
                {
                    case 200:
                    case 201 when _requestMethod == RequestMethod.Put:
                    case 204 when !(_requestMethod == RequestMethod.Put || _requestMethod == RequestMethod.Patch):
                        {
                            await SetValueAsync(async, finalResponse, cancellationToken).ConfigureAwait(false);
                            _rawResponse = finalResponse;
                            break;
                        }
                    default:
                        throw _clientDiagnostics.CreateRequestFailedException(finalResponse);
                }
            }

            return GetRawResponse();
        }

        protected virtual Task SetValueAsync(bool async, Response finalResponse, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async ValueTask<Response> UpdateStatusAsync(CancellationToken cancellationToken = default) => await UpdateStatusAsync(async: true, cancellationToken).ConfigureAwait(false);

        public Response UpdateStatus(CancellationToken cancellationToken = default) => UpdateStatusAsync(async: false, cancellationToken).EnsureCompleted();

#pragma warning disable CA1822
        //TODO: This is currently unused.
        public string Id => throw new NotImplementedException();
#pragma warning restore CA1822

        public bool HasCompleted => _hasCompleted;

        private HttpMessage CreateRequest(string link)
        {
            HttpMessage message = _pipeline.CreateMessage();
            Request request = message.Request;
            request.Method = RequestMethod.Get;

            if (Uri.TryCreate(link, UriKind.Absolute, out var nextLink) && nextLink.Scheme != "file")
            {
                request.Uri.Reset(nextLink);
            }
            else
            {
                request.Uri.Reset(new Uri(_originalUri, link));
            }

            return message;
        }

        private async ValueTask<Response> GetResponseAsync(string link, CancellationToken cancellationToken = default)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            using DiagnosticScope scope = _clientDiagnostics.CreateScope(_scopeName);
            scope.Start();
            try
            {
                using HttpMessage message = CreateRequest(link);
                await _pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
                return message.Response;
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        private Response GetResponse(string link, CancellationToken cancellationToken = default)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            using DiagnosticScope scope = _clientDiagnostics.CreateScope(_scopeName);
            scope.Start();
            try
            {
                using HttpMessage message = CreateRequest(link);
                _pipeline.Send(message, cancellationToken);
                return message.Response;
            }
            catch (Exception e)
            {
                scope.Failed(e);
                throw;
            }
        }

        private bool IsTerminalState(out string state)
        {
            Response response = GetRawResponse();
            state = string.Empty;
            if (_headerFrom == HeaderFrom.Location)
            {
                return response.Status != 202;
            }

            if (response.Status >= 200 && response.Status <= 204)
            {
                if (response.ContentStream?.Length > 0)
                {
                    try
                    {
                        using JsonDocument document = JsonDocument.Parse(response.ContentStream);
                        foreach (JsonProperty property in document.RootElement.EnumerateObject())
                        {
                            if ((_headerFrom == HeaderFrom.OperationLocation ||
                                 _headerFrom == HeaderFrom.AzureAsyncOperation) &&
                                property.NameEquals("status"))
                            {
                                state = property.Value.GetRequiredString().ToLowerInvariant();
                                return s_terminalStates.Contains(state);
                            }

                            if (_headerFrom == HeaderFrom.None && property.NameEquals("properties"))
                            {
                                foreach (JsonProperty innerProperty in property.Value.EnumerateObject())
                                {
                                    if (innerProperty.NameEquals("provisioningState"))
                                    {
                                        state = innerProperty.Value.GetRequiredString().ToLowerInvariant();
                                        return s_terminalStates.Contains(state);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        // It is required to reset the position of the content after reading as this response may be used for deserialization.
                        response.ContentStream.Position = 0;
                    }
                }

                // If provisioningState was not found, it defaults to Succeeded.
                if (_headerFrom == HeaderFrom.None)
                {
                    return true;
                }
            }

            throw _clientDiagnostics.CreateRequestFailedException(response);
        }

        private enum HeaderFrom
        {
            None,
            OperationLocation,
            AzureAsyncOperation,
            Location
        }

        private void InitializeScenarioInfo()
        {
            _originalHasLocation = _rawResponse.Headers.Contains("Location");

            if (_rawResponse.Headers.Contains("Operation-Location"))
            {
                _headerFrom = HeaderFrom.OperationLocation;
                return;
            }

            if (_rawResponse.Headers.Contains("Azure-AsyncOperation"))
            {
                _headerFrom = HeaderFrom.AzureAsyncOperation;
                return;
            }

            if (_originalHasLocation)
            {
                _headerFrom = HeaderFrom.Location;
                return;
            }

            _pollUri = _originalUri.AbsoluteUri;
            _headerFrom = HeaderFrom.None;
        }

        private void UpdatePollUri()
        {
            var hasLocation = _rawResponse.Headers.TryGetValue("Location", out string? location);
            if (hasLocation)
            {
                _lastKnownLocation = location;
            }

            switch (_headerFrom)
            {
                case HeaderFrom.OperationLocation when _rawResponse.Headers.TryGetValue("Operation-Location", out string? operationLocation):
                    _pollUri = operationLocation;
                    return;
                case HeaderFrom.AzureAsyncOperation when _rawResponse.Headers.TryGetValue("Azure-AsyncOperation", out string? azureAsyncOperation):
                    _pollUri = azureAsyncOperation;
                    return;
                case HeaderFrom.Location when hasLocation:
                    _pollUri = location!;
                    return;
            }
        }

        private string? GetFinalUri()
        {
            if (_headerFrom == HeaderFrom.OperationLocation || _headerFrom == HeaderFrom.AzureAsyncOperation)
            {
                if (_requestMethod == RequestMethod.Delete)
                {
                    return null;
                }

                if (_requestMethod == RequestMethod.Put || (_originalHasLocation && _finalStateVia == OperationFinalStateVia.OriginalUri))
                {
                    return _originalUri.AbsoluteUri;
                }

                if (_originalHasLocation && _finalStateVia == OperationFinalStateVia.Location)
                {
                    return _lastKnownLocation;
                }
            }

            return null;
        }
    }
}
