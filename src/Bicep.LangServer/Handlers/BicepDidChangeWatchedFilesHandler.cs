// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Configuration;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using FileSystemWatcher = OmniSharp.Extensions.LanguageServer.Protocol.Models.FileSystemWatcher;

namespace Bicep.LanguageServer.Handlers
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandlerBase
    {
        private readonly IWorkspace workspace;
        private readonly ICompilationManager compilationManager;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, IWorkspace workspace)
        {
            this.compilationManager = compilationManager;
            this.workspace = workspace;
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            Container<FileEvent> fileEvents = request.Changes;
            IEnumerable<FileEvent> bicepConfigFileChangeEvents = fileEvents.Where(x => x.Uri.Path.EndsWith(LanguageConstants.BicepConfigSettingsFileName));

            // Refresh compilation of source files in workspace when local bicepconfig.json file is created or deleted
            if (bicepConfigFileChangeEvents.Any(x => (x.Type == FileChangeType.Deleted || x.Type == FileChangeType.Created)))
            {
                Uri uri = bicepConfigFileChangeEvents.First().Uri.ToUri();
                BicepConfigChangeHandler.RefreshCompilationOfSourceFilesInWorkspace(compilationManager, uri, workspace, string.Empty);
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
