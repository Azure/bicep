// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;
using FileSystemWatcher = OmniSharp.Extensions.LanguageServer.Protocol.Models.FileSystemWatcher;

namespace Bicep.LanguageServer.Handlers
{
    public sealed class BicepDidChangeWatchedFilesHandler : DidChangeWatchedFilesHandlerBase
    {
        private readonly IWorkspace workspace;
        private readonly ICompilationManager compilationManager;
        private readonly IFileResolver fileResolver;
        private readonly JSchema bicepConfigSchema;

        public BicepDidChangeWatchedFilesHandler(ICompilationManager compilationManager, IFileResolver fileResolver, IWorkspace workspace)
        {
            this.compilationManager = compilationManager;
            this.fileResolver = fileResolver;
            this.workspace = workspace;

            var assembly = Assembly.GetExecutingAssembly();
            string manifestResourceName = assembly.GetManifestResourceNames().Where(p => p.EndsWith("bicepconfig.schema.json", StringComparison.Ordinal)).First();
            Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
            var streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            bicepConfigSchema = JSchema.Parse(streamReader.ReadToEnd());
        }

        public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            Container<FileEvent> fileEvents = request.Changes;
            IEnumerable<FileEvent> bicepConfigFileChangeEvents = fileEvents.Where(x => x.Uri.Path.EndsWith(LanguageConstants.BicepConfigSettingsFileName));

            if (bicepConfigFileChangeEvents.Any())
            {
                FileEvent bicepConfigFileEvent = bicepConfigFileChangeEvents.First();

                Uri bicepConfigUri = bicepConfigFileEvent.Uri.ToUri();

                fileResolver.TryRead(bicepConfigUri, out string? bicepFileContents, out ErrorBuilderDelegate _);

                if (bicepFileContents is not null && IsBicepConfigValid(bicepFileContents))
                {
                    IEnumerable<ISourceFile> sourceFiles = workspace.GetSourceFilesForDirectory(bicepConfigUri);

                    foreach (ISourceFile sourceFile in sourceFiles)
                    {
                        Uri uri = sourceFile.FileUri;
                        fileResolver.TryRead(uri, out string? fileContents, out ErrorBuilderDelegate _);

                        if (!string.IsNullOrWhiteSpace(fileContents))
                        {
                            compilationManager.UpsertCompilation(DocumentUri.From(uri), null, fileContents);
                        }
                    }
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

        public bool IsBicepConfigValid(string value)
        {
            try
            {
                JObject jObject = JObject.Parse(value);
                return jObject.IsValid(bicepConfigSchema);
            }
            catch
            {
                return true;
            }
        }
    }
}
