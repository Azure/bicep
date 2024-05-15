// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilationHelper
    {
        public interface ICompilationResult
        {
            IEnumerable<IDiagnostic> Diagnostics { get; }

            Compilation Compilation { get; }

            BicepSourceFile SourceFile { get; }
        }

        public record CompilationResult(
            JToken? Template,
            IEnumerable<IDiagnostic> Diagnostics,
            Compilation Compilation) : ICompilationResult
        {
            public BicepSourceFile SourceFile => Compilation.SourceFileGrouping.EntryPoint;

            public BicepFile BicepFile => (BicepFile)SourceFile;
        }

        public record ParamsCompilationResult(
            JToken? Parameters,
            IEnumerable<IDiagnostic> Diagnostics,
            Compilation Compilation) : ICompilationResult
        {

            public BicepSourceFile SourceFile => Compilation.SourceFileGrouping.EntryPoint;

            public BicepParamFile ParamsFile => (BicepParamFile)SourceFile;
        }

        public record CursorLookupResult(
            SyntaxBase Node,
            Symbol Symbol,
            TypeSymbol Type);

        public static Task<CompilationResult> RestoreAndCompile(ServiceBuilder services, string fileContents)
            => RestoreAndCompile(services, ("main.bicep", fileContents));

        public static async Task<CompilationResult> RestoreAndCompile(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");
            var filesToAppend = files.Select(file => ("/path/to", file.fileName, file.fileContents));

            var (uriDictionary, entryUri) = CreateFileDictionary(filesToAppend, "main.bicep");

            var compiler = services.Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(entryUri, CreateWorkspace(uriDictionary));

            return GetCompilationResult(compilation);
        }

        public static IWorkspace CreateWorkspace(IReadOnlyDictionary<Uri, string> uriDictionary)
        {
            var workspace = new Workspace();
            var sourceFiles = uriDictionary.Select(kvp => SourceFileFactory.CreateSourceFile(kvp.Key, kvp.Value));
            workspace.UpsertSourceFiles(sourceFiles);

            return workspace;
        }

        public static CompilationResult Compile(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            var a = services.Build().Construct<Core.Configuration.IConfigurationManager>();
            a.GetConfiguration(new Uri("http://hello"));

            files.Select(x => x.fileName).Should().Contain("main.bicep");
            var filesToAppend = files.Select(file => ("/path/to", file.fileName, file.fileContents));

            var (uriDictionary, entryUri) = CreateFileDictionary(filesToAppend, "main.bicep");
            var fileResolver = new InMemoryFileResolver(uriDictionary);

            return Compile(services, fileResolver, uriDictionary.Keys, entryUri);
        }

        public static CompilationResult Compile(ServiceBuilder services, IFileResolver fileResolver, IEnumerable<Uri> sourceFiles, Uri entryUri)
        {
            var compiler = services.WithFileResolver(fileResolver).Build().GetCompiler();
            var sourceFileDict = sourceFiles
                .Where(x => PathHelper.HasBicepExtension(x) || PathHelper.HasArmTemplateLikeExtension(x))
                .ToDictionary(x => x, x => fileResolver.TryRead(x).IsSuccess(out var fileContents) ? fileContents : throw new InvalidOperationException($"Failed to find file {x}"));

            var compilation = compiler.CreateCompilationWithoutRestore(entryUri);
            return GetCompilationResult(compilation);
        }

        public static CompilationResult Compile(IResourceTypeLoader resourceTypeLoader, params (string fileName, string fileContents)[] files)
            => Compile(new ServiceBuilder().WithFeatureOverrides(BicepTestConstants.FeatureOverrides).WithAzResourceTypeLoader(resourceTypeLoader), files);

        public static CompilationResult Compile(params (string fileName, string fileContents)[] files)
            => Compile(new ServiceBuilder().WithFeatureOverrides(BicepTestConstants.FeatureOverrides), files);

        public static CompilationResult Compile(string fileContents)
            => Compile(("main.bicep", fileContents));

        public static CompilationResult Compile(ServiceBuilder services, string fileContents)
            => Compile(services, ("main.bicep", fileContents));


        public static ParamsCompilationResult CompileParams(params (string fileName, string fileContents)[] files)
        {
            return CompileParams(new ServiceBuilder(), files);
        }

        public static ParamsCompilationResult CompileParams(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("parameters.bicepparam");

            var (uriDictionary, entryUri) = CreateFileDictionary(files.Select(file => ("/path/to", file.fileName, file.fileContents)).ToArray(), "parameters.bicepparam");

            var sourceFiles = uriDictionary
                .Where(x => PathHelper.HasBicepparamsExtension(x.Key) || PathHelper.HasBicepExtension(x.Key) || PathHelper.HasArmTemplateLikeExtension(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            var compilation = services
                .WithMockFileSystem(uriDictionary)
                .BuildCompilation(sourceFiles, entryUri);

            return CompileParams(compilation);
        }

        public static (IReadOnlyDictionary<Uri, string> files, Uri entryFileUri) CreateFileDictionary(IEnumerable<(string filePath, string fileName, string fileContents)> files, string entryFileName)
        {
            var filesArray = files.ToArray();
            var (entryFilePath, _, _) = filesArray.Where(x => x.fileName == entryFileName).First();
            var uriDictionary = filesArray.ToDictionary(
                x => InMemoryFileResolver.GetFileUri($"{x.filePath}/{x.fileName}"),
                x => x.fileContents);
            var entryUri = InMemoryFileResolver.GetFileUri($"{entryFilePath}/{entryFileName}");
            return (uriDictionary, entryUri);
        }

        public static CompilationResult GetCompilationResult(Compilation compilation)
        {
            SemanticModel semanticModel = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(semanticModel);

            var diagnostics = semanticModel.GetAllDiagnostics();

            JToken? template = null;
            if (!semanticModel.HasErrors())
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

        private static ParamsCompilationResult CompileParams(Compilation compilation)
        {
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var emitter = new ParametersEmitter(semanticModel);

            var diagnostics = semanticModel.GetAllDiagnostics();

            JToken? parameters = null;
            if (!semanticModel.HasErrors())
            {
                using var stream = new MemoryStream();
                var emitResult = emitter.Emit(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    stream.Position = 0;
                    var jsonOutput = new StreamReader(stream).ReadToEnd();

                    parameters = JToken.Parse(jsonOutput);
                }
            }

            return new(parameters, diagnostics, compilation);
        }
    }
}
