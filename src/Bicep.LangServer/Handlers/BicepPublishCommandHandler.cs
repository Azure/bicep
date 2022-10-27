// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.Helpers;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Bicep.Core.Modules;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepPublishCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly IApiVersionProviderFactory apiVersionProviderFactory;
        private readonly IBicepAnalyzer bicepAnalyzer;

        public BicepPublishCommandHandler(ICompilationManager compilationManager, ISerializer serializer, IFeatureProviderFactory featureProviderFactory, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IApiVersionProviderFactory apiVersionProviderFactory, IConfigurationManager configurationManager, IBicepAnalyzer bicepAnalyzer)
            : base(LangServerConstants.PublishCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.featureProviderFactory = featureProviderFactory;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.apiVersionProviderFactory = apiVersionProviderFactory;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public async override Task<string> Handle(string bicepFilePath, string targetModuleReference, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUri();
            var moduleReference = ValidateReference(targetModuleReference, uri);

            var compilation = GetCompilation(uri);

            if (compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Any(x => x.Level == DiagnosticLevel.Error))
            {
                return "Can't publish";
            }

            var stream = new MemoryStream();
            ToStream(compilation, stream);

            stream.Position = 0;

            await this.PublishModuleAsync(moduleReference, stream);

            return string.Empty;
        }

        private ModuleReference ValidateReference(string targetModuleReference, Uri targetModuleUri)
        {
            if (!this.moduleDispatcher.TryGetModuleReference(targetModuleReference, targetModuleUri, out var moduleReference, out var failureBuilder))
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

        private async Task PublishModuleAsync(ModuleReference target, Stream stream)
        {
            try
            {
                await this.moduleDispatcher.PublishModule(target, stream);
            }
            catch (ExternalModuleException exception)
            {
                throw new BicepException($"Unable to publish module \"{target.FullyQualifiedReference}\": {exception.Message}");
            }
        }

        public EmitResult ToStream(Compilation compilation, Stream stream)
        {
            var fileKind = compilation.SourceFileGrouping.EntryPoint.FileKind;
            switch (fileKind)
            {
                case BicepSourceFileKind.BicepFile:
                    return new TemplateEmitter(compilation.GetEntrypointSemanticModel()).Emit(stream);

                case BicepSourceFileKind.ParamsFile:
                    return new ParametersEmitter(compilation.GetEntrypointSemanticModel()).EmitParamsFile(stream);

                default:
                    throw new NotImplementedException($"Unexpected file kind '{fileKind}'");
            }
        }

        private Compilation GetCompilation(Uri uri)
        {
            CompilationContext? context = compilationManager.GetCompilation(uri);

            if (context is null)
            {
                SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, new Workspace(), uri);
                return new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
            }
            else
            {
                return context.Compilation;
            }
        }
    }
}
