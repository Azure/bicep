// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Compilation;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using FileSystemWatcher = OmniSharp.Extensions.LanguageServer.Protocol.Models.FileSystemWatcher;
using LspClientCapabilities = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities;

namespace Bicep.LanguageServer.Features.Workspace.DidChangeWatchedFiles
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandlerBase
    {
        private readonly ICompilationManager compilationManager;
        private readonly FileChangeEpoch fileChangeEpoch;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, FileChangeEpoch fileChangeEpoch)
        {
            this.fileChangeEpoch = fileChangeEpoch;
            this.compilationManager = compilationManager;
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            Container<FileEvent> fileEvents = request.Changes;

            // Caches (such as the configuration manager) check their entries against the file system again in the new epoch,
            // so the compilation manager finds the compilations whose configuration changed.
            fileChangeEpoch.StartNew();

            compilationManager.HandleFileChanges(fileEvents);

            return Unit.Task;
        }

        protected override DidChangeWatchedFilesRegistrationOptions CreateRegistrationOptions(DidChangeWatchedFilesCapability capability, LspClientCapabilities clientCapabilities) => new()
        {
            Watchers = new Container<FileSystemWatcher>(
                    new FileSystemWatcher()
                    {
                        Kind = WatchKind.Create | WatchKind.Change | WatchKind.Delete,
                        // Register to watch all files and folders, regardless of extension, because they could be referenced by load* functions.
                        // We will do the filtering in the language server. This glob pattern should be kept in-sync with client.ts.
                        GlobPattern = new("**/*")
                    }
                )
        };
    }
}
