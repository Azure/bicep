// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using OmniSharp.Extensions.LanguageServer.Protocol;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.LanguageServer.Handlers
{
    /// <summary>
    /// Represents a URI to request displaying a source file from an external module
    /// </summary>
    [DebuggerDisplay("{ToUri()}")]
    public partial class ExternalSourceReference
    {
        // Current real-life example:
        //   module m 'br:mcr.microsoft.com/bicep/avm/ptn/aca-lza/hosting-environment:0.1.0' = ...
        //     -> Press F12
        //   a) To display a bicep source file other than main.bicep inside the module, CTRL+click on:
        //     module spoke 'modules/spoke/deploy.spoke.bicep' = ...
        //   b) To display a nested external reference to another module, CTRL+click on:
        //     module spokeResourceGroup 'br/public:avm/res/resources/resource-group:0.2.3' = ...

        // The title to display for the document's tab,
        //   e.g. "br:myregistry.azurecr.io/myrepo/module:v1 -> main.json" or something similar.
        // VSCode will display everything after the last slash in the document's tab (interpreting it as
        //   a file path and name), and the full string on hover.
        public string FullTitle { get; init; }

        // Fully qualified module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
        public IOciArtifactAddressComponents Components { get; init; }

        // File being requested from the source, relative to the module root.
        //   e.g. main.bicep or myPath/module.bicep
        // This should be undefined to request the compiled JSON file (can't use "main.json" because there
        //   might actually be a source file called "main.json" in the original module sources, and that would
        //   be different from the compiled JSON file).
        private string? requestedFile;
        public string? RequestedFile
        {
            get => this.requestedFile;
            set
            {
                this.requestedFile = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        public bool IsRequestingCompiledJson => RequestedFile is null;

        // e.g. matches <cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json
        private static Regex externalModulePathRegex = ExternalModulePathRegex();

        public ExternalSourceReference(DocumentUri uri)
        : this(uri.Path, uri.Query, uri.Fragment) { }

        /// <summary>
        /// This constructor is used when we are receiving a request from vscode to display a source file from an external module.
        /// </summary>
        /// <param name="fullTitle"></param>
        /// <param name="fullyQualifiedModuleReference"></param>
        /// <param name="requestedFile"></param>
        /// <exception cref="ArgumentException"></exception>
        public ExternalSourceReference(string? fullTitle, string fullyQualifiedModuleReference, string? requestedFile)
        {
            DiagnosticBuilderDelegate? error = null;
            if (!fullyQualifiedModuleReference.StartsWith($"{OciArtifactReferenceFacts.Scheme}:", StringComparison.Ordinal) ||
                !OciArtifactAddressComponents.TryParse(fullyQualifiedModuleReference[(OciArtifactReferenceFacts.Scheme.Length + 1)..]).IsSuccess(out var components, out error))
            {
                string? innerMessage = null;
                if (error is { })
                {
                    innerMessage = error(DiagnosticBuilder.ForDocumentStart()).Message;
                }
                throw new ArgumentException($"Invalid module reference '{fullyQualifiedModuleReference}'. {innerMessage}", nameof(fullyQualifiedModuleReference));
            }

            Components = components;
            RequestedFile = requestedFile;
            FullTitle = fullTitle ?? GetFullTitle();
        }

        public ExternalSourceReference WithRequestForCompiledJson()
        {
            return this.WithRequestForSourceFile(null);
        }

        public ExternalSourceReference WithRequestForSourceFile(string? requestedSourceFile)
        {
            return new ExternalSourceReference(Components, requestedSourceFile); // recalculate title
        }

        public ExternalSourceReference(IOciArtifactAddressComponents moduleReferenceComponents, SourceArchive? sourceArchive)
        {
            Components = moduleReferenceComponents;

            if (sourceArchive is { })
            {
                // We have Bicep source code available
                RequestedFile = sourceArchive.EntrypointRelativePath;
            }
            else
            {
                // Just requesting the main.json
                RequestedFile = null;
            }

            FullTitle = GetFullTitle();
        }

        private ExternalSourceReference(IOciArtifactAddressComponents module, string? requestedFile, string? fullTitle = null, string? shortTitle = null) // title auto-calculated if not specified
        {
            Components = module;
            RequestedFile = requestedFile;
            FullTitle = fullTitle ?? GetFullTitle();
        }

        public Uri ToUri()
        {
            // Encode the module reference as a query and the file to retrieve as a fragment.
            // Vs Code will strip the fragment and query and use the main part of the uri as the document title.
            // The Bicep extension will use the fragment to make a call to use via textDocument/bicepExternalSource request (see BicepExternalSourceHandler)
            //   to get the actual source code contents to display.
            //
            // Example:
            //
            //   source available (will be encoded):
            //     bicep-extsrc:br:myregistry.azurecr.io/myrepo/module1:v1 -> subfolder>main.bicep?br:myregistry.azurecr.io/myrepo/module1:v1#subfolder/main.bicep
            //
            //   source not available, showing just JSON (will be encoded)
            //     bicep-extsrc:br:myregistry.azurecr.io/myrepo/module1:v1 -> main.json?br:myregistry.azurecr.io/myrepo/module1:v1
            //
            var uri = new UriBuilder($"{LangServerConstants.ExternalSourceFileScheme}:{Uri.EscapeDataString(this.FullTitle)}")
            {
                Query = Uri.EscapeDataString($"{OciArtifactReferenceFacts.Scheme}:{Components.ArtifactId}"),
                Fragment = this.RequestedFile is null ? null : Uri.EscapeDataString(this.RequestedFile),
            };

            return uri.Uri;
        }

        public Result<OciArtifactReference, string> ToArtifactReference(BicepFile referencingFile)
        {
            if (OciArtifactAddressComponents.TryParse(Components.ArtifactId).IsSuccess(out var components, out var failureBuilder))
            { // No parent file template is available or needed because these are absolute references
                return new(new OciArtifactReference(referencingFile, ArtifactType.Module, components));
            }
            else
            {
                return new(failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message);
            }
        }

        private string GetVersion()
        {
            return Components.Tag is string ? $":{Components.Tag}" : $"@{Components.Digest}";
        }

        private string GetFullTitle()
        {
            var shortTitle = GetShortTitle();
            var repositoryBasePath = Path.GetDirectoryName(Components.Repository) ?? string.Empty;

            // Example: br:mockregistry.io/test/modules/module1:v1 -> localpath>main.bicep
            var fullDocumentTitle = $"{OciArtifactReferenceFacts.Scheme}:{Components.Registry}/{Path.Join(repositoryBasePath, shortTitle).Replace('\\', '/')}";

            return fullDocumentTitle;
        }

        // Includes the filename and the module reference (repo and tag/digest).
        //  e.g. "module1:v1 -> main.json"
        // This portion will be visible in the document's tab (minus any parent folders of the filename) without hover.
        public string GetShortTitle()
        {
            string filePath = RequestedFile ?? "main.json";
            // Our display of "module1:v1 -> <path>.bicep" in vscode depends on vscode interpreting that as the "filename" part of the uri,
            //   since it's always displays the filename.
            // If <path> contains / or \, vscode will interpret that as a folder structure and only display the last part of it.
            filePath = CharsToReplaceInFilePath().Replace(filePath, ">");

            var version = GetVersion();
            var repoAndTag = $"{Path.GetFileName(Components.Repository)}{version}";

            string shortTitle;
            if (RequestedFile is not null && externalModulePathRegex.Match(RequestedFile) is Match match && match.Success)
            {
                // We're displaying a nested external module's source. Show both its info and the info of the module that references it.
                var externalRepoAndTag = $"{match.Groups["repoName"].Value}:{match.Groups["tag"].Value}";
                var fileNameInCache = match.Groups["filename"].Value;
                fileNameInCache = CharsToReplaceInFilePath().Replace(fileNameInCache, ">");
                shortTitle = $"{repoAndTag} -> {externalRepoAndTag} -> {fileNameInCache}";
            }
            else
            {
                shortTitle = $"{repoAndTag} -> {filePath}";
            }

            return shortTitle;
        }

        [GeneratedRegex("""
            \<cache\>\/br\/
            .*
            \$(?<repoName>[^\/\$]+)
            \/(?<tag>[^\/\$]+)\$[^\/\$]*
            \/(?<filename>[^\/]+)$            
            """, RegexOptions.IgnorePatternWhitespace)]
        private static partial Regex ExternalModulePathRegex();

        [GeneratedRegex(@"[\/:?]")]
        private static partial Regex CharsToReplaceInFilePath();
    }
}
