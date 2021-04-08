// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using System;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilationHelper
    {
        public record CompilationResult(
            JToken? Template,
            IEnumerable<Diagnostic> Diagnostics,
            Compilation Compilation)
        {
            public SyntaxTree SyntaxTree => Compilation.SyntaxTreeGrouping.EntryPoint;
        }

        public static CompilationResult Compile(IResourceTypeProvider resourceTypeProvider, params (string fileName, string fileContents)[] files)
        {
            var (uriDictionary, entryUri) = CreateFileDictionary(files);
            
            var syntaxTreeGrouping = SyntaxTreeGroupingFactory.CreateForFiles(uriDictionary, entryUri);

            return Compile(new Compilation(resourceTypeProvider, syntaxTreeGrouping));
        }

        public static CompilationResult Compile(params (string fileName, string fileContents)[] files)
            => Compile(new AzResourceTypeProvider(), files);

        public static CompilationResult Compile(string fileContents)
            => Compile(("main.bicep", fileContents));

        private static (IReadOnlyDictionary<Uri, string> files, Uri entryFileUri) CreateFileDictionary(params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");

            var uriDictionary = files.ToDictionary(
                x => new Uri($"file:///path/to/{x.fileName}"),
                x => x.fileContents);
            var entryUri = new Uri($"file:///path/to/main.bicep");

            return (uriDictionary, entryUri);
        }

        private static CompilationResult Compile(Compilation compilation)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.DevAssemblyFileVersion);

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