// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.Decompiler;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers;

public record DecompileInMemoryParams(
    DocumentUri BicepUri,
    string JsonContent);
public record DecompileInMemoryResult(
    Uri EntrypointUri,
    ImmutableDictionary<Uri, string> FilesToSave);

public class DecompileInMemoryHandler : ExecuteTypedResponseCommandHandlerBase<DecompileInMemoryParams, DecompileInMemoryResult>
{
    private readonly BicepDecompiler decompiler;

    public DecompileInMemoryHandler(BicepDecompiler decompiler, ISerializer serializer)
        : base(LangServerConstants.DecompileInMemoryCommand, serializer)
    {
        this.decompiler = decompiler;
    }

    public override async Task<DecompileInMemoryResult> Handle(DecompileInMemoryParams request, CancellationToken cancellationToken)
    {
        var result = await decompiler.Decompile(request.BicepUri.ToUriEncoded(), request.JsonContent);

        return new(
            result.EntrypointUri,
            result.FilesToSave);
    }
}