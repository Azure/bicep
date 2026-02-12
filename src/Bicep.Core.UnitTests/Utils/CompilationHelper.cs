// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Enumeration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.TextFixtures.IO;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilationHelper
    {
        public record InputFile(
            string FileName,
            string Contents);

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

        public static async Task<CompilationResult> RestoreAndCompile(params (string fileName, BinaryData fileContents)[] files)
        {
            return await RestoreAndCompile(new ServiceBuilder(), files);
        }

        public static async Task<CompilationResult> RestoreAndCompile(ServiceBuilder services, params (string fileName, BinaryData fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");

            var (uriDictionary, entryUri) = CreateFileDictionary(files.Select(file => ("/path/to", file.fileName, file.fileContents)).ToArray(), "main.bicep");

            return await RestoreAndCompile(services.WithMockFileSystem(uriDictionary), ImmutableDictionary<Uri, string>.Empty, entryUri);
        }

        public static Task<CompilationResult> RestoreAndCompile(ServiceBuilder services, string fileContents)
            => RestoreAndCompile(services, ("main.bicep", fileContents));

        public static async Task<CompilationResult> RestoreAndCompile(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");
            var filesToAppend = files.Select(file => ("/path/to", file.fileName, file.fileContents));

            var (uriDictionary, entryUri) = CreateFileDictionary(filesToAppend, "main.bicep");

            return await RestoreAndCompile(services, uriDictionary, entryUri);
        }

        public static async Task<ParamsCompilationResult> RestoreAndCompileParams(ServiceBuilder services, IReadOnlyDictionary<Uri, BinaryData> uriDictionary, Uri entryUri)
        {
            var compiler = services.WithMockFileSystem(uriDictionary).Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(entryUri.ToIOUri());

            return CompileParams(compilation);
        }

        public static async Task<CompilationResult> RestoreAndCompile(ServiceBuilder services, IReadOnlyDictionary<Uri, string> uriDictionary, Uri entryUri)
        {
            var compiler = services.Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(entryUri.ToIOUri(), CreateWorkspace(compiler.SourceFileFactory, uriDictionary));

            return GetCompilationResult(compilation);
        }

        public static Task<ParamsCompilationResult> RestoreAndCompileParams(ServiceBuilder services, string mainBicepFileContents, string bicepParamsFileContent)
            => RestoreAndCompileParams(services, ("main.bicep", mainBicepFileContents), ("parameters.bicepparam", bicepParamsFileContent));

        public static async Task<ParamsCompilationResult> RestoreAndCompileParams(ServiceBuilder services, params (string fileName, BinaryData fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("parameters.bicepparam");

            var (uriDictionary, entryUri) = CreateFileDictionary(files.Select(file => ("/path/to", file.fileName, file.fileContents)).ToArray(), "parameters.bicepparam");

            return await RestoreAndCompileParams(services, uriDictionary, entryUri);
        }

        public static async Task<ParamsCompilationResult> RestoreAndCompileParams(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("parameters.bicepparam");
            var filesToAppend = files.Select(file => ("/path/to", file.fileName, file.fileContents));

            var (uriDictionary, entryUri) = CreateFileDictionary(filesToAppend, "parameters.bicepparam");

            return await RestoreAndCompileParams(services, uriDictionary, entryUri);
        }

        public static async Task<ParamsCompilationResult> RestoreAndCompileParams(ServiceBuilder services, IReadOnlyDictionary<Uri, string> uriDictionary, Uri entryUri)
        {
            var compiler = services.WithMockFileSystem(uriDictionary).Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(entryUri.ToIOUri());

            return CompileParams(compilation);
        }

        public static IActiveSourceFileSet CreateWorkspace(ISourceFileFactory sourceFileFactory, IReadOnlyDictionary<Uri, string> uriDictionary)
        {
            var workspace = new ActiveSourceFileSet();
            var sourceFiles = uriDictionary.Select(kvp => sourceFileFactory.CreateSourceFile(kvp.Key.ToIOUri(), kvp.Value));
            workspace.UpsertSourceFiles(sourceFiles);

            return workspace;
        }

        public static CompilationResult Compile(ServiceBuilder services, params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");

            var fileSet = new MockFileSystemTestFileSet();
            foreach (var (fileName, fileContents) in files)
            {
                fileSet.AddFile(fileName, fileContents);
            }

            return Compile(services, fileSet, fileSet.GetUri("main.bicep"));
        }

        public static CompilationResult Compile(ServiceBuilder services, MockFileSystemTestFileSet fileSet, IOUri entryUri)
        {
            var compiler = services.WithFileExplorer(fileSet.FileExplorer).WithFileSystem(fileSet.FileSystem).Build().GetCompiler();
            var compilation = compiler.CreateCompilationWithoutRestore(entryUri);

            return GetCompilationResult(compilation);
        }

        public static CompilationResult Compile(IResourceTypeLoader resourceTypeLoader, params (string fileName, string fileContents)[] files)
            => Compile(new ServiceBuilder().WithFeatureOverrides(BicepTestConstants.FeatureOverrides).WithAzResourceTypeLoader(resourceTypeLoader), files);

        public static CompilationResult Compile(params InputFile[] files)
            => Compile(files.Select(x => (x.FileName, x.Contents)).ToArray());

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

        public static (IReadOnlyDictionary<Uri, TContents> files, Uri entryFileUri) CreateFileDictionary<TContents>(IEnumerable<(string filePath, string fileName, TContents fileContents)> files, string entryFileName)
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

        public static ParamsCompilationResult CompileParams(Compilation compilation)
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
