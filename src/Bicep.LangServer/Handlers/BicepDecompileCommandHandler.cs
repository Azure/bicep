// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.Helpers;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using SharpYaml.Serialization.Logging;
using static Bicep.LanguageServer.Handlers.BicepDecompileCommandHandler;

namespace Bicep.LanguageServer.Handlers
{
    public enum BicepDecompileCommandStatus
    {
        Canceled = 1,
        Failed = 2,
        Success = 3,
    }

    public record BicepDecompileCommandParams(DocumentUri jsonUri);
    public record BicepDecompileCommandResult(BicepDecompileCommandStatus status, string output, Uri? bicepUri, string? failureError);

    /// <summary>
    /// Handles a request from the client to decompile a JSON file for given a file path, creating one or more bicep files
    /// </summary>
    /// <remarks>
    /// Using ExecuteTypedResponseCommandHandlerBase instead of IJsonRpcRequestHandler because IJsonRpcRequestHandler will throw "Content modified" if text changes are detected, and for this command
    /// that is expected.
    /// </remarks>
    public class BicepDecompileCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDecompileCommandParams, BicepDecompileCommandResult>
    {
        private readonly IFeatureProvider features;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly IModuleRegistryProvider registryProvider;
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly ILanguageServerFacade server;
        private readonly TelemetryAndErrorHandlingHelper<BicepDecompileCommandResult> telemetryHelper;

        public BicepDecompileCommandHandler(
            ISerializer serializer,
            IFeatureProvider features,
            INamespaceProvider namespaceProvider,
            IConfigurationManager configurationManager,
            IModuleRegistryProvider registryProvider,
            IClientCapabilitiesProvider clientCapabilitiesProvider,
            ILanguageServerFacade server,
            ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DecompileCommand, serializer)
        {
            this.features = features;
            this.namespaceProvider = namespaceProvider;
            this.configurationManager = configurationManager;
            this.registryProvider = registryProvider;
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.server = server;
            this.telemetryHelper = new TelemetryAndErrorHandlingHelper<BicepDecompileCommandResult>(server.Window, telemetryProvider);
        }

        private record TelemetryProperties
        {
            public bool OverwriteFiles;
            public int CountConflictingFiles;
        }

        public override Task<BicepDecompileCommandResult> Handle(BicepDecompileCommandParams parameters, CancellationToken cancellationToken)
        {
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling(async () =>
            {
                if (parameters.jsonUri is null)
                {
                    throw new ArgumentException("Input file path is null");
                }

                return await Decompile(parameters.jsonUri.GetFileSystemPath());
            });
        }

        private record PathToSave(string Path, string Contents) { }

        private async Task<(BicepDecompileCommandResult result, BicepTelemetryEvent? successTelemetry)> Decompile(string jsonPath)
        {
            StringBuilder output = new StringBuilder();
            var telemetryProperties = new TelemetryProperties();

            Uri jsonUri = new Uri(jsonPath, UriKind.Absolute);
            string compiledFilePath = PathHelper.GetDefaultDecompileOutputPath(jsonPath);

            Uri? bicepUri;
            ImmutableDictionary<Uri, string>? filesToSave;
            try
            {
                // Decompile
                var decompiler = new TemplateDecompiler(features, namespaceProvider, new FileResolver(), registryProvider, configurationManager);
                Log(output, $"Decompiling {jsonPath} into Bicep...");
                (bicepUri, filesToSave) = decompiler.DecompileFileWithModules(jsonUri, PathHelper.ChangeToBicepExtension(jsonUri));
            }
            catch (Exception ex)
            {
                Log(output, ex.Message);
                throw telemetryHelper.CreateException(
                    ex.Message,
                    BicepTelemetryEvent.DecompileFailure(ex.Message),
                    new BicepDecompileCommandResult(BicepDecompileCommandStatus.Failed, output.ToString(), null, ex.Message)
                );
            }

            // Determine output files to save
            Trace.TraceInformation($"Decompilation main output: {bicepUri.LocalPath}");
            Trace.TraceInformation($"Decompilation all files to save: {string.Join(", ", filesToSave.Select(kvp => kvp.Key.LocalPath))}");
            string? outputFolder = Path.GetDirectoryName(bicepUri.LocalPath);
            Debug.Assert(outputFolder is not null, "outputFolder should not be null");

            PathToSave[] pathsToSave = filesToSave.Select(kvp => new PathToSave(kvp.Key.LocalPath, kvp.Value)).ToArray();

            // Put main bicep file first in the array
            pathsToSave = pathsToSave.OrderByAscending(f => f.Path == bicepUri.LocalPath ? "" : f.Path).ToArray();
            Debug.Assert(pathsToSave[0].Path == bicepUri.LocalPath, "Expected Bicep URL to be in the files to save");
            Debug.Assert(pathsToSave.Length >= 1, "No files to save?");
            Debug.Assert(pathsToSave.All(f => !Path.GetRelativePath(outputFolder, f.Path).Contains("..")), $"Expected all output files to be relative to {outputFolder}");

            // Conflicts with any existing files?
            if (await DeterminateFinalPathsToSave(output, outputFolder, pathsToSave, pathsToSave[0].Path, telemetryProperties) is { } finalPathsToSave)
            {
                outputFolder = finalPathsToSave.outputFolder;
                pathsToSave = finalPathsToSave.pathsToSave;
            }
            else
            {
                return (
                    result: new BicepDecompileCommandResult(BicepDecompileCommandStatus.Canceled, output.ToString(), null, null),
                    successTelemetry: BicepTelemetryEvent.DecompileFailure("canceled")
                    );
            }

            // Save output files
            SaveOutputFiles(output, pathsToSave);

            string mainBicepPath = pathsToSave[0].Path;
            Debug.Assert(File.Exists(mainBicepPath), $"Bicep file should have been saved: {mainBicepPath}");

            // Show disclaimer
            Log(output, TemplateDecompiler.DecompilerDisclaimerMessage);

            Log(output, $"Finished decompiling to {mainBicepPath}");

            // Show main decompiled file            
            if (clientCapabilitiesProvider.DoesClientSupportShowDocumentRequest())
            {
                await server.Window.ShowDocument(new() { TakeFocus = true, Uri = DocumentUri.File(mainBicepPath) });
            }

            return (
                result: new BicepDecompileCommandResult(BicepDecompileCommandStatus.Success, output.ToString(), new Uri(mainBicepPath, UriKind.Absolute), null),
                successTelemetry: BicepTelemetryEvent.DecompileSuccess(pathsToSave.Length, telemetryProperties.CountConflictingFiles, telemetryProperties.OverwriteFiles)
                );
        }

