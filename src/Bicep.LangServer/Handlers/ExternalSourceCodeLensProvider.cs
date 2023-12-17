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
                string? error = null;
                ExternalSourceReference? externalReference = null;

                try
                {
                    externalReference = new ExternalSourceReference(request.TextDocument.Uri);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                if (externalReference is not null)
                {
                    Debug.Assert(error is null);

                    var isDisplayingCompiledJson = externalReference.IsRequestingCompiledJson;
                    if (externalReference.ToArtifactReference().IsSuccess(out var artifactReference, out error))
                    {
                        var sourceArchive = artifactReference is { } ? moduleDispatcher.TryGetModuleSources(artifactReference) : null;

                        if (isDisplayingCompiledJson)
                        {
                            if (sourceArchive is { })
                            {
                                yield return CreateCodeLens(
                                    DocumentStart,
                                    "Show Bicep source",
                                    "bicep.internal.showModuleSourceFile",
                                    new ExternalSourceReference(request.TextDocument.Uri).WithRequestForSourceFile(sourceArchive.EntrypointRelativePath).ToUri().ToString());
                            }
                            else
                            {
                                yield return CreateCodeLens(
                                    DocumentStart,
                                    "No source code is available for this module");
                            }
                        }
                        else
                        {
                            if (sourceArchive is { })
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
                                // This can happen if the user has a source file open in the editor and then restores to a version of the module that doesn't have source code available.
                                error = "Could not find the expected source archive in the module registry";
                            }
                        }
                    }
                }

                if (error is not null)
                {
                    yield return CreateErrorLens($"There was an error retrieving source code for this module: {error}");
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
