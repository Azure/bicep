// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Decompiler;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Path = System.IO.Path;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDecompileSaveCommandParams(
        string decompileId,
        DecompiledFile[] outputFiles, // first is assumed to be main output file
        bool overwrite
    );
    public record BicepDecompileSaveCommandResult(
        string output,
        string? errorMessage,
        string? mainSavedBicepPath,
        string[] savedPaths);

    /// <summary>
    /// Handles a request from the client to decompile a JSON file for given a file path, creating one or more bicep files
    /// </summary>
    public class BicepDecompileSaveCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDecompileSaveCommandParams, BicepDecompileSaveCommandResult>
    {
        private readonly TemplateDecompiler templateDecompiler;
        private readonly ILanguageServerFacade languageServerFacade;
        private readonly IClientCapabilitiesProvider clientCapabilitiesProvider;
        private readonly TelemetryAndErrorHandlingHelper<BicepDecompileSaveCommandResult> telemetryHelper;

        public BicepDecompileSaveCommandHandler(
            ISerializer serializer,
            IFeatureProviderFactory featureProviderFactory,
            INamespaceProvider namespaceProvider,
            IModuleRegistryProvider registryProvider,
            ILanguageServerFacade server,
            ITelemetryProvider telemetryProvider,
            IApiVersionProviderFactory apiVersionProviderFactory,
            IBicepAnalyzer bicepAnalyzer,
            IFileResolver fileResolver,
            IClientCapabilitiesProvider clientCapabilitiesProvider,
            ILanguageServerFacade languageServerFacade)
            : base(LangServerConstants.DecompileSaveCommand, serializer)
        {
            this.telemetryHelper = new TelemetryAndErrorHandlingHelper<BicepDecompileSaveCommandResult>(server.Window, telemetryProvider);

            this.templateDecompiler = new TemplateDecompiler(featureProviderFactory, namespaceProvider, fileResolver, registryProvider, apiVersionProviderFactory, bicepAnalyzer);
            this.clientCapabilitiesProvider = clientCapabilitiesProvider;
            this.languageServerFacade = languageServerFacade;
        }

        public override Task<BicepDecompileSaveCommandResult> Handle(BicepDecompileSaveCommandParams parameters, CancellationToken cancellationToken)
        {
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling(async () =>
            {
                return await SaveDecompileResults(parameters.decompileId, parameters.outputFiles, parameters.overwrite);
            });
        }

        private async Task<(BicepDecompileSaveCommandResult result, BicepTelemetryEvent? successTelemetry)> SaveDecompileResults(
            string decompileId,
            DecompiledFile[] outputFiles,
            bool overwrite
        )
        {
            StringBuilder output = new StringBuilder();

            try
            {
                if (outputFiles.Length == 0)
                {
                    throw new ArgumentException($"{nameof(outputFiles)} should not be empty");
                }

                string proposedMainBicepPath = outputFiles[0].absolutePath;
                string? outputFolder = Path.GetDirectoryName(proposedMainBicepPath);
                if (outputFolder is null)
                {
                    throw new ArgumentException($"Invalid input path {proposedMainBicepPath}");
                }

                // Figure out proper place to save files
                (string path, string contents)[] filesToSave = DeterminePathsToSave(output, ref outputFolder, proposedMainBicepPath, outputFiles, overwrite);
                if (!overwrite)
                {
                    Debug.Assert(filesToSave.All(f => IsPathRelativeToBasePath(f.path, outputFolder)), $"Expected all output files to be relative to {outputFolder}");
                }
                var actualMainBicepPath = filesToSave[0].path;

                // Save files
                SaveFiles(output, filesToSave);

                Log(output, $"Decompilation complete.");

                // Show main output file
                if (this.clientCapabilitiesProvider.DoesClientSupportShowDocumentRequest())
                {
                    await this.languageServerFacade.Window.ShowDocument(
                        new()
                        {
                            TakeFocus = true,
                            Uri = actualMainBicepPath,
                        },
                        CancellationToken.None);
                }

                return (
                    result: new BicepDecompileSaveCommandResult(
                        output.ToString(),
                        null,
                        actualMainBicepPath,
                        filesToSave.Select(f => f.path).ToArray()),
                    successTelemetry: BicepTelemetryEvent.DecompileSaveSuccess(decompileId)
                    );
            }
            catch (Exception ex)
            {
                Log(output, ex.Message);
                throw telemetryHelper.CreateException(
                    ex.Message,
                    BicepTelemetryEvent.DecompileSaveFailure(decompileId, ex.GetType().Name),
                    new BicepDecompileSaveCommandResult(
                        output.ToString(),
                        ex.Message,
                        null,
                        new string[] { })
                );
            }
        }

        private (string path, string contents)[] DeterminePathsToSave(StringBuilder output, ref string outputFolder, string mainBicepPath, DecompiledFile[] outputFiles, bool overwrite)
        {
            var singleFileDecompilation = outputFiles.Length == 1;

            if (overwrite)
            {
                // Save to original paths
                return outputFiles.Select(of => (of.absolutePath, of.bicepContents)).ToArray();
            }
            {
                if (singleFileDecompilation)
                {
                    // Create a bicep file with unique name alongside the existing bicep file
                    string newBicepPath = FindUniqueFileOrFolderName(outputFolder, mainBicepPath);
                    return new[] { (newBicepPath, outputFiles[0].bicepContents) };
                }
                else
                {
                    // Output every to a new subfolder (using the relative clonable paths)
                    var newOutputFolder = CreateUniqueSubfolder(outputFolder, $"{Path.GetFileNameWithoutExtension(mainBicepPath)}_decompiled");
                    Log(output, $"Created new subfolder for output files: {newOutputFolder}");
                    outputFolder = newOutputFolder;

                    var newOutputFiles = new List<(string path, string contents)>();
                    foreach (var outputFile in outputFiles)
                    {
                        var newPath = MakePathRelativeToFolder(newOutputFolder, Path.Combine(newOutputFolder, outputFile.clonableRelativePath), newOutputFiles.Select(f => f.path).ToArray());
                        newOutputFiles.Add((newPath, outputFile.bicepContents));
                    }

                    return newOutputFiles.ToArray();
                }
            }
        }

        private string MakePathRelativeToFolder(string baseFolder, string path, string[] existingPaths)
        {
            if (IsPathRelativeToBasePath(path, baseFolder))
            {
                return path;
            }
            else
            {
                // Munge the name
                string folderName = Path.GetDirectoryName(path) ?? "invalidpath";
                folderName = Path.GetFileName(folderName); // Get last folder name

                foreach (char c in Path.GetInvalidPathChars())
                {
                    folderName = folderName.Replace(c, '_');
                }
                folderName = folderName.Replace("/", "_")
                    .Replace("\\", "_")
                    .Replace("..", "parent")
                    .Replace(".", "_");

                string mungedFilename = folderName + "_" + Path.GetFileName(path);

                var uniquePath = FindUniqueFileOrFolderName(
                    baseFolder,
                    mungedFilename,
                    path => existingPaths.Contains(path));

                Debug.Assert(IsPathRelativeToBasePath(uniquePath, baseFolder), $"Expected {uniquePath} to be relative to {baseFolder}");
                return uniquePath;
            }
        }

        private bool IsPathRelativeToBasePath(string path, string baseFolder)
        {
            baseFolder = Path.EndsInDirectorySeparator(baseFolder) ? baseFolder : baseFolder + Path.DirectorySeparatorChar;
            string relativePath = Path.GetRelativePath(baseFolder, path);
            return !Path.IsPathFullyQualified(relativePath) && !relativePath.StartsWith("..");
        }

        private static void Log(StringBuilder output, string message)
        {
            output.AppendLine(message);
            Trace.TraceInformation(message);
        }


        private static void SaveFiles(StringBuilder output, (string path, string contents)[] pathsToSave)
        {
            foreach (var (path, content) in pathsToSave)
            {

                Log(output, File.Exists(path) ? $"Overwriting {path}" : $"Writing {path}");
                File.WriteAllText(path, content);
            }
        }

        private string CreateUniqueSubfolder(string root, string desiredName)
        {
            string folder = FindUniqueFileOrFolderName(root, desiredName);
            Directory.CreateDirectory(folder);
            return folder;
        }

        delegate bool DoesPathExist(string path);
        private string FindUniqueFileOrFolderName(string root, string desiredFilename, DoesPathExist doesPathExist)
        {
            string desiredName = Path.GetFileNameWithoutExtension(desiredFilename);
            string extension = Path.GetExtension(desiredFilename);

            int nextAppend = 2;
            string path = Path.Join(root, $"{desiredName}{extension}");
            do
            {
                if (!doesPathExist(path))
                {
                    return path;
                }

                path = Path.Join(root, $"{desiredName}{nextAppend}{extension}");
                ++nextAppend;
            } while (true);
        }

        private string FindUniqueFileOrFolderName(string root, string desiredFilename)
        {
            return FindUniqueFileOrFolderName(root, desiredFilename, path => File.Exists(path) || Directory.Exists(path));
        }
    }
}
