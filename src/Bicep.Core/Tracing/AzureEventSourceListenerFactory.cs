// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core.Diagnostics;
using Bicep.Core.Features;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;

namespace Bicep.Core.Tracing
{
    /// <summary>
    /// Listens to SDK events and redirects them to Trace.WriteLine(). In certain cases, we will strip HTTP headers out from the events that are written out.
    /// </summary>
    public static class AzureEventSourceListenerFactory
    {
        public static AzureEventSourceListener Create(TraceVerbosity verbosity) => new((eventArgs, formattedMessage) => WriteTraceLine(eventArgs, formattedMessage, verbosity), EventLevel.LogAlways);

        private static void WriteTraceLine(EventWrittenEventArgs eventArgs, string formattedMessage, TraceVerbosity verbosity)
        {
            var updatedMessage = GetFormattedMessageWithoutHeaders(
                verbosity,
                eventArgs.EventSource.Name,
                eventArgs.Message,
                eventArgs.PayloadNames,
                eventArgs.Payload) ?? formattedMessage;

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", eventArgs.Level, updatedMessage), eventArgs.EventSource.Name);
        }

        // EventWrittenEventArgs does not appear mockable, so we will expose this method with deconstructed parameters
        public static string? GetFormattedMessageWithoutHeaders(TraceVerbosity verbosity, string eventSourceName, string formatString, ReadOnlyCollection<string> parameterNames, ReadOnlyCollection<object> parameterValues)
        {
            if (verbosity == TraceVerbosity.Full || !string.Equals(eventSourceName, "Azure-Core"))
            {
                // we have full verbosity enabled or the event came from non Azure-Core source
                // (non Azure-Core sources do not appear to produce large traces)
                return null;
            }

            var headerIndex = IndexOfHeaders(parameterNames);
            if (headerIndex < 0)
            {
                // no headers on Azure-Core event
                return null;
            }

            // Azure-Core insists on logging a fairly large dump of headers and there's no way to opt out
            var parameters = parameterValues.ToArray();

            // remove headers
            parameters[headerIndex] = string.Empty;

            // reformat the message
            return string.Format(CultureInfo.InvariantCulture, formatString, parameters);
        }

        private static int IndexOfHeaders(ReadOnlyCollection<string> payloadNames)
        {
            // Most common events from Azure-Core have "headers"
            // and those events have it as the 2nd to last payload
            for (var i = payloadNames.Count - 1; i >= 0; i--)
            {
                var current = payloadNames[i];
                if (string.Equals(current, "headers"))
                {
                    return i;
                }
            }

            // headers parameter is not present
            return -1;
        }
    }
}
