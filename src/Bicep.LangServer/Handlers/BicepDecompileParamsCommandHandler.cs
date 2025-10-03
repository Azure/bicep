// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Extensions;
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
    public class BicepDecompileParamsCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDecompileParamsCommandParams, BicepDecompileParamsCommandResult>
    {
        private readonly IFileExplorer fileExplorer;
        private readonly BicepDecompiler decompiler;

        public BicepDecompileParamsCommandHandler(
            IFileExplorer fileExplorer,
            ISerializer serializer,
            BicepDecompiler decompiler)
            : base(LangServerConstants.DecompileParamsCommand, serializer)
        {
            this.fileExplorer = fileExplorer;
            this.decompiler = decompiler;
        }

        public override async Task<BicepDecompileParamsCommandResult> Handle(BicepDecompileParamsCommandParams parameters, CancellationToken cancellationToken)
        {
            await Task.Yield();
            try
            {
                var jsonUri = parameters.jsonUri.ToIOUri();
                var bicepUri = parameters.bicepUri?.ToIOUri();
                if (!this.fileExplorer.GetFile(jsonUri).TryReadAllText().IsSuccess(out var jsonContents))
                {
                    throw new InvalidOperationException($"Failed to read {jsonUri}");
                }

                var (entryUri, filesToSave) = decompiler.DecompileParameters(jsonContents, jsonUri.WithExtension(LanguageConstants.ParamsFileExtension), bicepUri);

                return new(new(filesToSave[entryUri], entryUri.ToDocumentUri()), null);
            }
            catch (Exception ex)
            {
                var message = string.Format(LangServerResources.Decompile_DecompilationFailed, ex.Message);

                return new(null, message);
            }
        }
    }
}
