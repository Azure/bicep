// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
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
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileSystem fileSystem;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IOContext ioContext;
        private readonly ILogger logger;

        public PublishCommand(
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            IOContext ioContext,
            ILogger logger,
            IModuleDispatcher moduleDispatcher,
            IFileSystem fileSystem,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.moduleDispatcher = moduleDispatcher;
            this.fileSystem = fileSystem;
            this.featureProviderFactory = featureProviderFactory;
            this.ioContext = ioContext;
            this.logger = logger;
        }

        public async Task<int> RunAsync(PublishArguments args)
        {
            var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
            var features = featureProviderFactory.GetFeatureProvider(inputUri);
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
                using var armTemplateStream = this.fileSystem.FileStream.New(inputUri.LocalPath, FileMode.Open, FileAccess.Read);
                await this.PublishModuleAsync(moduleReference, BinaryData.FromStream(armTemplateStream), null, documentationUri, overwriteIfExists);

                return 0;
            }

            var compilation = await compiler.CreateCompilation(inputUri, skipRestore: args.NoRestore);
            var result = compilation.Emitter.Template();

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, result.Diagnostics);

            if (result.Template is not { } compiledArmTemplate)
            {
                // can't publish if we can't compile
                return 1;
            }

            // Handle publishing source
            Stream? sourcesStream = null;
            if (publishSource)
            {
                sourcesStream = SourceArchive.PackSourcesIntoStream(moduleDispatcher, compilation.SourceFileGrouping, features.CacheRootDirectory);
                Trace.WriteLine("Publishing Bicep module with source");
            }

            using (sourcesStream)
            {
                Trace.WriteLine(sourcesStream is { } ? "Publishing Bicep module with source" : "Publishing Bicep module without source");
                var sourcesPayload = sourcesStream is { } ? BinaryData.FromStream(sourcesStream) : null;
                await this.PublishModuleAsync(moduleReference, BinaryData.FromString(compiledArmTemplate), sourcesPayload, documentationUri, overwriteIfExists);
            }

            return 0;
        }

        private async Task PublishModuleAsync(ArtifactReference target, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, bool overwriteIfExists)
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

            if (!this.moduleDispatcher.GetRegistryCapabilities(ArtifactType.Module, moduleReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified module target \"{targetModuleReference}\" is not supported.");
            }

            return moduleReference;
        }
    }
}
