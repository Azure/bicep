// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    public class TelemetryAndErrorHandlingHelper<T>
    {
        private readonly IWindowLanguageServer window;
        private readonly ITelemetryProvider telemetryProvider;

        public class TelemetryAndErrorHandlingException : Exception
        {
            public BicepTelemetryEvent TelemetryEvent { get; }
            public T ErrorResponse { get; }

            public TelemetryAndErrorHandlingException(string message, BicepTelemetryEvent telemetryEvent, T errorResponse)
                : base(message)
            {
                TelemetryEvent = telemetryEvent;
                ErrorResponse = errorResponse;
            }
        }

        public TelemetryAndErrorHandlingHelper(
            IWindowLanguageServer window,
            ITelemetryProvider telemetryProvider)
        {
            this.window = window;
            this.telemetryProvider = telemetryProvider;
        }

        public TelemetryAndErrorHandlingException CreateException(string message, BicepTelemetryEvent telemetryEvent, T errorResponse)
        {
            return new TelemetryAndErrorHandlingException(message, telemetryEvent, errorResponse);
        }

        public async Task<T> ExecuteWithTelemetryAndErrorHandling(Func<Task<(T result, BicepTelemetryEvent? successTelemetry)>> executeFunc)
        {
            try
            {
                var (result, successTelemetry) = await executeFunc();
                if (successTelemetry is not null)
                {
                    telemetryProvider.PostEvent(successTelemetry);
                }

                return result;
            }
            catch (TelemetryAndErrorHandlingException ex)
            {
                window.ShowError(ex.Message);
                telemetryProvider.PostEvent(ex.TelemetryEvent);

                return ex.ErrorResponse;
            }
            catch (Exception ex)
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.UnhandledException(ex));
                throw;
            }
        }
    }
}
