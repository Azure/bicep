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
        string? bicepPath
    );

    public record DecompiledBicepparamFile(
        string contents,
        string absolutePath
    );

    public record BicepDecompileParamsCommandResult(
        DecompiledBicepparamFile? decompiledBicepparamFile,
        string? errorMessage
    );

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

        public override Task<BicepDecompileParamsCommandResult> Handle(BicepDecompileParamsCommandParams parameters, CancellationToken cancellationToken)
        {
            try
            {
                Uri jsonUri = new Uri(parameters.jsonUri.GetFileSystemPath());
                var (entryUri, filesToSave) = bicepparamDecompiler.Decompile(jsonUri, PathHelper.ChangeToBicepparamExtension(jsonUri), parameters.bicepPath);
                
                return Task.FromResult(new BicepDecompileParamsCommandResult(new DecompiledBicepparamFile(filesToSave[entryUri], entryUri.AbsolutePath), null));
            }
            catch (Exception ex)
            {   
                var message = string.Format(LangServerResources.Decompile_DecompilationFailed, ex.Message);
                
                return Task.FromResult(new BicepDecompileParamsCommandResult(null, message));
            }
        }
    }
}
