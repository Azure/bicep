// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using Bicep.LanguageServer.Telemetry;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly BicepparamDecompiler bicepparamDecompiler;

        public BicepDecompileParamsCommandHandler(
            ISerializer serializer,
            BicepparamDecompiler bicepparamDecompiler)
            : base(LangServerConstants.DecompileParamsCommand, serializer)
        {
            this.bicepparamDecompiler = bicepparamDecompiler;
        }

        public override async Task<BicepDecompileParamsCommandResult> Handle(BicepDecompileParamsCommandParams parameters, CancellationToken cancellationToken)
        {
            await Task.Yield();
            try
            {
                var jsonUri = parameters.jsonUri.ToUriEncoded();
                var bicepUri = parameters.bicepUri?.ToUriEncoded();

                var (entryUri, filesToSave) = bicepparamDecompiler.Decompile(jsonUri, PathHelper.ChangeToBicepparamExtension(jsonUri), bicepUri);
                
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
