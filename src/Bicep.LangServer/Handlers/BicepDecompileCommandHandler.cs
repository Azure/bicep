// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
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
        ) : this(decompileId, output, errorMessage, null, [], [])
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
        private readonly IFileExplorer fileExplorer;
        private readonly BicepDecompiler bicepDecompiler;
        private readonly TelemetryAndErrorHandlingHelper<BicepDecompileCommandResult> telemetryHelper;

        public BicepDecompileCommandHandler(
            ISerializer serializer,
            ILanguageServerFacade server,
            ITelemetryProvider telemetryProvider,
            IFileExplorer fileExplorer,
            BicepDecompiler bicepDecompiler)
            : base(LangServerConstants.DecompileCommand, serializer)
        {
            this.telemetryHelper = new TelemetryAndErrorHandlingHelper<BicepDecompileCommandResult>(server.Window, telemetryProvider);
            this.fileExplorer = fileExplorer;
            this.bicepDecompiler = bicepDecompiler;
        }

        public override Task<BicepDecompileCommandResult> Handle(BicepDecompileCommandParams parameters, CancellationToken cancellationToken)
        {
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling(() =>
            {
                return Decompile(parameters.jsonUri.ToIOUri());
            });
        }

        private async Task<(BicepDecompileCommandResult result, BicepTelemetryEvent? successTelemetry)> Decompile(IOUri jsonUri)
        {
            StringBuilder output = new();
            string decompileId = Guid.NewGuid().ToString();

            Uri? bicepUri;
            ImmutableDictionary<Uri, string>? filesToSave;
            try
            {
                // Decompile
                Log(output, String.Format(LangServerResources.Decompile_DecompilationStartMsg, jsonUri));
                if (!this.fileExplorer.GetFile(jsonUri).TryReadAllText().IsSuccess(out var jsonContents, out _))
                {
                    throw new InvalidOperationException($"Failed to read {jsonUri}");
                }

                (bicepUri, filesToSave) = await bicepDecompiler.Decompile(jsonUri.WithExtension(LanguageConstants.LanguageFileExtension).ToUri(), jsonContents);
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
            pathsToSave = [.. pathsToSave.OrderByAscending(f => f.path == bicepUri.LocalPath ? "" : f.path)];
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
