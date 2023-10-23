// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.SourceCode;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class PublishCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter compilationWriter;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileSystem fileSystem;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IOContext ioContext;
        private readonly ILogger logger;

        public PublishCommand(
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

        public async Task<int> RunAsync(PublishArguments args)
        {
            const string PublishSourceFeatureName = "publishSource";
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(inputPath));
            var documentationUri = args.DocumentationUri;
            var moduleReference = ValidateReference(args.TargetModuleReference, inputUri);
            var overwriteIfExists = args.Force;
            var publishSource = args.WithSource;

            if (PathHelper.HasArmTemplateLikeExtension(inputUri))
            {
                if (publishSource)
                {
                    await ioContext.Error.WriteLineAsync($"Cannot publish with source when the target is an ARM template file.");
                    return 1;
                }

                // Publishing an ARM template file.
                using var armTemplateStream = this.fileSystem.FileStream.New(inputPath, FileMode.Open, FileAccess.Read);
                await this.PublishModuleAsync(moduleReference, armTemplateStream, null, documentationUri, overwriteIfExists);

                return 0;
            }

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (diagnosticLogger.ErrorCount > 0)
            {
                // can't publish if we can't compile
                return 1;
            }

            var compiledArmTemplateStream = new MemoryStream();
            compilationWriter.ToStream(compilation, compiledArmTemplateStream);
            compiledArmTemplateStream.Position = 0;

            // Handle publishing source
            Stream? sourcesStream = null;
            if (publishSource)
            {
                if (!features.PublishSourceEnabled)
                {
                    await ioContext.Error.WriteLineAsync($"The experimental flag \"{PublishSourceFeatureName}\" must be enabled in bicepconfig.json in order to publish a module with source.");
                    return 1;
                }

                await ioContext.Output.WriteLineAsync(string.Format(CliResources.ExperimentalFeaturesDisclaimerMessage, PublishSourceFeatureName));

                sourcesStream = SourceArchive.PackSourcesIntoStream(compilation.SourceFileGrouping);
                Trace.WriteLine("Publishing Bicep module with source");
            }
            else
            {
                if (features.PublishSourceEnabled)
                {
                    await ioContext.Output.WriteLineAsync($"NOTE: Experimental feature {PublishSourceFeatureName} is enabled, but --with-source must also be specified to publish a module with source.");
                }
            }

            using (sourcesStream)
            {
                Trace.WriteLine(sourcesStream is { } ? "Publishing Bicep module with source" : "Publishing Bicep module without source");
                await this.PublishModuleAsync(moduleReference, compiledArmTemplateStream, sourcesStream, documentationUri, overwriteIfExists);
            }

            return 0;
        }

        private async Task PublishModuleAsync(ArtifactReference target, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure module doesn't exist
                if (!overwriteIfExists && await this.moduleDispatcher.CheckModuleExists(target))
                {
                    throw new BicepException($"The module \"{target.FullyQualifiedReference}\" already exists in registry. Use --force to overwrite the existing module.");
                }
                await this.moduleDispatcher.PublishModule(target, compiledArmTemplate, bicepSources, documentationUri);
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish module \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetModuleReference, Uri targetModuleUri)
        {
            if (!this.moduleDispatcher.TryGetArtifactReference(ArtifactType.Module, targetModuleReference, targetModuleUri).IsSuccess(out var moduleReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!this.moduleDispatcher.GetRegistryCapabilities(moduleReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified module target \"{targetModuleReference}\" is not supported.");
            }

            return moduleReference;
        }
    }
}
