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
using FluentAssertions.Execution;
using FluentAssertions;
using System;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilationHelper
    {
        public static Compilation CreateCompilation(IResourceTypeProvider resourceTypeProvider, params (string fileName, string fileContents)[] files)
        {
            var (uriDictionary, entryUri) = CreateFileDictionary(files);

            return CreateCompilation(resourceTypeProvider, uriDictionary, entryUri);
        }

        public static Compilation CreateCompilation(params (string fileName, string fileContents)[] files)
            => CreateCompilation(new AzResourceTypeProvider(), files);

        public static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(params (string fileName, string fileContents)[] files)
        {
            var (uriDictionary, entryUri) = CreateFileDictionary(files);

            return Compile(CreateCompilation(new AzResourceTypeProvider(), uriDictionary, entryUri));
        }

        public static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(string fileContents)
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

        private static Compilation CreateCompilation(IResourceTypeProvider resourceTypeProvider, IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var syntaxTreeGrouping = SyntaxTreeGroupingFactory.CreateForFiles(files, entryFileUri);

            return new Compilation(resourceTypeProvider, syntaxTreeGrouping);
        }

        private static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(Compilation compilation)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            string? jsonOutput = null;
            if (!compilation.GetEntrypointSemanticModel().HasErrors())
            {
                using var stream = new MemoryStream();
                var emitResult = emitter.Emit(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    stream.Position = 0;
                    jsonOutput = new StreamReader(stream).ReadToEnd();
                }
            }

            return (jsonOutput, diagnostics);
        }

        public static void AssertFailureWithDiagnostics(string fileContents,  IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            AssertFailureWithDiagnostics(new Dictionary<Uri, string> { [entryFileUri] = fileContents }, entryFileUri, expectedDiagnostics);
        }

        public static void AssertFailureWithDiagnostics(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri, IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics)
        {
            var (jsonOutput, diagnostics) = Compile(CreateCompilation(new AzResourceTypeProvider(), files, entryFileUri));
            using (new AssertionScope())
            {
                jsonOutput.Should().BeNull();
                diagnostics.Should().HaveDiagnostics(expectedDiagnostics);
            }
        }

        public static string AssertSuccessWithTemplateOutput(string fileContents)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            return AssertSuccessWithTemplateOutput(new Dictionary<Uri, string> { [entryFileUri] = fileContents }, entryFileUri);
        }

        public static string AssertSuccessWithTemplateOutput(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var (jsonOutput, diagnostics) = Compile(CreateCompilation(new AzResourceTypeProvider(), files, entryFileUri));
            using (new AssertionScope())
            {
                jsonOutput.Should().NotBeNull();
                diagnostics.Should().BeEmpty();
            }

            return jsonOutput!;
        }
    }
}