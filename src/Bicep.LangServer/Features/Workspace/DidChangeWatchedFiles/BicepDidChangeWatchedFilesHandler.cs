// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using Bicep.LanguageServer.BicepConfig;
using Bicep.LanguageServer.Compilation;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using FileSystemWatcher = OmniSharp.Extensions.LanguageServer.Protocol.Models.FileSystemWatcher;

namespace Bicep.LanguageServer.Features.Workspace.DidChangeWatchedFiles
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandlerBase
    {
        private readonly ICompilationManager compilationManager;
        private readonly IBicepConfigLifecycleManager bicepConfigLifecycleManager;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, IBicepConfigLifecycleManager bicepConfigLifecycleManager)
        {
            this.bicepConfigLifecycleManager = bicepConfigLifecycleManager;
            this.compilationManager = compilationManager;
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            Container<FileEvent> fileEvents = request.Changes;
            IEnumerable<FileEvent> bicepConfigFileChangeEvents = fileEvents.Where(x => string.Equals(Path.GetFileName(x.Uri.Path),
                                                                                       LanguageConstants.BicepConfigurationFileName,
                                                                                       StringComparison.OrdinalIgnoreCase));

            // Refresh compilation of source files in workspace when local bicepconfig.json file is created, deleted or changed
            if (bicepConfigFileChangeEvents.Any())
            {
                bicepConfigLifecycleManager.RefreshCompilationOfSourceFilesInWorkspace();
            }

            compilationManager.HandleFileChanges(fileEvents);

            return Unit.Task;
        }

        protected override DidChangeWatchedFilesRegistrationOptions CreateRegistrationOptions(DidChangeWatchedFilesCapability capability, global::OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities clientCapabilities) => new()
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
