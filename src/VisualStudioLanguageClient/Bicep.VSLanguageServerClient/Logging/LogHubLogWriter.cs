// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Bicep.VSLanguageServerClient.Logging
{
    internal abstract class LogHubLogWriter
    {
        public abstract TraceSource GetTraceSource();

        public abstract void TraceInformation(string format, params object[] args);

        public abstract void TraceWarning(string format, params object[] args);

        public abstract void TraceError(string format, params object[] args);
    }
}
