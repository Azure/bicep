// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LanguageServer.Handlers
{
    public class TelemetryAndErrorHandlingException : Exception
    {
        public BicepTelemetryEvent TelemetryEvent { get; }

        public TelemetryAndErrorHandlingException(string message, BicepTelemetryEvent telemetryEvent)
            : base(message)
        {
            TelemetryEvent = telemetryEvent;
        }
    }

    public class TelemetryAndErrorHandlingHelper<T>
    {
        private readonly IWindowLanguageServer window;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly T errorResponse;

        public TelemetryAndErrorHandlingHelper(
            IWindowLanguageServer window,
            ITelemetryProvider telemetryProvider,
            T errorResponse)
        {
            this.window = window;
            this.telemetryProvider = telemetryProvider;
            this.errorResponse = errorResponse;
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

                return errorResponse;
            }
            catch (Exception ex)
            {
                telemetryProvider.PostEvent(BicepTelemetryEvent.UnhandledException(ex));
                throw;
            }
        }
    }
}
