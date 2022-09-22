// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public class Options
        {
            public IAzResourceTypeLoader AzResourceTypeLoader { get; init; }
            public IFeatureProvider Features { get; init; }
            public EmitterSettings EmitterSettings { get; init; }
            public INamespaceProvider NamespaceProvider { get; init; }
            public RootConfiguration Configuration { get; init; }
            public ApiVersionProvider ApiVersionProvider { get; init; }
            public LinterAnalyzer LinterAnalyzer { get; init; }

            public Options(
                        IAzResourceTypeLoader? AzResourceTypeLoader = null,
                        IFeatureProvider? Features = null,
                        EmitterSettings? EmitterSettings = null,
                        INamespaceProvider? NamespaceProvider = null,
                        RootConfiguration? Configuration = null,
                        ApiVersionProvider? ApiVersionProvider = null,
                        LinterAnalyzer? LinterAnalyzer = null)
            {
                this.AzResourceTypeLoader = AzResourceTypeLoader ?? BicepTestConstants.AzResourceTypeLoader;
                this.Features = Features ?? BicepTestConstants.Features;
                this.NamespaceProvider = NamespaceProvider ?? new DefaultNamespaceProvider(this.AzResourceTypeLoader, this.Features);
                this.EmitterSettings = EmitterSettings ?? new EmitterSettings(this.Features);
                this.Configuration = Configuration ?? BicepTestConstants.BuiltInConfiguration;
                this.ApiVersionProvider = ApiVersionProvider ?? BicepTestConstants.ApiVersionProvider;
                this.LinterAnalyzer = LinterAnalyzer ?? new LinterAnalyzer(this.Configuration);
            }
        }

        public static Compilation CreateCompilation(SourceFileGrouping sourceFileGrouping, Options? context = null)
        {
            context ??= new();
            return new Compilation(
                context.Features,
                context.NamespaceProvider,
                sourceFileGrouping,
                context.Configuration,
                context.ApiVersionProvider,
                context.LinterAnalyzer);
        }

        public static CompilationResult Compile(Options context, string fileContents)
            => Compile(fileContents, context);

        public static CompilationResult Compile(string fileContents, Options? context = null)
            => Compile(context ?? new(), ("main.bicep", fileContents));

        public static CompilationResult Compile(SourceFileGrouping sourceFileGrouping, Options? context = null)
        {
            context ??= new();
            return Compile(CreateCompilation(sourceFileGrouping, context), context);
        }

        public static CompilationResult Compile(params (string fileName, string fileContents)[] files)
            => Compile(new Options(), files);

        public static CompilationResult Compile(Options context, params (string fileName, string fileContents)[] files)
        {
            var bicepFiles = files.Where(x => x.fileName.EndsWith(".bicep", StringComparison.InvariantCultureIgnoreCase));
            bicepFiles.Select(x => x.fileName).Should().Contain("main.bicep");

            var systemFiles = files.Where(x => !x.fileName.EndsWith(".bicep", StringComparison.InvariantCultureIgnoreCase));

            var (uriDictionary, entryUri) = CreateFileDictionary(bicepFiles, "main.bicep");
            var fileResolver = new InMemoryFileResolver(CreateFileDictionary(systemFiles, "main.bicep").files);

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(uriDictionary, entryUri, fileResolver, context.Configuration, context.Features);

            return Compile(sourceFileGrouping, context);
        }

        public static CompilationResult Compile(Uri entryUri, IReadOnlyDictionary<Uri, string> bicepFiles, Options? context = null)
        {
            context ??= new();
            var fileResolver = new InMemoryFileResolver(bicepFiles);

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(bicepFiles, entryUri, fileResolver, context.Configuration, context.Features);

            return Compile(sourceFileGrouping, context);
        }

        public static CompilationResult Compile(IAzResourceTypeLoader resourceTypeLoader, params (string fileName, string fileContents)[] files)
            => Compile(new Options(AzResourceTypeLoader: resourceTypeLoader), files);

        public static ParamsCompilationResult CompileParams(Uri entryUri, IReadOnlyDictionary<Uri, string> bicepFiles, Options? context = null)
        {
            context ??= new();
            var fileResolver = new InMemoryFileResolver(bicepFiles);

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(bicepFiles, entryUri, fileResolver, context.Configuration, context.Features);

            var model = new ParamsSemanticModel(sourceFileGrouping, file =>
            {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);

                return CreateCompilation(compilationGrouping, context);
            });

            return CompileParams(model, context);
        }

        public static ParamsCompilationResult CompileParams(Options context, params (string fileName, string fileContents)[] files)
        {
            var paramsFiles = files.Where(x => x.fileName.EndsWith(".bicepparam", StringComparison.InvariantCultureIgnoreCase));
            paramsFiles.Select(x => x.fileName).Should().Contain("parameters.bicepparam");

            var bicepFiles = files.Where(x => x.fileName.EndsWith(".bicep", StringComparison.InvariantCultureIgnoreCase));

            var systemFiles = files.Where(x => !x.fileName.EndsWith(".bicep", StringComparison.InvariantCultureIgnoreCase) && !x.fileName.EndsWith(".bicepparam", StringComparison.InvariantCultureIgnoreCase));

            var (uriDictionary, entryUri) = CreateFileDictionary(bicepFiles.Concat(paramsFiles), "parameters.bicepparam");
            var fileResolver = new InMemoryFileResolver(CreateFileDictionary(systemFiles, "parameters.bicepparam").files);

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(uriDictionary, entryUri, fileResolver, context.Configuration, context.Features);

            var model = new ParamsSemanticModel(sourceFileGrouping, file =>
            {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);

                return CreateCompilation(compilationGrouping, context);
            });

            return CompileParams(model, context);
        }

        public static ParamsCompilationResult CompileParams(params (string fileName, string fileContents)[] files)
            => CompileParams(new Options(), files);

        public static ParamsCompilationResult CompileParams(string fileContents)
            => CompileParams(("parameters.bicepparam", fileContents));

        public static ParamsCompilationResult CompileParams(Options context, string fileContents)
            => CompileParams(context, ("parameters.bicepparam", fileContents));

        private static (IReadOnlyDictionary<Uri, string> files, Uri entryFileUri) CreateFileDictionary(IEnumerable<(string fileName, string fileContents)> files, string entryFileName)
        {
            var uriDictionary = files.ToDictionary(
                x => new Uri($"file:///path/to/{x.fileName}"),
                x => x.fileContents);
            var entryUri = new Uri($"file:///path/to/{entryFileName}");
            return (uriDictionary, entryUri);
        }

        private static CompilationResult Compile(Compilation compilation, Options context)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), context.EmitterSettings);

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

        private static ParamsCompilationResult CompileParams(ParamsSemanticModel semanticModel, Options context)
        {
            var emitter = new ParametersEmitter(semanticModel, context.EmitterSettings);

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
