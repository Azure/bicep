// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Bicep.Core.Registry;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code lenses when displaying source from an external bicep module
    public static class ExternalSourceCodeLensProvider
    {
        private static readonly Range DocumentStart = new(new Position(0, 0), new Position(0, 0));

        public static IEnumerable<CodeLens> GetCodeLenses(IModuleDispatcher moduleDispatcher, CodeLensParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (request.TextDocument.Uri.Scheme == LangServerConstants.ExternalSourceFileScheme)
            {
                string? message = null;
                ExternalSourceReference? externalReference = null;

                try
                {
                    externalReference = new ExternalSourceReference(request.TextDocument.Uri);
                }
                catch (Exception ex)
                {
                    message = $"(Experimental) There was an error retrieving source code for this module: {ex.Message}";
                }

                if (externalReference is not null)
                {
                    Debug.Assert(message is null);

                    var isDisplayingCompiledJson = externalReference.IsRequestingCompiledJson; //asdfgasdfg
                    if (externalReference.ToArtifactReference().IsSuccess(out var artifactReference, out message))
                    {
                        var sourceArchiveResult = moduleDispatcher.TryGetModuleSources(artifactReference);

                        if (isDisplayingCompiledJson)
                        {
                            // Displaying main.json

                            if (sourceArchiveResult.SourceArchive is { })
                            {
                                yield return CreateCodeLens(
                                    DocumentStart,
                                    "Show Bicep source (experimental)",
                                    "bicep.internal.showModuleSourceFile",
                                    new ExternalSourceReference(request.TextDocument.Uri).WithRequestForSourceFile(sourceArchiveResult.SourceArchive.EntrypointRelativePath).ToUri().ToString());
                            }
                            else if (sourceArchiveResult.Message is { })
                            {
                                message = $"(Experimental) Cannot display source code for this module. {sourceArchiveResult.Message}";
                            }
                            else
                            {
                                message = "(Experimental) No source code is available for this module";
                            }
                        }
                        else
                        {
                            // Displaying a bicep file

                            if (sourceArchiveResult.SourceArchive is { })
                            {
                                // We're displaying some source file other than the compiled JSON for the module. Allow user to switch to the compiled JSON.
                                yield return CreateCodeLens(
                                    DocumentStart,
                                    "Show compiled JSON",
                                    "bicep.internal.showModuleSourceFile",
                                    new ExternalSourceReference(request.TextDocument.Uri).WithRequestForCompiledJson().ToUri().ToString());
                            }
                            else
                            {
                                message = sourceArchiveResult.Message ??
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
