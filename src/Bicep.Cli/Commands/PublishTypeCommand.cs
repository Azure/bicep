// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceCode;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Workspaces;
using FluentAssertions.Equivalency.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Bicep.Core.UnitTests.TypeSystem.Az;
using Bicep.Core.TypeSystem.Providers.Az;
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
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);
            var features = featureProviderFactory.GetFeatureProvider(PathHelper.FilePathToFileUrl(inputPath));
            var documentationUri = args.DocumentationUri;
            var typeReference = ValidateReference(args.TargetTypeReference, inputUri);
            var overwriteIfExists = args.Force;

            //attempt to validate types here, move to better location later

            var typePath = "C:\\bicep\\bicep\\src\\Bicep.Core.Samples\\Files\\baselines\\Publish_Types\\types.json";
            var indexJsonString = ProcessJsonFile(inputPath);

            AzResourceTypeLoader azTypeLoader = new(FileAzTypeLoader.FromFile(indexJsonString, typePath));
            AzResourceTypeProvider azProvider = new(azTypeLoader, "dummyVersion");

            IResourceTypeProviderFactory ResourceTypeProviderFactory = new ResourceTypeProviderFactory();
            //call factory function

            INamespaceProvider nsProvider = new DefaultNamespaceProvider(ResourceTypeProviderFactory);
            var azNamespaceType = nsProvider.TryGetNamespace(new(AzNamespaceType.BuiltInName, AzNamespaceType.Settings.ArmTemplateProviderVersion), ResourceScope.ResourceGroup, BicepTestConstants.Features, BicepSourceFileKind.BicepFile)!;
            var resourceTypeProvider = azNamespaceType.ResourceTypeProvider;

            var availableTypes = azTypeLoader.GetAvailableTypes();

            foreach (var type in availableTypes)
            {
                azTypeLoader.LoadType(type);
            }

            foreach (var availableType in availableTypes)
            {
                var reference = ResourceTypeReference.Parse(availableType.Name);

                var resourceType = resourceTypeProvider.TryGetDefinedType(azNamespaceType, reference, ResourceTypeGenerationFlags.None)!;

            }


            if (PathHelper.HasArmTemplateLikeExtension(inputUri))
            {
                // Publishing an ARM template file.
                using var armTemplateStream = this.fileSystem.FileStream.New(inputPath, FileMode.Open, FileAccess.Read);
                await this.PublishTypeAsync(typeReference, armTemplateStream, overwriteIfExists);

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

        /*private static NamespaceType GetAzNamespaceType(AzResourceTypeLoaderFactory azTypeLoaderFactory)
        {
            IFeatureProvider Features = new OverriddenFeatureProvider(new FeatureProvider(BicepTestConstants.BuiltInConfiguration), BicepTestConstants.FeatureOverrides);
            var nsProvider = new DefaultNamespaceProvider(azTypeLoaderFactory);

            return nsProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup, BicepTestConstants.Features, BicepSourceFileKind.BicepFile, null)!;
        }*/

        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        private static string ProcessJsonFile(string typesPath){

            FileStream typeStream = new(typesPath, FileMode.Open, FileAccess.Read);
            Dictionary<string, TypeLocation> resources = new();
            Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<TypeLocation>>> functions = new();


            var types = TypeSerializer.Deserialize(typeStream);

            for (int i = 0; i <= types.Length-1; i++)
            {
                if (types[i] is Azure.Bicep.Types.Concrete.ResourceType)
                {
                    var resourceType = (Azure.Bicep.Types.Concrete.ResourceType)types[i];

                    var typeLocation = new TypeLocation("types.json", i);
                    resources.Add(resourceType.Name, typeLocation);
                }
            }

            // Perform any processing on the data if needed

            TypeIndex typeIndex = new(resources, functions);

            string indexJson = JsonSerializer.Serialize(typeIndex);

            return indexJson;
        }
    }
}
