// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Configuration;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using FileSystemWatcher = OmniSharp.Extensions.LanguageServer.Protocol.Models.FileSystemWatcher;

namespace Bicep.LanguageServer.Handlers
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandlerBase
    {
        private readonly ICompilationManager compilationManager;
        private readonly IBicepConfigChangeHandler bicepConfigChangeHandler;
        private readonly IWorkspace workspace;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, IBicepConfigChangeHandler bicepConfigChangeHandler, IWorkspace workspace)
        {
            this.bicepConfigChangeHandler = bicepConfigChangeHandler;
            this.compilationManager = compilationManager;
            this.workspace = workspace;
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            Container<FileEvent> fileEvents = request.Changes;

            List<FileEvent> bicepConfigFileChangeEvents = fileEvents
                .Where(x => string.Equals(Path.GetFileName(x.Uri.Path), LanguageConstants.BicepConfigurationFileName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var configFileChangeEvent in bicepConfigFileChangeEvents)
            {
                bicepConfigChangeHandler.HandleBicepConfigChangeEvent(configFileChangeEvent.Uri);
            }

            // Refresh compilation of source files in workspace when local bicepconfig.json file is created, deleted or changed
            if (bicepConfigFileChangeEvents.Any())
            {
                foreach (var uri in workspace.GetActiveSourceFilesByUri().Keys.Select(DocumentUri.From))
                {
                    compilationManager.RefreshCompilation(uri);
                }
            }

            compilationManager.HandleFileChanges(fileEvents);

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
