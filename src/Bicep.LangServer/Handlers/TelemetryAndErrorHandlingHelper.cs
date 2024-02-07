// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LanguageServer.Handlers
{
    public class TelemetryAndErrorHandlingHelper<T>(
        IWindowLanguageServer window,
        ITelemetryProvider telemetryProvider)
    {
        private readonly IWindowLanguageServer window = window;
        private readonly ITelemetryProvider telemetryProvider = telemetryProvider;

        public class TelemetryAndErrorHandlingException(string message, BicepTelemetryEvent telemetryEvent, T errorResponse) : Exception(message)
        {
            public BicepTelemetryEvent TelemetryEvent { get; } = telemetryEvent;
            public T ErrorResponse { get; } = errorResponse;
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
