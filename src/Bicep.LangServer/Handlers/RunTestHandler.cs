// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Tests;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Handlers;

[Method("bicep/runTest", Direction.ClientToServer)]
public record RunTestRequest(TextDocumentIdentifier TextDocument, string TestId)
    : IRequest<RunTestResponse>;

public record RunTestResponse(
    bool Success,
    string? Message);

public class RunTestHandler : IJsonRpcRequestHandler<RunTestRequest, RunTestResponse>
{
    private readonly ILanguageServerFacade server;
    private readonly ICompilationManager compilationManager;

    public RunTestHandler(ILanguageServerFacade server, ICompilationManager compilationManager)
    {
        this.server = server;
        this.compilationManager = compilationManager;
    }

    public async Task<RunTestResponse> Handle(RunTestRequest request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        if (compilationManager.GetCompilation(request.TextDocument.Uri) is not {} context)
        {
            return new(false, $"Failed to find test {request.TestId}");
        }

        var model = context.Compilation.GetEntrypointSemanticModel();
        if (model.Root.TestDeclarations.FirstOrDefault(x => LanguageConstants.IdentifierComparer.Equals(x.Name, request.TestId)) is not {} test)
        {
            return new(false, $"Failed to find test {request.TestId}");
        }

        var results = TestRunner.Run(test.AsEnumerable().ToImmutableArray());
        if (results.Results.FirstOrDefault() is not {} result)
        {
            return new(false, $"Failed to get results for test {request.TestId}");
        }

        return new(result.Result.Success, result.Result.Error);
    }
}