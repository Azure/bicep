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
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Providers;
using Bicep.Core.TypeSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class PublishProviderCommand : ICommand
    {
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileSystem fileSystem;
        private readonly IOContext ioContext;
        public PublishProviderCommand(
            IOContext ioContext,
            IModuleDispatcher moduleDispatcher,
            IFileSystem fileSystem)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.fileSystem = fileSystem;
            this.ioContext = ioContext;
        }

        public async Task<int> RunAsync(PublishProviderArguments args)
        {
            ProviderBinary? TryGetBinary(SupportedArchitecture architecture)
            {
                if (args.Binaries.TryGetValue(architecture.Name) is not { } binaryPath)
                {
                    return null;
                }

                using var binaryStream = fileSystem.FileStream.New(PathHelper.ResolvePath(binaryPath), FileMode.Open, FileAccess.Read, FileShare.Read);
                return new(architecture, BinaryData.FromStream(binaryStream));
            }

            await ioContext.Error.WriteLineAsync("The 'publish-provider' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

            var indexPath = PathHelper.ResolvePath(args.IndexFile);
            var indexUri = PathHelper.FilePathToFileUrl(indexPath);
            var providerReference = ValidateReference(args.TargetProviderReference, indexUri);
            var overwriteIfExists = args.Force;

            BinaryData tarPayload;
            try
            {
                tarPayload = await TypesV1Archive.GenerateProviderTarStream(this.fileSystem, indexPath);
                ValidateProvider(tarPayload);
            }
            catch (Exception exception)
            {
                throw new BicepException($"Provider package creation failed: {exception.Message}");
            }

            var binaries = SupportedArchitectures.All.Select(TryGetBinary).WhereNotNull().ToImmutableArray();

            var package = new ProviderPackage(
                Types: tarPayload,
                LocalDeployEnabled: binaries.Any(),
                Binaries: binaries);

            await this.PublishProviderAsync(providerReference, package, overwriteIfExists);
            return 0;
        }

        private async Task PublishProviderAsync(ArtifactReference target, ProviderPackage package, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure provider doesn't exist
                if (!overwriteIfExists && await this.moduleDispatcher.CheckProviderExists(target))
                {
                    throw new BicepException($"The Provider \"{target.FullyQualifiedReference}\" already exists. Use --force to overwrite the existing provider.");
                }
                await this.moduleDispatcher.PublishProvider(target, package);
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish provider \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetProviderReference, Uri targetProviderUri)
        {
            if (!targetProviderReference.StartsWith("br:"))
            {
                // convert to a relative path, as this is the only format supported for the local filesystem
                targetProviderUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(targetProviderReference));
                targetProviderReference = Path.GetFileName(targetProviderUri.LocalPath);
            }

            if (!this.moduleDispatcher.TryGetArtifactReference(ArtifactType.Provider, targetProviderReference, targetProviderUri).IsSuccess(out var providerReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!this.moduleDispatcher.GetRegistryCapabilities(ArtifactType.Provider, providerReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified provider target \"{targetProviderReference}\" is not supported.");
            }

            return providerReference;
        }

        private static void ValidateProvider(BinaryData provider)
        {
            using var tempStream = provider.ToStream();

            var typeLoader = OciTypeLoader.FromStream(tempStream);
            var index = typeLoader.LoadTypeIndex();
            foreach (var (_, typeLocation) in index.Resources)
            {
                typeLoader.LoadResourceType(typeLocation);
            }
        }
    }
}
