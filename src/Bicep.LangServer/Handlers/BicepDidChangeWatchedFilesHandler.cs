// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandler
    {
        private readonly ICompilationManager compilationManager;
        private readonly IFileResolver fileResolver;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, IFileResolver fileResolver)
            : base(GetDidChangeWatchedFilesRegistrationOptions())
        {
            this.compilationManager = compilationManager;
            this.fileResolver = fileResolver;
        }

        private static DidChangeWatchedFilesRegistrationOptions GetDidChangeWatchedFilesRegistrationOptions()
            => new DidChangeWatchedFilesRegistrationOptions()
            {
                Watchers = new Container<FileSystemWatcher>(
                    new FileSystemWatcher()
                    {
                        Kind = WatchKind.Create | WatchKind.Change | WatchKind.Delete,
                        GlobPattern = "**/*.*"
                    }
                )
            };

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            compilationManager.HandleFileChanges(request.Changes);

            return Unit.Task;
        }
    }
}
