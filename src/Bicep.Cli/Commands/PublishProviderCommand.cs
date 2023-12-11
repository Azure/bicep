// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.TypeSystem.Providers;
using System.IO.Compression;
using Bicep.Core.Registry.Providers;

namespace Bicep.Cli.Commands
{
    public class PublishProviderCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter compilationWriter;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileSystem fileSystem;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IOContext ioContext;
        private readonly ILogger logger;

        public PublishProviderCommand(
            IDiagnosticLogger diagnosticLogger,
            CompilationService compilationService,
            IOContext ioContext,
            ILogger logger,
            CompilationWriter compilationWriter,
            IModuleDispatcher moduleDispatcher,
            IFileSystem fileSystem,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
            this.compilationWriter = compilationWriter;
            this.moduleDispatcher = moduleDispatcher;
            this.fileSystem = fileSystem;
            this.featureProviderFactory = featureProviderFactory;
            this.ioContext = ioContext;
            this.logger = logger;
        }

        public async Task<int> RunAsync(PublishProviderArguments args)
        {
            var indexPath = PathHelper.ResolvePath(args.IndexFile);
            var inputUri = PathHelper.FilePathToFileUrl(indexPath);
            var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(indexPath));
            var documentationUri = args.DocumentationUri;
            var providerReference = ValidateReference(args.TargetProviderReference, inputUri);
            var overwriteIfExists = args.Force;

            await ioContext.Error.WriteLineAsync("The 'publish-provider' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

            //TODO: attempt to validate types here

            IFileSystem fileSystem = new FileSystem();
            Stream tarStream = await TypesV1Archive.GenerateProviderTarStream(fileSystem, indexPath);

            await this.PublishProviderAsync(providerReference, tarStream, overwriteIfExists);
            return 0;
        }

        private async Task PublishProviderAsync(ArtifactReference target, Stream compiledArmTemplate, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure module doesn't exist
                if (!overwriteIfExists && await this.moduleDispatcher.CheckProviderExists(target))
                {
                    throw new BicepException($"The Provider \"{target.FullyQualifiedReference}\" already exists in registry. Use --force to overwrite the existing provider.");
                }
                await this.moduleDispatcher.PublishProvider(target, compiledArmTemplate);
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish provider \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetProviderReference, Uri targetProviderUri)
        {
            if (!this.moduleDispatcher.TryGetArtifactReference(ArtifactType.Provider, targetProviderReference, targetProviderUri).IsSuccess(out var providerReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!this.moduleDispatcher.GetRegistryCapabilities(providerReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified provider target \"{targetProviderReference}\" is not supported.");
            }

            return providerReference;
        }
    }
}
