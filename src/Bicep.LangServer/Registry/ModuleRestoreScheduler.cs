// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Registry
{
    public sealed class ModuleRestoreScheduler : IModuleRestoreScheduler, IAsyncDisposable
    {
        private record QueueItem(ICompilationManager CompilationManager, DocumentUri Uri, ImmutableArray<ModuleReference> ModuleReferences);

        private record CompletionNotification(ICompilationManager CompilationManager, DocumentUri Uri);

        private readonly IModuleDispatcher moduleDispatcher;

        private readonly Queue<QueueItem> queue = new();

        private readonly CancellationTokenSource cancellationTokenSource = new();

        // block on initial wait until signaled
        // wait times are expected to be long, so we are intentionally not using the Slim variant
        private readonly ManualResetEvent manualResetEvent = new(false);

        private bool disposed = false;
        private Task? consumerTask;

        public ModuleRestoreScheduler(IModuleDispatcher moduleDispatcher)
        {
            this.moduleDispatcher = moduleDispatcher;
        }

        /// <summary>
        /// Requests that the specified modules be restored to the local file system.
        /// Does not wait for the operation to complete and returns immediately.
        /// </summary>
        /// <param name="modules">The module references</param>
        public void RequestModuleRestore(ICompilationManager compilationManager, DocumentUri documentUri, IEnumerable<ModuleDeclarationSyntax> modules)
        {
            this.CheckDisposed();

            var moduleReferences = this.moduleDispatcher.GetValidModuleReferences(modules).ToImmutableArray();
            var item = new QueueItem(compilationManager, documentUri, moduleReferences);
            lock (this.queue)
            {
                this.queue.Enqueue(item);

                // notify consumer about new items
                this.manualResetEvent.Set();
            }
        }

        public void Start()
        {
            this.CheckDisposed();
            this.consumerTask = Task.Run(this.ProcessQueueItemsAsync, this.cancellationTokenSource.Token);
        }

        public async ValueTask DisposeAsync()
        {
            // this is a sealed class - no need for full IDisposable implementation
            if (!this.disposed)
            {
                this.disposed = true;
                if (this.consumerTask is not null)
                {
                    // signal cancellation first
                    this.cancellationTokenSource.Cancel();

                    lock(this.queue)
                    {
                        // unblock the background task
                        // this MUST happen after cancellation is signaled so the task immediately cancels
                        this.manualResetEvent.Set();
                    }

                    try
                    {
                        await this.consumerTask;
                    }
                    catch
                    {
                        // the task never completes and can only be canceled
                        // which is signaled to us by an exception
                    }

                    this.consumerTask = null;
                }
            }
        }

        private async Task ProcessQueueItemsAsync()
        {
            var token = this.cancellationTokenSource.Token;
            while (true)
            {
                // the non-slim MRE doesn't support a cancellation token,
                // so DisposeAsync will signal the task AFTER cancellation to release it
                this.manualResetEvent.WaitOne();
                token.ThrowIfCancellationRequested();

                var notifications = new HashSet<CompletionNotification>();
                var moduleReferences = new List<ModuleReference>();
                lock (this.queue)
                {
                    this.UnsafeCollectModuleReferences(notifications, moduleReferences);
                    Debug.Assert(this.queue.Count == 0, "this.queue.Count == 0");

                    // queue has been consumed - next iteration should block until more items have been added
                    this.manualResetEvent.Reset();
                }

                // this blocks until restore is completed
                // the dispatcher stores the results internally and manages their lifecycle
                token.ThrowIfCancellationRequested();
                if(!await this.moduleDispatcher.RestoreModules(moduleReferences))
                {
                    // nothing needed to be restored
                    // no need to notify about completion
                    continue;
                }

                // notify compilation manager that restore is completed
                // to recompile the affected modules
                token.ThrowIfCancellationRequested();
                foreach (var notification in notifications)
                {
                    notification.CompilationManager.RefreshCompilation(notification.Uri);
                }

                this.moduleDispatcher.PruneRestoreStatuses();
            }
        }

        private void UnsafeCollectModuleReferences(HashSet<CompletionNotification> notifications, List<ModuleReference> moduleReferences)
        {
            while (this.queue.TryDequeue(out var item))
            {
                // the record implementation combined with the hashset will dedupe compilation manager/Uri combinations
                notifications.Add(new(item.CompilationManager, item.Uri));
                moduleReferences.AddRange(item.ModuleReferences);
            }
        }

        private void CheckDisposed()
        {
            if(this.disposed)
            {
                throw new ObjectDisposedException($"The {nameof(ModuleRestoreScheduler)} has already been disposed.", innerException: null);
            }
        }
    }
}
