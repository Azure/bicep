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
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class PublishCommand : ICommand
    {
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IFileExplorer fileExplorer;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly IOContext ioContext;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;

        public PublishCommand(
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            IOContext ioContext,
            IModuleDispatcher moduleDispatcher,
            ISourceFileFactory sourceFileFactory,
            IFileExplorer fileExplorer,
            InputOutputArgumentsResolver inputOutputArgumentsResolver)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.moduleDispatcher = moduleDispatcher;
            this.fileExplorer = fileExplorer;
            this.sourceFileFactory = sourceFileFactory;
            this.ioContext = ioContext;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
        }

        public async Task<int> RunAsync(PublishArguments args)
        {
            var inputUri = inputOutputArgumentsResolver.ResolveInputArguments(args);
            var documentationUri = args.DocumentationUri;
            var moduleReference = ValidateReference(args.TargetModuleReference, inputUri);
            var overwriteIfExists = args.Force;
            var publishSource = args.WithSource;

            if (inputUri.HasArmTemplateLikeExtension())
            {
                if (publishSource)
                {
                    await ioContext.Error.WriteLineAsync($"Cannot publish with source when the target is an ARM template file.");
                    return 1;
                }

                // Publishing an ARM template file.
                if (!this.fileExplorer.GetFile(inputUri).TryReadBinaryData().IsSuccess(out var templateData, out var diagnosticBuilder))
                {
                    var diagnostic = diagnosticBuilder(DiagnosticBuilder.ForDocumentStart());
                    throw new DiagnosticException(diagnostic);
                }

                await this.PublishModuleAsync(moduleReference, templateData, null, documentationUri, overwriteIfExists);
                return 0;
            }

            var compilation = await compiler.CreateCompilation(inputUri.ToUri(), skipRestore: args.NoRestore);
            var result = compilation.Emitter.Template();

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, result.Diagnostics);

            if (result.Template is not { } compiledArmTemplate)
            {
                // can't publish if we can't compile
                return 1;
            }

            // Handle publishing source
            SourceArchive? sourceArchive = null;
            if (publishSource)
            {
                sourceArchive = SourceArchive.CreateFrom(compilation.SourceFileGrouping);
                Trace.WriteLine("Publishing Bicep module with source");
            }

            Trace.WriteLine(sourceArchive is { } ? "Publishing Bicep module with source" : "Publishing Bicep module without source");
            var sourcesPayload = sourceArchive is { } ? sourceArchive.PackIntoBinaryData() : null;
            await this.PublishModuleAsync(moduleReference, BinaryData.FromString(compiledArmTemplate), sourcesPayload, documentationUri, overwriteIfExists);

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

        private ArtifactReference ValidateReference(string targetModuleReference, IOUri targetModuleUri)
        {
            var dummyReferencingFile = this.sourceFileFactory.CreateBicepFile(targetModuleUri.ToUri(), string.Empty);

            if (!this.moduleDispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Module, targetModuleReference).IsSuccess(out var moduleReference, out var failureBuilder))
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
