// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace Bicep.Core.Tracing
{
    public sealed class ExecutionTimer : IDisposable
    {
        private readonly string description;
        private readonly Stopwatch stopwatch;

        private Status status = Status.Started;
        private string? error = null;

        public ExecutionTimer(string description, bool logInitial = true)
        {
            this.description = description;
            this.stopwatch = Stopwatch.StartNew();
            if (logInitial)
            {
                this.WriteStatus();
            }
        }

        public void OnFail(string? error = null)
        {
            this.stopwatch.Stop();
            this.status = Status.Failed;
            this.error = error;
        }

        public void Dispose()
        {
            this.stopwatch.Stop();
            if(this.status == Status.Started)
            {
                this.status = Status.Succeeded;
            }

            this.WriteStatus();
        }

        private void WriteStatus()
        {
            var elapsedText = this.status != Status.Started ? $" (Elapsed = {this.stopwatch.ElapsedMilliseconds} ms)" : string.Empty;
            var errorSuffix = this.error is null ? string.Empty : $" Error: {this.error}";

            Trace.WriteLine($"{this.description}: {this.status}{elapsedText}{errorSuffix}");
        }

        private enum Status
        {
            Started,
            Succeeded,
            Failed
        }
    }
}
