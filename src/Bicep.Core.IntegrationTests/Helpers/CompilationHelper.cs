// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions.Execution;
using FluentAssertions;
using System;

namespace Bicep.Core.IntegrationTests
{
    public static class CompilationHelper
    {
        public static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(params (string fileName, string fileContents)[] files)
        {
            files.Select(x => x.fileName).Should().Contain("main.bicep");

            var uriDictionary = files.ToDictionary(
                x => new Uri($"file:///path/to/{x.fileName}"),
                x => x.fileContents);
            var entryUri = new Uri($"file:///path/to/main.bicep");

            return Compile(uriDictionary, entryUri);
        }

        public static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var syntaxTreeGrouping = SyntaxFactory.CreateForFiles(files, entryFileUri);
            var compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
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
            var (jsonOutput, diagnostics) = Compile(files, entryFileUri);
            using (new AssertionScope())
            {
                jsonOutput.Should().BeNull();
                diagnostics.Should().HaveDiagnostics(expectedDiagnostics);
            }
        }

        public static string AssertSuccessWithTemplateOutput(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var (jsonOutput, diagnostics) = Compile(files, entryFileUri);
            using (new AssertionScope())
            {
                jsonOutput.Should().NotBeNull();
                diagnostics.Should().BeEmpty();
            }

            return jsonOutput!;
        }
    }
}