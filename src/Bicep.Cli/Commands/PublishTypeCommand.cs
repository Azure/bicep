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
using Bicep.Core.UnitTests;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Cli.Commands
{
    public class PublishTypeCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter compilationWriter;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileSystem fileSystem;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IOContext ioContext;
        private readonly ILogger logger;

        public PublishTypeCommand(
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

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public async Task<int> RunAsync(PublishTypeArguments args)
        {
            var indexPath = PathHelper.ResolvePath(args.IndexFile);
            var inputUri = PathHelper.FilePathToFileUrl(indexPath);
            var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(indexPath));
            var documentationUri = args.DocumentationUri;
            var typeReference = ValidateReference(args.TargetTypeReference, inputUri);
            var overwriteIfExists = args.Force;

            //attempt to validate types here, move to better location later

            AzResourceTypeLoader azTypeLoader = new(FileAzTypeLoader.FromFile(indexPath));

            var availableTypes = azTypeLoader.GetAvailableTypes();

            //Going to assume that this is enough validation, if it is not, we'll have to create the namespace, factory etc. similar to what happens in the AzResourceTypeProvider_can_deserialize_all_types_without_throwing function
            foreach (var type in availableTypes)
            {
                azTypeLoader.LoadType(type);
            }

            if (PathHelper.HasArmTemplateLikeExtension(inputUri))
            {
                // Publishing an ARM template file.
                using var armTemplateStream = this.fileSystem.FileStream.New(indexPath, FileMode.Open, FileAccess.Read);
                await this.PublishTypeAsync(typeReference, armTemplateStream, overwriteIfExists);

                return 0;
            }

            var compilation = await compilationService.CompileAsync(indexPath, args.NoRestore);

            if (diagnosticLogger.ErrorCount > 0)
            {
                // can't publish if we can't compile
                return 1;
            }

            var compiledArmTemplateStream = new MemoryStream();
            compilationWriter.ToStream(compilation, compiledArmTemplateStream);
            compiledArmTemplateStream.Position = 0;

            return 0;
        }

        private async Task PublishTypeAsync(ArtifactReference target, Stream compiledArmTemplate, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure module doesn't exist
                if (!overwriteIfExists && await this.moduleDispatcher.CheckTypeExists(target))
                {
                    throw new BicepException($"The Type \"{target.FullyQualifiedReference}\" already exists in registry. Use --force to overwrite the existing type.");
                }
                await this.moduleDispatcher.PublishType(target, compiledArmTemplate);
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish type \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetTypeReference, Uri targetTypeUri)
        {
            if (!this.moduleDispatcher.TryGetArtifactReference(ArtifactType.Type, targetTypeReference, targetTypeUri).IsSuccess(out var typeReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!this.moduleDispatcher.GetRegistryCapabilities(typeReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified type target \"{targetTypeReference}\" is not supported.");
            }

            return typeReference;
        }
    }
}
