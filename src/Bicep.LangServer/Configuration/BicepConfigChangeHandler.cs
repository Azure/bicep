// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler : IBicepConfigChangeHandler
    {
        private JSchema bicepConfigSchema;
        private readonly IFileResolver fileResolver;

        public BicepConfigChangeHandler(IFileResolver fileResolver)
        {
            this.fileResolver = fileResolver;
            var assembly = Assembly.GetExecutingAssembly();
            string manifestResourceName = assembly.GetManifestResourceNames().Where(p => p.EndsWith("bicepconfig.schema.json", StringComparison.Ordinal)).First();
            Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
            var streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            bicepConfigSchema = JSchema.Parse(streamReader.ReadToEnd());
        }

        public void RetriggerCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, FileEvent bicepConfigFileEvent, IWorkspace workspace)
        {
            Uri bicepConfigUri = bicepConfigFileEvent.Uri.ToUri();

            if (ShouldUpsertCompilation(bicepConfigFileEvent.Type, bicepConfigUri))
            {
                foreach (ISourceFile sourceFile in workspace.GetSourceFilesForDirectory(bicepConfigUri))
                {
                    Uri uri = sourceFile.FileUri;

                    if (fileResolver.TryRead(uri, out string? bicepFileContents, out ErrorBuilderDelegate _) &&
                        !string.IsNullOrWhiteSpace(bicepFileContents))
                    {
                        compilationManager.UpsertCompilation(DocumentUri.From(uri), null, bicepFileContents);
                    }
                }
            }
        }

        private bool ShouldUpsertCompilation(FileChangeType fileChangeType, Uri bicepConfigUri)
        {
            // When fileChangeType is "Changed", we don't want to retrigger compilation for every key stroke.
            // We'll check the validity of file against the schema and retrigger compilation only if the file is valid
            if (fileChangeType == FileChangeType.Changed)
            {
                return fileResolver.TryRead(bicepConfigUri, out string? bicepConfigFileContents, out ErrorBuilderDelegate _) &&
                    bicepConfigFileContents is not null &&
                    IsBicepConfigValid(bicepConfigFileContents);
            }

            return true;
        }

        private bool IsBicepConfigValid(string bicepConfig)
        {
            try
            {
                JObject jObject = JObject.Parse(bicepConfig);
                return jObject.IsValid(bicepConfigSchema);
            }
            catch
            {
                // We want to push error dignostics in case of invalid bicepconfig.json.
                // We won't be able to apply validation until user fixes issues in config file
                return true;
            }
        }
    }
}
