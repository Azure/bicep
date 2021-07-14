// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandlerBase
    {
        private readonly ICompilationManager compilationManager;
        private readonly IFileResolver fileResolver;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, IFileResolver fileResolver)
        {
            this.compilationManager = compilationManager;
            this.fileResolver = fileResolver;
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            compilationManager.HandleFileChanges(request.Changes);

            return Unit.Task;
        }

        protected override DidChangeWatchedFilesRegistrationOptions CreateRegistrationOptions(DidChangeWatchedFilesCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            // These file watcher globs should be kept in-sync with those defined in client.ts
            Watchers = new Container<FileSystemWatcher>(
                    new FileSystemWatcher()
                    {
                        Kind = WatchKind.Create | WatchKind.Change | WatchKind.Delete,
                        GlobPattern = "**/"
                    },
                    new FileSystemWatcher()
                    {
                        Kind = WatchKind.Create | WatchKind.Change | WatchKind.Delete,
                        GlobPattern = "**/*.bicep"
                    },
                    new FileSystemWatcher()
                    {
                        Kind = WatchKind.Create | WatchKind.Change | WatchKind.Delete,
                        GlobPattern = "**/*.{json,jsonc,arm}"
                    }
                )
        };
    }
}
