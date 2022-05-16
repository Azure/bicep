// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Bicep.VSLanguageServerClient.Logging
{
    public abstract class LogHubLoggerProviderFactory
    {
        /// <summary>
        /// Returns a <see cref="LogHubLoggerProvider"/>. This ensures we don't load an extra logging dll at MEF load time in Visual Studio.
        /// </summary>
        /// <remarks>When VS looks for MEF exports it has to load assembly types that correspond to a contracts signature. SO in our case we used to
        /// return a <see cref="LogHubLoggerProvider"/>; however, that resulted in MEF needing to load the ILoggerProvider (what it implements)
        /// assembly which was Microsoft.Extensions.Logging.Abstractions. This wasn't great because it required MEF to then load that assembly in order
        /// to understand this type. Returning <c>object</c> works around requiring the logging assembly.</remarks>
        /// <param name="logIdentifier">An identifier to prefix the log hub file name with.</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>A created <c>Task</c> <see cref="LogHubLoggerProvider"/>.</returns>
        public abstract Task<object> GetOrCreateAsync(string logIdentifier, CancellationToken token);
    }
}
