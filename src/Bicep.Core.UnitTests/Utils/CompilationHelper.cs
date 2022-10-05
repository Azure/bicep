// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
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

        public static CompilationResult Compile(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");

            var (uriDictionary, entryUri) = CreateFileDictionary(files, "main.bicep");
            var fileResolver = new InMemoryFileResolver(uriDictionary);

            return Compile(services, fileResolver, uriDictionary.Keys, entryUri);
        }

        public static CompilationResult Compile(ServiceBuilder services, IFileResolver fileResolver, IEnumerable<Uri> sourceFiles, Uri entryUri)
        {
            services = services.WithFileResolver(fileResolver);
            var sourceFileDict = sourceFiles
                .Where(x => PathHelper.HasBicepExtension(x) || PathHelper.HasArmTemplateLikeExtension(x))
                .ToDictionary(x => x, x => fileResolver.TryRead(x, out var fileContents, out _) ? fileContents : throw new InvalidOperationException($"Failed to find file {x}"));

            return Compile(services.BuildCompilation(sourceFileDict, entryUri));
        }

        public static CompilationResult Compile(IAzResourceTypeLoader resourceTypeLoader, params (string fileName, string fileContents)[] files)
            => Compile(new ServiceBuilder().WithFeatureProvider(BicepTestConstants.Features).WithAzResourceTypeLoader(resourceTypeLoader), files);

        public static CompilationResult Compile(params (string fileName, string fileContents)[] files)
            => Compile(new ServiceBuilder().WithFeatureProvider(BicepTestConstants.Features), files);

        public static CompilationResult Compile(string fileContents)
            => Compile(("main.bicep", fileContents));

        public static CompilationResult Compile(ServiceBuilder services, string fileContents)
            => Compile(services, ("main.bicep", fileContents));

        public static ParamsCompilationResult CompileParams(params (string fileName, string fileContents)[] files)
        {
            var features = BicepTestConstants.Features;
            var configuration = BicepTestConstants.BuiltInConfiguration;
            var services = new ServiceBuilder().WithFeatureProvider(features).WithConfiguration(configuration);

            files.Select(x => x.fileName).Should().Contain("parameters.bicepparam");

            var (uriDictionary, entryUri) = CreateFileDictionary(files, "parameters.bicepparam");
            var fileResolver = new InMemoryFileResolver(uriDictionary);

            var sourceFiles = uriDictionary
                .Where(x => PathHelper.HasBicepparamsExension(x.Key) || PathHelper.HasBicepExtension(x.Key) || PathHelper.HasArmTemplateLikeExtension(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            var sourceFileGrouping = services.BuildSourceFileGrouping(sourceFiles, entryUri);

            var model = new ParamsSemanticModel(sourceFileGrouping, configuration, features, file => {
                var compilationGrouping = new SourceFileGrouping(fileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);

                return services.Build().BuildCompilation(sourceFileGrouping);
            });

            return CompileParams(model);
        }

        public static ParamsCompilationResult CompileParams(string fileContents)
            => CompileParams(("parameters.bicepparam", fileContents));

        private static (IReadOnlyDictionary<Uri, string> files, Uri entryFileUri) CreateFileDictionary(IEnumerable<(string fileName, string fileContents)> files, string entryFileName)
        {
            var uriDictionary = files.ToDictionary(
                x => InMemoryFileResolver.GetFileUri($"/path/to/{x.fileName}"),
                x => x.fileContents);
            var entryUri = InMemoryFileResolver.GetFileUri($"/path/to/{entryFileName}");
            return (uriDictionary, entryUri);
        }

        public static CompilationResult Compile(Compilation compilation)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

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

        private static ParamsCompilationResult CompileParams(ParamsSemanticModel semanticModel)
        {
            var emitter = new ParametersEmitter(semanticModel);

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
