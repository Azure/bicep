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
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.LanguageServer.Handlers
{
    public class ExternalSourceReference
    {
        public static string Scheme => "bicep-extsrc";

        // The title to display for the document,
        //   e.g. "br:myregistry.azurecr.io/myrepo/module/v1/main.json (module:v1)" or something similar.
        // VSCode will display everything after the last slash in the document's tab (interpreting it as
        //   a file path and name), and the full string on hover.
        public string Title { get; init; }

        // Fully qualified module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
        public IArtifactAddressComponents ModuleParts { get; init; }

        // File being requested from the source, relative to the module root.
        //   e.g. main.bicep or mypath/module.bicep
        // This should be undefined to request the compiled JSON file (can't use "main.json" because there
        //   might actually be a source file called "main.json" in the original module sources, and that would
        //   be different from the compiled JSON file).
        public string? RequestedFile { get; init; }

        public ExternalSourceReference(string title, IArtifactAddressComponents module, string? requestedFile)
        {
            ModuleParts = module;
            RequestedFile = requestedFile;
            Title = title;
        }

        public ExternalSourceReference(string title, string fullyQualifiedModuleReference, string? requestedFile)
        {
            ErrorBuilderDelegate? error = null;
            if (!fullyQualifiedModuleReference.StartsWith($"{OciArtifactReferenceFacts.Scheme}:", StringComparison.Ordinal) ||
                !OciArtifactReference.TryParseFullyQualifiedParts(fullyQualifiedModuleReference.Substring(OciArtifactReferenceFacts.Scheme.Length + 1)).IsSuccess(out var parts, out error))
            {
                string? innerMessage = null;
                if (error is { })
                {
                    innerMessage = error(DiagnosticBuilder.ForDocumentStart()).Message;
                }
                throw new ArgumentException($"Invalid module reference '{fullyQualifiedModuleReference}'. {innerMessage}", nameof(fullyQualifiedModuleReference));
            }

            ModuleParts = parts;
            RequestedFile = requestedFile;
            Title = title;
        }

        public ExternalSourceReference(DocumentUri uri)
        : this(uri.Path, uri.Query, uri.Fragment) { }

        public ExternalSourceReference(OciArtifactReference moduleReference, SourceArchive? sourceArchive)
        {
            Debug.Assert(moduleReference.Type == ArtifactType.Module && moduleReference.Scheme == OciArtifactReferenceFacts.Scheme, "Expecting a module reference, not a provider reference");
            ModuleParts = moduleReference.AddressComponents;

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

            Title = GetTitle();
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
            var uri = new UriBuilder($"{Scheme}:{Uri.EscapeDataString(this.Title)}")
            {
                Query = Uri.EscapeDataString($"{OciArtifactReferenceFacts.Scheme}:{ModuleParts.ArtifactId}"),
                Fragment = this.RequestedFile is null ? null : Uri.EscapeDataString(this.RequestedFile),
            };

            return uri.Uri;
        }

        private string GetTitle()
        {
            string filename = this.RequestedFile ?? "main.json";

            var version = ModuleParts.Tag is string ? $":{ModuleParts.Tag}" : $"@{ModuleParts.Digest}";

            var shortDocumentTitle = $"{filename} ({Path.GetFileName(ModuleParts.Repository)}{version})";
            var fullDocumentTitle = $"{OciArtifactReferenceFacts.Scheme}:{ModuleParts.Registry}/{ModuleParts.Repository}{version}/{shortDocumentTitle}";

            return fullDocumentTitle;
        }
    }
}
