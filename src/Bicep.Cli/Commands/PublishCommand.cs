// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Bicep.Cli.Commands
{
    public class PublishCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter compilationWriter;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IConfigurationManager configurationManager;
        private readonly IFileSystem fileSystem;

        public PublishCommand(
            IDiagnosticLogger diagnosticLogger,
            CompilationService compilationService,
            CompilationWriter compilationWriter,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IFileSystem fileSystem)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
            this.compilationWriter = compilationWriter;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.fileSystem = fileSystem;
        }

        public async Task<int> RunAsync(PublishArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var configuration = this.configurationManager.GetConfiguration(inputUri);
            var moduleReference = ValidateReference(args.TargetModuleReference, configuration);

            if (PathHelper.HasArmTemplateLikeExtension(inputUri))
            {
                // Publishing an ARM template file.
                using var armTemplateStream = this.fileSystem.FileStream.Create(inputPath, FileMode.Open, FileAccess.Read);
                await this.PublishModuleAsync(configuration, moduleReference, armTemplateStream);

                return 0;
            }

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (diagnosticLogger.ErrorCount > 0)
            {
                // can't publish if we can't compile
                return 1;
            }

            var stream = new MemoryStream();
            compilationWriter.ToStream(compilation, stream);

            stream.Position = 0;
            await this.PublishModuleAsync(compilation.Configuration, moduleReference, stream);

            return 0;
        }

        private async Task PublishModuleAsync(RootConfiguration configuration, ModuleReference target, Stream stream)
        {
            try
            {
                await this.moduleDispatcher.PublishModule(configuration, target, stream);
            }
            catch (ExternalModuleException exception)
            {
                throw new BicepException($"Unable to publish module \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ModuleReference ValidateReference(string targetModuleReference, RootConfiguration configuration)
        {
            var moduleReference = this.moduleDispatcher.TryGetModuleReference(targetModuleReference, configuration, out var failureBuilder);
            if (moduleReference is null)
            {
                failureBuilder = failureBuilder ?? throw new InvalidOperationException($"{nameof(moduleDispatcher.TryGetModuleReference)} did not provide an error.");

                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0))).Message;

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
