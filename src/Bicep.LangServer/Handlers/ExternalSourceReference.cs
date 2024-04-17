// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;
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
        // The title to display for the document's tab,
        //   e.g. "br:myregistry.azurecr.io/myrepo/module:v1/main.json (module:v1)" or something similar.
        // VSCode will display everything after the last slash in the document's tab (interpreting it as
        //   a file path and name), and the full string on hover.
        public string FullTitle { get; init; }

        // Fully qualified module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
        public IArtifactAddressComponents Components { get; init; }

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
            ErrorBuilderDelegate? error = null;
            if (!fullyQualifiedModuleReference.StartsWith($"{OciArtifactReferenceFacts.Scheme}:", StringComparison.Ordinal) ||
                !OciArtifactReference.TryParseFullyQualifiedComponents(fullyQualifiedModuleReference.Substring(OciArtifactReferenceFacts.Scheme.Length + 1)).IsSuccess(out var components, out error))
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

        public ExternalSourceReference(OciArtifactReference moduleReference, SourceArchive? sourceArchive)
        {
            Debug.Assert(moduleReference.Type == ArtifactType.Module && moduleReference.Scheme == OciArtifactReferenceFacts.Scheme, "Expecting a module reference, not a provider reference");
            Components = moduleReference.AddressComponents;

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

        private ExternalSourceReference(IArtifactAddressComponents module, string? requestedFile, string? fullTitle = null, string? shortTitle = null) // title auto-calculated if not specified
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
            //     bicep-extsrc:br:myregistry.azurecr.io/myrepo:main.bicep (v1)?br:myregistry.azurecr.io/myrepo:v1#main.bicep
            //
            //   source not available, showing just JSON (will be encoded)
            //     bicep-extsrc:br:myregistry.azurecr.io/myrepo:main.json (v1)?br:myregistry.azurecr.io/myrepo:v1
            //
            var uri = new UriBuilder($"{LangServerConstants.ExternalSourceFileScheme}:{Uri.EscapeDataString(this.FullTitle)}")
            {
                Query = Uri.EscapeDataString($"{OciArtifactReferenceFacts.Scheme}:{Components.ArtifactId}"),
                Fragment = this.RequestedFile is null ? null : Uri.EscapeDataString(this.RequestedFile),
            };

            return uri.Uri;
        }

        public Result<OciArtifactReference, string> ToArtifactReference()
        {
            if (OciArtifactReference.TryParseFullyQualifiedComponents(Components.ArtifactId).IsSuccess(out var components, out var failureBuilder))
            { // No parent file template is available or needed because these are absolute references
                return new(new OciArtifactReference(ArtifactType.Module, components, new Uri("file:///no-parent-file-is-available.bicep")));
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
            var version = GetVersion();
            var shortTitle = GetShortTitle();
            var fullDocumentTitle = $"{OciArtifactReferenceFacts.Scheme}:{Components.Registry}/{Components.Repository}{version}/{shortTitle}";

            return fullDocumentTitle;
        }

        // Includes the filename and the module reference (repo and tag/digest).
        //  e.g. "main.json (myregistry.azurecr.io/myrepo:v1)"
        // This portion will be visible in the document's tab (minus any parent folders of the filename) without hover.
        public string GetShortTitle()
        {
            string filename = RequestedFile ?? "main.json";
            var version = GetVersion();
            var repoAndTag = $"{Path.GetFileName(Components.Repository)}{version}";

            string shortTitle;
            if (RequestedFile is not null && externalModulePathRegex.Match(RequestedFile) is Match match && match.Success)
            {
                // We're display a nested external module's source. Show both its info and the info of the module that references it.
                var externalRepoAndTag = $"{match.Groups["repoName"].Value}:{match.Groups["tag"].Value}";
                shortTitle = $"{Path.GetFileName(filename)} ({repoAndTag}->{externalRepoAndTag})";
            }
            else
            {
                shortTitle = $"{filename} ({repoAndTag})";
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
    }
}
