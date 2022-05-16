// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace Bicep.VSLanguageServerClient.Logging
{
    internal class DefaultLogHubLogWriter : LogHubLogWriter, IDisposable
    {
        private TraceSource? _traceSource;

        private TraceSource TraceSource
        {
            get
            {
                if (_traceSource is null)
                {
                    throw new ObjectDisposedException($"{nameof(DefaultLogHubLogWriter)} called after being disposed.");
                }

                return _traceSource;
            }
        }

        public DefaultLogHubLogWriter(TraceSource traceSource)
        {
            if (traceSource is null)
            {
                throw new ArgumentNullException(nameof(traceSource));
            }

            _traceSource = traceSource;
        }

        public override TraceSource GetTraceSource() => TraceSource;

        public override void TraceInformation(string format, params object[] args)
        {
            TraceSource.TraceInformation(format, args);
        }

        public override void TraceWarning(string format, params object[] args)
        {
            TraceSource.TraceEvent(TraceEventType.Warning, id: 0, format, args);
        }

        public override void TraceError(string format, params object[] args)
        {
            TraceSource.TraceEvent(TraceEventType.Error, id: 0, format, args);
        }

        public void Dispose()
        {
            _traceSource?.Flush();
            _traceSource?.Close();
            _traceSource = null;
        }
    }
}
