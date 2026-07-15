// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using Azure.Core;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Deploy.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands
{
    public class PublishExtensionCommand(
        IModuleDispatcher moduleDispatcher,
        ISourceFileFactory sourceFileFactory,
        IFileExplorer fileExplorer,
        InputOutputArgumentsResolver inputOutputArgumentsResolver,
        ILocalExtensionFactory localExtensionFactory,
        IEnvironment environment,
        ILogger logger) : ICommand
    {
        public async Task<int> RunAsync(PublishExtensionArguments args, CancellationToken cancellationToken)
        {
            ExtensionBinary? TryGetBinary(SupportedArchitecture architecture)
            {
                if (args.Binaries.TryGetValue(architecture.Name) is not { } binaryPath)
                {
                    return null;
                }

                var binaryUri = inputOutputArgumentsResolver.PathToUri(binaryPath);
                using var binaryStream = fileExplorer.GetFile(binaryUri).OpenRead();
                return new(architecture, BinaryData.FromStream(binaryStream));
            }

            if (args.IndexFile is null && args.Binaries.Count == 0)
            {
                throw new CommandLineException($"The input file path was not specified.");
            }

            logger.LogWarning($"WARNING: The '{Constants.Command.PublishExtension}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");
            if (args.TargetExtensionReference is null)
            {
                throw new CommandLineException("The target extension was not specified.");
            }
            var reference = ValidateReference(args.TargetExtensionReference);
            var overwriteIfExists = args.Force;

            Trace.WriteLine($"Preparing to publish extension \"{reference.FullyQualifiedReference}\" (force={overwriteIfExists}, indexFile={args.IndexFile ?? "<none>"}, binariesSpecified={args.Binaries.Count}).");

            var binaries = SupportedArchitectures.All.Select(TryGetBinary).WhereNotNull().ToImmutableArray();
            var tarPayload = await GetTypesTarPayload(args, binaries, cancellationToken);

            var package = new ExtensionPackage(
                Types: tarPayload,
                LocalDeployEnabled: binaries.Any(),
                Binaries: binaries);

            await this.PublishExtensionAsync(reference, package, overwriteIfExists);
            return 0;
        }

        private async Task<BinaryData> GetTypesTarPayload(PublishExtensionArguments args, ImmutableArray<ExtensionBinary> binaries, CancellationToken cancellationToken)
        {
            if (args.IndexFile is { })
            {
                var indexUri = inputOutputArgumentsResolver.PathToUri(args.IndexFile);
                var indexFile = fileExplorer.GetFile(indexUri);

                Trace.WriteLine($"Packaging extension types from index file \"{indexUri.Path}\".");
                return await CreateTypesTar(indexFile);
            }

            if (!binaries.Any())
            {
                throw new CommandLineException($"The input file path was not specified.");
            }

            if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
            {
                throw new BicepException($"Failed to load type information: Unable to determine the current architecture and OS platform.");
            }

            if (args.Binaries.TryGetValue(architecture.Name) is not { } binaryPath ||
                inputOutputArgumentsResolver.PathToUri(binaryPath) is not { } binaryUri)
            {
                throw new BicepException($"Failed to load type information: Unable to find a binary for the current architecture ({architecture.Name}).");
            }

            Trace.WriteLine($"Extracting extension types from binary \"{binaryUri.Path}\" for architecture \"{architecture.Name}\".");
            var indexHandle = await GetTypesFromExtension(binaryUri, cancellationToken);
            return await CreateTypesTar(indexHandle);
        }

        private async Task PublishExtensionAsync(ArtifactReference target, ExtensionPackage package, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure extension doesn't exist
                if (!overwriteIfExists && await moduleDispatcher.CheckExtensionExists(target))
                {
                    throw new BicepException($"The extension \"{target.FullyQualifiedReference}\" already exists. Use --force to overwrite the existing extension.");
                }
                Trace.WriteLine($"Publishing extension package to \"{target.FullyQualifiedReference}\".");
                await moduleDispatcher.PublishExtension(target, package);
                Trace.WriteLine($"Successfully published extension package to \"{target.FullyQualifiedReference}\".");
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish extension \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private ArtifactReference ValidateReference(string targetReference)
        {
            IOUri dummyReferencingFileUri;

            if (!targetReference.StartsWith("br:") && !targetReference.StartsWith("ts:"))
            {
                // If targetReference is a fully qualified file, we need to create a dummy artifact referencing
                // file with the path, and change targetReference to be the file name so it is a relative reference,
                // because absoluate reference is not supported by Bicep.
                dummyReferencingFileUri = inputOutputArgumentsResolver.PathToUri(targetReference);
                targetReference = dummyReferencingFileUri.GetFileName();
            }
            else
            {
                dummyReferencingFileUri = IOUri.FromFilePath(Path.Join(environment.CurrentDirectory, "dummy"));
            }

            var dummyReferencingFile = sourceFileFactory.CreateBicepFile(dummyReferencingFileUri, "");

            if (!moduleDispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Extension, targetReference).IsSuccess(out var extensionReference, out var failureBuilder))
            {
                // TODO: We should probably clean up the dispatcher contract so this sort of thing isn't necessary (unless we change how target module is set in this command)
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (!moduleDispatcher.GetRegistryCapabilities(ArtifactType.Extension, extensionReference).HasFlag(RegistryCapabilities.Publish))
            {
                throw new BicepException($"The specified extension target \"{targetReference}\" is not supported.");
            }

            return extensionReference;
        }

        private static void ValidateExtension(BinaryData extension)
        {
            using var tempStream = extension.ToStream();

            var typeLoader = ArchivedTypeLoader.FromStream(tempStream);
            var azTypeLoader = new AzResourceTypeLoader(typeLoader);
            foreach (var typeReference in azTypeLoader.GetAvailableTypes())
            {
                azTypeLoader.LoadType(typeReference);
            }
        }

        private async Task<BinaryData> CreateTypesTar(IFileHandle indexHandle)
        {
            try
            {
                Trace.WriteLine($"Bundling extension types from index \"{indexHandle.Uri.Path}\".");
                var tarPayload = await TypesV1Archive.PackIntoBinaryData(indexHandle);
                ValidateExtension(tarPayload);
                Trace.WriteLine($"Successfully bundled extension types from index \"{indexHandle.Uri.Path}\".");

                return tarPayload;
            }
            catch (Exception exception)
            {
                throw new BicepException($"Extension package creation failed: {exception.Message}");
            }
        }

        private async Task<IFileHandle> GetTypesFromExtension(IOUri binaryUri, CancellationToken cancellationToken)
        {
            await using var extension = await localExtensionFactory.Start(binaryUri);

            Trace.WriteLine($"Requesting extension type definitions from binary \"{binaryUri.Path}\" via gRPC.");
            var typeFiles = await extension.GetTypeFiles(cancellationToken);
            Trace.WriteLine($"Received extension type index and {typeFiles.TypeFileContents.Count} additional type file(s) from binary \"{binaryUri.Path}\".");

            var fileExplorer = new InMemoryFileExplorer();

            var indexUri = IOUri.FromFilePath("/index.json");
            var indexHandle = fileExplorer.GetFile(indexUri);
            indexHandle.Write(typeFiles.IndexFileContent);

            foreach (var (path, content) in typeFiles.TypeFileContents)
            {
                var fileUri = IOUri.FromFilePath($"/{path}");
                fileExplorer.GetFile(fileUri).Write(content);
            }

            return indexHandle;
        }

        internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
        {
            var command = new System.CommandLine.Command(Constants.Command.PublishExtension, "[Experimental] Publishes a Bicep extension to a registry.");

            var indexFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.IndexFile)
            {
                Description = "The path to the index file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var targetOption = new System.CommandLine.Option<string?>(Option.Target)
            {
                Description = "The target extension reference.",
            };
            var forceOption = new System.CommandLine.Option<bool>(Option.Force)
            {
                Description = "Force publish even if the extension already exists.",
            };
            var binLinuxX64Option = new System.CommandLine.Option<string?>(Option.BinLinuxX64) { Description = "Path to the linux-x64 binary." };
            var binLinuxArm64Option = new System.CommandLine.Option<string?>(Option.BinLinuxArm64) { Description = "Path to the linux-arm64 binary." };
            var binOsxX64Option = new System.CommandLine.Option<string?>(Option.BinOsxX64) { Description = "Path to the osx-x64 binary." };
            var binOsxArm64Option = new System.CommandLine.Option<string?>(Option.BinOsxArm64) { Description = "Path to the osx-arm64 binary." };
            var binWinX64Option = new System.CommandLine.Option<string?>(Option.BinWinX64) { Description = "Path to the win-x64 binary." };
            var binWinArm64Option = new System.CommandLine.Option<string?>(Option.BinWinArm64) { Description = "Path to the win-arm64 binary." };

            command.Add(indexFileArgument);
            command.Add(targetOption);
            command.Add(forceOption);
            command.Add(binLinuxX64Option);
            command.Add(binLinuxArm64Option);
            command.Add(binOsxX64Option);
            command.Add(binOsxArm64Option);
            command.Add(binWinX64Option);
            command.Add(binWinArm64Option);
            command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, indexFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(async () =>
            {
                var binaries = new Dictionary<string, string>();
                if (result.GetValue(binLinuxX64Option) is { } p1) { binaries["linux-x64"] = p1; }
                if (result.GetValue(binLinuxArm64Option) is { } p2) { binaries["linux-arm64"] = p2; }
                if (result.GetValue(binOsxX64Option) is { } p3) { binaries["osx-x64"] = p3; }
                if (result.GetValue(binOsxArm64Option) is { } p4) { binaries["osx-arm64"] = p4; }
                if (result.GetValue(binWinX64Option) is { } p5) { binaries["win-x64"] = p5; }
                if (result.GetValue(binWinArm64Option) is { } p6) { binaries["win-arm64"] = p6; }

                var args = new PublishExtensionArguments(
                    result.GetValue(indexFileArgument),
                    result.GetValue(targetOption),
                    binaries,
                    result.GetValue(forceOption));

                return await context.GetCommand<PublishExtensionCommand>().RunAsync(args, ct);
            }));

            return command;
        }
    }
}
