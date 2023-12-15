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
    // TODO: Currently you need to parse this via asdfg
    public class ExternalSourceReference
    {
        public static string Scheme => "bicep-extsrc";

        // The title to display for the document,
        //   e.g. "br:myregistry.azurecr.io/myrepo/module/v1/main.json (module:v1)" or something similar.
        // VSCode will display everything after the last slash in the document's tab (interpreting it as
        //   a file path and name), and the full string on hover.
        public string Title { get; init; }

        // Fully qualified module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
        public IArtifactAddressComponents ModuleAddressComponents { get; init; }

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
                !OciArtifactReference.TryParseFullyQualifiedParts(fullyQualifiedModuleReference.Substring(OciArtifactReferenceFacts.Scheme.Length + 1)).IsSuccess(out var parts, out error))
            {
                string? innerMessage = null;
                if (error is { })
                {
                    innerMessage = error(DiagnosticBuilder.ForDocumentStart()).Message;
                }
                throw new ArgumentException($"Invalid module reference '{fullyQualifiedModuleReference}'. {innerMessage}", nameof(fullyQualifiedModuleReference));
            }

            ModuleAddressComponents = parts;
            RequestedFile = requestedFile;
            Title = title;
        }

        public ExternalSourceReference WithRequestForCompiledJson()
        {
            return this.WithRequestForSourceFile(null);
        }

        public ExternalSourceReference WithRequestForSourceFile(string? requestedSourceFile) //asdfg test
        {
            return new ExternalSourceReference(ModuleAddressComponents, requestedSourceFile); // recalculate title
        }

        public ExternalSourceReference(OciArtifactReference moduleReference, SourceArchive? sourceArchive)
        {
            Debug.Assert(moduleReference.Type == ArtifactType.Module && moduleReference.Scheme == OciArtifactReferenceFacts.Scheme, "Expecting a module reference, not a provider reference");
            ModuleAddressComponents = moduleReference.AddressComponents;

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

        private ExternalSourceReference(IArtifactAddressComponents module, string? requestedFile, string? title = null) // title auto-calculated if not specified
        {
            ModuleAddressComponents = module;
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
            var uri = new UriBuilder($"{Scheme}:{Uri.EscapeDataString(this.Title)}")
            {
                Query = Uri.EscapeDataString($"{OciArtifactReferenceFacts.Scheme}:{ModuleAddressComponents.ArtifactId}"),
                Fragment = this.RequestedFile is null ? null : Uri.EscapeDataString(this.RequestedFile),
            };

            return uri.Uri;
        }

        public OciArtifactReference? ToArtifactReference() //asdfg error?
        {
            if (OciArtifactReference.TryParseFullyQualifiedParts(ModuleAddressComponents.ArtifactId).IsSuccess(out var parts, out var error)) //asdfg error?
            { //asdfg No parent file template available/needed because these are absolute references
                return new OciArtifactReference(ArtifactType.Module, parts, new Uri("file:///no-parent-file-is-available.bicep"));
            }

            return null;
        }

        private string GetTitle()
        {
            string filename = this.RequestedFile ?? "main.json";

            var version = ModuleAddressComponents.Tag is string ? $":{ModuleAddressComponents.Tag}" : $"@{ModuleAddressComponents.Digest}";

            var shortDocumentTitle = $"{filename} ({Path.GetFileName(ModuleAddressComponents.Repository)}{version})";
            var fullDocumentTitle = $"{OciArtifactReferenceFacts.Scheme}:{ModuleAddressComponents.Registry}/{ModuleAddressComponents.Repository}{version}/{shortDocumentTitle}";

            return fullDocumentTitle;
        }
    }
}
