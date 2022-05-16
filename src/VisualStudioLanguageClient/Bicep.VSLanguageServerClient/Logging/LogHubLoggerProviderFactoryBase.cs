// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Bicep.VSLanguageServerClient.Logging
{
    public abstract class LogHubLoggerProviderFactoryBase : LogHubLoggerProviderFactory
    {
        // Unique, monotomically increasing ID to identify loghub session to persist
        // across server restarts.
        private static int s_logHubSessionId;

        private readonly BicepLogHubTraceProvider _traceProvider;
        private readonly SemaphoreSlim _initializationSemaphore;
        private DefaultLogHubLogWriter? _currentLogWriter;

        public LogHubLoggerProviderFactoryBase(BicepLogHubTraceProvider traceProvider)
        {
            if (traceProvider is null)
            {
                throw new System.ArgumentNullException(nameof(traceProvider));
            }

            _traceProvider = traceProvider;

            _initializationSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        }

        public async override Task<object> GetOrCreateAsync(string logIdentifier, CancellationToken cancellationToken)
        {
            await _initializationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_currentLogWriter != null)
                {
                    // Dispose last log writer so we can start a new session. Technically only one should only ever be active at a time.
                    _currentLogWriter.Dispose();
                }

                var logInstanceNumber = Interlocked.Increment(ref s_logHubSessionId);
                var traceSource = await _traceProvider.InitializeTraceAsync(logIdentifier, logInstanceNumber, cancellationToken).ConfigureAwait(false);

                _currentLogWriter = new DefaultLogHubLogWriter(traceSource!);
                var provider = new LogHubLoggerProvider(_currentLogWriter);

                return provider;
            }
            finally
            {
                _initializationSemaphore.Release();
            }
        }
    }
}
