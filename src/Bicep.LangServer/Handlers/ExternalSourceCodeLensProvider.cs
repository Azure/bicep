// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Extensions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code lenses when displaying source from an external bicep module
    public static class ExternalSourceCodeLensProvider
    {
        private static readonly Range DocumentStart = new(new Position(0, 0), new Position(0, 0));

        public static IEnumerable<CodeLens> GetCodeLenses(IModuleDispatcher moduleDispatcher, ISourceFileFactory sourceFileFactory, CodeLensParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (request.TextDocument.Uri.Scheme == LangServerConstants.ExternalSourceFileScheme)
            {
                if (request.TextDocument.Uri.Query.StartsWith("ts:", StringComparison.Ordinal))
                {
                    yield break;
                }

                string? message = null;
                ExternalSourceReference? externalReference = null;

                try
                {
                    externalReference = new ExternalSourceReference(request.TextDocument.Uri);
                }
                catch (Exception ex)
                {
                    message = $"There was an error retrieving source code for this module: {ex.Message}";
                }

                if (externalReference is not null)
                {
                    Debug.Assert(message is null);

                    var isDisplayingCompiledJson = externalReference.IsRequestingCompiledJson;
                    if (externalReference.ToArtifactReference(sourceFileFactory.CreateDummyArtifactReferencingFile()).IsSuccess(out var artifactReference, out message))
                    {
                        var sourceArchiveResult = moduleDispatcher.TryGetModuleSources(artifactReference);

                        if (isDisplayingCompiledJson)
                        {
                            // Displaying main.json

                            if (sourceArchiveResult.IsSuccess(out var sourceArchive, out var ex))
                            {
                                yield return CreateCodeLens(
                                    DocumentStart,
                                    "Show Bicep source",
                                    "bicep.internal.showModuleSourceFile",
                                    new ExternalSourceReference(request.TextDocument.Uri).WithRequestForSourceFile(sourceArchive.EntrypointRelativePath).ToUri().ToString());
                            }
                            else if (ex is SourceNotAvailableException)
                            {
                                message = ex.Message;
                            }
                            else
                            {
                                message = $"Cannot display source code for this module. {ex.Message}";
                            }
                        }
                        else
                        {
                            // Displaying a bicep file

                            if (sourceArchiveResult.IsSuccess(out var _, out var ex))
                            {
                                // We're displaying some source file other than the compiled JSON for the module. Allow user to switch to the compiled JSON.
                                var moduleName = Path.GetFileName(externalReference.Components.Repository);
                                var artifactId = externalReference.Components.ArtifactId;
                                yield return CreateCodeLens(
                                    DocumentStart,
                                    $"Show the compiled JSON for module \"{moduleName}\" ({OciArtifactReferenceFacts.Scheme}:{artifactId})",
                                    "bicep.internal.showModuleSourceFile",
                                    new ExternalSourceReference(request.TextDocument.Uri).WithRequestForCompiledJson().ToUri().ToString());
                            }
                            else
                            {
                                message = ex.Message ??
                                    // This can happen if the user has a source file open in the editor and then restores to a version of the module that doesn't have source code available.
                                    "Could not find the expected source archive in the module registry";
                            }
                        }
                    }
                }

                if (message is not null)
                {
                    yield return CreateErrorLens(message);
                }
            }

            CodeLens CreateErrorLens(string message)
            {
                return CreateCodeLens(DocumentStart, message);
            }
        }

        private static CodeLens CreateCodeLens(Range range, string title, string? commandName = null, params string[] commandArguments)
        {
            return new CodeLens
            {
                Range = range,
                Command = new Command
                {
                    Title = title,
                    Name = commandName ?? string.Empty,
                }
                .WithArguments(commandArguments),
                Data = nameof(ExternalSourceCodeLensProvider)
            };
        }

        // Allows tests to filter to just code lenses that were created by this provider
        public static bool IsExternalSourceCodeLens(this CodeLens codeLens)
        {
            return codeLens.Data?.Value<string>() == nameof(ExternalSourceCodeLensProvider);
        }
    }
}
