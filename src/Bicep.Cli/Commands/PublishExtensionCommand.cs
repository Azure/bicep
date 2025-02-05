// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class PublishExtensionCommand : ICommand
    {
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileSystem fileSystem;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly IOContext ioContext;

        public PublishExtensionCommand(
            IOContext ioContext,
            IModuleDispatcher moduleDispatcher,
            ISourceFileFactory sourceFileFactory,
            IFileSystem fileSystem)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.fileSystem = fileSystem;
            this.sourceFileFactory = sourceFileFactory;
            this.ioContext = ioContext;
        }

        public async Task<int> RunAsync(PublishExtensionArguments args)
        {
            ExtensionBinary? TryGetBinary(SupportedArchitecture architecture)
            {
                if (args.Binaries.TryGetValue(architecture.Name) is not { } binaryPath)
                {
                    return null;
                }

                using var binaryStream = fileSystem.FileStream.New(PathHelper.ResolvePath(binaryPath), FileMode.Open, FileAccess.Read, FileShare.Read);
                return new(architecture, BinaryData.FromStream(binaryStream));
            }

            await ioContext.Error.WriteLineAsync($"WARNING: The '{args.CommandName}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

            var indexPath = PathHelper.ResolvePath(args.IndexFile);
            var indexUri = PathHelper.FilePathToFileUrl(indexPath);
            var reference = ValidateReference(args.TargetExtensionReference, indexUri);
            var overwriteIfExists = args.Force;

            BinaryData tarPayload;
            try
            {
                tarPayload = await TypesV1Archive.GenerateExtensionTarStream(this.fileSystem, indexPath);
                ValidateExtension(tarPayload);
            }
            catch (Exception exception)
            {
                throw new BicepException($"Extension package creation failed: {exception.Message}");
            }

            var binaries = SupportedArchitectures.All.Select(TryGetBinary).WhereNotNull().ToImmutableArray();

            var package = new ExtensionPackage(
                Types: tarPayload,
                LocalDeployEnabled: binaries.Any(),
                Binaries: binaries);

            await this.PublishExtensionAsync(reference, package, overwriteIfExists);
            return 0;
        }

        private async Task PublishExtensionAsync(ArtifactReference target, ExtensionPackage package, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure extension doesn't exist
                if (!overwriteIfExists && await this.moduleDispatcher.CheckExtensionExists(target))
                {
                    throw new BicepException($"The extension \"{target.FullyQualifiedReference}\" already exists. Use --force to overwrite the existing extension.");
                }
                await this.moduleDispatcher.PublishExtension(target, package);
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish extension \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetReference, Uri targetUri)
        {
            if (!targetReference.StartsWith("br:"))
            {
                // convert to a relative path, as this is the only format supported for the local filesystem
                targetUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(targetReference));
                targetReference = Path.GetFileName(targetUri.LocalPath);
            }

            var dummyReferencingFile = this.sourceFileFactory.CreateBicepFile(targetUri, "");


            if (!this.moduleDispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Extension, targetReference).IsSuccess(out var extensionReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!this.moduleDispatcher.GetRegistryCapabilities(ArtifactType.Extension, extensionReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified extension target \"{targetReference}\" is not supported.");
            }

            return extensionReference;
        }

        private static void ValidateExtension(BinaryData extension)
        {
            using var tempStream = extension.ToStream();

            var typeLoader = OciTypeLoader.FromStream(tempStream);
            var index = typeLoader.LoadTypeIndex();
            foreach (var (_, typeLocation) in index.Resources)
            {
                typeLoader.LoadResourceType(typeLocation);
            }
        }
    }
}
