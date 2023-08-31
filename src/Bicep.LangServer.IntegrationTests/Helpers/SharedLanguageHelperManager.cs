// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;

namespace Bicep.LangServer.IntegrationTests.Helpers
{
    public sealed class SharedLanguageHelperManager : System.IAsyncDisposable
    {
        private AsyncLazy<MultiFileLanguageServerHelper>? lazy = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD012:Provide JoinableTaskFactory where allowed", Justification = "<Pending>")]
        public void Initialize(Func<Task<MultiFileLanguageServerHelper>> helperCreator)
        {
            if (this.lazy is null)
            {
                this.lazy = new AsyncLazy<MultiFileLanguageServerHelper>(helperCreator);
                return;
            }

            throw new AssertFailedException("Already initialized");
        }

        public async Task<MultiFileLanguageServerHelper> GetAsync()
        {
            if (this.lazy is not null)
            {
                return await this.lazy.GetValueAsync();
            }

            throw new AssertFailedException($"Not yet initialized.");
        }

        public async ValueTask DisposeAsync()
        {
            if (this.lazy is null)
            {
                return;
            }

            var helper = await this.lazy.GetValueAsync();
            helper.Dispose();
        }
    }
}
