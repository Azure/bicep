// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilationHelper
    {
        public record CompilationResult(
            JToken? Template,
            IEnumerable<IDiagnostic> Diagnostics,
            Compilation Compilation)
        {
            public BicepFile BicepFile => Compilation.SourceFileGrouping.EntryPoint;
        }

        public record ParamsCompilationResult(
            JToken? Parameters,
            IEnumerable<IDiagnostic> Diagnostics,
            ParamsSemanticModel SemanticModel)
        {
            public BicepParamFile ParamsFile => SemanticModel.BicepParamFile;
        }

        public record CursorLookupResult(
            SyntaxBase Node,
            Symbol Symbol,
            TypeSymbol Type);

        public record CompilationHelperContext(
            IAzResourceTypeLoader? AzResourceTypeLoader = null,
            IFeatureProvider? Features = null,
            EmitterSettings? EmitterSettings = null,
            INamespaceProvider? NamespaceProvider = null,
            RootConfiguration? Configuration = null,
            ApiVersionProvider? ApiVersionProvider = null)
        {
            // TODO: can we use IoC here instead of DIY-ing it?

            public IAzResourceTypeLoader GetAzResourceTypeLoader()
                => AzResourceTypeLoader ?? BicepTestConstants.AzResourceTypeLoader;

            public INamespaceProvider GetNamespaceProvider()
                => NamespaceProvider ?? new DefaultNamespaceProvider(GetAzResourceTypeLoader());

            public IFeatureProvider GetFeatures()
                => Features ?? BicepTestConstants.Features;

            public EmitterSettings GetEmitterSettings()
                => EmitterSettings ?? new EmitterSettings(GetFeatures());

            public RootConfiguration GetConfiguration()
                => Configuration ?? BicepTestConstants.BuiltInConfiguration;

            public ApiVersionProvider GetApiVersionProvider()
                => ApiVersionProvider ?? BicepTestConstants.ApiVersionProvider;
        }

        public static CompilationResult Compile(CompilationHelperContext context, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");

            var (uriDictionary, entryUri) = CreateFileDictionary(files, "main.bicep");
            var fileResolver = new InMemoryFileResolver(uriDictionary);

            return Compile(context, fileResolver, uriDictionary.Keys, entryUri);
        }

        public static CompilationResult Compile(CompilationHelperContext context, IFileResolver fileResolver, IEnumerable<Uri> sourceFiles, Uri entryUri)
        {
            var configuration = context.GetConfiguration();
            var apiVersionProvider = context.GetApiVersionProvider();

            var sourceFileDict = sourceFiles
                .Where(x => PathHelper.HasBicepExtension(x) || PathHelper.HasArmTemplateLikeExtension(x))
                .ToDictionary(x => x, x => fileResolver.TryRead(x, out var fileContents, out _) ? fileContents : throw new InvalidOperationException($"Failed to find file {x}"));

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(sourceFileDict, entryUri, fileResolver, configuration, context.GetFeatures());

            return Compile(context, new Compilation(context.Features ?? BicepTestConstants.Features, context.GetNamespaceProvider(), sourceFileGrouping, IConfigurationManager.WithStaticConfiguration(configuration), apiVersionProvider, BicepTestConstants.LinterAnalyzer));
        }

        public static CompilationResult Compile(IAzResourceTypeLoader resourceTypeLoader, params (string fileName, string fileContents)[] files)
            => Compile(new CompilationHelperContext(AzResourceTypeLoader: resourceTypeLoader), files);

        public static CompilationResult Compile(params (string fileName, string fileContents)[] files)
            => Compile(new CompilationHelperContext(), files);

        public static CompilationResult Compile(string fileContents)
            => Compile(("main.bicep", fileContents));

        public static CompilationResult Compile(CompilationHelperContext context, string fileContents)
            => Compile(context, ("main.bicep", fileContents));

        public static ParamsCompilationResult CompileParams(CompilationHelperContext context, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("parameters.bicepparam");

            var (uriDictionary, entryUri) = CreateFileDictionary(files, "parameters.bicepparam");
            var fileResolver = new InMemoryFileResolver(uriDictionary);

            var configuration = context.GetConfiguration();
            var apiVersionProvider = context.GetApiVersionProvider();

            var sourceFiles = uriDictionary
                .Where(x => PathHelper.HasBicepparamsExension(x.Key) || PathHelper.HasBicepExtension(x.Key) || PathHelper.HasArmTemplateLikeExtension(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(sourceFiles, entryUri, fileResolver, configuration, context.GetFeatures());

            var model = new ParamsSemanticModel(sourceFileGrouping, configuration, context.GetFeatures(), file => {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);

                return new Compilation(context.GetFeatures(), context.GetNamespaceProvider(), compilationGrouping, IConfigurationManager.WithStaticConfiguration(configuration), apiVersionProvider, new LinterAnalyzer());
            });

            return CompileParams(context, model);
        }

        public static ParamsCompilationResult CompileParams(params (string fileName, string fileContents)[] files)
            => CompileParams(new CompilationHelperContext(), files);

        public static ParamsCompilationResult CompileParams(string fileContents)
            => CompileParams(("parameters.bicepparam", fileContents));

        public static ParamsCompilationResult CompileParams(CompilationHelperContext context, string fileContents)
            => CompileParams(context, ("parameters.bicepparam", fileContents));

        private static (IReadOnlyDictionary<Uri, string> files, Uri entryFileUri) CreateFileDictionary(IEnumerable<(string fileName, string fileContents)> files, string entryFileName)
        {
            var uriDictionary = files.ToDictionary(
                x => InMemoryFileResolver.GetFileUri($"/path/to/{x.fileName}"),
                x => x.fileContents);
            var entryUri = InMemoryFileResolver.GetFileUri($"/path/to/{entryFileName}");
            return (uriDictionary, entryUri);
        }

        private static CompilationResult Compile(CompilationHelperContext context, Compilation compilation)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), context.GetEmitterSettings());

            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            JToken? template = null;
            if (!compilation.GetEntrypointSemanticModel().HasErrors())
            {
                using var stream = new MemoryStream();
                var emitResult = emitter.Emit(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    stream.Position = 0;
                    var jsonOutput = new StreamReader(stream).ReadToEnd();

                    template = JToken.Parse(jsonOutput);
                }
            }

            return new(template, diagnostics, compilation);
        }

        private static ParamsCompilationResult CompileParams(CompilationHelperContext context, ParamsSemanticModel semanticModel)
        {
            var emitter = new ParametersEmitter(semanticModel, context.GetEmitterSettings());

            var diagnostics = semanticModel.GetAllDiagnostics();

            JToken? template = null;
            if (!semanticModel.HasErrors())
            {
                using var stream = new MemoryStream();
                var emitResult = emitter.EmitParamsFile(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    stream.Position = 0;
                    var jsonOutput = new StreamReader(stream).ReadToEnd();

                    template = JToken.Parse(jsonOutput);
                }
            }

            return new(template, diagnostics, semanticModel);
        }
    }
}