        private async Task<(string outputFolder, PathToSave[] pathsToSave)?> DeterminateFinalPathsToSave(StringBuilder output, string outputFolder, PathToSave[] pathsToSave, string mainBicepPath, TelemetryProperties telemetryProperties)
        {
            pathsToSave = pathsToSave.ToArray(); // clone
            PathToSave[] existingPaths = pathsToSave.Where(f => File.Exists(f.Path)).ToArray();
            var singleFileDecompilation = pathsToSave.Length == 1;
            bool overwriteFiles = false;

            telemetryProperties.CountConflictingFiles = existingPaths.Length;

            if (existingPaths.Any())
            {
                if (clientCapabilitiesProvider.DoesClientSupportShowMessageRequest())
                {
                    Debug.Assert(!singleFileDecompilation || existingPaths[0].Path == pathsToSave[0].Path);

                    var overwriteAction = new MessageActionItem() { Title = singleFileDecompilation ? "Overwrite" : "Overwrite all" };
                    var createCopyAction = new MessageActionItem() { Title = singleFileDecompilation ? "Create copy" : "New subfolder" };
                    var cancelAction = new MessageActionItem() { Title = "Cancel" };
                    var conflictFilesWithQuotes = string.Join(", ", existingPaths.Select(f => "\"" + Path.GetRelativePath(outputFolder, f.Path) + "\""));
                    var message =
                        singleFileDecompilation ?
                        $"Output file already exists: {conflictFilesWithQuotes}" :
                        $"There are multiple decompilation output files and the following already exist: {conflictFilesWithQuotes}";
                    Log(output, message);

                    var result = await server.Window.ShowMessageRequest(
                        new()
                        {
                            Message = message,
                            Actions = new(new[] { overwriteAction, createCopyAction, cancelAction })
                        });

                    if (result.Title == overwriteAction.Title)
                    {
                        Log(output, "Will overwrite existing files.");
                        telemetryProperties.OverwriteFiles = overwriteFiles = true;
                    }
                    else if (result.Title == cancelAction.Title)
                    {
                        Log(output, "Decompile canceled.");
                        return null;
                    }
                    else
                    {
                        Debug.Assert(result.Title == createCopyAction.Title, $"Unexpected result {result.Title}");
                        overwriteFiles = false;
                    }
                }
                else
                {
                    // If client doesn't support asking, assume to create a copy
                    overwriteFiles = false;
                }

                if (!overwriteFiles)
                {
                    if (singleFileDecompilation)
                    {
                        // Pick new output filename
                        Debug.Assert(pathsToSave.Length == 1);
                        string newBicepPath = FindUniqueFileOrFolderName(outputFolder, Path.GetFileNameWithoutExtension(mainBicepPath), ".bicep");
                        pathsToSave[0] = new PathToSave(newBicepPath, pathsToSave[0].Contents);
                    }
                    else
                    {
                        // Output to a new subfolder
                        var newOutputFolder = CreateUniqueSubfolder(outputFolder, $"{Path.GetFileNameWithoutExtension(mainBicepPath)}_decompiled");
                        pathsToSave = pathsToSave.Select(f => new PathToSave(Path.Combine(newOutputFolder, Path.GetRelativePath(outputFolder, f.Path)), f.Contents)).ToArray();
                        outputFolder = newOutputFolder;
                        Log(output, $"Created new subfolder for output files: {outputFolder}");
                    }
                }
            }

            return (outputFolder, pathsToSave);
        }

        private static void SaveOutputFiles(StringBuilder output, PathToSave[] pathsToSave)
        {
            foreach (var (path, content) in pathsToSave)
            {
                Log(output, File.Exists(path) ? $"Overwriting {path}" : $"Writing {path}");
                File.WriteAllText(path, content);
            }
        }

        private string CreateUniqueSubfolder(string root, string desiredName)
        {
            string folder = FindUniqueFileOrFolderName(root, desiredName, "");
            Directory.CreateDirectory(folder);
            return folder;
        }

        private string FindUniqueFileOrFolderName(string root, string desiredName, string extension)
        {
            int nextAppend = 2;
            string path = Path.Join(root, $"{desiredName}{extension}");
            do
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    return path;
                }

                path = Path.Join(root, $"{desiredName}{nextAppend}{extension}");
                ++nextAppend;
            } while (true);
        }

        private static void Log(StringBuilder output, string message)
        {
            output.AppendLine(message);
            Trace.TraceInformation(message);
        }
    }
}
