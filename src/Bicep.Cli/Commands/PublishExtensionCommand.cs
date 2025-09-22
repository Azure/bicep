// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Bicep.Cli.Arguments;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Deploy.Types;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class PublishExtensionCommand(
        IModuleDispatcher moduleDispatcher,
        ISourceFileFactory sourceFileFactory,
        IFileExplorer fileExplorer,
        InputOutputArgumentsResolver inputOutputArgumentsResolver,
        ILocalExtensionFactory localExtensionFactory,
        ILogger logger) : ICommand
    {
        public async Task<int> RunAsync(PublishExtensionArguments args, CancellationToken cancellationToken)
        {
            ExtensionBinary? TryGetBinary(SupportedArchitecture architecture)
            {
                if (args.Binaries.TryGetValue(architecture.Name) is not { } binaryPath)
                {
                    return null;
                }

                var binaryUri = inputOutputArgumentsResolver.PathToUri(binaryPath);
                using var binaryStream = fileExplorer.GetFile(binaryUri).OpenRead();
                return new(architecture, BinaryData.FromStream(binaryStream));
            }

            if (args.IndexFile is null && args.Binaries.Count == 0)
            {
                throw new CommandLineException($"The input file path was not specified.");
            }

            logger.LogWarning($"WARNING: The '{args.CommandName}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");
            var reference = ValidateReference(args.TargetExtensionReference);
            var overwriteIfExists = args.Force;

            var binaries = SupportedArchitectures.All.Select(TryGetBinary).WhereNotNull().ToImmutableArray();
            var tarPayload = await GetTypesTarPayload(args, binaries, cancellationToken);

            var package = new ExtensionPackage(
                Types: tarPayload,
                LocalDeployEnabled: binaries.Any(),
                Binaries: binaries);

            await this.PublishExtensionAsync(reference, package, overwriteIfExists);
            return 0;
        }

        private async Task<BinaryData> GetTypesTarPayload(PublishExtensionArguments args, ImmutableArray<ExtensionBinary> binaries, CancellationToken cancellationToken)
        {
            if (args.IndexFile is { })
            {
                var indexUri = inputOutputArgumentsResolver.PathToUri(args.IndexFile);
                var indexFile = fileExplorer.GetFile(indexUri);

                return await CreateTypesTar(indexFile);
            }

            if (!binaries.Any())
            {
                throw new CommandLineException($"The input file path was not specified.");
            }

            if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
            {
                throw new BicepException($"Failed to load type information: Unable to determine the current architecture and OS platform.");
            }

            if (args.Binaries.TryGetValue(architecture.Name) is not { } binaryPath ||
                inputOutputArgumentsResolver.PathToUri(binaryPath) is not { } binaryUri)
            {
                throw new BicepException($"Failed to load type information: Unable to find a binary for the current architecture ({architecture.Name}).");
            }

            var indexHandle = await GetTypesFromExtension(binaryUri, cancellationToken);
            return await CreateTypesTar(indexHandle);
        }

        private async Task PublishExtensionAsync(ArtifactReference target, ExtensionPackage package, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure extension doesn't exist
                if (!overwriteIfExists && await moduleDispatcher.CheckExtensionExists(target))
                {
                    throw new BicepException($"The extension \"{target.FullyQualifiedReference}\" already exists. Use --force to overwrite the existing extension.");
                }
                await moduleDispatcher.PublishExtension(target, package);
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish extension \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetReference)
        {
            IOUri dummyReferencingFileUri;

            if (!targetReference.StartsWith("br:") && !targetReference.StartsWith("ts:"))
            {
                // If targetReference is a fully qualified file, we need to create a dummy artifact referencing
                // file with the path, and change targetReference to be the file name so it is a relative reference,
                // because absoluate reference is not supported by Bicep.
                dummyReferencingFileUri = inputOutputArgumentsResolver.PathToUri(targetReference);
                targetReference = dummyReferencingFileUri.GetFileName();
            }
            else
            {
                dummyReferencingFileUri = new IOUri("file", "", "/dummy");
            }

            var dummyReferencingFile = sourceFileFactory.CreateBicepFile(dummyReferencingFileUri, "");

            if (!moduleDispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Extension, targetReference).IsSuccess(out var extensionReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!moduleDispatcher.GetRegistryCapabilities(ArtifactType.Extension, extensionReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified extension target \"{targetReference}\" is not supported.");
            }

            return extensionReference;
        }

        private static void ValidateExtension(BinaryData extension)
        {
            using var tempStream = extension.ToStream();

            var typeLoader = ArchivedTypeLoader.FromStream(tempStream);
            var index = typeLoader.LoadTypeIndex();
            foreach (var (_, typeLocation) in index.Resources)
            {
                typeLoader.LoadResourceType(typeLocation);
            }
        }

        private static async Task<BinaryData> CreateTypesTar(IFileHandle indexHandle)
        {
            try
            {
                var tarPayload = await TypesV1Archive.PackIntoBinaryData(indexHandle);
                ValidateExtension(tarPayload);

                return tarPayload;
            }
            catch (Exception exception)
            {
                throw new BicepException($"Extension package creation failed: {exception.Message}");
            }
        }

        private async Task<IFileHandle> GetTypesFromExtension(IOUri binaryUri, CancellationToken cancellationToken)
        {
            await using var extension = await localExtensionFactory.Start(binaryUri);

            var typeFiles = await extension.GetTypeFiles(cancellationToken);

            var fileExplorer = new InMemoryFileExplorer();

            var indexUri = IOUri.FromFilePath("/index.json");
            var indexHandle = fileExplorer.GetFile(indexUri);
            indexHandle.Write(typeFiles.IndexFileContent);

            foreach (var (path, content) in typeFiles.TypeFileContents)
            {
                var fileUri = IOUri.FromFilePath($"/{path}");
                fileExplorer.GetFile(fileUri).Write(content);
            }

            return indexHandle;
        }
    }
}
