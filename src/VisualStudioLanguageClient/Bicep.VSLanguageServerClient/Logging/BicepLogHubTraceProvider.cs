// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.VSLanguageServerClient.Logging
{
    public abstract class BicepLogHubTraceProvider
    {
        public abstract Task<TraceSource?> InitializeTraceAsync(string logIdentifier, int logHubSessionId, CancellationToken cancellationToken);
    }
}
