// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bicep.VSLanguageServerClient.Logging
{
    [Export(typeof(BicepLanguageServerLogHubLoggerProvider))]
    public class BicepLanguageServerLogHubLoggerProvider : ILoggerProvider
    {
        private const string LogFileIdentifier = "Razor.HTMLCSharpLanguageServerClient";

        private LogHubLoggerProvider? _loggerProvider;

        // Internal for testing / do not remove, used by Moq to construct
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal BicepLanguageServerLogHubLoggerProvider()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        private LogHubLoggerProvider LoggerProvider
        {
            get
            {
                if (_loggerProvider is null)
                {
                    throw new InvalidOperationException($"{nameof(LoggerProvider)} accessed before being set.");
                }

                return _loggerProvider;
            }
        }

        private readonly BicepLanguageServerLogHubLoggerProviderFactory _loggerFactory;
        private readonly SemaphoreSlim _initializationSemaphore;

        [ImportingConstructor]
        public BicepLanguageServerLogHubLoggerProvider(BicepLanguageServerLogHubLoggerProviderFactory loggerFactory)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _loggerFactory = loggerFactory;

            _initializationSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        }

        // Virtual for testing
        public virtual async Task InitializeLoggerAsync(CancellationToken cancellationToken)
        {
            if (_loggerProvider is not null)
            {
                return;
            }

            await _initializationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (_loggerProvider is null)
                {
                    _loggerProvider = (LogHubLoggerProvider)await _loggerFactory.GetOrCreateAsync(LogFileIdentifier, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _initializationSemaphore.Release();
            }
        }

        // Virtual for testing
        public virtual ILogger CreateLogger(string categoryName)
        {
            return LoggerProvider.CreateLogger(categoryName);
        }

        public virtual async Task<ILogger> CreateLoggerAsync(string categoryName, CancellationToken cancellationToken)
        {
            await InitializeLoggerAsync(cancellationToken);
            return CreateLogger(categoryName);
        }

        public TraceSource GetTraceSource()
        {
            return LoggerProvider.GetTraceSource();
        }

        public void Dispose()
        {
        }
    }
}
