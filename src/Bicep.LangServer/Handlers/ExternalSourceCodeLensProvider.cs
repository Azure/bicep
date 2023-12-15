// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    // Provides code actions/fixes for a range in a Bicep document
    public static class ExternalSourceCodeLensProvider
    {
        private static readonly Range DocumentStart = new(new Position(0, 0), new Position(0, 0));

        public static IEnumerable<CodeLens> GetCodeLenses(IModuleDispatcher moduleDispatcher/*refactor: use registryprovider instead asdfgasdfg*/, CodeLensParams request, CancellationToken cancellationToken)
        {
            Trace.WriteLine("ExternalSourceCodeLensProvider:GetCodeLenses asdfg");

            cancellationToken.ThrowIfCancellationRequested();
            Trace.WriteLine("ExternalSourceCodeLensProvider:GetCodeLenses not canceled asdfg");

            if (request.TextDocument.Uri.Scheme == ExternalSourceReference.Scheme)
            {
                var externalReference = new ExternalSourceReference(request.TextDocument.Uri);
                var isDisplayingCompiledJson = externalReference.IsRequestingCompiledJson;
                var artifactReference = externalReference.ToArtifactReference(); //asdfg error?
                var sourceArchive = artifactReference is { } ? moduleDispatcher.TryGetModuleSourceArchive(artifactReference) : null;

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
                            "Source code has not been published for this module");
                    }
                }
                else
                {
                    Debug.Assert(sourceArchive is { }, "If we're showing a bicep file from a module, it must be because we have source code for it"); //asdfg?

                    // We're displaying some source file other than the compiled JSON for the module.  It could be a .json source file (asdfg test), a bicep file, a nested external module (asdfg), or a nested external module's compiled JSON (asdfg).
                    yield return CreateCodeLens(
                        DocumentStart,
                        "Show compiled JSON",
                        "bicep.internal.showModuleSourceFile",
                        new ExternalSourceReference(request.TextDocument.Uri).WithRequestForCompiledJson().ToUri().ToString());
                }
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
