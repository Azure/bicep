// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.Diagnostics;
using System.IO.Abstractions;
using Azure.Core;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

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
                    await ioContext.Error.Writer.WriteLineAsync($"Cannot publish with source when the target is an ARM template file.");
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

            var compilation = await compiler.CreateCompilation(inputUri, skipRestore: args.NoRestore);
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
            var dummyReferencingFile = this.sourceFileFactory.CreateBicepFile(targetModuleUri, string.Empty);

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

        internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
        {
            var command = new System.CommandLine.Command(Constants.Command.Publish, "Publishes the .bicep file to the module registry.");

            var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the .bicep file to publish.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var targetOption = new System.CommandLine.Option<string>(Option.Target)
            {
                Description = "The target module reference.",
            };
            var documentationUriOption = new System.CommandLine.Option<string[]>(Option.DocumentationUri)
            {
                Description = "Module documentation URI.",
                Arity = ArgumentArity.ZeroOrMore,
            };
            var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
            {
                Description = "Do not restore modules prior to publishing.",
            };
            var forceOption = new System.CommandLine.Option<bool>(Option.Force)
            {
                Description = "Overwrite existing published module or file.",
            };
            var withSourceOption = new System.CommandLine.Option<bool>(Option.WithSource)
            {
                Description = "[Experimental] Publish source code with the module.",
            };

            command.Add(inputFileArgument);
            command.Add(targetOption);
            command.Add(documentationUriOption);
            command.Add(noRestoreOption);
            command.Add(forceOption);
            command.Add(withSourceOption);
            command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(async () =>
            {
                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var target = result.GetValue(targetOption)
                    ?? throw new CommandLineException("The target module was not specified.");

                var docUriResult = result.GetResult(documentationUriOption);
                if (docUriResult is not null)
                {
                    if (docUriResult.Tokens.Count == 0)
                    {
                        throw new CommandLineException("The --documentation-uri parameter expects an argument.");
                    }
                    if (docUriResult.Tokens.Count > 1)
                    {
                        throw new CommandLineException("The --documentation-uri parameter cannot be specified more than once.");
                    }
                }

                var documentationUri = docUriResult?.Tokens.Count == 1 ? docUriResult.Tokens[0].Value : null;
                if (documentationUri is not null && !Uri.IsWellFormedUriString(documentationUri, UriKind.Absolute))
                {
                    throw new CommandLineException("The --documentation-uri should be a well formed uri string.");
                }

                var args = new PublishArguments(
                    inputFile,
                    target,
                    documentationUri,
                    result.GetValue(noRestoreOption),
                    result.GetValue(forceOption),
                    result.GetValue(withSourceOption));

                return await context.GetCommand<PublishCommand>().RunAsync(args);
            }));

            return command;
        }
    }
}
