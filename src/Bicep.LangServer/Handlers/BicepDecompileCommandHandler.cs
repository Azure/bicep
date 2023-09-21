// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using Bicep.LanguageServer.Telemetry;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record DecompiledFile(
        // absolute path to overwrite - could be outside input file's folder/subfolders (e.g. ".." in module path)
        string absolutePath,
        // relative path if choosing to copy to new folder - original paths outside input file's folder/subfolders may be renamed/moved
        string clonableRelativePath,
        string bicepContents
    );

    public record BicepDecompileCommandParams(DocumentUri jsonUri);

    public record BicepDecompileCommandResult
    {
        public string decompileId; // Used to synchronize telemetry events
        public string output;
        public string? errorMessage;
        public string? mainBicepPath; // non-null if errorMessage==null
        public DecompiledFile[] outputFiles;
        public string[] conflictingOutputPaths; // client should verify overwrite with user

        private BicepDecompileCommandResult(
            string decompileId,
            string output,
            string? errorMessage,
            string? mainBicepPath,
            DecompiledFile[] outputFiles,
            string[] conflictingOutputPaths)
        {
            this.decompileId = decompileId;
            this.errorMessage = errorMessage;
            this.output = output;
            this.mainBicepPath = mainBicepPath;
            this.outputFiles = outputFiles;
            this.conflictingOutputPaths = conflictingOutputPaths;
        }

        public BicepDecompileCommandResult(
            string decompileId,
            string output,
            string? errorMessage
        ) : this(decompileId, output, errorMessage, null, new DecompiledFile[] { }, new string[] { })
        {
        }

        public BicepDecompileCommandResult(
            string decompileId,
            string output,
            string mainBicepPath,
            DecompiledFile[] outputFiles,
            string[] conflictingOutputPaths
        ) : this(decompileId, output, null, mainBicepPath, outputFiles, conflictingOutputPaths)
        {
        }
    }

    /// <summary>
    /// Handles a request from the client to decompile a JSON file for given a file path, creating one or more bicep files
    /// </summary>
    public class BicepDecompileCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDecompileCommandParams, BicepDecompileCommandResult>
    {
        private readonly BicepDecompiler bicepDecompiler;
        private readonly TelemetryAndErrorHandlingHelper<BicepDecompileCommandResult> telemetryHelper;

        public BicepDecompileCommandHandler(
            ISerializer serializer,
            ILanguageServerFacade server,
            ITelemetryProvider telemetryProvider,
            BicepDecompiler bicepDecompiler)
            : base(LangServerConstants.DecompileCommand, serializer)
        {
            this.telemetryHelper = new TelemetryAndErrorHandlingHelper<BicepDecompileCommandResult>(server.Window, telemetryProvider);
            this.bicepDecompiler = bicepDecompiler;
        }

        public override Task<BicepDecompileCommandResult> Handle(BicepDecompileCommandParams parameters, CancellationToken cancellationToken)
        {
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling(() =>
            {
                return Decompile(parameters.jsonUri.GetFileSystemPath());
            });
        }

        private async Task<(BicepDecompileCommandResult result, BicepTelemetryEvent? successTelemetry)> Decompile(string jsonPath)
        {
            StringBuilder output = new StringBuilder();
            string decompileId = Guid.NewGuid().ToString();

            Uri jsonUri = new Uri(jsonPath, UriKind.Absolute);

            Uri? bicepUri;
            ImmutableDictionary<Uri, string>? filesToSave;
            try
            {
                // Decompile
                Log(output, String.Format(LangServerResources.Decompile_DecompilationStartMsg, jsonPath));
                (bicepUri, filesToSave) = await bicepDecompiler.Decompile(jsonUri, PathHelper.ChangeToBicepExtension(jsonUri));
            }
            catch (Exception ex)
            {
                var message = string.Format(LangServerResources.Decompile_DecompilationFailed, ex.Message);
                Log(output, message);
                throw telemetryHelper.CreateException(
                    message,
                    BicepTelemetryEvent.DecompileFailure(decompileId, message),
                    new BicepDecompileCommandResult(decompileId, output.ToString(), message)
                );
            }

            // Determine output files to save
            Trace.TraceInformation($"Decompilation main output: {bicepUri.LocalPath}");
            Trace.TraceInformation($"Decompilation all files to save: {string.Join(", ", filesToSave.Select(kvp => kvp.Key.LocalPath))}");

            (string path, string content)[] pathsToSave = filesToSave.Select(kvp => (kvp.Key.LocalPath, kvp.Value)).ToArray();

            // Put main bicep file first in the array
            pathsToSave = pathsToSave.OrderByAscending(f => f.path == bicepUri.LocalPath ? "" : f.path).ToArray();
            Debug.Assert(pathsToSave[0].path == bicepUri.LocalPath, "Expected Bicep URL to be in the files to save");
            Debug.Assert(pathsToSave.Length >= 1, "No files to save?");

            // Conflicts with any existing files?
            string[] conflictingPaths = pathsToSave.Where(f => File.Exists(f.path)).Select(f => f.path).ToArray();

            string? outputFolder = Path.GetDirectoryName(bicepUri.LocalPath);
            Debug.Assert(outputFolder is not null, "outputFolder should not be null");
            DecompiledFile[] outputFiles =
                pathsToSave.Select(pts => DetermineDecompiledPaths(outputFolder, pts.path, pts.content))
                .ToArray();


            // Show disclaimer and completion
            Log(output, BicepDecompiler.DecompilerDisclaimerMessage);

            // Return result
            string mainBicepPath = pathsToSave[0].path;
            var result = new BicepDecompileCommandResult(
                decompileId,
                output.ToString(),
                mainBicepPath,
                outputFiles,
                conflictingPaths);
            return (
                result,
                successTelemetry: BicepTelemetryEvent.DecompileSuccess(result.decompileId, pathsToSave.Length, conflictingPaths.Length)
                );
        }

        private DecompiledFile DetermineDecompiledPaths(string outputFolder, string absolutePath, string contents)
        {
            string relativePath = Path.GetRelativePath(outputFolder, absolutePath);
            return new DecompiledFile(absolutePath, relativePath, contents);
        }

        private static void Log(StringBuilder output, string message)
        {
            output.AppendLine(message);
            Trace.TraceInformation(message);
        }
    }
}
