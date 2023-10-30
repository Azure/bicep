// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using StreamJsonRpc;

namespace Bicep.Cli.Rpc;

public class RpcServer
{
#pragma warning disable IDE1006
    public record CompileRequest(
        string path);

    public record CompileResponse(
        bool success,
        ImmutableArray<CompileResponseDiagnostic> diagnostics,
        string? contents);

    public record CompileResponseDiagnostic(
        int line,
        int @char,
        string level,
        string code,
        string message);
#pragma warning restore IDE1006

    private readonly BicepCompiler compiler;

    public RpcServer(BicepCompiler compiler)
    {
        this.compiler = compiler;
    }

    [JsonRpcMethod("bicep/compile", UseSingleObjectParameterDeserialization = true)]
    public async Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken)
    {
        var inputUri = PathHelper.FilePathToFileUrl(request.path);
        if (!PathHelper.HasBicepExtension(inputUri) &&
            !PathHelper.HasBicepparamsExension(inputUri))
        {
            throw new InvalidOperationException("Cannot compile");
        }

        var compilation = await compiler.CreateCompilation(inputUri, new Workspace());
        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = GetDiagnostics(compilation).ToImmutableArray();

        if (model.HasErrors())
        {
            return new(false, diagnostics, null);
        }

        var writer = new StringWriter();
        if (PathHelper.HasBicepparamsExension(inputUri))
        {
            new ParametersEmitter(model).Emit(writer);
        }
        else
        {
            new TemplateEmitter(model).Emit(writer);
        }

        return new(true, diagnostics, writer.ToString());
    }

    private static IEnumerable<CompileResponseDiagnostic> GetDiagnostics(Compilation compilation)
    {
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                (int line, int character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);
                yield return new(line, character, diagnostic.Level.ToString(), diagnostic.Code, diagnostic.Message);
            }
        }
    }
}