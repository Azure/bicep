// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Cli.Commands
{
    public class PublishCommand : ICommand
    {
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter compilationWriter;
        private readonly IModuleDispatcher moduleDispatcher;

        public PublishCommand(
            IDiagnosticLogger diagnosticLogger,
            CompilationService compilationService,
            CompilationWriter compilationWriter,
            IModuleDispatcher moduleDispatcher)
        {
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
            this.compilationWriter = compilationWriter;
            this.moduleDispatcher = moduleDispatcher;
        }

        public async Task<int> RunAsync(PublishArguments args)
        {
            var moduleReference = ValidateReference(args.TargetModuleReference);

            var inputPath = PathHelper.ResolvePath(args.InputFile);

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if(diagnosticLogger.ErrorCount > 0)
            {
                // can't publish if we can't compile
                return 1;
            }

            var stream = new MemoryStream();
            compilationWriter.ToStream(compilation, stream);

            stream.Position = 0;
            await this.moduleDispatcher.PublishModule(moduleReference, stream);

            return 0;
        }

        private ModuleReference ValidateReference(string targetModuleReference)
        {
            var moduleReference = this.moduleDispatcher.TryGetModuleReference(targetModuleReference, out var failureBuilder);
            if(moduleReference is null)
            {
                failureBuilder = failureBuilder ?? throw new InvalidOperationException($"{nameof(moduleDispatcher.TryGetModuleReference)} did not provide an error.");

                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(new DiagnosticBuilder.DiagnosticBuilderInternal(new TextSpan(0, 0))).Message;

                throw new BicepException(message);
            }

            if(!this.moduleDispatcher.GetRegistryCapabilities(moduleReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified module target \"{targetModuleReference}\" is not supported.");
            }

            return moduleReference;
        }
    }
}
