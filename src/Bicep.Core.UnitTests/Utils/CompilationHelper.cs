// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
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

        public record CompilationHelperContext(IResourceTypeProvider ResourceTypeProvider, IFeatureProvider Features);

        public static CompilationResult Compile(CompilationHelperContext context, params (string fileName, string fileContents)[] files)
        {
            var bicepFiles = files.Where(x => x.fileName.EndsWith(".bicep", StringComparison.InvariantCultureIgnoreCase));
            bicepFiles.Select(x => x.fileName).Should().Contain("main.bicep");

            var systemFiles = files.Where(x => !x.fileName.EndsWith(".bicep", StringComparison.InvariantCultureIgnoreCase));

            var (uriDictionary, entryUri) = CreateFileDictionary(bicepFiles);
            var fileResolver = new InMemoryFileResolver(CreateFileDictionary(systemFiles).files);

            var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(uriDictionary, entryUri, fileResolver, context.Features);

            return Compile(new Compilation(context.ResourceTypeProvider, sourceFileGrouping));
        }

        public static CompilationResult Compile(IResourceTypeProvider resourceTypeProvider, params (string fileName, string fileContents)[] files)
            => Compile(new CompilationHelperContext(resourceTypeProvider, BicepTestConstants.Features), files);

        public static CompilationResult Compile(params (string fileName, string fileContents)[] files)
            => Compile(new CompilationHelperContext(AzResourceTypeProvider.CreateWithAzTypes(), BicepTestConstants.Features), files);

        public static CompilationResult Compile(string fileContents)
            => Compile(("main.bicep", fileContents));

        public static CompilationResult Compile(CompilationHelperContext context, string fileContents)
            => Compile(context, ("main.bicep", fileContents));

        private static (IReadOnlyDictionary<Uri, string> files, Uri entryFileUri) CreateFileDictionary(IEnumerable<(string fileName, string fileContents)> files)
        {
            var uriDictionary = files.ToDictionary(
                x => new Uri($"file:///path/to/{x.fileName}"),
                x => x.fileContents);
            var entryUri = new Uri($"file:///path/to/main.bicep");
            return (uriDictionary, entryUri);
        }

        private static CompilationResult Compile(Compilation compilation)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), EmitterSettingsHelper.DefaultTestSettings);

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
    }
}
