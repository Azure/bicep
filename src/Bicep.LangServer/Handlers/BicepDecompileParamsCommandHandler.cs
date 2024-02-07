// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDecompileParamsCommandParams(
        DocumentUri jsonUri,
        DocumentUri? bicepUri);

    public record DecompiledBicepparamFile(
        string contents,
        DocumentUri uri);

    public record BicepDecompileParamsCommandResult(
        DecompiledBicepparamFile? decompiledBicepparamFile,
        string? errorMessage);

    /// <summary>
    /// Handles a request from the client to decompile a JSON file for given a file path, creating a bicepparam file
    /// </summary>
    public class BicepDecompileParamsCommandHandler(
        IFileResolver fileResolver,
        ISerializer serializer,
        BicepDecompiler decompiler) : ExecuteTypedResponseCommandHandlerBase<BicepDecompileParamsCommandParams, BicepDecompileParamsCommandResult>(LangServerConstants.DecompileParamsCommand, serializer)
    {
        private readonly IFileResolver fileResolver = fileResolver;
        private readonly BicepDecompiler decompiler = decompiler;

        public override async Task<BicepDecompileParamsCommandResult> Handle(BicepDecompileParamsCommandParams parameters, CancellationToken cancellationToken)
        {
            await Task.Yield();
            try
            {
                var jsonUri = parameters.jsonUri.ToUriEncoded();
                var bicepUri = parameters.bicepUri?.ToUriEncoded();
                if (!fileResolver.TryRead(jsonUri).IsSuccess(out var jsonContents))
                {
                    throw new InvalidOperationException($"Failed to read {jsonUri}");
                }

                var (entryUri, filesToSave) = decompiler.DecompileParameters(jsonContents, PathHelper.ChangeToBicepparamExtension(jsonUri), bicepUri);

                return new(new(filesToSave[entryUri], entryUri), null);
            }
            catch (Exception ex)
            {
                var message = string.Format(LangServerResources.Decompile_DecompilationFailed, ex.Message);

                return new(null, message);
            }
        }
    }
}
