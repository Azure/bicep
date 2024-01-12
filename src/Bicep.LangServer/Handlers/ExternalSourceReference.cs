// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.LanguageServer.Handlers
{
    /// <summary>
    /// Represents a URI to request displaying a source file from an external module
    /// </summary>
    public class ExternalSourceReference
    {
        // The title to display for the document,
        //   e.g. "br:myregistry.azurecr.io/myrepo/module/v1/main.json (module:v1)" or something similar.
        // VSCode will display everything after the last slash in the document's tab (interpreting it as
        //   a file path and name), and the full string on hover.
        public string Title { get; init; }

        // Fully qualified module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
        public IArtifactAddressComponents Components { get; init; }

        // File being requested from the source, relative to the module root.
        //   e.g. main.bicep or myPath/module.bicep
        // This should be undefined to request the compiled JSON file (can't use "main.json" because there
        //   might actually be a source file called "main.json" in the original module sources, and that would
        //   be different from the compiled JSON file).
        public string? RequestedFile { get; init; }

        public bool IsRequestingCompiledJson => string.IsNullOrWhiteSpace(RequestedFile);

        public ExternalSourceReference(DocumentUri uri)
        : this(uri.Path, uri.Query, uri.Fragment) { }

        public ExternalSourceReference(string title, string fullyQualifiedModuleReference, string? requestedFile)
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
            Title = title;
        }

        public ExternalSourceReference WithRequestForCompiledJson()
        {
            return this.WithRequestForSourceFile(null);
        }

        public ExternalSourceReference WithRequestForSourceFile(string? requestedSourceFile)
        {
            return new ExternalSourceReference(Components, requestedSourceFile); // recalculate title
        }

        public ExternalSourceReference(OciArtifactReference moduleReference, SourceArchive? sourceArchive, bool defaultToDisplayingBicep = true)
        {
            Debug.Assert(moduleReference.Type == ArtifactType.Module && moduleReference.Scheme == OciArtifactReferenceFacts.Scheme, "Expecting a module reference, not a provider reference");
            Components = moduleReference.AddressComponents;

            if (sourceArchive is { } && defaultToDisplayingBicep)
            {
                // We have Bicep source code available
                RequestedFile = sourceArchive.EntrypointRelativePath;
            }
            else
            {
                // Just requesting the main.json
                RequestedFile = null;
            }

            Title = GetTitle();
        }

        private ExternalSourceReference(IArtifactAddressComponents module, string? requestedFile, string? title = null) // title auto-calculated if not specified
        {
            Components = module;
            RequestedFile = requestedFile;
            Title = title ?? GetTitle();
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
            var uri = new UriBuilder($"{LangServerConstants.ExternalSourceFileScheme}:{Uri.EscapeDataString(this.Title)}")
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

        private string GetTitle()
        {
            string filename = this.RequestedFile ?? "main.json";

            var version = Components.Tag is string ? $":{Components.Tag}" : $"@{Components.Digest}";

            var shortDocumentTitle = $"{filename} ({Path.GetFileName(Components.Repository)}{version})";
            var fullDocumentTitle = $"{OciArtifactReferenceFacts.Scheme}:{Components.Registry}/{Components.Repository}{version}/{shortDocumentTitle}";

            return fullDocumentTitle;
        }
    }
}
