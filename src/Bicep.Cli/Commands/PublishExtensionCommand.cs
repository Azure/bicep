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

namespace Bicep.Cli.Commands
{
    public class PublishExtensionCommand(
        IArtifactReferenceResolver referenceResolver,
        OciArtifactRegistry ociRegistry,
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

            logger.LogWarning($"WARNING: The '{CommandNames.PublishExtension}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");
            if (args.TargetExtensionReference is null)
            {
                throw new CommandLineException("The target extension was not specified.");
            }
            OciArtifactReference? reference = null;
            IFileHandle? fileTarget = null;
            if (args.TargetExtensionReference.StartsWith("br:") || args.TargetExtensionReference.StartsWith("ts:"))
            {
                reference = ValidateReference(args.TargetExtensionReference, GetCurrentDirectoryReferencingFileUri());
            }
            else
            {
                fileTarget = fileExplorer.GetFile(inputOutputArgumentsResolver.PathToUri(args.TargetExtensionReference));
                var fileName = fileTarget.Uri.GetFileName();
                if (ArtifactAddress.Parse(fileName) is not ArtifactAddress.Path)
                {
                    // Preserve diagnostics for malformed paths whose file name is parsed as a scheme.
                    reference = ValidateReference(fileName, fileTarget.Uri);
                    fileTarget = null;
                }
                else if (!RelativePath.TryCreate(fileName).IsSuccess(out _, out var failureBuilder))
                {
                    throw new BicepException(failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message);
                }
            }
            var overwriteIfExists = args.Force;

            Trace.WriteLine($"Preparing to publish extension \"{reference?.FullyQualifiedReference ?? fileTarget?.Uri.GetFileName()}\" (force={overwriteIfExists}, indexFile={args.IndexFile ?? "<none>"}, binariesSpecified={args.Binaries.Count}).");

            var binaries = SupportedArchitectures.All.Select(TryGetBinary).WhereNotNull().ToImmutableArray();
            var tarPayload = await GetTypesTarPayload(args, binaries, cancellationToken);

            var package = new ExtensionPackage(
                Types: tarPayload,
                LocalDeployEnabled: binaries.Any(),
                Binaries: binaries);

            if (reference is not null)
            {
                await this.PublishExtensionAsync(reference, package, overwriteIfExists);
            }
            else if (fileTarget is not null)
            {
                fileTarget.Write(await ExtensionV1Archive.Build(package));
            }
            else
            {
                throw new UnreachableException("A publish target must be a file or an OCI reference.");
            }
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

        private async Task PublishExtensionAsync(OciArtifactReference target, ExtensionPackage package, bool overwriteIfExists)
        {
            try
            {
                // If we don't want to overwrite, ensure extension doesn't exist
                if (!overwriteIfExists && await ociRegistry.CheckArtifactExists(target))
                {
                    throw new BicepException($"The extension \"{target.FullyQualifiedReference}\" already exists. Use --force to overwrite the existing extension.");
                }
                Trace.WriteLine($"Publishing extension package to \"{target.FullyQualifiedReference}\".");
                await ociRegistry.PublishExtension(target, package);
                Trace.WriteLine($"Successfully published extension package to \"{target.FullyQualifiedReference}\".");
            }
            catch (ExternalArtifactException exception)
            {
                throw new BicepException($"Unable to publish extension \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        private IOUri GetCurrentDirectoryReferencingFileUri()
        {
            // Configuration discovery starts in a file's containing directory. This file is never read.
            return IOUri.FromFilePath(Path.Join(environment.CurrentDirectory, "dummy"));
        }

        private OciArtifactReference ValidateReference(string targetReference, IOUri referencingFileUri)
        {
            if (!referenceResolver.TryResolve(referencingFileUri, ArtifactType.Extension, targetReference).IsSuccess(out var extensionReference, out var failureBuilder))
            {
                var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;

                throw new BicepException(message);
            }

            if (extensionReference is not OciArtifactReference { Tag: not null } ociReference)
            {
                throw new BicepException($"The specified extension target \"{targetReference}\" is not supported.");
            }

            return ociReference;
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

        internal static Command CreateCommand(CommandLineBuilderContext context)
        {
            var indexFileArgument = new Argument<string?>(ArgumentNames.IndexFile)
            {
                Description = "The path to the index file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var targetOption = new Option<string?>(OptionNames.Target)
            {
                Description = "The target extension reference.",
            };
            var forceOption = CommonOptions.Force("Force publish even if the extension already exists.");
            var binLinuxX64Option = new Option<string?>(OptionNames.BinLinuxX64) { Description = "Path to the linux-x64 binary." };
            var binLinuxArm64Option = new Option<string?>(OptionNames.BinLinuxArm64) { Description = "Path to the linux-arm64 binary." };
            var binOsxX64Option = new Option<string?>(OptionNames.BinOsxX64) { Description = "Path to the osx-x64 binary." };
            var binOsxArm64Option = new Option<string?>(OptionNames.BinOsxArm64) { Description = "Path to the osx-arm64 binary." };
            var binWinX64Option = new Option<string?>(OptionNames.BinWinX64) { Description = "Path to the win-x64 binary." };
            var binWinArm64Option = new Option<string?>(OptionNames.BinWinArm64) { Description = "Path to the win-arm64 binary." };
            var configOption = CommonOptions.Config();

            var command = new Command(CommandNames.PublishExtension, "[Experimental] Publishes a Bicep extension to a registry.")
            {
                indexFileArgument,
                targetOption,
                forceOption,
                binLinuxX64Option,
                binLinuxArm64Option,
                binOsxX64Option,
                binOsxArm64Option,
                binWinX64Option,
                binWinArm64Option,
                configOption,
            };
            command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, indexFileArgument));

            command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
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
